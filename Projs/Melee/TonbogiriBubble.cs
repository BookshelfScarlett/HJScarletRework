using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriBubble : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<VenomBubble>().Texture;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8, 2);
        }
        public enum Style
        {
            Spawn,
            ShootLaser,
            Disapper
        }
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float SpriteRotation => ref Projectile.localAI[0];
        public int SourceDamage => Projectile.originalDamage;
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 300;
            Projectile.scale *= 1.05f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            //这里会起码延后一帧进行
            Projectile.rotation = Projectile.velocity.ToRotation(); ;
            SpriteRotation += ToRadians(10f);
            float rate = Clamp(Projectile.velocity.Length(), 0f, 1f);
            float spawnSize = 4f * rate;
            Dust venom = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(spawnSize, spawnSize), DustID.VenomStaff);
            venom.velocity = Projectile.SafeDirByRot() * Main.rand.NextFloat(1.2f, 2.1f);
            venom.noGravity = true;
            venom.scale *= 0.87f;


            //泡泡本身不允许造成任何伤害，这里在AI写死以避免任何可能的外部直接修改
            switch (AttackType)
            {
                case Style.Spawn:
                    DoSpawn();
                    break;
                case Style.ShootLaser:
                    DoShootLaser();
                    break;
            }
        }
        public void DoSpawn()
        {
            Timer++;
            Projectile.velocity *= 0.96f;
            if (Timer > 20f)
            {
                AttackType = Style.ShootLaser;
                Projectile.netUpdate = true;
                Timer = 0;
            }
        }
        public void DoShootLaser()
        {
            Vector2 dir = Projectile.SafeDir();
            Vector2 spawnPos = Projectile.Center;
            Vector2 offset = dir * Main.rand.NextFloat(4.5f) + dir.RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(4.4f);
            Vector2 vel = dir * Main.rand.NextFloat(1.2f, 1.4f);
            new ShinyOrbParticle(spawnPos + offset, vel, RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.3f).Spawn();

            //用这个Timer发射laser，这里只会一次发射两个
            if (Projectile.GetTargetSafe(out NPC target))
                Projectile.HomingTarget(target.Center, -1f, 15f, 30f);
            else
                Projectile.Kill();
        }
        public override bool? CanDamage() => AttackType == Style.ShootLaser;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, GetSeconds(5));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreKill(int timeLeft)
        {
            //处死时生成点粒子
            for (int i = 0;i < 10;i++)
            {
                new TurbulenceGlowOrb(Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f), 0.24f, RandLerpColor(Color.DarkViolet, Color.Pink), 40, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();
            }
            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            //绘制残影
            Color edgeColor = Color.HotPink.ToAddColor(50) * Projectile.Opacity;
            float scale = 1f;
            int length = Projectile.oldPos.Length;
            SB.Draw(projTex, projPos, null, Color.Pink.ToAddColor(50), Projectile.rotation, ori, new Vector2(1f, scale) * Projectile.scale, 0, 0);
            for (int i = 0; i < length; i++)
            {
                edgeColor *= 0.85f;
                scale *= 0.93f;
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, new Vector2(1f * scale) * Projectile.scale, 0, 0);
            }
            //白色高光打底
            edgeColor = Color. WhiteSmoke.ToAddColor(15) * Projectile.Opacity;
            scale = 1f;
            SB.Draw(projTex, projPos, null, Color.WhiteSmoke.ToAddColor(15), Projectile.rotation, ori, new Vector2(1f, scale) * Projectile.scale * 0.45f, 0, 0);
            for (int i = 0; i < length; i++)
            {
                edgeColor *= 0.85f;
                scale *= 0.93f;
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, new Vector2(1f * scale) * Projectile.scale * 0.45f, 0, 0);
            }
            return false;
        }
    }
}
