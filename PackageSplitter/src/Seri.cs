using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;

namespace PackageSplitter
{
    public class Seri
    {
        private static Logger _log;
        public static Logger Log
        {
            get
            {
                if (_log == null)
                    throw new ArgumentNullException("SeriLog not loaded");
                return _log;
            }
        }
        public static void InitConfig(IConfiguration congig)
        {
            _log = new LoggerConfiguration()
                .ReadFrom.Configuration(congig)
                .CreateLogger();
        }
    }
}
