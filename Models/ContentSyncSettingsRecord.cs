using Orchard.ContentManagement.Records;

namespace CJP.ContentSync.Models
{
    public class ContentSyncSettingsRecord : ContentPartRecord
    {
        public virtual string AvailableExportSteps { get; set; }
        public virtual string AvailableSiteSettings { get; set; }
        public virtual string AvailableContentTypes { get; set; }
    }
}