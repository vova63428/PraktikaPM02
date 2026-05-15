using System.Windows;

namespace AppDesktop
{
    public partial class App : Application
    {
        public static Models.User CurrentUser { get; set; }
    }
}