using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.Specialized;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<DeepTone>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(24, 2);
        public enum Style
        {
            Shoot,
            Stuck
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public List<Vector2> PortalPosList = [];
        public ref float AttackTimer => ref Projectile.ai[0];
        public Vector2 ImpactPos = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.extraUpdates = 5;
            Projectile.height = Projectile.width = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            switch(AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.Stuck:
                    DoStuck();
                    break;
            }
        }

        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            //粒子
            Vector2 offset = Projectile.SafeDir() * 90f + Projectile.SafeDir().RotatedBy(PiOver2) * 2.1f;
            Vector2 mountedPos = Projectile.Center + offset;
            Dust d = Dust.NewDustPerfect(mountedPos + Main.rand.NextVector2Circular(6f, 6f), DustID.GreenTorch);
            d.noGravity = true;
            new TurbulenceShinyOrb(mountedPos + Main.rand.NextVector2Circular(6f, 6f), 1.2f, RandLerpColor(Color.DarkOliveGreen, Color.DarkSeaGreen), 40, 0.1f, Projectile.SafeDir().ToRotation()).SpawnToPriority();
            //除非超出这个最大点位数，不然添加传送门了
        }
        public void DoStuck()
        {
            Projectile.velocity *= 0.01f;
            NPC target = Projectile.ToHJScarletNPC();
            if(target != null && target.CanBeChasedBy())
            {
                Projectile.Center = target.Center + ImpactPos;
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override bool PreKill(int timeLeft)
        {
            NPC target = Projectile.ToHJScarletNPC();
            if (target != null)
                target.HJScarlet().DeepToneThrownMark = false;
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch(AttackType)
            {
                case Style.Shoot:
                    DoShoot_OnHit(target, hit, damageDone);
                    break;
                case Style.Stuck:
                    DoStuck_OnHit(target, hit, damageDone);
                    break;
            }
        }

        private void DoStuck_OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnTenctacle_Portal(target, hit, damageDone);
        }
        private void DoShoot_OnHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnTenctacle(target, hit, damageDone);
        }
        private void SpawnTenctacle_Portal(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int j = 0; j < 3; j++)
            {
                //随机生成的位置方向
                Vector2 realspawnPos = Projectile.Center + Vector2.UnitY.RotatedBy(Main.rand.NextFloat(PiOver4) * Main.rand.NextBool().ToDirectionInt()) * -30f;
                //什么玩意，为啥咋生成出来的位置都是一样的
                Vector2 dir = (realspawnPos - target.Center).SafeNormalize(Vector2.UnitX);
                for (int k = 0; k < 3; k++)
                {
                    //让开始的方向有一定的偏差
                    Vector2 randDir = dir.RotatedBy(Main.rand.NextFloat(ToRadians(10f) * Main.rand.NextBool().ToDirectionInt()));
                    float tentacleYDirection = Main.rand.Next(10, 120) * 0.001f;
                    if (Main.rand.NextBool())
                    {
                        tentacleYDirection *= -1f;
                    }
                    float tentacleXDirection = Main.rand.Next(10, 120) * 0.001f;
                    if (Main.rand.NextBool())
                    {
                        tentacleXDirection *= -1f;
                    }
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), realspawnPos, randDir * Main.rand.NextFloat(12f, 18f), ProjectileType<DeepToneTenctacle>(), Projectile.damage, 0f, Owner.whoAmI, tentacleXDirection, tentacleYDirection);
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    ((DeepToneTenctacle)proj.ModProjectile).InitPos = realspawnPos;
                }
            }
        }
        private void SpawnTenctacle(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 randDir = Main.rand.NextFloat(TwoPi).ToRotationVector2();
                float tentacleYDirection = (float)Main.rand.Next(10, 160) * 0.001f;
                if (Main.rand.NextBool())
                {
                    tentacleYDirection *= -1f;
                }
                float tentacleXDirection = (float)Main.rand.Next(10, 160) * 0.001f;
                if (Main.rand.NextBool())
                {
                    tentacleXDirection *= -1f;
                }
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, randDir * Main.rand.NextFloat(12f, 18f), ProjectileType<DeepToneTenctacle>(), Projectile.damage, 0f, Owner.whoAmI, tentacleXDirection, tentacleYDirection);
                ((DeepToneTenctacle)proj.ModProjectile).InitPos = Projectile.Center;
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 offset = Projectile.SafeDir() * 90f + Projectile.SafeDir().RotatedBy(PiOver2) * 2.1f;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.DarkOliveGreen, Color.DarkSeaGreen, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                SB.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter() + offset, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1f, 1.5f), 0, 0);
            }
            for (int k = 0; k < 6; k++)
            {
                float rads2 = (float)k / 6;
                Color drawColor = (Color.Lerp(Color.DeepPink, Color.LightPink, rads2) with { A = 0 }) * 0.5f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads2);
                Vector2 offsetPos = Projectile.SafeDir() * (110f - 10f * k) + Projectile.SafeDir().RotatedBy(PiOver2) * 1.5f;
                SB.Draw(star, Projectile.Center - Main.screenPosition + offsetPos, null, drawColor, Projectile.rotation - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1f, 1.2f), 0, 0);
            }
            //最顶端位置，绘制一个小的辉光 
            Projectile.DrawGlowEdge(Color.White, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White, drawTime: 4, rotFix: ToRadians(135));
            return false;
        }
    }
}
