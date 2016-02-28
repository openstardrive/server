var shipSystems = require('../shipSystems.js');
var sensors = shipSystems.getSystemById('sensors');

exports.registerRoutes = function (app, config) {
  app.post('/api/systems/sensors/contacts', function (request, response) {
    var contact = sensors.addContact(request.body);
    response.send(contact);
  });

  app.put('/api/systems/sensors/contacts/:id', function (request, response) {
    if (request.params.id != request.body.id) {
      return response.status(400).send({error: 'Contact id mismatch.'});
    }
    var contact = sensors.updateContact(request.body);
    response.send(contact);
  });

  app.delete('/api/systems/sensors/contacts/:id', function (request, response) {
    var contact = sensors.removeContact(request.params.id);
    response.send(contact);
  });
};
