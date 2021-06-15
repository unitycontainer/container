using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Unity.Lifetime;

namespace Unity.Tests.v5.TestSupport
{
    [TypeConverter(typeof(SessionKeyTypeConverter))]
    public class SessionLifetimeManager : LifetimeManager
    {
        private readonly string sessionKey;
        public static string LastUsedSessionKey;

        public SessionLifetimeManager(string sessionKey)
        {
            this.sessionKey = sessionKey;
        }

        public string SessionKey
        {
            get { return this.sessionKey; }
        }

        public override object GetValue(ILifetimeContainer container = null)
        {
            LastUsedSessionKey = this.sessionKey;
            return null;
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            throw new NotImplementedException();
        }
    }

    public class SessionKeyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.GetType() == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new SessionLifetimeManager((string)value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return ((SessionLifetimeManager)value).SessionKey;
        }
    }

    public class ReversedSessionKeyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string key = Reverse((string)value);
            return new SessionLifetimeManager(key);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            string key = Reverse(((SessionLifetimeManager)value).SessionKey);
            return key;
        }

        private static string Reverse(IEnumerable<char> s)
        {
            var chars = new Stack<char>(s);
            return chars.JoinStrings(String.Empty);
        }
    }
}
