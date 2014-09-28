using System.Collections.Generic;
using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Data;
using Orchard.Services;

namespace CJP.ContentSync.Services {
    public class DefaultContentMigrationStateService : IContentMigrationStateService{

        private readonly IRepository<MigrationExecutionRecord> _repository; 
        private readonly IClock _clock;
        public DefaultContentMigrationStateService(IRepository<MigrationExecutionRecord> repository, IClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public IEnumerable<string> GetPendingMigrations()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetExecutedMigrations() {
            return _repository.Table.ToList().Select(m => m.MigrationName);
        }

        public void MarkMigrationAsExecuted(string migrationName){
            _repository.Create(new MigrationExecutionRecord { MigrationName = migrationName, ExecutedAt = _clock.UtcNow });
        }

        public void MarkMigrationAsPending(string migrationName) {
            foreach (var record in _repository.Fetch(r => r.MigrationName == migrationName))
            {
                _repository.Delete(record);
            }
        }
    }
}