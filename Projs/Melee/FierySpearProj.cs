using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FierySpearProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + "Proj_" + nameof(FierySpear);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public Vector2 PosOffsetFix => Projectile.SafeDir() * 60f;
        public override void ExSD()
        {
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.scale *= 1.0f;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 spawnPos = Projectile.Center - Projectile.SafeDir() * 60f;
            //获得最前面的顶点位置
            int i = 0;
            while (i < 4)
            {
                if (i < 2)
                {
                    if (Main.rand.NextBool())
                    {
                        Dust d = Dust.NewDustPerfect(spawnPos.ToRandCirclePosEdge(6f), DustID.Torch);
                        d.velocity = Projectile.SafeDir() * Main.rand.NextFloat(0.5f, 1.4f);
                        d.scale = 0.98f;
                        d.noGravity = true;
                    }
                }
                else
                {
                    if (Main.rand.NextBool(4))
                        new ShinyCrossStar(spawnPos.ToRandCirclePosEdge(6f), Projectile.SafeDir() * Main.rand.NextFloat(0.5f, 1.3f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 30, 0, 1, 0.45f, false).Spawn();

                }
                i++;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, GetSeconds(5));
            SpawnVolcanoDustAndProj(target.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.IsMe())
                SpawnVolcanoDustAndProj();

            return true;
        }
        public void SpawnVolcanoDustAndProj(int targetIndex = -1)
        {
            int spawnBallCounts = 2;
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(0f, 8f) * Main.rand.NextBool().ToDirectionInt();
                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                if (Main.rand.NextBool())

                    new SmokeParticle(spawnpos, Projectile.oldVelocity.ToSafeNormalize().RotatedByRandom(Pi) * Main.rand.NextFloat(0.2f, 8f), RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.75f), Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
            }
            for (int j = 0; j < 30; j++)
            {
                Vector2 dir = -Projectile.SafeDirByRot();
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 6f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.Orange, Color.OrangeRed), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
            }
            for (int i = 0; i < 16; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                Vector2 vel = RandVelTwoPi(1f, 4.9f);
                new HRShinyOrb(pos, vel, RandLerpColor((Color.Lerp(Color.Red, Color.Orange, 0.5f)), Color.OrangeRed), 40, 0.12f).Spawn();
                new HRShinyOrb(pos, vel, Color.White, 40, 0.12f * 0.5f).Spawn();
            }
            for (int i = 0; i < spawnBallCounts; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot().ToRandVelocity(PiOver4);
                float speed = targetIndex == -1 ? -Main.rand.NextFloat(6f, 8f) : -Main.rand.NextFloat(6f, 12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * speed, ProjectileType<FierySpearFireball>(), Projectile.damage / 2, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = targetIndex;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            Vector2 offsetValue = PosOffsetFix;
            int drawLength = Projectile.oldPos.Length;
            for (int i = drawLength - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 trailingDrawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.10f) + Projectile.PosToCenter() - offsetValue;
                float faded = 1 - i / (float)drawLength;
                //平方放缩
                faded = MathF.Pow(faded, 3);
                Color trailColor = Color.Lerp(Color.OrangeRed, Color.Lerp(Color.Orange, Color.White, 0.8f), faded) * 0.9f;
                float opa = Lerp(0.85f, 1f, faded);
                trailColor = trailColor.ToAddColor((byte)(Lerp(105, 0, faded))) * opa;
                float scaleMult = Lerp(0.60f, 1f, faded);
                SB.Draw(tex, trailingDrawPos, null, trailColor, Projectile.oldRot[i] + PiOver4, orig, Projectile.scale * scaleMult, 0, 0);
            }
            SB.Draw(projTex, drawPos - offsetValue, null, Color.White, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
