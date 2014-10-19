using System.Web.Mvc;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.UI.Admin;
using Orchard.UI.Notify;

namespace CJP.ContentSync.Controllers
{
    [Admin]
    public class RedactionsController : Controller
    {
        private readonly ITextRedactionService _textRedactionService;
        private readonly INotifier _notifier;

        public RedactionsController(ITextRedactionService textRedactionService, INotifier notifier)
        {
            _textRedactionService = textRedactionService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_textRedactionService.GetRedactions());
        }

        public ActionResult Edit(int id = 0)
        {
            var redaction = _textRedactionService.GetRedaction(id);

            if (redaction == null)
            {
                redaction = new RedactionRecord();
            }

            return View(redaction);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditRedactionPost(RedactionRecord redaction, int id = 0) {

            RedactionOperationStatus status;

            if (id == 0)
            {
                redaction.Id = id;
                status = _textRedactionService.UpdateRedaction(redaction);
            }
            else {
                redaction.Id = id;
                status = _textRedactionService.UpdateRedaction(redaction);

                _notifier.Information(T("Redaction updated"));
            }

            switch (status)
            {
                case RedactionOperationStatus.Created:
                    _notifier.Information(T("Redaction created"));
                    break;
                case RedactionOperationStatus.Updated:
                    _notifier.Information(T("Redaction updated"));
                    break;
                case RedactionOperationStatus.PlaceholderNotUnique:
                    _notifier.Error(T("Redaction could not be saved because the Placeholder was not unique. Ensure that the placeholder you specify does not already exist for another redaction."));
                    return View(redaction);
            }

            return RedirectToAction("Index");
        }
    }
}