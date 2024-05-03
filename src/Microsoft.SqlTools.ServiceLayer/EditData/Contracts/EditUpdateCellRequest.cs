//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#nullable disable

using Microsoft.SqlTools.Hosting.Protocol.Contracts;

namespace Microsoft.SqlTools.ServiceLayer.EditData.Contracts
{
    public class EditUpdateCellRequest
    {
        public static readonly
            RequestType<EditUpdateCellParams, EditUpdateCellResult> Type =
            RequestType<EditUpdateCellParams, EditUpdateCellResult>.Create("edit/updateCell");
    }
}
