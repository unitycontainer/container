using System;
using System.ComponentModel;

namespace Unity
{
    public class TypeRegistration 
    {
        public TypeRegistration()
        {
        }

        public virtual MultiRegistration Multi => this as MultiRegistration;

        public virtual void TestMethod()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType() => base.GetType();
    }


    public class MultiRegistration : TypeRegistration
    {
        public MultiRegistration()
        {
        }

        public TypeRegistration Type => this;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override MultiRegistration Multi => throw new NotImplementedException();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void TestMethod()
        {
        }
    }

}
