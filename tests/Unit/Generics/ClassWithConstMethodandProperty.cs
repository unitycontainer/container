namespace Unity.Tests.v5.Generics
{
    public class ClassWithConstMethodandProperty<T>
    {
        private T value;
        public ClassWithConstMethodandProperty()
        { }
        public ClassWithConstMethodandProperty(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public void SetValue(T value)
        {
            this.value = value;
        }
    }
}
