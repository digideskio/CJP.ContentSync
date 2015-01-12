using System.Linq;
using System.Security.Authentication;
using System.Web.Mvc;
using CJP.ContentSync.Models;
using CJP.ContentSync.Permissions;
using CJP.ContentSync.Services;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Controllers
{
    [Admin]
    public class SnapshotsController : Controller
    {
        private readonly ISnapshotService _snapshotService;
        private readonly INotifier _notifier;
        private readonly IClock _clock;
        private readonly IContentExportService _contentExportService;
        private readonly ShellSettings _shellSettings;
        private readonly IAuthorizer _authorizer;

        public SnapshotsController(ISnapshotService snapshotService, INotifier notifier, IClock clock, IContentExportService contentExportService, ShellSettings shellSettings, IAuthorizer authorizer)
        {
            _snapshotService = snapshotService;
            _notifier = notifier;
            _clock = clock;
            _contentExportService = contentExportService;
            _shellSettings = shellSettings;
            _authorizer = authorizer;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            if (!_authorizer.Authorize(ContentSyncPermissions.SnapshotManager, T("You need the {0} permission to do this.", ContentSyncPermissions.SnapshotManager.Name)))
            {
                return new HttpUnauthorizedResult();
            }

            return View(_snapshotService.GetSnaphots().OrderByDescending(ss => ss.TimeTaken));
        }

        [HttpGet]
        public ActionResult View(int? id = null)
        {
            if (!_authorizer.Authorize(ContentSyncPermissions.SnapshotManager, T("You need the {0} permission to do this.", ContentSyncPermissions.SnapshotManager.Name)))
            {
                return new HttpUnauthorizedResult();
            }

            SnapshotRecord record = null;

            if (id.HasValue)
            {
                record = _snapshotService.GetSnaphot(id.Value);
            }

            if (record == null)
            {
                record = new SnapshotRecord
                {
                    TimeTaken = _clock.UtcNow,
                    Data = _contentExportService.GetContentExportText()
                };
            }

            return View(record);
        }

        [HttpGet]
        public ActionResult Download()
        {
            if (!_authorizer.Authorize(ContentSyncPermissions.SnapshotDownloader, T("You need the {0} permission to do this.", ContentSyncPermissions.SnapshotDownloader.Name)))
            {
                return new HttpUnauthorizedResult();
            }

            var filePath = _contentExportService.GetContentExportFilePath();
            var fileName = string.Format("Snapshot from {0} - taken {1:yyyy-MM-dd HH-mm-ss}.xml", _shellSettings.Name, _clock.UtcNow);

            return File(filePath, "text/xml", fileName);
        }

        [HttpPost]
        public ActionResult Take()
        {
            if (!_authorizer.Authorize(ContentSyncPermissions.SnapshotManager, T("You need the {0} permission to do this.", ContentSyncPermissions.SnapshotManager.Name)))
            {
                return new HttpUnauthorizedResult();
            }

            _snapshotService.TakeSnaphot();
            _notifier.Information(T("A snapshot of this site's current configuration has been taken and added to the collection."));

            return RedirectToAction("Index");
        }
    }
}