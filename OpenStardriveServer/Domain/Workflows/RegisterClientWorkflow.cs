using System;
using System.Threading.Tasks;
using OpenStardriveServer.Crypto;

namespace OpenStardriveServer.Domain.Workflows;

public interface IRegisterClientWorkflow
{
    Task<RegisterClientResult> Register(string name);
}

public class RegisterClientWorkflow : IRegisterClientWorkflow
{
    private readonly ICommandRepository commandRepository;
    private readonly IByteGenerator byteGenerator;

    public RegisterClientWorkflow(ICommandRepository commandRepository, IByteGenerator byteGenerator)
    {
        this.commandRepository = commandRepository;
        this.byteGenerator = byteGenerator;
    }
        
    public async Task<RegisterClientResult> Register(string name)
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
            Payload = Json.Serialize(new
            {
                clientId,
                clientSecret,
                name,
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