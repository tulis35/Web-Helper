using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHelper.Models.Enums
{
    public static class AllowedExtensions
    {
        public static string[] AllowedTxtExt = { "txt" };
        public static string[] AllowedTablesExt = { "xml" };
        public static string[] AllowedImagesExt = { "png", "jpeg", "jpg", "jfif" };
        public static string[] AllowedURLsExt = { "xml" };
        public static string[] AllowedCheckListsExt = { "json" };
    }
}
