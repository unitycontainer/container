using System;
using System.ComponentModel;

namespace Unity
{

    public static class RegistrationExtension
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        public interface IFluentSyntax
        {
            /// <summary>
            /// Gets the type of this instance.
            /// </summary>
            /// <returns>The type of this instance.</returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            Type GetType();

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            int GetHashCode();

            /// <summary>
            /// Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="string"/> that represents this instance.
            /// </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            string ToString();

            /// <summary>
            /// Determines whether the specified <see cref="object"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="object"/> to compare with this instance.</param>
            /// <returns>
            /// <see langword="true"/> if the specified <see cref="object"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            bool Equals(object other);

            IFluentSyntax Test { get; }
        }


        public class Syntax : IFluentSyntax
        {
            public void TestMethos() { }

            public IFluentSyntax Test => this;

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


        public static IFluentSyntax Type(this IUnityContainer container, Type type, string name = null)
        {
            return new Syntax();
        }

        public static IFluentSyntax FirstName(this IFluentSyntax syntax, string name)
        {
            return syntax;
        }

        public static IFluentSyntax LastName(this IFluentSyntax syntax, string name)
        {
            return syntax;
        }


        public static TypeRegistration Type<TType>(this IUnityContainer container, string name = null)
        {
            return new TypeRegistration();
        }

        //public static TypeMonad Invoke(this TypeMonad monad, string name = null)
        //{
        //    return monad;
        //}



    }
}
