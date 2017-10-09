// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.Attributes
{
    /// <summary>
    /// This attribute is used to mark methods that should be called when
    /// the container is building an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InjectionMethodAttribute : Attribute
    {
    }
}
