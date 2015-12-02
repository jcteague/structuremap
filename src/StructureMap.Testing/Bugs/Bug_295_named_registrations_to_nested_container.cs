﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_295_named_registrations_to_nested_container
    {
        [Test]
        public void named_registrations_should_be_isolated_on_child_containers()
        {
            var container = new Container(_ => { }); //Parent Container doesnt contain the named instance

            var child1 = container.CreateChildContainer();
            var child1Thing = Thing1.Build("child1");
            child1.Configure(_ => _.For<IThing>().Add(child1Thing).Named("A"));

            child1.GetInstance<IThing>("A").ShouldBeTheSameAs(child1Thing);

            Debug.WriteLine(container.WhatDoIHave());

            var child2 = container.CreateChildContainer();
            var child2Thing = Thing1.Build("child2");

            child2.Configure(_ => _.For<IThing>().Add(child2Thing).Named("A"));

            child2.GetInstance<IThing>("A")
                .ShouldBeTheSameAs(child2Thing);

            child2.TryGetInstance<IThing>("A")
                .ShouldBeTheSameAs(child2Thing);


            child2.TryGetInstance<IThing>("A")
                .ShouldBeTheSameAs(child2Thing);


            container.TryGetInstance<IThing>("A").ShouldBeNull();
        }

        [Test]
        public void named_registrations_with_singleton_should_be_isolated_on_child_containers()
        {
            var container = new Container(_ => { }); //Parent Container doesnt contain the named instance

            var child1 = container.CreateChildContainer();
            child1.Configure(_ => _.For<IThing>().Singleton().Add<Thing1>().Named("A"));
            var containerChild1Thing = child1.GetInstance<IThing>("A");

            var child2 = container.CreateChildContainer();
            child2.Configure(_ => _.For<IThing>().Singleton().Add<Thing1>().Named("A"));
            var containerChild2Thing = child2.TryGetInstance<IThing>("A");


            containerChild2Thing.ShouldNotBe(containerChild1Thing);

            var parentContainerThing = container.TryGetInstance<IThing>("A");
            parentContainerThing.ShouldBeNull();
        }


        [Test]
        public void named_registrations_should_be_isolated_on_nested_containers()
        {
            var container = new Container(_ => { }); //Parent Container doesnt contain the named instance

            var nested1 = container.GetNestedContainer();
            var nested1Thing = Thing1.Build("child1");
            nested1.Configure(_ => _.For<IThing>().Add(nested1Thing).Named("A"));
            var containerNested1Thing = nested1.GetInstance<IThing>("A");
            containerNested1Thing.ShouldBe(nested1Thing);

            var nested2 = container.GetNestedContainer();
            var nested2Thing = Thing1.Build("child2");
            nested2.Configure(_ => _.For<IThing>().Add(nested2Thing).Named("A"));
            var nested2ContainerThing = nested2.TryGetInstance<IThing>("A");

            nested2ContainerThing.ShouldBe(nested2Thing);

            var parentContainerThing = container.TryGetInstance<IThing>("A");
            parentContainerThing.ShouldBeNull();
        }


        // ******** THIS ONE IS NOT VALID. SINGLETON MAKE NO SENSE IN THIS CONTEXT ************
        // As of now, I've tightened the validation logic to make Configure() blow up
        // if you try to register a SingletonThing to a nested container
        /*
        [Test]
        public void named_registrations_with_singleton_should_be_isolated_on_nested_containers()
        {
            var container = new Container(_ => { }); //Parent Container doesnt contain the named instance

            var nested1 = container.GetNestedContainer();
            nested1.Configure(_ => _.For<IThing>().SingletonThing().Add<Thing1>().Named("A"));
            var containerNested1Thing = nested1.GetInstance<IThing>("A");

            var nested2 = container.GetNestedContainer();
            nested2.Configure(_ => _.For<IThing>().SingletonThing().Add<Thing1>().Named("A"));
            var containerNested2Thing = nested2.TryGetInstance<IThing>("A");

            Assert.AreNotEqual(containerNested1Thing, containerNested2Thing);

            var parentContainerThing = container.TryGetInstance<IThing>("A");
            parentContainerThing.ShouldBeNull();
        }
        */


        public interface IThing
        {
        }

        public class Thing1 : IThing, IEquatable<Thing1>
        {
            public Thing1()
            {
                Value = Guid.NewGuid().ToString();
            }

            public override string ToString()
            {
                return Value;
            }

            public bool Equals(Thing1 other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Value.Equals(other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Thing1) obj);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public static bool operator ==(Thing1 left, Thing1 right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Thing1 left, Thing1 right)
            {
                return !Equals(left, right);
            }

            public static Thing1 Build(string source)
            {
                return new Thing1
                {
                    Value = source,
                };
            }


            public string Value { get; private set; }
        }
    }
}