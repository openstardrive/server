var shipSystems = require('../shipSystems.js');
var thrusters = shipSystems.getSystemById('thrusters');

exports.registerRoutes = function (app, config) {
  app.post('/api/systems/thrusters/attitude', function (request, response) {
    thrusters.setAttitude(request.body);
    response.send(thrusters.attitude);
  });
};
