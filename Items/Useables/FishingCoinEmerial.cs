using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class FishingCoinEmerial : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void ExSD()
        {
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(0, 0, 50, 0);
        }
    }
}
