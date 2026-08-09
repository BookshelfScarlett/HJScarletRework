using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FishronKnifeProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<FishronKnife>().Texture;
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
                new ShinyRing(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir() + RandVelTwoPi(0.2f, .4f), RandLerpColor(Color.LightSkyBlue, Color.DeepSkyBlue), 60, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .023f, RandRotTwoPi, 0.1f, 0.9f).Spawn();
            if (Main.rand.NextBool(6))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.34f, .031f);
            if (Main.rand.NextBool(6))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 8, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1, 0.42f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<FishronKnifeMark>())
                Projectile.AddExecutionTimeImmediate<FishronKnife>();
            for(int i =0;i<3;i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.ToRandVelocity(ToRadians(15f), 16f), ProjectileType<FishronKnifeBubble>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), Projectile.SafeDir().ToRandVelocity(ToRadians(20f), 0.1f, 12f), RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 40, 1, 0.4f, 0.2f);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float scale = Lerp(.264f, 1f, ratios);
                float opa = Lerp(.31f, 1f, ratios);
                Color c = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, ratios);
                Vector2 sharpScale = new Vector2(0.83f, 1.4f);
                SB.Draw(HJScarletTexture.Particle_SharpTear, pos - new Vector2(25,-3).RotatedBy(Projectile.rotation), null, c.ToAddColor(10)*opa, Projectile.oldRot[i] + PiOver2, HJScarletTexture.Particle_SharpTear.Size() / 2f, sharpScale * scale, 0, 0);
                c = Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, ratios).ToAddColor(250);
                SB.FastDraw(tex, pos, c * opa, Projectile.oldRot[i] + PiOver4, tex.Size() / 2f, Projectile.scale * scale, se);
                
            }
            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 2f, Color.White.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);

            return false;
        }

    }
}
