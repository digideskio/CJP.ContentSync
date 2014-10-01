using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CJP.ContentSync.Services;
using Orchard;
using Orchard.ImportExport.Services;
using Orchard.Localization;
using Orchard.Logging;

namespace CJP.ContentSync.ExportSteps
{
    public class ExecutedContentMigrationsExportStep : IExportEventHandler, ICustomExportStep
    {
        private readonly IContentMigrationStateService _contentMigrationStateService;

        public ExecutedContentMigrationsExportStep(IContentMigrationStateService contentMigrationStateService) 
        {
            _contentMigrationStateService = contentMigrationStateService;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public void Exporting(ExportContext context) {
            if (!context.ExportOptions.CustomSteps.Contains("Sync Content Migrations")) { return; }

            var xmlElement = new XElement("ExecutedContentMigrations");

            foreach (var migration in _contentMigrationStateService.GetExecutedMigrations())
            {
                xmlElement.Add(new XElement("Migration", new XAttribute("Name", migration)));
            }

            var rootElement = context.Document.Descendants("Orchard").FirstOrDefault();

            if (rootElement == null) 
            {
                var ex = new OrchardException(T("Could not export this site's Executed Data Migrations because the document passed via the Export Context did not contain a node called 'Orchard'. The document was malformed."));
                Logger.Error(ex, ex.Message);
                throw ex;
            }

            rootElement.Add(xmlElement);
        }

        public void Exported(ExportContext context) {}

        public void Register(IList<string> steps) {
            steps.Add("Sync Content Migrations");
        }
    }
}