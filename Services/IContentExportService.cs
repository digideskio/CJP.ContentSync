using System;
using CJP.ContentSync.Models;
using Orchard;

namespace CJP.ContentSync.Services
{
    public interface IContentExportService : IDependency {
        string GetContentExportText();
        ApiResult GetContentExportFromUrl(string url, string username, string password);
    }
}