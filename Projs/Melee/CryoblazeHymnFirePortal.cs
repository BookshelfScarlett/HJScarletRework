using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CryoblazeHymnFirePortal : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public bool UseFireColor = false;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            //无限穿透，但是存续时间非常短
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.noEnchantmentVisuals = true;
        }
        public void SpawnEnegry()
        {
            SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { MaxInstances = 1 }, Projectile.Center);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center))
                    TheParticleThatCanSpawn();
            for (int i = 0; i < 3; i++)
            {
                
                if (Projectile.GetTargetSafe(out NPC target, true, 800f, true))
                {
                    Vector2 dir = Projectile.SafeDir().ToRandVelocity(ToRadians(30f));
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 34f, ProjectileType<CryoblazeHymnFireball>(), Projectile.damage, Projectile.knockBack);
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    for (int j = 0; j < 10; j++)
                    {
                        //最大的原因是白天过曝看不到
                        //这里的光球粒子需要携带一个暗色的烟出来
                        Vector2 pos = Projectile.Center.ToRandCirclePos(8f);
                        Vector2 vel = dir.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(8f);
                        //由于粒子绘制原因。shinyorn的速度需要更短
                        new ShinyOrbParticle(pos, vel * 0.8f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, 0.7f).Spawn();
                        new SmokeParticle(pos, vel, RandLerpColor(Color.Orange, Color.Gray), 40, 0.5f, 1f, 0.14f).SpawnToPriorityNonPreMult();
                    }
                }
            }

        }
        private void TheParticleThatCanSpawn()
        {
            new CrossGlow(Projectile.Center, Color.Orange, 40, 1f, 0.35f).Spawn();
            new CrossGlow(Projectile.Center, Color.White, 40, 0.5f, 0.3f).Spawn();
            //而后我们开始生成火球
            for (int i = 0; i < 10; i++)
            {
                new Fire(Projectile.Center.ToRandCirclePos(32f), RandVelTwoPi(4f), RandLerpColor(Color.DarkOrange, Color.Black), 40, RandRotTwoPi, 1f, 0.24f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 15; i++)
            {
                float randX = Main.rand.NextFloat(-10f, 10f);
                float randY = Main.rand.NextFloat(-10f, 10f);
                float randVelocity = Main.rand.NextFloat(5f, 10f);
                float speed = (float)Math.Sqrt((double)(randX * randX + randY * randY));
                speed = randVelocity / speed;
                randX *= speed;
                randY *= speed;
                Vector2 vel = new Vector2(randX, randY) * Main.rand.NextFloat(0.24f, 0.28f);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Torch, vel);
                d.scale *= 1.25f;
                d.noGravity = true;
                //new ShinyOrbParticle(pos, vel, RandLerpColor(Color.OrangeRed, Color.Orange), 30, 0.8f).Spawn();
                //new ShinyOrbParticle(pos, vel, Color.White, 30, 0.3f).Spawn();
            }
            new OpticalLineGlow(Projectile.Center, Color.Orange, 40, 1f, 0.15f).Spawn();
            //new BloomShockwave(Projectile.Center, Color.Gold, 40, 0.74f, 0.15f).Spawn();

        }
        public override void AI()
        {
           //产打底的烟雾与辉光粒子
            if (Projectile.timeLeft < 50)
            {
                Projectile.Opacity -= .1f;
                if (Projectile.timeLeft == 49)
                    SpawnEnegry();
                Vector2 smokePos = Projectile.Center.ToRandCirclePos(32f);
                new SmokeParticle(smokePos, RandVelTwoPi(1.4f), RandLerpColor(Color.Orange, Color.Gray), 30, RandRotTwoPi, 1f, 0.12f).SpawnToPriorityNonPreMult();
            }
            else
            {
                Projectile.velocity *= .89f;
            }
            //这里实际上结合了原版粒子去用，这样尽量会减少场上同时存在的粒子数量
            //yysy这里确实不如用原版的火花粒子
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(32f), DustID.Torch, RandVelTwoPi(0.4f, 0.8f));
                d.noGravity = true;
                d.scale *= 1.25f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D crossGlow = HJScarletTexture.Particle_CrossGlow.Value;
            Texture2D circle = HJScarletTexture.Texture_SoftCircleEdge.Value;
            //底图处理
            SB.Draw(HJScarletTexture.Texture_BloomShockwave.Value, drawPos, null, Color.Orange, 0, HJScarletTexture.Texture_BloomShockwave.Origin, 0.02f * Projectile.scale * Projectile.Opacity, 0, 0);
            //光圈，叠加
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //绘制辉光
            SB.Draw(circle, drawPos, null, Color.Orange, 0, circle.Size() / 2, 0.12f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.OrangeRed, Projectile.rotation, crossGlow.Size() / 2, 0.17f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.White, Projectile.rotation, crossGlow.Size() / 2, 0.08f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.End();
            SB.BeginDefault();
            return false;
        }
    }
}
