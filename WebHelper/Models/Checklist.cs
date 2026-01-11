using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebHelper.Models
{
    public class Checklist
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public List<ChecklistItem> Items { get; set; }        
        public string FileExt { get; set; }
        public bool IsSaved { get; private set; }

        public Checklist() 
        {
            FileName = string.Empty;
            Items = new List<ChecklistItem>();

            Name = string.Empty;
            FileExt = string.Empty;

            IsSaved = false;
        }
        public Checklist(string fileName, bool isSaved = false)
        {
            FileName = fileName;
            Items = new List<ChecklistItem>();

            string[] values = FileName.Split(".");

            Name = values[0];
            FileExt = values[1];

            IsSaved = isSaved;
        }

        public Checklist(string fileName, List<ChecklistItem> items, bool isSaved = true)
        {
            FileName = fileName;
            Items = items;

            string[] values = FileName.Split(".");

            Name = values[0];
            FileExt = values[1];

            IsSaved = isSaved;
        }
    }
}
