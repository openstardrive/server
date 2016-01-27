"use strict";

class System {
  constructor(data) {
    data = data || {};
    this.id = data.id;
    this.name = data.name;
    this.power = {
      current: 0,
      required: 0
    }
  }

  getState() {
    return {
      id: this.id,
      name: this.name,
      power: this.power
    };
  }

  hasEnoughPower() {
    return this.power.current >= this.power.required;
  }
}

module.exports = System;
