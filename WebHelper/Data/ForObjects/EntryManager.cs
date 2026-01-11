using System.Text.Json;
using System.Threading.Tasks;
using System;
using WebHelper.Models.Enums;
using WebHelper.Models;
using WebHelper.Data.ForObjects;
using System.IO;
using System.Collections.Generic;

namespace WebHelper.Data.ForObjects
{
    public class EntryManager
    {
        public static Result CreateNewEntry(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new Result(ResultState.Error, "Musí být vyplněn název!");

            try
            {
                GenericManager.IsValidFileName(name);
                Entry newEntry = new Entry(name);

                if (File.Exists($@"{GenericManager.SavesFolder}/{name}.json"))
                {
                    return new Result(ResultState.Error, $"Soubor s názvem \"{name}\" již existuje.");
                }

                string json = JsonSerializer.Serialize(newEntry, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($@"{GenericManager.SavesFolder}/{name}.json", json);

                return new Result(ResultState.Success, $"Záznam \"{name}\" vytvořen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{name}\" nešel vytvořit." + "\r\n" + ex.Message);
            }

        }

        public static Result SaveEntry(Entry entry)
        {
            if (entry == null)
                return new Result(ResultState.Error, "Prázdný záznam!");

            try
            {
                string json = JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($@"{GenericManager.SavesFolder}/{entry.Name}.json", json);

                return new Result(ResultState.Success, $"Záznam \"{entry.Name}\" vytvořen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{entry.Name}\" nešel vytvořit." + "\r\n" + ex.Message);
            }

        }

        public static Result RenameEntry(Entry entry, string newName)
        {
            try
            {
                if (File.Exists($@"{GenericManager.SavesFolder}/{newName}.json"))
                {
                    return new Result(ResultState.Error, $"Soubor s názvem \"{newName}\" již existuje!");
                }
                GenericManager.IsValidFileName(newName);
                File.Move($@"{GenericManager.SavesFolder}/{entry.Name}.json", $@"{GenericManager.SavesFolder}/{newName}.json");
                entry.Name = newName;
                SaveEntry(entry);

                return new Result(ResultState.Success, "Soubor přejmenován.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{entry.Name}\" nešel přejmenovat." + "\r\n" + ex.Message);
            }
        }

        public static Result DeleteEntry(string entryName)
        {
            try
            {
                string jsonString = File.ReadAllText($"{GenericManager.SavesFolder}/{entryName}.json");

                var itemJson = JsonSerializer.Deserialize<Entry>(jsonString);
                foreach (string file in itemJson.TextFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.Text));
                foreach (string file in itemJson.TableFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.Table));
                foreach (string file in itemJson.URLsFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.URLs));
                foreach (string file in itemJson.TasksFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.Task));
                foreach (string file in itemJson.PDFsFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.PDF));
                foreach (string file in itemJson.ExtAppsFiles)
                    ItemManager.DeleteItem(itemJson, new ItemFile(entryName, file, ItemFileType.ExtApp));

                File.Delete($@"{GenericManager.SavesFolder}/{entryName}.json");

                return new Result(ResultState.Success, $"Záznam \"{entryName}\" smazán.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Success, $"Záznam \"{entryName}\" nesmazán. \r\n " + ex.Message);
            }
        }

        public static List<Entry> GetEntries()
        {
            List<Entry> itemsList = new List<Entry>();

            try
            {
                string[] files = Directory.GetFiles(GenericManager.SavesFolder);

                foreach (string file in files)
                {
                    string jsonString = File.ReadAllText(file);

                    Entry item = JsonSerializer.Deserialize<Entry>(jsonString);

                    itemsList.Add(item);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return itemsList;
        }

        public static Entry GetEntry(string name)
        {
            try
            {
                string jsonString = File.ReadAllText($"{GenericManager.SavesFolder}/{name}.json");

                Entry item = JsonSerializer.Deserialize<Entry>(jsonString);

                return item;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }
    }
}
