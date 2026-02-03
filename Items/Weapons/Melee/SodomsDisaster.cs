using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SodomsDisaster : ThrownSpearClass
    {
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 75;
            Item.useTime = Item.useAnimation = 24;
            Item.rare = RarityType<DisasterRarity>();
            Item.shootSpeed = 14;
            Item.shoot = ProjectileType<SodomsDisasterProj>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = HJScarletSounds.SodomsDisaster_Toss with { MaxInstances = 0, Pitch= 0.21f, Volume = 0.74f};
            Item.HJScarlet().EnableCritDamage = true;
            Item.HJScarlet().CritsDamageBonus = 0.30f + 0.70f * HJScarletMethods.HasFuckingCalamity.ToInt();
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 12f;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.Mod == "Terraria" && line.Name =="ItemName")
            {
                DisasterRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override Color MainTooltipColor => Color.IndianRed;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FierySpear>().
                AddIngredient<LightBiteThrown>().
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
