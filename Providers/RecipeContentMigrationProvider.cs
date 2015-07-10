using System.Collections.Generic;
using System.IO;
using System.Linq;
using CJP.ContentSync.Models;
using Orchard.Environment.Features;
using Orchard.FileSystems.VirtualPath;
using Orchard.ImportExport.Services;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.Providers {
    public class RecipeContentMigrationProvider : IContentMigrationProvider 
    {
        private readonly IFeatureManager _featureManager;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IImportExportService _importExportService;
        //private readonly IRecipeJournal _recipeJournal;

        public RecipeContentMigrationProvider(IFeatureManager featureManager, IVirtualPathProvider virtualPathProvider, IImportExportService importExportService/*, IRecipeJournal recipeJournal*/) {
            _featureManager = featureManager;
            _virtualPathProvider = virtualPathProvider;
            _importExportService = importExportService;
            //_recipeJournal = recipeJournal;
        }

        public IEnumerable<string> GetAvailableMigrations() {
            return GetAvailableRecipeFiles().Select(GetMigrationNameFromFileName);
        }

        public MigrationExectutionSummary ExecuteMigrations(IList<string> migrationNames)
        {
            var result = new MigrationExectutionSummary();

            foreach (var recipeFile in GetAvailableRecipeFiles()) {
                var migrationName = GetMigrationNameFromFileName(recipeFile);
                if (migrationNames.Contains(migrationName)) {

                    var mappedPath = _virtualPathProvider.MapPath(recipeFile);
                    var executionId = _importExportService.Import(File.ReadAllText(mappedPath));
                    //var recipeJournal = _recipeJournal.GetRecipeJournal(executionId);

                    //result.Messages.AddRange(recipeJournal.Messages.Select(m=>string.Format("Recipe Content Migration Provider: Migration: {0}. {1}", migrationName, m)));

                    //if (recipeJournal.Status == RecipeStatus.Complete) {
                    //    result.SuccessfulMigrations.Add(migrationName);
                    //}
                    //else{
                    //    result.FailedMigrations.Add(migrationName);
                    //}
                }
            }

            return result;
        }

        private IEnumerable<string> GetAvailableRecipeFiles()
        {
            //go through each enabled feature- any xml files inside a folder called ContentMigrations will be discovered
            var features = _featureManager.GetEnabledFeatures();

            foreach (var feature in features)
            {
                var migrationPath = _virtualPathProvider.Combine(feature.Extension.Location, feature.Extension.Id, "ContentMigrations", feature.Id);
                var mappedPath = _virtualPathProvider.MapPath(migrationPath);

                if (!Directory.Exists(mappedPath)) { continue; }

                foreach (var recipeFile in Directory.EnumerateFiles(mappedPath, "*.Recipe.xml"))
                {
                    yield return _virtualPathProvider.Combine(migrationPath, Path.GetFileName(recipeFile));
                }
            }
        }

        private string GetMigrationNameFromFileName(string fileName) {
            return fileName.Substring(0, fileName.LastIndexOf(".Recipe.xml"));
        }
    }
}