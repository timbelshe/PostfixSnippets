using Xunit;

namespace PostfixSnippets.Tests
{
    public class BasePostfixSnippetsUnitTest
    {
    }

    public static class XunitHelpers
    {
        public static void Fail(string message = null) =>
            throw new Xunit.Sdk.XunitException(message);
    }

    public class XunitHelpersTests
    {
        [Fact]
        public void Fail_throws_XunitException_when_message_is_false()
        {
            Assert.Throws<Xunit.Sdk.XunitException>(() => XunitHelpers.Fail());
        }
    }
}