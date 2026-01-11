namespace WebHelper.Models
{
    public class CopyItem
    {
        public ItemFile ItemToCopy { get; private set; }
        public Entry CopyToEntry { get; private set; }
        public string NewName { get; set; }
        public string NewFileName { get { return GetNewFileName(); } }

        public CopyItem(ItemFile itemToCopy, Entry copyToEntry, string newName)
        {
            ItemToCopy = itemToCopy;
            CopyToEntry = copyToEntry;
            NewName = newName;
        }

        public CopyItem(ItemFile itemToCopy, Entry copyToEntry)
        {
            ItemToCopy = itemToCopy;
            CopyToEntry = copyToEntry;
            NewName = CopyToEntry.Name + " " + itemToCopy.Name;
        }

        private string GetNewFileName()
        {
            return NewName + "." + ItemToCopy.FileExt;
        }
    }
}
