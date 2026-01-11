using System.Collections.Generic;

namespace WebHelper.Models
{
    public class ImportExport
    {
        public string Version { get; private set; }
        public List<ImportItem> ImportItems { get; set; }

        public ImportExport(string version) 
        {
            Version = version;
            ImportItems = new List<ImportItem>();
        }
        /*public ImportExport(string version, List<ImportItem> imports)
        {
            Version = version;
            ImportItems = imports;
        }*/
    }
}
