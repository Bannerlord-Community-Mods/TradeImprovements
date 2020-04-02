using System;
using System.Linq;
using SandBox.GauntletUI;
using SandBox.View;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.TwoDimension;

namespace TradeImprovements
{
[GameStateScreen(typeof (InventoryState))]
  public class InventoryGauntletScreenOverride : ScreenBase, IInventoryStateHandler, IGameStateListener, IChangeableScreen
  {
    private readonly InventoryState _inventoryState;
    private GauntletMovie _gauntletMovie;
    private SPInventoryVM _dataSource;
    private GauntletLayer _gauntletLayer;
    private bool _closed;
    private bool _openedFromMission;

    protected override void OnFrameTick(float dt)
    {
      base.OnFrameTick(dt);
      if (!this._closed)
        LoadingWindow.DisableGlobalLoadingWindow();
      this._dataSource.IsFiveStackModifierActive = this._gauntletLayer.Input.IsHotKeyDown("FiveStackModifier");
      
      MainApp.Instance.OnInventoryScreenFrame(this._dataSource);

      if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchAlternative") && this._dataSource != null)
      {
        this._dataSource.CompareNextItem();
      }
      else
      {
        if (!this._gauntletLayer.Input.IsHotKeyReleased("Exit") && !this._gauntletLayer.Input.IsGameKeyReleased(31))
          return;
        this.ExecuteCancel();
      }
    }

    public InventoryGauntletScreenOverride(InventoryState inventoryState)
    {
      this._inventoryState = inventoryState;
      this._inventoryState.Handler = (IInventoryStateHandler) this;
      this._inventoryState.Listener = (IGameStateListener) this;
    }
    
    protected override void OnInitialize()
    {
      UIResourceManager.SpriteData.SpriteCategories["ui_inventory"].Load((ITwoDimensionResourceContext) UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
      InventoryLogic inventoryLogic = this._inventoryState.InventoryLogic;
      Mission current = Mission.Current;
      int num = current != null ? (current.DoesMissionRequireCivilianEquipment ? 1 : 0) : 0;
      Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags = new Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>(this.GetItemUsageSetFlag);
      string stackShortcutkeyText = this.GetFiveStackShortcutkeyText();
      this._dataSource = new SPInventoryVM(inventoryLogic, num != 0, getItemUsageSetFlags, stackShortcutkeyText);
      GauntletLayer gauntletLayer = new GauntletLayer(15, "GauntletLayer");
      gauntletLayer.IsFocusLayer = true;
      this._gauntletLayer = gauntletLayer;
      this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
      this.AddLayer((ScreenLayer) this._gauntletLayer);
      ScreenManager.TrySetFocus((ScreenLayer) this._gauntletLayer);
      this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("InventoryHotKeyCategory"));
      this._gauntletMovie = this._gauntletLayer.LoadMovie("Inventory", (ViewModel) this._dataSource);
      this._openedFromMission = this._inventoryState.Predecessor is MissionState;
      InformationManager.ClearAllMessages();
    }

    private string GetFiveStackShortcutkeyText()
    {
      return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", HotKeyManager.GetCategory("InventoryHotKeyCategory").RegisteredHotKeys.Single<HotKey>((Func<HotKey, bool>) (k => k.Id == "FiveStackModifier")).ToString().ToLower()).ToString();
    }

    protected override void OnDeactivate()
    {
      base.OnDeactivate();
      this._closed = true;
      LoadingWindow.EnableGlobalLoadingWindow(false);
      InformationManager.HideInformations();
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      this._dataSource?.RefreshCallbacks();
    }

    protected override void OnFinalize()
    {
      base.OnFinalize();
      this._gauntletMovie = (GauntletMovie) null;
      this._dataSource.OnFinalize();
      this._dataSource = (SPInventoryVM) null;
      this._gauntletLayer = (GauntletLayer) null;
    }

    void IGameStateListener.OnActivate()
    {
      Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.InventoryScreen));
    }

    void IGameStateListener.OnDeactivate()
    {
      Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(TutorialContexts.None));
    }

    void IGameStateListener.OnInitialize()
    {
    }

    void IGameStateListener.OnFinalize()
    {
    }

    void IInventoryStateHandler.FilterInventoryAtOpening(
      InventoryManager.InventoryCategoryType inventoryCategoryType)
    {
      switch (inventoryCategoryType)
      {
        case InventoryManager.InventoryCategoryType.Armors:
          this._dataSource.ExecuteFilterArmors();
          break;
        case InventoryManager.InventoryCategoryType.Weapon:
          this._dataSource.ExecuteFilterWeapons();
          break;
        case InventoryManager.InventoryCategoryType.HorseCategory:
          this._dataSource.ExecuteFilterMounts();
          break;
        case InventoryManager.InventoryCategoryType.Goods:
          this._dataSource.ExecuteFilterMisc();
          break;
      }
    }

    public void ExecuteLootingScript()
    {
      this._dataSource.ExecuteBuyAllItems();
    }

    public void ExecuteSellAllLoot()
    {
      this._dataSource.ExecuteSellAllItems();
    }

    public void ExecuteCancel()
    {
      this._dataSource.ExecuteResetAndCompleteTranstactions();
    }

    public void ExecuteBuyConsumableItem()
    {
      this._dataSource.ExecuteBuyItemTest();
    }

    private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
    {
      return !string.IsNullOrEmpty(item.ItemUsage) ? MBItem.GetItemUsageSetFlags(item.ItemUsage) : (ItemObject.ItemUsageSetFlags) 0;
    }

    private void CloseInventoryScreen()
    {
      InventoryManager.Instance.CloseInventoryPresentation();
    }

    bool IChangeableScreen.AnyUnsavedChanges()
    {
      return this._inventoryState.InventoryLogic.IsThereAnyChanges();
    }
  }
}