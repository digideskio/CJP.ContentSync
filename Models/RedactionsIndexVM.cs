using System.Collections.Generic;

namespace CJP.ContentSync.Models
{
    public class RedactionsIndexVM
    {
        public IEnumerable<RedactionRecord> ContentRedactions { get; set; }
        public IEnumerable<SettingRedactionRecord> SettingRedactions { get; set; }
        public IEnumerable<FeatureRedactionRecord> FeatureRedactions { get; set; }
    }
}