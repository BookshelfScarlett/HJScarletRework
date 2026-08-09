using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;

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
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LightGray, Color.White), 40, 1, 0.34f, .031f);
            if (Main.rand.NextBool(6))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8, RandLerpColor(Color.LightGray, Color.White), 40, 1, 0.42f);
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
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                Color c = Color.Lerp(Color.LightGray, Color.White, ratios);
                Vector2 sharpScale = new Vector2(1f, 1.4f);
                SB.Draw(HJScarletTexture.Particle_SharpTear, pos - new Vector2(25,0).RotatedBy(Projectile.rotation), null, c.ToAddColor(10)*opa*.35f, Projectile.oldRot[i] + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale * scale, 0, 0);
                c = Color.Lerp(Color.LightGray, Color.White, ratios).ToAddColor(250);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * scale, se);
                
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 2f, Color.White.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);
        }
    }
}
