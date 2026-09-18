using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.General;
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
            if (player.HeldItem.IsLegal() && player.HeldItem.DamageType.CountsAsClass<RangedDamageClass>())
            {
                if (!hideVisual && !player.IsInInventory())
                    player.HJScarlet().cursorID = 1;
                if (player.whoAmI == Main.myPlayer && !player.HasProj<PreciousTargetCross>())
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_Accessory(Item), Main.MouseWorld, Vector2.Zero, ProjectileType<PreciousTargetCross>(), 0, 0, player.whoAmI);
                }
            }
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
