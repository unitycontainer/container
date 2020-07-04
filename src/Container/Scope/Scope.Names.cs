using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerScope
        {


            #region Metadata Entry

            //[DebuggerDisplay("{Value}", Name = "{Key}")]
            public struct Identity
            {
                public string  Name;
                public int HashCode;
                public int Count;
                public int[] References;
            }

            #endregion
        }
    }
}
