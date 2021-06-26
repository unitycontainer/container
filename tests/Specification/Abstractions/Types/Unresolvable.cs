using System;

namespace Regression
{

    public interface IUnresolvable { }

    public class Unresolvable : PatternBaseType, IUnresolvable
    {
        protected Unresolvable(string id) { Value = id; }

        public static Unresolvable Create(string name) => new Unresolvable(name);

        public override string ToString() => $"Unresolvable.{Value}";
    }

    public class SubUnresolvable : Unresolvable
    {
        private SubUnresolvable(string id)
            : base(id) { }

        public override string ToString() => $"SubUnresolvable.{Value}";

        public new static SubUnresolvable Create(string name) => new SubUnresolvable(name);
    }
}


