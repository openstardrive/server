var should = require('chai').should();
var Thrusters = require('../../src/systems/thrusters.js');
var InputError = require('../../src/inputError.js');


describe('thrusters', function () {
  var thrusters;

  beforeEach(function () {
    thrusters = new Thrusters();
  });

  describe('initial state', function () {
    it('has power levels set to zero', function () {
      thrusters.power.required.should.equal(0);
      thrusters.power.current.should.equal(0);
    })
    it('has velocity set to 0 on x, y, and z axis', function () {
      thrusters.velocity.x.should.equal(0);
      thrusters.velocity.y.should.equal(0);
      thrusters.velocity.z.should.equal(0);
    });
    it('has attitude set to 0 for yaw, pitch, and roll', function () {
      thrusters.attitude.yaw.should.equal(0);
      thrusters.attitude.pitch.should.equal(0);
      thrusters.attitude.roll.should.equal(0);
    });
  });

  describe('getState', function () {
    it('should return the current state of the system', function () {
      thrusters.setRequiredPower(1);
      thrusters.setCurrentPower(2);
      thrusters.setAttitude({yaw: 10, pitch: 20, roll: 30});
      thrusters.setVelocity({x: 1, y: 2, z: 3});

      var state = thrusters.getState();
      state.should.deep.equal({
        id: 'thrusters',
        name: 'Thrusters',
        power: {
          current: 2,
          required: 1
        },
        attitude: {yaw: 10, pitch: 20, roll: 30},
        velocity: {x: 1, y: 2, z: 3}
      });
    });
  });

  describe('setAttitude', function () {
    it('should update the attitude', function () {
      thrusters.setAttitude({yaw:10, pitch:20, roll:30});
      thrusters.attitude.yaw.should.equal(10);
      thrusters.attitude.pitch.should.equal(20);
      thrusters.attitude.roll.should.equal(30);
    });

    it('should keep the values in range [0, 360)', function () {
      thrusters.setAttitude({yaw: 360, pitch: -715, roll: -90});
      thrusters.attitude.yaw.should.equal(0);
      thrusters.attitude.pitch.should.equal(5);
      thrusters.attitude.roll.should.equal(270);
    });

    it('should round decimals to integers', function () {
      thrusters.setAttitude({yaw: -90.2, pitch: 10.7, roll: 180.5});
      thrusters.attitude.yaw.should.equal(270);
      thrusters.attitude.pitch.should.equal(11);
      thrusters.attitude.roll.should.equal(181);
    });

    it('should throw on non-numbers', function () {
      should.throw(function () { thrusters.setAttitude({yaw: null, pitch: "abc", roll: []}); }, InputError, 'Invalid attitude.' );
    });

    it('should throw on null input', function () {
      should.throw(function () { thrusters.setAttitude(); }, InputError, 'Invalid attitude.' );
    });

    it('should throw if there is not enough power', function () {
      thrusters.setRequiredPower(10);
      should.throw(function () { thrusters.setAttitude({yaw: 9, pitch: 8, roll: 7}); }, InputError, 'Insufficient power.' );
    });
  });

  describe('setVelocity', function () {
    it('should update the velocity', function () {
      thrusters.setVelocity({x:10, y:20, z:30});
      thrusters.velocity.x.should.equal(10);
      thrusters.velocity.y.should.equal(20);
      thrusters.velocity.z.should.equal(30);
    });

    it('should round decimals to integers', function () {
      thrusters.setVelocity({x: -90.2, y: 10.7, z: 180.5});
      thrusters.velocity.x.should.equal(-90);
      thrusters.velocity.y.should.equal(11);
      thrusters.velocity.z.should.equal(181);
    });

    it('should throw on non-numbers', function () {
      should.throw(function () { thrusters.setVelocity({x: null, y: "abc", z: []}); }, InputError, 'Invalid velocity.' );
    });

    it('should throw on null input', function () {
      should.throw(function () { thrusters.setVelocity(); }, InputError, 'Invalid velocity.' );
    });

    it('should throw if there is not enough power', function () {
      thrusters.setRequiredPower(10);
      should.throw(function () { thrusters.setVelocity({x: 7, y: 8, z: 9}); }, InputError, 'Insufficient power.' );
    });
  });
});
