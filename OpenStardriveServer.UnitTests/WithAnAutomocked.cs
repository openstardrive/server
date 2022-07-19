using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoqCore;
using Moq;

namespace OpenStardriveServer.UnitTests
{
    public class WithAnAutomocked<T>
    {
        private AutoMoqer mocker;

        [SetUp]
        public void WithAnAutoMockedSetup()
        {
            mocker = new AutoMoqer();
        }

        [TearDown]
        public void TearDown()
        {
            classUnderTest = default;
        }

        private T classUnderTest;
        protected T ClassUnderTest => classUnderTest ??= mocker.Create<T>();

        protected Mock<TMock> GetMock<TMock>() where TMock : class
        {
            return mocker.GetMock<TMock>();
        }

        protected TAny Any<TAny>()
        {
            return It.IsAny<TAny>();
        }

        protected List<TAny> IsSequenceEqual<TAny>(IEnumerable<TAny> collection)
        {
            return It.Is<List<TAny>>(x => x.SequenceEqual(collection));
        }

        protected Guid NewGuid() => Guid.NewGuid();

        protected string RandomString() => Guid.NewGuid().ToString();
    }
}