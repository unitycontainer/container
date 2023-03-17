using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    public abstract partial class MemberStrategy<TMemberInfo, TDependency, TData>
    {
        [DebuggerDisplay("Type: {ContractType?.Name}, Name: {ContractName}  {ValueData}")]
        public struct MemberDescriptor<TContext, TMember> : IImportDescriptor<TMember>
            where TContext : IBuilderContext
        {
            #region Fields

            private static readonly Func<TMember, Type> _memberType;
            private static readonly Func<TMember, Type> _declaringType;

            public ImportData ValueData;
            public ImportData DefaultData;
            private TMember _info;

            #endregion


            #region Constructors

            static MemberDescriptor()
            {
                switch (typeof(TMember))
                {
                    case Type type when type == typeof(ParameterInfo):
                        _memberType = Unsafe.As<Func<TMember, Type>>((Func<ParameterInfo, Type>)GetParameterType);
                        _declaringType = Unsafe.As<Func<TMember, Type>>((Func<ParameterInfo, Type>)GetParameterDeclaringType);
                        break;

                    case Type type when type == typeof(FieldInfo):
                        _memberType = Unsafe.As<Func<TMember, Type>>((Func<FieldInfo, Type>)GetFieldType);
                        _declaringType = Unsafe.As<Func<TMember, Type>>((Func<FieldInfo, Type>)GetFieldDeclaringType);
                        break;

                    case Type type when type == typeof(PropertyInfo):
                        _memberType = Unsafe.As<Func<TMember, Type>>((Func<PropertyInfo, Type>)GetPropertyType);
                        _declaringType = Unsafe.As<Func<TMember, Type>>((Func<PropertyInfo, Type>)GetPropertyDeclaringType);
                        break;

                    case Type type when type == typeof(ConstructorInfo):
                        _memberType = _declaringType = Unsafe.As<Func<TMember, Type>>((Func<ConstructorInfo, Type>)GetConstructorDeclaringType);
                        break;

                    case Type type when type == typeof(MethodInfo):
                        _memberType = _declaringType = Unsafe.As<Func<TMember, Type>>((Func<MethodInfo, Type>)GetMethodDeclaringType);
                        break;

                    default:
                        _memberType = _declaringType = (TMember _) => throw new NotImplementedException();
                        break;
                }
            }

            public MemberDescriptor(TMember info)
            {
                _info = info;
                IsImport = default;
                ValueData = default;
                DefaultData = default;
                AllowDefault = default;
                ContractName = default;
                ContractType = _memberType(info);
            }

            private MemberDescriptor(ref MemberDescriptor<TContext, TMember> parent, Type type, object? data)
            {
                _info = parent._info;
                IsImport     = false;
                AllowDefault = false;
                DefaultData  = default;
                ValueData    = default;
                ContractName = default;
                ContractType = type;

                Dynamic = data;
            }

            #endregion


            #region Member Info

            /// <inheritdoc />
            public TMember MemberInfo
            {
                get => _info;
                set
                {
                    _info = value;
                    ContractType = _memberType(value);
                }
            }

            /// <inheritdoc />
            public Type MemberType => _memberType(MemberInfo);

            /// <inheritdoc />
            public Type DeclaringType => _declaringType(MemberInfo);

            #endregion


            #region Metadata

            /// <inheritdoc />
            public bool IsImport { get; set; }

            /// <inheritdoc />
            public bool RequireBuild => ImportType.Dynamic == ValueData.Type;

            #endregion


            #region Contract

            public Type ContractType { get; set; }

            public string? ContractName { get; set; }

            #endregion


            #region Parameters

            public IList Arguments
            {
                set => ValueData[ImportType.Arguments] = value;
            }

            #endregion


            #region Default Value

            /// <inheritdoc />
            public bool AllowDefault { get; set; }

            /// <inheritdoc />
            public object? Default
            {
                set
                {
                    AllowDefault = true;
                    DefaultData[ImportType.Value] = value;
                }
            }

            #endregion


            #region Value

            /// <inheritdoc />
            public object? Value
            {
                set => ValueData[ImportType.Value] = value;
            }


            /// <inheritdoc />
            public object? Dynamic
            {
                set
                {
                    if (value is IImportProvider provider)
                        provider.ProvideImport<TContext, MemberDescriptor<TContext, TMember>>(ref this);
                    else
                        ValueData[ImportType.Dynamic] = value;
                }
            }

            /// <inheritdoc />
            public void None() => ValueData = default;

            #endregion


            #region Scope

            public MemberDescriptor<TContext, TMember> With(Type type, object? value)
                => new MemberDescriptor<TContext, TMember>(ref this, type, value);

            #endregion


            #region Implementation

            private static Type GetParameterType(ParameterInfo info) => info.ParameterType;
            private static Type GetParameterDeclaringType(ParameterInfo info) => info.Member.DeclaringType!;

            private static Type GetFieldType(FieldInfo info) => info.FieldType;
            private static Type GetFieldDeclaringType(FieldInfo info) => info.DeclaringType!;

            private static Type GetPropertyType(PropertyInfo info) => info.PropertyType;
            private static Type GetPropertyDeclaringType(PropertyInfo info) => info.DeclaringType!;

            private static Type GetMethodDeclaringType(MethodInfo info) => info.DeclaringType!;
            private static Type GetConstructorDeclaringType(ConstructorInfo info) => info.DeclaringType!;

            #endregion
        }
    }
}
