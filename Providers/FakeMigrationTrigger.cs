using System.Collections.Generic;
using System.Linq;
using CJP.ContentSync.Services;
using Orchard.Localization;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Providers {
    public class FakeMigrationTrigger : INotificationProvider {
        private readonly IContentMigrationManager _contentMigrationManager;


        //todo: allow for export settings to be defined as part of site settings
        //todo: allow for saving of credentials
        //todo: allow for creation of scheduled tasks
        public FakeMigrationTrigger(IContentMigrationManager contentMigrationManager)
        {
            _contentMigrationManager = contentMigrationManager;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            var result = _contentMigrationManager.ExecutePendingMigrations().ToList();
            return Enumerable.Empty<NotifyEntry>();
        }
    }
}
