using System.Collections.Generic;
using System.Linq;
using ListDllsParser.Model;
using Sprache;

namespace ListDllsParser.Dsl
{
    public class ListDllsParser : IParser<string, IEnumerable<ProcessInfo>>
    {
        public IEnumerable<ProcessInfo> Parse(string input)
        {
            var result = ListDllsGrammar.ProcessInfoCollection.TryParse(input);

            if (result.WasSuccessful)
                return result.Value;

            return Enumerable.Empty<ProcessInfo>();
        }
    }
}