var serverVersion = require('./package.json').version;
var express = require('express');
var bodyParser = require('body-parser');
var cors = require('cors');
var apiRoutes = require('./src/api/routes.js');
var inputErrorHandler = require('./src/api/inputErrorHandler.js');

var app = express();
app.use(bodyParser.json());
app.use(cors());

apiRoutes.registerRoutes(app, {serverVersion: serverVersion});

app.use(inputErrorHandler);

app.listen(3000, function () {
  console.log('OpenStardrive server (v' + serverVersion + ') listening on port 3000!');
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
