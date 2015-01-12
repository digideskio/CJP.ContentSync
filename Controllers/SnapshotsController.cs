using System.Linq;
using System.Web.Mvc;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
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

        public SnapshotsController(ISnapshotService snapshotService, INotifier notifier, IClock clock, IContentExportService contentExportService, ShellSettings shellSettings)
        {
            _snapshotService = snapshotService;
            _notifier = notifier;
            _clock = clock;
            _contentExportService = contentExportService;
            _shellSettings = shellSettings;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_snapshotService.GetSnaphots().OrderByDescending(ss => ss.TimeTaken));
        }

        [HttpGet]
        public ActionResult View(int? id = null)
        {
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
            var filePath = _contentExportService.GetContentExportFilePath();
            var fileName = string.Format("Snapshot from {0} - taken {1:yyyy-MM-dd HH-mm-ss}.xml", _shellSettings.Name, _clock.UtcNow);

            return File(filePath, "text/xml", fileName);
        }

        [HttpPost]
        public ActionResult Take()
        {
            _snapshotService.TakeSnaphot();
            _notifier.Information(T("A snapshot of this site's current configuration has been taken and added to the collection."));

            return RedirectToAction("Index");
        }
    }
}