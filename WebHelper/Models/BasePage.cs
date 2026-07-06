using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace WebHelper.Models
{
    public class BasePage : ComponentBase
    {
        public Result result = null;
        public bool ShowHideLoading = true;
        public bool ShowRenameDialog = false;
        public bool DialogDeleteIsOpen = false;
        public bool AreChanges = false;
    }
}
