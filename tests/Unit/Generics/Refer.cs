namespace Unity.Tests.v5.Generics
{
    public class Refer<TEntity>
    {
        private string str;

        public string Str
        {
            get { return str; }
            set { str = value; }
        }

        public Refer()
        {
            str = "Hello";
        }
    }
}