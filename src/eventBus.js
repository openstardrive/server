const EventEmitter = require('events');

var bus = new EventEmitter();

module.exports = bus;

setInterval(emitPulse, 500);

var lastPulse = Date.now();
function emitPulse() {
  var now = Date.now();
  var diff = now - lastPulse;
  bus.emit('pulse', diff, now, lastPulse);
  lastPulse = now;
}
