using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebHelper.Models.Enums;
using WebHelper.Pages;

namespace WebHelper.Models
{
    public class Entry
    {
        public string Name { get; set; }

        public List<string> TextFiles { get; set; }
        public List<string> TableFiles { get; set; }
        public List<string> URLsFiles { get; set; }
        public List<string> ImagesFiles { get; set; }
        public List<string> TasksFiles { get; set; }
        public List<string> PDFsFiles { get; set; }
        public List<string> ExtAppsFiles { get; set; }
        [JsonIgnore]
        public List<KeyValuePair<string, ItemFileType>> AllFiles { get { return GetAllFiles(); } }

        public Entry(string name)
        {
            Name = name;
            TextFiles = new List<string>();
            TableFiles = new List<string>();
            URLsFiles = new List<string>();
            ImagesFiles = new List<string>();
            TasksFiles = new List<string>();
            PDFsFiles = new List<string>();
            ExtAppsFiles = new List<string>();
        }

        private List<KeyValuePair<string, ItemFileType>> GetAllFiles()
        {
            List<KeyValuePair<string, ItemFileType>> list = new List<KeyValuePair<string, ItemFileType>>();

            foreach (string file in TextFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.Text));

            foreach (string file in TableFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.Table));

            foreach (string file in URLsFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.URLs));

            foreach (string file in ImagesFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.Images));

            foreach (string file in TasksFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.Task));

            foreach (string file in PDFsFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.PDF));

            foreach (string file in ExtAppsFiles)
                list.Add(new KeyValuePair<string, ItemFileType>(file, ItemFileType.ExtApp));

            list.Sort((l1, l2) => l1.Key.CompareTo(l2.Key));

            return list;
        }

        public void AddItem(string fileName, ItemFileType itemFileType)
        {
            AddItem(new ItemFile(Name, fileName, itemFileType));
        }

        public void AddItem(ItemFile item)
        {
            switch (item.ItemFileType)
            {
                case ItemFileType.Text:
                    TextFiles.Add(item.Name);
                    break;
                case ItemFileType.Table:
                    TableFiles.Add(item.Name);
                    break;
                case ItemFileType.URLs:
                    URLsFiles.Add(item.Name);
                    break;
                case ItemFileType.Task:
                    TasksFiles.Add(item.Name);
                    break;
                case ItemFileType.ExtApp:
                    ExtAppsFiles.Add(item.Name);
                    break;
            }
        }

        public void RemoveItem(ItemFile item) 
        {
            switch (item.ItemFileType)
            {
                case ItemFileType.Text:
                    TextFiles.Remove(item.FileName);
                    break;
                case ItemFileType.Table:
                    TableFiles.Remove(item.FileName);
                    break;
                case ItemFileType.URLs:
                    URLsFiles.Remove(item.FileName);
                    break;
                case ItemFileType.Images:
                    ImagesFiles.Remove(item.FileName);
                    break;
                case ItemFileType.Task:
                    TasksFiles.Remove(item.FileName);
                    break;
                case ItemFileType.PDF:
                    PDFsFiles.Remove(item.FileName);
                    break;
                case ItemFileType.ExtApp:
                    ExtAppsFiles.Remove(item.FileName);
                    break;
            }
        }

        public void ItemRenamed(string oldName, string newName, ItemFileType itemFileType)
        {
            int index = -1;
            switch (itemFileType)
            {
                case ItemFileType.Text:
                    index = TextFiles.IndexOf(oldName);
                    TextFiles[index] = newName;
                    break;
                case ItemFileType.Table:
                    index = TableFiles.IndexOf(oldName);
                    TableFiles[index] = newName;
                    break;
                case ItemFileType.Images:
                    index = ImagesFiles.IndexOf(oldName);
                    ImagesFiles[index] = newName;
                    break;
                case ItemFileType.URLs:
                    index = URLsFiles.IndexOf(oldName);
                    URLsFiles[index] = newName;
                    break;
                case ItemFileType.Task:
                    index = TasksFiles.IndexOf(oldName);
                    TasksFiles[index] = newName;
                    break;
                case ItemFileType.PDF:
                    index = PDFsFiles.IndexOf(oldName);
                    PDFsFiles[index] = newName;
                    break;
                case ItemFileType.ExtApp:
                    index = ExtAppsFiles.IndexOf(oldName);
                    ExtAppsFiles[index] = newName;
                    break;
            }
        }
    }
}
