using System.Collections.Generic;
using Orchard.ContentManagement;

namespace CJP.ContentSync.Models 
{
    public class ContentSyncSettingsPart : ContentPart<ContentSyncSettingsRecord>
    {
        public string[] AvailableExportSteps
        {
            get { return (Record.AvailableExportSteps ?? "").Split('#'); }
            set { Record.AvailableExportSteps = string.Join("#", value); }
        }

        public string[] AvailableSiteSettings
        {
            get { return (Record.AvailableSiteSettings ?? "").Split('#'); }
            set { Record.AvailableSiteSettings = string.Join("#", value); }
        }

        public string[] AvailableContentTypes
        {
            get { return (Record.AvailableContentTypes ?? "").Split('#'); }
            set { Record.AvailableContentTypes = string.Join("#", value); }
        }

        public IList<SelectableItem<string>> AllContentTypes { get; set; }
        public IList<SelectableItem<string>> AllExportSteps { get; set; }
        public IList<SelectableItem<string>> AllSiteSettings { get; set; }

        public string Test { get; set; }
    }
}