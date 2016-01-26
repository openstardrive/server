"use strict";

class System {
  constructor() {
    this.requiredPower = 0;
    this.currentPower = 0;
  }

  hasEnoughPower() {
    return this.currentPower >= this.requiredPower;
  }
}

module.exports = System;
