using System.Collections.Generic;
using Orchard;

namespace CJP.ContentSync.Services {
    public interface IContentMigrationStateService : IDependency
    {
        IEnumerable<string> GetExecutedMigrations();
        IEnumerable<string> GetPendingMigrations();
        void MarkMigrationAsExecuted(string migrationName);
        void MarkMigrationAsPending(string migrationName);
    }
}