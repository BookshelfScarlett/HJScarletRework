using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CryoblazeHymnFrostPortal : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public bool UseFireColor = false;
        public NPC HomingTarget = null;
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
            int spawnCounts = HJScarletMethods.HasFuckingCalamity ? 6 : 3;
            SoundEngine.PlaySound(SoundID.Item109 with { MaxInstances = 1 }, Projectile.Center);
            new CrossGlow(Projectile.Center, Color.SkyBlue, 40, 1f, 0.25f,false).Spawn();
            new CrossGlow(Projectile.Center, Color.White, 40, 0.5f, 0.20f,false).Spawn();
            for (int i = 0; i < spawnCounts; i++)
            {
                Vector2 dir = Projectile.SafeDir().ToRandVelocity(ToRadians(30f));
                //注意这里：如果没有找到正常的敌人，这里是不会尝试生成的
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 800, canPassWall: true))
                {
                    if (Projectile.IsMe())
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 15f, ProjectileType<CryoblazeHymnFrostEnergy>(), Projectile.originalDamage / 2, Projectile.knockBack);
                        proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        //最大的原因是白天过曝看不到
                        //这里的光球粒子需要携带一个暗色的烟出来
                        Vector2 pos = Projectile.Center.ToRandCirclePos(8f);
                        Vector2 vel = dir.ToRandVelocity(ToRadians(10f)) * Main.rand.NextFloat(8f);
                        //由于粒子绘制原因。shinyorn的速度需要更短
                        new ShinyOrbParticle(pos, vel * 0.8f, RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue), 40, 0.7f).Spawn();
                        new SmokeParticle(pos, vel, RandLerpColor(Color.DeepSkyBlue, Color.Gray), 40, 0.5f, 1f, 0.14f).SpawnToPriorityNonPreMult();
                    }
                }
            }
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
                new SmokeParticle(smokePos, RandVelTwoPi(1.4f), RandLerpColor(Color.DeepSkyBlue, Color.Gray), 30, RandRotTwoPi, 1f, 0.12f).SpawnToPriorityNonPreMult();
            }
            else
                Projectile.velocity *= .89f;
            for (int i = 0; i < 3; i++)
                new TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(32f), Main.rand.NextFloat(0.4f, 0.8f), RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue), 30, 0.18f, RandRotTwoPi).Spawn();
        }
          public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D crossGlow = HJScarletTexture.Particle_CrossGlow.Value;
            Texture2D circle = HJScarletTexture.Texture_SoftCircleEdge.Value;
            float scaleLerp = Projectile.Opacity * Projectile.scale;
            //底图处理
            SB.Draw(HJScarletTexture.Texture_BloomShockwave.Value, drawPos, null, Color.DeepSkyBlue, 0, HJScarletTexture.Texture_BloomShockwave.Origin, 0.02f * scaleLerp, 0, 0);
            //光圈，叠加
            SB.EnterShaderArea();
            //绘制辉光
            SB.Draw(circle, drawPos, null, Color.DeepSkyBlue, 0, circle.Size() / 2, 0.12f * scaleLerp, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.SkyBlue, Projectile.rotation, crossGlow.Size() / 2, 0.17f * scaleLerp, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.White, Projectile.rotation, crossGlow.Size() / 2, 0.08f * scaleLerp, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
