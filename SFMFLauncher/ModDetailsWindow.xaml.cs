using SFMFManager;
using SFMFManager.Dto;
using SFMFManager.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace SFMFLauncher
{
    /// <summary>
    /// Interaction logic for ModDetailsWindow.xaml
    /// </summary>
    public partial class ModDetailsWindow : Window
    {
        public IEnumerable<string> KeyboardKeys => Enum.GetNames(typeof(KeyboardKeys));
        public IEnumerable<string> ControllerButtons => Enum.GetNames(typeof(ControllerButtons));

        private Manager Manager { get; set; }
        private Mod Mod { get; set; }

        public bool EditingEnabled { get; set; }

        public ModDetailsWindow(Manager manager, Mod mod, bool editingEnabled)
        {
            Manager = manager;
            Mod = mod;

            DataContext = mod;

            EditingEnabled = editingEnabled;

            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            var hasSettings = Mod.Settings?.Settings != null && Mod.Settings.Settings.Count > 0;
            var hasControls = Mod.Settings?.Controls != null && Mod.Settings.Controls.Count > 0;

            if (!hasSettings)
            {
                lblSettings.Visibility = Visibility.Collapsed;
                itmesSettings.Visibility = Visibility.Collapsed;
            }

            if (!hasControls)
            {
                lblControls.Visibility = Visibility.Collapsed;
                itemsControls.Visibility = Visibility.Collapsed;
            }

            if (!hasSettings && !hasControls)
                btnSave.Visibility = Visibility.Collapsed;

            if (!Mod.DisableScoreReporting)
                lblWarning.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(Mod.Homepage))
                BtnHomepage.Visibility = Visibility.Hidden;
        }

        private void BtnWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Mod.Homepage);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Manager.SaveModSettings(Mod);
            Close();
        }
    }
}
