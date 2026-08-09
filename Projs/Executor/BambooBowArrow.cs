using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class BambooBowArrow : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override Vector2 TileHitbox => new Vector2(4);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            //Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.AffactedByGrav(1f, 1f,.036f);
            if (Projectile.IsOutScreen())
                return;
            if(Main.rand.NextBool(9))
            ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 60, 1, RandRotTwoPi, 0.1f*Main.rand.NextFloat(.8f,1.01f), 0.6f, false, fullBright: true, blendState: BlendState.AlphaBlend);
            if (Projectile.extraUpdates > 1)
            {
                if(Main.rand.NextBool(6))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(6), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 40, 1, 0.34f);
            }
            if(Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(6), DustID.JungleSpore);
                d.velocity = Projectile.SafeDir();
                d.noGravity = true;
                d.scale = 1f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate<BambooBow>();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color c = Color.Lerp(Color.LimeGreen, Color.White, ratios);
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.51f, 1f, ratios);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale*scale, se);
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.DarkGreen.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);
            return false;
        }
    }
}
