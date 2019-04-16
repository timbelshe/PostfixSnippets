using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

[assembly: InternalsVisibleTo("PostfixSnippets.Tests")]

namespace PostfixSnippets
{
    [ExportCompletionProvider(name: nameof(PostfixSnippetsCompletionProvider), language: LanguageNames.CSharp), Shared]
    internal class PostfixSnippetsCompletionProvider : CompletionProvider
    {
        private IEnumerable<BasePostfixSnippet> Snippets;

        public PostfixSnippetsCompletionProvider()
        {
            Snippets = new List<BasePostfixSnippet>
            {
                new VarSnippet()
            };
        }

        public override bool ShouldTriggerCompletion(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
        {
            Debug.WriteLine($"ShouldTriggerCompletion text: {text}");
            Debug.WriteLine($"caretPosition: {caretPosition}");

            switch (trigger.Kind)
            {
                case CompletionTriggerKind.Insertion:
                    return ShouldTriggerCompletion(text, caretPosition);

                default:
                    return false;
            }
        }

        private bool ShouldTriggerCompletion(SourceText text, int position)
        {
            // Replace this with code that analyzes the syntax type and returns true if type = expression that returns value.

            // Provide completion if user typed "." after a non-whitespace/tab/newline char.
            var insertedCharacterPosition = position - 1;
            if (insertedCharacterPosition <= 0)
            {
                return false;
            }

            var ch = text[insertedCharacterPosition];
            var previousCh = text[insertedCharacterPosition - 1];
            return ch == '.' && (char.IsLetterOrDigit(previousCh) || char.IsPunctuation(previousCh));
        }

        public async override Task ProvideCompletionsAsync(CompletionContext context)
        {
            Debug.WriteLine($"ProvideCompletionsAsync context: {await context.Document.GetTextAsync()}");

            context.AddItems(Snippets.Select(async s => await s.GetCompletionItemAsync(context).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(r => !(r is null)));
        }

        public async override Task<CompletionDescription> GetDescriptionAsync(Document document, CompletionItem item, CancellationToken cancellationToken)
        {
            return CompletionDescription.FromText(item.Properties["Description"]);
        }

        public async override Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item, char? commitKey, CancellationToken cancellationToken)
        {
            return await Snippets.FirstOrDefault(s => s.Name == item.Properties["Name"])?.GetChangeAsync(document, item);
        }
    }
}