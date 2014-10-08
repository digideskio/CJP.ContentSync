using System;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Themes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class CurrentThemeRecipeStepHandler : IRecipeHandler
    {
        private readonly ISiteThemeService _siteThemeService;

        public CurrentThemeRecipeStepHandler(ISiteThemeService siteThemeService) {
            _siteThemeService = siteThemeService;
        }

        /*
            <CurrentTheme name="MySuperTheme" />
         */
        //Enable any features that are in the list, disable features that aren't in the list
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "CurrentTheme", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var themeToEnable = recipeContext.RecipeStep.Step.Attribute("id").Value;
            _siteThemeService.SetSiteTheme(themeToEnable);

            recipeContext.Executed = true;
        }
    }
}