using System;

namespace OpenStardriveServer.Domain;

public interface IRandomNumberGenerator
{
    public int RandomInt(int max);
}

public class RandomNumberGenerator : IRandomNumberGenerator
{
    private static readonly Random random = new();

    public int RandomInt(int max)
    {
        return random.Next(max);
    }
}