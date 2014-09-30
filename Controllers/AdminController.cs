using System.Web.Mvc;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard;
using Orchard.ImportExport.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Controllers
{
    public class AdminController : Controller
    {
        private readonly IImportExportService _importExportService;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentExportService _contentExportService;

        public AdminController(IImportExportService importExportService, IOrchardServices orchardServices, IContentExportService contentExportService) {
            _importExportService = importExportService;
            _orchardServices = orchardServices;
            _contentExportService = contentExportService;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new AdminImportVM { Url = "http://localhost:30321/orchardlocal", Username="OrchardAdmin", Password = "Password"});
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(AdminImportVM vm) {
            var result = _contentExportService.GetContentExportFromUrl(vm.Url, vm.Username, vm.Password);

            if (result.Status == ApiResultStatus.Unauthorized)
            {
                _orchardServices.Notifier.Warning(T("Either the username and password you supplied is incorrect, or this user does not have the correct permissions to export content"));
                return View("Index", vm);
            }

            if (result.Status == ApiResultStatus.Failed)
            {
                _orchardServices.Notifier.Error(T("There was an unexpected error when trying to export the remote site"));
                return View("Index", vm);
            }

            _orchardServices.Notifier.Information(T("Site content and configurations have been synced"));
            _importExportService.Import(result.Text);

            return RedirectToAction("Index");
        }
    }
}