﻿using System;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class SessionCacheTester
    {
        private IBuildSession theResolver;
        private SessionCache theCache;
        private IPipelineGraph thePipeline;
        private IInstanceGraph theInstances;

        [SetUp]
        public void SetUp()
        {
            theResolver = MockRepository.GenerateMock<IBuildSession>();

            theCache = new SessionCache(theResolver);

            thePipeline = MockRepository.GenerateMock<IPipelineGraph>();
            thePipeline.Stub(x => x.ToModel()).Return(new Container().Model);

            theInstances = MockRepository.GenerateMock<IInstanceGraph>();
            thePipeline.Stub(x => x.Instances).Return(theInstances);
        }

        [Test]
        public void get_instance_if_the_object_does_not_already_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));

            var foo = new Foo();

            theResolver.Stub(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo);


            theCache.GetObject(typeof (IFoo), instance, new TransientLifecycle()).ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_instance_if_the_object_is_unique_and_does_not_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            instance.SetLifecycleTo(Lifecycles.Unique);

            var foo = new Foo();
            var foo2 = new Foo();

            theResolver.Stub(x => x.BuildUnique(typeof (IFoo), instance)).Return(foo).Repeat.Once();
            theResolver.Stub(x => x.BuildUnique(typeof (IFoo), instance)).Return(foo2).Repeat.Once();

            theCache.GetObject(typeof (IFoo), instance, new UniquePerRequestLifecycle()).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance, new UniquePerRequestLifecycle()).ShouldBeTheSameAs(foo2);
        }

        [Test]
        public void get_instance_remembers_the_first_object_created()
        {
            var instance = new ConfiguredInstance(typeof (Foo));

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo).Repeat.Once();


            theCache.GetObject(typeof (IFoo), instance, Lifecycles.Transient).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance, Lifecycles.Transient).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance, Lifecycles.Transient).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance, Lifecycles.Transient).ShouldBeTheSameAs(foo);
            theCache.GetObject(typeof (IFoo), instance, Lifecycles.Transient).ShouldBeTheSameAs(foo);

            theResolver.VerifyAllExpectations();
        }

        [Test]
        public void get_default_if_it_does_not_already_exist()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            theInstances.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Stub(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo);

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void get_default_is_cached()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            theInstances.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo)
                .Repeat.Once();

            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);
            theCache.GetDefault(typeof (IFoo), thePipeline).ShouldBeTheSameAs(foo);


            theResolver.VerifyAllExpectations();
        }

        [Test]
        public void start_with_explicit_args()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            theInstances.Stub(x => x.GetDefault(typeof (IFoo))).Throw(new NotImplementedException());

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void try_get_default_completely_negative_case()
        {
            theCache.TryGetDefault(typeof (IFoo), thePipeline).ShouldBeNull();
        }

        [Test]
        public void try_get_default_with_explicit_arg()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void try_get_default_with_a_default()
        {
            var instance = new ConfiguredInstance(typeof (Foo));
            theInstances.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo)
                .Repeat.Once();

            theCache.TryGetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo);
        }

        [Test]
        public void explicit_wins_over_instance_in_try_get_default()
        {
            var foo1 = new Foo();

            var args = new ExplicitArguments();
            args.Set<IFoo>(foo1);

            theCache = new SessionCache(theResolver, args);

            var instance = new ConfiguredInstance(typeof (Foo));
            theInstances.Stub(x => x.GetDefault(typeof (IFoo))).Return(instance);

            var foo2 = new Foo();

            theResolver.Expect(x => x.ResolveFromLifecycle(typeof (IFoo), instance)).Return(foo2)
                .Repeat.Once();

            theCache.GetDefault(typeof (IFoo), thePipeline)
                .ShouldBeTheSameAs(foo1);
        }

        [Test]
        public void
            should_throw_configuration_exception_if_you_try_to_build_the_default_of_something_that_does_not_exist()
        {
            var ex =
                Exception<StructureMapConfigurationException>.ShouldBeThrownBy(
                    () => theCache.GetDefault(typeof (IFoo), thePipeline));

            ex.Context.ShouldBe(
                "There is no configuration specified for StructureMap.Testing.SessionCacheTester+IFoo");

            ex.Title.ShouldBe(
                "No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.SessionCacheTester+IFoo'");
        }

        [Test]
        public void
            should_throw_configuration_exception_if_you_try_to_build_the_default_when_there_is_configuration_by_no_default
            ()
        {
            var container = new Container(x =>
            {
                x.For<IFoo>().Add<Foo>().Named("one");
                x.For<IFoo>().Add<Foo>().Named("two");
            });

            var ex =
                Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => { container.GetInstance<IFoo>(); });

            ex.Context.ShouldContain(
                "No default instance is specified.  The current configuration for type StructureMap.Testing.SessionCacheTester+IFoo is:");
        }


        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
        }
    }
}