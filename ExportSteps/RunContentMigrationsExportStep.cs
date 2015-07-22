using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard;
using Orchard.ImportExport.Models;
using Orchard.ImportExport.Services;
using Orchard.Localization;
using Orchard.Logging;

namespace CJP.ContentSync.ExportSteps
{
    public class RunContentMigrationsExportStep : IExportEventHandler, ICustomExportStep
    {
        public RunContentMigrationsExportStep() 
        {
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public void Exporting(ExportContext context) {
            if (!context.ExportOptions.CustomSteps.Contains("RunContentMigrations")) { return; }

            var xmlElement = new XElement("RunContentMigrations");

            var rootElement = context.Document.Descendants("Orchard").FirstOrDefault();

            if (rootElement == null) 
            {
                var ex = new OrchardException(T("Could not add the Run Data Migrations step to the export because the document passed via the Export Context did not contain a node called 'Orchard'. The document was malformed."));
                Logger.Error(ex, ex.Message);
                throw ex;
            }

            rootElement.Add(xmlElement);
        }

        public void Exported(ExportContext context) {}

        public void Register(IList<string> steps) {
            steps.Add("RunContentMigrations");
        }
    }
}