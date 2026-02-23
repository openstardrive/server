# OpenStardrive Server: Detailed Capabilities and Educational Simulation Critique

---

## Overview

OpenStardrive is a prototype event-sourced simulation server written in ASP.NET Core (.NET). Its
purpose is to manage the state of a starship simulation, expose that state to many connected clients
via a REST HTTP API, and process commands that mutate that state. It was built with a test-driven
approach, uses SQLite for persistence, and is designed to run as a single local process rather than
behind a load balancer. Immutable records and functional programming patterns (particularly the
"maybe monad") are used heavily throughout, making the codebase easy to reason about and test.

The system is explicitly a prototype. Its goal was to validate an event-sourced architecture before
committing to it as the backbone for a broader simulation platform.

---

## Architecture

### Event Sourcing

The core architectural choice is event sourcing via two append-only SQLite tables:

1. A **command log** where every incoming command is immediately written before processing.
2. A **command result log** where every meaningful state change is written after processing.

When the server starts or restarts, the command result log is dropped and rebuilt by replaying all
commands through the current business logic. This means the simulation state is always consistent
with the code, and bugs can be fixed without losing the record of what happened. Each flight session
is a separate database file, making it easy to archive sessions for review or debugging.

### Command Processor

A background service continually drains the command log, passing each command to every registered
system that handles that command type. Each system returns a `CommandResult` that is one of:

- `state-updated`: the system state changed, and the new state is included in the result payload.
- `no-change`: the command was processed but nothing changed (not written to the result log).
- `error`: the command was rejected (bad payload, insufficient power, damage, etc.).
- `unrecognized`: no registered system handles this command type.

This model means that a single command can produce multiple `state-updated` results, one per
affected system, which is useful when one system's state depends on another.

### Chronometer

A background hosted service fires a `chronometer` command roughly once per second. Systems can
subscribe to this command to drive time-based behavior: engine heat buildup and cooldown, sensor
contact movement along waypoints, power reactor drift, and navigation travel all update via the
chronometer rather than on a wall-clock timer. This keeps all time-dependent logic inside the
normal command/result pipeline and therefore inside the event log.

### API

The server exposes three HTTP endpoints:

- `POST /register` -- registers a client and returns a `clientId` and `clientSecret`.
- `POST /command` -- submits a command (requires `clientSecret`).
- `GET /results?cursor={cursor}` -- polls for new command results since the last cursor position.

The polling model is straightforward and makes it easy to build clients in any language or
framework. However it is polling-only; there is no WebSocket or server-sent-events channel.

### Client Identity

Clients register with the server and receive a unique `clientId`. This ID can be used to associate
a client with a specific station or role. The `ClientsSystem` tracks which clients are connected,
what screen each is showing, whether a client is disabled, and who is operating it. This forms a
lightweight presence and station-assignment mechanism.

---

## Ship Systems

Every ship system implements the `ISystem` interface and is auto-registered at startup. Most
systems extend `StandardSystemBaseState`, which provides four universal properties: `currentPower`,
`requiredPower`, `disabled`, and `damaged`. The `StandardTransforms` class provides shared command
handlers for setting these properties, so every powered system automatically understands the
`set-power`, `set-required-power`, `set-damaged`, and `set-disabled` commands.

### Alert System

Tracks a current alert level (Red, Yellow, Green by default) and the full list of configured
levels. Levels are configurable at runtime and carry a name and a hex color code. The system
supports `set-alert-level` and `configure-alert-levels` commands. This gives a flight director an
immediate way to change the overall tone of the simulation.

### Propulsion -- Sublight Engines

Tracks current speed (integer, bounded by max speed), a heat model, and per-speed power
requirements. When the chronometer fires, heat rises or falls depending on current speed relative
to cruising speed and the configured heat parameters. Heat is bounded by a maximum, and the engine
accepts a set-speed command that is blocked when power is insufficient, the system is disabled, or
it is damaged. Speed configurations (max speed, cruising speed) and heat configurations
(poweredHeat, cruisingHeat, maxHeat, minutesAtMaxSpeed, minutesToCoolDown) are all configurable at
runtime.

### Propulsion -- Thrusters

Tracks orientation as attitude (yaw, pitch, roll in integer degrees) and velocity (X, Y, Z as
integers). These values are accepted from clients and stored; the system does not itself animate
the ship's position. The attitude and velocity values are intended for use by display clients such
as a viewscreen renderer to show ship orientation.

### Navigation

Models the lifecycle of a navigation solution: a client requests a course calculation for a named
destination, the server (or a flight-director client) provides a calculated course with 3D
coordinates and an ETA broken down by engine speed, the crew accepts it by setting the course, and
the chronometer advances travel along the course. The current course, all pending requested
calculations, and all calculated-but-not-yet-accepted courses are in state. ETAs link to the
sublight engines system via the `SystemsRegistry` so travel times reflect the actual configured
engine speeds.

### Sensors

Supports two modes of sensor activity:

- **Active scans**: a client initiates a scan with a target description, producing an in-flight scan
  record. A flight director or automation can then call `set-sensor-scan-result` to provide a text
  answer that completes the scan. This is the mechanism by which scripted narrative can be delivered
  in response to crew actions.

- **Passive scans**: a command that provides information about what the passive sensors are picking up.

- **Sensor contacts**: named objects with 3D positions that can be added, removed, and updated. Each
  contact can carry a list of waypoint destinations with remaining travel times, and the chronometer
  moves contacts along their waypoints automatically. Contacts carry an `icon` field to identify
  what kind of object they are (ship, anomaly, planet, etc.).

### Power

Tracks a reactor output value that drifts around a configured target value at each chronometer
tick, with a configurable drift amount and update rate. Supports a battery system with a
configurable number of batteries, max charge, and independent damage state per battery. Power is
the cross-cutting concern for nearly every other system: any system extending
`StandardSystemBaseState` is blocked from acting when `currentPower` is below `requiredPower`.

### Shields

Tracks a raised/lowered state, a modulation frequency, and four independent section strengths
(forward, aft, port, starboard) expressed as percentages. Supports `raise-shields`,
`lower-shields`, `set-shield-modulation`, and `set-shield-strength` commands. Section strengths
allow for directional damage modeling.

### Energy Beam (Weapons)

Tracks multiple named beam banks, each with a charge percentage, frequency, and arc angle. Banks
must be charged before firing. The `fire-energy-beam` command fires a bank at a named target,
records the discharge and a timestamp, and resets the charge. The last fired beam is preserved in
state so display clients can trigger visual effects. Banks can be configured individually or all at
once.

### Warhead Launcher

Tracks one or more launchers, a typed inventory of warheads (e.g., torpedoes), and a loaded-tube
list. Warheads must be loaded before they can be fired. Firing records the warhead type, target,
and timestamp. Inventory counts decrease as warheads are fired.

### Short-Range Communications

Models a radio-like system with a current frequency, a set of configurable named frequency ranges,
a list of active signals (each with a frequency and name), and a broadcasting flag. A crew member
tunes the frequency and either finds a matching signal (provided by the flight director via
`set-active-signals`) or transmits into the void. This supports frequency-scanning puzzles where
the correct channel must be found.

### Long-Range Communications

A richer communications system with an encrypted message mechanic. Messages (inbound or outbound)
carry a sender, recipient, body, and a reference to a cypher. Cyphers define character substitution
rules for encoding and decoding, and carry a `percentDecoded` value that can be updated to reflect
crew decryption progress. This allows for multi-step puzzle scenarios where the crew must decode a
distress signal or intercepted transmission.

### Life Support

Exists as a registered system with the standard power/damage/disabled properties. No additional
simulation logic is currently implemented. It is essentially a placeholder.

### Teams

Models away teams and internal response teams (damage control, security, medical). Each team has a
type, a priority, a set of assigned officers with inventory, a current location expressed as a
named deck, and a text-based orders field. Commands allow creating, moving, and updating teams.
This is the mechanism for managing crew roles that are not seated at bridge stations.

### Damage Teams

A stub state that extends `StandardSystemBaseState` with no additional fields. Teams-based damage
management is partially modeled in the Teams system but this system has not been developed further.

### Clients System

Tracks every registered client with its current screen name, whether it is disabled, and an
operator field which can name who is sitting at that station. Flight director tools can use this to
see which stations are active and what they are showing.

### JSON Plugin System

A flexible extensibility mechanism. JSON files placed in the `Plugins/` folder are loaded at
startup and each becomes a distinct named system. The plugin file declares fields, and the server
automatically generates commands of the form `update-{pluginName}-{fieldName}` for each field.
This allows new simulation systems to be prototyped in JSON without writing C# code. The included
`viewscreen.json` defines a system with a `CurrentImage` field and a `Cards` list, enabling a
flight director to change what the viewscreen is showing.

### Debug System

A development-time system that can accept debug commands. Not intended for production use.

### Thorium Integration

The codebase contains an integration module that can translate simulation commands into API calls
against Thorium, a separate established spaceship simulator. This integration targets Thorium's
engine speed and team management. It is partial and appears to be exploratory, but it demonstrates
that the architecture supports bridging to external systems.

---

## Cross-Cutting Infrastructure

### Maybe Monad and Immutable Records

The codebase makes heavy use of C# record types for all state and payload objects, which are
immutable by design. The `Maybe<T>` type (a simple discriminated union of "some value" or "none")
is used throughout transforms to chain validation checks without nested conditionals. This makes
individual transform functions easy to read and unit test. The `IfFunctional` method on the base
state consolidates all three standard checks (disabled, damaged, insufficient power) into a single
guard that every system can call before allowing a state change.

### Systems Registry

A singleton registry holds references to all active system instances. Systems that need data from
other systems (such as Navigation needing engine speed for ETA calculations) query the registry
rather than having direct dependencies. This keeps system coupling loose.

### Auto-Registration

Any class that implements `ISystem` is automatically discovered and registered at startup via
dependency injection scanning. This makes adding a new system as simple as creating the class;
no manual wiring is required.

### Test Coverage

The repository contains both a unit test project (`OpenStardriveServer.UnitTests`) and an
integration test project (`OpenStardriveServer.IntegrationTests`). The unit tests use an
automocking helper that minimizes boilerplate, and the integration tests test command processing
end-to-end. This is a noteworthy asset for a prototype, providing a safety net for future
development.

---

## Critique: Building an Educational Starship Simulation

The following is an analysis of what is present, what is absent, and what needs significant
development to support a human-directed starship simulation for 5th-8th grade students, guided by
a flight director, with emphasis on approachable graphics, timeline-based branching scenarios, and
layered complexity.

---

### 1. The Core Architecture Is a Strong Foundation

The event-sourcing approach is well-suited to this application. Commands and results form a
complete audit trail of what happened in a mission, which is useful for post-mission debriefs.
The cursor-based results polling is simple enough that a crew station built in plain HTML and
JavaScript (as the dev-client demonstrates) can integrate without any specialist framework
knowledge. The fact that the server rebuilds state from the command log on restart means a crash
during a mission does not necessarily lose the simulation state. The immutable record pattern makes
the codebase predictable, and the test coverage reduces regression risk when features are added.

The single-process, local-only design is appropriate for the physical setup of a school or
community organization running a simulation on a local network.

---

### 2. Scenario and Timeline Engine -- Entirely Absent

This is the most significant gap. There is no concept of a mission scenario, a timeline, a
scripted event, a branch point, or a trigger condition anywhere in the codebase.

For the target use case, the flight director needs to be able to:

- Load a named mission that defines an initial state, a narrative arc, and a set of possible events.
- Trigger events (a new sensor contact appears, a distress signal arrives, the reactor takes damage)
  either on a schedule or in response to what the crew does.
- Jump between narrative branches depending on crew decisions (did the crew hail the alien ship or
  open fire?).
- Queue events for future execution ("in 90 seconds, trigger the asteroid field encounter") while
  retaining the ability to skip ahead, skip back, or divert to an alternate branch.

Today, none of this exists. The server processes commands faithfully but has no opinion about story
progression. A flight director operating the current system would need to manually issue every
command in the right sequence with no tooling support, making it impractical for a real mission. A
scenario engine, likely built as a combination of a JSON or YAML mission definition format and a
flight-director command client, needs to be designed and implemented from scratch.

A branching scenario system is considerably more complex than a linear one. It needs to represent
the mission as a directed graph of states, track which branch the crew is currently in, allow the
flight director to override the current branch mid-mission, and handle the possibility that the same
crew action means different things depending on prior context.

---

### 3. Flight Director Tools -- Entirely Absent

The server has no flight director interface. There is a rudimentary dev-client HTML page, but it
is a development tool rather than a mission-control surface. A flight director running a live
mission with students needs:

- A real-time dashboard showing the state of every system and every connected crew station.
- A way to see which students are at which stations.
- One-click access to triggering common mission events (alert change, incoming transmission,
  hostile contact appearing, system damage, reactor fluctuation).
- A view of the current position in the mission timeline with easy navigation to alternate branches.
- A communication panel for injecting FD-voiced or text-based communications to crew stations.
- Controls for adjusting difficulty on the fly (boosting or reducing shield strength, adding hull
  damage, changing power levels) without breaking the narrative immersion.

None of this tooling exists. Building it is a substantial project in its own right and should be
treated as a first-class deliverable alongside the scenario engine.

---

### 4. Graphics and Viewscreen -- Minimal

The current viewscreen support consists of a JSON plugin with a single `CurrentImage` field and a
hard-coded list of five image card names. There is no renderer, no animation system, no transition
effects, and no way to overlay dynamic data on top of imagery.

For a compelling middle-school audience, the main viewscreen is the most important visual element
of the simulation. It needs to show:

- Space imagery appropriate to the current location that changes meaningfully as the ship travels.
- Visual representation of sensor contacts and their relative positions.
- Weapon fire effects when the crew fires.
- Damage effects during incoming attacks.
- Dynamic overlays (signal intercept text, planet approach graphics, warp tunnel transition).

Building this properly requires a dedicated viewscreen client -- likely a browser-based canvas or
WebGL application -- that interprets simulation state and renders it as immersive imagery. The
server already has the state (sensor contacts with positions, alert levels, course and speed, last
fired weapon data) to drive most of this; what is needed is the rendering layer.

Crew station interfaces also need graphical polish. The existing dev-client renders raw JSON in a
formatted list, which is fine for development but completely unsuitable for a student at a science
station who has never used the system before. Each station needs a purpose-built interface designed
to be discovered intuitively, with clear visual feedback for every action.

---

### 5. Real-Time Communication Infrastructure -- Limitations

The polling model works, but it creates a meaningful latency floor. A client polling once per
second faces up to approximately one second of delay between when a command result is written and
when the client sees it. In a live mission, this can break the sense of immediacy during fast-paced
scenarios (an alarm should sound the moment the alert is raised, not up to a second later).

The documentation acknowledges a future socket-based API, and the command-result structure is
well-positioned to support WebSockets or server-sent events because it is essentially an event
stream. Implementing a WebSocket channel that pushes new command results to subscribers would
dramatically improve the live-simulation experience and is a relatively contained enhancement to
the server. This should be prioritized before building production station UIs, because the polling
latency would otherwise cause frustration for students and flight directors alike.

---

### 6. Audio -- Entirely Absent

There is no audio system. For a student audience, sound is a large part of immersion. Alert klaxons,
engine hum, weapon fire, incoming transmission chimes, and proximity warnings are all expected
experiences in a spaceship simulation. Audio cues also help students understand what is happening
without needing to watch a display all the time.

An audio system would most likely live entirely in the clients rather than the server (browsers can
play sounds natively), but the server needs to emit the right events at the right time. Most of the
needed trigger points already exist in state: alert level changes, last-fired-weapon updates, sensor
contact appearances, and power fluctuations are all captured in command results. The audio layer
needs to be wired to these events in each client that should produce sound.

A centralized "sound cue" system controlled by the flight director (and also automatically triggered
by server events) would complement this and allow for ambient background tracks and scripted
sound effects during key story moments.

---

### 7. Crew Station Role Design -- Incomplete

The current system has enough mechanics to support most standard bridge roles, but they are not
mapped out or formalized for the target audience.

- **Helm / Pilot**: Thrusters (attitude and velocity) and engines (speed) exist, but thrusters
  expose raw yaw/pitch/roll integers that have no clear meaning to a student who has not been
  trained on coordinate systems. A helm interface needs to abstract this into intuitive direction
  controls with visual confirmation of ship orientation.

- **Science / Sensors**: The sensor scan request/result model is well-designed. A science officer
  can initiate a scan and the flight director can provide a text result. However there is no
  structured scan-result format or template system to guide what kinds of answers are appropriate
  for 5th-8th grade comprehension. Without FD tools that suggest or auto-populate scan results, the
  flight director bears a heavy creative burden.

- **Engineering**: The power system (reactor drift, battery management) and the per-system
  power/damage flags are good raw material. A student engineer should be able to manage power
  allocation across systems and repair damage. The current model supports this conceptually —
  setting `set-required-power` and `set-power` per system — but there is no aggregate power
  budget enforcement or graphical power routing diagram.

- **Communications**: Both short-range and long-range comms systems are reasonably well-developed.
  The frequency-tuning mechanic and the cypher/decode mechanic are engaging puzzle elements for
  this age group. The gap is in the FD side: there is no pre-authored message library, no quick way
  to send a scripted transmission, and no log of displayed messages in the current client.

- **Weapons / Tactical**: Shields, energy beams, and warhead launchers are all present and
  mechanically complete. The tactical officer can raise shields, modulate frequency, charge and fire
  beams, and launch warheads. The gap is in feedback: the server records the last fired event, but
  without a viewscreen effect or audio cue, the crew has no visceral sense that something happened.

---

### 8. Life Support and Damage Control -- Largely Stubs

Life support exists only as a system with the standard power/damage/disabled flags and no
simulation logic. For a student audience, life support is a high-drama system: oxygen levels
dropping, a hull breach affecting a section, CO2 scrubber failure. None of this is modeled.

The damage teams system is similarly skeletal. The Teams system has enough structure to represent
teams dispatched to fix things, but there is no linkage between a damaged system flag, a repair
process (time to fix, resources consumed), and the clearing of the damage flag. A student damage
control officer should be able to see which systems are damaged, dispatch a team, watch the repair
timer, and then see the system come back online. This entire cycle needs to be designed and
implemented.

Hull integrity is also absent. There is no concept of the ship itself taking damage beyond
per-system damage flags. For the age group, total ship health as a simple percentage or by deck
section is an important tension-building mechanic.

---

### 9. Onboarding and Approachability -- Not Addressed

The codebase has no student-facing onboarding mechanisms. For 5th-8th graders with no prior
experience, the first few minutes at a station are critical. The current dev-client displays a
raw JSON dump of server state, which would be completely overwhelming to a new student.

Each station interface needs:

- A clear indication of what the student's role is and what they are responsible for.
- Visible, labeled controls with minimal cognitive overhead for the core actions.
- Contextual help or tooltips that explain what a control does without requiring external documentation.
- Visual and audio confirmation that an action was received and processed.
- Graceful error handling: if a student tries to fire the weapons with shields down or engines off, the
  interface should explain why it did not work in plain language, not show a raw error string.

A "beginner mode" for each station -- where the most advanced or confusing settings are hidden or
preset -- would allow students to participate meaningfully from their first mission. More experienced
students can unlock or explore the additional depth available in the underlying systems.

---

### 10. Role-Based Access and Security -- Minimal

Any client that registers can issue any command. There is no enforcement that only the helm station
can change engine speed, or that only the flight director can add sensor contacts. The `clientId`
tracking exists, but no access control is applied to commands.

For a live mission, this is a real operational concern: a curious student at the science station
should not be able to accidentally (or deliberately) manipulate another station's systems. The
`clientSecret` mechanism provides a baseline, but role-based command permissions need to be
layered on top of it. The architecture supports this addition cleanly because command processing
is centralized, but the implementation does not yet exist.

---

### 11. Missing Systems for a Complete Bridge Experience

Several systems that students would reasonably expect on a starship bridge do not exist:

- **Viewscreen control**: beyond the image-switching JSON plugin, there is no structured way to
  display mission-relevant media (video clips, planetary approach sequences, alien transmission feeds).
- **Galaxy map / star chart**: there is no system representing the broader space environment, star
  systems, jump points, or territorial boundaries. Navigation coordinates exist but there is no map
  for students to look at.
- **Medical / sickbay**: no system models crew injury, medical readiness, or the medical officer
  role at all.
- **Transporter**: absent entirely.
- **Internal ship status**: no model of the ship's interior (decks, sections, room states) beyond
  the deck/location data in the Teams system.
- **Mission clock / elapsed time**: the chronometer fires once per second but there is no system
  that tracks mission elapsed time, countdown timers, or mission objectives with due dates.

---

### 12. Content Management -- No Tooling

A flight director should be able to create and edit missions without writing code. There is no
mission authoring tool, no content management interface, no asset library for images and audio, and
no way to test a mission script in isolation. This tooling is not strictly part of the server but
it is essential infrastructure for the program. Without it, creating new missions requires
developer-level involvement every time, which is not sustainable for an educational program that
wants to run new scenarios regularly.

---

### 13. What Is Already Solid

Several things in the existing codebase are genuinely good and should be preserved:

- The command/result event architecture is sound and scales well for this use case.
- The immutable state and functional transforms make the simulation logic easy to test and extend.
- The chronometer-driven time model is elegant and keeps time-based behavior deterministic.
- The sensor contact system (3D positions, named icons, waypoint movement) is a strong foundation
  for a tactical map.
- The long-range comms cypher system is a creative, age-appropriate puzzle mechanic that is already
  implemented.
- The JSON plugin system is a valuable rapid prototyping tool that will let a team add new systems
  quickly before committing to full C# implementations.
- The Thorium integration suggests the team is already aware of the broader ecosystem of spaceship
  simulation software and could draw on ideas and assets from it.
- The test coverage is an asset that will protect against regressions as the codebase grows.

---

## Summary

OpenStardrive server is a well-engineered prototype simulation backend. It cleanly models the core
ship systems a spaceship crew would operate, tracks state reliably through an event-sourced SQLite
pipeline, and exposes a simple API that makes building client interfaces straightforward. The
engineering quality and test coverage are strong for a prototype.

However, as a foundation for a fully realized educational starship simulation for guided student
missions, it is an early-stage core engine rather than a near-complete product. The most critical
missing pieces -- scenario and timeline management, flight director tooling, immersive crew station
interfaces, a viewscreen graphics layer, audio, real-time transport, approachable onboarding, and
role-based access -- are all significant parallel workstreams that need dedicated design and
implementation effort. None of these are small features; each represents a substantial body of work
that must be designed thoughtfully for the 5th-8th grade audience.

The right framing is that this codebase provides a solid and trustworthy server-side state machine.
Everything built on top of it -- missions, interfaces, audio, visuals, FD tools -- still needs to
be created. The architectural choices made so far do not block any of that work and in several cases
actively support it.
