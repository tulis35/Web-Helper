using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHelper.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebHelper.Models.Enums;
using System.Xml;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using WebHelper.Data.ForObjects;
using Microsoft.AspNetCore.Http;

namespace WebHelper.Data
{
    public class FileManager : IFileManager
    {
        #region Params
        public const string SavesFolder = GenericManager.SavesFolder;
        public const string TmpFolder = GenericManager.TmpFolder;
        public long MaxImportFileSize = GenericManager.MaxImportFileSize;
        public const string ChecklistFolder = GenericManager.ChecklistFolder;
        public const string SavedChecklistFolder = GenericManager.SavedChecklistFolder;
        public const string ReturnStringForAllTasks = GenericManager.ReturnStringForAllTasks;
        #endregion

        #region Stuff
        public async Task CreateFolders()
        {
            GenericManager.CreateFolders();
        }

        public async Task ClearTmp()
        {
            GenericManager.ClearTmp();
        }

        public async Task ClearTmpFile(string fileName)
        {
            GenericManager.ClearTmpFile(fileName);
        }
        public async Task ClearRmpFolder(string folderName)
        {
            GenericManager.ClearRmpFolder(folderName);
        }

        public void CreateChecklistFolder()
        {
            GenericManager.CreateChecklistFolder();
        }

        public async Task<Result> CreateExport(List<ExportItem> exports, string ExportName)
        {
            return null;
            /*try
            {
                if (exports == null || exports.Count == 0)
                    return await Task.FromResult(new Result(ResultState.Error, "Prázdný seznam."));
                if (Directory.Exists(TmpFolder + "\\" + ExportName) || File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{ExportName}.zip"))
                    return await Task.FromResult(new Result(ResultState.Error, $"Export s názve {ExportName} již existuje."));

                List<ImportItem> items = new List<ImportItem>();
                foreach (ExportItem exportItem in exports)
                    items.Add(new ImportItem(exportItem.ItemName, exportItem.ItemType, exportItem.ItemParent, exportItem.ItemFileName));

                ImportExport export = new ImportExport(new AppState().ActualVersion);
                export.ImportItems = items;
                string exportFolder = TmpFolder + "\\" + ExportName;
                Directory.CreateDirectory(exportFolder);
                string configJson = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($@"{exportFolder}\config.json", configJson);
                foreach(ImportItem import in export.ImportItems)
                {
                    string folder = "";
                    if(import.ItemType == (Int16)ItemFileType.Text)
                        folder = "Texts";
                    if (import.ItemType == (Int16)ItemFileType.Table)
                        folder = "Tables";
                    if (import.ItemType == (Int16)ItemFileType.URLs)
                        folder = "URLs";
                    if (import.ItemType == (Int16)ItemFileType.Images)
                        folder = "Images";
                    if (import.ItemType == -1)
                        folder = "Checklists";
                    if (import.ItemType == -2)
                        folder = "Logs";

                    if (!Directory.Exists(exportFolder + "\\" + folder))
                        Directory.CreateDirectory(exportFolder + "\\" + folder);

                    File.Copy((import.ItemType == -2 ? "." : SavesFolder) + $"\\{folder}\\" + import.ItemFileName, exportFolder + $"\\{folder}\\" + import.ItemFileName);
                }

                string zipTmpPath = TmpFolder + $"\\{ExportName}.zip";
                ZipFile.CreateFromDirectory(exportFolder, zipTmpPath);
                
                File.Move(zipTmpPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{ExportName}.zip");

                string logMsg = $@"Message: Export ""{ExportName}"" vytvořen. Počet exportovaných souborů: {export.ImportItems.Count}. " + "\r\n";
                Logger.Log(Models.Enums.LogLevel.Info, logMsg);

                return await Task.FromResult(new Result(ResultState.Success, $@"Export ""{ExportName}"" vytvořen na ploše."));
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                return await Task.FromResult(new Result(ResultState.Error, "Vytvoření exportu nevyšlo." + "\r\n" + ex.Message));
            }
            finally
            {
                if (File.Exists(TmpFolder + $"\\{ExportName}.zip"))
                    await ClearTmpFile(ExportName + ".zip");
                if(Directory.Exists(TmpFolder + "\\" + ExportName))
                    await ClearRmpFolder(ExportName);
            }*/
        }

        public string GetTmpFolder()
        {
            return TmpFolder;
        }
        public string GetSavesFolder()
        {
            return SavesFolder;
        }
        public string GetChecklistFolder()
        {
            return ChecklistFolder;
        }
        public long GetMaxImportFileSize()
        {
            return MaxImportFileSize;
        }
        public string GetSavedChecklistFolder()
        {
            return SavedChecklistFolder;
        }
        public string GetReturnStringForAllTasks()
        {
            return ReturnStringForAllTasks;
        }
        public Task<int> GetNmberOfAllUnfinishedTasks()
        {
            return Task.FromResult(ItemManager.GetNmberOfAllUnfinishedTasks());
        }
        public void ExtAppStart(ItemFile itemFile)
        {
            ItemManager.ExtAppStart(itemFile);
        }
        #endregion

        #region Gets
        public Task<List<Entry>> GetEntries()
        {
            return Task.FromResult(EntryManager.GetEntries());
        }

        public Task<Entry> GetEntry(string name)
        {
            return Task.FromResult(EntryManager.GetEntry(name));
        }

        public Task<ItemFile> GetTextFile(string entryName, string fileName)
        {
            return Task.FromResult(ItemManager.GetTextFile(entryName, fileName));
        }

        public Task<ItemFile> GetTableFile(string entryName, string fileName)
        {
            return Task.FromResult(ItemManager.GetTableFile(entryName, fileName));
        }

        public Task<ItemFile> GetTableURL(string entryName, string fileName)
        {
            return Task.FromResult(ItemManager.GetTableURL(entryName, fileName));
        }

        public async Task<Image> GetImage(string path)
        {
            return ItemManager.GetImage(path);
        }

        public async Task<List<Log>> GetLogs()
        {
            return LogManager.GetLogs();
        }

        public async Task<Log> GetLog(string file)
        {
            return LogManager.GetLog(file);
        }

        public async Task<List<Checklist>> GetChecklists(bool GetSaved)
        {
            return ChecklistManager.GetChecklists(GetSaved);
        }

        public async Task<Checklist> GetChecklist(string fileName, bool GetSaved)
        {
            return ChecklistManager.GetChecklist(fileName, GetSaved);
        }

        public async Task<List<ExportItem>> GetFilesForExport(bool loadItems, bool loadChecklists, bool loadLogs)
        {
            /*List<ExportItem> list = new List<ExportItem>();

            try
            {
                if (loadItems)
                {
                    List<Entry> entries = await GetEntries();
                    foreach (Entry entry in entries)
                    {
                        foreach (KeyValuePair<string, ItemFileType> kvp in entry.AllFiles)
                        {
                            ItemFile itemFile = new ItemFile(kvp.Key, kvp.Value);
                            list.Add(new ExportItem(itemFile.Name, entry.Name, (Int16)itemFile.ItemFileType, itemFile.FileName));
                        }
                    }
                }
                if (loadChecklists)
                {
                    List<Checklist> checklists = await GetChecklists();
                    foreach (Checklist checklist in checklists)
                        list.Add(new ExportItem(checklist.Name, "", -1, checklist.FileName));

                }
                if (loadLogs)
                {
                    List<Log> logs = await GetLogs();
                    foreach (Log log in logs)
                        list.Add(new ExportItem(log.Name, "", -2, log.FileName));

                }


                return await Task.FromResult(list);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                return null;
            }*/
            return null;
        }

        public async Task<ImportExport> LoadImport(string importFile)
        {
            return null;
            /*try
            {
                if (string.IsNullOrEmpty(importFile) || !File.Exists(TmpFolder + "\\" + importFile))
                    return null;

                string importFolderName = importFile.Split('.').First();
                System.IO.Compression.ZipFile.ExtractToDirectory(TmpFolder + "\\" + importFile, TmpFolder + "\\" + importFolderName);

                string jsonString = File.ReadAllText(TmpFolder + "\\" + importFolderName + "\\config.json");

                var import = JsonSerializer.Deserialize<ImportExport>(jsonString);

                foreach(ImportItem importItem in import.ImportItems)
                {
                    string folder = SavesFolder + "\\";

                    if(importItem.ItemType == 0)//txt
                    {
                        folder += new ItemFile(importItem.ItemFileName, ItemFileType.Text).SaveFolder;
                    }else if(importItem.ItemType == 1)//tabulka
                    {
                        folder += new ItemFile(importItem.ItemFileName, ItemFileType.Table).SaveFolder;
                    }
                    else if (importItem.ItemType == 2)//obrázek
                    {
                        folder += new ItemFile(importItem.ItemFileName, ItemFileType.Images).SaveFolder;
                    }
                    else if(importItem.ItemType == 3)//url
                    {
                        folder += new ItemFile(importItem.ItemFileName, ItemFileType.URLs).SaveFolder;
                    }
                    else if (importItem.ItemType == -1)//checklist
                    {
                        folder = ChecklistFolder;
                    }else if(importItem.ItemType == -2)//log
                    {
                        folder = @".\Logs";
                    }

                    if (File.Exists(folder + "\\" + importItem.ItemFileName))
                        importItem.ErrorMsg = "Soubor již existuje!";
                }

                return await Task.FromResult(import);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }*/
        }

        public Task<ItemFile> GetTableTask(string entryName, string fileName)
        {
            return Task.FromResult(ItemManager.GetTableTasks(entryName, fileName));
        }
        public Task<List<ItemFile>> GetAllTasks()
        {
            return Task.FromResult(ItemManager.GetAllTasks());
        }
        public Task<string> GetImageNote(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.GetImageNote(itemFile));
        }

        public Task<UserSettings> GetUserSettings()
        {
            return Task.FromResult(UserSettingsManager.GetUserSettings());
        }

        public Task<ItemFile> GetExtApp(string entryName, string fileName)
        {
            return Task.FromResult(ItemManager.GetExtApp(entryName, fileName));
        }
        #endregion

        #region Saves/Writes
        public Task<Result> CreateNewEntry(string name)
        {
            return Task.FromResult(EntryManager.CreateNewEntry(name));

        }

        public Task<Result> SaveEntry(Entry entry)
        {
            return Task.FromResult(EntryManager.SaveEntry(entry));

        }

        public Task<Result> RenameEntry(Entry entry, string newName)
        {
            return Task.FromResult(EntryManager.RenameEntry(entry, newName));
        }

        public Task<Result> CreateNewItem(Entry entry, string ItemName, ItemFileType itemFileType)
        {
            return Task.FromResult(ItemManager.CreateNewItem(entry, ItemName, itemFileType));
        }

        public Task<Result> RenameItem(Entry entry, ItemFile itemToRename, string newName)
        {
            return Task.FromResult(ItemManager.RenameItem(entry, itemToRename, newName));
        }

        public Task<Result> SaveTextFile(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.SaveTextFile(itemFile));
        }

        public Task<Result> SaveTable(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.SaveTable(itemFile));
        }

        public Task<Result> ImportImage(Entry entry, string imageName)
        {
            return Task.FromResult(ItemManager.ImportImage(entry, imageName));
        }

        public Task<Result> RenameChecklist(Checklist checklist, string newName)
        {
            return Task.FromResult(ChecklistManager.RenameChecklist(checklist, newName));
        }

        public Task<Result> SaveChecklist(Checklist checklist)
        {
            return Task.FromResult(ChecklistManager.SaveChecklist(checklist));
        }

        /* prozatím zahozeno :/
        public Task<Result> ImportData(ImportExport import)
        {
            try
            {
                if(import.ImportItems.Count == 0)
                    return Task.FromResult(new Result(ResultState.Error, "Nejsou žádné soubory pro import!"));

                import.ImportItems.Sort((im1, im2) => im1.ItemParent.CompareTo(im2.ItemParent));
                Entry entry = null;
                int importedCount = 0;
                foreach (ImportItem item in import.ImportItems)
                {
                    if (item.ItemType >= 0)
                    {
                        if (entry == null)
                        {
                            if (!File.Exists(SavesFolder + "\\" + item.ItemParent + ".json"))
                                CreateNewEntry(item.ItemParent);

                            entry = GetEntry(item.ItemParent + ".json").Result;
                        }
                        else if (entry.Name != item.ItemParent)
                        {
                            SaveEntry(entry);

                            if (!File.Exists(SavesFolder + "\\" + item.ItemParent + ".json"))
                                CreateNewEntry(item.ItemParent);

                            entry = GetEntry(item.ItemParent + ".json").Result;
                        }

                        ItemFile itemFile = null;
                        if (item.ItemType == 0 && !entry.TextFiles.Contains(item.ItemName + ".txt"))
                        {
                            itemFile = new ItemFile(item.ItemName + ".txt", ItemFileType.Text);
                            entry.TextFiles.Add(itemFile.FileName);
                        }
                        else if (item.ItemType == 1 && !entry.TextFiles.Contains(item.ItemName + ".xml"))
                        {
                            itemFile = new ItemFile(item.ItemName + ".xml", ItemFileType.Table);
                            entry.TableFiles.Add(itemFile.FileName);
                        }
                        else if (item.ItemType == 2 && !entry.TextFiles.Contains(item.ItemName + ".xml"))
                        {
                            //??? jak s koncovkami obrázků?
                            //itemFile = new ItemFile(item.ItemName + ".xml", ItemFileType.Images);
                            //entry.TableFiles.Add(itemFile.FileName);
                        }
                        else if (item.ItemType == 3 && !entry.TextFiles.Contains(item.ItemName + ".xml"))
                        {
                            itemFile = new ItemFile(item.ItemName + ".xml", ItemFileType.URLs);
                            entry.URLsFiles.Add(itemFile.FileName);
                        }

                        if (itemFile == null)
                        {
                            continue;
                        }

                        File.Move(TmpFolder + $"\\{itemFile.SaveFolder}{itemFile.FileName}", SavesFolder + $"\\{itemFile.SaveFolder}{itemFile.FileName}");
                    }
                    else if(item.ItemType == -1)
                    {

                    }
                    else if(item.ItemType == -2)
                    {

                    }

                    importedCount++;
                }

                Logger.Log(Models.Enums.LogLevel.Info, $"Import dokončen. Importováno {importedCount} souborů.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return Task.FromResult(new Result(ResultState.Error, "Import souborů selhal!" + "\r\n" + ex.Message));
            }
        }*/
        public Task<Result> CopyItems(Entry entryCopyTo, List<CopyItem> itemsToCopy)
        {
            return Task.FromResult(ItemManager.CopyItems(entryCopyTo, itemsToCopy));
        }

        public Task<Result> SaveTableTask(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.SaveTableTask(itemFile));
        }
        public Task<Result> ImportPDF(Entry entry, string pdfName)
        {
            return Task.FromResult(ItemManager.ImportPDF(entry, pdfName));
        }
        public Task<Result> SaveImageNote(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.SaveImageNote(itemFile));
        }
        public Task<Result> SaveUserSettings(UserSettings userSettings)
        {
            return Task.FromResult(UserSettingsManager.Save(userSettings));
        }
        public Task<Result> AddFavorite(string entryName, ItemFile itemFile)
        {
            return Task.FromResult(UserSettingsManager.AddFavorite(entryName, itemFile));
        }
        public Task<Result> AddLastShownItem(string entryName, ItemFile itemFile)
        {
            return Task.FromResult(UserSettingsManager.AddLastShownItem(entryName, itemFile));
        }
        public Task<Result> SaveExtApp(ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.SaveExtApp(itemFile));
        }
        #endregion

        #region Deletes
        public Task<Result> DeleteEntry(string entryName)
        {
            return Task.FromResult(EntryManager.DeleteEntry(entryName));
        }

        public Task<Result> DeleteItem(Entry entry, ItemFile itemFile)
        {
            return Task.FromResult(ItemManager.DeleteItem(entry, itemFile));
        }

        public Task<Result> DeleteChecklist(Checklist checklist)
        {
            return Task.FromResult(ChecklistManager.DeleteChecklist(checklist));
        }
        #endregion
    }
}
