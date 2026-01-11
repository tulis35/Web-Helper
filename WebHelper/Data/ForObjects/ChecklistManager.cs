using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using WebHelper.Models;
using WebHelper.Models.Enums;

namespace WebHelper.Data.ForObjects
{
    public class ChecklistManager
    {
        public static List<Checklist> GetChecklists(bool GetSaved)
        {
            try
            {
                string folder = GetChecklistFolder(GetSaved);
                List<Checklist> checklists = new List<Checklist>();
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string[] files = Directory.GetFiles(folder);

                for ( int i = 0; i < files.Length; i++)
                {
                    string jsonString = File.ReadAllText(files[i]);

                    var itemJson = JsonSerializer.Deserialize<Checklist>(jsonString);

                    Checklist checklist = new Checklist(itemJson.FileName, GetSaved);
                    foreach (ChecklistItem item in itemJson.Items)
                        checklist.Items.Add(new ChecklistItem(item.Text, item.Rank, item.IsChecked));

                    checklists.Add(checklist);
                }

                return checklists;

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static Checklist GetChecklist(string fileName, bool GetSaved)
        {
            try
            {
                string folder = GetChecklistFolder(GetSaved);
                Checklist checklist = new Checklist(fileName, GetSaved);
                string jsonString = File.ReadAllText($@"{folder}/{checklist.FileName}");

                var itemJson = JsonSerializer.Deserialize<Checklist>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                foreach (ChecklistItem item in itemJson.Items)
                    checklist.Items.Add(new ChecklistItem(item.Text, item.Rank, item.IsChecked));

                checklist.Items.Sort((i1, i2) => i1.Rank.CompareTo(i2.Rank));

                return checklist;

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static Result RenameChecklist(Checklist checklist, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return new Result(ResultState.Error, "Nové jméno musí být vyplněno!");

            try
            {
                GenericManager.IsValidFileName(newName);
                newName = newName + "." + checklist.FileExt;
                string folder = GetChecklistFolder(checklist.IsSaved);
                if (File.Exists($@"{folder}/{newName}"))
                    return new Result(ResultState.Error, $@"Soubor ""{checklist.Name}"" již existuje!");

                File.Move($@"{folder}/{checklist.FileName}", $@"{folder}/{newName}");

                Checklist newChecklist = new Checklist(newName, checklist.IsSaved);
                foreach (ChecklistItem item in checklist.Items)
                    newChecklist.Items.Add(new ChecklistItem(item.Text, item.Rank, item.IsChecked));

                SaveChecklist(newChecklist);

                return new Result(ResultState.Success, "Soubor byl přejmenován!");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $@"Soubor ""{checklist.Name}"" nešel přejmenovat." + "\r\n" + ex.Message);
            }
        }

        public static Result SaveChecklist(Checklist checklist)
        {
            if (checklist == null || string.IsNullOrEmpty(checklist.FileName))
                return new Result(ResultState.Error, "Není vybrán soubor!");

            try
            {
                GenericManager.IsValidFileName(checklist.FileName);
                GenericManager.CreateChecklistFolder();
                if (checklist.Items.Count > 0)
                    checklist.Items.Sort((i1, i2) => i1.Rank.CompareTo(i2.Rank));

                string folder = GetChecklistFolder(checklist.IsSaved);
                string json = JsonSerializer.Serialize(checklist, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($@"{folder}/{checklist.Name}.json", json);

                return new Result(ResultState.Success, $"Záznam \"{checklist.Name}\" uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $@"Soubor ""{checklist.Name}"" nešel uložit." + "\r\n" + ex.Message);
            }
        }

        public static Result DeleteChecklist(Checklist checklist)
        {
            if (checklist == null)
                return new Result(ResultState.Error, "Není vybrán soubor!");

            try
            {
                string folder = GetChecklistFolder(checklist.IsSaved);
                File.Delete($@"{folder}/{checklist.FileName}");

                return new Result(ResultState.Success, $@"Soubor ""{checklist.Name}"" smazán.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{checklist.Name}\" nesmazán. \r\n " + ex.Message);
            }
        }

        private static string GetChecklistFolder(bool IsSaved)
        {
            return IsSaved ? GenericManager.SavedChecklistFolder : GenericManager.ChecklistFolder;
        }
    }
}
