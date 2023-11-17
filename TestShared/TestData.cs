namespace TestShared
{
    using System.Collections.Generic;

    public static class TestData
    {
        public static IEnumerable<object[]> InvalidFileNameCharacters => new[]
        {
            new object[] { "\0" },
            new object[] { "\u0001" },
            new object[] { "\u0002" },
            new object[] { "\u0003" },
            new object[] { "\u0004" },
            new object[] { "\u0005" },
            new object[] { "\u0006" },
            new object[] { "\a" },
            new object[] { "\b" },
            new object[] { "\t" },
            new object[] { "\n" },
            new object[] { "\v" },
            new object[] { "\f" },
            new object[] { "\r" },
            new object[] { "\u000e" },
            new object[] { "\u000f" },
            new object[] { "\u0010" },
            new object[] { "\u0011" },
            new object[] { "\u0012" },
            new object[] { "\u0013" },
            new object[] { "\u0014" },
            new object[] { "\u0015" },
            new object[] { "\u0016" },
            new object[] { "\u0017" },
            new object[] { "\u0018" },
            new object[] { "\u0019" },
            new object[] { "\u001a" },
            new object[] { "\u001b" },
            new object[] { "\u001c" },
            new object[] { "\u001d" },
            new object[] { "\u001e" },
            new object[] { "\u001f" },
            new object[] { "\"" },
            new object[] { "*" },
            new object[] { ":" },
            new object[] { "<" },
            new object[] { ">" },
            new object[] { "?" },
            new object[] { "|" }
        };

        public static IEnumerable<object[]> InvalidPathCharacters => new[]
        {
            new object[] { "\0" },
            new object[] { "\u0001" },
            new object[] { "\u0002" },
            new object[] { "\u0003" },
            new object[] { "\u0004" },
            new object[] { "\u0005" },
            new object[] { "\u0006" },
            new object[] { "\a" },
            new object[] { "\b" },
            new object[] { "\t" },
            new object[] { "\n" },
            new object[] { "\v" },
            new object[] { "\f" },
            new object[] { "\r" },
            new object[] { "\u000e" },
            new object[] { "\u000f" },
            new object[] { "\u0010" },
            new object[] { "\u0011" },
            new object[] { "\u0012" },
            new object[] { "\u0013" },
            new object[] { "\u0014" },
            new object[] { "\u0015" },
            new object[] { "\u0016" },
            new object[] { "\u0017" },
            new object[] { "\u0018" },
            new object[] { "\u0019" },
            new object[] { "\u001a" },
            new object[] { "\u001b" },
            new object[] { "\u001c" },
            new object[] { "\u001d" },
            new object[] { "\u001e" },
            new object[] { "\u001f" },
            new object[] { "|" }
        };

        public static IEnumerable<object[]> Whitespace => new[]
        {
            new object[] { "" },
            new object[] { "\t" },
            new object[] { "\n" },
            new object[] { "\v" },
            new object[] { "\f" },
            new object[] { "\r" },
            new object[] { " " },
            new object[] { "\u0085" },
            new object[] { "\u00a0" },
            new object[] { "\u2002" },
            new object[] { "\u2003" },
            new object[] { "\u2028" },
            new object[] { "\u2029" },
            new object[] { Globals.Whitespace }
        };
    }
}