using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class RhinoMockRepositoryProxyTester
    {
        [Test]
        public void can_make_dynamic_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var fooMock = mockRepository.DynamicMock(typeof (ITestMocks));

            fooMock.ShouldNotBeNull();
        }

        [Test]
        public void can_make_partial_mocks()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var testPartials = (TestPartials) mockRepository.PartialMock(typeof (TestPartials), new object[0]);

            testPartials.ShouldNotBeNull();
            mockRepository.Replay(testPartials);
            testPartials.Concrete().ShouldBe("Concrete");
            testPartials.Virtual().ShouldBe("Virtual");

            testPartials.Stub(t => t.Virtual()).Return("MOCKED!");
            testPartials.Virtual().ShouldBe("MOCKED!");
        }

        [Test]
        public void can_put_mock_in_replay_mode()
        {
            var mockRepository = new RhinoMockRepositoryProxy();
            var test = (ITestMocks) mockRepository.DynamicMock(typeof (ITestMocks));

            mockRepository.Replay(test);

            test.Stub(t => t.Answer()).Return("YES");
            test.ShouldNotBeNull();
            test.Answer().ShouldBe("YES");
        }
    }

    public interface ITestMocks
    {
        string Answer();
    }

    public class TestPartials
    {
        public string Concrete()
        {
            return "Concrete";
        }

        public virtual string Virtual()
        {
            return "Virtual";
        }
    }
}