using System.Collections.Generic;

namespace WebHelper.Models
{
    public class UserSettings
    {
        public int NumberOfLastItemsSaved { get; set; }
        public List<UserSettingsItems> LastShownItems { get; set; }
        public List<UserSettingsItems> FavoriteItems { get; set; }
        public int MaxFileSizeInMB { get; set; }

        public UserSettings()
        {
            NumberOfLastItemsSaved = 5;
            LastShownItems = new List<UserSettingsItems>();
            FavoriteItems = new List<UserSettingsItems>();
            MaxFileSizeInMB = 20;
        }

        public UserSettings(int numberOfLastItemsSaved, List<UserSettingsItems> lastShownItems, List<UserSettingsItems> favoriteItems, int maxSize)
        {
            NumberOfLastItemsSaved = numberOfLastItemsSaved;
            LastShownItems = lastShownItems;
            FavoriteItems = favoriteItems;
            MaxFileSizeInMB = maxSize;
        }        
    }
}
