using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class IceFireEnergy : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.penetrate = 4;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 350;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {

            //本质素材复用，但是这里的粒子总体来说，一定程度上降低了生成数量，因为这里可能场上会有非常非常多
            //向上喷火的粒子，使用原版的更加合适一些。
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            Vector2 pos = Projectile.Center.ToRandCirclePos(4f);
            Vector2 vel = -Projectile.velocity / 4;
            //这里的球比起雪球，更接近于霜火球
            //因此，会沿途留下烟火轨迹
            //烟的粒子会靠后一点，并且会先生成让原版的冰粒子正常绘制
            if (Main.rand.NextBool())
                new SmokeParticle(pos - Projectile.SafeDir() * 15f, vel * 0.5f, RandLerpColor(Color.DeepSkyBlue, Color.White), 20, 0.5f, 1f, 0.15f).SpawnToPriorityNonPreMult();
            //生成光球类粒子。增效
                //new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity.ToRandVelocity(ToRadians(10f), -1.2f), RandLerpColor(Color.DeepSkyBlue, Color.LightBlue), 15, 0.35f).Spawn();
                //这里会附带原版的火焰粒子用于一定程度上的增效
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(8f), Main.rand.NextBool() ? DustID.IceTorch : DustID.WhiteTorch, Projectile.velocity.ToRandVelocity(ToRadians(10f), -1.2f));
                d.noGravity = true;
                d.scale *= 1.35f;
            Projectile.position += RandVelTwoPi(-1.2f,1.2f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
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
            //撞击墙体时反弹，同时注意这里记录撞击墙体的总次数
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.90f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightSkyBlue, Color.Lerp(Color.LightSkyBlue, Color.Blue, rads * 0.7f), (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DeepSkyBlue.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
}
