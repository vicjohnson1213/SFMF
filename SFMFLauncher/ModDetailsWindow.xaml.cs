using SFMFManager.Util;
using System.Windows;

namespace SFMFLauncher
{
    /// <summary>
    /// Interaction logic for ModDetailsWindow.xaml
    /// </summary>
    public partial class ModDetailsWindow : Window
    {
        private Mod Mod { get; set; }
        public ModDetailsWindow(Mod mod)
        {
            DataContext = mod;
            InitializeComponent();

            if (!mod.DisableScoreReporting)
                lblWarning.Visibility = Visibility.Collapsed;
        }
    }
}
