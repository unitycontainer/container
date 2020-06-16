using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : MethodBase<ConstructorInfo>
    {
        #region Fields

        private const string ctor = ".ctor";

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="arguments">The values for the constructor's parameters, that will
        /// be used to create objects.</param>
        public InjectionConstructor(params object[] arguments)
            : base(ctor, arguments)
        {
        }

        public InjectionConstructor(ConstructorInfo info, params object[] arguments)
            : base(info, arguments)
        {
        }

        #endregion


        #region IMatch

        public override bool Match(ConstructorInfo other)
        {
            if (null != Info)
            {
                if (Info.Equals(other)) return true;

                return false;
            }

            return base.Match(other);
        }

        #endregion


        #region Validation

        public override void Validate(Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));

            // Select valid constructor
            ConstructorInfo? selection = null;
            foreach (var info in DeclaredMembers(type))
            {
                if (!Data.MatchMemberInfo(info)) continue;

                if (null != selection)
                {
                    var message = $" InjectionConstructor({Data.Signature()})  is ambiguous \n" +
                        $" It could be matched with more than one constructor on type '{type.Name}': \n\n" +
                        $"    {selection} \n    {info}";

                    throw new InvalidOperationException(message);
                }

                selection = info;
            }

            // stop if found
            if (null != selection) return;

            // Select invalid constructor
            foreach (var info in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                      BindingFlags.Instance  | BindingFlags.Static)
                                     .Where(ctor => ctor.IsFamily || ctor.IsPrivate || ctor.IsStatic))
            {
                if (!Data.MatchMemberInfo(info)) continue;

                if (info.IsStatic)
                {
                    var message = $" InjectionConstructor({Data.Signature()})  does not match any valid constructors \n" +
                        $" It matches static constructor {info} but static constructors are not supported.";

                    throw new InvalidOperationException(message);
                }

                if (info.IsPrivate)
                {
                    var message = $" InjectionConstructor({Data.Signature()})  does not match any valid constructors \n" +
                        $" It matches private constructor {info} but private constructors are not supported.";

                    throw new InvalidOperationException(message);
                }

                if (info.IsFamily)
                {
                    var message = $" InjectionConstructor({Data.Signature()})  does not match any valid constructors \n" +
                        $" It matches protected constructor {info} but protected constructors are not supported.";

                    throw new InvalidOperationException(message);
                }
            }

            throw new InvalidOperationException(
                $"InjectionConstructor({Data.Signature()}) could not be matched with any constructors on type {type.Name}.");
        }

        #endregion


        #region Overrides

        public override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => 
            type.GetConstructors(BindingFlags)
                .Where(SupportedMembersFilter);

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return null == Selection
                        ? $"{GetType().Name}: {ctor}({Data.Signature()})"
                        : $"{GetType().Name}: {Selection.DeclaringType}({Selection.Signature()})";
            }
            else
            {
                return null == Selection
                        ? $"Invoke.Constructor({Data.Signature()})"
                        : $"Invoke {Selection.DeclaringType}({Selection.Signature()})";
            }
        }

        #endregion
    }
}
