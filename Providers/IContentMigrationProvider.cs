using System.Collections.Generic;
using CJP.ContentSync.Models;
using Orchard;

namespace CJP.ContentSync.Providers
{
    public interface IContentMigrationProvider : IDependency 
    {
        IEnumerable<string> GetAvailableMigrations();
        MigrationExectutionSummary ExecuteMigrations(IList<string> migrationNames);
    }
}