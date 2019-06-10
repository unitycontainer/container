using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Registration;
using Unity.Storage;
using Unity.Tests.TestObjects;

namespace Unity.Tests.v5.Storage
{
    [TestClass]
    public class RegistrationSetTests
    {
        [TestMethod]
        public void ShouldHandleCollisions()
        {
            var (s1, s2) = MakeCollision();

            var registrationSet = new RegistrationSet();
            var registration1 = new InternalRegistration();
            var registration2 = new InternalRegistration();
            var registration3 = new InternalRegistration();

            registrationSet.Add(typeof(IService), s1, registration1);
            Assert.AreEqual(1, registrationSet.Count);
            registrationSet.Add(typeof(IService), s2, registration2);
            Assert.AreEqual(2, registrationSet.Count);
            registrationSet.Add(typeof(IService), s1, registration3);
            Assert.AreEqual(2, registrationSet.Count);
        }

        private static (string, string) MakeCollision()
        {
            var strings = new Dictionary<int, string>();
            var random = new Random();
            var size = 10;

            var builder = new StringBuilder(size);
            while (true)
            {
                for (var j = 0; j < size; j++)
                    builder.Append((char) random.Next('a', 'z' + 1));

                var str = builder.ToString();
                var hash = str.GetHashCode();
                if (strings.TryGetValue(hash, out var other))
                    return (str, other);

                strings[hash] = str;
                builder.Clear();
            }
        }

    }
}