
namespace Unity
{
    internal static class Constants
    {
        public const string CannotInjectGenericMethod = "The method {0}.{1}({2}) is an open generic method. Open generic methods cannot be injected.";
        public const string CannotInjectMethodWithOutParams = "The method {0}.{1}({2}) has at least one out parameter. Methods with out parameters cannot be injected.";
        public const string CannotInjectMethodWithRefParams = "The method {0}.{1}({2}) has at least one ref parameter.Methods with ref parameters cannot be injected.";
        public const string CannotInjectStaticMethod = "The method {0}.{1}({2}) is static. Static methods cannot be injected.";
        public const string NoMatchingGenericArgument = "The type {0} does not have a generic argument named '{1}'";
        public const string NotAGenericType = "The type {0} is not a generic type, and you are attempting to inject a generic parameter named '{1}'.";
        public const string ResolutionFailed = "Resolution of the dependency failed, type = '{0}', name = '{1}'.\nException occurred while: {2}.\nException is: {3} - {4}\n-----------------------------------------------\nAt the time of the exception, the container was: ";
        public const string TypesAreNotAssignable = "The type {1} cannot be assigned to variables of type {0}.";
    }
}

