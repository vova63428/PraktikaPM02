using System.Windows;
using LabDesktop.Models;

namespace LabDesktop
{
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
    }
}