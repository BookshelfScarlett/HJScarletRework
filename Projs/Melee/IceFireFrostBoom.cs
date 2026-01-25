using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class IceFireFrostBoom : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            //无限穿透，但是存续时间非常短
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0f;
            Projectile.scale = 1f;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.velocity *= 0;
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            //辉光，一层即可
            if(!Projectile.HJScarlet().FirstFrame)
            {
                new CrossGlow(Projectile.Center, Color.SkyBlue, 40, 1f, 0.35f).Spawn();
                new CrossGlow(Projectile.Center, Color.White, 40, 0.5f, 0.3f).Spawn();
            }
            //产打底的烟雾与辉光粒子
            Vector2 smokePos = Projectile.Center.ToRandCirclePos(32f);
            new SmokeParticle(smokePos, RandVelTwoPi(1.4f), RandLerpColor(Color.DeepSkyBlue, Color.Gray), 30, RandRotTwoPi, 1f, 0.12f).SpawnToPriorityNonPreMult();
            //这里实际上结合了原版粒子去用，这样尽量会减少场上同时存在的粒子数量
            for (int i = 0;i<3;i++)
            {
                //Vector2 spawnPos = Projectile.Center.ToRandCirclePos(32f);
                //float rot = RandRotTwoPi;
                //new TurbulenceShinyOrb(spawnPos, 1f, RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue), 30, 0.25f, rot).Spawn();
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(32f), Main.rand.NextBool() ? DustID.IceTorch : DustID.WhiteTorch, RandVelTwoPi(1.2f, 1.4f));
                d.noGravity = true ;
                d.scale *= 1.4f;
            }
            //最后让粒子下雪。是的。
            
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
