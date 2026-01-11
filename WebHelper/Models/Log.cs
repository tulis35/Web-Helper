using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelper.Models.Enums;

namespace WebHelper.Models
{
    public class Log
    {
        public string FileName { get; private set; }
        public string Name { get; private set; }
        public DateTime Created { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public string Content { get; set; }

        public Log(string fileName)
        {
            FileName = fileName;
            Name = FileName.Split(".txt").First();

            string[] vals = Name.Split("_");
            Created = Convert.ToDateTime(vals[1]);

            switch (vals[0])
            {
                case "Error":
                    LogLevel = LogLevel.Error;
                    break;
                case "Log":
                    LogLevel = LogLevel.Info;
                    break;
            }

            Content = "";
        }
    }
}
