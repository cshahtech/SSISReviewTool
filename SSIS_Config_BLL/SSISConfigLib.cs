using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using System.Xml;
using System.Configuration;

namespace SSIS_Config_BLL
{
    class SSISVariable
    {
        public string vName { get; set; }
        public string vDatatype { get; set; }
        public string vDefault { get; set; }

        public SSISVariable()
        {
            vName = string.Empty;
            vDatatype = string.Empty;
            vDefault = string.Empty;
        }
    }

    public class SSISConfigLib
    {
        #region VariableDeclaration
        enum ComponentType
        {
            UserVariable,
            SSISConfiguration,
            OLEDBConnection,
            EnvironmentVariable,
            DTSConfigPath,
            SSISVarXMLPath
        };
        enum testStatus
        {
            Missing,
            Present,
            Added,
            FailedToAdd
        };
        #endregion

        #region Initial Checks
        public LogEntity CheckEnvVar(bool modifySSIS)
        {
            try
            {
                LogEntity mLogEntity = new LogEntity();
                mLogEntity.PackageName = string.Empty;

                mLogEntity.ComponentType = ComponentType.EnvironmentVariable.ToString();

                var s = ((System.Configuration.ConfigurationSettings.AppSettings["EnvVar"]));

                if ((System.Configuration.ConfigurationManager.AppSettings["EnvVar"] != null))
                {
                    String EnvVar = ConfigurationManager.AppSettings["EnvVar"];
                    mLogEntity.ComponentValue = EnvVar;

                    String DTSConfigPath = Environment.GetEnvironmentVariable(EnvVar, EnvironmentVariableTarget.Machine);

                    if (DTSConfigPath == null)
                    {
                        mLogEntity.TestStatus = testStatus.Missing.ToString();
                        if (ConfigurationManager.AppSettings["EnvVarValue"] != null)
                        {
                            String EnvVarValue = ConfigurationManager.AppSettings["EnvVarValue"];
                            if (modifySSIS)
                            {
                                try
                                {
                                    Environment.SetEnvironmentVariable(EnvVar, EnvVarValue, EnvironmentVariableTarget.Machine);
                                    mLogEntity.TestStatus = testStatus.Missing.ToString();
                                }
                                catch
                                {
                                    mLogEntity.TestStatus = testStatus.FailedToAdd.ToString();
                                }
                            }
                        }
                        else
                        {
                            mLogEntity.TestStatus = "Unable to read Environment variable value";
                        }
                    }
                    else
                    {
                        mLogEntity.TestStatus = testStatus.Present.ToString();
                    }
                }
                else
                {
                    mLogEntity.TestStatus = "Unable to read Environment variable";
                }

                return mLogEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public LogEntity CheckDTSConfigPath()
        {
            try
            {
                LogEntity mLogEntity = new LogEntity();

                mLogEntity.PackageName = string.Empty;
                mLogEntity.ComponentType = ComponentType.DTSConfigPath.ToString();

                if ((ConfigurationManager.AppSettings["SSISVarXMLPath"] != null))
                {
                    String SSISVarXMLPath = ConfigurationManager.AppSettings["SSISVarXMLPath"];
                    mLogEntity.ComponentValue = SSISVarXMLPath;

                    String DTSConfigPath = Environment.GetEnvironmentVariable(SSISVarXMLPath, EnvironmentVariableTarget.Machine);

                    if (DTSConfigPath == null)
                    {
                        mLogEntity.TestStatus = "XML file for SSIS Variable(s)";
                    }
                    else
                    {
                        if (File.Exists(DTSConfigPath))
                        {
                            mLogEntity.TestStatus = testStatus.Present.ToString();
                        }
                        else
                        {
                            mLogEntity.TestStatus = testStatus.Missing.ToString();
                        }
                    }
                }
                else
                {
                    mLogEntity.TestStatus = "Unable to read SSIS Variable XML file path";
                }

                return mLogEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public LogEntity CheckSSISVarXMLPath()
        {
            try
            {
                LogEntity mLogEntity = new LogEntity();

                mLogEntity.PackageName = string.Empty;
                mLogEntity.ComponentType = ComponentType.SSISVarXMLPath.ToString();


                if ((ConfigurationManager.AppSettings["SSISVarXMLPath"] != null))
                {
                    String SSISVarXMLPath = ConfigurationManager.AppSettings["SSISVarXMLPath"];
                    mLogEntity.ComponentValue = SSISVarXMLPath;

                    if (File.Exists(SSISVarXMLPath))
                    {
                        mLogEntity.TestStatus = testStatus.Present.ToString();
                    }
                    else
                    {
                        mLogEntity.TestStatus = testStatus.Missing.ToString();
                    }
                }
                else
                {
                    mLogEntity.TestStatus = "SSISVarXML path not found in app.config";
                }

                return mLogEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

    }

    public class CreateConnection
    {
        private ConnectionManager ConMgr;

        public void CreateOLEDBConnection(Package p)
        {
            ConMgr = p.Connections.Add("OLEDB");
            ConMgr.ConnectionString = ConfigurationManager.AppSettings["OLEDBConnStr"]; ;
            ConMgr.Name = ConfigurationManager.AppSettings["OLEDBConnName_Config_Database"];
            ConMgr.Description = "OLE DB connection for Config Database";
            ConMgr.DelayValidation = false;

        }

        public void CreateOLEDBConnection(Package p, string ConnName)
        {
            ConMgr = p.Connections.Add("OLEDB");
            //ConMgr.ConnectionString = ConfigurationManager.AppSettings["OLEDBConnStr"]; ;
            ConMgr.Name = ConnName;
            ConMgr.Description = string.Format("OLE DB connection for {0} Database", ConnName);
            ConMgr.DelayValidation = false;
        }

        public void CreateFileConnection(Package p)
        {
            ConMgr = p.Connections.Add("File");
            ConMgr.ConnectionString = @"\\<yourserver>\<yourfolder>\books.xml";
            ConMgr.Name = "SSIS Connection Manager for Files";
            ConMgr.Description = "Flat File connection";
        }
    }
}

