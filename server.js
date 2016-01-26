var serverVersion = require('./package.json').version;
var express = require('express');
var apiRoutes = require('./src/api/routes.js');

var app = express();

apiRoutes.registerRoutes(app, {serverVersion: serverVersion});

app.listen(3000, function () {
  console.log('OpenStarDrive server (v' + serverVersion + ') listening on port 3000!');
});