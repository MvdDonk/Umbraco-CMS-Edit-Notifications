﻿using System;
using System.Collections.Generic;
using Umbraco.Core.Events;
using Umbraco.Core.Persistence;

namespace Umbraco.Core.Scoping
{
    /// <summary>
    /// Implements <see cref="IScope"/> when there is no scope.
    /// </summary>
    internal class NoScope : IScope
    {
        private readonly ScopeProvider _scopeProvider;
        private bool _disposed;

        private UmbracoDatabase _database;
        private IList<EventMessage> _messages;

        public NoScope(ScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        /// <inheritdoc />
        public UmbracoDatabase Database
        {
            get
            {
                EnsureNotDisposed();
                return _database ?? (_database = _scopeProvider.DatabaseFactory.CreateNewDatabase());
            }
        }

        public UmbracoDatabase DatabaseOrNull
        {
            get
            {
                EnsureNotDisposed();
                return _database;
            }
        }

        /// <inheritdoc />
        public IList<EventMessage> Messages
        {
            get
            {
                EnsureNotDisposed();
                return _messages ?? (_messages = new List<EventMessage>());
            }
        }

        public IList<EventMessage> MessagesOrNull
        {
            get
            {
                EnsureNotDisposed();
                return _messages;
            }
        }

        /// <inheritdoc />
        public void Complete()
        {
            throw new NotImplementedException();
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("this");
        }

        public void Dispose()
        {
            EnsureNotDisposed();

            if (this != _scopeProvider.AmbientScope)
                throw new InvalidOperationException("Not the ambient scope.");

            if (_database != null)
                _database.Dispose();

            _scopeProvider.AmbientScope = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
