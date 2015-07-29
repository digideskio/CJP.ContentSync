using System;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Data.Migration;

namespace CJP.ContentSync.Services
{
    public class InfoSetMigrationService : IInfoSetMigrationService
    {
        private readonly IWorkContextAccessor _wca;

        public InfoSetMigrationService(IWorkContextAccessor wca)
        {
            _wca = wca;
        }

        public void Migrate<T>(DataMigrationImpl migration, string tableName, Action<T, object[]> migrate, bool dropTable = true) where T : ContentPart
        {
            using (var scope = _wca.CreateWorkContextScope())
            {
                var sessionLocator = scope.WorkContext.Resolve<ISessionLocator>();
                var session = sessionLocator.For(typeof(InfoSetMigrationService));
                var contentManager = scope.WorkContext.Resolve<IContentManager>();
                var schemaBuilder = migration.SchemaBuilder;
                var sql = String.Format("select * from {0}", schemaBuilder.TableDbName(tableName));
                var query = session.CreateSQLQuery(sql);
                var rows = query.List<object[]>();
                var contentIds = rows.Select(x => (int) x[0]).ToArray();
                var contentItems = contentManager.GetMany<T>(contentIds, VersionOptions.Published, QueryHints.Empty).ToDictionary(x => x.Id);

                foreach (var row in rows)
                {
                    var contentId = (int)row[0];
                    var content = contentItems.ContainsKey(contentId) ? contentItems[contentId] : default(T);

                    if (content == null)
                        continue;

                    // Migrate table storage to infoset storage.
                    migrate(content, row);
                }

                if (dropTable)
                    schemaBuilder.DropTable(tableName);
            }
        }
    }
}