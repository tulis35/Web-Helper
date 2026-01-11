using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using WebHelper.Models;

namespace WebHelper.Data.ForObjects
{
    public class LogManager
    {
        public static List<Log> GetLogs()
        {
            List<Log> logs = new List<Log>();
            try
            {

                string[] files = Directory.GetFiles(@".\Logs");

                foreach (string file in files)
                    logs.Add(new Log(file.Replace(@".\Logs\", "")));

                logs.Sort((l1, l2) => l2.Created.CompareTo(l1.Created));

                return logs;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static Log GetLog(string file)
        {
            try
            {
                Log log = new Log(file);
                log.Content = File.ReadAllText($@".\Logs\{log.FileName}");
                return log;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }
    }
}
