using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenStardriveServer.Domain.Workflows;

namespace OpenStardriveServer.Controllers
{
    public class MainController : ControllerBase
    {
        private readonly IPostCommandWorkflow postCommandWorkflow;
        private readonly IGetCommandResultsWorkflow getCommandResultsWorkflow;
        private readonly IRegisterClientWorkflow registerClientWorkflow;

        public MainController(IPostCommandWorkflow postCommandWorkflow,
            IGetCommandResultsWorkflow getCommandResultsWorkflow,
            IRegisterClientWorkflow registerClientWorkflow)
        {
            this.postCommandWorkflow = postCommandWorkflow;
            this.getCommandResultsWorkflow = getCommandResultsWorkflow;
            this.registerClientWorkflow = registerClientWorkflow;
        }

        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("OpenStardrive server");
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterClientRequest request)
        {
            var result = await registerClientWorkflow.Register(request.Name);
            if (result.Status == RegisterClientResultStatus.InvalidName)
            {
                return BadRequest("Invalid client name");
            }
            
            return Ok(new { result.ClientId, result.ClientSecret });
        }
        

        [Route("/command")]
        [HttpPost]
        public async Task<IActionResult> PostCommand([FromBody] PostCommandRequest request)
        {
            var payload = request.ExtensionData["payload"].GetRawText();
            var result = await postCommandWorkflow.PostCommand(request.ClientSecret, request.Type, payload);
            if (result.Status == PostCommandStatus.ClientNotFound)
            {
                return Unauthorized();
            }
            return Ok(new { result.CommandId });
        }

        [Route("/results")]
        [HttpGet]
        public async Task<IActionResult> GetResults([FromQuery] long cursor = 0)
        {
            return Ok(await getCommandResultsWorkflow.GetCommandResults(cursor));
        }
    }

    public class RegisterClientRequest
    {
        public string Name { get; set; }
    }
    
    public class PostCommandRequest
    {
        public string ClientSecret { get; set; }
        public string Type { get; set; }
        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionData { get; set; }
    }
}