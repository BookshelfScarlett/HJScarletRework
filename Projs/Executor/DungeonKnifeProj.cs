using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonKnifeProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<DungeonKnife>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public int BounceTime = 0;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LightGray, Color.White), 40, 1, 0.34f, .031f);
            if (Main.rand.NextBool(6))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8, RandLerpColor(Color.LightGray, Color.White), 40, 1, 0.42f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<DungeonKnifeMark>())
                Projectile.AddExecutionTimeImmediate<DungeonKnife>();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
                float centerGlowScale = .2f;
                Vector2 center = Projectile.oldPosition + Projectile.Size / 2f;
                ECSParticle.CrossGlow(center, Color.White, 45, 1, centerGlowScale);
                ECSParticle.CrossGlow(center, Color.White, 45, 1, centerGlowScale * .98f);
                ECSParticle.CrossGlow(center, Color.White, 45, 1, centerGlowScale * .96f);
                for (int i = 0; i < 16; i++)
                    ECSParticle.TurbulenceShinyOrb(center.ToRandCirclePosEdge(16), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.LightGray, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
                ScarletSound(SoundID.Dig, center);

            if (BounceTime > 1)
            {
                Projectile.Kill();
                return true;
            }
            else
            {
                Projectile.BounceOnTile(oldVelocity);
                if (Projectile.GetTargetSafe(out NPC target, true, 600, canPassWall: false))
                {
                    Projectile.velocity = HJScarletMethods.PredictAimToTarget(Projectile.Center, target.Center, target.velocity, 24f, 0);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
                BounceTime += 1;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawProj(Vector2.Zero);
            return false;
        }
        public void DrawProj(Vector2 offset)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                Color c = Color.Lerp(Color.LightGray, Color.White, ratios);
                Vector2 sharpScale = new Vector2(1f, 1.4f);
                SB.Draw(HJScarletTexture.Particle_SharpTear, pos - new Vector2(20, 0).RotatedBy(Projectile.oldRot[i]), null, c.ToAddColor(10) * opa * .35f, Projectile.oldRot[i] + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale * scale, 0, 0);
                c = Color.Lerp(Color.LightGray, Color.White, ratios).ToAddColor(250);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * scale, se);

            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 2f, Color.White.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);
        }
    }
}
