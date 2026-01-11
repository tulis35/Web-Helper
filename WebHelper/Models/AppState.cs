using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebHelper.Data;
using WebHelper.Models;
using WebHelper.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace WebHelper.Models
{
    public class AppState
    {
        private string _mainExeFile = @".\WebHelper.exe";
        private FileInfo _fileInfo;
        public DateTime LastModified { get { return _fileInfo.LastWriteTime; } }
        public string FullPath { get { return _fileInfo.FullName; } }        
        public bool ExeFileFound { get; private set; }
        public string ActualVersion { get; private set; }
        public string CurrentDirectory { get { return _fileInfo.DirectoryName; } }
        public DataTable DataShow { get; private set; }
        public long AllDataSize { get; private set; }
        public string AllDataSize_Display { get {return DisplaySize(AllDataSize); } }
        public long AllLogsSize { get; private set; }
        public string AllLogsSize_Display { get { return DisplaySize(AllLogsSize); } }

        public AppState()
        {
            _fileInfo = new FileInfo(_mainExeFile);
           
            ExeFileFound = File.Exists(_mainExeFile);

            if (ExeFileFound)
            {
                ActualVersion = LastModified.Year.ToString();
                ActualVersion += LastModified.Month < 10 ? "0" + LastModified.Month.ToString() : LastModified.Month.ToString();
                ActualVersion += LastModified.Day < 10 ? "0" + LastModified.Day.ToString() : LastModified.Day.ToString();
            }
            else
                ActualVersion = "00000000";

            DataShow = InitDataShow();

            AllDataSize = 0;
            AllLogsSize = 0;
        }

        private DataTable InitDataShow()
        {
            DataTable dt = new DataTable("DataView");
            dt.Columns.Add(new DataColumn("FileName", typeof(string)));
            dt.Columns.Add(new DataColumn("Folder", typeof(string)));
            dt.Columns.Add(new DataColumn("FileSize", typeof(long)));
            dt.Columns.Add(new DataColumn("FileSize_Display", typeof(string)));
            dt.Columns.Add(new DataColumn("ChildCount", typeof(int)));
            dt.Columns.Add(new DataColumn("ChildFilesSize", typeof(long)));
            dt.Columns.Add(new DataColumn("ChildFilesSize_Display", typeof(string)));
            dt.Columns.Add(new DataColumn("AllSize", typeof(long)));
            dt.Columns.Add(new DataColumn("AllSize_Display", typeof(string)));
            dt.Columns.Add(new DataColumn("BiggestFile", typeof(string)));

            return dt;
        }

        public void LoadDataSizes()
        {
            try
            {
                if (Directory.Exists(FileManager.SavesFolder))
                {
                    #region Entry
                    string[] entries = Directory.GetFiles(FileManager.SavesFolder);

                    foreach (string file in entries)
                    {
                        long entrySize = new FileInfo(file).Length;

                        string jsonString = File.ReadAllText(file);

                        var itemJson = JsonSerializer.Deserialize<Entry>(jsonString);

                        int childCount = 0;
                        long childSizes = 0;
                        long biggestSize = entrySize;
                        string biggestFile = itemJson.Name;
                        Dictionary<string, ItemFileType> items = new Dictionary<string, ItemFileType>();
                        foreach(string text in itemJson.TextFiles)
                            items.Add(text, ItemFileType.Text);
                        foreach(string table in itemJson.TableFiles)
                            items.Add(table, ItemFileType.Table);
                        foreach (string urls in itemJson.URLsFiles)
                            items.Add(urls, ItemFileType.URLs);
                        foreach (string images in itemJson.ImagesFiles)
                            items.Add(images, ItemFileType.Images);
                        foreach (string tasks in itemJson.TasksFiles)
                            items.Add(tasks, ItemFileType.Task);
                        foreach (string pdfs in itemJson.PDFsFiles)
                            items.Add(pdfs, ItemFileType.PDF);
                        foreach (string apps in itemJson.ExtAppsFiles)
                            items.Add(apps, ItemFileType.ExtApp);

                        foreach(KeyValuePair<string, ItemFileType> kvp in items)
                        {
                            ItemFile itemFile = new ItemFile(itemJson.Name, kvp.Key, kvp.Value);
                            long size = new FileInfo($@"{itemFile.Path}").Length;
                            childSizes += size;
                            childCount++;

                            if (size > biggestSize)
                            {
                                biggestSize = size;
                                biggestFile = itemFile.SaveFolder + itemFile.Name;
                            }
                        }

                        DataShow.Rows.Add(CreateNewDataRow(itemJson.Name, FileManager.SavesFolder, entrySize, childCount, childSizes, biggestFile));
                        AllDataSize += (entrySize + childSizes);
                    }
                    #endregion

                    if (Directory.Exists(FileManager.ChecklistFolder))
                    {
                        #region Checklist
                        string[] files = Directory.GetFiles(FileManager.ChecklistFolder);

                        foreach(string file in files)
                        {
                            long checklistSize = new FileInfo(file).Length;

                            string jsonString = File.ReadAllText(file);

                            var checklistJson = JsonSerializer.Deserialize<Entry>(jsonString);

                            DataShow.Rows.Add(CreateNewDataRow(checklistJson.Name, FileManager.ChecklistFolder, checklistSize, 0, 0, checklistJson.Name));
                            AllDataSize += checklistSize;
                        }
                        #endregion
                    }
                    
                }

                if (Directory.Exists(@".\Logs"))
                {
                    #region Log
                    string[] files = Directory.GetFiles(@".\Logs");

                    foreach(string file in files)
                    {
                        long logSize = new FileInfo(file).Length;

                        string name = file.Split("\\").Last().Replace(".txt", "");

                        DataShow.Rows.Add(CreateNewDataRow(name, @".\Logs", logSize, 0, 0, name));

                        AllLogsSize += logSize;
                    }
                    #endregion
                }

                DataView dv = DataShow.DefaultView;
                dv.Sort = "AllSize DESC";
                DataShow = dv.ToTable();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }


        private DataRow CreateNewDataRow(string fileName, string folder, long fileSize, int childCount, long childSize, string biggestFile)
        {
            DataRow dataRow = DataShow.NewRow();
            dataRow["FileName"] = fileName;
            dataRow["Folder"] = folder;
            dataRow["FileSize"] = fileSize;
            dataRow["ChildCount"] = childCount;
            dataRow["ChildFilesSize"] = childSize;
            dataRow["BiggestFile"] = biggestFile;

            dataRow["FileSize_Display"] = DisplaySize((long)dataRow["FileSize"]);
            dataRow["ChildFilesSize_Display"] = DisplaySize((long)dataRow["ChildFilesSize"]);
            dataRow["AllSize"] = (long)dataRow["FileSize"] + (long)dataRow["ChildFilesSize"];
            dataRow["AllSize_Display"] = DisplaySize((long)dataRow["AllSize"]);

            return dataRow;
        }

        private string DisplaySize(long size)
        {
            if (size >= 1048576)
                return size / 1048576 + "MB";
            else if (size >= 1024)
                return size / 1024 + "kB";
            else
                return size + "B";
        }

    }
}
