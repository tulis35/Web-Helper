using System;
using System.IO;
using System.Text.Json;
using WebHelper.Models;

namespace WebHelper.Data.ForObjects
{
    public static class UserSettingsManager
    {
        private static string SavePath = ".\\wwwroot\\UserSettings.json";

        public static UserSettings GetUserSettings()
        {
            if (!File.Exists(SavePath))
                return new UserSettings();
            else
            {
                try
                {
                    string jsonString = File.ReadAllText(SavePath);
                    return JsonSerializer.Deserialize<UserSettings>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return new UserSettings();
                }
            }
        }

        public static Result Save(UserSettings userSettings)
        {
            try
            {
                if (userSettings.NumberOfLastItemsSaved < 0)
                    userSettings.NumberOfLastItemsSaved = 0;
                if(userSettings.MaxFileSizeInMB < 0)
                    userSettings.MaxFileSizeInMB = 0;

                while (userSettings.NumberOfLastItemsSaved < userSettings.LastShownItems.Count)
                    userSettings.LastShownItems.RemoveAt(userSettings.LastShownItems.Count - 1);

                string json = JsonSerializer.Serialize(userSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SavePath, json);
                return new Result(Models.Enums.ResultState.Success, "Nastavení úspěšně uloženo.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(Models.Enums.ResultState.Error, ex.Message);
            }
        }

        public static Result AddFavorite(string entryName, ItemFile itemFile)
        {
            try
            {
                UserSettings Settings = GetUserSettings();

                UserSettingsItems settingsItem = new UserSettingsItems(entryName, itemFile);

                for (int i = 0; i < Settings.FavoriteItems.Count; i++)
                {
                    if (Settings.FavoriteItems[i].ItemName == settingsItem.ItemName)
                        return new Result(Models.Enums.ResultState.Error, "Záznam už je v oblíbených!");
                }

                Settings.FavoriteItems.Add(settingsItem);

                return Save(Settings);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(Models.Enums.ResultState.Error, ex.Message);
            }
        }

        public static Result AddLastShownItem(string entryName, ItemFile itemFile)
        {
            try
            {
                UserSettings Settings = GetUserSettings();

                UserSettingsItems settingsItem = new UserSettingsItems(entryName, itemFile);

                for (int i = Settings.LastShownItems.Count - 1; i >=0; i--)
                {
                    if (Settings.LastShownItems[i].ItemName == settingsItem.ItemName)
                        Settings.LastShownItems.RemoveAt(i);
                }

                Settings.LastShownItems.Insert(0, settingsItem);

                return Save(Settings);
                
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(Models.Enums.ResultState.Error, ex.Message);
            }
        }
    }
}
