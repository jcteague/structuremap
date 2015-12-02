using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing
{
    public static class Exception<T> where T : Exception
    {
        public static T ShouldBeThrownBy(Action action)
        {
            T exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e.ShouldBeOfType<T>();
            }

            if (exception == null) Assert.Fail("An exception was expected, but not thrown by the given action.");

            return exception;
        }
    }


    public delegate void MethodThatThrows();

    public static class SpecificationExtensions
    {
        public static void ShouldContain<T>(this IEnumerable<T> actual, Func<T, bool> expected)
        {
            actual.Count().ShouldBeGreaterThan(0);
            actual.Any(expected).ShouldBeTrue();
        }


        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = (actual is IList) ? (IList) actual : actual.ToList();
            var expectedList = (expected is IList) ? (IList) expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList);
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, params T[] expected)
        {
            var actualList = (actual is IList) ? (IList) actual : actual.ToList();
            var expectedList = (expected is IList) ? (IList) expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList);
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            try
            {
                actual.Count.ShouldBe(expected.Count);

                for (var i = 0; i < actual.Count; i++)
                {
                    actual[i].ShouldBe(expected[i]);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("ACTUAL:");
                foreach (var o in actual)
                {
                    Debug.WriteLine(o);
                }
                throw;
            }
        }

        public static void ShouldBeFalse(this bool condition)
        {
            condition.ShouldBe(false);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            condition.ShouldBe(true);
        }

        public static void ShouldBeNull(this object anObject)
        {
            anObject.ShouldBe(null);
        }

        public static void ShouldNotBeNull(this object anObject)
        {
            anObject.ShouldNotBe(null);
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            ReferenceEquals(actual, expected).ShouldBeTrue();
            return expected;
        }

        public static T IsType<T>(this object actual)
        {
            actual.ShouldBeOfType(typeof (T));
            return (T) actual;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            ReferenceEquals(actual, expected).ShouldBeFalse();
            return expected;
        }

        public static void ShouldNotBeOfType<T>(this object actual)
        {
            actual.ShouldNotBeOfType(typeof (T));
        }

        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            actual.GetType().ShouldNotBe(expected);
        }


        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            (arg1.CompareTo(arg2) > 0).ShouldBeTrue();

            return arg2;
        }

        public static string ShouldNotBeEmpty(this string aString)
        {
            aString.IsNotEmpty().ShouldBeTrue();

            return aString;
        }

        public static void ShouldContain(this string actual, string expected)
        {
            actual.Contains(expected).ShouldBeTrue();
        }

        public static string ShouldNotContain(this string actual, string expected)
        {
            actual.Contains(expected).ShouldBeFalse();
            return actual;
        }


        public static void ShouldStartWith(this string actual, string expected)
        {
            actual.StartsWith(expected).ShouldBeTrue();
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                e.GetType().ShouldBe(exceptionType);
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail(String.Format("Expected {0} to be thrown.", exceptionType.FullName));
            }

            return exception;
        }
    }
}