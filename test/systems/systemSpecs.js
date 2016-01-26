require('chai').should();
var System = require('../../src/systems/system.js');


describe('system', function () {
  var system;
  beforeEach(function () {
    system = new System();
  });


  describe('initial state', function () {
    it('has power levels set to zero', function () {
        system.requiredPower.should.equal(0);
        system.currentPower.should.equal(0);
    });
  });

  describe('hasEnoughPower', function () {
    it('returns true if there is enough power', function () {
      system.requiredPower = 1;
      system.currentPower = 1;
      system.hasEnoughPower().should.equal(true);
    });
    it('returns false if there is not enough power', function () {
      system.requiredPower = 2;
      system.currentPower = 1;
      system.hasEnoughPower().should.equal(false);
    });
  });
});
