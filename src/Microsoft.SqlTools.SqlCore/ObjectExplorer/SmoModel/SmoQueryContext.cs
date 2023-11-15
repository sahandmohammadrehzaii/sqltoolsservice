﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlTools.Extensibility;
using Microsoft.SqlTools.SqlCore.Connection;
using Microsoft.SqlTools.SqlCore.ObjectExplorer.Nodes;

#nullable disable

namespace Microsoft.SqlTools.SqlCore.ObjectExplorer.SmoModel
{
    /// <summary>
    /// Context object containing key properties needed to query for SMO objects
    /// </summary>
    public class SmoQueryContext
    {
        private Server server;
        private Database database;
        private SmoObjectBase parent;
        private SmoWrapper smoWrapper;
        private ValidForFlag validFor = 0;

        /// <summary>
        /// Creates a context object with a server to use as the basis for any queries
        /// </summary>
        /// <param name="server"></param>
        public SmoQueryContext(Server server, IMultiServiceProvider serviceProvider, Func<bool> groupBySchemaFlag = null, SecurityToken token = null)
            : this(server, serviceProvider, null, groupBySchemaFlag, token)
        {
        }

        internal SmoQueryContext(Server server, IMultiServiceProvider serviceProvider, SmoWrapper serverManager, Func<bool> groupBySchemaFlag = null, SecurityToken token = null)
        {
            this.server = server;
            ServiceProvider = serviceProvider;
            this.smoWrapper = serverManager ?? new SmoWrapper();
            this.GroupBySchemaFlag = groupBySchemaFlag ?? new Func<bool>(() => false);
            if(token != null && !string.IsNullOrEmpty(token.Token))
            {
                UpdateAccessToken(token.Token);
            }
        }

        /// <summary>
        /// The server type
        /// </summary>
        public SqlServerType SqlServerType { get; set; }

        /// <summary>
        /// The server SMO will query against
        /// </summary>
        public Server Server
        {
            get
            {
                return GetObjectWithOpenedConnection(server);
            }
        }

        /// <summary>
        /// Optional Database context object to query against
        /// </summary>
        public Database Database
        {
            get
            {
                return GetObjectWithOpenedConnection(database);
            }
            set
            {
                database = value;
            }
        }

        /// <summary>
        /// Parent of a give node to use for queries
        /// </summary>
        public SmoObjectBase Parent
        {
            get
            {
                return GetObjectWithOpenedConnection(parent);
            }
            set
            {
                parent = value;
            }
        }

        /// <summary>
        /// Function that gets the group by schema 
        /// </summary>
        /// <value></value>
        public Func<bool> GroupBySchemaFlag { get; set; }
        
        /// <summary>
        /// Returns group by schema flag value.
        /// </summary>
        public bool GroupBySchema
        {
            get => GroupBySchemaFlag();
        }

        /// <summary>
        /// A query loader that can be used to find <see cref="SmoQuerier"/> objects
        /// for specific SMO types
        /// </summary>
        public IMultiServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Helper method to cast a parent to a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ParentAs<T>()
            where T : TreeNode
        {
            return Parent as T;
        }

        /// <summary>
        /// Copies the context for use by another node
        /// </summary>
        /// <param name="parent">New Parent to set</param>
        /// <returns>new <see cref="SmoQueryContext"/> with all fields except <see cref="Parent"/> the same</returns>
        public SmoQueryContext CopyWithParent(SmoObjectBase parent)
        {
            SmoQueryContext context = new SmoQueryContext(this.Server, this.ServiceProvider, this.smoWrapper, this.GroupBySchemaFlag)
            {
                database = this.Database,
                Parent = parent,
                SqlServerType = this.SqlServerType,
                ValidFor = ValidFor
            };
            return context;
        }

        /// <summary>
        /// Indicates which platforms the server and database is valid for
        /// </summary>
        public ValidForFlag ValidFor
        {
            get
            {
                if (validFor == 0)
                {
                    validFor = ServerVersionHelper.GetValidForFlag(SqlServerType, Database);
                }
                return validFor;
            }
            set
            {
                validFor = value;
            }
        }

        private T GetObjectWithOpenedConnection<T>(T smoObj)
            where T : SmoObjectBase
        {
            if (smoObj != null)
            {
                EnsureConnectionOpen(smoObj);
            }
            return smoObj;
        }

        /// <summary>
        /// Updates access token on parent connection context.
        /// </summary>
        /// <param name="accessToken">Acquired access token</param>
        public void UpdateAccessToken(string? accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Update all applicable nodes that could contain access token
                // to prevent stale token from being re-used.
                if (server != null)
                {
                    (server as SqlSmoObject).UpdateAccessToken(accessToken);
                }
                if (database != null)
                {
                    (database as SqlSmoObject).UpdateAccessToken(accessToken);
                }
                if (parent != null)
                {
                    (parent as SqlSmoObject).UpdateAccessToken(accessToken);
                }
            }
        }

        /// <summary>
        /// Ensures the server objects connection context is open. This is used by all child objects, and
        /// the only way to easily access is via the server object. This should be called during access of
        /// any of the object properties
        /// </summary>
        public void EnsureConnectionOpen(SmoObjectBase smoObj)
        {
            if (!smoWrapper.IsConnectionOpen(smoObj))
            {
                // We have a closed server connection. Reopen this
                // Note: not currently catching connection exceptions. Expect this to bubble
                // up to calling methods and be logged there as this would be happening there in any case
                smoWrapper.OpenConnection(smoObj);
            }
        }
    }
}
