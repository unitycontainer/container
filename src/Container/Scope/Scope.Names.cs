using Unity.Storage;

namespace Unity.Container
{
    public partial class ContainerScope
    {


        #region Metadata Entry

        //[DebuggerDisplay("{Value}", Name = "{Key}")]
        public struct Identity
        {
            public string Name;
            public int HashCode;
            public int Count;
            public int[] References;
        }

        #endregion
    }
}
