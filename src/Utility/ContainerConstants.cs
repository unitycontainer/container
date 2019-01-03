
namespace Unity
{
    internal static class Error
    {
        public const string MissingDependency = "Could not resolve dependency for build key {0}.";
        public const string NoOperationExceptionReason = "while resolving";
        public const string NotAGenericType = "The type {0} is not a generic type, and you are attempting to inject a generic parameter named '{1}'.";
        public const string ResolutionFailed = "Resolution of the dependency failed, type = '{0}', name = '{1}'.\nException occurred while: {2}.\nException is: {3} - {4}\n-----------------------------------------------\nAt the time of the exception, the container was: ";
        public const string SelectedConstructorHasRefParameters = "The constructor {1} selected for type {0} has ref or out parameters. Such parameters are not supported for constructor injection.";
        public const string SelectedConstructorHasRefItself = "The constructor {1} selected for type {0} has reference to itself. Such references create infinite loop during resolving.";
        public const string TypesAreNotAssignable = "The type {1} cannot be assigned to variables of type {0}.";
        public const string UnknownType = "<unknown>";
    }
}

