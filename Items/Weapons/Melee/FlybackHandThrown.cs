using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class FlybackHandThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<FlybackHand>().Texture;
        public override void ExSD()
        {
            Item.damage = 415;
            Item.useTime = Item.useAnimation = 20;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<FlybackHandThrownProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeExpired with { MaxInstances = 0, Volume = 1.5f, Pitch = 0.3f, PitchVariance = 0.1f };
            Item.shootSpeed = 26f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                TimeRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ExUpdateInventory(Player player)
        {
            Item.useTime = Item.useAnimation = player.HJScarlet().flybackhandBuffTime > 0 ? (10 + (int)(10f * (1f - (float)player.HJScarlet().flybackhandBuffTime / player.HJScarlet().flybackhandBuffTimeCurrent))) : 20;
        }
        public override Color MainTooltipColor => Color.Yellow;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                if (!player.HasProj<FlybackHandClockMounted>(out int projID) && player.HJScarlet().flybackhandCloclCD == 0)
                    Projectile.NewProjectileDirect(source, position, velocity, projID, 0, 0, player.whoAmI);
                else
                {

                    string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.TimeClockOnCDText").ToLangValue().ToFormatValue(player.HJScarlet().flybackhandCloclCD / 60);
                    CombatText.NewText(player.Hitbox, Color.SkyBlue, value, true);
                    return false;
                }
            }
            else 
            {
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                proj.rotation = velocity.ToRotation();
            }
            return false;
        }
    }
}
