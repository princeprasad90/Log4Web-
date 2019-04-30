using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Log4WebService
{
    /// <summary>
    /// Logging  class defenition
    /// </summary>
    public class Log4Web : Common
    {
        public SettingsModel Model;
        string DOCFILEPATH;
        /// <summary>
        /// Overide the config parameter using SettingsModel 
        /// </summary>
        /// <param name="settings"></param>
        public Log4Web(SettingsModel settings)
        {
            Model = GetConfigurationUsingSectionGroup(settings);
            DOCFILEPATH = Model.Filepath + Model.Filename;

        }
        /// <summary>
        /// Add Log Only To Database.
        /// </summary>
        /// <param name="Datas"></param>
        public void Log_DB(DataModel Datas)
        {
            try
            {
                if (Datas.Exception != null)
                {
                    Datas.ERRPRMSG = ExceptionExtensions.ToLogString(Datas.Exception, Environment.StackTrace);
                }
                Thread DBThread = new Thread(() => Add_To_DB(Model, Datas));
                //  Thread FileRemvThrd = new Thread(() => RemoveOldLogs(Model));
                //  FileRemvThrd.Start();
                DBThread.Start();
            }
            catch (Exception ex)
            {
                Add_To_File(new SettingsModel() { Filepath = @"C:\Logs\", Filename = "ExceptionDLL.txt" }, new DataModel() { ERRPRMSG = ExceptionExtensions.ToLogString(ex, Environment.StackTrace), Exception = ex });
            }
        }
        /// <summary>
        /// Add Log Only To File.
        /// </summary>
        /// <param name="Datas"></param>
        public void Log_FILE(DataModel Datas)
        {
            try
            {
                if (Datas.Exception != null)
                {
                    Datas.ERRPRMSG = ExceptionExtensions.ToLogString(Datas.Exception, Environment.StackTrace);
                }
                Thread FileThread = new Thread(() => Add_To_File(Model, Datas));
                FileThread.Start();
                // Thread FileRemvThrd = new Thread(() => RemoveOldLogs(Model));
                //  FileRemvThrd.Start();
            }
            catch (Exception ex)
            {
                Add_To_File(new SettingsModel() { Filepath = @"C:\Logs\", Filename = "ExceptionDLL.txt" }, new DataModel() { ERRPRMSG = ExceptionExtensions.ToLogString(ex, Environment.StackTrace), Exception = ex });
            }
        }
        public DataTable ExecuteReader(string Constring, string ProcName, ListDictionary list = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Constring) || String.IsNullOrEmpty(ProcName))
                    return null;
                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(ProcName, conn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (list != null)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            foreach (DictionaryEntry kv in list)
                            {
                                cmd.Parameters.AddWithValue(kv.Key.ToString(), kv.Value);
                            }
                        }
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        cmd.Parameters.Clear();
                        return dt;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public void ExecuteNoQuery(string Constring, string ProcName, ListDictionary list = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Constring) || String.IsNullOrEmpty(ProcName))
                {
                    throw new Exception("Parameters Shouldn't be Empty");
                }
                using (SqlConnection conn = new SqlConnection(Constring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(ProcName, conn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (list != null)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            foreach (DictionaryEntry kv in list)
                            {
                                cmd.Parameters.AddWithValue(kv.Key.ToString(), kv.Value);
                            }
                        }
                        cmd.ExecuteNonQueryAsync();
                        cmd.Parameters.Clear();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
