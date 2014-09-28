using CJP.ContentSync.Services;
using Orchard.Environment;

namespace CJP.ContentSync.EventHandlers {
    public class ContentMigrationTriggerOnShellStart : IOrchardShellEvents {
        private readonly IContentMigrationManager _contentMigrationManager;

        public ContentMigrationTriggerOnShellStart(IContentMigrationManager contentMigrationManager)
        {
            _contentMigrationManager = contentMigrationManager;
        }

        public void Activated() {
            _contentMigrationManager.ExecutePendingMigrations();
        }

        public void Terminating() {}
    }
}