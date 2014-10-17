using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CJP.ContentSync.Models;
using Orchard.Data;

namespace CJP.ContentSync.Services {
    public class DefaultTextRedactionService : ITextRedactionService {
        private readonly IRepository<RedactionRecord> _repository;

        public DefaultTextRedactionService(IRepository<RedactionRecord> repository) {
            _repository = repository;
        }

        public string RedactText(string text) {
            var redactions = GetRedactions();

            foreach (var redaction in redactions) {
                text = Regex.Replace(text, redaction.Regex, CreatePlaceholder(redaction.Placeholder));
            }

            return text;
        }

        public string RestoreText(string text)
        {
            var redactions = GetRedactions();

            foreach (var redaction in redactions){
                text = text.Replace(CreatePlaceholder(redaction.Placeholder), redaction.ReplaceWith);
            }

            return text;
        }

        public void AddRedaction(RedactionRecord redaction) {
            _repository.Create(redaction);
        }

        public void UpdateRedaction(RedactionRecord redaction){
            _repository.Update(redaction);
        }

        public void DeleteRedaction(RedactionRecord redaction){
            _repository.Delete(redaction);
        }

        public RedactionRecord GetRedaction(int id) {
            return _repository.Get(id);
        }

        public IEnumerable<RedactionRecord> GetRedactions() {
            return _repository.Table.ToList();
        }

        private string CreatePlaceholder(string token) {
            return "$##" + token + "##";
        }
    }
}