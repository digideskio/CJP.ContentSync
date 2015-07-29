using System.Collections.Generic;
using Orchard.ContentManagement;

namespace CJP.ContentSync.Models 
{
    public class ContentSyncSettingsPart : ContentPart
    {
        public string[] ExcludedExportSteps
        {
            get { return (Retrieve<string>("ExcludedExportSteps") ?? "").Split('#'); }
            set { Store("ExcludedExportSteps", string.Join("#", value)); }
        }

        public string[] ExcludedSiteSettings
        {
            get { return (Retrieve<string>("ExcludedSiteSettings") ?? "").Split('#'); }
            set { Store("ExcludedSiteSettings", string.Join("#", value)); }
        }

        public string[] ExcludedContentTypes
        {
            get { return (Retrieve<string>("ExcludedContentTypes") ?? "").Split('#'); }
            set { Store("ExcludedContentTypes", string.Join("#", value)); }
        }

        public IList<SelectableItem<string>> AllContentTypes { get; set; }
        public IList<SelectableItem<string>> AllExportSteps { get; set; }
        public IList<SelectableItem<string>> AllSiteSettings { get; set; }

        public int SnapshotFrequencyMinutes
        {
            get { return this.Retrieve(x => x.SnapshotFrequencyMinutes); }
            set { this.Store(x => x.SnapshotFrequencyMinutes, value); }
        }
    }
}