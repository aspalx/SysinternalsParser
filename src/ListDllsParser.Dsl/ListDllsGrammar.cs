using System;
using System.Collections.Generic;
using System.Globalization;
using ListDllsParser.Model;
using Sprache;

namespace ListDllsParser.Dsl
{
    public class ListDllsGrammar
    {
        public static readonly Parser<string> Identifier =
        (from upper in Parse.Upper.Once().Text().Token()
            from other in Parse.LetterOrDigit.AtLeastOnce().Text().Token()
            select upper + other).Token();

        public static readonly Parser<string> PropertyName =
            from first in Identifier
            from second in Parse.Lower.AtLeastOnce().Text().Token().Optional()
            from colon in Parse.Char(':')
            select second.IsEmpty ? first : first + ' ' + second.GetOrDefault();

        public static readonly Parser<string> PropertyValue =
            from value in Parse.AnyChar.Until(Parse.LineEnd).Text()
            select value.Trim();

        public static readonly Parser<string> Property =
        (from name in PropertyName
            from value in PropertyValue
            select value).Token();

        public static readonly Parser<int> HexValue =
        (from prefix in Parse.String("0x").Once()
            from hex in Parse.Regex("[a-f0-9]+")
            select int.Parse(hex, NumberStyles.HexNumber)).Token();

        public static readonly Parser<DateTime> DateTimeProperty = Property
            .Select(str => DateTime.ParseExact(str, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture));

        public static readonly Parser<string> AnyCharExceptWhiteSpace =
            Parse.AnyChar.Except(Parse.WhiteSpace).AtLeastOnce().Text().Token();

        public static readonly Parser<ProcessModuleInfo> ProcessModuleInfo =
        (from @base in HexValue
            from size in HexValue
            from filePath in PropertyValue
            from verified in Property
            from publisher in Property
            from description in Property
            from product in Property
            from version in Property
            from fileVersion in Property
            from createTime in DateTimeProperty
            select new ProcessModuleInfo
            {
                Base = new IntPtr(@base),
                Size = size,
                FilePath = filePath,
                Verified = verified,
                Publisher = publisher,
                Description = description,
                Product = product,
                Version = version,
                FileVersion = fileVersion,
                CreateTime = createTime
            }).Token();

        public static readonly Parser<ProcessModuleInfo> ProcessModuleInfoNt5 =
        (from @base in HexValue
            from size in HexValue
            from version in VersionForNt5.Optional()
            from filePath in AnyCharExceptWhiteSpace
            select new ProcessModuleInfo
            {
                Base = new IntPtr(@base),
                Size = size,
                FilePath = filePath,
                Version = Version.Parse(version.GetOrDefault() ?? "0.0.0.0").ToString()
            }).Token();

        public static readonly Parser<string> VersionForNt5 =
            Parse.Digit.Or(Parse.Char('.')).AtLeastOnce().Text().Token();

        public static readonly Parser<ProcessInfo> ProcessInfo =
        (from line in Parse.Char('-').AtLeastOnce()
            from name in Parse.AnyChar.Until(Parse.String(" pid:")).Text().Token()
            from pid in Parse.Digit.AtLeastOnce().Text().Token().Select(int.Parse)
            from commandLine in Property
            from header in Parse.Regex("Base\\s+Size\\s+Path")
            from modules in ProcessModuleInfo.Many()
            select new ProcessInfo {Name = name, Pid = pid, CommandLine = commandLine, Modules = modules}).Token();

        public static readonly Parser<ProcessInfo> ProcessInfoNt5 =
        (
            from infoHeader in Parse.String("ListDLLs v2.25 - DLL lister for Win9x/NT" + Environment.NewLine +
                                            "Copyright (C) 1997-2004 Mark Russinovich" + Environment.NewLine +
                                            "Sysinternals - www.sysinternals.com").Token().Optional()
            from line in Parse.Char('-').AtLeastOnce()
            from name in Parse.AnyChar.Until(Parse.String(" pid:")).Text().Token()
            from pid in Parse.Digit.AtLeastOnce().Text().Token().Select(int.Parse)
            from commandLine in Property
            from header in Parse.Regex("Base\\s+Size\\s+Version\\s+Path")
            from modules in ProcessModuleInfoNt5.Many()
            select new ProcessInfo {Name = name, Pid = pid, CommandLine = commandLine, Modules = modules}).Token();

        public static readonly Parser<IEnumerable<ProcessInfo>> ProcessInfoCollection = ProcessInfo.Many().End();

        public static readonly Parser<IEnumerable<ProcessInfo>> ProcessInfoCollectionNt5 = ProcessInfoNt5.Many().End();
    }
}