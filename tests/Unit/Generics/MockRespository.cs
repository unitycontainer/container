namespace Unit.Test.Generics
{
    public class MockRespository<TEntity> : IRepository<TEntity>
    {
        private Refer<TEntity> obj;

        [Dependency]
        public Refer<TEntity> Add
        {
            get { return obj; }
            set { obj = value; }
        }

        [InjectionConstructor]
        public MockRespository(Refer<TEntity> obj)
        { }
    }
}