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

  get(clientId) {
    return this.clients[clientId]
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

    let existing = this.clients[clientData.id] || {}

    this.clients[clientData.id] = {
      id: clientData.id,
      name: clientData.name,
      features: clientData.features
        ? clientData.features.map(x => {return { name: x.name, enabled: x.enabled }})
        : [],
      lastSeen: existing.lastSeen || null
    }
  }

  validateFeature(feature) {
    if (typeof feature.name !== 'string' || typeof feature.enabled !== 'boolean') {
      throw new InputError('Invalid feature')
    }
  }

  visited(clientId) {
    if (typeof clientId !== 'string') return

    if (!this.clients[clientId]) {
      this.clients[clientId] = {
        id: clientId,
        name: 'Unknown'
      }
    }
    this.clients[clientId].lastSeen = new Date().valueOf()
  }
}

module.exports = Clients
