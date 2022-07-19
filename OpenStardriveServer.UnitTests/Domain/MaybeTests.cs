using System;
using OpenStardriveServer.Domain;

namespace OpenStardriveServer.UnitTests.Domain
{
    public class MaybeTests
    {
        [Test]
        public void When_there_is_a_value()
        {
            var maybe = Maybe.Some("hi");
            Assert.That(maybe.HasValue, Is.True);
            Assert.That(maybe.Value, Is.EqualTo("hi"));
        }

        [Test]
        public void When_there_is_no_value()
        {
            var maybe = Maybe<string>.None;
            Assert.That(maybe.HasValue, Is.False);
            var result = Assert.Catch(() => _ = maybe.Value);
            Assert.That(result, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void The_default_maybe_is_none()
        {
            Maybe<int> maybe = default;
            Assert.That(maybe.HasValue, Is.False);
        }

        [Test]
        public void When_calling_some_with_a_null_value()
        {
            Assert.Catch(() => Maybe<string>.Some(null));
        }

        [Test]
        public void When_getting_value_or_default_and_there_was_a_value()
        {
            var maybe = Maybe.Some("yes");
            Assert.That(maybe.ValueOrDefault("no"), Is.EqualTo("yes"));
        }
        
        [Test]
        public void When_getting_value_or_default_and_there_was_no_value()
        {
            var maybe = Maybe<string>.None;
            Assert.That(maybe.ValueOrDefault("no"), Is.EqualTo("no"));
        }
        
        [Test]
        public void When_getting_value_or_throw_and_there_was_a_value()
        {
            var maybe = Maybe.Some("yes");
            var exception = new Exception("test exception");
            Assert.That(maybe.ValueOrThrow(exception), Is.EqualTo("yes"));
        }
        
        [Test]
        public void When_getting_value_or_throw_and_there_was_no_value()
        {
            var maybe = Maybe<string>.None;
            var exception = new Exception("test exception");
            var result = Assert.Catch(() => maybe.ValueOrThrow(exception));
            Assert.That(result, Is.SameAs(exception));
        }

        [Test]
        public void When_calling_case_with_a_func_and_there_is_a_value()
        {
            var maybe = Maybe.Some("hi");
            var result = maybe.Case(x => x + "!", () => "none");
            Assert.That(result, Is.EqualTo("hi!"));
        }
        
        [Test]
        public void When_calling_case_with_a_func_and_there_is_no_value()
        {
            var maybe = Maybe<string>.None;
            var result = maybe.Case(x => x + "!", () => "none");
            Assert.That(result, Is.EqualTo("none"));
        }
        
        [Test]
        public void When_calling_case_with_an_action_and_there_is_a_value()
        {
            var maybe = Maybe.Some("hi");
            var result = "nothing set"; 
            maybe.Case(x => { result = x; }, () => { result = "none"; });
            Assert.That(result, Is.EqualTo("hi"));
        }
        
        [Test]
        public void When_calling_case_with_an_action_and_there_is_no_value()
        {
            var maybe = Maybe<string>.None;
            var result = "nothing set"; 
            maybe.Case(x => { result = x; }, () => { result = "none"; });
            Assert.That(result, Is.EqualTo("none"));
        }

        [Test]
        public void When_calling_if_some_and_there_is_a_value()
        {
            var now = DateTimeOffset.UtcNow;
            var maybe = Maybe.Some(now);
            var result = DateTimeOffset.MinValue;
            maybe.IfSome(x => result = x);
            Assert.That(result, Is.EqualTo(now));
        }

        [Test]
        public void When_calling_if_some_and_there_is_no_value()
        {
            var maybe = Maybe<DateTimeOffset>.None;
            var result = DateTimeOffset.MinValue;
            maybe.IfSome(x => result = x);
            Assert.That(result, Is.EqualTo(DateTimeOffset.MinValue));
        }

        [Test]
        public void When_mapping_to_another_maybe_and_there_is_a_value()
        {
            var result = Maybe.Some("hi").Map(x => Maybe.Some(123));
            Assert.That(result.Value, Is.EqualTo(123));
        }
        
        [Test]
        public void When_mapping_to_another_maybe_and_there_is_no_value()
        {
            var result = Maybe<string>.None.Map(x => Maybe.Some(123));
            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void When_mapping_to_another_type_and_there_is_a_value()
        {
            var result = Maybe.Some("hi").Map(x => 123);
            Assert.That(result.Value, Is.EqualTo(123));
        }
        
        [Test]
        public void When_mapping_to_another_type_and_there_is_no_value()
        {
            var result = Maybe<string>.None.Map(x => 123);
            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public void When_calling_or_else_and_there_is_a_value()
        {
            var result = Maybe.Some("hi").OrElse(() => Maybe<string>.Some("else case"));
            Assert.That(result.Value, Is.EqualTo("hi"));
        }
        
        [Test]
        public void When_calling_or_else_and_there_is_no_value()
        {
            var result = Maybe<string>.None.OrElse(() => Maybe<string>.Some("else case"));
            Assert.That(result.Value, Is.EqualTo("else case"));
        }

        [Test]
        public void When_calling_or_else_and_the_chain_also_has_no_value()
        {
            var result = Maybe<string>.None.OrElse(() => Maybe<string>.None);
            Assert.That(result.HasValue, Is.False);
        }
    }

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
    }
}