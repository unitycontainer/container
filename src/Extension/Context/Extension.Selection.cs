namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        #region Declarations

        public abstract GetConstructorsSelector? GetConstructors { get; set; }
        
        public abstract GetFieldsSelector? GetFields { get; set; }

        public abstract GetPropertiesSelector? GetProperties { get; set; }

        public abstract GetMethodsSelector? GetMethods { get; set; }

        #endregion
    }
}
