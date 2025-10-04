// Test script to verify teams system is working
const testTeams = [
    {
        id: "team-test-1",
        name: "Test Alpha Team",
        type: "damage",
        simulatorId: "simulator-1",
        priority: "high",
        location: {
            id: "deck-3",
            name: "Engineering Bay 2",
            deck: {
                id: "deck-3",
                number: 3,
                name: "Engineering Deck"
            }
        },
        orders: "Test repair operations",
        officers: [
            {
                id: "officer-test-1",
                name: "Test Officer Johnson",
                position: "Engineer",
                inventory: []
            }
        ]
    }
];

fetch('http://localhost:5002/api/command', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify({
        type: 'teams-update',
        payload: JSON.stringify(testTeams)
    })
})
.then(response => response.text())
.then(data => {
    console.log('Success:', data);
})
.catch((error) => {
    console.error('Error:', error);
});