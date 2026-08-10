using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ArcticGuanDaoCloud : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.RainCloudRaining);
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(2);
        }
        public override void ExSD()
        {
            Projectile.timeLeft = GetSeconds(30);
            Projectile.width = 75;
            Projectile.height = 40;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public ref float Timer => ref Projectile.ai[0];
        public override void ProjAI()
        {
            Projectile.AddFrames(5, 6);
            Projectile.MinionAntiClump();
            Projectile.velocity *= 0.95f;
            Vector2 normalVec = -Vector2.UnitY;
            if (!Projectile.IsOutScreen())
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(24, 12), -normalVec * Main.rand.NextFloat(0.3f, 4.2f), RandLerpColor(Color.White, Color.SkyBlue), 40, 1, Projectile.scale * 0.3f, 0.2f);
                if (Main.rand.NextBool())
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(24, 12), -normalVec * Main.rand.NextFloat(0.3f, 4.2f), RandLerpColor(Color.White, Color.SkyBlue), 40, RandRotTwoPi, 1, Projectile.scale * 0.13f, blendstate: BlendState.AlphaBlend);
            }
            if (!Projectile.IsMe())
                return;
            Timer++;
            float dropTime = HJScarletMethods.HasFuckingCalamity ? 15f : 15f;
            if (Timer <= dropTime)
                return;
            Timer = 0;
            Vector2 dir = Vector2.UnitY;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(24, 12) + dir * 10f, dir * Main.rand.NextFloat(9f, 10f), ProjectileType<ArcticGuanDaoSpike>(), Projectile.originalDamage, 2f, Owner.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? CanDamage() => false;
        public float LerpValue = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, 6, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length;
            LerpValue = Lerp(LerpValue, 1f, 0.1f);
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .42f) + Projectile.PosToCenter();
                float ratios = 1 - i / (float)length;
                Color c = Color.Lerp(Color.SkyBlue, Color.White.ToAddColor(100), ratios);
                SB.Draw(tex, oldPos, frame, c.ToAddColor(50) * ratios, 0, ori, Projectile.scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f * LerpValue, frame, Color.White.ToAddColor(0), 0, ori, Projectile.scale, 0, 0);
            SB.Draw(tex, pos, frame, Color.White.ToAddColor(150), 0, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
