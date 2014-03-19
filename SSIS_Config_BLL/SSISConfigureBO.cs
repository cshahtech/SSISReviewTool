using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Linq;
using Microsoft.SqlServer.Dts.Runtime;
using System.IO;
using Microsoft.SqlServer.Dts.Pipeline;


namespace SSIS_Config_BLL
{
    public class SSISConfigureBO
    {
        #region VariableDeclaration
        enum ComponentType
        {
            UserVariable,
            SSISConfiguration,
            OLEDBConnection,
            EnvironmentVariable,
            DTSConfigPath
        };
        enum testStatus
        {
            Missing,
            Present,
            Added,
            FailedToAdd,
            True,
            False
        };
        #endregion

        public void CheckModifySSIS(ref OutputEntity rObj, string SSISVarXMLPath)
        {
            Application app = null;
            Package pkg = null;
            List<LogEntity> listLogObj =rObj.logEnt;
            try
            {
                app = new Application();
                pkg = new Package();
                if((rObj.isPkgPwdProtected ==true)&&(rObj.pkgPassword!=""))
                {
                    app.PackagePassword = rObj.pkgPassword;
                }

                pkg = app.LoadPackage(rObj.pkgFilePath, null);

            }
            catch (Microsoft.SqlServer.Dts.Runtime.DtsRuntimeException e)
            {
                if (((Microsoft.SqlServer.Dts.Runtime.DtsException)(e)).ErrorCode == -1073659849)
                {
                    rObj.isPkgPwdProtected = true;
                    return;
                }
                else
                {
                    throw e;
                }
            }
            catch (Exception e)
            { 
                throw e;
            }

                Check_LoggingOption(rObj.modifySSIS, pkg, listLogObj);

                CheckModify_SSISControlFlowExecutables(rObj.modifySSIS, rObj.pkgFilePath,pkg, listLogObj,"DelayValidation",true );

                CheckModify_SSISControlFlowExecutables(rObj.modifySSIS, rObj.pkgFilePath,pkg, listLogObj, "LoggingMode", "Enabled");

                CheckModify_SSISVariables(rObj.modifySSIS, pkg, listLogObj, SSISVarXMLPath);
                           
                CheckModify_PkgConfigEnabled(rObj.modifySSIS, pkg, listLogObj);

                CheckModify_ConfigDB(rObj.modifySSIS, pkg, listLogObj);

                CheckModify_OLEDBConn(rObj.modifySSIS, pkg, listLogObj);

                CheckModify_NonOLEDBConn(rObj.modifySSIS, pkg, listLogObj);

                if (rObj.modifySSIS)
                {
                    app.SaveToXml(rObj.pkgFilePath, pkg, null);
                }


                rObj.isSuccess = true;
            
                app = null;
                pkg = null;


                return ;
        }

        private static void CheckModify_SSISControlFlowExecutables(bool modifySSIS, string pkgFilePath,Package pkg, List<LogEntity> listLogObj, string PropName, object PropValue)
        {

            
            for(int i=0;i<pkg.Executables.Count;i++)
            {
                CheckModify_SSISComponents(modifySSIS, pkg.Executables[i], listLogObj,pkgFilePath,pkg, PropName,  PropValue );
            }
         }

        private static void CheckModify_SSISComponents(bool modifySSIS, Executable pkgExecutable, List<LogEntity> listLogObj, string pkgFilePath,Package pkg, string PropName, object PropValue)
        {
            LogEntity mLogEntity = new LogEntity(pkg.Name);
            mLogEntity.ComponentValue =PropName;
                

                string ExecutableType=pkgExecutable.GetType().FullName.ToString();
                string ExtypeVal=string.Empty;


                switch (ExecutableType)
                {
                    case "Microsoft.SqlServer.Dts.Runtime.Sequence":
                    var objSequence = (Microsoft.SqlServer.Dts.Runtime.Sequence)(pkgExecutable);

                    for (int j = 0; j < objSequence.Executables.Count; j++)
                    {
                        CheckModify_SSISComponents(modifySSIS, objSequence.Executables[j], listLogObj,pkgFilePath,pkg,PropName,PropValue );
                    }
                       
                    mLogEntity.ComponentType = (objSequence).Properties["Name"].GetValue((objSequence)).ToString();
                    mLogEntity.TestStatus = objSequence.Properties [PropName].GetValue(objSequence).ToString();
                    if (modifySSIS)
                    {
                        (objSequence).Properties[PropName].SetValue(objSequence,PropValue);
                        mLogEntity.TestStatus = PropValue.ToString();

                    }

                        if (PropName == "LoggingMode")
                        {

                            if (objSequence.LoggingOptions.EventFilter.Length == 0)
                            {
                                LogEntity mLogEntity1 = new LogEntity(pkg.Name);
                                mLogEntity1.ComponentType  = (objSequence).Properties["Name"].GetValue((objSequence)).ToString();
                                mLogEntity1.ComponentValue = "Logging-Log Events";
                                mLogEntity1.TestStatus = "Failed";

                                listLogObj.Add(mLogEntity1);
                                
                            }
                        
                        }
                        break;

                    case "Microsoft.SqlServer.Dts.Runtime.TaskHost":
                        TaskHost objTaskHost = (Microsoft.SqlServer.Dts.Runtime.TaskHost)(pkgExecutable);

                        mLogEntity.ComponentType = (objTaskHost).Properties["Name"].GetValue((objTaskHost)).ToString();
                        mLogEntity.TestStatus = objTaskHost.Properties[PropName].GetValue((objTaskHost)).ToString();

                        if (modifySSIS)
                        {
                            (objTaskHost).Properties[PropName].SetValue((objTaskHost), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }

                        //To check if the TaskHost is Data flow Task
                        if (objTaskHost.InnerObject is Microsoft.SqlServer.Dts.Pipeline.Wrapper.MainPipe)
                        {
                            CheckModify_DataFlowTask(modifySSIS, objTaskHost, listLogObj, pkgFilePath,pkg.Name, PropName, PropValue);
                        }


                        if (PropName == "LoggingMode")
                        {
                            
                            objTaskHost.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            if (objTaskHost.LoggingOptions.EventFilter.Length == 0)
                            {
                                LogEntity mLogEntity1 = new LogEntity(pkg.Name);
                                mLogEntity1.ComponentType = (objTaskHost).Properties["Name"].GetValue((objTaskHost)).ToString();
                                mLogEntity1.ComponentValue = "Logging-Log Events";
                                mLogEntity1.TestStatus = "Failed";

                                listLogObj.Add(mLogEntity1);
                             
                            }
                            
                        }
                        break;

                    case "Microsoft.SqlServer.Dts.Runtime.ForLoop":
                        ForLoop objForLoop= (Microsoft.SqlServer.Dts.Runtime.ForLoop)(pkgExecutable);

                        for (int j = 0; j < objForLoop.Executables.Count; j++)
                        {
                            CheckModify_SSISComponents(modifySSIS, objForLoop.Executables[j], listLogObj,pkgFilePath, pkg, PropName, PropValue);
                        }

                        mLogEntity.ComponentType = (objForLoop).Properties["Name"].GetValue((objForLoop)).ToString();
                        mLogEntity.TestStatus = (objForLoop).Properties[PropName].GetValue((objForLoop)).ToString();
                        if (modifySSIS)
                        {
                            (objForLoop).Properties[PropName].SetValue((objForLoop), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }

                        if (PropName == "LoggingMode")
                        {

                            objForLoop.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            if (objForLoop.LoggingOptions.EventFilter.Length == 0)
                            {
                                LogEntity mLogEntity1 = new LogEntity(pkg.Name);
                                mLogEntity1.ComponentType  = (objForLoop).Properties["Name"].GetValue((objForLoop)).ToString();
                                mLogEntity1.ComponentValue = "Logging-Log Events";
                                mLogEntity1.TestStatus = "Failed";

                                listLogObj.Add(mLogEntity1);
                             }
                            //pkg.LoggingOptions.EventFilterKind = DTSEventFilterKind.Inclusion;
                            //pkg.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            //pkg.LoggingMode = DTSLoggingMode.Enabled;

                        }
                        
                        break;


                    case "Microsoft.SqlServer.Dts.Runtime.ForEachLoop":
                        ForEachLoop objForEachLoop = (Microsoft.SqlServer.Dts.Runtime.ForEachLoop)(pkgExecutable);

                        for (int j = 0; j < objForEachLoop.Executables.Count; j++)
                        {
                            CheckModify_SSISComponents(modifySSIS, objForEachLoop.Executables[j], listLogObj, pkgFilePath,pkg, PropName, PropValue);
                        }

                        mLogEntity.ComponentType = (objForEachLoop).Properties["Name"].GetValue((objForEachLoop)).ToString();
                        mLogEntity.TestStatus = (objForEachLoop).Properties[PropName].GetValue((objForEachLoop)).ToString();
                        if (modifySSIS)
                        {
                            (objForEachLoop).Properties[PropName].SetValue((objForEachLoop), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }

                        if (PropName == "LoggingMode")
                        {

                            objForEachLoop.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            if (objForEachLoop.LoggingOptions.EventFilter.Length == 0)
                            {
                                LogEntity mLogEntity1 = new LogEntity(pkg.Name);
                                mLogEntity1.ComponentValue = "Logging-Log Events";
                                mLogEntity1.ComponentType = (objForEachLoop).Properties["Name"].GetValue((objForEachLoop)).ToString();
                                mLogEntity1.TestStatus = "Failed";

                                listLogObj.Add(mLogEntity1);
                                
                            }
                            
                        }
                        
                        break;


                    case " Microsoft.SqlServer.Dts.Runtime.Package":
                        Package objPackage =(Microsoft.SqlServer.Dts.Runtime.Package)(pkgExecutable);

                        mLogEntity.ComponentType = (objPackage).Properties["Name"].GetValue((objPackage)).ToString();
                        mLogEntity.TestStatus = (objPackage).Properties[PropName].GetValue((objPackage)).ToString();
                        
                        if (modifySSIS)
                        {
                            (objPackage).Properties[PropName].SetValue((objPackage), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }

                        if (PropName == "LoggingMode")
                        {

                            objPackage.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            if (objPackage.LoggingOptions.EventFilter.Length == 0)
                            {
                                LogEntity mLogEntity1 = new LogEntity(pkg.Name);
                                mLogEntity1.ComponentValue = (objPackage).Properties["Name"].GetValue((objPackage)).ToString();
                                mLogEntity1.ComponentType = "Log Events";
                                mLogEntity1.TestStatus = "Failed";

                                listLogObj.Add(mLogEntity1);
                            
                            }
                            
                        }
                        

                        break;


                    default:
                        break;
                }
            //This will ensure that the only records where LoggingMode is Disabled will be displayed.
                if (!(PropName == "LoggingMode" && (mLogEntity.TestStatus == "0" || mLogEntity.TestStatus == "1")))
                {
                    listLogObj.Add(mLogEntity);
                    mLogEntity = null;
                }
        }

        private static void CheckModify_DataFlowTask(bool modifySSIS, TaskHost objTaskHost, List<LogEntity> listLogObj, string pkgFilePath, string pkgName, string PropName, object PropValue)
        {
            MainPipe dataFlowTask = (MainPipe)objTaskHost.InnerObject;

            List<IOColEntity> lstIOColEntity = new List<IOColEntity>();

            foreach (IDTSComponentMetaData100 comp in dataFlowTask.ComponentMetaDataCollection)
            {
                LogEntity mLogEntity = new LogEntity();
                
                mLogEntity.PackageName = pkgName;
                mLogEntity.ComponentType = "DF-" + objTaskHost.Name + "-" + comp.Name;
                mLogEntity.ComponentValue = "ValidateExternalMetadata";
                mLogEntity.TestStatus = comp.ValidateExternalMetadata.ToString();

                listLogObj.Add(mLogEntity);
                               
                
                try
                {
                    IDTSInput100 input = comp.InputCollection[0];
                
                    for (int col = 0; col < input.InputColumnCollection.Count; col++)
                    {
                        IDTSInputColumn100 Column = input.InputColumnCollection[col];
                        IOColEntity mIOColEntity = new IOColEntity("input"
                                                    , Convert.ToString(Column.ID)
                                                    , Convert.ToString(Column.LineageID)
                                                    , Convert.ToString(Column.Name)
                                                    , Convert.ToString(Column.ExternalMetadataColumnID)
                                                    , Convert.ToString(comp.Name));
                            
                        lstIOColEntity.Add(mIOColEntity);
                        //inputColumns[col] = BufferManager.FindColumnByLineageID(input.Buffer, inputColumn.LineageID);

                    }

                    IDTSOutput100 output = comp.OutputCollection[0];
                    for (int col = 0; col < output.OutputColumnCollection.Count; col++)
                    {
                        IDTSOutputColumn100 Column = output.OutputColumnCollection[col];
                        IOColEntity mIOColEntity =  new IOColEntity("output"
                                                    , Convert.ToString(Column.ID)
                                                    , Convert.ToString(Column.LineageID)
                                                    , Convert.ToString(Column.Name)
                                                    , Convert.ToString(Column.ExternalMetadataColumnID)
                                                    , Convert.ToString(comp.Name));
                        lstIOColEntity.Add(mIOColEntity);
                        //outputColumns[col] = BufferManager.FindColumnByLineageID(input.Buffer, outputColumn.LineageID);
                    }

                }
                catch (Exception e)
                { }
               
            }//End of For Each


            ReadOPCOlsXML(pkgFilePath, lstIOColEntity);

            var externalMetadataColumn = (from o in lstIOColEntity
                                          where o.iotype == "externalmetadata"
                                          select o).Distinct();

            
            //finding the mismatched columns here
            //Fetching the list of all the input columns
            var ipColumns = (from mIOColEntity in lstIOColEntity
                                where mIOColEntity.iotype == "input"
                                select mIOColEntity).Distinct();

            //Fetching the list of all the output columns
            var opColumns = (from o in lstIOColEntity
                                where o.iotype == "output" //&& o.externalMetadataColumn != "0"
                                select o).Distinct();

            var MatchedColumns1 = (from o in opColumns
                                  join i in ipColumns on o.colID equals i.lineageID
                                  where o.iotype == "output"
                                  select o).Distinct();

            var MatchedColumns = (from e in externalMetadataColumn join i in ipColumns on e.colID equals i.lineageID
                                    select e).Distinct();

            

            var opColumnsUnmatched1 = opColumns.Except(MatchedColumns1);
            var opColumnsUnmatched = opColumnsUnmatched1.Except(MatchedColumns);

            foreach (var unmatchedcols in opColumnsUnmatched)
            {
                LogEntity mLogEntity = new LogEntity(pkgName);
                mLogEntity.ComponentType = "Unmapped Column : " + unmatchedcols.component ;
                mLogEntity.ComponentValue = unmatchedcols.colName;
                mLogEntity.TestStatus = testStatus.Missing.ToString();

                listLogObj.Add(mLogEntity);
            }

        }

        private static void ReadOPCOlsXML(string pkgFilePath, List<IOColEntity> lstIOColEntity)
        {
            //Read the externamMetadata for unmapped columns
            XmlDocument doc = new XmlDocument();
            doc.Load(pkgFilePath);
            foreach (XmlNode node in doc.SelectNodes("//externalMetadataColumn[@id!= '']"))
            {

                IOColEntity newcol = new IOColEntity("externalmetadata"
                                                , Convert.ToString(node.Attributes["id"].Value)
                                                , "0000"
                                                , Convert.ToString(node.Attributes["name"].Value)
                                                , "0000"
                                                , "");
                lstIOColEntity.Add(newcol);
            }
        }

        private static void Check_LoggingOption(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {
            
            LogEntity mLogEntity = new LogEntity(pkg.Name);
                mLogEntity.ComponentType = "SSIS_Logging Option";
                mLogEntity.TestStatus = "Missing";

                for (int i = 0; i < pkg.LogProviders.Count; i++)
                {

                    if (!(pkg.LogProviders[i].ConfigString == "SSIS_Logging"))
                    {
                        mLogEntity.TestStatus = "Present";
                        if (modifySSIS)
                        {

                            LogProvider provider = pkg.LogProviders.Add("DTS.LogProviderSQLServer.1");
                            provider.Name = "SSIS log provider for SQL Server";
                            provider.ConfigString = "SSIS_Logging";
                            pkg.LoggingOptions.SelectedLogProviders.Add(provider);
                            pkg.LoggingOptions.EventFilterKind = DTSEventFilterKind.Inclusion;
                            pkg.LoggingOptions.EventFilter = pkg.LoggingOptions.EventFilter;
                            pkg.LoggingMode = DTSLoggingMode.Enabled;

                            mLogEntity.TestStatus = "Added";
                        }
                    }
                }
                listLogObj.Add(mLogEntity);

            //Package Level Logging Option
                mLogEntity = new LogEntity(pkg.Name);
                mLogEntity.ComponentType = "SSIS_Logging Property of Pkg";
                mLogEntity.TestStatus = pkg.LoggingMode.ToString();
                listLogObj.Add(mLogEntity); 
        }

        private static void CheckModify_SSISVariables(bool modifySSIS, Package pkg, List<LogEntity> listLogObj, string SSISVarXMLPath)
        {
            
            if (File.Exists(SSISVarXMLPath))
            {
                Variables pkgVars = pkg.Variables;
                List<SSISVariable> ListVariable = new List<SSISVariable>();
                getVariables_XML(ref ListVariable, SSISVarXMLPath);

                LogEntity mLogEntity = null;
                foreach (SSISVariable pkgVar in ListVariable)
                {
                    String pkgVarT = pkgVar.vName.Trim();
                    mLogEntity = new LogEntity(pkg.Name);
                    mLogEntity.ComponentType = ComponentType.UserVariable.ToString();
                    mLogEntity.ComponentValue = pkgVarT;

                    if (!(pkg.Variables.Contains(pkgVarT)))
                    {

                        mLogEntity.TestStatus = testStatus.Missing.ToString();
                        if (modifySSIS)
                        {
                            switch (pkgVar.vDatatype.ToLower())
                            {
                                case "boolean":
                                    bool b = new bool();
                                    if (pkgVar.vDefault.ToString().ToLower() == "true")
                                    {
                                        b = true;
                                    }
                                    else if (pkgVar.vDefault.ToString().ToLower() == "false")
                                    {
                                        b = false;
                                    }
                                    pkg.Variables.Add((pkgVarT), false, "User", b);
                                    break;

                                case "char":
                                    char c = new char();
                                    c = Convert.ToChar(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", c);
                                    break;

                                case "datetime":
                                    DateTime d = new DateTime();
                                    d = Convert.ToDateTime(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", d);
                                    break;

                                case "string":
                                    string s = String.Empty;
                                    s = Convert.ToString(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", s);
                                    break;

                                case "int16":
                                    Int16 i16 = 0;
                                    i16 = Convert.ToInt16(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", i16);
                                    break;


                                case "int32":
                                    Int32 i32 = 0;
                                    i32 = Convert.ToInt32(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", i32);
                                    break;


                                case "int64":
                                    Int64 i64 = 0;
                                    i64 = Convert.ToInt64(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", i64);
                                    break;

                                case "Uint32":
                                    UInt32 u32 = 0;
                                    u32 = Convert.ToUInt32(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", u32);
                                    break;


                                case "uint64":
                                    UInt64 u64 = 0;
                                    u64 = Convert.ToUInt64(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", u64);
                                    break;

                                case "single":
                                    Single single;
                                    single = Convert.ToSingle(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", single);
                                    break;

                                case "double":
                                    double db = 0.0;
                                    db = Convert.ToDouble(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", db);
                                    break;

                                case "object":
                                    object ob = new object();
                                    ob = (pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", ob);
                                    break;

                                case "sbyte":
                                    SByte sb = new SByte();
                                    sb = Convert.ToSByte(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", sb);
                                    break;

                                case "dbnull":
                                    pkg.Variables.Add((pkgVarT), false, "User", null);
                                    break;

                                case "dbbyte":
                                    Byte vbyte = new Byte();
                                    vbyte = Convert.ToByte(pkgVar.vDefault);
                                    pkg.Variables.Add((pkgVarT), false, "User", vbyte);

                                    break;

                                default:
                                    //pkg.Variables.Add((pkgVarT), false, "User", String.Empty);
                                    mLogEntity.TestStatus = "Unable to read datatype";
                                    break;
                            }


                            mLogEntity.TestStatus = testStatus.Added.ToString();
                        }
                    }
                    else
                    {
                        mLogEntity.TestStatus = testStatus.Present.ToString();
                    }
                    listLogObj.Add(mLogEntity);
                    mLogEntity = null;
                }

            }
        }

        private  static void getVariables_XML(ref List<SSISVariable> ListVariable,string SSISVarXMLPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(SSISVarXMLPath);

            
            XmlNode SSISVariables = xmlDoc.FirstChild;
            for (int i = 0; i < SSISVariables.ChildNodes.Count; i++)
            {
                SSISVariable var = new SSISVariable();

                XmlNode variableNode = SSISVariables.ChildNodes[i];

                var.vName = variableNode["name"].InnerText;
                var.vDatatype = variableNode["datatype"].InnerText;
                var.vDefault = variableNode["default"].InnerText;
                ListVariable.Add(var);
            }
        }
    
        private static void CheckModify_PkgConfigEnabled(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {
            LogEntity mLogEntity = new LogEntity(pkg.Name);
            mLogEntity.ComponentType = ComponentType.SSISConfiguration.ToString();
            mLogEntity.ComponentValue = "Package Configuration Enabled";
            mLogEntity.TestStatus = pkg.EnableConfigurations.ToString();
            //mLogEntity.PackageName = pkg.Name;

            if (modifySSIS)
            {
                pkg.EnableConfigurations = true;
                mLogEntity.TestStatus = pkg.EnableConfigurations.ToString();
            }
            listLogObj.Add(mLogEntity);
        }

        private void CheckModify_ConfigDB(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {
            LogEntity mLogEntity = new LogEntity(pkg.Name);
            mLogEntity.ComponentType = ComponentType.SSISConfiguration.ToString();
            mLogEntity.ComponentValue = "Config_Database";
            //mLogEntity.PackageName = pkg.Name;


            bool isSSIS_Config = false;
            for (int i = 0; i < pkg.Configurations.Count; i++)
            {
                if (pkg.Configurations[i].ConfigurationString == "SSIS_Config")
                {
                    mLogEntity.TestStatus = testStatus.Present.ToString();
                    isSSIS_Config = true;
                    break;
                }
            }


            if (!isSSIS_Config)
            {
                mLogEntity.TestStatus = testStatus.Missing.ToString();

                if (modifySSIS)
                {
                    mLogEntity.TestStatus = testStatus.Added.ToString();
                    Microsoft.SqlServer.Dts.Runtime.Configuration config = pkg.Configurations.Add();
                    config.Name = "SSIS_Config";
                    config.ConfigurationType = DTSConfigurationType.IConfigFile;

                    String path = "SSIS_Config";
                    if (path != null)
                    {
                        config.ConfigurationString = path;
                    }
                }
            }
            listLogObj.Add(mLogEntity);
            mLogEntity = null;
            
            
            mLogEntity = new LogEntity();
            mLogEntity.ComponentType = ComponentType.OLEDBConnection.ToString();
            mLogEntity.ComponentValue = "Config_Database";
            mLogEntity.PackageName = pkg.Name;

            Connections pkgConns = pkg.Connections;
            if (!pkg.Connections.Contains("Config_Database"))
            {
                mLogEntity.TestStatus = testStatus.Missing.ToString();
                if (modifySSIS)
                {
                    mLogEntity.TestStatus = testStatus.Added.ToString();
                    CreateConnection myOLEDBConn = new CreateConnection();
                    myOLEDBConn.CreateOLEDBConnection(pkg);
                }
            }
            else
            {
                mLogEntity.TestStatus = testStatus.Present.ToString();
            }
            listLogObj.Add(mLogEntity);
            mLogEntity = null;
        }
        
        private static void CheckModify_OLEDBConn(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {
            if (ConfigurationManager.AppSettings[pkg.Name] != null)
            {
                string pkgOLEDBConn = ConfigurationManager.AppSettings[pkg.Name];
                string[] pkgOLEDBConnList = pkgOLEDBConn.Split(',');

                foreach (String pkgOLEDB in pkgOLEDBConnList)
                {
                    String pkgOLEDBT = pkgOLEDB.Trim();
                    LogEntity mLogEntity = new LogEntity(pkg.Name);
                    bool isDBConn = pkg.Configurations.Contains(pkgOLEDBT);

                    mLogEntity.ComponentType = ComponentType.SSISConfiguration.ToString();
                    mLogEntity.ComponentValue = pkgOLEDBT;
                    //mLogEntity.PackageName = pkg.Name;

                    if (!isDBConn)
                    {
                        mLogEntity.TestStatus = testStatus.Missing.ToString();
                        if (modifySSIS)
                        {
                            mLogEntity.TestStatus = testStatus.Added.ToString();
                            Microsoft.SqlServer.Dts.Runtime.Configuration config = pkg.Configurations.Add();
                            config.Name = pkgOLEDBT;
                            config.ConfigurationType = DTSConfigurationType.SqlServer;
                            config.ConfigurationString = "Config_Database;[dbo].[SSIS_Configurations];" + pkgOLEDBT + ";";
                        }
                    }
                    else
                    {
                        mLogEntity.TestStatus = testStatus.Present.ToString();
                    }
                    listLogObj.Add(mLogEntity);
                    mLogEntity = null;

                    mLogEntity = new LogEntity(pkg.Name);
                    mLogEntity.ComponentType = ComponentType.OLEDBConnection.ToString();
                    mLogEntity.ComponentValue = pkgOLEDBT;
                    //mLogEntity.PackageName = pkg.Name;

                    if (!pkg.Connections.Contains(pkgOLEDBT))
                    {
                        mLogEntity.TestStatus = testStatus.Missing.ToString();
                        if (modifySSIS)
                        {
                            mLogEntity.TestStatus = testStatus.Added.ToString();
                            CreateConnection myOLEDBConn = new CreateConnection();
                            myOLEDBConn.CreateOLEDBConnection(pkg, pkgOLEDBT);
                        }
                    }
                    else
                    {
                        mLogEntity.TestStatus = testStatus.Present.ToString();
                    }
                    listLogObj.Add(mLogEntity);
                    mLogEntity = null;
                }
            }
        }
    
        private static void CheckModify_NonOLEDBConn(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {

        LogEntity mLogEntity =null;
                    
                    for(int i=0;i<pkg.Connections.Count;i++)
                    {
                        if (pkg.Connections[i].CreationName.ToString() != "OLEDB")
                        {

                        mLogEntity = new LogEntity(pkg.Name);
                        mLogEntity.ComponentType = pkg.Connections[i].CreationName.ToString() + "-ConnStringExpression";
                        mLogEntity.ComponentValue = pkg.Connections[i].Name.ToString();
                        
                        String conn=pkg.Connections[i].GetExpression("ConnectionString");
                        if (conn != null)
                        {
                            mLogEntity.TestStatus = testStatus.Present.ToString();
                        }
                        else
                        {
                            mLogEntity.TestStatus = testStatus.Missing.ToString();
                        }

                        
                        listLogObj.Add(mLogEntity);
                        //mLogEntity = null;

                        //Checking the delay Validation property of Non-OLEDB Connections

                        mLogEntity = new LogEntity(pkg.Name);
                        mLogEntity.ComponentType = pkg.Connections[i].CreationName.ToString() + "-DelayValidation";
                        mLogEntity.ComponentValue = pkg.Connections[i].Name.ToString();

                        String delayVal = Convert.ToString(pkg.Connections[i].DelayValidation);
                        if (delayVal != null)
                        {
                            mLogEntity.TestStatus = delayVal;
                        }
                        else
                        {
                            mLogEntity.TestStatus = delayVal;
                        }


                        listLogObj.Add(mLogEntity);

                        }
                    }
                    
                }

        /*private static void CheckModify_SSISDATAFlowTaskProp(bool modifySSIS, Package pkg, List<LogEntity> listLogObj)
        {
            bool PropValue = true;
            for (int i = 0; i < pkg.Executables.Count; i++)
            {
                LogEntity mLogEntity = new LogEntity(pkg.Name);
                mLogEntity.ComponentType = "DTSProp-DelayValidation";
                string Extype = pkg.Executables[i].GetType().FullName.ToString();
                string ExtypeVal = string.Empty;


                switch (Extype)
                {
                    case "Microsoft.SqlServer.Dts.Runtime.Sequence":
                        ExtypeVal = ((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i])).Properties[PropName].GetValue(((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i]))).ToString();
                        mLogEntity.ComponentValue = ((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i])).Properties["Name"].GetValue(((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i]))).ToString();
                        mLogEntity.TestStatus = ExtypeVal.ToString();
                        if (modifySSIS)
                        {
                            ((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i])).Properties[PropName].SetValue(((Microsoft.SqlServer.Dts.Runtime.Sequence)(pkg.Executables[i])), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();

                        }
                        break;

                    case "Microsoft.SqlServer.Dts.Runtime.TaskHost":
                        TaskHost objTaskHost =objTaskHost;

                        ExtypeVal = objTaskHost.Properties[PropName].GetValue((objTaskHost)).ToString();
                        mLogEntity.ComponentValue = (objTaskHost).Properties["Name"].GetValue((objTaskHost)).ToString();
                        mLogEntity.TestStatus = ExtypeVal.ToString();

                        if (modifySSIS)
                        {
                            (objTaskHost).Properties[PropName].SetValue((objTaskHost), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }

                        //Check if the task host is a dataflow task
                        if (objTaskHost.InnerObject is Microsoft.SqlServer.Dts.Pipeline.Wrapper.MainPipe)
                        {

                            //Cast the Executable as a data flow

                            MainPipe pipe = (MainPipe)objTaskHost.InnerObject;

                            foreach (IDTSComponentMetaData90 comp in pipe.ComponentMetaDataCollection)
                            {

                                Console.WriteLine("  Component Name = " + comp.Name);

                            }

                        }



                        break;
                    case "Microsoft.SqlServer.Dts.Runtime.ForLoop":
                        ExtypeVal = ((Microsoft.SqlServer.Dts.Runtime.ForLoop)(pkg.Executables[i])).Properties[PropName].GetValue((objForLoop)).ToString();
                        mLogEntity.ComponentValue = (objForLoop).Properties["Name"].GetValue((objForLoop)).ToString();
                        mLogEntity.TestStatus = ExtypeVal.ToString();
                        if (modifySSIS)
                        {
                            (objForLoop).Properties[PropName].SetValue((objForLoop), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }
                        break;

                    case "Microsoft.SqlServer.Dts.Runtime.ForEachLoop":
                        ExtypeVal = (objForEachLoop).Properties[PropName].GetValue((objForEachLoop)).ToString();
                        mLogEntity.ComponentValue = (objForEachLoop).Properties["Name"].GetValue((objForEachLoop)).ToString();
                        mLogEntity.TestStatus = ExtypeVal.ToString();
                        if (modifySSIS)
                        {
                            (objForEachLoop).Properties[PropName].SetValue((objForEachLoop), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }
                        break;
                    case " Microsoft.SqlServer.Dts.Runtime.Package":
                        ExtypeVal = (objPackage).Properties[PropName].GetValue((objPackage)).ToString();
                        mLogEntity.ComponentValue = (objPackage).Properties["Name"].GetValue((objPackage)).ToString();
                        mLogEntity.TestStatus = ExtypeVal.ToString();
                        if (modifySSIS)
                        {
                            (objPackage).Properties[PropName].SetValue((objPackage), PropValue);
                            mLogEntity.TestStatus = PropValue.ToString();
                        }
                        break;


                    default:
                        break;
                }

                listLogObj.Add(mLogEntity);
                mLogEntity = null;
            }
        }*/
}

}


    