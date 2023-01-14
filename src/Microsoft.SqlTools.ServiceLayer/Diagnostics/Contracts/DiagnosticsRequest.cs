//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using Microsoft.SqlTools.Hosting.Protocol.Contracts;

namespace Microsoft.SqlTools.ServiceLayer.Diagnostics.Contracts
{
    /// <summary>
    /// A request to check if an error can be handled by the code or not.
    /// </summary>
    public class DiagnosticsRequest
    {
        public static readonly
            RequestType<DiagnosticsParams, DiagnosticsResponse> Type =
            RequestType<DiagnosticsParams, DiagnosticsResponse>.Create("diagnostics/errorHandler");
    }

    public class DiagnosticsParams
    {
        /// <summary>
        /// The error code used to defined the error type
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// The error message associated with the error.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    public class DiagnosticsResponse {
         /// <summary>
        /// The error message associated with the error.
        /// </summary>
        public string ErrorAction { get; set; }
    }
}