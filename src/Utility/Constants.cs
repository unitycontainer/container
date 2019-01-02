
namespace Unity
{
    internal static class Constants
    {
        public const string AmbiguousInjectionConstructor = "The type {0} has multiple constructors of length {1}. Unable to disambiguate.";
        public const string CannotConstructAbstractClass = "The current type, {0}, is an abstract class and cannot be constructed. Are you missing a type mapping?";
        public const string CannotConstructDelegate = "The current type, {0}, is delegate and cannot be constructed. Unity only supports resolving Func&lt;T&gt; and Func&lt;IEnumerable&lt;T&gt;&gt; by default.";
        public const string CannotConstructInterface = "The current type, {0}, is an interface and cannot be constructed. Are you missing a type mapping?";
        public const string CannotInjectMethodWithOutParam = "The method {1} on type {0} has an out parameter. Injection cannot be performed.";
        public const string CannotInjectOpenGenericMethod = "The method {1} on type {0} is marked for injection, but it is an open generic method. Injection cannot be performed.";
        public const string CannotResolveOpenGenericType = "The type {0} is an open generic type. An open generic type cannot be resolved.";
        public const string LifetimeManagerInUse = "The lifetime manager is already registered. Lifetime managers cannot be reused, please create a new one.";
        public const string MissingDependency = "Could not resolve dependency for build key {0}.";
        public const string NoOperationExceptionReason = "while resolving";
        public const string NotAGenericType = "The type {0} is not a generic type, and you are attempting to inject a generic parameter named '{1}'.";
        public const string ResolutionFailed = "Resolution of the dependency failed, type = '{0}', name = '{1}'.\nException occurred while: {2}.\nException is: {3} - {4}\n-----------------------------------------------\nAt the time of the exception, the container was: ";
        public const string SelectedConstructorHasRefParameters = "The constructor {1} selected for type {0} has ref or out parameters. Such parameters are not supported for constructor injection.";
        public const string SelectedConstructorHasRefItself = "The constructor {1} selected for type {0} has reference to itself. Such references create infinite loop during resolving.";
        public const string TypeIsNotConstructable = "The type {0} cannot be constructed. You must configure the container to supply this value.";
        public const string TypesAreNotAssignable = "The type {1} cannot be assigned to variables of type {0}.";
        public const string UnknownType = "<unknown>";
    }
}

