require('chai').should();
var Thrusters = require('../../src/systems/thrusters.js');


describe('thrusters', function () {
  var thrusters;

  beforeEach(function () {
    thrusters = new Thrusters();
  });

  describe('initial state', function () {
    it('has power levels set to zero', function () {
      thrusters.requiredPower.should.equal(0);
      thrusters.currentPower.should.equal(0);
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
      thrusters.requiredPower = 1;
      thrusters.currentPower = 2;
      thrusters.setAttitude({yaw: 10, pitch: 20, roll: 30});
      thrusters.setVelocity({x: 1, y: 2, z: 3});

      var state = thrusters.getState();
      state.should.deep.equal({
        id: 'thrusters',
        name: 'Thrusters',
        currentPower: 2,
        requiredPower: 1,
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

    it('should ignore non-numbers', function () {
      thrusters.setAttitude({yaw: 1, pitch: 2, roll: 3});
      thrusters.setAttitude({yaw: null, pitch: "abc", roll: []});
      thrusters.attitude.yaw.should.equal(1);
      thrusters.attitude.pitch.should.equal(2);
      thrusters.attitude.roll.should.equal(3);
    });

    it('should ignore null input', function () {
      thrusters.setAttitude({yaw: 1, pitch: 2, roll: 3});
      thrusters.setAttitude();
      thrusters.attitude.yaw.should.equal(1);
      thrusters.attitude.pitch.should.equal(2);
      thrusters.attitude.roll.should.equal(3);
    });

    it('should not change the attitude if there is not enough power', function () {
      thrusters.setAttitude({yaw: 1, pitch: 2, roll: 3});
      thrusters.requiredPower = 10;
      thrusters.setAttitude({yaw: 9, pitch: 8, roll: 7});
      thrusters.attitude.yaw.should.equal(1);
      thrusters.attitude.pitch.should.equal(2);
      thrusters.attitude.roll.should.equal(3);
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

    it('should ignore non-numbers', function () {
      thrusters.setVelocity({x: 1, y: 2, z: 3});
      thrusters.setVelocity({x: null, y: "abc", z: []});
      thrusters.velocity.x.should.equal(1);
      thrusters.velocity.y.should.equal(2);
      thrusters.velocity.z.should.equal(3);
    });

    it('should ignore null input', function () {
      thrusters.setVelocity({x: 1, y: 2, z: 3});
      thrusters.setVelocity();
      thrusters.velocity.x.should.equal(1);
      thrusters.velocity.y.should.equal(2);
      thrusters.velocity.z.should.equal(3);
    });

    it('should not change velocity if there is not enough power', function () {
      thrusters.setVelocity({x: 1, y: 2, z: 3});
      thrusters.requiredPower = 10;
      thrusters.setVelocity({x: 7, y: 8, z: 9});
      thrusters.velocity.x.should.equal(1);
      thrusters.velocity.y.should.equal(2);
      thrusters.velocity.z.should.equal(3);
    });
  });
});
