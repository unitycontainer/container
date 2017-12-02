
namespace Unity
{
    internal static class Constants
    {
        public const string AmbiguousInjectionConstructor = "The type {0} has multiple constructors of length {1}. Unable to disambiguate.";
        public const string ArgumentMustNotBeEmpty = "The provided string argument must not be empty.";
        public const string BuildFailedException = "The current build operation (build key {2}) failed: {3} (Strategy type {0}, index {1})";
        public const string CannotConstructAbstractClass = "The current type, {0}, is an abstract class and cannot be constructed. Are you missing a type mapping?";
        public const string CannotConstructDelegate = "The current type, {0}, is delegate and cannot be constructed. Unity only supports resolving Func&lt;T&gt; and Func&lt;IEnumerable&lt;T&gt;&gt; by default.";
        public const string CannotConstructInterface = "The current type, {0}, is an interface and cannot be constructed. Are you missing a type mapping?";
        public const string CannotExtractTypeFromBuildKey = "Cannot extract type from build key {0}.";
        public const string CannotInjectGenericMethod = "The method {0}.{1}({2}) is an open generic method. Open generic methods cannot be injected.";
        public const string CannotInjectIndexer = "The property {0} on type {1} is an indexer. Indexed properties cannot be injected.";
        public const string CannotInjectMethodWithOutParam = "The method {1} on type {0} has an out parameter. Injection cannot be performed.";
        public const string CannotInjectMethodWithOutParams = "The method {0}.{1}({2}) has at least one out parameter. Methods with out parameters cannot be injected.";
        public const string CannotInjectMethodWithRefParams = "The method {0}.{1}({2}) has at least one ref parameter.Methods with ref parameters cannot be injected.";
        public const string CannotInjectOpenGenericMethod = "The method {1} on type {0} is marked for injection, but it is an open generic method. Injection cannot be performed.";
        public const string CannotInjectStaticMethod = "The method {0}.{1}({2}) is static. Static methods cannot be injected.";
        public const string CannotInjectFactory = "Injection factory can not be used in combination with type mapping (RegisterType<To, From>() or RegisterType<To>(new [Delegate]InjectionFactory()) )";
        public const string CannotResolveOpenGenericType = "The type {0} is an open generic type. An open generic type cannot be resolved.";
        public const string ConstructorArgumentResolveOperation = "Resolving parameter '{0}' of constructor {1}";
        public const string ConstructorParameterResolutionFailed = "The parameter {0} could not be resolved when attempting to call constructor {1}.";
        public const string ExceptionNullParameterValue = "Parameter type inference does not work for null values. Indicate the parameter type explicitly using a properly configured instance of the InjectionParameter or InjectionParameter&lt;T&gt; classes.";
        public const string InvokingConstructorOperation = "Calling constructor {0}";
        public const string InvokingMethodOperation = "Calling method {0}.{1}";
        public const string KeyAlreadyPresent = "An item with the given key is already present in the dictionary.";
        public const string LifetimeManagerInUse = "The lifetime manager is already registered. Lifetime managers cannot be reused, please create a new one.";
        public const string MarkerBuildPlanInvoked = "The override marker build plan policy has been invoked. This should never happen, looks like a bug in the container.";
        public const string MethodArgumentResolveOperation = "Resolving parameter '{0}' of method {1}.{2}";
        public const string MethodParameterResolutionFailed = "The value for parameter '{1}' of method {0} could not be resolved. ";
        public const string MissingDependency = "Could not resolve dependency for build key {0}.";
        public const string MultipleInjectionConstructors = "The type {0} has multiple constructors marked with the InjectionConstructor attribute. Unable to disambiguate.";
        public const string MustHaveOpenGenericType = "The supplied type {0} must be an open generic type.";
        public const string MustHaveSameNumberOfGenericArguments = "The supplied type {0} does not have the same number of generic arguments as the target type {1}.";
        public const string NoConstructorFound = "The type {0} does not have an accessible constructor.";
        public const string NoMatchingGenericArgument = "The type {0} does not have a generic argument named '{1}'";
        public const string NoOperationExceptionReason = "while resolving";
        public const string NoSuchConstructor = "The type {0} does not have a constructor that takes the parameters ({1}).";
        public const string NoSuchMethod = "The type {0} does not have a public method named {1} that takes the parameters ({2}).";
        public const string NoSuchProperty = "The type {0} does not contain an instance property named {1}.";
        public const string NotAGenericType = "The type {0} is not a generic type, and you are attempting to inject a generic parameter named '{1}'.";
        public const string NotAnArrayTypeWithRankOne = "The type {0} is not an array type with rank 1, and you are attempting to use a [DependencyArray] attribute on a parameter or property with this type.";
        public const string OptionalDependenciesMustBeReferenceTypes = "Optional dependencies must be reference types. The type {0} is a value type.";
        public const string PropertyNotSettable = "The property {0} on type {1} is not settable.";
        public const string PropertyTypeMismatch = "The property {0} on type {1} is of type {2}, and cannot be injected with a value of type {3}.";
        public const string PropertyValueResolutionFailed = "The value for the property '{0}' could not be resolved.";
        public const string ProvidedStringArgMustNotBeEmpty = "The provided string argument must not be empty.";
        public const string ResolutionFailed = "Resolution of the dependency failed, type = '{0}', name = '{1}'.\nException occurred while: {2}.\nException is: {3} - {4}\n-----------------------------------------------\nAt the time of the exception, the container was: ";
        public const string ResolutionTraceDetail = "Resolving {0},{1}";
        public const string ResolutionWithMappingTraceDetail = "Resolving {0},{1} (mapped from {2}, {3})";
        public const string ResolvingPropertyValueOperation = "Resolving value for property {0}.{1}";
        public const string SelectedConstructorHasRefParameters = "The constructor {1} selected for type {0} has ref or out parameters. Such parameters are not supported for constructor injection.";
        public const string SelectedConstructorHasRefItself = "The constructor {1} selected for type {0} has reference to itself. Such references create infinite loop during resolving.";
        public const string SettingPropertyOperation = "Setting value for property {0}.{1}";
        public const string TypeIsNotConstructable = "The type {0} cannot be constructed. You must configure the container to supply this value.";
        public const string TypesAreNotAssignable = "The type {1} cannot be assigned to variables of type {0}.";
        public const string UnknownType = "<unknown>";
    }
}

