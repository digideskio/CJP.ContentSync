using System.Collections.Generic;
using CJP.ContentSync.Models;
using Orchard;

namespace CJP.ContentSync.Services {
    public interface IContentMigrationManager : IDependency 
    {
        IEnumerable<MigrationExectutionSummary> ExecutePendingMigrations();
    }
}