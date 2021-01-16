namespace Unity.Injection
{
    public class InjectionMetadata<TItem1, TItem2> : InjectionMember
    {
        public readonly TItem1 Item1;
        public readonly TItem2 Item2;

        #region Constructors

        public InjectionMetadata(TItem1 item1, TItem2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        #endregion
    }
}
