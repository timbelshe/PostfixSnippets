using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using Moq;
using PostfixSnippets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PostfixSnippets.Tests
{
    public class PostfixSnippetsCompletionProviderTests : IDisposable
    {
        private MockRepository mockRepository;

        public PostfixSnippetsCompletionProviderTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private PostfixSnippetsCompletionProvider CreateProvider()
        {
            return new PostfixSnippetsCompletionProvider();
        }

        [Theory]
        [ClassData(typeof(ShouldTriggerCompletionTestData))]
        public void ShouldTriggerCompletion_StateUnderTest_ExpectedBehavior(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
        {
            // Arrange
            var unitUnderTest = this.CreateProvider();

            // Act
            var result = unitUnderTest.ShouldTriggerCompletion(
                text,
                caretPosition,
                trigger,
                options);

            // Assert
            Assert.True(result);
        }

        public class ShouldTriggerCompletionTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { SourceText.From("1 + 1"), 5, new CompletionTrigger(), null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        //[Fact]
        //public async Task ProvideCompletionsAsync_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateProvider();
        //    CompletionContext context = TODO;

        //    // Act
        //    await unitUnderTest.ProvideCompletionsAsync(
        //        context);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetDescriptionAsync_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateProvider();
        //    Document document = TODO;
        //    CompletionItem item = TODO;
        //    CancellationToken cancellationToken = TODO;

        //    // Act
        //    var result = await unitUnderTest.GetDescriptionAsync(
        //        document,
        //        item,
        //        cancellationToken);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public async Task GetChangeAsync_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateProvider();
        //    Document document = TODO;
        //    CompletionItem item = TODO;
        //    char? commitKey = TODO;
        //    CancellationToken cancellationToken = TODO;

        //    // Act
        //    var result = await unitUnderTest.GetChangeAsync(
        //        document,
        //        item,
        //        commitKey,
        //        cancellationToken);

        //    // Assert
        //    Assert.True(false);
        //}
    }
}