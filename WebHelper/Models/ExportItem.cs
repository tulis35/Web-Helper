using System;

namespace WebHelper.Models
{
    public class ExportItem
    {
        public bool ToExport { get; set; }
        public string ItemName { get; private set; }
        public Int16 ItemType { get; private set; }
        public string ItemParent { get; private set; }
        public string ItemFileName { get; private set; }

        public ExportItem(string itemName, string itemParent, Int16 itemType, string FileName) 
        { 
            ItemName = itemName;
            ItemType = itemType;
            ItemParent = itemParent;
            ToExport = false;
            ItemFileName = FileName;
        }
    }
}
