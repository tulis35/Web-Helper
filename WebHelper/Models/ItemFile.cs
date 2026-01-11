using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using WebHelper.Models.Enums;

namespace WebHelper.Models
{
    public class ItemFile
    {
        public string Path { get; private set; }
        public string Name { get { return GetName(); } }
        public ItemFileType ItemFileType { get; private set; }
        public string SaveFolder { get { return GetSaveFolder(); } }
        public string FileName { get; private set; }
        public string RawContent { get; set; }
        public DataTable TableView { get; set; }
        public string FileExt { get { return GetExt(); } }
        public string ParentEntryName { get; set; }
        public ExtAppSettings ExtAppSettings { get; set; }

        public static string DetailPath(string entryName, string fileName, ItemFileType itemFileType)
        {
            string path = string.Empty;

            if (itemFileType == ItemFileType.Text)
                path = "/textfiledetail/";
            else if (itemFileType == ItemFileType.Table)
                path = "/tablefiledetail/";
            else if (itemFileType == ItemFileType.URLs)
                path = "/tableurldetail/";
            else if (itemFileType == ItemFileType.Images)
                path = "/imagedetail/";
            else if (itemFileType == ItemFileType.Task)
                path = "/taskdetail/";
            else if (itemFileType == ItemFileType.ExtApp)
                path = "/extappdetail/";
            else
                throw new Exception("Neznámý typ!");

            path += entryName + "/" + fileName;

            return path;
        }

        public ItemFile(string entryName, string file, ItemFileType itemFileType)
        {            
            ItemFileType = itemFileType;
            Path = GetSaveFolder() + file;
            FileName = file;
            switch (itemFileType)
            {
                case ItemFileType.Table:
                    TableView = CreateTable();
                    break;
                case ItemFileType.URLs:
                    TableView = CreateURLTable();
                    break;
                case ItemFileType.Task:
                    TableView = CreateTasksTable();
                    break;
                default:
                    TableView = new DataTable();
                    break;
            }
            ParentEntryName = entryName;
            ExtAppSettings = new ExtAppSettings();
        }

        private string GetName()
        {
            if (string.IsNullOrEmpty(Path))
                return "";

            string split = Path.Split("\\").Last();

            return split.Split(".").First();
        }

        private string GetSaveFolder()
        {
            string folder = WebHelper.Data.ForObjects.GenericManager.SavesFolder + "\\";

            switch (this.ItemFileType)
            {
                case ItemFileType.Text:
                    folder += "Texts\\";
                    break;
                case ItemFileType.Table:
                    folder += "Tables\\";
                    break;
                case ItemFileType.Images:
                    folder += "Images\\";
                    break;
                case ItemFileType.URLs:
                    folder += "URLs\\";
                    break;
                case ItemFileType.Task:
                    folder += "Tasks\\";
                    break;
                case ItemFileType.PDF:
                    folder = ".\\wwwroot\\PDFs\\";
                    break;
                case ItemFileType.ExtApp:
                    folder += "ExtApps\\";
                    break;
            }

            return folder;
        }

        public static DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Text", typeof(string)));
            table.Columns.Add(new DataColumn("Rank", typeof(Int32)));

            return table;
        }

        public static DataTable CreateURLTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("URL", typeof(string)));
            table.Columns.Add(new DataColumn("Rank", typeof(Int32)));
            table.Columns.Add(new DataColumn("Description", typeof(string)));

            return table;
        }

        public static DataTable CreateTasksTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Text", typeof(string)));
            table.Columns.Add(new DataColumn("CompleteTo", typeof(DateTime)));
            table.Columns.Add(new DataColumn("Completed", typeof(bool)));
            table.Columns.Add(new DataColumn("CompletedTime", typeof(DateTime)));

            return table;
        }

        private string GetExt()
        {
            return FileName.Split('.').Last();
        }

        public string GetDetailPath()
        {
            return DetailPath(this.ParentEntryName, this.FileName, this.ItemFileType);
        }        
    }
}
