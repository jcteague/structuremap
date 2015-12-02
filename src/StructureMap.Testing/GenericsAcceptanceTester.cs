using System;
using System.Reflection;
using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.TypeRules;

namespace StructureMap.Testing
{
    [TestFixture]
    public class GenericsAcceptanceTester
    {
        public interface IService<T>
        {
        }

        public interface IHelper<T>
        {
        }

        public class Service<T> : IService<T>
        {
            private readonly IHelper<T> _helper;

            public Service(IHelper<T> helper)
            {
                _helper = helper;
            }

            public IHelper<T> Helper
            {
                get { return _helper; }
            }
        }

        public class Service2<T> : IService<T>
        {
            public Type GetT()
            {
                return typeof (T);
            }
        }

        public class ServiceWithPlug<T> : IService<T>
        {
            private readonly IPlug<T> _plug;

            public ServiceWithPlug(IPlug<T> plug)
            {
                _plug = plug;
            }


            public IPlug<T> Plug
            {
                get { return _plug; }
            }
        }

        public class Helper<T> : IHelper<T>
        {
        }

        [Test]
        public void CanBuildAGenericObjectThatHasAnotherGenericObjectAsAChild()
        {
            var container = new Container(x =>
            {
                x.For(typeof (IService<>)).Use(typeof (Service<>));
                x.For(typeof (IHelper<>)).Use(typeof (Helper<>));
            });

            container.GetInstance<IService<string>>()
                .ShouldBeOfType<Service<string>>()
                .Helper.ShouldBeOfType<Helper<string>>();
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithGenericParameter()
        {
            var family = new PluginFamily(typeof (IGenericService<int>));
        }

        [Test]
        public void CanCreatePluginFamilyForGenericTypeWithoutGenericParameter()
        {
            var family = new PluginFamily(typeof (IGenericService<>));
        }

        [Test]
        public void CanGetPluginFamilyFromPluginGraphWithNoParameters()
        {
            //var builder = new PluginGraphBuilder();
            //var scanner = new AssemblyScanner();
            //scanner.Assembly(GetType().Assembly);

            //builder.AddScanner(scanner);
            var graph = PluginGraph.CreateRoot();

            graph.Families[typeof (IGenericService<int>)].ShouldBeTheSameAs(
                graph.Families[typeof (IGenericService<int>)]);

            graph.Families[typeof (IGenericService<string>)].ShouldBeTheSameAs(
                graph.Families[typeof (IGenericService<string>)]);

            graph.Families[typeof (IGenericService<>)].ShouldBeTheSameAs(
                graph.Families[typeof (IGenericService<>)]);
        }

        [Test]
        public void CanGetTheSameInstanceOfGenericInterfaceWithSingletonLifecycle()
        {
            var con = new Container(x =>
            {
                x.ForSingletonOf(typeof (IService<>)).Use(typeof (Service<>));
                x.For(typeof (IHelper<>)).Use(typeof (Helper<>));
            });

            var first = con.GetInstance<IService<string>>();
            var second = con.GetInstance<IService<string>>();

            first.ShouldBeTheSameAs(second);
        }


        [Test]
        public void CanPlugGenericConcreteClassIntoGenericInterfaceWithNoGenericParametersSpecified()
        {
            var canPlug = typeof (GenericService<>).CanBeCastTo(typeof (IGenericService<>));
            canPlug.ShouldBeTrue();
        }

        [Test]
        public void CanPlugConcreteNonGenericClassIntoGenericInterface()
        {
            typeof (NotSoGenericService).CanBeCastTo(typeof (IGenericService<>))
                .ShouldBeTrue();
        }

        [Test]
        public void Define_profile_with_generics_and_concrete_type()
        {
            var container = new Container(registry =>
            {
                registry.For(typeof (IHelper<>)).Use(typeof (Helper<>));

                registry.Profile("1", x => x.For(typeof (IService<>)).Use(typeof (Service<>)));

                registry.Profile("2", x => x.For(typeof (IService<>)).Use(typeof (Service2<>)));
            });

            container.GetProfile("1").GetInstance<IService<string>>().ShouldBeOfType<Service<string>>();
            container.GetProfile("2").GetInstance<IService<string>>().ShouldBeOfType<Service2<string>>();
        }

        [Test]
        public void Define_profile_with_generics_with_named_instance()
        {
            IContainer container = new Container(r =>
            {
                r.For(typeof (IService<>)).Add(typeof (Service<>)).Named("Service1");
                r.For(typeof (IService<>)).Add(typeof (Service2<>)).Named("Service2");

                r.For(typeof (IHelper<>)).Use(typeof (Helper<>));

                r.Profile("1", x => x.For(typeof (IService<>)).Use("Service1"));

                r.Profile("2", x => x.For(typeof (IService<>)).Use("Service2"));
            });

            container.GetProfile("1").GetInstance<IService<string>>().ShouldBeOfType<Service<string>>();


            container.GetProfile("2").GetInstance<IService<int>>().ShouldBeOfType<Service2<int>>();
        }

        [Test]
        public void GenericsTypeAndProfileOrMachine()
        {
            var container = new Container(registry =>
            {
                registry.For(typeof (IHelper<>)).Use(typeof (Helper<>));
                registry.For(typeof (IService<>)).Use(typeof (Service<>)).Named("Default");
                registry.For(typeof (IService<>)).Add(typeof (ServiceWithPlug<>)).Named("Plugged");
                registry.For(typeof (IPlug<>)).Use(typeof (ConcretePlug<>));

                registry.Profile("1", x => { x.For(typeof (IService<>)).Use("Default"); });

                registry.Profile("2", x => { x.For(typeof (IService<>)).Use("Plugged"); });
            });

            container.GetProfile("1").GetInstance(typeof (IService<string>)).ShouldBeOfType<Service<string>>();

            container.GetProfile("2").GetInstance(typeof (IService<string>))
                .ShouldBeOfType<ServiceWithPlug<string>>();

            container.GetProfile("1").GetInstance(typeof (IService<string>)).ShouldBeOfType<Service<string>>();
        }


        [Test]
        public void GetGenericTypeByString()
        {
            var assem = Assembly.GetExecutingAssembly();
            var type = assem.GetType("StructureMap.Testing.ITarget`2");

            type.GetGenericTypeDefinition()
                .ShouldBe(typeof (ITarget<,>));
        }


        [Test]
        public void SmokeTestCanBeCaseWithImplementationOfANonGenericInterface()
        {
            GenericsPluginGraph.CanBeCast(typeof (ITarget<,>), typeof (DisposableTarget<,>)).ShouldBeTrue();
        }
    }


    public class ComplexType<T>
    {
        private readonly int _age;
        private readonly string _name;

        public ComplexType(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Age
        {
            get { return _age; }
        }

        [ValidationMethod]
        public void Validate()
        {
            throw new ApplicationException("Break!");
        }
    }

    public interface ITarget<T, U>
    {
    }

    public class SpecificTarget<T, U> : ITarget<T, U>
    {
    }

    public class DisposableTarget<T, U> : ITarget<T, U>, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

    public interface ITarget2<T, U, V>
    {
    }

    public class SpecificTarget2<T, U, V> : ITarget2<T, U, V>
    {
    }

    public interface IGenericService<T>
    {
        void DoSomething(T thing);
    }

    public class GenericService<T> : IGenericService<T>
    {
        #region IGenericService<T> Members

        public void DoSomething(T thing)
        {
            throw new NotImplementedException();
        }

        #endregion

        public Type GetGenericType()
        {
            return typeof (T);
        }
    }

    public class NotSoGenericService : IGenericService<string>
    {
        public void DoSomething(string thing)
        {
        }
    }
}