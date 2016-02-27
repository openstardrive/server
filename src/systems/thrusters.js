"use strict";

var System = require('./system.js');
var InputError = require('../inputError.js');

class Thrusters extends System {
  constructor() {
    super({id: 'thrusters', name: 'Thrusters'});
    this.velocity = {x: 0, y:0, z:0};
    this.attitude = {yaw: 0, pitch: 0, roll: 0};
  }

  getState() {
    var state = super.getState();
    state.velocity = this.velocity;
    state.attitude = this.attitude;
    return state;
  }

  setAttitude(newAttitude) {
    if (!super.hasEnoughPower()) {
      throw new InputError('Insufficient power.');
    };

    newAttitude = newAttitude || {};

    if (!isNumber(newAttitude.yaw) || !isNumber(newAttitude.pitch) || !isNumber(newAttitude.roll)) {
      throw new InputError('Invalid attitude.');
    }

    this.attitude.yaw = adjustDegrees(newAttitude.yaw);
    this.attitude.pitch = adjustDegrees(newAttitude.pitch);
    this.attitude.roll = adjustDegrees(newAttitude.roll);
  }

  setVelocity(newVelocity) {
    if (!super.hasEnoughPower()) {
      throw new InputError('Insufficient power.')
    };

    newVelocity = newVelocity || {};

    if (!isNumber(newVelocity.x) || !isNumber(newVelocity.y) || !isNumber(newVelocity.z)) {
      throw new InputError('Invalid velocity.');
    }

    this.velocity.x = Math.round(newVelocity.x);
    this.velocity.y = Math.round(newVelocity.y);
    this.velocity.z = Math.round(newVelocity.z);
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
