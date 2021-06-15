using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Parameters
{
    public abstract partial class Pattern
    {
#if !UNITY_V4 && !UNITY_V5
        [TestCategory("ParameterBase")]
        [DataTestMethod, DynamicData(nameof(Injection_Parameters_Data))]
        public virtual void ToString(string test, ParameterValue value, string startsWith, string[] contains)
        {
            var parameter = value.ToString();
            
            Assert.IsTrue(parameter.StartsWith(startsWith));
            foreach (var term in contains) Assert.IsTrue(parameter.Contains(term));
        }
#endif

        public static IEnumerable<object[]> Injection_Parameters_Data
        {
            get
            {
                var type = typeof(Pattern);


                #region ResolvedArrayParameter

                yield return new object[]
                {
                    "ResolvedArrayParameter(Type)",      // Name
                    new ResolvedArrayParameter(type),    // The parameter
                    typeof(ResolvedArrayParameter).Name, // Starts with
                    new string [] {
                        "[]"
                    }
                };

                #endregion


                #region GenericParameter

                yield return new object[]
                {
                    "GenericParameter(T)",    // Name
                    new GenericParameter(Name),    // The parameter
                    typeof(GenericParameter).Name, // Starts with
                    new string [] {
                        Name
                    }
                };

                yield return new object[]
                {
                    "GenericParameter(T, name)",        // Name
                    new GenericParameter("T", Name),    // The parameter
                    typeof(GenericParameter).Name,      // Starts with
                    new string [] {
                        "T",
                        Name,
                        "Contract="
                    }
                };

                #endregion


                #region ResolvedParameter

#if !UNITY_V4 && !UNITY_V5
                yield return new object[]
                {
                    "ResolvedParameter()",         // Name
                    new ResolvedParameter(),       // The parameter
                    typeof(ResolvedParameter).Name,// Starts with
                    new string [] {
                        "Type=",
                        "Name=",
                        Contract.AnyContractName
                    }
                };
#endif

                yield return new object[]
                {
                    "ResolvedParameter(Type)",      // Name
                    new ResolvedParameter(type),    // The parameter
                    typeof(ResolvedParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                        "null"
                    }
                };

                yield return new object[]
                {
                    "ResolvedParameter(Type)",      // Name
                    new ResolvedParameter<Pattern>(),
                    typeof(ResolvedParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                        "null"
                    }
                };

#if !UNITY_V4
                yield return new object[]
                {
                    "ResolvedParameter(string)",    // Name
                    new ResolvedParameter(Name),    // The parameter
                    typeof(ResolvedParameter).Name, // Starts with
                    new string [] {
                        Name
                    }
                };
#endif
                yield return new object[]
                {
                    "ResolvedParameter(Type)",      // Name
                    new ResolvedParameter<Pattern>(Name),
                    typeof(ResolvedParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                        Name
                    }
                };

                #endregion


                #region OptionalParameter

#if !UNITY_V4 && !UNITY_V5
                yield return new object[]
                {
                    "OptionalParameter()",         // Name
                    new OptionalParameter(),       // The parameter
                    typeof(OptionalParameter).Name,// Starts with
                    new string [] {
                        "Type=",
                        "Name=",
                        Contract.AnyContractName
                    }
                };
#endif
                yield return new object[]
                {
                    "OptionalParameter(Type)",      // Name
                    new OptionalParameter(type),    // The parameter
                    typeof(OptionalParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                        "null"
                    }
                };

                yield return new object[]
                {
                    "OptionalParameter(Type)",      // Name
                    new OptionalParameter<Pattern>(),
                    typeof(OptionalParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                    }
                };

#if !UNITY_V4
                yield return new object[]
                {
                    "OptionalParameter(string)",    // Name
                    new OptionalParameter(Name),    // The parameter
                    typeof(OptionalParameter).Name, // Starts with
                    new string [] {
                        Name
                    }
                };
#endif

                yield return new object[]
                {
                    "OptionalParameter(Type)",      // Name
                    new OptionalParameter<Pattern>(Name),
                    typeof(OptionalParameter).Name, // Starts with
                    new string [] {
                        type.Name,
                        Name
                    }
                };

                #endregion


                #region InjectionParameter

                yield return new object[]
                {
                    "InjectionParameter(null)",         // Name
                    new InjectionParameter(null),       // The parameter
                    typeof(InjectionParameter).Name,    // Starts with
                    new string [] {
                        "Type=",
                        "Value="
                    }
                };

                yield return new object[]
                {
                    "InjectionParameter(string)",       // Name
                    new InjectionParameter(Name),       // The parameter
                    typeof(InjectionParameter).Name,    // Starts with
                    new string [] {
                        typeof(string).Name,
                        Name
                    }
                };

                yield return new object[]
                {
                    "InjectionParameter(string)",
                    new InjectionParameter<string>(null),
                    typeof(InjectionParameter).Name,
                    new string [] {
                        typeof(string).Name,
                        "null"
                    }
                };

                #endregion
            }
        }
    }
}
