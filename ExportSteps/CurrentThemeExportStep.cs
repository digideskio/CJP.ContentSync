using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard;
using Orchard.ImportExport.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes.Services;

namespace CJP.ContentSync.ExportSteps
{
    public class CurrentThemeExportStep : IExportEventHandler, ICustomExportStep
    {
        private readonly ISiteThemeService _siteThemeService;

        public CurrentThemeExportStep(ISiteThemeService siteThemeService) 
        {
            _siteThemeService = siteThemeService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public void Exporting(ExportContext context) {
            if (!context.ExportOptions.CustomSteps.Contains("Current Theme")) { return; }

            var currentTheme = _siteThemeService.GetSiteTheme();

            if (currentTheme == null)
            {
                var ex = new OrchardException(T("Could not add the Current Theme step to the export because there is currently no theme activated."));
                Logger.Error(ex, ex.Message);
                throw ex;
            }

            var xmlElement = new XElement("CurrentTheme", new XAttribute("name", currentTheme.Name), new XAttribute("id", currentTheme.Id));

            var rootElement = context.Document.Descendants("Orchard").FirstOrDefault();

            if (rootElement == null) 
            {
                var ex = new OrchardException(T("Could not add the Current Theme step to the export because the document passed via the Export Context did not contain a node called 'Orchard'. The document was malformed."));
                Logger.Error(ex, ex.Message);
                throw ex;
            }

            rootElement.Add(xmlElement);
        }

        public void Exported(ExportContext context) {}

        public void Register(IList<string> steps) {
            steps.Add("Current Theme");
        }
    }
}