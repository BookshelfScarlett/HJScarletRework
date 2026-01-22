using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Melee
{
    public class SodomsDisasterProj : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => ProjPath  + nameof(SodomsDisaster);
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(16, 2);
        public enum Style
        {
            Shoot,
            StabOnTarget,
            StabOnGround
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public Vector2 StabPosition = Vector2.Zero;
        public ref int TargetIndex => ref Projectile.HJScarlet().GlobalTargetIndex;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 4;
        }
        //AI总控的基本上是特效方面的绘制
        //和转角问题
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 dir = Projectile.SafeDirByRot();
            //超出距离立刻处死射弹
            if (Projectile.TooAwayFromOwner())
                Projectile.Kill();
            //如果超出玩家屏幕范围内，做掉所有特效
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            //先生成starShape, 而后生成需要的火焰粒子，最后再生成需要的shinyorb
            for (int k = 0; k < 2;k++)
            {
                Vector2 starShapePos = Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f) - dir * Main.rand.NextFloat(0.9f, 1.2f);
                Color drawColor = RandLerpColor(Color.Black, Color.DarkRed);
                new StarShape(starShapePos, dir * 2f, drawColor, 0.6f, 20).SpawnToPriorityNonPreMult();
            }

            for (int j = 0; j < 1; j++)
            {
                Color Firecolor = RandLerpColor(Color.DarkRed, Color.Crimson);
                new SmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(4, 4), dir * Main.rand.NextFloat(2.4f, 3.6f), Firecolor, Main.rand.Next(30, 41), Main.rand.NextFloat(TwoPi), 1f, Main.rand.NextFloat(0.24f, 0.29f)).SpawnToNonPreMult();
            }

            Vector2 drawPos = Projectile.Center + dir * 30f;
            for (int i = 0;i< 2;i++)
            {
                new ShinyOrbParticle(drawPos + Main.rand.NextVector2CircularEdge(4f, 4f), dir * Main.rand.NextFloat(2.4f, 3.6f), RandLerpColor(Color.DarkRed, Color.Red), 20, 0.43f).Spawn();
            }
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //直接处死
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<SodomsDisasterBoom>(), Projectile.damage, 12f, Owner.whoAmI);
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1 }, Projectile.Center);
            return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public override bool ShouldUpdatePosition() => AttackType == Style.Shoot && Projectile.velocity.Length() > 0f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<SodomsDisasterBoom>(), Projectile.damage, 12f, Owner.whoAmI);
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 1 }, Projectile.Center);
            for (int i = 0; i < 3;i++)
            {
                float spawnX = target.Center.X + Main.rand.NextFloat(50f, 100f) * Main.rand.NextBool().ToDirectionInt();
                float spawnY = target.Center.Y - Main.rand.NextFloat(1200f, 1800f);
                Vector2 spawnPos = new Vector2(spawnX, spawnY);
                Vector2 dir = (target.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, dir * Main.rand.NextFloat(12f, 16f), ProjectileType<SodomsDisasterFireball>(), Projectile.damage, 12f, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            DrawTheTrail(drawPos, starShape);
            Projectile.DrawGlowEdge(Color.Red,posMove: 1.4f, rotFix:PiOver4);
            Projectile.DrawProj(Color.White, rotFix:PiOver4);
            DrawGlow(drawPos);
            return false;
        }

        private void DrawGlow(Vector2 drawPos)
        {
            SB.EnterShaderArea();
            Vector2 dir = Projectile.SafeDirByRot();
            Vector2 glowCirclePos = drawPos + dir * 68f;
            SB.Draw(HJScarletTexture.Texture_SoftCircleEdge.Value, glowCirclePos, null, Color.Red, Projectile.rotation, HJScarletTexture.Texture_SoftCircleEdge.Origin, Projectile.scale * 0.30f, 0, 0);
            Tex2DWithPath lineGlow = HJScarletTexture.Particle_OpticalLineGlow;
            Vector2 glowScale = Projectile.scale * new Vector2(1.2f, 0.7f);
            SB.Draw(lineGlow.Value, drawPos + dir * 52f, null, Color.DarkRed, Projectile.rotation, lineGlow.Origin, glowScale * 0.22f, 0, 0);
            SB.EndShaderArea();
        }

        public void DrawTheTrail(Vector2 drawPos, Texture2D starShape)
        {
            Vector2 offset = Projectile.SafeDirByRot() +Projectile.SafeDirByRot() * 30f;
            int length = Projectile.oldPos.Length;
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos[i].Equals(Vector2.Zero))
                    continue;
                Vector2 thePos = drawPos + offset - Projectile.SafeDirByRot() * 5.2f * i;
                drawPos = Projectile.oldPos[i] + Projectile.PosToCenter() + offset;
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.DarkRed, Color.Red, rads)) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Color drawColor2 = (Color.Lerp(Color.White, Color.Crimson, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 scale = Projectile.scale * new Vector2(0.8f, 2f);
                SB.Draw(starShape, thePos, null, drawColor, Projectile.oldRot[i] - PiOver2, starShape.Size() / 2, scale, 0, 0);
                if (i > 18)
                    continue;
                SB.Draw(starShape, thePos, null, drawColor2, Projectile.oldRot[i] - PiOver2, starShape.Size() / 2, scale * 0.4f, 0, 0);
            }
        }
    }
}
