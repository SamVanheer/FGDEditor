using FGDEditor.Modules.GameDataEditor;
using FGDEditor.Services;
using FGDEditor.Services.Interfaces;
using FGDEditor.Services.Wpf;
using FGDEditor.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace FGDEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageDialogService, MessageDialogService>();
            containerRegistry.RegisterSingleton<IGameDataEditor, GameDataEditor>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<GameDataEditorModule>();
        }
    }
}
