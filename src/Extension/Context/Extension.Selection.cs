using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        #region Declarations

        public abstract Func<Type, ConstructorInfo[]>? GetConstructors { get; set; }
        
        public abstract Func<Type, FieldInfo[]>? GetFields { get; set; }

        public abstract Func<Type, PropertyInfo[]>? GetProperties { get; set; }

        public abstract Func<Type, MethodInfo[]>? GetMethods { get; set; }

        #endregion
    }
}
