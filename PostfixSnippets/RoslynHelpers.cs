using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PostfixSnippets
{
    public static class RoslynHelpers
    {
        public static SyntaxToken NoneToken => Token(SyntaxKind.None);
    }
}