let should = require('chai').should()
let Clients = require('../src/clients.js')
let InputError = require('../src/inputError.js');

describe('clients', () => {
  let clients;

  beforeEach(() => {
    clients = new Clients()
  })

  describe('getAll', () => {
    it('should return an empty array if there are no clients', () => {
      let result = clients.getAll()
      result.length.should.equal(0)
    })

    it('should return all the clients', () => {
      let clientA = { id: '1', name: 'Engineering', features: [] }
      let clientB = { id: '2', name: 'Flight Control', features: []}
      clients.set(clientA)
      clients.set(clientB)
      let result = clients.getAll()
      result.length.should.equal(2)
      result.should.contain(clientA)
      result.should.contain(clientB)
    })
  })

  describe('set', () => {
    it('should add a new client', () => {
      let client = { id: '1', name: 'Engineering', features: [
        { name: 'Engines', enabled: true }
      ]}
      clients.set(client)
      let result = clients.getAll()
      result[0].id.should.equal(client.id)
      result[0].name.should.equal(client.name)
    })

    it('should update an existing client', () => {
      let client = { id: '1', name: 'Engineering', features: [
        { name: 'Feature A', enabled: true }
      ]}
      clients.set(client)
      let updatedClient = { id: '1', name: 'Flight Control', features: [
        { name: 'Feature A', enabled: false },
        { name: 'Feature B', enabled: true }
      ]}
      clients.set(updatedClient)
      let result = clients.getAll()
      result.length.should.equal(1)
      result[0].should.deep.equal(updatedClient)
    })

    it('should throw an InputError if there is no id', () => {
      should.throw(() => clients.set({}), InputError, 'Invalid client id')
    })

    it('should throw an InputError if the id is not a string', () => {
      should.throw(() => clients.set({ id: 1 }), InputError, 'Invalid client id')
    })

    it('should throw an InputError if there is no name', () => {
      should.throw(() => clients.set({id: '1'}), InputError, 'Invalid client name')
    })

    it('should throw an InputError if the name is not a string', () => {
      should.throw(() => clients.set({id: '1', name: 123 }), InputError, 'Invalid client name')
    })

    it('should throw an InputError is there are any features without a name', () => {
      let client = { id: '1', name: 'Engineering', features: [
        { name: 'Engines', enabled: true },
        { enabled: false }
      ]}
      should.throw(() => clients.set(client), InputError, 'Invalid feature')
    })

    it('should throw an InputError is there are any features without enabled state', () => {
      let client = { id: '1', name: 'Engineering', features: [
        { name: 'Engines', enabled: true },
        { name: 'X' }
      ]}
      should.throw(() => clients.set(client), InputError, 'Invalid feature')
    })

    it('should ignore extra data', () => {
      let client = { id: '1', name: 'Engineering', other: 'data', x: 123 }
      clients.set(client)
      let result = clients.getAll()
      result[0].should.deep.equal({ id: '1', name: 'Engineering', features: [] })
    })
  })
})
