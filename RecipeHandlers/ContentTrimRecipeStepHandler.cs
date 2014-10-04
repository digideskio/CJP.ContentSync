using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.RecipeHandlers
{
    public class ContentTrimRecipeStepHandler : IRecipeHandler
    {
        private readonly IContentManager _contentManager;

        public ContentTrimRecipeStepHandler(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        /* <ContentTrim>
         *   <ContentTypes>
         *      <add type="page"/>
         *      <add type="widget"/>
         *   </ContentTypes>
         *   <ContentToKeep>
         *      <add identifier="123456789"/>
         *      <add identifier="321654897"/>
         *   </ContentType>
         *  </ContentTrim>
         */
        public void ExecuteRecipeStep(RecipeContext recipeContext) {
            if (!String.Equals(recipeContext.RecipeStep.Name, "ContentTrim", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contentTypesElement = recipeContext.RecipeStep.Step.Descendants("ContentTypes").FirstOrDefault();
            var contentToKeepElement = recipeContext.RecipeStep.Step.Descendants("ContentToKeep").FirstOrDefault();

            if (contentTypesElement == null) { throw new Exception("Could not execute the Content Trim step as there was no 'ContentTypes' element in the step."); }
            if (contentToKeepElement == null) { throw new Exception("Could not execute the Content Trim step as there was no 'ContentToKeep' element in the step."); }

            var contentTypes = contentTypesElement.Descendants("add").Select(e => e.Attribute("type").Value);
            var contentToKeep = contentToKeepElement.Descendants("add").Select(e => e.Attribute("identifier").Value).ToList();

            var contentItems = _contentManager.Query<IdentityPart, IdentityPartRecord>(contentTypes.ToArray()).Where(c => !contentToKeep.Contains(c.Identifier)).List();

            foreach (var contentItem in contentItems)
            {
                _contentManager.Remove(contentItem.ContentItem);
            }

            recipeContext.Executed = true;
        }
    }
}