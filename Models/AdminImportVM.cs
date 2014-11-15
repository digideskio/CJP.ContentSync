using System.Collections.Generic;

namespace CJP.ContentSync.Models {
    public class AdminImportVM
    {
        public AdminImportVM() {
            SavedRemoteSiteConfigs = new List<RemoteSiteConfigRecord>();
        }

        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<RemoteSiteConfigRecord> SavedRemoteSiteConfigs { get; set; }
    }
}