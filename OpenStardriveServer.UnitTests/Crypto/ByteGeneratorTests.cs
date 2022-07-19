using System;
using System.Linq;
using OpenStardriveServer.Crypto;

namespace OpenStardriveServer.UnitTests.Crypto;

public class ByteGeneratorTests : WithAnAutomocked<ByteGenerator>
{
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(32)]
    [TestCase(64)]
    public void When_generating_bytes_it_returns_the_requested_number(int byteCount)
    {
        var result = ClassUnderTest.Generate(byteCount);

        Assert.That(result.Length, Is.EqualTo(byteCount));
    }

    [Test]
    public void When_generating_bytes_the_results_are_random()
    {
        var results = Enumerable.Range(0, 100)
            .Select(x => Convert.ToBase64String(ClassUnderTest.Generate(32)))
            .ToList();

        results.ForEach(item => Assert.That(results.Count(x => x == item), Is.EqualTo(1)));
    }

    [TestCase(1, 4)]
    [TestCase(2, 4)]
    [TestCase(32, 44)]
    [TestCase(64, 88)]
    public void When_generating_bytes_returned_as_base64_strings(int byteCount, int expectedLength)
    {
        var result = ClassUnderTest.GenerateAsBase64(byteCount);

        Assert.That(result.Length, Is.EqualTo(expectedLength));
    }
}