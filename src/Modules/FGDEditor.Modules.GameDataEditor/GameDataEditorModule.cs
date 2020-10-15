using FGDEditor.Core;
using FGDEditor.Modules.GameDataEditor.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FGDEditor.Modules.GameDataEditor
{
    public class GameDataEditorModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public GameDataEditorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.EditorRegion, typeof(Editor));
            _regionManager.RegisterViewWithRegion(RegionNames.EntityClassEditorRegion, typeof(EntityClassEditor));
        }
    }
}