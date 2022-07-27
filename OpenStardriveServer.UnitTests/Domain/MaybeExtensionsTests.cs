using System;
using System.Collections.Generic;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.UnitTests.Domain;

public class MaybeExtensionsTests
{
    [Test]
    public void When_calling_to_maybe_of_class_and_there_is_a_value()
    {
        var testClass = new MaybeExtensionsTests();
        var result = testClass.ToMaybe();
        Assert.That(result.Value, Is.SameAs(testClass));
    }
        
    [Test]
    public void When_calling_to_maybe_of_class_and_there_is_no_value()
    {
        MaybeExtensionsTests testClass = null;
        var result = testClass.ToMaybe();
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_to_maybe_of_nullable_struct_and_there_is_a_value()
    {
        DateTimeOffset? now = DateTimeOffset.UtcNow;
        var result = now.ToMaybe();
        Assert.That(result.Value, Is.EqualTo(now));
    }
        
    [Test]
    public void When_calling_to_maybe_of_nullable_struct_and_there_is_no_value()
    {
        DateTimeOffset? now = null;
        var result = now.ToMaybe();
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_none_if_empty_and_the_string_is_not_null_or_empty()
    {
        var result = "hello".NoneIfEmpty();
        Assert.That(result.Value, Is.EqualTo("hello"));
    }
        
    [Test]
    public void When_calling_none_if_empty_and_the_string_is_empty()
    {
        var result = "".NoneIfEmpty();
        Assert.That(result.HasValue, Is.False);
    }
        
    [Test]
    public void When_calling_none_if_empty_and_the_string_is_null()
    {
        string myString = null;
        var result = myString.NoneIfEmpty();
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_first_or_none_on_a_class_and_there_are_values()
    {
        var result = new[] {"a", "b", "c"}.FirstOrNone();
        Assert.That(result.Value, Is.EqualTo("a"));
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_class_and_there_are_values_and_the_first_is_null()
    {
        var result = new[] {null, "b", "c"}.FirstOrNone();
        Assert.That(result.HasValue, Is.False);
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_class_and_there_are_no_values()
    {
        var result = new string[0].FirstOrNone();
        Assert.That(result.HasValue, Is.False);
    }
    
    [Test]
    public void When_calling_first_or_none_on_a_class_with_a_predicate_and_there_are_values()
    {
        var result = new[] {"a", "b", "c"}.FirstOrNone(x => x == "b");
        Assert.That(result.Value, Is.EqualTo("b"));
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_class_with_a_predicate_and_there_are_values_and_none_match_the_predicate()
    {
        var result = new[] {"a", "b", "c"}.FirstOrNone(x => x == "z");
        Assert.That(result.HasValue, Is.False);
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_class_with_a_predicate_and_there_are_no_values()
    {
        var result = new string[0].FirstOrNone(x => x == "q");
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_first_or_none_on_a_struct_and_there_are_values()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new DateTimeOffset?[] {now, now.AddSeconds(2)}.FirstOrNone();
        Assert.That(result.Value, Is.EqualTo(now));
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_struct_and_there_are_values_and_the_first_is_null()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new DateTimeOffset?[] {null, now, now.AddSeconds(2)}.FirstOrNone();
        Assert.That(result.HasValue, Is.False);
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_struct_and_there_are_no_values()
    {
        var result = new DateTimeOffset?[0].FirstOrNone();
        Assert.That(result.HasValue, Is.False);
    }
    
    [Test]
    public void When_calling_first_or_none_on_a_struct_with_a_predicate_and_there_are_values()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new DateTimeOffset?[] {now.AddSeconds(-2), now, now.AddSeconds(2)}.FirstOrNone(x => x == now);
        Assert.That(result.Value, Is.EqualTo(now));
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_struct_with_a_predicate_and_there_are_values_and_none_match_the_predicate()
    {
        var now = DateTimeOffset.UtcNow;
        var result = new DateTimeOffset?[] {null, now, now.AddSeconds(2)}.FirstOrNone(x => x == DateTimeOffset.UtcNow.AddDays(1));
        Assert.That(result.HasValue, Is.False);
    }
        
    [Test]
    public void When_calling_first_or_none_on_a_struct_with_a_predicate_and_there_are_no_values()
    {
        var result = new DateTimeOffset?[0].FirstOrNone(x => x == DateTimeOffset.UtcNow);
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_maybe_if_and_the_boolean_is_true()
    {
        var result = true.MaybeIf("was true");
        Assert.That(result.Value, Is.EqualTo("was true"));
    }
        
    [Test]
    public void When_calling_maybe_if_and_the_boolean_is_false()
    {
        var result = false.MaybeIf("was true");
        Assert.That(result.HasValue, Is.False);
    }

    [Test]
    public void When_calling_value_or_none_on_a_dictionary_and_there_is_a_match()
    {
        var dictionary = new Dictionary<string, int> {  ["a"] = 1, ["b"] = 2, ["c"] = 3, };
        Assert.That(dictionary.ValueOrNone("b").Value, Is.EqualTo(2));
    }
    
    [Test]
    public void When_calling_value_or_none_on_a_dictionary_and_there_is_not_a_match()
    {
        var dictionary = new Dictionary<string, int> {  ["a"] = 1, ["b"] = 2, ["c"] = 3, };
        Assert.That(dictionary.ValueOrNone("z").HasValue, Is.False);
    }
}