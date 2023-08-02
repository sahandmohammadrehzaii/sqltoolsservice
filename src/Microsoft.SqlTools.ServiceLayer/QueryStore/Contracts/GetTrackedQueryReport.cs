//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.SqlTools.Hosting.Protocol.Contracts;

#nullable disable

namespace Microsoft.SqlTools.ServiceLayer.QueryStore.Contracts
{
    public class GetTrackedQueriesReportParams : QueryStoreReportParams
    {

    }

    /// <summary>
    /// Gets the report for a Forced Plan Queries summary
    /// </summary>
    public class GetTrackedQueriesReportRequest
    {
        public static readonly RequestType<GetTrackedQueriesReportParams, QueryStoreQueryResult> Type
            = RequestType<GetTrackedQueriesReportParams, QueryStoreQueryResult>.Create("queryStore/getTrackedQueriesReport");
    }
}
