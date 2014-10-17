using System.Collections.Generic;
using CJP.ContentSync.Models;
using Orchard;

namespace CJP.ContentSync.Services {
    public interface ITextRedactionService : IDependency
    {
        string RedactText(string text);
        string RestoreText(string text);

        void AddRedaction(RedactionRecord redaction);
        void UpdateRedaction(RedactionRecord redaction);
        void DeleteRedaction(RedactionRecord redaction);
        RedactionRecord GetRedaction(int id);
        IEnumerable<RedactionRecord> GetRedactions();
    }
}