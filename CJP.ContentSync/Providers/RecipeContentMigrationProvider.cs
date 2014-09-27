using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CJP.ContentSync.Models;
using Orchard.Environment.Features;
using Orchard.FileSystems.VirtualPath;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.Providers {
    public class RecipeContentMigrationProvider : IContentMigrationProvider 
    {
        private readonly IFeatureManager _featureManager;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IEnumerable<IRecipeHandler> _recipeHandlers;

        public RecipeContentMigrationProvider(IFeatureManager featureManager, IVirtualPathProvider virtualPathProvider, IEnumerable<IRecipeHandler> recipeHandlers) {
            _featureManager = featureManager;
            _virtualPathProvider = virtualPathProvider;
            _recipeHandlers = recipeHandlers;
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

                    var recipeExecutionWasSuccessful = true;

                    var mappedPath = _virtualPathProvider.MapPath(recipeFile);
                    var recipeXml = XElement.Parse(File.ReadAllText(mappedPath));

                    foreach (var descendant in recipeXml.Elements())
                    {
                        var stepName = descendant.Name.LocalName;
                        var recipeStep = new RecipeStep
                        {
                            Name = stepName,
                            Step = descendant
                        };

                        var recipeContext = new RecipeContext { RecipeStep = recipeStep, Executed = false };
                        foreach (var recipeHandler in _recipeHandlers)
                        {
                            recipeHandler.ExecuteRecipeStep(recipeContext);
                        }

                        if (!recipeContext.Executed) {
                            result.FailedMigrations.Add(new FailedMigrationSummary {
                                MigrationName = migrationName,
                                FailureReason = string.Format("Recipe step {0} failed to execute successfully.", stepName)
                            });

                            recipeExecutionWasSuccessful = false;
                        }
                    }

                    if (recipeExecutionWasSuccessful) {
                        result.SuccessfulMigrations.Add(migrationName);
                    }
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