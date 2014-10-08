using System;
using System.Linq;
using Orchard.Environment.Features;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class FeatureSyncRecipeStepHandler : IRecipeHandler
    {
        private readonly IFeatureManager _featureManager;

        public FeatureSyncRecipeStepHandler(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        /*
          <EnabledFeatures>
            <Feature Id="Orchard.ImportExport" />
         */
        //Enable any features that are in the list, disable features that aren't in the list
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "FeatureSync", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var features = recipeContext.RecipeStep.Step.Descendants();
            var featureIds = features.Where(f => f.Name == "Feature").Select(f => f.Attribute("Id").Value);

            var availableFeatures = _featureManager.GetAvailableFeatures();
            var enabledFeatures = _featureManager.GetEnabledFeatures().ToList();

            var featuresToDisable = enabledFeatures.Where(f => !featureIds.Contains(f.Id))
                .Select(f => f.Id);

            var featuresToEnable = availableFeatures
                .Where(f => featureIds.Contains(f.Id)) //available features that are in the list of features that need to be enabled
                .Where(f => !enabledFeatures.Select(ef => ef.Id).Contains(f.Id)) //remove features that are already enabled
                .Select(f => f.Id);

            _featureManager.DisableFeatures(featuresToDisable, true);
            _featureManager.EnableFeatures(featuresToEnable, true);

            recipeContext.Executed = true;
        }
    }
}