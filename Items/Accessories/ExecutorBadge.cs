using ContinentOfJourney.Items.Accessories;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class ExecutorBadge : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            Type.ShimmerFrom(ItemType<CounsellorBadge>());
            Type.ShimmerTo(ItemType<SwordmasterBadge>());
        }
        public override void ExSD()
        {
            Item.width = Item.height = 28;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.20f;
            player.GetCritChance<ExecutorDamageClass>() += 5;
        }
    }
}
