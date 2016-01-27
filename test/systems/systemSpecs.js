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
        system.power.required.should.equal(0);
        system.power.current.should.equal(0);
    });
  });

  describe('getState', function () {
    it('should return the current state of the system', function () {
      system.power.required = 1;
      system.power.current = 2;

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
      system.power.required = 1;
      system.power.current = 1;
      system.hasEnoughPower().should.equal(true);
    });
    it('returns false if there is not enough power', function () {
      system.power.required = 2;
      system.power.current = 1;
      system.hasEnoughPower().should.equal(false);
    });
  });
});
