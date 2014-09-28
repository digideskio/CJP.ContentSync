using Orchard.ImportExport.Services;

namespace CJP.ContentSync.Services {
    public class DefaultContentSyncService : IContentSyncService
    {
        private readonly IImportExportService _importExportService;
        private readonly IContentSyncSettingsService _contentSyncSettingsService;
        private readonly IExportStorageAndRetrievalService _exportStorageAndRetrievalService;

        public DefaultContentSyncService(IImportExportService importExportService, IContentSyncSettingsService contentSyncSettingsService, IExportStorageAndRetrievalService exportStorageAndRetrievalService) {
            _importExportService = importExportService;
            _contentSyncSettingsService = contentSyncSettingsService;
            _exportStorageAndRetrievalService = exportStorageAndRetrievalService;
        }

        public string GenerateExportText() {
            return _importExportService.Export(_contentSyncSettingsService.GetContentTypes(), _contentSyncSettingsService.GetExportOptions());
        }

        public void PublishExportText(string exportText) {
            _exportStorageAndRetrievalService.PublishExport(GenerateExportText());
        }

        public void SyncWithExport(string exportText) {
            _importExportService.Import(exportText);
        }

        public void SyncWithLatestExport() {
            SyncWithExport(_exportStorageAndRetrievalService.GetLatestExport());
        }
    }
}