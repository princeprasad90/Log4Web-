using System;

namespace Log4WebService
{
    public  class SettingsModel
    {
      
        public string DatabseConnection { get; set; }
        public string Filepath { get; set; }
        public string Filename { get; set; }
        public string FileDeleteDays { get; set; }
        public string FileNameWithDate { get; set; }
        public String SP_NAME { get; set; }
        public Nullable<int> SP_STATUS1 { get; set; }
        public Nullable<int> SP_STATUS2 { get; set; }
    }
}
