var InputError = require('../inputError.js');

module.exports = function (error, request, response, next) {
  if (error instanceof InputError) {
    response.status(400).send({error: error.message});
  } else {
    next(error);
  }
};
