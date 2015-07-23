using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CJP.ContentSync.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ImportExport.Models;
using Orchard.ImportExport.Providers.ExportActions;
using Orchard.ImportExport.Recipes.Builders;
using Orchard.ImportExport.Services;
using Orchard.Logging;
using Orchard.Recipes.Providers.Builders;
using Orchard.Recipes.Services;

namespace CJP.ContentSync.Services 
{
    public class DefaultContentExportService : IContentExportService 
    {
        private readonly IImportExportService _importExportService;
        private readonly IContentManager _contentManager;
        private readonly ICustomExportStep _customExportStep;
        private readonly IOrchardServices _orchardServices;

        public DefaultContentExportService(IImportExportService importExportService, IContentManager contentManager, ICustomExportStep customExportStep, IOrchardServices orchardServices)
        {
            _importExportService = importExportService;
            _contentManager = contentManager;
            _customExportStep = customExportStep;
            _orchardServices = orchardServices;

            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        public string GetContentExportFilePath() 
        {
            var settings = _orchardServices.WorkContext.CurrentSite.As<ContentSyncSettingsPart>();
            var contentTypes = _contentManager.GetContentTypeDefinitions().Select(ctd => ctd.Name).Except(settings.ExcludedContentTypes).ToList();
            var customSteps = new List<string>();
            _customExportStep.Register(customSteps);
            customSteps = customSteps.Except(settings.ExcludedExportSteps).ToList();

            var exportActionContext = new ExportActionContext();
            var buildRecipeAction = Resolve<BuildRecipeAction>(action =>
            {
                action.RecipeBuilderSteps = new List<IRecipeBuilderStep>
                {
                    Resolve<ContentStep>(contentStep =>
                    {
                        contentStep.SchemaContentTypes = contentTypes;
                        contentStep.DataContentTypes = contentTypes;
                        contentStep.VersionHistoryOptions = Orchard.Recipes.Models.VersionHistoryOptions.Published;
                    }),
                    Resolve<CustomStepsStep>(customStepsStep => customStepsStep.CustomSteps = customSteps),
                    Resolve<SettingsStep>()
                };
            });

            _importExportService.Export(exportActionContext, new IExportAction[] { buildRecipeAction });
            return _importExportService.WriteExportFile(exportActionContext.RecipeDocument);
        }

        public string GetContentExportText() 
        {
            var filePath = GetContentExportFilePath();

            return File.ReadAllText(filePath);
        }

        private T Resolve<T>(Action<T> initializer = null)
        {
            var service = _orchardServices.WorkContext.Resolve<T>();

            if (initializer != null)
                initializer(service);

            return service;
        }
    }
}