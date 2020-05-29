using System.Collections.Generic;
using System.Linq;
using Unity.Injection;

namespace Injection.Parameters
{
    public static class DynamicDataSource
    {
        public static IEnumerable<object[]> GenericBases()
        {
            foreach (GenericBase parameter in GetParameterValues().OfType<GenericBase>())
            {
                yield return new object[] { parameter };
            }
        }

        public static IEnumerable<object[]> ParameterBases()
        {
            foreach (ParameterBase parameter in GetParameterValues().OfType<ParameterBase>())
            {
                yield return new object[] { parameter };
            }
        }


        public static IEnumerable<ParameterValue> GetParameterValues()
        {

            yield return new InjectionParameter(string.Empty);
            yield return new InjectionParameter(typeof(string), string.Empty);

            yield return new OptionalParameter();
            yield return new OptionalParameter(typeof(string));
            yield return new OptionalParameter(string.Empty);
            yield return new OptionalParameter(typeof(string), string.Empty);


            yield return new ResolvedParameter();
            yield return new ResolvedParameter(typeof(string));
            yield return new ResolvedParameter(string.Empty);
            yield return new ResolvedParameter(typeof(string), string.Empty);


            yield return new ResolvedArrayParameter(typeof(string));
            yield return new ResolvedArrayParameter(typeof(string), string.Empty);
            yield return new ResolvedArrayParameter(typeof(string), typeof(string), string.Empty);


            yield return new GenericResolvedArrayParameter("T");
            yield return new GenericResolvedArrayParameter("T", string.Empty);

            yield return new GenericParameter("T");
            yield return new GenericParameter("T", string.Empty);
            yield return new GenericParameter("T[]");
            yield return new GenericParameter("T[]", string.Empty);
            yield return new GenericParameter("T()");
            yield return new GenericParameter("T()", string.Empty);
        }
    }
}
