﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.QueryStoreModel.Common;
using Microsoft.SqlServer.Management.QueryStoreModel.ForcedPlanQueries;
using Microsoft.SqlServer.Management.QueryStoreModel.HighVariation;
using Microsoft.SqlServer.Management.QueryStoreModel.OverallResourceConsumption;
using Microsoft.SqlServer.Management.QueryStoreModel.RegressedQueries;
using Microsoft.SqlServer.Management.QueryStoreModel.TopResourceConsumers;
using Microsoft.SqlServer.Management.QueryStoreModel.TrackedQueries;
using Microsoft.SqlTools.Hosting.Protocol;
using Microsoft.SqlTools.ServiceLayer.Connection;
using Microsoft.SqlTools.ServiceLayer.Hosting;
using Microsoft.SqlTools.ServiceLayer.QueryStore.Contracts;
using Microsoft.SqlTools.ServiceLayer.Utility;

namespace Microsoft.SqlTools.ServiceLayer.QueryStore
{
    /// <summary>
    /// Main class for SqlProjects service
    /// </summary>
    public sealed class QueryStoreService : BaseService
    {
        private static readonly Lazy<QueryStoreService> instance = new Lazy<QueryStoreService>(() => new QueryStoreService());

        /// <summary>
        /// Gets the singleton instance object
        /// </summary>
        public static QueryStoreService Instance => instance.Value;

        /// <summary>
        /// Instance of the connection service, used to get the connection info for a given owner URI
        /// </summary>
        private ConnectionService ConnectionService { get; }

        private QueryStoreService()
        {
            ConnectionService = ConnectionService.Instance;
        }

        internal QueryStoreService(ConnectionService connService)
        {
            ConnectionService = connService;
        }

        /// <summary>
        /// Initializes the service instance
        /// </summary>
        /// <param name="serviceHost"></param>
        public void InitializeService(ServiceHost serviceHost)
        {
            // Top Resource Consumers report
            serviceHost.SetRequestHandler(GetTopResourceConsumersSummaryRequest.Type, HandleGetTopResourceConsumersSummaryReportRequest, isParallelProcessingSupported: true);
            serviceHost.SetRequestHandler(GetTopResourceConsumersDetailedSummaryRequest.Type, HandleGetTopResourceConsumersDetailedSummaryReportRequest, isParallelProcessingSupported: true);
            serviceHost.SetRequestHandler(GetTopResourceConsumersDetailedSummaryWithWaitStatsRequest.Type, HandleGetTopResourceConsumersDetailedSummaryWithWaitStatsReportRequest, isParallelProcessingSupported: true);

            // Forced Plan Queries report
            serviceHost.SetRequestHandler(GetForcedPlanQueriesReportRequest.Type, HandleGetForcedPlanQueriesReportRequest, isParallelProcessingSupported: true);

            // Tracked Queries report
            serviceHost.SetRequestHandler(GetTrackedQueriesReportRequest.Type, HandleGetTrackedQueriesReportRequest, isParallelProcessingSupported: true);

            // High Variation Queries report
            serviceHost.SetRequestHandler(GetHighVariationQueriesSummaryRequest.Type, HandleGetHighVariationQueriesSummaryReportRequest, isParallelProcessingSupported: true);
            serviceHost.SetRequestHandler(GetHighVariationQueriesDetailedSummaryRequest.Type, HandleGetHighVariationQueriesDetailedSummaryReportRequest, isParallelProcessingSupported: true);
            serviceHost.SetRequestHandler(GetHighVariationQueriesDetailedSummaryWithWaitStatsRequest.Type, HandleGetHighVariationQueriesDetailedSummaryWithWaitStatsReportRequest, isParallelProcessingSupported: true);

            // Overall Resource Consumption report
            serviceHost.SetRequestHandler(GetOverallResourceConsumptionReportRequest.Type, HandleGetOverallResourceConsumptionReportRequest, isParallelProcessingSupported: true);

            // Regressed Queries report
            serviceHost.SetRequestHandler(GetRegressedQueriesSummaryRequest.Type, HandleGetRegressedQueriesSummaryReportRequest, isParallelProcessingSupported: true);
            serviceHost.SetRequestHandler(GetRegressedQueriesDetailedSummaryRequest.Type, HandleGetRegressedQueriesDetailedSummaryReportRequest, isParallelProcessingSupported: true);
        }

        #region Handlers

        /* 
         * General process is to:
         * 1. Convert the ADS config to the Query Store Model config format
         * 2. Call the unordered query generator to get the list of columns
         * 3. Select the intended ColumnInfo for sorting
         * 4. Call the ordered query generator to get the actual query
         * 5. Prepend any necessary TSQL parameters to the generated query
         * 6. Return the query text to ADS for execution
         */

        #region Top Resource Consumers report

        internal async Task HandleGetTopResourceConsumersSummaryReportRequest(GetTopResourceConsumersReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                TopResourceConsumersConfiguration config = new();
                TopResourceConsumersQueryGenerator.TopResourceConsumersSummary(config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = TopResourceConsumersQueryGenerator.TopResourceConsumersSummary(config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query,
                };
            }, requestContext);
        }

        internal async Task HandleGetTopResourceConsumersDetailedSummaryReportRequest(GetTopResourceConsumersReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                TopResourceConsumersConfiguration config = new();
                TopResourceConsumersQueryGenerator.TopResourceConsumersDetailedSummary(GetAvailableMetrics(requestParams), config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = TopResourceConsumersQueryGenerator.TopResourceConsumersDetailedSummary(GetAvailableMetrics(requestParams), config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        internal async Task HandleGetTopResourceConsumersDetailedSummaryWithWaitStatsReportRequest(GetTopResourceConsumersReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                TopResourceConsumersConfiguration config = new();
                TopResourceConsumersQueryGenerator.TopResourceConsumersDetailedSummaryWithWaitStats(GetAvailableMetrics(requestParams), config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = TopResourceConsumersQueryGenerator.TopResourceConsumersDetailedSummaryWithWaitStats(GetAvailableMetrics(requestParams), config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #region Forced Plans report

        internal async Task HandleGetForcedPlanQueriesReportRequest(GetForcedPlanQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                ForcedPlanQueriesConfiguration config = new();
                ForcedPlanQueriesQueryGenerator.ForcedPlanQueriesSummary(config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = ForcedPlanQueriesQueryGenerator.ForcedPlanQueriesSummary(config, orderByColumn, requestParams.Descending, out IList<ColumnInfo> _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #region Tracked Queries report

        internal async Task HandleGetTrackedQueriesReportRequest(GetTrackedQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                TrackedQueriesConfiguration config = new();
                string query = QueryIDSearchQueryGenerator.GetQuery();

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #region High Variation Queries report

        internal async Task HandleGetHighVariationQueriesSummaryReportRequest(GetHighVariationQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                HighVariationConfiguration config = new();
                HighVariationQueryGenerator.HighVariationSummary(config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = HighVariationQueryGenerator.HighVariationSummary(config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        internal async Task HandleGetHighVariationQueriesDetailedSummaryReportRequest(GetHighVariationQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                HighVariationConfiguration config = new();
                IList<Metric> availableMetrics = GetAvailableMetrics(requestParams);
                HighVariationQueryGenerator.HighVariationDetailedSummary(availableMetrics, config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = HighVariationQueryGenerator.HighVariationDetailedSummary(availableMetrics, config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        internal async Task HandleGetHighVariationQueriesDetailedSummaryWithWaitStatsReportRequest(GetHighVariationQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                HighVariationConfiguration config = new();
                IList<Metric> availableMetrics = GetAvailableMetrics(requestParams);
                HighVariationQueryGenerator.HighVariationDetailedSummaryWithWaitStats(availableMetrics, config, out IList<ColumnInfo> columns);
                ColumnInfo orderByColumn = GetOrderByColumn(requestParams, columns);
                string query = HighVariationQueryGenerator.HighVariationDetailedSummaryWithWaitStats(availableMetrics, config, orderByColumn, requestParams.Descending, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #region Overall Resource Consumption report

        internal async Task HandleGetOverallResourceConsumptionReportRequest(GetOverallResourceConsumptionReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                OverallResourceConsumptionConfiguration config = new();
                string query = OverallResourceConsumptionQueryGenerator.GenerateQuery(GetAvailableMetrics(requestParams), config, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #region Regressed Queries report

        internal async Task HandleGetRegressedQueriesSummaryReportRequest(GetRegressedQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                RegressedQueriesConfiguration config = new();
                string query = RegressedQueriesQueryGenerator.RegressedQuerySummary(config, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        internal async Task HandleGetRegressedQueriesDetailedSummaryReportRequest(GetRegressedQueriesReportParams requestParams, RequestContext<QueryStoreQueryResult> requestContext)
        {
            await RunWithErrorHandling(() =>
            {
                RegressedQueriesConfiguration config = new();
                string query = RegressedQueriesQueryGenerator.RegressedQueryDetailedSummary(GetAvailableMetrics(requestParams), config, out _);

                return new QueryStoreQueryResult()
                {
                    Success = true,
                    ErrorMessage = null,
                    Query = query
                };
            }, requestContext);
        }

        #endregion

        #endregion

        #region Helpers

        private ColumnInfo GetOrderByColumn(QueryStoreReportParams requestParams, IList<ColumnInfo> columnInfoList)
        {
            return requestParams.OrderByColumnId != null ? columnInfoList.First(col => col.GetQueryColumnLabel() == requestParams.OrderByColumnId) : columnInfoList[0];
        }

        private IList<Metric> GetAvailableMetrics(QueryStoreReportParams requestParams)
        {
            ConnectionService.TryFindConnection(requestParams.ConnectionOwnerUri, out ConnectionInfo connectionInfo);

            if (connectionInfo != null)
            {
                using (SqlConnection connection = ConnectionService.OpenSqlConnection(connectionInfo, "QueryStoreService available metrics"))
                {
                    return QdsMetadataMapper.GetAvailableMetrics(connection);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unable to find connection for '{requestParams.ConnectionOwnerUri}'");
            }
        }

        #endregion
    }
}
