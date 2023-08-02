//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.SqlTools.Hosting.Protocol.Contracts;

#nullable disable

namespace Microsoft.SqlTools.ServiceLayer.QueryStore.Contracts
{
    public class GetOverallResourceConsumptionReportParams : QueryStoreReportParams
    {

    }

    /// <summary>
    /// Gets the report for a Forced Plan Queries summary
    /// </summary>
    public class GetOverallResourceConsumptionReportRequest
    {
        public static readonly RequestType<GetOverallResourceConsumptionReportParams, QueryStoreQueryResult> Type
            = RequestType<GetOverallResourceConsumptionReportParams, QueryStoreQueryResult>.Create("queryStore/getOverallResourceConsumptionReport");
    }
}
