using System.Collections.Generic;
using System.Linq;
using CJP.ContentSync.Models;
using CJP.ContentSync.Providers;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Services {
    public class DefaultContentMigrationManager : IContentMigrationManager {
        private readonly IEnumerable<IContentMigrationProvider> _contentMigrationProviders;
        private readonly INotifier _notifier;
        private readonly IContentMigrationStateService _contentMigrationStateService;

        public DefaultContentMigrationManager(IEnumerable<IContentMigrationProvider> contentMigrationProviders, INotifier notifier, IContentMigrationStateService contentMigrationStateService) {
            _contentMigrationProviders = contentMigrationProviders;
            _notifier = notifier;
            _contentMigrationStateService = contentMigrationStateService;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public IEnumerable<MigrationExectutionSummary> ExecutePendingMigrations() {
            var executedMigrations = _contentMigrationStateService.GetExecutedMigrations().ToList();

            foreach (var provider in _contentMigrationProviders) {
                var pendingMigrations = provider.GetAvailableMigrations().Where(m => !executedMigrations.Contains(m)).ToList();

                if (!pendingMigrations.Any()) {
                    continue;
                }

                var result = provider.ExecuteMigrations(pendingMigrations);

                foreach (var successfulMigration in result.SuccessfulMigrations) {
                    _contentMigrationStateService.MarkMigrationAsExecuted(successfulMigration);
                }

                foreach (var failedMigration in result.FailedMigrations) {
                    var message = string.Format("Content Migration {0} failed.", failedMigration);

                    _notifier.Add(NotifyType.Error, T(message));
                    Logger.Error(message);
                }

                yield return result;
            }
        }
    }
}