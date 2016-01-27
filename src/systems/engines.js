"use strict";

var System = require('./system.js');

const millisecondsPerMinute = 60000;
const millisecondsPerSecond = 1000;

class Engines extends System {
  constructor(data) {
    super(data);
    this.speed = { max: 10, current: 0, cruising: 6 };
    this.heat = {
      current: 10, target: 10, delta: 0, secondsUntilOverheat: null,
      max: 100, powered: 10, cruising: 50,
      minutesAtMaxSpeed: 5, minutesToCoolDown: 10
    }
  }

  getState() {
    var state = super.getState();
    state.speed = this.speed;
    state.heat = {
      current: Math.round(this.heat.current),
      max: this.heat.max,
      powered: this.heat.powered,
      cruising: this.heat.cruising
    };
    return state;
  }

  setCurrentSpeed(newSpeed) {
    if (!super.hasEnoughPower()) {
      return;
    }
    if (typeof newSpeed === "number") {
      this.speed.current = Math.round(Math.max(Math.min(newSpeed, this.speed.max), 0));
      this.calculateHeatValuesForCurrentSpeed();
    }
  }

  calculateHeatValuesForCurrentSpeed() {
    this.calculateTargetHeat();
    this.calculateHeatDelta();
    this.calculateSecondsUntilOverheat();
  }

  calculateTargetHeat() {
    if (this.speed.current > 0) {
      if (this.speed.current > this.speed.cruising) {
        this.heat.target = this.heat.max
      } else {
        var steps = (this.heat.cruising - this.heat.powered) / Math.max(this.speed.cruising, 1);
        this.heat.target = this.heat.powered + (steps * this.speed.current);
      }
    } else {
      if (super.hasEnoughPower()) {
        this.heat.target =  this.heat.powered;
      } else {
        this.heat.target = 0;
      }
    }
  }

  calculateHeatDelta() {
    if (this.heat.target > this.heat.current) {
      this.calculateHeatDeltaForCurrentSpeed();
    } else {
      this.calculateHeatDeltaForCooldown();
    }
  }

  calculateHeatDeltaForCurrentSpeed() {
    var currentSpeed = Math.max(this.speed.current, 1);
    var minutesAtCurrentSpeed = this.heat.minutesAtMaxSpeed * (this.speed.max / currentSpeed);
    var heatPerMinute = (this.heat.max - this.heat.powered) / minutesAtCurrentSpeed;
    this.heat.delta = heatPerMinute / millisecondsPerMinute;
  }

  calculateHeatDeltaForCooldown() {
    var coolingPerMinute = this.heat.max / this.heat.minutesToCoolDown;
    this.heat.delta = -1 * coolingPerMinute / millisecondsPerMinute;
  }

  calculateSecondsUntilOverheat() {
    var remainingHeat = this.heat.max - this.heat.current;
    if (this.heat.target == this.heat.max && this.heat.delta > 0 && remainingHeat > 0) {
      var millisecondsUntilOverheat = remainingHeat / this.heat.delta;
      this.heat.secondsUntilOverheat = Math.floor(millisecondsUntilOverheat / millisecondsPerSecond);
      return;
    }
    this.heat.secondsUntilOverheat = null;
  }

  setEventBus(emitter) {
    super.setEventBus(emitter);
    var self = this;
    this.eventBus.on('pulse', function (millisecondsSinceLastPulse) {
      self.onPulse(millisecondsSinceLastPulse);
    });
  }

  onPulse(millisecondsSinceLastPulse) {
    var newHeat = this.heat.current + (this.heat.delta * millisecondsSinceLastPulse);
    if (this.heat.delta < 0 && newHeat < this.heat.target) {
      newHeat = this.heat.target;
    }
    if (this.heat.delta > 0 && newHeat > this.heat.target) {
      newHeat = this.heat.target;
    }
    this.heat.current = Math.max(Math.min(newHeat, this.heat.max), 0);
  }
}

module.exports = Engines;
