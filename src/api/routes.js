var shipSystems = require('../shipSystems.js');
var Clients = require('../clients.js');

var clients = new Clients();

exports.registerRoutes = function (app, config) {
  app.use(function (request, response, next) {
    if (request.headers['client-id']) {
      clients.visited(request.headers['client-id'])
    }
    next()
  })

  app.get('/api', function (request, response) {
    response.send({name: 'OpenStardrive server', version: config.serverVersion});
  });

  app.get('/api/state', function (request, response) {
    response.send({
      systems: shipSystems.getState(),
      clients: clients.getAll()
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

  app.get('/api/clients/:id', function (request, response) {
    var client = clients.get(request.params.id)
    if (client) {
      response.send(client);
      return;
    }
    response.status(404).send({error: 'client ' + request.params.id + ' does not exist'});
  });

  app.put('/api/clients/:id', function (request, response) {
    var clientData = request.body;
    clientData.id = request.params.id;
    clients.set(clientData);
    response.send();
  });

  require('./engineRoutes.js').registerRoutes(app, config);
  require('./thrusterRoutes.js').registerRoutes(app, config);
  require('./sensorRoutes.js').registerRoutes(app, config);
};
