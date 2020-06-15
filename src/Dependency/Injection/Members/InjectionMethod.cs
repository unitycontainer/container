using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Injection
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : MethodBase<MethodInfo>, IAddPolicies
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given method with the given parameters.
        /// </summary>
        /// <param name="name">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given method with the given parameters.
        /// </summary>
        /// <param name="info"><see cref="MethodInfo"/> of the method to call</param>
        /// <param name="arguments">Arguments to pass to the method</param>
        public InjectionMethod(MethodInfo info, params object[] arguments)
            : base(info, arguments)
        {
        }

        #endregion


        #region IMatch

        public override bool Match(MethodInfo other)
        {
            if (null != Info)
            {
                if (Info.Equals(other)) return true;

                return false;
            }

            if (Name != other.Name) return false;
            if (null == Data) return true;

            return base.Match(other);
        }

        #endregion


        #region Validation

        public override void Validate(Type type)
        {
            if (null == type) throw new ArgumentNullException(nameof(type));

            // Select valid constructor
            MethodInfo? selection = null;
            foreach (var info in DeclaredMembers(type))
            {
                if (!Data.MatchMemberInfo(info)) continue;

                if (null != selection)
                {
                    var message = $" InjectionMethod({Data.Signature()})  is ambiguous \n" +
                        $" It could be matched with more than one method on type '{type.Name}': \n\n" +
                        $"    {selection} \n    {info}";

                    throw new InvalidOperationException(message);
                }

                selection = info;
            }

            // stop if found
            if (null != selection) return;

            // Select invalid constructor
            foreach (var info in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                      BindingFlags.Instance | BindingFlags.Static)
                                     .Where(ctor => ctor.IsFamily || ctor.IsPrivate || ctor.IsStatic))
            {
                if (!Data.MatchMemberInfo(info)) continue;

                if (info.IsStatic)
                {
                    var message = $" InjectionMethod({Data.Signature()})  does not match any valid methods \n" +
                        $" It matches static method {info} but static methods are not supported.";

                    throw new InvalidOperationException(message);
                }

                if (info.IsPrivate)
                {
                    var message = $" InjectionMethod({Data.Signature()})  does not match any valid constructors \n" +
                        $" It matches private method {info} but private methods are not supported.";

                    throw new InvalidOperationException(message);
                }

                if (info.IsFamily)
                {
                    var message = $" InjectionMethod({Data.Signature()})  does not match any valid constructors \n" +
                        $" It matches protected method {info} but protected methods are not supported.";

                    throw new InvalidOperationException(message);
                }
            }

            throw new InvalidOperationException(
                $"InjectionMethod({Data.Signature()}) could not be matched with any method on type {type.Name}.");
        }

        #endregion


        #region Overrides

        protected override MethodInfo SelectMember(Type type, InjectionMember _)
        {
            var noData = 0 == (Data?.Length ?? 0);

            foreach (var member in DeclaredMembers(type))
            {
                if (null != Name && noData) return member;

                if (!Data!.MatchMemberInfo(member)) continue;

                return member;
            }

            throw new ArgumentException(NoMatchFound);
        }

        public override IEnumerable<MethodInfo> DeclaredMembers(Type type)
        {
            return type.SupportedMethods()
                       .Where(member => member.Name == Name);
        }

        protected override string ToString(bool debug = false)
        {
            if (debug)
            {
                return null == Selection
                        ? $"{GetType().Name}: {Name}({Data.Signature()})"
                        : $"{GetType().Name}: {Selection.DeclaringType}.{Name}({Selection.Signature()})";
            }
            else
            {
                return null == Selection
                    ? $"Invoke.Method('{Name}', {Data.Signature()})"
                    : $"Invoke: {Selection.DeclaringType}.{Name}({Selection.Signature()})";
            }
        }

        #endregion



        public void AddPolicies<TPolicySet>(Type type, string? name, ref TPolicySet policies)
            where TPolicySet : IPolicySet
        {
            var select = policies.Get<Func<Type, InjectionMember, MethodInfo>>()
                      ?? SelectMember;

            Selection = select(type, this);
        }
    }
}
