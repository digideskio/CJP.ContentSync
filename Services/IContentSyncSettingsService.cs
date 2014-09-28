using System.Collections.Generic;
using Orchard;
using Orchard.ImportExport.Models;

namespace CJP.ContentSync.Services {
    public interface IContentSyncSettingsService : IDependency
    {
        ExportOptions GetExportOptions();
        IEnumerable<string> GetContentTypes();
    }
}