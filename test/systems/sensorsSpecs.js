var should = require('chai').should();
var Sensors = require('../../src/systems/sensors.js');
var InputError = require('../../src/inputError.js');


describe('sensors', function () {
  var sensors;

  beforeEach(function () {
    sensors = new Sensors();
  });

  describe('initial state', function () {
    it('has id and name set', function () {
      sensors.id.should.equal('sensors');
      sensors.name.should.equal('Sensors');
    });

    it('has power levels set to zero', function () {
      sensors.power.required.should.equal(0);
      sensors.power.current.should.equal(0);
    });

    it('has no contacts', function () {
      sensors.contacts.length.should.equal(0);
    });
  });

  describe('getState', function () {
    it('should return the full state', function () {
      sensors.getState().should.deep.equal({
        id: 'sensors',
        name: 'Sensors',
        power: {current: 0, required: 0},
        contacts: []
      });
    });

    it('should return contact data', function () {
      var contact1 = sensors.addContact({
        name: 'Asteroid',
        position: {x: 3, y: -5},
        destinations: [
          {x: 10, y: 20, arriveInMilliseconds: 10},
          {x: 50, y: -20, arriveInMilliseconds: 50}
        ]
      });
      var contact2 = sensors.addContact({
        name: 'USS Voyager', position: {x: 150, y: 300}
      });
      sensors.getState().contacts.should.deep.equal([contact1, contact2]);
    });
  });

  describe('addContact', function () {
    it('adds a contact with a unique id', function () {
      var contact = {name: 'Asteroid', position: {x: 3, y: -5}};
      var result = sensors.addContact(contact);
      result.should.equal(sensors.contacts[0]);
      should.exist(sensors.contacts[0].id);
      sensors.contacts[0].name.should.equal(contact.name);
      sensors.contacts[0].position.x.should.equal(contact.position.x);
      sensors.contacts[0].position.y.should.equal(contact.position.y);
      sensors.contacts[0].destinations.length.should.equal(0);
    });

    it('can add a contact with destinations', function () {
      var contact = {name: 'Asteroid', position: {x: 3, y: -5}, destinations: [
        {x: 10, y: 20, arriveInMilliseconds: 60},
        {x: 50, y: -20, arriveInMilliseconds: 120}
      ]};
      sensors.addContact(contact);
      sensors.contacts[0].destinations.should.deep.equal(contact.destinations);
    });

    it('should throw if the contact is missing', function () {
      var contact = null;
      should.throw(function () { sensors.addContact(contact); }, InputError, 'Invalid contact.');
    });

    it('should throw if the contact does not include a name', function () {
      var contact = {position: {x: 3, y: -5}};
      should.throw(function () { sensors.addContact(contact); }, InputError, 'Invalid contact name.');
    });

    it('should throw if the contact does not include a position', function () {
      var contact = { name: 'Romulan Warbird' };
      should.throw(function () { sensors.addContact(contact); }, InputError, 'Invalid contact position.');
    });

    it('should throw if the contact destination position is invalid', function () {
      var contact = { name: 'Romulan Warbird', position: {x: 10, y: 0 }, destinations: [
        {x: 100, y: 'apple', arriveAt: '2016-02-27T14:50:01.123Z'}
      ]};
      should.throw(function () { sensors.addContact(contact); }, InputError, 'Invalid contact destination.');
    });

    it('should throw if the contact destination arriveAt is invalid', function () {
      var contact = { name: 'Romulan Warbird', position: {x: 10, y: 0 }, destinations: [
        {x: 100, y: 5, arriveAt: 'like whenever?'}
      ]};
      should.throw(function () { sensors.addContact(contact); }, InputError, 'Invalid contact destination.');
    });
  });

  describe('removeContact', function () {
    it('should remove a contact', function () {
      var contact1 = sensors.addContact({name: 'A', position: {x: 0, y:0}});
      var contact2 = sensors.addContact({name: 'B', position: {x: 1, y:2}});
      var contact3 = sensors.addContact({name: 'C', position: {x: 3, y:4}});
      sensors.removeContact(contact2.id);
      sensors.getState().contacts.should.deep.equal([contact1, contact3]);
    });

    it('should throw if the contact is not found', function () {
      should.throw(function () { sensors.removeContact('fake-id'); }, InputError, 'Unable to locate contact: fake-id');
    });
  });

  describe('updateContact', function () {
    it('should update an existing contact', function () {
      var original = sensors.addContact({name: 'A', position: {x: 0, y:0}});
      var updated = {
        id: original.id, name: 'B', position: {x: 1, y: 2},
        destinations: [
          {x: 50, y: -20, arriveInMilliseconds: 10}
        ]
      };
      sensors.updateContact(updated);
      sensors.getState().contacts[0].should.deep.equal({
        id: original.id, name: 'B', position: {x: 1, y: 2},
        destinations: [
          {x: 50, y: -20, arriveInMilliseconds: 10}
        ]
      });
    });

    it('should throw an error if the update is invalid', function () {
      var original = sensors.addContact({name: 'A', position: {x: 0, y:0}});
      should.throw(function () { sensors.updateContact({id: original.id}); }, InputError, 'Invalid contact name.');
    });

    it('should throw an error if the contact.id is not found', function () {
      should.throw(function () { sensors.updateContact({id: 'contact-1'}); }, InputError, 'Unable to locate contact: contact-1');
    });
  });

  describe('moveContacts', function () {
    var contactA, contactB;

    beforeEach(function () {
      contactA = sensors.addContact({ name: 'A', position: {x: 0, y: 0 }, destinations: [
        {x: 100, y: 50, arriveInMilliseconds: 1000},
        {x: 100, y: 150, arriveInMilliseconds: 2000},
      ]});
      contactB = sensors.addContact({ name: 'A', position: {x: 100, y: 50 }, destinations: [
        {x: 200, y: -50, arriveInMilliseconds: 2000}
      ]});
    });

    it('should move both contacts', function () {
      sensors.moveContacts(500);

      Math.round(contactA.position.x).should.equal(50);
      Math.round(contactA.position.y).should.equal(25);
      Math.round(contactA.destinations[0].arriveInMilliseconds).should.equal(500);

      Math.round(contactB.position.x).should.equal(125);
      Math.round(contactB.position.y).should.equal(25);
      Math.round(contactB.destinations[0].arriveInMilliseconds).should.equal(1500);
    });

    it('should remove the destination once reached', function () {
      sensors.moveContacts(1000);
      Math.round(contactA.position.x).should.equal(100);
      Math.round(contactA.position.y).should.equal(50);
      contactA.destinations.should.deep.equal([{x: 100, y: 150, arriveInMilliseconds: 2000}]);
    });

    it('should move to the second destination', function () {
      sensors.moveContacts(2000);
      Math.round(contactA.position.x).should.equal(100);
      Math.round(contactA.position.y).should.equal(100);
      contactA.destinations.should.deep.equal([{x: 100, y: 150, arriveInMilliseconds: 1000}]);
    });

    it('should stop when no destinations remain', function () {
      sensors.moveContacts(3000);
      Math.round(contactB.position.x).should.equal(200);
      Math.round(contactB.position.y).should.equal(-50);
      contactB.destinations.should.deep.equal([]);
    });
  });
});
