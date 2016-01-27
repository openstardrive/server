"use strict";

var System = require('./system.js');

class Engines extends System {
  constructor(data) {
    super(data);
    this.speed = {
      max: 10,
      current: 0
    };
    this.heat = { min: 0, max: 100, current: 10 }
  }

  getState() {
    var state = super.getState();
    state.speed = this.speed;
    state.heat = this.heat;
    return state;
  }
}

module.exports = Engines;
