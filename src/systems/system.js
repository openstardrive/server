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

  setCurrentPower(newValue) {
    if (typeof newValue !== "number" || newValue < 0) return;
    this.power.current = Math.round(newValue);
  }

  setRequiredPower(newValue) {
    if (typeof newValue !== "number" || newValue < 0) return;
    this.power.required = Math.round(newValue);
  }

  setEventBus(emitter) {
    this.eventBus = emitter;
  }
}

module.exports = System;
