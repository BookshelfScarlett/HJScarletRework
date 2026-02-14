using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class FlybackHandThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<FlybackHand>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<FlybackHand>();
        public override void ExSD()
        {
            Item.damage = 233;
            Item.useTime = Item.useAnimation = 25;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<FlybackHandThrownProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeExpired with { MaxInstances = 0 , Volume = 1.5f,Pitch = 0.3f, PitchVariance = 0.1f};
            Item.shootSpeed = 26f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ExUpdateInventory(Player player)
        {
            Item.useTime = Item.useAnimation = player.HJScarlet().FlybackBuffTime > 0 ? (10 + (int)(15f * (1f - (float)player.HJScarlet().FlybackBuffTime / player.HJScarlet().CurrentFullFlyBackTime))) : 25;
        }
        public override Color MainTooltipColor => Color.Yellow;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                if (!player.HasProj<FlybackHandClockMounted>(out int projID))
                    Projectile.NewProjectileDirect(source, position, velocity, projID, 0, 0, player.whoAmI);
                else
                {
                    Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                    proj.rotation = velocity.ToRotation();
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
