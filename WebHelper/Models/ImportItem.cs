using System;

namespace WebHelper.Models
{
    public class ImportItem
    {
        public string ItemName { get; private set; }
        public Int16 ItemType { get; private set; }
        public string ItemParent { get; private set; }
        public bool ToImport { get;  set; }
        public string ErrorMsg { get;  set; }
        public string ItemFileName { get; private set; }

        public ImportItem(string itemName, Int16 itemType, string itemParent, string itemFileName) 
        { 
            ItemName = itemName;
            ItemType = itemType;
            ItemParent = itemParent;
            ToImport = false;
            ErrorMsg = "";
            ItemFileName = itemFileName;
        }
    }
}
