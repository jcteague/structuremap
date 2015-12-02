﻿using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class TransientLifecycleTester
    {
        private ILifecycleContext theContext;
        private TransientLifecycle theLifecycle;
        private ITransientTracking theCache;

        [SetUp]
        public void SetUp()
        {
            theContext = MockRepository.GenerateMock<ILifecycleContext>();
            theLifecycle = new TransientLifecycle();

            theCache = MockRepository.GenerateMock<ITransientTracking>();
            theContext.Stub(x => x.Transients).Return(theCache);
        }

        [Test]
        public void the_cache_is_from_the_transient_of_the_context()
        {
            theLifecycle.FindCache(theContext).ShouldBeTheSameAs(theCache);
        }

        [Test]
        public void eject_all_delegates()
        {
            theLifecycle.EjectAll(theContext);

            theCache.AssertWasCalled(x => x.DisposeAndClear());
        }
    }
}