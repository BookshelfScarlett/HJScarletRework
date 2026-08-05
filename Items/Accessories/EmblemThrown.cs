using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class EmblemThrown : HJScarletItemClass
    {
        public float CritDamage = .30f;
        public int Crit = 5;

        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent(), Crit + "%");
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HeldItem.CheckExecuteTypes(ExecutorWeaponType.Throw))
            {
                player.HJScarlet().critDamageExecutor += CritDamage;
                player.GetCritChance<ExecutorDamageClass>() += Crit;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AvengerEmblem).
                AddIngredient(ItemID.EyeoftheGolem).
                AddIngredient(ItemID.Bomb, 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
