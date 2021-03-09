using Caliburn.Micro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using PlcComUI.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PlcComUI.ViewModels
{
    /* Based on  https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MainDemo.Wpf/PaletteHelperExtensions.cs */
    public class PaletteSelectorViewModel : Screen
    {
        public PaletteSelectorViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            LightDarkThemeToggle(Properties.Settings.Default.SettingsMain.MainWindow.IsDarkMode);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            LightDarkThemeToggle(true);
        }

        public IEnumerable<Swatch> Swatches { get; }

        public void LightDarkThemeToggle(bool IsChecked)
        {
            ModifyTheme(theme => theme.SetBaseTheme(IsChecked ? Theme.Dark : Theme.Light));
        }

        public ICommand ApplyPrimaryCommand { get; } = new RelayCommand(o => ApplyPrimary((Swatch)o));

        private static void ApplyPrimary(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
        }

        public ICommand ApplyAccentCommand { get; } = new RelayCommand(o => ApplyAccent((Swatch)o));

        private static void ApplyAccent(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
