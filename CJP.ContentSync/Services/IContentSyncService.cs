using Orchard;

namespace CJP.ContentSync.Services {
    public interface IContentSyncService : IDependency
    {
        string GenerateExportText();
        void PublishExportText(string exportText);
        void SyncWithExport(string exportText);
        void SyncWithLatestExport();
    }
}