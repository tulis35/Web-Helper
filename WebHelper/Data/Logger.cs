using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelper.Models;
using WebHelper.Data;
using System.IO;
using WebHelper.Models.Enums;

namespace WebHelper.Data
{
    public class Logger
    {
        private const string LogFolder = @".\Logs";

        private static string LogName(LogLevel logLevel)
        {
            string name = "";

            switch (logLevel)
            {
                case LogLevel.Error:
                    name = "Error";
                    break;
                case LogLevel.Info:
                    name = "Log";
                    break;
            }

            return name;
        }
        private static string LogFileName(LogLevel logLevel)
        {
            string name = "";

            switch (logLevel)
            {
                case LogLevel.Error:
                    name = "Error_";
                    break;
                default:
                    name = "Log_";
                    break;
            }

            string date = (DateTime.Now).ToString("yyyy-MM-dd");

            return name + date + ".txt";
        }
        private static void CreateFile(string fileName, string text)
        {
            if(!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);

            File.AppendAllText(LogFolder + "\\" + fileName, text);
        }
        
        public static void Log(LogLevel logLevel, string msg)
        {
            string log = @"/********************************************************************************************/" + "\r\n"
                        + @"LogLevel: " + LogName(logLevel) + "\r\n"
                        + @"TS: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\r\n"
                        + @"Message: " + msg + "\r\n"
                        ;

            CreateFile(LogFileName(logLevel), log);
        }

        public static void Log(Exception ex)
        {
            string log = @"/********************************************************************************************/" + "\r\n"
                        + @"LogLevel: " + LogName(LogLevel.Error) + "\r\n"
                        + @"TS: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "\r\n"
                        + @"Message: " + ex.Message + "\r\n"
                        + @"Full Error: " + ex.ToString() + "\r\n"
                        ;

            CreateFile(LogFileName(LogLevel.Error), log);
        }
    }
}
