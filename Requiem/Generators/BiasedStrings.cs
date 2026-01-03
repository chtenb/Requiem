using CsCheck;

namespace Requiem.Generators;

internal static class BiasedStrings
{
    public static readonly Gen<string> String = Gen.Frequency(
        (10, Gen.Const("a")),
        (10, Gen.Const("b")),
        (10, Gen.Const(" ")),
        (10, Gen.Const("")),
        (10, Gen.Const("\n")),
        (50, Gen.String)
    );

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
}