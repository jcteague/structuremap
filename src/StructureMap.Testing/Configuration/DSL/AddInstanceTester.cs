using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class AddInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(registry =>
            {
                registry.Scan(x => x.AssemblyContainingType<ColorWidget>());

                // Add an instance with properties
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("DarkGreen")
                    .Ctor<string>("color").Is("DarkGreen");

                // Add an instance by specifying the ConcreteKey
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("Purple")
                    .Ctor<string>("color").Is("Purple");

                registry.For<IWidget>().Add<AWidget>();
            });
        }

        #endregion

        private IContainer container;

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteKey()
        {
            var widget = (ColorWidget) container.GetInstance<IWidget>("Purple");
            widget.Color.ShouldBe("Purple");
        }

        [Test]
        public void AddAnInstanceWithANameAndAPropertySpecifyingConcreteType()
        {
            var widget = (ColorWidget) container.GetInstance<IWidget>("DarkGreen");
            widget.Color.ShouldBe("DarkGreen");
        }

        [Test]
        public void AddInstanceAndOverrideTheConcreteTypeForADependency()
        {
            IContainer container = new Container(x =>
            {
                x.For<Rule>().Add<WidgetRule>()
                    .Named("AWidgetRule")
                    .Ctor<IWidget>().IsSpecial(i => i.Type<AWidget>());
            });

            container.GetInstance<Rule>("AWidgetRule")
                .IsType<WidgetRule>()
                .Widget.IsType<AWidget>();
        }

        // SAMPLE: named-instance
        [Test]
        public void SimpleCaseWithNamedInstance()
        {
            container = new Container(x => { x.For<IWidget>().Add<AWidget>().Named("MyInstance"); });
            // retrieve an instance by name
            var widget = (AWidget) container.GetInstance<IWidget>("MyInstance");
            widget.ShouldNotBeNull();
        }

        // ENDSAMPLE

        [Test]
        public void SpecifyANewInstanceOverrideADependencyWithANamedInstance()
        {
            container = new Container(registry =>
            {
                registry.For<Rule>().Add<ARule>().Named("Alias");

                // Add an instance by specifying the ConcreteKey
                registry.For<IWidget>()
                    .Add<ColorWidget>()
                    .Named("Purple")
                    .Ctor<string>("color").Is("Purple");

                // Specify a new Instance, override a dependency with a named instance
                registry.For<Rule>().Add<WidgetRule>().Named("RuleThatUsesMyInstance")
                    .Ctor<IWidget>("widget").IsSpecial(x => x.TheInstanceNamed("Purple"));
            });

            container.GetInstance<Rule>("Alias").ShouldBeOfType<ARule>();

            var rule = (WidgetRule) container.GetInstance<Rule>("RuleThatUsesMyInstance");
            rule.Widget.As<ColorWidget>().Color.ShouldBe("Purple");
        }

        [Test]
        public void SpecifyANewInstanceWithADependency()
        {
            // Specify a new Instance, create an instance for a dependency on the fly
            var instanceKey = "OrangeWidgetRule";

            var theContainer = new Container(registry =>
            {
                registry.For<Rule>().Add<WidgetRule>().Named(instanceKey)
                    .Ctor<IWidget>().IsSpecial(
                        i => i.Type<ColorWidget>().Ctor<string>("color").Is("Orange").Named("Orange"));
            });

            theContainer.GetInstance<Rule>(instanceKey).As<WidgetRule>()
                .Widget.As<ColorWidget>()
                .Color.ShouldBe("Orange");
        }


        [Test]
        public void UseAPreBuiltObjectWithAName()
        {
            // Return the specific instance when an IWidget named "Julia" is requested
            var julia = new CloneableWidget("Julia");

            container =
                new Container(x => x.For<IWidget>().Add(julia).Named("Julia"));

            var widget1 = (CloneableWidget) container.GetInstance<IWidget>("Julia");
            var widget2 = (CloneableWidget) container.GetInstance<IWidget>("Julia");
            var widget3 = (CloneableWidget) container.GetInstance<IWidget>("Julia");

            julia.ShouldBeTheSameAs(widget1);
            julia.ShouldBeTheSameAs(widget2);
            julia.ShouldBeTheSameAs(widget3);
        }
    }


    public class WidgetRule : Rule
    {
        private readonly IWidget _widget;

        public WidgetRule(IWidget widget)
        {
            _widget = widget;
        }


        public IWidget Widget
        {
            get { return _widget; }
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var widgetRule = obj as WidgetRule;
            if (widgetRule == null) return false;
            return Equals(_widget, widgetRule._widget);
        }

        public override int GetHashCode()
        {
            return _widget != null ? _widget.GetHashCode() : 0;
        }
    }

    public class WidgetThing : IWidget
    {
        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [Serializable]
    public class CloneableWidget : IWidget, ICloneable
    {
        private readonly string _name;


        public CloneableWidget(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ARule : Rule
    {
    }
}