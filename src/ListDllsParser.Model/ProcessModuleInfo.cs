using System;

namespace ListDllsParser.Model
{
    public struct ProcessModuleInfo
    {
        public IntPtr Base { get; set; }
        public int Size { get; set; }
        public string FilePath { get; set; }
        public string Verified { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string Version { get; set; }
        public string FileVersion { get; set; }
        public DateTime CreateTime { get; set; }
    }
}