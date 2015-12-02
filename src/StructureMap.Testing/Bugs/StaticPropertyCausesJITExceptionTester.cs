using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class StaticPropertyCausesJITExceptionTester
    {
        [Test]
        public void Get_instance_should_work()
        {
            var container = new Container(x => x.ForConcreteType<ClassWithStaticProperty>());

            container.GetInstance<ClassWithStaticProperty>();
        }
    }

    public class ClassWithStaticProperty
    {
        private static volatile bool disabled;

        public static bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }
    }
}