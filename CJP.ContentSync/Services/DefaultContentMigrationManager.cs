using System.Collections.Generic;
using System.Linq;
using CJP.ContentSync.Models;
using CJP.ContentSync.Providers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Services {
    public class DefaultContentMigrationManager : IContentMigrationManager 
    {
        private readonly IEnumerable<IContentMigrationProvider> _contentMigrationProviders;
        private readonly IRepository<MigrationExecutionRecord> _repository; //todo: abstract this away from the repository
        private readonly IClock _clock;
        private readonly INotifier _notifier;

        public DefaultContentMigrationManager(IEnumerable<IContentMigrationProvider> contentMigrationProviders, IRepository<MigrationExecutionRecord> repository, IClock clock, INotifier notifier)
        {
            _contentMigrationProviders = contentMigrationProviders;
            _repository = repository;
            _clock = clock;
            _notifier = notifier;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public IEnumerable<MigrationExectutionSummary> ExecutePendingMigrations()
        {
            var executedMigrations = _repository.Table.ToList().Select(m => m.MigrationName).ToList();
            //var executedMigrations = Enumerable.Empty<string>(); 

            foreach (var provider in _contentMigrationProviders) 
            {
                var pendingMigrations = provider.GetAvailableMigrations().Where(m=>!executedMigrations.Contains(m)).ToList();

                if (!pendingMigrations.Any()) {
                    continue;
                }

                var result = provider.ExecuteMigrations(pendingMigrations);

                foreach (var successfulMigration in result.SuccessfulMigrations) {
                    _repository.Create(new MigrationExecutionRecord{MigrationName = successfulMigration, ExecutedAt = _clock.UtcNow});
                }

                foreach (var failedMigration in result.FailedMigrations) {
                    var message = string.Format("Content Migration {0} failed with the reason: {1}", failedMigration.MigrationName,failedMigration.FailureReason);

                    _notifier.Add(NotifyType.Error, T(message));
                    Logger.Error(message);
                }

                yield return result;
            }
        }
    }
}