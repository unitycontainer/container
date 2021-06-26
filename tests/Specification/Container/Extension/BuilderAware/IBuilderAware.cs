using System;


namespace Regression.Container
{
    /// <summary>
    /// Implemented on a class when it wants to receive notifications
    /// about the build process.
    /// </summary>
    public interface IBuilderAware
    {
        /// <summary>
        /// Called by the <see cref="BuilderAwareStrategy"/> when the object is being built up.
        /// </summary>
        void OnBuiltUp(Type type, string name);

        // Unity v5 and up do not support tear-down.
        // void OnTearingDown();
    }
}
