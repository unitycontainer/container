using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Extension;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// This enumeration identifies type of registration 
    /// </summary>
    public enum RegistrationCategory
    {
        /// <summary>
        /// Initial, uninitialized state
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// Collection of cached metadata
        /// </summary>
        Cache,

        /// <summary>
        /// This registration is a clone
        /// </summary>
        /// <remarks>
        /// In most cases this category implies that
        /// the Data field holds reference to a parent
        /// manager
        /// </remarks>
        Clone,

        /// <summary>
        /// This is implicit/internal registration
        /// </summary>
        Internal,

        /// <summary>
        /// This is RegisterType registration
        /// </summary>
        Type,

        /// <summary>
        /// This is RegisterInstance registration
        /// </summary>
        Instance,

        /// <summary>
        /// This is RegisterFactory registration
        /// </summary>
        Factory
    }
}
