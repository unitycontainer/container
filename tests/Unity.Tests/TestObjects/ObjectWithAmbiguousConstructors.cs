// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Tests.TestObjects
{
    public class ObjectWithAmbiguousConstructors
    {
        public const string One = "ObjectWithAmbiguousConstructors()";
        public const string Two = "ObjectWithAmbiguousConstructors(int,string,float)";
        public const string Three = "ObjectWithAmbiguousConstructors(string,string,int)";

        public string Signature { get; }

        public ObjectWithAmbiguousConstructors()
        {
            Signature = One;
        }

        public ObjectWithAmbiguousConstructors(int first, string second, float third)
        {
            Signature = Two;
        }

        public ObjectWithAmbiguousConstructors(string first, string second, int third)
        {
            Signature = Three;
        }
    }
}
