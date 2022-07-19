curl http://localhost:5000/results ; echo

curl -i -X POST -H 'content-type: application/json' -d '{"name":"Curl Client"}' http://localhost:5000/register ; echo

```
{
  "clientId":"c1a8be77-5f4f-4a8e-bb57-a4171f1084c5",
  "clientSecret":"OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk="
}
```

curl -i -X POST -H 'content-type: application/json' -d '{"clientSecret": "OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk=", "type":"set-thrusters-attitude", "payload": {"yaw": 1, "pitch": 2, "roll": 3}}' http://localhost:5000/command ; echo

curl -i -X POST -H 'content-type: application/json' -d '{"clientSecret": "OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk=", "type":"set-thrusters-attitude", "payload": "turtles"}' http://localhost:5000/command ; echo

curl -i -X POST -H 'content-type: application/json' -d '{"clientSecret": "OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk=", "type":"set-thrusters-velocity", "payload": {"x": 123, "y": 234, "z": "345"}}' http://localhost:5000/command ; echo

curl -i -X POST -H 'content-type: application/json' -d '{"clientSecret": "OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk=", "type":"chronometer", "payload": {"elapsedMilliseconds": 1000}}' http://localhost:5000/command ; echo

curl -i -X POST -H 'content-type: application/json' -d '{"clientSecret": "OZPokmaTog8+k4jKVAVhJ6rVh3EEL7Ev4xvuQu4p5Rk=", "type":"set-ftl-engines-speed", "payload": {"speed": 4}}' http://localhost:5000/command ; echo




## Systems

For long range messages, put a `[JsonIgnore]` on the full list of messages received.
Then expose the latest message on the state.
That way the output states won't duplicate all the messages (lots of bandwidth by the end of a mission).


