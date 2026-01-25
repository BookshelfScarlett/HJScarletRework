using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FierySpearProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath +"Proj_" + nameof(FierySpear);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public override void ExSD()
        {
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (HJScarletMethods.HasFuckingCalamity && !Projectile.HJScarlet().FirstFrame)
            {
                Projectile.extraUpdates = 5;
                Projectile.penetrate = 2;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool())
            {
                //获得最前面的顶点位置
                new SmokeParticle(Projectile.Center.ToRandCirclePosEdge(6f), -Projectile.velocity / 4, RandLerpColor(Color.Orange, Color.OrangeRed), 40, RandRotTwoPi, 1, 0.24f).Spawn();
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 drawPos = Projectile.Center + dir * 60f;
                //而后，获取粒子需要的方向
                //而后我们开始绘制需要的粒子
                int i = 0;
                while (i < 2)
                {
                    new TurbulenceGlowOrb(drawPos.ToRandCirclePosEdge(12f), 0.8f, RandLerpColor(Color.OrangeRed, Color.Orange), 40, 0.12f, RandRotTwoPi).Spawn();
                    i++;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnVolcanoDustAndProj(target.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnVolcanoDustAndProj();
            return true;
        }
        public void SpawnVolcanoDustAndProj(int targetIndex = -1)
        {
            int spawnBallCounts = 2 + HJScarletMethods.HasFuckingCalamity.ToInt() * 2;
            for (int i = 0; i < spawnBallCounts; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot().ToRandVelocity(PiOver4);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * -Main.rand.NextFloat(6f, 12f), ProjectileType<FierySpearFireball>(), Projectile.damage, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = targetIndex;
                for (int j = 0; j < 30; j++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    dust.velocity -= dir * 12f * Main.rand.NextFloat(0.1f, 0.5f);
                    dust.scale *= Main.rand.NextFloat(1.5f, 3f);
                    dust.noGravity = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            DrawSideStreak(star, drawPos);
            Projectile.DrawGlowEdge(Color.Gold, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, offset: 0.3f, rotFix: PiOver4);
            
            return false;
        }
        public void DrawSideStreak(Texture2D star, Vector2 drawPos)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float rads = (float)i / 10;
                Color drawColor = (Color.Lerp(Color.OrangeRed, Color.Orange, rads) with { A = 0 }) * 0.7f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Main.spriteBatch.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter(), null, drawColor, Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.5f, 1.0f) * 1.2f, 0, 0);
            }
        }
    }
}
