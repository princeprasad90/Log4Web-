using System;

namespace Log4WebService
{
    public class DataModel
    {
        public string MODULE { get; set; }
        public string URL { get; set; }
        public string STATUS { get; set; }
        public string REQUEST { get; set; }
        public string RESPONSE { get; set; }
        public string ERRPRMSG { get; set; }
        public string REMARKS { get; set; }
        public string MISC1 { get; set; }
        public string MISC2 { get; set; }
        public string MISC3 { get; set; }
        public int? MISC_INT1 { get; set; }
        public int? MISC_INT2 { get; set; }
        public Exception Exception { get; set; }
    }
}
