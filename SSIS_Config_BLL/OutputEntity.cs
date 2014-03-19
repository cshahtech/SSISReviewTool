using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSIS_Config_BLL
{
    public class OutputEntity
    {
        public string pkgFilePath { get; set; }
        public string pkgPassword { get; set; }
        public List<LogEntity> logEnt { get; set; }

        public bool modifySSIS { get; set; }
        public bool isSuccess { get; set; }
        public bool isPkgPwdProtected { get; set; }


        public OutputEntity()
    {
        this.pkgFilePath =string.Empty;
        this.logEnt =new List<LogEntity>();

        this.modifySSIS=false;
        this.isSuccess =false ;
        this.isPkgPwdProtected = false;
    }
    }


}
