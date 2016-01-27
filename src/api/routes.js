var shipSystems = require('../shipSystems.js');

exports.registerRoutes = function (app, config) {
  app.get('/api', function (request, response) {
    response.send({name: 'OpenStarDrive server', version: config.serverVersion});
  });

  app.get('/api/state', function (request, response) {
    response.send({
      systems: shipSystems.getState()
    });
  });

  app.get('/api/systems', function (request, response) {
    response.send(shipSystems.getList());
  });

  app.get('/api/systems/:id/state', function (request, response) {
    var system = shipSystems.getSystemById(request.params.id);
    if (system) {
      response.send(system.getState());
      return;
    }
    response.status(404).send({error: 'system ' + request.params.id + ' does not exist'});
  });

  require('./engineRoutes.js').registerRoutes(app, config);
  require('./thrusterRoutes.js').registerRoutes(app, config);
};
