using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace PostfixSnippets
{
    public class VarSnippet : BasePostfixSnippet
    {
        public VarSnippet() : base()
        {
            Description = "Introduce Variable";
            DisplayText = "var";
            Name = "PostfixVarSnippet";
        }

        public override async Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item)
        {
            return CompletionChange.Create(new TextChange(item.Span, "new variable declaration"));
        }

        public override async Task<CompletionItem> GetCompletionItemAsync(CompletionContext context)
        {
            if (await IsApplicableAsync(context).ConfigureAwait(false))
            {
                return CompletionItem.Create(DisplayText,
                                properties: ImmutableDictionary<string, string>.Empty
                                    .Add("Name", Name)
                                    .Add("Description", Description
                                )
                            );
            }
            return null;
        }

        protected override async Task<bool> IsApplicableAsync(CompletionContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var expressionNode = root.FindToken(context.CompletionListSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().FirstOrDefault();

            return !(expressionNode is null);
        }
    }
}