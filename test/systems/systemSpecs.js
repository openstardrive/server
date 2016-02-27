var should = require('chai').should();
var System = require('../../src/systems/system.js');
var InputError = require('../../src/inputError.js');


describe('system', function () {
  var system;
  beforeEach(function () {
    system = new System({id: "test-system", name: "Test System"});
  });


  describe('initial state', function () {
    it('has set the name of the system', function () {
      system.id.should.equal("test-system");
      system.name.should.equal("Test System");
    });

    it('has power levels set to zero', function () {
        system.power.required.should.equal(0);
        system.power.current.should.equal(0);
    });
  });

  describe('getState', function () {
    it('should return the current state of the system', function () {
      system.setRequiredPower(1);
      system.setCurrentPower(2);

      var state = system.getState();
      state.should.deep.equal({
        id: 'test-system',
        name: 'Test System',
        power: {
          current: 2,
          required: 1
        }
      });
    });
  });

  describe('hasEnoughPower', function () {
    it('returns true if there is enough power', function () {
      system.setRequiredPower(1);
      system.setCurrentPower(1);
      system.hasEnoughPower().should.equal(true);
    });
    it('returns false if there is not enough power', function () {
      system.setRequiredPower(2);
      system.setCurrentPower(1);
      system.hasEnoughPower().should.equal(false);
    });
  });

  describe('setCurrentPower', function () {
    it('sets the current power level', function () {
      system.setCurrentPower(1);
      system.power.current.should.equal(1);
    });
    it('rounds the value', function () {
      system.setCurrentPower(2.78);
      system.power.current.should.equal(3);
    });
    it('throws on non-numbers', function () {
      should.throw(function () { system.setCurrentPower("x"); }, InputError, 'Invalid power value.');
      system.power.current.should.equal(0);
    });
    it('treats negative numbers as zero', function () {
      system.setCurrentPower(2);
      system.setCurrentPower(-1);
      system.power.current.should.equal(0);
    });
  });

  describe('setRequiredPower', function () {
    it('sets the required power level', function () {
      system.setRequiredPower(1);
      system.power.required.should.equal(1);
    });
    it('rounds the value', function () {
      system.setRequiredPower(2.78);
      system.power.required.should.equal(3);
    });
    it('throws on non-numbers', function () {
      should.throw(function () { system.setRequiredPower("x"); }, InputError, 'Invalid power value.');
      system.power.required.should.equal(0);
    });
    it('treats negative numbers as zero', function () {
      system.setRequiredPower(2);
      system.setRequiredPower(-1);
      system.power.required.should.equal(0);
    });
  });
});
