namespace WebHelper.Models
{
    public class ChecklistItem
    {
        public string Text { get; set; }
        public int Rank { get; set; }
        public bool IsChecked { get; set; }

        public ChecklistItem()
        {
            Text = string.Empty;
            Rank = -1;
            IsChecked = false;
        }

        public ChecklistItem(string text, int rank, bool isChecked)
        {
            Text = text;
            Rank = rank;
            IsChecked = isChecked;
        }
    }
}
