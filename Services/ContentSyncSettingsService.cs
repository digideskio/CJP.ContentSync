using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ImportExport.Models;

namespace CJP.ContentSync.Services {
    public class ContentSyncSettingsService : IContentSyncSettingsService 
    {
        private readonly IContentManager _contentManager;

        public ContentSyncSettingsService(IContentManager contentManager) 
        {
            _contentManager = contentManager;
        }

        public ExportOptions GetExportOptions() {
            return new ExportOptions { ExportData = true, ExportMetadata = false, ExportSiteSettings = false, VersionHistoryOptions = VersionHistoryOptions.Published, CustomSteps = new[] { "Features" }};
        }

        public IEnumerable<string> GetContentTypes() {
            return _contentManager.GetContentTypeDefinitions().Select(c => c.Name);
        }
    }
}