using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class GrassKnifeProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<GrassKnife>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.GetTargetSafe(out NPC tagrget, searchDistance: 500) && Projectile.numHits < 1)
                Projectile.HomingTarget(tagrget.Center, -1, 16f, 10f);
            else
            {
                if (Projectile.velocity.LengthSquared() < 16f * 16f)
                    Projectile.velocity *= 1.1f;
                else
                    Projectile.velocity *= .9f;
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(5))
                ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 60, 1, RandRotTwoPi, 0.1f * Main.rand.NextFloat(.8f, 1.01f), 0.6f, false, fullBright: true, blendState: BlendState.AlphaBlend);
            if (Main.rand.NextBool(6) && Projectile.velocity.LengthSquared() > Main.rand.NextFloat(4f * 4f, 10f * 10f))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 40, 1, 0.4f);
            if (Main.rand.NextBool(3))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4f), Projectile.SafeDir().ToRandVelocity(ToRadians(5f), .3f, 1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.25f, false, BlendState.Additive);
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(6), DustID.JungleGrass);
                d.velocity = Projectile.SafeDir();
                d.noGravity = true;
                d.scale = 1f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<GrassKnifeMark>())
                Projectile.AddExecutionTimeImmediate<GrassKnife>();
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
                Color c = Color.Lerp(Color.LimeGreen, Color.White, ratios).ToAddColor(150);
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * scale, se);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.DarkGreen.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);
        }
    }
}
