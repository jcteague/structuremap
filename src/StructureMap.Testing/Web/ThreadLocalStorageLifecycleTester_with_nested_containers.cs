﻿using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Web;

namespace StructureMap.Testing.Web
{
    [TestFixture]
    public class ThreadLocalStorageLifecycleTester_with_nested_containers
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            lifecycle = new ThreadLocalStorageLifecycle();

            container =
                new Container(
                    x => { x.For<Rule>().HybridHttpOrThreadLocalScoped().Use(() => new ColorRule("Red")); });

            nestedContainer = container.GetNestedContainer();

            var count = 0;
            nestedContainer.Configure(x =>
            {
                x.For<Rule>().Transient().Use("counting", () =>
                {
                    count++;
                    return new ColorRule("Red" + count);
                });
            });
        }

        #endregion

        private Container container;
        private IContainer nestedContainer;
        private ThreadLocalStorageLifecycle lifecycle;

        [Test]
        public void Overrides_Lifecycle()
        {
            var rule1 = container.GetInstance<Rule>();
            var rule2 = nestedContainer.GetInstance<Rule>();

            rule1.ShouldNotBeTheSameAs(rule2);
        }

        [Test]
        public void Does_not_cache_item()
        {
            var initial = lifecycle.FindCache(null).Count;

            nestedContainer.GetInstance<Rule>();

            var after = lifecycle.FindCache(null).Count;

            after.ShouldBe(initial);
        }
    }
}