using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
            var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            var expressionNode = root
                .FindToken(item.Span.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<ExpressionStatementSyntax>()
                .FirstOrDefault();

            TypeSyntax variableType = IdentifierName("var");
            var variableDeclaration = VariableDeclaration(variableType);

            var expression = EqualsValueClause(expressionNode
                .Expression
                .WithoutTrivia()
                .WithLeadingTrivia(ParseLeadingTrivia(" ")));

            variableDeclaration = variableDeclaration.AddVariables(VariableDeclarator(
                Identifier(string.Empty)
                    .WithLeadingTrivia(ParseLeadingTrivia(" "))
                    .WithTrailingTrivia(ParseTrailingTrivia(" ")),
                null,
                expression));

            var localDeclaration = LocalDeclarationStatement(variableDeclaration).WithLeadingTrivia(expressionNode.GetLeadingTrivia());

            var lastDotToken = localDeclaration
                .DescendantTokens(localDeclaration.FullSpan, _ => true)
                .SingleOrDefault(t => t.IsKind(SyntaxKind.DotToken) && t.HasTrailingTrivia && t.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia));

            var dotTokenParent = lastDotToken.Parent;

            var newDotTokenParent = dotTokenParent.ReplaceToken(
                lastDotToken,
                MissingToken(SyntaxKind.DotToken));

            localDeclaration = localDeclaration
                .ReplaceNode(dotTokenParent, newDotTokenParent)
                .WithTrailingTrivia(ParseTrailingTrivia("\n"));

            var newRoot = root.ReplaceNode(expressionNode, localDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            var textChanges = await newDocument.GetTextChangesAsync(document);

            return CompletionChange.Create(textChanges.FirstOrDefault(), newPosition: expressionNode.Span.Start + 4);
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

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var lastDotToken = node.DescendantTokens(node.FullSpan, n => true).SingleOrDefault(t => t.IsKind(SyntaxKind.DotToken)
                                   && t.HasTrailingTrivia
                                   && t.TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia));

            return null;
        }
    }
}