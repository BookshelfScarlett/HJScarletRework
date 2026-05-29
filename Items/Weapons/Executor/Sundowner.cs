using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Sundowner : ExecutorWeaponClass
    {
        public override int ExecutionProgress =>120;
        public override void ExSSD()
        {
            HJScarletList.DisasterRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.damage = 54;
            Item.SetUpNoUseGraphicItem(true,false);
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DisasterBar>(10).
                AddIngredient(ItemID.RocketLauncher).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
        public override bool CanUseItem(Player player) => true;
        public override void HoldItem(Player player)
        {
            if (player.HasProj<SundownerHeldProj>(out int projID))
                return;
            Vector2 dir = player.ToMouseVector2();
            for (int i = 0; i < 26; i++)
            {
                Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), 8.8f, 10.8f) + player.velocity;
                Vector2 offset = dir.ToRandVelocity(ToRadians(0), 6f, 9f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 posOffset = offset + player.velocity + Main.rand.NextVector2Circular(10f,5f);
                new ShinyCrossStar(player.Center.ToRandCirclePos(20f) +  posOffset- Vector2.UnitY * 12f, vel, RandLerpColor(Color.OrangeRed, Color.Orange), 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.5f, 0.8f), false, 0.2f).Spawn();
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 vel = -dir.ToRandVelocity(ToRadians(4f), -10.8f, 10.8f) + player.velocity;
                Vector2 offset = dir.ToRandVelocity(ToRadians(0), 7, 11f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 posOffset = offset + player.velocity + Main.rand.NextVector2Circular(10f,5f);
                new SmokeParticle(player.Center.ToRandCirclePos(20f) + posOffset - Vector2.UnitY * 12f, vel, RandLerpColor(Color.OrangeRed, Color.Gray), 40, RandRotTwoPi, 1f, 0.34f, Main.rand.NextBool()).SpawnToPriority();
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 1, Pitch = -0.25f }, player.Center);
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projID, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
        }
    }
}
