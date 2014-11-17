using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CJP.ContentSync.Models {
    public class AdminImportVM
    {
        public AdminImportVM() {
            SavedRemoteSiteConfigs = new List<RemoteSiteConfigRecord>();
        }

        [Required]
        public string Url { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public List<RemoteSiteConfigRecord> SavedRemoteSiteConfigs { get; set; }
    }
}