var should = require('chai').should();
var Engines = require('../../src/systems/engines.js');
const EventEmitter = require('events');

var closeEnough = 0.001;

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
      engines.heat.max.should.equal(100);
      engines.heat.powered.should.equal(10);
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
        speed: { max: 10, current: 0, cruising: 6 },
        heat: {max: 100,  powered: 10, current: 10, cruising: 50 }
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

    it('should calculate the heat values for max speed', function () {
      engines.setCurrentSpeed(10);
      engines.heat.target.should.equal(engines.heat.max);
      engines.heat.delta.should.be.closeTo(0.0003, closeEnough);
      engines.heat.secondsUntilOverheat.should.equal(300);
    });

    it('should calculate the heat values for partial speed', function () {
      engines.setCurrentSpeed(5);
      engines.heat.target.should.equal(50);
      engines.heat.delta.should.be.closeTo(0.00015, closeEnough);
      should.equal(engines.heat.secondsUntilOverheat, null);
    });

    it('should calculate the heat values for no speed', function () {
      engines.setCurrentSpeed(0);
      engines.heat.target.should.equal(engines.heat.powered);
      engines.heat.delta.should.be.closeTo(-0.00016, closeEnough);
      should.equal(engines.heat.secondsUntilOverheat, null);
    });
  });

  describe('onPulse', function () {
    it('should update the current heat', function () {
      engines.setCurrentSpeed(10);
      engines.heat.current.should.equal(engines.heat.powered);

      engines.onPulse(150000);
      engines.heat.current.should.be.closeTo(55, closeEnough);

      engines.onPulse(150001);
      engines.heat.current.should.equal(engines.heat.max);

      engines.setCurrentSpeed(0);
      engines.onPulse(150000);
      engines.heat.current.should.be.closeTo(75, closeEnough);
    });
  });

  describe('setEventBus', function () {
    var bus;

    beforeEach(function () {
      bus = new EventEmitter();
      engines.setEventBus(bus);
    });

    it('should set the event bus', function () {
      engines.eventBus.should.equal(bus);
    });

    it('should register for the pulse event', function () {
      bus.listeners('pulse').length.should.equal(1);
    });
  });
});
