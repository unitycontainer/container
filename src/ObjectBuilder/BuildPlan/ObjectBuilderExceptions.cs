using System;
using System.Globalization;
using System.Reflection;
using Unity.Builder;

namespace Unity.ObjectBuilder.BuildPlan
{
    public static class ObjectBuilderExceptions
    {
         /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an interface (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructInterface(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.CannotConstructInterface,
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an abstract class (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructAbstractClass(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.CannotConstructAbstractClass, 
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an delegate other than Func{T} or Func{IEnumerable{T}}.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForAttemptingToConstructDelegate(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture, Constants.CannotConstructDelegate, 
                    context.BuildKey.Type), 
                new InvalidRegistrationException());
        }

        public class InvalidRegistrationException : Exception
        {
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        public static void ThrowForNullExistingObject(IBuilderContext context)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.NoConstructorFound,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved because of an invalid constructor.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        /// <param name="signature">The signature of the invalid constructor.</param>
        public static void ThrowForNullExistingObjectWithInvalidConstructor(IBuilderContext context, string signature)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.SelectedConstructorHasRefParameters,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name,
                    signature));
        }


        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved because of an invalid constructor.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        /// <param name="signature">The signature of the invalid constructor.</param>
        public static void ThrowForReferenceItselfConstructor(IBuilderContext context, string signature)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Constants.SelectedConstructorHasRefItself,
                    (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type.GetTypeInfo().Name,
                    signature));
        }
    }
}
