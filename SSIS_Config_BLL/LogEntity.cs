using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSIS_Config_BLL
{
    public class LogEntity
    {
        //public int ID { get; set; }
        //public bool isModify { get; set; }
        public string PackageName { get; set; }
        //public string analyst { get; set; }
        public string ComponentType { get; set; }
        public string ComponentValue { get; set; }
        public string TestStatus { get; set; }
        //public DateTime LogDateTime { get; set; }

        public LogEntity()
        { //this.isModify = false;
        }

        public LogEntity(string ppackageName)
        {
            this.PackageName = ppackageName;
            //this.isModify = false;
            
        }
    }
}
