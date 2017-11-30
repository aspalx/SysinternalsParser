using System.Collections.Generic;

namespace ListDllsParser.Model
{
    public struct ProcessInfo
    {
        public string Name { get; set; }
        public int Pid { get; set; }
        public string CommandLine { get; set; }
        public IEnumerable<ProcessModuleInfo> Modules { get; set; }
    }
}