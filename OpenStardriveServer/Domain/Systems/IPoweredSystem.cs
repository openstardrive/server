namespace OpenStardriveServer.Domain.Systems;

public interface IPoweredSystem : ISystem
{
    int CurrentPower { get; }
}