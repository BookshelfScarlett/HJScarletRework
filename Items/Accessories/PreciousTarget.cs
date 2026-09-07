using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class PreciousTarget : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public float Damage = .10f;
        public int Crit = 5;
        public static float ExtraDamage = 1.05f;
        public static float ChanceToCrit = .20f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%", ExtraDamage + "x", ChanceToCrit.ToPercent());
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RangedDamageClass>() += Damage;
            player.GetCritChance<RangedDamageClass>() += Crit;
            player.HJScarlet().preciousTargetLevel = 1;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RangerEmblem).
                AddIngredient(ItemID.ShroomiteBar, 15).
                AddTile(TileID.Autohammer).
                Register();
        }
    }
}
