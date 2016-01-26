require('chai').should();
var System = require('../../src/systems/system.js');


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
        system.requiredPower.should.equal(0);
        system.currentPower.should.equal(0);
    });
  });

  describe('getState', function () {
    it('should return the current state of the system', function () {
      system.requiredPower = 1;
      system.currentPower = 2;

      var state = system.getState();
      state.should.deep.equal({
        id: 'test-system',
        name: 'Test System',
        currentPower: 2,
        requiredPower: 1
      });
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
