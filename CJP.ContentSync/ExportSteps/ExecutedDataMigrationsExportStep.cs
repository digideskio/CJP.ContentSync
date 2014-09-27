using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CJP.ContentSync.Models;
using Orchard;
using Orchard.Data;
using Orchard.ImportExport.Services;
using Orchard.Localization;
using Orchard.Logging;

namespace CJP.ContentSync.ExportSteps
{
    public class ExecutedDataMigrationsExportStep : IExportEventHandler, ICustomExportStep
    {
        private readonly IRepository<MigrationExecutionRecord> _repository;

        public ExecutedDataMigrationsExportStep(IRepository<MigrationExecutionRecord> repository) 
        {
            _repository = repository;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public void Exporting(ExportContext context) {
            if (!context.ExportOptions.CustomSteps.Contains("Executed Data Migrations")) { return; }

            var xmlElement = new XElement("ExecutedDataMigrations");

            foreach (var migration in _repository.Table.ToList()) {
                xmlElement.Add(new XElement("Migration", new XAttribute("Name", migration.MigrationName)));
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
            steps.Add("Executed Data Migrations");
        }
    }
}