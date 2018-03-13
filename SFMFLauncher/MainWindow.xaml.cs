using SFMFManager;
using SFMFManager.Util;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SFMFLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<Mod> OnlineMods;
        ObservableCollection<Mod> SavedMods;
        ObservableCollection<Mod> InstalledMods;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            OnlineMods = new ObservableCollection<Mod>();
            SavedMods = new ObservableCollection<Mod>();
            InstalledMods = new ObservableCollection<Mod>();

            ICOnlineMods.ItemsSource = OnlineMods;
            ICSavedMods.ItemsSource = SavedMods;
            ICInstalledMods.ItemsSource = InstalledMods;

            InitializeUI();
       }

        private void InitializeUI()
        {
            UpdateStates();

            UpdateSavedMods();
            UpdateInstalledMods();
            UpdateOnlineMods();
        }

        private void UpdateStates()
        {
            BtnToggleFramework.Content = Manager.IsSFMFInstalled() ? "Uninstall SFMF" : "Install SFMF";
            ChBxDisableScoreReporting.IsEnabled = !Manager.IsSFMFInstalled();
        }

        private void BtnToggleFramework_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.IsSFMFInstalled())
            {
                Manager.UninstallSFMF();
                BtnToggleFramework.Content = "Install SFMF";
            }
            else
            {
                Manager.InstallSFMF(ChBxDisableScoreReporting.IsChecked ?? false);
                BtnToggleFramework.Content = "Uninstall SFMF";
            }

            UpdateStates();
        }

        private void BtnDownloadMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ((Button)sender).Tag as Mod;

            var mods = Manager.SaveMod(mod);

            SavedMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SavedMods"));
            UpdateOnlineMods();
        }

        private void BtnRemoveMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ((Button)sender).Tag as Mod;

            Manager.RemoveMod(mod);

            SavedMods.Remove(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SavedMods"));
            UpdateOnlineMods();
        }

        private void BtnInstallMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ((Button)sender).Tag as Mod;

            Manager.InstallMod(mod);

            SavedMods.Remove(mod);
            InstalledMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SavedMods"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstalledMods"));
        }

        private void BtnUninstallMod_Click(object sender, RoutedEventArgs e)
        {
            var mod = ((Button)sender).Tag as Mod;

            Manager.UninstallMod(mod);

            InstalledMods.Remove(mod);
            SavedMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SavedMods"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstalledMods"));
        }

        private void UpdateOnlineMods()
        {

            var mods = Manager.GetOnlineMods();

            OnlineMods.Clear();
            foreach (Mod mod in mods)
                if (!SavedMods.Any(m => m.Download == mod.Download) && !InstalledMods.Any(m => m.Download == mod.Download))
                    OnlineMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnlineMods"));
        }

        private void UpdateSavedMods()
        {
            var mods = Manager.GetSavedMods();

            SavedMods.Clear();
            foreach (Mod mod in mods)
                SavedMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SavedMods"));
        }

        private void UpdateInstalledMods()
        {
            var mods = Manager.GetInstalledMods();

            InstalledMods.Clear();
            foreach (Mod mod in mods)
                InstalledMods.Add(mod);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstalledMods"));
        }
    }
}
