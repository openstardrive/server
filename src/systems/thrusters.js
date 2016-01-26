"use strict";

var System = require('./system.js');

class Thrusters extends System {
  constructor() {
    super({id: 'thrusters', name: 'Thrusters'});
    this.velocity = {x: 0, y:0, z:0};
    this.attitude = {yaw: 0, pitch: 0, roll: 0};
  }

  setAttitude(newAttitude) {
    if (!super.hasEnoughPower()) return;

    newAttitude = newAttitude || {};
    if (isNumber(newAttitude.yaw)) {
      this.attitude.yaw = adjustDegrees(newAttitude.yaw);
    }
    if (isNumber(newAttitude.pitch)) {
      this.attitude.pitch = adjustDegrees(newAttitude.pitch);
    }
    if (isNumber(newAttitude.roll)) {
      this.attitude.roll = adjustDegrees(newAttitude.roll);
    }
  }

  setVelocity(newVelocity) {
    if (!super.hasEnoughPower()) return;
    
    newVelocity = newVelocity || {};
    if (isNumber(newVelocity.x)) {
      this.velocity.x = Math.round(newVelocity.x);
    }
    if (isNumber(newVelocity.y)) {
      this.velocity.y = Math.round(newVelocity.y);
    }
    if (isNumber(newVelocity.z)) {
      this.velocity.z = Math.round(newVelocity.z);
    }
  }
}

function isNumber(value) {
  return typeof value === "number";
}

function adjustDegrees(value) {
  var adjusted = Math.round(value) % 360;
  if (adjusted < 0) {
    adjusted = adjusted + 360;
  }
  return adjusted;
}

module.exports = Thrusters;
