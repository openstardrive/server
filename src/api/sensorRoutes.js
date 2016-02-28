var shipSystems = require('../shipSystems.js');
var sensors = shipSystems.getSystemById('sensors');

exports.registerRoutes = function (app, config) {
  app.post('/api/systems/sensors/contacts', function (request, response) {
    var contact = sensors.addContact(request.body);
    response.send(contact);
  });
};
