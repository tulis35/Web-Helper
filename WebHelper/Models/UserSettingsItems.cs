using System.Text.Json.Serialization;
using WebHelper.Data.ForObjects;
using WebHelper.Models.Enums;

namespace WebHelper.Models
{
    public class UserSettingsItems
    {
        [JsonIgnore]
        public bool Save { get; set; }
        public string EntryName { get; set; }
        public string ItemName { get; set; }
        public string ItemFileName { get; set; }
        public ItemFileType ItemFileType { get; set; }
        public bool IsShortCut { get; set; }

        public UserSettingsItems() 
        {
            EntryName = string.Empty;
            ItemName = string.Empty;
            ItemFileName = string.Empty;
            Save = true;
            IsShortCut = false;
        }

        public UserSettingsItems(string entryName, string itemName, string itemFileName, ItemFileType itemFileType) 
        {
            IsShortCut = entryName == GenericManager.ReturnStringForAllTasks;
            EntryName = IsShortCut ? "Úkoly" : entryName;
            ItemName = itemName;
            ItemFileName = itemFileName;
            Save = true;
            ItemFileType = itemFileType;
        }

        public UserSettingsItems(string entryName, ItemFile itemFile)
        {
            IsShortCut = entryName == GenericManager.ReturnStringForAllTasks;
            EntryName = IsShortCut ? "Úkoly" : entryName;
            ItemName = itemFile.Name;
            ItemFileName = itemFile.FileName;
            Save = true;
            ItemFileType = itemFile.ItemFileType;
        }

        public string ShowName()
        {
            return EntryName + " > " + ItemName;
        }
    }
}
