﻿using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Constants

        public const float LoadFactor = 0.75f;
        public const float ReLoadFactor = 1.45f;

        protected const int START_DATA  = 4;
        protected const int START_INDEX = 1;
        protected const int HASH_CODE_SEED = 52361;

        #endregion


        #region Fields

        // Entire scope lock
        public readonly object SyncRoot;

        // Segment
        public readonly int Level;
        private readonly Scope[] _ancestry;

        // Contracts
        protected int Prime;
        protected int Index;
        protected int Revision;
        protected Entry[] Data;

        #endregion


        #region Constructors

        /// <summary>
        /// Root scope
        /// </summary>
        protected internal Scope(int capacity)
        {
            // Scope lock
            SyncRoot = new object();

            // Contracts
            Prime = Storage.Prime.IndexOf(capacity);
            Data = new Entry[Storage.Prime.Numbers[Prime++]];

            // Segment
            Disposables = new List<IDisposable>();
            Next  = null;
            Level = 0;
            _ancestry = new[] { this };
        }

        /// <summary>
        /// Child scope
        /// </summary>
        /// <param name="parent">parent scope</param>
        protected Scope(Scope parent, int capacity)
        {
            // Scope lock
            SyncRoot = new object();

            // Contracts
            Prime = Storage.Prime.IndexOf(capacity);
            Data = new Entry[Storage.Prime.Numbers[Prime++]];

            // Segment
            Disposables = new List<IDisposable>();
            Next  = parent;
            Level = parent.Level + 1;
            _ancestry = new Scope[parent._ancestry.Length + 1];
            _ancestry[parent._ancestry.Length] = this;
            Array.Copy(parent._ancestry, _ancestry, parent._ancestry.Length);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scope">Scope to copy from</param>
        /// <param name="sync">Synchronization root</param>
        protected internal Scope(Scope scope, object sync)
        {
            // Scope lock
            SyncRoot = sync;

            // Contracts
            Prime = scope.Prime;
            Index = scope.Index;
            Data = scope.Data;

            // Info
            Revision = scope.Revision;

            // Segment
            Disposables = scope.Disposables;
            Next  = scope.Next;
            Level = scope.Level;
            _ancestry = scope._ancestry;
            _ancestry[scope._ancestry.Length - 1] = this;
        }

        #endregion
    }
}
