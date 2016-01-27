var shipSystems = require('../shipSystems.js');
var ftl = shipSystems.getSystemById('ftl-engines');
var sublight = shipSystems.getSystemById('sublight-engines');

exports.registerRoutes = function (app, config) {
  app.post('/api/systems/ftl-engines/speed', function (request, response) {
    ftl.setCurrentSpeed(request.body.value);
    response.end();
  });

  app.post('/api/systems/sublight-engines/speed', function (request, response) {
    sublight.setCurrentSpeed(request.body.value);
    response.end();
  });
};
