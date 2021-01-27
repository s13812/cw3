using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Services
{
    public class FileLoggingService : ILoggingService
    {
        public string path = @"requestsLog.txt";

        public void Log(string message)
        {
            StreamWriter sw = File.AppendText(path);
            sw.WriteLine($"{DateTime.Now}: {message}");
            sw.Dispose();
        }
    }
}
