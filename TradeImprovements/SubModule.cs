using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TradeImprovements
{
	public class SubModule : MBSubModuleBase
	{
		public override void OnGameLoaded(Game game, object initializerObject)
		{
			base.OnGameLoaded(game, initializerObject);
			MainApp.Instance.GameLoaded(true);
		}
		
	}
}