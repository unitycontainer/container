using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.BuiltIn
{
    public abstract partial class MemberProcessor<TMemberInfo, TDependency, TData>
    {
        /// <summary>
        /// This method returns an array of <see cref="MemberInfo"/> objects implemented
        /// by the <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// Each processor overrides this method and returns appropriate members. 
        /// Constructor processor returns an array of <see cref="ConstructorInfo"/> objects,
        /// Property processor returns objects of type <see cref="PropertyInfo"/>, and etc.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> implementing members</param>
        /// <returns>A <see cref="Span{MemberInfo}"/> of appropriate <see cref="MemberInfo"/> objects</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TMemberInfo[] GetMembers(Type type);

        /// <summary>
        /// Returns attribute the info is annotated with
        /// </summary>
        /// <param name="info"><see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or <see cref="PropertyInfo"/> member</param>
        /// <returns>Attribute or null if nothing found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual ImportAttribute? GetImportAttribute(TDependency info) => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual TMember? GetInjected<TMember>(RegistrationManager? registration) where TMember : class => null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual Type MemberType(TDependency member) => member.GetType();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetValue(TDependency info, object target, object? value) => throw new NotImplementedException();
    }
}
