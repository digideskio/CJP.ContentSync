using System.Collections.Generic;
using System.Linq;
using System.Net;
using CJP.ContentSync.Models;
using Orchard.ContentManagement;
using Orchard.ImportExport.Models;
using Orchard.ImportExport.Services;
using Orchard.Logging;

namespace CJP.ContentSync.Services {
    public class DefaultContentExportService : IContentExportService {
        private readonly IImportExportService _importExportService;
        private readonly IContentManager _contentManager;
        private readonly ICustomExportStep _customExportStep;

        public DefaultContentExportService(IImportExportService importExportService, IContentManager contentManager, ICustomExportStep customExportStep)
        {
            _importExportService = importExportService;
            _contentManager = contentManager;
            _customExportStep = customExportStep;

            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }

        public string GetContentExportText() {
            var contentTypes = _contentManager.GetContentTypeDefinitions().Select(ctd=>ctd.Name);

            var customSteps = new List<string>();
            _customExportStep.Register(customSteps);

            return _importExportService.Export(contentTypes, new ExportOptions { CustomSteps = customSteps, ExportData = true, ExportMetadata = true, ExportSiteSettings = false, VersionHistoryOptions = VersionHistoryOptions.Published });
        }

        public ApiResult GetContentExportFromUrl(string url, string username, string password)
        {
            url = string.Format("{0}/contentsync/contentExport?username={1}&password={2}", url, username, password);
            var importText = string.Empty;

            try
            {
                importText = (new WebClient()).DownloadString(url);
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;

                if (statusCode == HttpStatusCode.Unauthorized) {
                    return new ApiResult {Status = ApiResultStatus.Unauthorized};
                }

                if (statusCode != HttpStatusCode.OK)
                {
                    Logger.Log(LogLevel.Error, ex, "There was an error exporting the remote site at {0}", url);

                    return new ApiResult { Status = ApiResultStatus.Failed };
                }
            }

            return new ApiResult {Status = ApiResultStatus.OK, Text = importText};
        }
    }
}