using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class FlowerofDance : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.Donator);
        }
        public override void ExSD()
        {
            Item.damage = 145;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.SetUpNoUseGraphicItem();
            Item.shootSpeed = 16;
            Item.UseSound = HJScarletSounds.Misc_KnifeTossAlt with { Pitch = 0.5f, Variants = [2] };
            Item.HJScarlet().ItemBelongTo = ItemBelong.Donator;
            Item.shoot = ProjectileType<FlowerofDanceProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -2; i < 2; i++)
            {

                float baseRad = ToRadians(15f *i);
                Vector2 这个刀就不能再小点吗 = velocity.RotatedBy(Main.rand.NextFloat(ToRadians(-1f), ToRadians(1f)) + baseRad).ToSafeNormalize();
                Vector2 vel = 这个刀就不能再小点吗 * Main.rand.NextFloat(19f, 22f);
                Projectile proj = Projectile.NewProjectileDirect(source, position, vel, type, damage, knockback, player.whoAmI);
                if (i == 0)
                    proj.ai[2] = 1;
            }
            return false;
        }
    }
}
