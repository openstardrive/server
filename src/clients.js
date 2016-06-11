"use strict"

var InputError = require('./inputError.js');

class Clients {
  constructor() {
    this.clients = {}
  }

  getAll() {
    let list = []
    for (let key in this.clients) {
      list.push(this.clients[key])
    }
    return list
  }

  set(clientData) {
    if (typeof clientData.id !== 'string') {
      throw new InputError('Invalid client id')
    }
    if (typeof clientData.name !== 'string') {
      throw new InputError('Invalid client name')
    }
    if (clientData.features) {
      clientData.features.forEach(this.validateFeature)
    }
    this.clients[clientData.id] = {
      id: clientData.id,
      name: clientData.name,
      features: clientData.features
        ? clientData.features.map(x => {return { name: x.name, enabled: x.enabled }})
        : []
    }
  }

  validateFeature(feature) {
    if (typeof feature.name !== 'string' || typeof feature.enabled !== 'boolean') {
      throw new InputError('Invalid feature')
    }
  }
}

module.exports = Clients
