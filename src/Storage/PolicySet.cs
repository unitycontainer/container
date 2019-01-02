using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.Policy;

namespace Unity.Storage
{
    [SecuritySafeCritical]
    public struct PolicySet : IPolicySet
    {
        #region Fields

        private Type[]   _iArray;
        private object[] _oArray;

        private IntPtr _iBuffer;
        private IntPtr _oBuffer;

        private int _capacity;
        private int _length;

        #endregion


        #region Constructors

        public PolicySet(object[] buffer)
        {
            _capacity = buffer.Length;
            _length = buffer.Length;

            _oArray = buffer;
            _iArray = new Type[_capacity];

            unsafe
            {
                _iBuffer = new IntPtr(Unsafe.AsPointer(ref _iArray[0]));
                _oBuffer = new IntPtr(Unsafe.AsPointer(ref _oArray[0]));
            }
        }

        #endregion


        #region IPolicySet

        [SecuritySafeCritical]
        public void Clear(Type policyInterface)
        {
            unsafe
            {
                var iPointer = _iBuffer.ToPointer();

                for (var i = _length - 1; 0 <= i; i--)
                {
                    var pointer = Unsafe.Add<Type>(iPointer, i);
                    var iCurrent = Unsafe.Read<Type>(pointer);

                    if (null == iCurrent || policyInterface != iCurrent) continue;

                    Unsafe.Write<Type>(pointer, null);
                    Unsafe.Write<object>(Unsafe.Add<object>(_oBuffer.ToPointer(), i), null);
                    return;
                }
            }
        }

        [SecuritySafeCritical]
        public object Get(Type policyInterface)
        {
            unsafe
            {
                var iPointer = _iBuffer.ToPointer();
                var oPointer = _oBuffer.ToPointer();

                for (var i = _length - 1; 0 <= i; i--)
                {
                    var iCurrent = Unsafe.Read<Type>(Unsafe.Add<Type>(iPointer, i));
                    var oCurrent = Unsafe.Read<object>(Unsafe.Add<object>(oPointer, i));

                    if (null != iCurrent)
                    {
                        if (policyInterface == iCurrent)
                            return oCurrent;
                    }
                    else
                    {
                        var policyInfo = policyInterface?.GetTypeInfo();
                        var objectInfo = oCurrent?.GetType().GetTypeInfo();
                        if (null != policyInfo && null != objectInfo &&
                            objectInfo.IsAssignableFrom(policyInfo))
                            return oCurrent;
                    }
                }
            }

            return null;
        }

        [SecuritySafeCritical]
        public void Set(Type policyInterface, object policy)
        {
            if (_capacity == _length)
            {
                _capacity += 5;

                var iNewArray = new Type[_capacity];
                var oNewArray = new object[_capacity];

                unsafe
                {
                    var iNewBuffer = new IntPtr(Unsafe.AsPointer(ref iNewArray[0]));
                    var oNewBuffer = new IntPtr(Unsafe.AsPointer(ref oNewArray[0]));

                    Unsafe.CopyBlock(iNewBuffer.ToPointer(), _iBuffer.ToPointer(), (uint)(_length * Unsafe.SizeOf<Type>()));
                    Unsafe.CopyBlock(oNewBuffer.ToPointer(), _oBuffer.ToPointer(), (uint)(_length * Unsafe.SizeOf<object>()));

                    _iArray = iNewArray;
                    _oArray = oNewArray;

                    _iBuffer = iNewBuffer;
                    _oBuffer = oNewBuffer;
                }
            }

            unsafe
            {
                Unsafe.Write(Unsafe.Add<object>(_oBuffer.ToPointer(), _length), policy);
                Unsafe.Write(Unsafe.Add<Type>(_iBuffer.ToPointer(), _length), policyInterface);
            }

            _length += 1;
        }

        #endregion
    }
}
