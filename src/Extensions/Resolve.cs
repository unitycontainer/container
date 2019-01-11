using System;
using Unity.Injection;

namespace Unity
{
    public static partial class Resolve
    {
        #region Dependency

        public static ParameterBase Dependency<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        //public static ParameterBase Dependency(string name) => new ResolvedParameter(name);

        #endregion


        #region Parameter

        public static ParameterBase Parameter() => new ResolvedParameter();

        public static ParameterBase Parameter(string name) => new ResolvedParameter(name);

        public static ParameterBase Parameter(Type type) => new ResolvedParameter(type);

        public static ParameterBase Parameter<TTarget>() => new ResolvedParameter(typeof(TTarget));

        public static ParameterBase Parameter(Type type, string name) => new ResolvedParameter(type, name);

        public static ParameterBase Parameter<TTarget>(string name) => new ResolvedParameter(typeof(TTarget), name);

        #endregion


        #region Generic

        public static GenericParameter Generic(string genericParameterName) => new GenericParameter(genericParameterName);

        public static GenericParameter Generic(string genericParameterName, string registrationName) => new GenericParameter(genericParameterName, registrationName);

        #endregion


        #region Optional

        public static ParameterBase Optional() => new OptionalParameter();

        public static ParameterBase Optional(string name) => new OptionalParameter(name);

        public static ParameterBase Optional(Type type) => new OptionalParameter(type);

        public static ParameterBase Optional<TTarget>() => new OptionalParameter(typeof(TTarget));

        public static ParameterBase Optional(Type type, string name) => new OptionalParameter(type, name);

        public static ParameterBase Optional<TTarget>(string name) => new OptionalParameter(typeof(TTarget), name);

        #endregion


        #region Field

        public static InjectionMember Field(string name) => new InjectionField(name);

        public static InjectionMember OptionalField(string name) => new InjectionField(name, true);

        #endregion


        #region Property

        public static InjectionMember Property(string name) => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)));

        public static InjectionMember OptionalProperty(string name) => new InjectionProperty(name ?? throw new ArgumentNullException(nameof(name)), true);

        #endregion
    }
}
