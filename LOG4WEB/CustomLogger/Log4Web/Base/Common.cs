using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Log4WebService
{
    public class Common
    {
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _readWriteLockXML = new ReaderWriterLockSlim();
        private static readonly ReaderWriterLockSlim _readWriteLockJSON = new ReaderWriterLockSlim();

        protected SettingsModel GetConfigurationUsingSectionGroup(SettingsModel Settings)
        {
            var GlobalSettings = ConfigurationManager.GetSection("Log4Web/GlobalSettings") as NameValueCollection;
            try
            {
                if (GlobalSettings != null)
                {
                    if (string.IsNullOrEmpty(Settings.FileDeleteDays))
                        Settings.FileDeleteDays = GlobalSettings["FileDeleteDays"];
                    if (string.IsNullOrEmpty(Settings.DatabseConnection))
                        Settings.DatabseConnection = GlobalSettings["DatabseConnection"];
                    if (string.IsNullOrEmpty(Settings.FileNameWithDate))
                        Settings.FileNameWithDate = GlobalSettings["FileNameWithDate"];
                    if (string.IsNullOrEmpty(Settings.Filepath))
                    {
                        Settings.Filepath = GlobalSettings["Filepath"];
                        Settings.Filepath = (Settings.Filepath.EndsWith(@"\")) ? Settings.Filepath : Settings.Filepath + @"\";
                    }
                    if (string.IsNullOrEmpty(Settings.Filename))
                        Settings.Filename = GlobalSettings["Filename"];
                    if (string.IsNullOrEmpty(Settings.SP_NAME))
                        Settings.SP_NAME = GlobalSettings["SP_NAME"];
                    if (Settings.SP_STATUS1 == null)
                        Settings.SP_STATUS1 = Convert.ToInt32(GlobalSettings["SP_STATUS1"]);
                    if (Settings.SP_STATUS2 == null)
                        Settings.SP_STATUS2 = Convert.ToInt32(GlobalSettings["SP_STATUS2"]);
                    return Settings;
                }
                else
                {
                    throw new Exception("Config File Null");
                }
            }
            catch (Exception ex)
            {
                Add_To_File(new SettingsModel() { Filepath = @"C:\Logs\", Filename = "ExceptionDLL.txt" }, new DataModel() { ERRPRMSG = ExceptionExtensions.ToLogString(ex, Environment.StackTrace), Exception = ex });
                return null;
            }
        }
        protected void Add_To_DB(SettingsModel Set, DataModel Dat)
        {
            try
            {
                var SP_SET = ConfigurationManager.GetSection("Log4Web/SP_PROCS_FIELDS") as NameValueCollection;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[Set.DatabseConnection].ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(Set.SP_NAME, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (!string.IsNullOrEmpty(SP_SET["SP_STATUS1"]) && (Set.SP_STATUS1 != 0))
                            cmd.Parameters.AddWithValue(SP_SET["SP_STATUS1"], Set.SP_STATUS1);
                        if (!string.IsNullOrEmpty(SP_SET["SP_STATUS2"]) && (Set.SP_STATUS2 != 0))
                            cmd.Parameters.AddWithValue(SP_SET["SP_STATUS2"], Set.SP_STATUS2);
                        if (!string.IsNullOrEmpty(SP_SET["MODULE"]))
                            cmd.Parameters.AddWithValue(SP_SET["MODULE"], Dat.MODULE);
                        if (!string.IsNullOrEmpty(SP_SET["URL"]))
                            cmd.Parameters.AddWithValue(SP_SET["URL"], Dat.URL);
                        if (!string.IsNullOrEmpty(SP_SET["STATUS"]))
                            cmd.Parameters.AddWithValue(SP_SET["STATUS"], Dat.STATUS);
                        if (!string.IsNullOrEmpty(SP_SET["REQUEST"]))
                            cmd.Parameters.AddWithValue(SP_SET["REQUEST"], Dat.REQUEST);
                        if (!string.IsNullOrEmpty(SP_SET["RESPONSE"]))
                            cmd.Parameters.AddWithValue(SP_SET["RESPONSE"], Dat.RESPONSE);
                        if (!string.IsNullOrEmpty(SP_SET["ERRPRMSG"]))
                            cmd.Parameters.AddWithValue(SP_SET["ERRPRMSG"], Dat.ERRPRMSG);
                        if (!string.IsNullOrEmpty(SP_SET["REMARKS"]))
                            cmd.Parameters.AddWithValue(SP_SET["REMARKS"], Dat.REMARKS);
                        if (!string.IsNullOrEmpty(SP_SET["MISC1"]))
                            cmd.Parameters.AddWithValue(SP_SET["MISC1"], Dat.MISC1);
                        if (!string.IsNullOrEmpty(SP_SET["MISC2"]))
                            cmd.Parameters.AddWithValue(SP_SET["MISC2"], Dat.MISC2);
                        if (!string.IsNullOrEmpty(SP_SET["MISC3"]))
                            cmd.Parameters.AddWithValue(SP_SET["MISC3"], Dat.MISC3);
                        if (!string.IsNullOrEmpty(SP_SET["MISC_INT1"]))
                            cmd.Parameters.AddWithValue(SP_SET["MISC_INT1"], Dat.MISC_INT1);
                        if (!string.IsNullOrEmpty(SP_SET["MISC_INT2"]))
                            cmd.Parameters.AddWithValue(SP_SET["MISC_INT2"], Dat.MISC_INT2);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
            }
            catch(Exception ex)
            {
                Add_To_File(new SettingsModel() { Filepath = @"C:\Logs\", Filename = "ExceptionDLL.txt" }, new DataModel() { ERRPRMSG = ExceptionExtensions.ToLogString(ex, Environment.StackTrace), Exception = ex });
            }
        }
        protected void Add_To_File(SettingsModel Set, DataModel Dat)
        {
            try
            {
                _readWriteLock.EnterWriteLock();
                string FilePath = string.Empty;
                GetFileNameWithDate(Set, out FilePath);
                using (StreamWriter sw = new StreamWriter(FilePath, true))
                {
                    sw.Write("");
                    sw.Flush();
                }
                string str;
                Dat.Exception = null;
                string cont = BuidFileContent(Set, Dat) + Environment.NewLine;
                using (StreamReader sreader = new StreamReader(FilePath))
                {
                    str = sreader.ReadToEnd();
                }
                File.Delete(FilePath);
                using (StreamWriter _testData = new StreamWriter(FilePath, false))
                {
                    _testData.Write(cont + str);
                }
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }
        private void GetFileNameWithDate(SettingsModel Set, out string FilePath)
        {
            if (!string.IsNullOrEmpty(Set.FileNameWithDate))
            {
                DateTime dateAndTime = DateTime.Now;
                var strDt = dateAndTime.ToString(Set.FileNameWithDate);
                string[] FileName = Set.Filename.Split('.');
                if (FileName.Length != 1)
                    FilePath = Set.Filepath + FileName[0] + strDt + "." + FileName[1];
                else
                    FilePath = Set.Filepath + FileName[0] + strDt + ".txt";
            }
            else
            {
                FilePath = Set.Filepath + Set.Filename;
            }
        }
        private string BuidFileContent(SettingsModel Set, DataModel Dat)
        {
            string cont = string.Format("{0} :- ", DateTime.Now);
            foreach (PropertyInfo prop in Dat.GetType().GetProperties())
            {
                if (prop.GetValue(Dat, null) != null)
                    cont += string.Format("{0} : {1} ,", prop.Name, prop.GetValue(Dat, null).ToString());
            }
            return cont;
        }
        protected bool RemoveOldLogs(SettingsModel Set)
        {
            try
            {
                //    string[] files = Directory.GetFiles(Set.Filepath);
                //    string Filename = null;
                //    foreach (string file in files)
                //    {
                //        FileInfo fi = new FileInfo(file);
                //        int deletetime = Convert.ToInt32(Set.FileDeleteDays);
                //        if (fi.LastWriteTime < DateTime.Now.AddDays(deletetime))
                //        {
                //            Filename += fi.FullName;
                //            fi.Delete();
                //        }
                //    }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
