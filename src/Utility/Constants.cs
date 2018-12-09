
namespace Unity.Utility
{
    internal static class Constants
    {
        public const string CannotInjectGenericMethod = "The method {0}.{1}({2}) is an open generic method. Open generic methods cannot be injected.";
        public const string CannotInjectIndexer = "The property {0} on type {1} is an indexer. Indexed properties cannot be injected.";
        public const string CannotInjectMethodWithOutParams = "The method {0}.{1}({2}) has at least one out parameter. Methods with out parameters cannot be injected.";
        public const string CannotInjectMethodWithRefParams = "The method {0}.{1}({2}) has at least one ref parameter.Methods with ref parameters cannot be injected.";
        public const string CannotInjectStaticMethod = "The method {0}.{1}({2}) is static. Static methods cannot be injected.";
        public const string NoMatchingGenericArgument = "The type {0} does not have a generic argument named '{1}'";
        public const string NoSuchConstructor = "The type {0} does not have a constructor that takes the parameters ({1}).";
        public const string NoSuchMethod = "The type {0} does not have a public method named {1} that takes the parameters ({2}).";
        public const string NoSuchProperty = "The type {0} does not contain an instance property named {1}.";
        public const string NotAGenericType = "The type {0} is not a generic type, and you are attempting to inject a generic parameter named '{1}'.";
        public const string PropertyNotSettable = "The property {0} on type {1} is not settable.";
        public const string PropertyTypeMismatch = "The property {0} on type {1} is of type {2}, and cannot be injected with a value of type {3}.";
        public const string ResolutionFailed = "Resolution of the dependency failed, type = '{0}', name = '{1}'.\nException occurred while: {2}.\nException is: {3} - {4}\n-----------------------------------------------\nAt the time of the exception, the container was: ";
        public const string TypesAreNotAssignable = "The type {1} cannot be assigned to variables of type {0}.";
    }
}

