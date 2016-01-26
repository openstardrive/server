"use strict";

class System {
  constructor(data) {
    data = data || {};
    this.id = data.id;
    this.name = data.name;
    this.requiredPower = 0;
    this.currentPower = 0;
  }

  getState() {
    return {
      id: this.id,
      name: this.name,
      requiredPower: this.requiredPower,
      currentPower: this.currentPower
    };
  }

  hasEnoughPower() {
    return this.currentPower >= this.requiredPower;
  }
}

module.exports = System;
