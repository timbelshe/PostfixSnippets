using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;

namespace PostfixSnippets
{
    public abstract class BasePostfixSnippet
    {
        public string DisplayText;
        public string Name;

        public string Description { get; set; }

        public abstract Task<CompletionItem> GetCompletionItemAsync(CompletionContext context);

        protected abstract Task<bool> IsApplicableAsync(CompletionContext context);

        public abstract Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item);
    }
}