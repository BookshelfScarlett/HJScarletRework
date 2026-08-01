using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.Design.Serialization;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ArcticGuanDaoCloudMoving : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.RainCloudMoving);
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public Vector2 TargetVector2 = Vector2.Zero;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            Projectile.AddFrames(5, 4);
            int maxTime = 30;
            float pro = EaseInOutExpo(Utils.GetLerpValue(0, maxTime, Timer, true));
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetVector2, 0.1f);
            Vector2 normalVec = Projectile.Center.GetNormalVector2(TargetVector2);
            ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(24,12), -normalVec * Main.rand.NextFloat(0.3f, 1.2f), RandLerpColor(Color.White, Color.SkyBlue), 40, 1, Projectile.scale * 0.3f, 0.2f);
            if (Main.rand.NextBool())
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(24,12), -normalVec * Main.rand.NextFloat(0.3f, 1.2f), RandLerpColor(Color.White, Color.SkyBlue), 40, RandRotTwoPi, 1, Projectile.scale * 0.13f, blendstate: BlendState.AlphaBlend);
            Timer++;
            if (pro == 1)
            {
                Projectile.Kill();
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<ArcticGuanDaoCloud>(), 0, 0, Owner.whoAmI);
            proj.originalDamage = Projectile.originalDamage;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, 4, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .42f) + Projectile.PosToCenter();
                float ratios = 1 - i / (float)length;
                Color c = Color.Lerp(Color.SkyBlue, Color.White.ToAddColor(100), ratios);
                SB.Draw(tex, oldPos, frame, c.ToAddColor(50) * ratios, 0, ori, Projectile.scale, 0, 0);
            }
            SB.Draw(tex, pos, frame, Color.White.ToAddColor(150), 0, ori, Projectile.scale, 0, 0);
            return false;
        }

    }
}
