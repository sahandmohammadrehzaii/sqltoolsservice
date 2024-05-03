//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#nullable disable

namespace Microsoft.SqlTools.ServiceLayer.EditData.Contracts
{
    /// <summary>
    /// Parameters for the cell revert request
    /// </summary>
    public class EditRevertCellParams : RowOperationParams
    {
        public int ColumnId { get; set; }
    }

    /// <summary>
    /// Parameters to return upon successful revert of the cell
    /// </summary>
    public class EditRevertCellResult : EditCellResult
    {
    }
}
