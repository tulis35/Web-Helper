using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebHelper.Models;
using WebHelper.Data;
using System;
using System.Diagnostics;

namespace WebHelper.Data.ForObjects
{
    public class GenericManager
    {
        #region Params
        public const string SavesFolder = @".\Saves";
        public const string TmpFolder = @".\tmp";
        public static long MaxImportFileSize = UserSettingsManager.GetUserSettings().MaxFileSizeInMB*1000000; 
        public const string ChecklistFolder = SavesFolder + @"\Checklists";
        public const string SavedChecklistFolder = ChecklistFolder + @"\Saved";
        public const string ReturnStringForAllTasks = "ReturnStringForAllTasks";
        #endregion

        public static void CreateFolders()
        {
            if (!Directory.Exists(SavesFolder))
                Directory.CreateDirectory(SavesFolder);

            if (!Directory.Exists(TmpFolder))
                Directory.CreateDirectory(TmpFolder);
            else
                ClearTmp();
        }

        public static void CreateItemSaveFolders(ItemFile itemFile)
        {
            int index = itemFile.SaveFolder.LastIndexOf("\\");
            string folder = itemFile.SaveFolder.Substring(0, index);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }        

        public static void ClearTmp()
        {
            DirectoryInfo dir = new DirectoryInfo(TmpFolder);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo directoryInfo in dir.GetDirectories())
            {
                foreach (FileInfo file in directoryInfo.GetFiles()) { file.Delete(); }
                foreach (DirectoryInfo info in directoryInfo.GetDirectories())
                {
                    foreach (FileInfo file in info.GetFiles()) { file.Delete(); }
                    info.Delete();
                }
                directoryInfo.Delete();
            }
        }

        public static void ClearTmpFile(string fileName)
        {
            File.Delete($@"{TmpFolder}/{fileName}");
        }
        public static void ClearRmpFolder(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(TmpFolder + "\\" + folderName);
            foreach (DirectoryInfo directoryInfo in dir.GetDirectories())
            {
                foreach (FileInfo file in directoryInfo.GetFiles()) { file.Delete(); }
                foreach (DirectoryInfo info in directoryInfo.GetDirectories())
                {
                    foreach (FileInfo file in info.GetFiles()) { file.Delete(); }
                    info.Delete();
                }
                directoryInfo.Delete();
            }
            foreach (FileInfo file in dir.GetFiles()) { file.Delete(); }
            dir.Delete();
        }


        public static void CreateChecklistFolder()
        {
            if (!Directory.Exists(ChecklistFolder))
                Directory.CreateDirectory(ChecklistFolder);
            if (!Directory.Exists(SavedChecklistFolder))
                Directory.CreateDirectory(SavedChecklistFolder);
        }

        public static void IsValidFileName(string fileName)
        {
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                throw new Exception("Nevaldní název souboru!");
            if (fileName.Length > 255)
                throw new Exception("Moc dlouhý název souboru!");
        }
    }
}
