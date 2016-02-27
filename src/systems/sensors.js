"use strict";

var System = require('./system.js');
var InputError = require('../inputError.js');
var uuid = require('node-uuid');

class Sensors extends System {
  constructor() {
    super({id: 'sensors', name: 'Sensors'});
    this.contacts = [];
  }

  getState() {
    var state = super.getState();
    state.contacts = this.contacts;
    return state;
  }

  addContact(contact) {
    var newContact = buildContact(contact);
    this.contacts.push(newContact);
    return newContact;
  }

  removeContact(contactId) {
    var index = this.findContactIndex(contactId);
    if (index < 0) {
      throw new InputError('Unable to locate contact: ' + contactId);
    }
    this.contacts.splice(index, 1);
  }

  findContactIndex(contactId) {
    return this.contacts.findIndex(function (item) {
      return item.id == contactId;
    });
  }

  updateContact(contact) {
    var index = this.findContactIndex(contact.id);
    if (index < 0) {
      throw new InputError('Unable to locate contact: ' + contact.id);
    }
    var updated = buildContact(contact);
    updated.id = this.contacts[index].id;
    this.contacts[index] = updated;
  }

  moveContacts(millisecondsElapsed) {
    this.contacts.forEach(function (contact) {
      var remainingMilliseconds = millisecondsElapsed;
      while (remainingMilliseconds && contact.destinations.length > 0) {
        var destination = contact.destinations[0];

        if (destination.arriveInMilliseconds <= remainingMilliseconds) {
          remainingMilliseconds -= destination.arriveInMilliseconds;
          contact.position.x = destination.x;
          contact.position.y = destination.y;
          contact.destinations.shift();
        } else {
          var timeDelta = remainingMilliseconds / destination.arriveInMilliseconds;
          var deltaX = (destination.x - contact.position.x) * timeDelta;
          var deltaY = (destination.y - contact.position.y) * timeDelta;

          contact.position.x += deltaX;
          contact.position.y += deltaY;
          destination.arriveInMilliseconds -= remainingMilliseconds;
          remainingMilliseconds = 0;
        }
      }
    });
  }
}

module.exports = Sensors;


function buildContact(contact) {
  if (!contact) {
    throw new InputError('Invalid contact.')
  }

  if (typeof contact.name !== 'string') {
    throw new InputError('Invalid contact name.')
  }

  if (!contact.position ||
      typeof contact.position.x !== 'number' ||
      typeof contact.position.y !== 'number') {
    throw new InputError('Invalid contact position.')
  }

  var destinations = contact.destinations || [];

  return {
    id: uuid.v4(),
    name: contact.name,
    position: { x: contact.position.x, y: contact.position.y },
    destinations: destinations.map(function (item) {
      if (typeof item.x !== 'number' ||
          typeof item.y !== 'number' ||
          typeof item.arriveInMilliseconds !== 'number') {
        throw new InputError('Invalid contact destination.');
      }
      return {x: item.x, y: item.y, arriveInMilliseconds: item.arriveInMilliseconds};
    })
  }
}
