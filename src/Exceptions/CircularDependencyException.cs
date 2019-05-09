using System;

namespace Unity.Exceptions
{
    internal class CircularDependencyException : Exception
    {
        public CircularDependencyException(Type type, string name)
            : base($"Circular reference: Type: {type}, Name: {name}")
        {
        }
    }
}
