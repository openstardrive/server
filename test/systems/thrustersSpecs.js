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
  });
});
