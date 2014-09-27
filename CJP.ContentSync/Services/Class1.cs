using System;
using System.Collections.Generic;
using Orchard;

namespace CJP.ContentSync.Services
{
    //todo: import/export step to delete all content except by id or content type (maybe override data import to log all the ids that are to be imported and then delete all but those?)
    //todo: conditional recipe step
    //todo: create command to execute pending migrations
    //todo: create ui to execute pending migrations
    //todo: create a task that can be scheduled to export site data and publish it somewhere
    //todo: create a task that can be scheduled to download and execute an export

    public interface IExportStorageAndRetrievalService : IDependency
    {
        string GetLatestExport();
        string GetExportAt(DateTime dateTime);
        void PublishExport(string export);
        IEnumerable<DateTime> GetAvailableExportDates();
    }

    public class ExportStorageAndRetrievalService : IExportStorageAndRetrievalService {
        public string GetLatestExport() {
            throw new NotImplementedException();
        }

        public string GetExportAt(DateTime dateTime) {
            throw new NotImplementedException();
        }

        public void PublishExport(string export) {
            throw new NotImplementedException();
        }

        public IEnumerable<DateTime> GetAvailableExportDates() {
            throw new NotImplementedException();
        }
    }
}