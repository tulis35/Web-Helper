using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebHelper.Data.ForObjects;
using WebHelper.Models.Enums;
using WebHelper.Models;
using System.Drawing;

namespace WebHelper.Data
{
    public interface IFileManager
    {
        #region Params
        public static string SavesFolder { get; }
        public static string TmpFolder { get; }
        public static long MaxImportFileSize { get; }
        public static string ChecklistFolder { get; }
        #endregion

        #region Stuff
        public Task CreateFolders();

        public Task ClearTmp();

        public Task ClearTmpFile(string fileName);
        public Task ClearRmpFolder(string folderName);

        public void CreateChecklistFolder();

        public Task<Result> CreateExport(List<ExportItem> exports, string ExportName);

        public string GetTmpFolder();
        public string GetSavesFolder();
        public long GetMaxImportFileSize();
        public string GetChecklistFolder();
        public string GetReturnStringForAllTasks();
        public Task<int> GetNmberOfAllUnfinishedTasks();
        public void ExtAppStart(ItemFile itemFile);
        #endregion

        #region Gets
        public Task<List<Entry>> GetEntries();

        public Task<Entry> GetEntry(string name);

        public Task<ItemFile> GetTextFile(string entryName, string fileName);

        public Task<ItemFile> GetTableFile(string entryName, string fileName);

        public Task<ItemFile> GetTableURL(string entryName, string fileName);

        public Task<Image> GetImage(string path);

        public Task<List<Log>> GetLogs();

        public Task<Log> GetLog(string file);

        public Task<List<Checklist>> GetChecklists(bool GetSaved);

        public Task<Checklist> GetChecklist(string fileName, bool GetSaved);

        public Task<List<ExportItem>> GetFilesForExport(bool loadItems, bool loadChecklists, bool loadLogs);

        public Task<ImportExport> LoadImport(string importFile);

        public Task<ItemFile> GetTableTask(string entryName, string fileName);
        public Task<List<ItemFile>> GetAllTasks();
        Task<string> GetImageNote(ItemFile itemFile);
        public Task<UserSettings> GetUserSettings();        
        public Task<ItemFile> GetExtApp(string entryName, string fileName);
        #endregion

        #region Saves/Writes
        public Task<Result> CreateNewEntry(string name);

        public Task<Result> SaveEntry(Entry entry);

        public Task<Result> RenameEntry(Entry entry, string newName);

        public Task<Result> CreateNewItem(Entry entry, string ItemName, ItemFileType itemFileType);

        public Task<Result> RenameItem(Entry entry, ItemFile itemToRename, string newName);

        public Task<Result> SaveTextFile(ItemFile itemFile);

        public Task<Result> SaveTable(ItemFile itemFile);

        public Task<Result> ImportImage(Entry entry, string imageName);

        public Task<Result> RenameChecklist(Checklist checklist, string newName);

        public Task<Result> SaveChecklist(Checklist checklist);
        public Task<Result> CopyItems(Entry entryCopyTo, List<CopyItem> itemsToCopy);
        public Task<Result> SaveTableTask(ItemFile itemFile);
        Task<Result> ImportPDF(Entry entry, string pdfName);
        Task<Result> SaveImageNote(ItemFile itemFile);
        public Task<Result> SaveUserSettings(UserSettings userSettings);
        public Task<Result> AddFavorite(string entryName, ItemFile itemFile);
        public Task<Result> AddLastShownItem(string entryName, ItemFile itemFile);
        public Task<Result> SaveExtApp(ItemFile itemFile);
        #endregion

        #region Deletes
        public Task<Result> DeleteEntry(string entryName);

        public Task<Result> DeleteItem(Entry entry, ItemFile itemFile);

        public Task<Result> DeleteChecklist(Checklist checklist);
        #endregion
    }
}
