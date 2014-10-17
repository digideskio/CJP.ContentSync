using System.Web.Mvc;
using CJP.ContentSync.Models;
using CJP.ContentSync.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.UI.Admin;

namespace CJP.ContentSync.Controllers
{
    [Admin]
    public class RedactionsController : Controller
    {
        private readonly ITextRedactionService _textRedactionService;

        public RedactionsController(ITextRedactionService textRedactionService)
        {
            _textRedactionService = textRedactionService;

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

        public ActionResult EditRedaction(int id=0) {
            var redaction = _textRedactionService.GetRedaction(id);

            if (redaction == null) {
                redaction = new RedactionRecord();
            }

            return View(redaction);
        }
    }
}