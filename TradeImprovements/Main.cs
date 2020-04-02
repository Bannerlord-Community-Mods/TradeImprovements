using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace FeudalQoL
{
	public class Main : MBSubModuleBase
	{
		private bool _gameLoaded;
		private Game _game;

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			base.OnGameLoaded(game, initializerObject);

			_gameLoaded = true;
			_game = game;
		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);

			if (_gameLoaded)
			{
				InventoryFn();
			}
		}

		private void InventoryFn()
		{
			if (InventoryManager.MyInventoryLogic != null)
			{
				var layers = ScreenManager.SortedActiveLayers;
				var inventoryLayer = (GauntletLayer) ScreenManager.FocusedLayer;
				var movie = inventoryLayer._moviesAndDatasources[0];
				var inventoryVM = (SPInventoryVM)movie.Item2;
				AddMarginsToItems(inventoryVM.RightItemListVM);
				AddMarginsToItems(inventoryVM.LeftItemListVM);
			}
		}

		private static void AddMarginsToItems(MBBindingList<SPItemVM> itemList)
		{
			foreach (var item in itemList)
			{
				var baseElement = item.ItemRosterElement;
				var basePrice = baseElement.EquipmentElement.ItemValue;
				var currentPrice = item.ItemCost;
				var margin = CalculateMargin(currentPrice, basePrice);
				var baseName = baseElement.EquipmentElement.Item.Name;
				item.ItemDescription = $"{baseName} {margin:+#;-#;+0}%";
			}
		}

		private static double CalculateMargin(double currentPrice, double basePrice)
		{
			return (currentPrice - basePrice) * 100 / basePrice;
		}
	}
}