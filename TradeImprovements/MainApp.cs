using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TradeImprovements
{
    public class MainApp
    {
        public static readonly MainApp Instance = new MainApp();
        private bool _gameLoaded;

        public void GameLoaded(bool status)
        {
            _gameLoaded = status;
        }

        public void OnInventoryScreenFrame(SPInventoryVM dataSource)
        {
            if (!_gameLoaded) return;
            
            var hasPerk = Hero.MainHero.GetPerkValue(DefaultPerks.Trade.WholeSeller);
           
            if (hasPerk)
            {
                AddMarginsToItems(dataSource.RightItemListVM);
                AddMarginsToItems(dataSource.LeftItemListVM);
            }
        }

        private static void AddMarginsToItems(IEnumerable<SPItemVM> itemList)
        {
            foreach (var item in itemList)
            {
                var baseElement = item.ItemRosterElement;
                var basePrice = baseElement.EquipmentElement.ItemValue;
                var currentPrice = item.ItemCost;
                var margin = CalculateMargin(currentPrice, basePrice);
                var baseName = baseElement.EquipmentElement.Item.Name;
                item.ItemDescription = $"{margin:+#;-#;+0}% {baseName}";
            }
        }

        private static double CalculateMargin(double currentPrice, double basePrice)
        {
            return (currentPrice - basePrice) * 100 / basePrice;
        }
    }
}