using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorEmblem : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 38;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.15f;
        }
    }
}
