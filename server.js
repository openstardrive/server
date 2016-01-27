var serverVersion = require('./package.json').version;
var express = require('express');
var bodyParser = require('body-parser');
var apiRoutes = require('./src/api/routes.js');

var app = express();
app.use(bodyParser.json())

apiRoutes.registerRoutes(app, {serverVersion: serverVersion});

app.listen(3000, function () {
  console.log('OpenStarDrive server (v' + serverVersion + ') listening on port 3000!');
  logRoutes();
});

function logRoutes() {
  console.log('Routes:')
  app._router.stack.forEach(function (item) {
    if (item.route && item.route.path) {
      var methods = Object.keys(item.route.methods);
      methods.forEach(function (method) {
        console.log(method.toUpperCase(), item.route.path);
      });
    }
  });
}
