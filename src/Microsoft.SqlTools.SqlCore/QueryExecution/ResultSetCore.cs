//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.SqlTools.ServiceLayer.QueryExecution.Contracts;

namespace Microsoft.SqlTools.ServiceLayer.EditData
{
    public class ResultSetCore
    {       
        /// <summary>
        /// The columns for this result set
        /// </summary>
        public DbColumnWrapper[] Columns { get; private set; }
    }
}
