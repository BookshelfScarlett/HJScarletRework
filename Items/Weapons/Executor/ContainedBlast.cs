using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Rockets;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class ContainedBlast : ExecutorWeaponClass
    {
        public override WeaponCategory WeaponCategory => WeaponCategory.Firearm;
        public override int ExecutionProgress => 180;
        public override void ExSD()
        {
            Item.damage = 54;
            Item.SetUpNoUseGraphicItem(true, false);
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.useTime = Item.useAnimation = 9;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<ContainedBlastHeldProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.HJScarlet().ForceTacticalExecution = true;
            Item.UseSound = null;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void HoldItem(Player player)
        {
            player.HJScarlet().tacticalExecution = true;
            if (player.HasProj<ContainedBlastHeldProj>(out int projID))
                return;
            Vector2 dir = player.ToMouseVector2();
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = player.MountedCenter.ToRandCirclePos(6);
                Vector2 vel = player.ToMouseVector2() * Main.rand.NextFloat() * 16f * Main.rand.NextBool().ToDirectionInt();
                vel += RandVelTwoPi(0.1f, 0.3f);
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.WhiteSmoke, Color.White), 45, RandRotTwoPi, 1f, 0.35f * Main.rand.NextFloat(.9f, 1.1f), Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = player.MountedCenter.ToRandCirclePos(12);
                Vector2 vel = player.ToMouseVector2() * Main.rand.NextFloat() * 6f * Main.rand.NextBool().ToDirectionInt();
                ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.WhiteSmoke, Color.White), 40, .241f, 0.48f);
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = -0.25f }, player.Center);
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projID, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ClockworkMinigun>().
                AddIngredient<TheBlackBox>().
                AddIngredient(ItemID.IllegalGunParts, 10).
                AddIngredient<FinalBar>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
