using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Container
{
    [DebuggerDisplay("Type: {Contract.Type?.Name}, Name: {Contract.Name}  {ValueData}")]
    public partial struct ImportDescriptor<TMemberInfo> : IImportDescriptor<TMemberInfo>
    {
        #region Fields

        private static readonly Func<TMemberInfo, Type> _memberType;
        private static readonly Func<TMemberInfo, Type> _declaringType;

        public ImportData ValueData;
        public ImportData DefaultData;

        #endregion


        #region Constructors

        static ImportDescriptor()
        {
            switch (typeof(TMemberInfo))
            {
                case Type type when type == typeof(ParameterInfo):
                    _memberType    = Unsafe.As<Func<TMemberInfo, Type>>((Func<ParameterInfo, Type>)GetParameterType);
                    _declaringType = Unsafe.As<Func<TMemberInfo, Type>>((Func<ParameterInfo, Type>)GetParameterDeclaringType);
                    break;
                
                case Type type when type == typeof(FieldInfo):
                    _memberType    = Unsafe.As<Func<TMemberInfo, Type>>((Func<FieldInfo, Type>)GetFieldType);
                    _declaringType = Unsafe.As<Func<TMemberInfo, Type>>((Func<FieldInfo, Type>)GetFieldDeclaringType);
                    break;

                case Type type when type == typeof(PropertyInfo):
                    _memberType    = Unsafe.As<Func<TMemberInfo, Type>>((Func<PropertyInfo, Type>)GetPropertyType);
                    _declaringType = Unsafe.As<Func<TMemberInfo, Type>>((Func<PropertyInfo, Type>)GetPropertyDeclaringType);
                    break;

                case Type type when type == typeof(ConstructorInfo):
                    _memberType = _declaringType = Unsafe.As<Func<TMemberInfo, Type>>((Func<ConstructorInfo, Type>)GetConstructorDeclaringType);
                    break;

                case Type type when type == typeof(MethodInfo):
                    _memberType = _declaringType = Unsafe.As<Func<TMemberInfo, Type>>((Func<MethodInfo, Type>)GetMethodDeclaringType);
                    break;

                default:
                    _memberType = _declaringType = (TMemberInfo _) => throw new NotImplementedException();
                    break;
            }
        }

        public ImportDescriptor(TMemberInfo info)
        {
            MemberInfo = info;

            Source = default;
            Policy = default;
            IsImport = default;
            Contract = default;
            ValueData = default;
            Attributes = default;
            DefaultData = default;
            AllowDefault = default;
        }
        
        #endregion


        #region Member Info

        /// <inheritdoc />
        public TMemberInfo MemberInfo { get; set; }

        /// <inheritdoc />
        public Type MemberType => _memberType(MemberInfo);

        /// <inheritdoc />
        public Type DeclaringType => _declaringType(MemberInfo);

        #endregion


        #region Metadata

        /// <inheritdoc />
        public bool IsImport { get; set; }

        /// <inheritdoc />
        public Attribute[]? Attributes { get; set; }

        /// <inheritdoc />
        public ImportSource Source { get; set; }

        /// <inheritdoc />
        public CreationPolicy Policy { get; set; }

        /// <inheritdoc />
        public bool RequireBuild => ImportType.Dynamic == ValueData.Type;

        #endregion


        #region Contract

        /// <inheritdoc />
        public Contract Contract { get; set; }

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
            set => ValueData[ImportType.Dynamic] = value;
        }


        /// <inheritdoc />
        public Delegate Pipeline
        {
            set => ValueData[ImportType.Pipeline] = value;
        }

        /// <inheritdoc />
        public void None() => ValueData = default;

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
