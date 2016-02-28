var Engines = require('./systems/engines.js');
var Thrusters = require('./systems/thrusters.js');
var Sensors = require('./systems/sensors.js');
var eventBus = require('./eventBus.js');

var systems = [
  new Engines({id: "ftl-engines", name: "FTL Engines"}),
  new Engines({id: "sublight-engines", name: "Sublight Engines"}),
  new Thrusters(),
  new Sensors()
];
systems.forEach(function (system) {
  system.setEventBus(eventBus);
});

exports.getSystemById = function (id) {
  return systems.find(function (element) {
    return element.id === id;
  });
};

exports.getState = function () {
  var state = [];
  systems.forEach(function (system) {
    state.push(system.getState());
  });
  return state;
};

exports.getList = function () {
  var list = [];
  systems.forEach(function (system) {
    list.push({id: system.id, name: system.name});
  });
  return list;
};
