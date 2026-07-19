using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DualWraithStaff : ExecutorWeaponClass
    {
        public bool AlterVersion = false;
        public override int ExecutionProgress => 50;
        public override WeaponCategory WeaponCategory => WeaponCategory.Caster;
        public override void ExSD()
        {
            Item.damage = 60;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.shootSpeed = 21;
            Item.knockBack = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 40;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<DualWraithStaffHeldProj>();
            Item.HJScarlet().ExecutionProj = ProjectileType<DualWraithStaffJavelin>();

        }
        public override bool CanShoot(Player player)
        {
            return (!player.HasProj<DualWraithStaffHeldProj>() || AlterVersion);
        }
        public override float UseSpeedMultiplier(Player player)
        {
            if (AlterVersion)
                return (Item.useTime / (float)32);
            return base.UseSpeedMultiplier(player);
        }
        public override bool CanUseItem(Player player)
        {
            //if (AlterVersion)
            //{
            //    Item.useTime = Item.useAnimation = 32;
            //}
            //else
            //{
            //    Item.useTime = Item.useAnimation = 40;
            //}
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!AlterVersion)
                return false;
            //这里只是为了初始化
            player.CheckExecution(Type);
            SoundEngine.PlaySound(HJScarletSounds.Atom_StrikeAlt with { MaxInstances = 0, Variants = [2], Pitch = -0.31f, PitchVariance = 0.1f, Volume = 0.68f },player.Center);
            Vector2 pos1 = position + player.ToMouseVector2() * -1200f;
            Vector2 dir1 = pos1.GetNormalVector2(Main.MouseWorld);
            Projectile proj = Projectile.NewProjectileDirect(source, pos1, dir1 * velocity.Length(), Item.HJScarlet().ExecutionProj, damage, knockback, player.whoAmI);
            Vector2 pos2 = player.MountedCenter - player.ToMouseVector2().RotatedBy(PiOver2 * player.direction) * Main.rand.NextFloat(1200f, 1400f) + Vector2.UnitX * Main.rand.NextFloat(-100f, 101f);
            Vector2 dir = pos2.GetNormalVector2(player.LocalMouseWorld());
            Projectile proj2 = Projectile.NewProjectileDirect(source, pos2, dir * Item.shootSpeed, Item.HJScarlet().ExecutionProj, damage, knockback, player.whoAmI);

            proj.HJScarlet().HasExecutionMechanic = true;
            proj2.HJScarlet().HasExecutionMechanic = true;
            return false;
        }
        public override void HoldItem(Player player)
        {
            var usPlayer = player.HJScarlet();
            usPlayer.tacticalExecution = true;
            Asset<Texture2D> tex = !AlterVersion ? HJScarletItemProj.DualWraithStaff.Texture : HJScarletItemProj.DualWraithStaffBlade.Texture;
            TextureAssets.Item[Type] = tex;
            if (!AlterVersion && player.whoAmI == Main.myPlayer && !player.HasProj(Item.shoot))
            {
                player.CheckExecution(Type);
                float anchorPosX = player.MountedCenter.X - player.direction * 80f;
                Vector2 spawnPos = new Vector2(anchorPosX, player.MountedCenter.Y - 250f);
                int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnPos, Vector2.Zero, Item.shoot, projDamage, Item.knockBack, player.whoAmI);
                proj.originalDamage = projDamage;
                proj.netUpdate = true;
            }
            if (AlterVersion && player.whoAmI == Main.myPlayer && !player.HasProj(Item.shoot) && player.HeldItem.type == Type && player.CheckExecution(Type))
            {
                    SoundEngine.PlaySound(HJScarletSounds.Atom_StrikeAlt with { MaxInstances = 0, Variants = [2], Pitch = -0.31f, PitchVariance = 0.1f, Volume = 0.68f }, player.Center);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pos1 = player.MountedCenter + player.ToMouseVector2().RotateRandom(ToRadians(75)).ToSafeNormalize() * -1200f * Main.rand.NextFloat(.75f, 1.2f);
                    if (i > 3)
                    {
                        pos1 = player.MountedCenter - player.ToMouseVector2().RotatedBy(PiOver2 * player.direction) * Main.rand.NextFloat(1200f, 1400f) + Vector2.UnitX * Main.rand.NextFloat(-300f, 301f);
                    }
                    Vector2 dir1 = pos1.GetNormalVector2(Main.MouseWorld);
                    int dmg = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), pos1, dir1 * 18f, Item.HJScarlet().ExecutionProj, dmg, Item.knockBack, player.whoAmI);
                    proj.ai[1] = 1;
                    
                }
                AlterVersion = false;
                player.RemoveExecutionProgress(Type);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreStaff).
                AddIngredient(ItemID.SpectreBar, 12).
                AddIngredient(ItemID.Spear).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
