# First register a client
$registerBody = @{
    name = "test-client"
    clientType = "development"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-WebRequest -Uri "http://localhost:5002/register" -Method POST -Body $registerBody -ContentType "application/json"
    $clientData = $registerResponse.Content | ConvertFrom-Json
    Write-Host "Client registered: $($clientData.clientId)"
    
    # Now send the teams-update command
    $body = @{
        clientSecret = $clientData.clientSecret
        type = "teams-update"
        payload = @(
            @{
                id = "team-test-1"
                name = "Test Alpha Team"
                type = "damage"
                simulatorId = "simulator-1"
                priority = "high"
                location = $null
                orders = "Test repair operations"
                officers = @(
                    @{
                        id = "officer-test-1"
                        name = "Test Officer Johnson"
                        position = "Engineer"
                        inventory = @()
                    }
                )
            }
        )
    } | ConvertTo-Json -Depth 10

    $response = Invoke-WebRequest -Uri "http://localhost:5002/command" -Method POST -Body $body -ContentType "application/json"
    Write-Host "Command Status: $($response.StatusCode)"
    Write-Host "Command Response: $($response.Content)"
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
}