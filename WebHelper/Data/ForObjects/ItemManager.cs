using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using WebHelper.Models;
using WebHelper.Models.Enums;

namespace WebHelper.Data.ForObjects
{
    public class ItemManager
    {
        #region Helpers
        private static string TableToText(ItemFile itemFile)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in itemFile.TableView.Rows)
            {
                sb.AppendFormat(@"<Row>
        <Text>{0}</Text>
        <Rank>{1}</Rank>
    </Row>"
                            , ReplaceCharactersForXMLSave(row["Text"].ToString())
                            , row["Rank"]
                            );
            }

            return sb.ToString();
        }

        private static string URLsTableToText(ItemFile itemFile)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in itemFile.TableView.Rows)
            {
                sb.AppendFormat(@" <Row>
        <URL>{0}</URL>
        <Rank>{1}</Rank>
        <Description>{2}</Description>
    </Row>"
                            , ReplaceCharactersForXMLSave(row["URL"].ToString())
                            , row["Rank"]
                            , ReplaceCharactersForXMLSave(row["Description"].ToString())
                            );
            }

            return sb.ToString();
        }

        private static string TaskTableToText(ItemFile itemFile)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in itemFile.TableView.Rows)
            {
                sb.AppendFormat(@"
    <Row>
        <Text>{0}</Text>
        <CompleteTo>{1}</CompleteTo>
        <Completed>{2}</Completed>
        <CompletedTime>{3}</CompletedTime>
    </Row>
                            "
                            , ReplaceCharactersForXMLSave(row["Text"].ToString())
                            , ReplaceCharactersForXMLSave(row["CompleteTo"].ToString())
                            , row["Completed"]
                            , ReplaceCharactersForXMLSave(row["CompletedTime"].ToString())
                            );
            }

            return sb.ToString();
        }

        private static string ReplaceCharactersForXMLSave(string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;
            else
                return content.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;");
        }

        private static string ReplaceCharactersForXMLGet(string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;
            else
                return content.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&");
        }

        private static ItemFile GetTaskFromJson(ItemFile itemFile)
        {
            if (!string.IsNullOrEmpty(itemFile.RawContent))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(itemFile.RawContent);

                XmlNodeList text = doc.GetElementsByTagName("Text");
                XmlNodeList compl = doc.GetElementsByTagName("CompleteTo");
                XmlNodeList isCompl = doc.GetElementsByTagName("Completed");
                XmlNodeList complTime = doc.GetElementsByTagName("CompletedTime");

                for (int i = 0; i < text.Count; i++)
                {
                    DataRow newRow = itemFile.TableView.NewRow();

                    newRow["Text"] = ReplaceCharactersForXMLGet(text[i].InnerText);
                    newRow["CompleteTo"] = Convert.ToDateTime(ReplaceCharactersForXMLGet(compl[i].InnerText));
                    newRow["Completed"] = Convert.ToBoolean(isCompl[i].InnerText);
                    newRow["CompletedTime"] = string.IsNullOrEmpty(complTime[i].InnerText) ? DateTime.MinValue : Convert.ToDateTime(complTime[i].InnerText);

                    itemFile.TableView.Rows.Add(newRow);
                }
                DataView dv = itemFile.TableView.DefaultView;
                dv.Sort = "CompleteTo ASC";
                itemFile.TableView = dv.ToTable();
            }

            return itemFile;
        }

        public static int GetNmberOfAllUnfinishedTasks()
        {
            List<ItemFile> itemFiles = new List<ItemFile>();
            itemFiles = GetAllTasks();
            if (itemFiles == null || itemFiles.Count == 0)
                return 0;

            int number = 0;
            foreach (ItemFile itemFile in itemFiles)
            {                
                foreach (DataRow row in itemFile.TableView.Rows)
                    if (!Convert.ToBoolean(row["Completed"]))
                        number++;
            }
            return number;
        }

        public static void ExtAppStart(ItemFile itemFile)
        {
            if (itemFile.ItemFileType != ItemFileType.ExtApp)
                return;

            if (string.IsNullOrEmpty(itemFile.ExtAppSettings.AppPath))
                return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    Arguments = itemFile.ExtAppSettings.Arguments,
                    FileName = itemFile.ExtAppSettings.AppPath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    ErrorDialog = true
                });
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        #endregion

        #region Gets
        public static ItemFile GetTextFile(string entryName, string fileName)
        {
            try
            {
                ItemFile itemFile = new ItemFile(entryName, fileName, ItemFileType.Text);
                itemFile.RawContent = File.ReadAllText($@"{itemFile.SaveFolder}{itemFile.FileName}");

                return itemFile;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static ItemFile GetTableFile(string entryName, string fileName)
        {
            try
            {
                ItemFile itemFile = new ItemFile(entryName, fileName, ItemFileType.Table);
                itemFile.RawContent = File.ReadAllText($@"{itemFile.SaveFolder}{itemFile.FileName}");

                if (!string.IsNullOrEmpty(itemFile.RawContent))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(itemFile.RawContent);

                    XmlNodeList texts = doc.GetElementsByTagName("Text");
                    XmlNodeList ranks = doc.GetElementsByTagName("Rank");

                    for (int i = 0; i < texts.Count; i++)
                    {
                        DataRow newRow = itemFile.TableView.NewRow();

                        newRow["Text"] = ReplaceCharactersForXMLGet(texts[i].InnerText);
                        newRow["Rank"] = ranks[i].InnerText == "" ? 1 : Convert.ToInt32(ranks[i].InnerText);

                        itemFile.TableView.Rows.Add(newRow);
                    }
                    DataView dv = itemFile.TableView.DefaultView;
                    dv.Sort = "Rank";
                    itemFile.TableView = dv.ToTable();
                }

                return itemFile;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static ItemFile GetTableURL(string entryName, string fileName)
        {
            try
            {
                ItemFile itemFile = new ItemFile(entryName, fileName, ItemFileType.URLs);
                itemFile.RawContent = File.ReadAllText($@"{itemFile.SaveFolder}{itemFile.FileName}");

                if (!string.IsNullOrEmpty(itemFile.RawContent))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(itemFile.RawContent);

                    XmlNodeList urlss = doc.GetElementsByTagName("URL");
                    XmlNodeList ranks = doc.GetElementsByTagName("Rank");
                    XmlNodeList descs = doc.GetElementsByTagName("Description");

                    for (int i = 0; i < urlss.Count; i++)
                    {
                        DataRow newRow = itemFile.TableView.NewRow();

                        newRow["URL"] = ReplaceCharactersForXMLGet(urlss[i].InnerText);
                        newRow["Rank"] = ranks[i].InnerText == "" ? 1 : Convert.ToInt32(ranks[i].InnerText);
                        newRow["Description"] = ReplaceCharactersForXMLGet(descs[i].InnerText);

                        itemFile.TableView.Rows.Add(newRow);
                    }
                    DataView dv = itemFile.TableView.DefaultView;
                    dv.Sort = "Rank";
                    itemFile.TableView = dv.ToTable();
                }

                return itemFile;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static Image GetImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            try
            {
                return Image.FromFile(path);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static ItemFile GetTableTasks(string entryName, string fileName)
        {
            try
            {
                ItemFile itemFile = new ItemFile(entryName, fileName, ItemFileType.Task);
                itemFile.RawContent = File.ReadAllText($@"{itemFile.SaveFolder}{itemFile.FileName}");

                itemFile = GetTaskFromJson(itemFile);

                return itemFile;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static List<ItemFile> GetAllTasks(string entryName = FileManager.ReturnStringForAllTasks)
        {
            try
            {
                List<ItemFile> items = new List<ItemFile>();
                ItemFile temp = new ItemFile("temp","temp", ItemFileType.Task);
                string path = $@"{temp.SaveFolder}";
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                    items.Add(GetTableTasks(entryName,file.Replace(path, string.Empty)));

                return items;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }

        public static string GetImageNote(ItemFile itemFile)
        {
            try
            {
                string result = string.Empty;
                if(File.Exists(itemFile.SaveFolder + "Notes\\" + itemFile.Name + ".txt"))
                    result = File.ReadAllText(itemFile.SaveFolder + "Notes\\" + itemFile.Name + ".txt");

                return result;
            }
            catch (Exception ex) 
            { 
                Logger.Log(ex);
                return string.Empty;
            }
        }

        public static ItemFile GetExtApp(string entryName, string fileName)
        {
            try
            {
                ItemFile itemFile = new ItemFile(entryName, fileName, ItemFileType.ExtApp);
                itemFile.RawContent = File.ReadAllText($@"{itemFile.SaveFolder}{itemFile.FileName}");
                
                if(!string.IsNullOrEmpty(itemFile.RawContent))
                    itemFile.ExtAppSettings = JsonSerializer.Deserialize<ExtAppSettings>(itemFile.RawContent);

                return itemFile;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return null;
            }
        }
        #endregion

        #region Editors
        public static Result CreateNewItem(Entry entry, string ItemName, ItemFileType itemFileType)
        {
            if (string.IsNullOrEmpty(ItemName))
                return new Result(ResultState.Error, "Musí být vyplněn název!");

            try
            {
                GenericManager.IsValidFileName(ItemName);
                ItemFile itemFile = new ItemFile(entry.Name, ItemName, itemFileType);

                if (File.Exists($@"{itemFile.SaveFolder}{itemFile.FileName}"))
                {
                    return new Result(ResultState.Error, $"Soubor s názvem \"{itemFile.Name}\" již existuje!");
                }

                GenericManager.CreateItemSaveFolders(itemFile);

                File.Create($@"{itemFile.SaveFolder}{itemFile.FileName}").Close();

                entry.AddItem(itemFile);

                EntryManager.SaveEntry(entry);

                return new Result(ResultState.Success, $"Záznam \"{ItemName}\" vytvořen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{ItemName}\" nešel vytvořit." + "\r\n" + ex.Message);
            }
        }

        public static Result RenameItem(Entry entry, ItemFile itemToRename, string newName)
        {
            try
            {

                if (string.IsNullOrEmpty(newName))
                {
                    return new Result(ResultState.Error, $"Nový název musí být vyplněn.");
                }

                if (File.Exists($@"{itemToRename.SaveFolder}/{newName}.{itemToRename.FileExt}"))
                {
                    return new Result(ResultState.Error, $"Soubor s názvem \"{newName}\" již existuje!");
                }
                GenericManager.IsValidFileName(newName);

                int index = -1;
                string newNameFull = newName + "." + itemToRename.FileExt;
                entry.ItemRenamed(itemToRename.FileName, newNameFull, itemToRename.ItemFileType);                

                File.Move($@"{itemToRename.Path}", $@"{itemToRename.SaveFolder}/{newNameFull}");

                EntryManager.SaveEntry(entry);

                return new Result(ResultState.Success, $"Záznam přejmenován.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemToRename.Name}\" nešel přejmenovat." + "\r\n" + ex.Message);
            }
        }
        #endregion

        #region Saves
        public static Result SaveTextFile(ItemFile itemFile)
        {
            try
            {
                File.WriteAllText($@"{itemFile.Path}", itemFile.RawContent.Trim());

                return new Result(ResultState.Success, "Záznam uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemFile.Name}\" nešel uložit." + "\r\n" + ex.Message);
            }
        }

        public static Result SaveTable(ItemFile itemFile)
        {
            try
            {
                itemFile.RawContent = "<Data>\r\n" + (itemFile.ItemFileType == ItemFileType.Table ? TableToText(itemFile) : URLsTableToText(itemFile)) + "\r\n</Data>";
                File.WriteAllText($@"{itemFile.Path}", itemFile.RawContent.Trim());

                return new Result(ResultState.Success, "Záznam uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemFile.Name}\" nešel uložit." + "\r\n" + ex.Message);
            }
        }

        public static Result SaveTableTask(ItemFile itemFile)
        {
            try
            {
                itemFile.RawContent = "<Data>\r\n" + TaskTableToText(itemFile) + "\r\n</Data>";
                File.WriteAllText($@"{itemFile.Path}", itemFile.RawContent.Trim());

                return new Result(ResultState.Success, "Záznam uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemFile.Name}\" nešel uložit." + "\r\n" + ex.Message);
            }
        }

        public static Result ImportImage(Entry entry, string imageName)
        {
            try
            {
                ItemFile image = new ItemFile(entry.Name, imageName, ItemFileType.Images);
                if (!AllowedExtensions.AllowedImagesExt.Contains(image.FileExt))
                {
                    return new Result(ResultState.Error, $"Typ obrázku není povolen.");
                }
                GenericManager.CreateItemSaveFolders(image);

                File.Move($@"{GenericManager.TmpFolder}/{imageName}", $@"{image.Path}");

                entry.ImagesFiles.Add(image.FileName);

                EntryManager.SaveEntry(entry);

                return new Result(ResultState.Success, "Obrázek uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Obrázek se nepodařilo importovat." + "\r\n" + ex.Message);
            }
        }

        public static Result CopyItems(Entry entryCopyTo, List<CopyItem> itemsToCopy)
        {
            try
            {
                foreach (CopyItem itemToCopy in itemsToCopy)
                {
                    if (File.Exists($@"{itemToCopy.ItemToCopy.SaveFolder}{itemToCopy.NewFileName}"))
                        throw new Exception($"Soubor {itemToCopy.NewFileName} již existuje!");

                    GenericManager.IsValidFileName(itemToCopy.NewFileName);
                }

                foreach (CopyItem itemToCopy in itemsToCopy)
                {
                    File.Copy($@"{itemToCopy.ItemToCopy.Path}", $@"{itemToCopy.ItemToCopy.SaveFolder}{itemToCopy.NewFileName}");
                    if(itemToCopy.ItemToCopy.ItemFileType == ItemFileType.Images && File.Exists($@"{itemToCopy.ItemToCopy.SaveFolder}\\Notes\\{itemToCopy.ItemToCopy.Name}.txt"))
                        File.Copy($@"{itemToCopy.ItemToCopy.SaveFolder}\\Notes\\{itemToCopy.ItemToCopy.Name}.txt", $@"{itemToCopy.ItemToCopy.SaveFolder}\\Notes\\{itemToCopy.NewName}.txt");

                    entryCopyTo.AddItem(itemToCopy.ItemToCopy);
                }

                EntryManager.SaveEntry(entryCopyTo);

                return new Result(ResultState.Success, "Všechyn soubory byly úspěšně přeneseny.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, "Kopírování souborů selhalo!" + "\r\n" + ex.Message);
            }
        }

        public static Result ImportPDF(Entry entry, string pdfName)
        {
            try
            {
                ItemFile pdf = new ItemFile(entry.Name, pdfName, ItemFileType.PDF);
                if (pdf.FileExt != "pdf")
                {
                    return new Result(ResultState.Error, $"Typ souboru není povolen.");
                }
                GenericManager.CreateItemSaveFolders(pdf);

                File.Move($@"{GenericManager.TmpFolder}/{pdfName}", $@"{pdf.Path}");

                entry.PDFsFiles.Add(pdf.FileName);

                EntryManager.SaveEntry(entry);

                return new Result(ResultState.Success, "PDF uloženo.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"PDF se nepodařilo importovat." + "\r\n" + ex.Message);
            }
        }

        public static Result SaveImageNote(ItemFile itemFile)
        {
            try
            {
                if (!Directory.Exists(itemFile.SaveFolder + "Notes"))
                    Directory.CreateDirectory(itemFile.SaveFolder + "Notes");
                string path = itemFile.SaveFolder + "Notes\\" + itemFile.Name + ".txt";
                if (string.IsNullOrEmpty(itemFile.RawContent))
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                else
                    File.WriteAllText(itemFile.SaveFolder + "\\Notes\\" + itemFile.Name + ".txt", itemFile.RawContent);

                return new Result(ResultState.Success, "Poznámka uložena.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, "Poznámka se neuložila. \r\n " + ex.Message);
            }
        }

        public static Result SaveExtApp(ItemFile itemFile)
        {
            try
            {
                string json = JsonSerializer.Serialize(itemFile.ExtAppSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(itemFile.Path, json);

                return new Result(ResultState.Success, $"Záznam \"{itemFile.Name}\" uložen.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemFile.Name}\" nešel uložit." + "\r\n" + ex.Message);
            }
        }
        #endregion

        #region Deletes
        public static Result DeleteItem(Entry entry, ItemFile itemFile)
        {
            try
            {
                File.Delete($@"{itemFile.Path}");

                entry.RemoveItem(itemFile);

                EntryManager.SaveEntry(entry);

                return new Result(ResultState.Success, $"Záznam \"{itemFile.Name}\" smazán.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new Result(ResultState.Error, $"Záznam \"{itemFile.Name}\" nesmazán. \r\n " + ex.Message);
            }
        }
        #endregion        
    }
}
