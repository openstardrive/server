"use strict"

let InputError = require('../src/inputError.js');
let should = require('chai').should();

describe('InputError', function () {
  it('should extend error', function () {
    let error = new InputError();
    var isError = error instanceof Error;
    isError.should.be.true;
  });

  it('should carry the message', function () {
    var error = new InputError('test message');
    error.message.should.equal('test message');
  });
});
