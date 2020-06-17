using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PackageSplitter.Model.Parser
{
    class ParserObject
    {
        public string FileName { get; private set; }

        public long FileSizeKB { get; private set; }

        public eParseStatus Status { get; private set; }

        public TimeSpan LoadTime { get; private set; }

        public ParserObject(string path)
        {
            var fileInfo = new FileInfo(path);

            FileName = fileInfo.Name;
            FileSizeKB = fileInfo.Length / 1024;
            Status = eParseStatus.Wait;
            LoadTime = TimeSpan.Zero;
        }
    }
}
