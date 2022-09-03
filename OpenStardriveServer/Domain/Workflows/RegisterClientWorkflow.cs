using System;
using System.Threading.Tasks;
using OpenStardriveServer.Crypto;
using OpenStardriveServer.Domain.Systems.Clients;

namespace OpenStardriveServer.Domain.Workflows;

public interface IRegisterClientWorkflow
{
    Task<RegisterClientResult> Register(string name, string clientType);
}

public class RegisterClientWorkflow : IRegisterClientWorkflow
{
    private readonly ICommandRepository commandRepository;
    private readonly IByteGenerator byteGenerator;
    private readonly IJson json;

    public RegisterClientWorkflow(ICommandRepository commandRepository, IByteGenerator byteGenerator, IJson json)
    {
        this.commandRepository = commandRepository;
        this.byteGenerator = byteGenerator;
        this.json = json;
    }
        
    public async Task<RegisterClientResult> Register(string name, string clientType)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new RegisterClientResult
            {
                Status = RegisterClientResultStatus.InvalidName
            };
        }
            
        var clientId = Guid.NewGuid();
        var clientSecret = byteGenerator.GenerateAsBase64(32);
        
        var command = new Command
        {
            ClientId = clientId,
            Type = "register-client",
            Payload = json.Serialize(new RegisterClientPayload
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Name = name,
                ClientType = clientType
            })
        };
        await commandRepository.Save(command);
        return new RegisterClientResult
        {
            Status = RegisterClientResultStatus.Registered,
            ClientId = clientId,
            ClientSecret = clientSecret
        };
    }
}

public enum RegisterClientResultStatus
{
    Registered,
    InvalidName
}

public class RegisterClientResult
{
    public RegisterClientResultStatus Status { get; set; }
    public Guid ClientId { get; set; }
    public string ClientSecret { get; set; }
}