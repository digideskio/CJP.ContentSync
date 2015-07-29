using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data.Migration;

namespace CJP.ContentSync.Services
{
    // A temporary helper service to migrate table storage to infoset storage. Once all modules have been deployed to production, we should remove this interface and its usages.
    public interface IInfoSetMigrationService : IDependency 
    {
        void Migrate<T>(DataMigrationImpl migration, string tableName, Action<T, object[]> action, bool dropTable = true) where T : ContentPart;
    }
}