// -----------------------------------------------------------------------
// <copyright file="IOColEntity.cs" company="UAFC">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SSIS_Config_BLL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IOColEntity
    {
            public string iotype { get; set; }
            public string colID { get; set; }
            public string lineageID { get; set; }
            public string colName { get; set; }
            public string externalMetadataColumn { get; set; }
            public string component { get; set; }
            public IOColEntity(string iotype, string colID, string lineageID, string colname, string externalMetadataColumn, string component)
            {
                this.iotype = iotype;
                this.colID = colID;
                this.lineageID = lineageID;
                this.colName = colname;
                this.externalMetadataColumn = externalMetadataColumn;
                this.component = component;
            }

    }
}
