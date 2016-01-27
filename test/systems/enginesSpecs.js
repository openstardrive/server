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
        power: {
          current: 2,
          required: 1
        },
        speed: {
          max: 10,
          current: 0
        },
        heat: {
          min: 0,
          max: 100,
          current: 10
        }
      });
    });
  });
});
