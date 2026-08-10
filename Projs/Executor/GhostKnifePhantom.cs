using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class GhostKnifePhantom : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<GhostKnife>().Texture;
        public ref float Timer => ref Projectile.ai[0];
        public float CurRatios = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(14);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(-1);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 6;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 180;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            CurRatios = Clamp(Timer / (Projectile.MaxUpdates * 10), 0, 1);
            Projectile.velocity *= .96f;
            Vector2 offset = Projectile.SafeDir() * 30f;
            if (CurRatios >= 1f)
            {
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center - Projectile.SafeDir() * i * 5f, Projectile.velocity / 16f, RandLerpColor(Color.White, Color.SkyBlue), 45, RandRotTwoPi, 1, 0.1f, false, BlendState.AlphaBlend);
                }
                Projectile.Kill();
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(6f) - offset, Projectile.velocity / 16f, RandLerpColor(Color.White, Color.LightSkyBlue), 40, 1, 0.34f, 4);
            //ECSParticle.StarShape(Projectile.Center.ToRandCirclePos(6) - offset, Projectile.velocity / 16f, RandLerpColor(Color.White, Color.LightSkyBlue), 40, 1, 0.65f * (1-CurRatios));
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            drawPos -= Projectile.SafeDir() * 22f;
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            float disapperRatios = (1 - CurRatios);
            int length = (int)((Projectile.oldPos.Length - 4) * (disapperRatios));
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = EaseInOutExpo(1 - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter() - Projectile.SafeDir() * 22f;
                float rot = Projectile.oldRot[i];
                int addLerp = (int)(Lerp(0, 105, ratios));
                Color c = Color.Lerp(Color.White, Color.WhiteSmoke, ratios).ToAddColor((byte)addLerp) * (Lerp(0.14f, 1f, ratios));
                float scale = Projectile.scale * Lerp(.35f, 1f, ratios);
                SB.Draw(projTex, pos, null, c, rot + PiOver4, orig, scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + (TwoPi / 8 * i).ToRotationVector2() * 1.5f, null, Color.WhiteSmoke.ToAddColor(), Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            Color mainC = Color.Lerp(Color.White, Color.WhiteSmoke, disapperRatios);
            SB.Draw(projTex, drawPos, null, mainC, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);

            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }

    }
}
