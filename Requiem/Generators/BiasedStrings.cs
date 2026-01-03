#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

using CsCheck;
using NaughtyStrings;

namespace Requiem.Generators;

internal static class BiasedStrings
{
    public static Gen<string> String(int minLength = 0, int maxLength = 100)
    {
        var generators = new List<(int, Gen<string>)>();

        // Empty string only if minLength allows it
        if (minLength == 0)
            generators.Add((5, Gen.Const("")));

        // Single character strings only if minLength allows it
        if (minLength <= 1 && maxLength >= 1)
        {
            generators.Add((5, Gen.Const("a")));
            generators.Add((5, Gen.Const("b")));
            generators.Add((5, Gen.Const(" ")));
            generators.Add((5, Gen.Const("\n")));
        }

        // Two character strings only if minLength allows it
        if (minLength <= 2 && maxLength >= 2)
            generators.Add((5, Gen.Const("aa")));

        // Random strings within bounds - this is our fallback that always works
        generators.Add((20, Gen.String[minLength, maxLength]));

        // Helper to adjust string length to fit within bounds
        string AdjustLength(string s)
        {
            if (s.Length < minLength)
                return s + new string('x', minLength - s.Length);
            if (s.Length > maxLength)
                return s.Substring(0, maxLength);
            return s;
        }

        // Add naughty string generators with length adjustment
        generators.Add((10, _simpleNaughtyStrings.Select(AdjustLength)));
        generators.Add((1, _reservedStrings.Select(AdjustLength)));
        generators.Add((1, _numericStrings.Select(AdjustLength)));
        generators.Add((1, _specialCharacters.Select(AdjustLength)));
        generators.Add((1, _unicodeSymbols.Select(AdjustLength)));
        generators.Add((1, _unicodeSubscriptSuperscriptAccents.Select(AdjustLength)));
        generators.Add((1, _quotationMarks.Select(AdjustLength)));
        generators.Add((1, _twoByteCharacters.Select(AdjustLength)));
        generators.Add((1, _specialUnicodeCharactersUnion.Select(AdjustLength)));
        generators.Add((1, _japaneseEmoticons.Select(AdjustLength)));
        generators.Add((1, _emoji.Select(AdjustLength)));
        generators.Add((1, _rightToLeftStrings.Select(AdjustLength)));
        generators.Add((1, _trickUnicode.Select(AdjustLength)));
        generators.Add((1, _zalgoText.Select(AdjustLength)));
        generators.Add((1, _scriptInjection.Select(AdjustLength)));
        generators.Add((1, _sqlInjection.Select(AdjustLength)));
        generators.Add((1, _serverCodeInjection.Select(AdjustLength)));
        generators.Add((1, _fileInclusion.Select(AdjustLength)));
        generators.Add((1, _knownCVEsAndVulnerabilities.Select(AdjustLength)));
        generators.Add((1, _msdosWindowsSpecialFilenames.Select(AdjustLength)));
        generators.Add((1, _scunthorpeProblem.Select(AdjustLength)));
        generators.Add((1, _humaninjection.Select(AdjustLength)));
        generators.Add((1, _terminalEscapeCodes.Select(AdjustLength)));
        generators.Add((1, _iOSVulnerabilities.Select(AdjustLength)));

        return Gen.Frequency([.. generators]);
    }

    public static readonly Gen<string> FilePath = Gen.Frequency(
        (15, Gen.OneOfConst(
            "",
            ".",
            "..",
            "/",
            "\\",
            "C:\\",
            "//server/share",
            new string('a', 260), // MAX_PATH on Windows
            new string('x', 32767) // Extended path limit
        )),
        (15, WindowsReservedNames()),
        (10, PathsWithSpecialChars()),
        (10, RelativePaths()),
        (10, UncPaths()),
        (40, Gen.String.AlphaNumeric.Select(s => $"C:\\temp\\{s}.txt"))
    );

    public static readonly Gen<string> Email = Gen.Frequency(
        (10, Gen.OneOfConst(
            "",
            "@",
            "user@",
            "@domain.com",
            "user@@domain.com",
            "user@domain",
            "user@.com",
            "user@domain..com",
            new string('a', 64) + "@domain.com",
            "user@" + new string('a', 255)
        )),
        (15, ValidButUnusualEmails()),
        (75, Gen.String.AlphaNumeric.Select(Gen.String.AlphaNumeric, (local, domain) =>
            $"{local}@{domain}.com"))
    );

    public static readonly Gen<string> Url = Gen.Frequency(
        (10, Gen.OneOfConst(
            "",
            "http://",
            "https://",
            "ftp://",
            "file://",
            "http://localhost",
            "http://127.0.0.1",
            "http://[::1]",
            "http://example.com:65536",
            "http://example.com/../../../etc/passwd"
        )),
        (15, UrlsWithSpecialChars()),
        (10, UrlsWithQueryStrings()),
        (10, UrlsWithFragments()),
        (55, Gen.String.AlphaNumeric.Select(s => $"https://{s}.com"))
    );

    public static readonly Gen<string> IPv4 = Gen.Frequency(
        (30, Gen.OneOfConst(
            "0.0.0.0",
            "127.0.0.1",
            "255.255.255.255",
            "192.168.0.1",
            "10.0.0.1",
            "172.16.0.1",
            "224.0.0.1",
            "169.254.0.1",
            "256.0.0.1",
            "1.2.3",
            "1.2.3.4.5",
            "1.0.0.1"
        )),
        (70, Gen.Byte.Select(Gen.Byte, Gen.Byte, Gen.Byte, (a, b, c, d) =>
            $"{a}.{b}.{c}.{d}"))
    );

    public static readonly Gen<string> CreditCardNumber = Gen.Frequency(
        (20, Gen.OneOfConst(
            "4111111111111111",
            "5500000000000004",
            "340000000000009",
            "6011000000000004",
            "",
            "0000000000000000",
            "1234567890123456"
        )),
        (10, InvalidCreditCards()),
        (70, Gen.Long[1000000000000000L, 9999999999999999L].Select(n => n.ToString()))
    );

    public static readonly Gen<string> PhoneNumber = Gen.Frequency(
        (15, Gen.OneOfConst(
            "",
            "0",
            "1234567890",
            "+1234567890",
            "(123) 456-7890",
            "123-456-7890",
            "+1 (123) 456-7890",
            "001234567890"
        )),
        (10, PhoneNumbersWithExtensions()),
        (75, Gen.Long[1000000000L, 9999999999L].Select(n => n.ToString()))
    );

    public static readonly Gen<string> Json = Gen.Frequency(
        (15, Gen.OneOfConst(
            "{}",
            "[]",
            "null",
            "\"\"",
            "{",
            "}",
            "[",
            "]",
            "{\"key\":}",
            "{\"key\":\"value\",}",
            "{\"key\"\"value\"}"
        )),
        (10, JsonWithDeepNesting()),
        (10, JsonWithSpecialChars()),
        (65, Gen.String.Select(s => $"{{\"key\":\"{s}\"}}"))
    );

    public static readonly Gen<string> Xml = Gen.Frequency(
        (15, Gen.OneOfConst(
            "<root/>",
            "<root></root>",
            "",
            "<",
            ">",
            "<root>",
            "</root>",
            "<root><child/></root>",
            "<root attr=\"value\"/>",
            "<?xml version=\"1.0\"?><root/>"
        )),
        (10, XmlWithCdata()),
        (10, XmlWithEntities()),
        (65, Gen.String.Select(s => $"<root>{s}</root>"))
    );

    private static Gen<string> WindowsReservedNames() =>
        Gen.OneOfConst(
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4",
            "LPT1", "LPT2", "LPT3"
        );

    private static Gen<string> PathsWithSpecialChars() =>
        Gen.String.Select(s => $"C:\\temp\\{s}<>:\"|?*.txt");

    private static Gen<string> RelativePaths() =>
        Gen.Int[1, 5].Select(depth =>
            string.Join("/", Enumerable.Repeat("..", depth)) + "/file.txt");

    private static Gen<string> UncPaths() =>
        Gen.String.AlphaNumeric.Select(Gen.String.AlphaNumeric, (server, share) =>
            $"\\\\{server}\\{share}\\file.txt");

    private static Gen<string> ValidButUnusualEmails() =>
        Gen.OneOfConst(
            "user+tag@domain.com",
            "user.name@domain.com",
            "user_name@domain.com",
            "123@domain.com",
            "a@b.c"
        );

    private static Gen<string> UrlsWithSpecialChars() =>
        Gen.String.Select(s => $"https://example.com/{Uri.EscapeDataString(s)}");

    private static Gen<string> UrlsWithQueryStrings() =>
        Gen.String.Select(Gen.String, (key, value) =>
            $"https://example.com?{key}={value}");

    private static Gen<string> UrlsWithFragments() =>
        Gen.String.Select(s => $"https://example.com#{s}");

    private static Gen<string> InvalidCreditCards() =>
        Gen.OneOfConst(
            "1234",
            "12345678901234567890",
            "abcd-efgh-ijkl-mnop",
            "1234-5678-9012-3456"
        );

    private static Gen<string> PhoneNumbersWithExtensions() =>
        Gen.Long[1000000000L, 9999999999L].Select(Gen.Int[1, 9999], (phone, ext) =>
            $"{phone} ext. {ext}");

    private static Gen<string> JsonWithDeepNesting() =>
        Gen.Int[5, 20].Select(depth =>
        {
            var json = "";
            for (int i = 0; i < depth; i++) json += "{\"nested\":";
            json += "\"value\"";
            for (int i = 0; i < depth; i++) json += "}";
            return json;
        });

    private static Gen<string> JsonWithSpecialChars() =>
        Gen.OneOfConst(
            "{\"key\":\"value\\nwith\\nnewlines\"}",
            "{\"key\":\"value\\twith\\ttabs\"}",
            "{\"key\":\"value\\\"with\\\"quotes\"}",
            "{\"key\":\"value\\\\with\\\\backslashes\"}"
        );

    private static Gen<string> XmlWithCdata() =>
        Gen.String.Select(s => $"<root><![CDATA[{s}]]></root>");

    private static Gen<string> XmlWithEntities() =>
        Gen.OneOfConst(
            "<root><>&\"'</root>",
            "<root>&#60;&#62;&#38;</root>"
        );

    // NaughtyStrings category generators
    private static readonly Gen<string> _reservedStrings =
        Gen.Int[0, TheNaughtyStrings.ReservedStrings.Count - 1].Select(i => TheNaughtyStrings.ReservedStrings[i]!);

    private static readonly Gen<string> _numericStrings =
        Gen.Int[0, TheNaughtyStrings.NumericStrings.Count - 1].Select(i => TheNaughtyStrings.NumericStrings[i]!);

    private static readonly Gen<string> _specialCharacters =
        Gen.Int[0, TheNaughtyStrings.SpecialCharacters.Count - 1].Select(i => TheNaughtyStrings.SpecialCharacters[i]!);

    private static readonly Gen<string> _unicodeSymbols =
        Gen.Int[0, TheNaughtyStrings.UnicodeSymbols.Count - 1].Select(i => TheNaughtyStrings.UnicodeSymbols[i]!);

    private static readonly Gen<string> _unicodeSubscriptSuperscriptAccents =
        Gen.Int[0, TheNaughtyStrings.UnicodeSubscriptSuperscriptAccents.Count - 1].Select(i => TheNaughtyStrings.UnicodeSubscriptSuperscriptAccents[i]!);

    private static readonly Gen<string> _quotationMarks =
        Gen.Int[0, TheNaughtyStrings.QuotationMarks.Count - 1].Select(i => TheNaughtyStrings.QuotationMarks[i]!);

    private static readonly Gen<string> _twoByteCharacters =
        Gen.Int[0, TheNaughtyStrings.TwoByteCharacters.Count - 1].Select(i => TheNaughtyStrings.TwoByteCharacters[i]!);

    private static readonly Gen<string> _specialUnicodeCharactersUnion =
        Gen.Int[0, TheNaughtyStrings.SpecialUnicodeCharactersUnion.Count - 1].Select(i => TheNaughtyStrings.SpecialUnicodeCharactersUnion[i]!);

    private static readonly Gen<string> _japaneseEmoticons =
        Gen.Int[0, TheNaughtyStrings.JapaneseEmoticons.Count - 1].Select(i => TheNaughtyStrings.JapaneseEmoticons[i]!);

    private static readonly Gen<string> _emoji =
        Gen.Int[0, TheNaughtyStrings.Emoji.Count - 1].Select(i => TheNaughtyStrings.Emoji[i]!);

    private static readonly Gen<string> _rightToLeftStrings =
        Gen.Int[0, TheNaughtyStrings.RightToLeftStrings.Count - 1].Select(i => TheNaughtyStrings.RightToLeftStrings[i]!);

    private static readonly Gen<string> _trickUnicode =
        Gen.Int[0, TheNaughtyStrings.TrickUnicode.Count - 1].Select(i => TheNaughtyStrings.TrickUnicode[i]!);

    private static readonly Gen<string> _zalgoText =
        Gen.Int[0, TheNaughtyStrings.ZalgoText.Count - 1].Select(i => TheNaughtyStrings.ZalgoText[i]!);

    private static readonly Gen<string> _scriptInjection =
        Gen.Int[0, TheNaughtyStrings.ScriptInjection.Count - 1].Select(i => TheNaughtyStrings.ScriptInjection[i]!);

    private static readonly Gen<string> _sqlInjection =
        Gen.Int[0, TheNaughtyStrings.SQLInjection.Count - 1].Select(i => TheNaughtyStrings.SQLInjection[i]!);

    private static readonly Gen<string> _serverCodeInjection =
        Gen.Int[0, TheNaughtyStrings.ServerCodeInjection.Count - 1].Select(i => TheNaughtyStrings.ServerCodeInjection[i]!);

    private static readonly Gen<string> _fileInclusion =
        Gen.Int[0, TheNaughtyStrings.FileInclusion.Count - 1].Select(i => TheNaughtyStrings.FileInclusion[i]!);

    private static readonly Gen<string> _knownCVEsAndVulnerabilities =
        Gen.Int[0, TheNaughtyStrings.KnownCVEsandVulnerabilities.Count - 1].Select(i => TheNaughtyStrings.KnownCVEsandVulnerabilities[i]!);

    private static readonly Gen<string> _msdosWindowsSpecialFilenames =
        Gen.Int[0, TheNaughtyStrings.MSDOSWindowsSpecialFilenames.Count - 1].Select(i => TheNaughtyStrings.MSDOSWindowsSpecialFilenames[i]!);

    private static readonly Gen<string> _scunthorpeProblem =
        Gen.Int[0, TheNaughtyStrings.ScunthorpeProblem.Count - 1].Select(i => TheNaughtyStrings.ScunthorpeProblem[i]!);

    private static readonly Gen<string> _humaninjection =
        Gen.Int[0, TheNaughtyStrings.Humaninjection.Count - 1].Select(i => TheNaughtyStrings.Humaninjection[i]!);

    private static readonly Gen<string> _terminalEscapeCodes =
        Gen.Int[0, TheNaughtyStrings.Terminalescapecodes.Count - 1].Select(i => TheNaughtyStrings.Terminalescapecodes[i]!);

    private static readonly Gen<string> _iOSVulnerabilities =
        Gen.Int[0, TheNaughtyStrings.iOSVulnerabilities.Count - 1].Select(i => TheNaughtyStrings.iOSVulnerabilities[i]!);

    // Simple (short) naughty strings
    private static readonly string[] _simpleNaughtyStringsList = new[]
    {
        // Numbers as strings
        "0",
        "1",
        "-1",
        "999",
        "NaN",
        "Infinity",
        
        // Single special characters
        "!",
        "@",
        "#",
        "$",
        "%",
        "^",
        "&",
        "*",
        "(",
        ")",
        ",",
        ";",
        ":",
        "|",
        
        // Quotes
        "'",
        "\"",
        "`",
        
        // Brackets
        "[",
        "]",
        "{",
        "}",
        
        // Slashes
        "/",
        "\\",
        "//",
        "\\\\",
        
        // Dots
        ".",
        "..",
        "...",
        
        // Whitespace variations
        " ",
        "\t",
        "\r\n",
        " \t\n",
        
        // Path traversal (short)
        "..",
        "../",
        "..\\",
    };

    private static readonly Gen<string> _simpleNaughtyStrings =
        Gen.Int[0, _simpleNaughtyStringsList.Length - 1].Select(i => _simpleNaughtyStringsList[i]);
}