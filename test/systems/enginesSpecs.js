require('chai').should();
var Engines = require('../../src/systems/engines.js');


describe('engines', function () {
  var engines;

  beforeEach(function () {
    engines = new Engines({id: 'test-engines', name: 'Test Engines'});
  });

  describe('initial state', function () {
    it('has power levels set to zero', function () {
      engines.power.required.should.equal(0);
      engines.power.current.should.equal(0);
    });

    it('has default speeds', function () {
      engines.speed.max.should.equal(10);
      engines.speed.current.should.equal(0);
    });

    it('has default heat levels', function () {
      engines.heat.min.should.equal(0);
      engines.heat.max.should.equal(100);
      engines.heat.current.should.equal(10);
    });
  });

  describe('getState', function () {
    it('should return the current state of the system', function () {
      engines.power.required = 1;
      engines.power.current = 2;

      var state = engines.getState();
      state.should.deep.equal({
        id: 'test-engines',
        name: 'Test Engines',
        power: { current: 2, required: 1 },
        speed: { max: 10, current: 0 },
        heat: { min: 0, max: 100, current: 10 }
      });
    });
  });

  describe('setCurrentSpeed', function () {
    it('should set the current speed value', function () {
      engines.setCurrentSpeed(3);
      engines.speed.current.should.equal(3);
    });

    it('should max out at the maximum speed', function () {
      engines.setCurrentSpeed(11);
      engines.speed.current.should.equal(10);
    });

    it('should max out at the maximum speed', function () {
      engines.setCurrentSpeed(11);
      engines.speed.current.should.equal(10);
    });

    it('should treat negative numbers as zero', function () {
      engines.setCurrentSpeed(-1);
      engines.speed.current.should.equal(0);
    });

    it('should round to integers', function () {
      engines.setCurrentSpeed(1.21);
      engines.speed.current.should.equal(1);
    });

    it('should ignore non-numbers', function () {
      engines.setCurrentSpeed(5);
      engines.setCurrentSpeed("wat")
      engines.speed.current.should.equal(5);
    });

    it('should do nothing if there is not enough power', function () {
      engines.setCurrentSpeed(2);
      engines.power.required = 10;
      engines.setCurrentSpeed(3)
      engines.speed.current.should.equal(2);
    });
  });
});
