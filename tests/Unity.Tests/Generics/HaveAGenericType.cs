
namespace Unity.Tests.v5.Generics
{
    public class HaveAGenericType<T1> : IHaveAGenericType<T1>
    {
        public HaveAGenericType()
        { }

        public HaveAGenericType(T1 t1Value)
        {
            PropT1 = t1Value;
        }

        private T1 propT1;

        public T1 PropT1
        {
            get { return propT1; }
            set { propT1 = value; }
        }

        public void Set(T1 value)
        {
            PropT1 = value;
        }
    }
}