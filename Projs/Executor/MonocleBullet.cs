using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class MonocleBullet : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 8;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 10;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = GetSeconds(5);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 MountedPos = Projectile.SafeDir() * -15f;
            if (Projectile.IsOutScreen())
                return;
            ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.Violet, 10, 1, 0.35f);
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4) + MountedPos, Projectile.velocity / 8f, RandLerpColor(Color.Violet, Color.White), 40, 1, 0.54f, .2f);
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 pos = Projectile.oldPosition + Projectile.Size / 2;
            float scale = .3f;
            ECSParticle.CrossGlow(pos, Color.Violet, 45, 1, scale, 0.1f);
            ECSParticle.CrossGlow(pos, Color.DarkViolet, 45, 1, scale * .98f, .1f);
            ECSParticle.CrossGlow(pos, Color.White, 45, 1, scale * .96f, .1f);
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePos(6), 1.1f, RandLerpColor(Color.Violet, Color.Purple), 45, 1, 0.14f, glowMult: .7f);
            }

            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float rat = Utils.GetLerpValue(0, 10, Projectile.numHits, true);
            modifiers.SourceDamage *= Lerp(1f, 2.5f, rat);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<Monocle>());
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 11.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.Violet, Color.Purple), timeLeft, 1, scale);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(3);
                Vector2 vel = Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15), .1f, 19.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.38f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.LightntingGlow(pos2, vel, RandLerpColor(Color.Purple, Color.Violet), timeLeft, 1, scale);
            }
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.HighResolutionThunder(pos.ToRandCirclePos(3), Projectile.SafeDirByRot().ToRandVelocity(ToRadians(5), .1f, .2f), RandLerpColor(Color.Violet, Color.Purple), 45, 1, Projectile.SafeDirByRot().ToRotation(), 0.12f, 1);
            }
            ScarletSound(HJScarletSounds.Atom_StrikeAlt, Projectile.Center, 0.85f, 1, pitch: Projectile.numHits * .1f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            SB.EnterShaderArea();
            //绘制残影
            float oriScale = 1f;
            float scale = 0.97f;
            int length = 19;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.Purple, Color.Violet, (1 - rads)).ToAddColor(255) * Clamp(Projectile.velocity.Length(), 0f, 1f);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(tex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SB.Draw(tex, pos, null, Color.Violet, Projectile.rotation + PiOver2, ori, oriScale, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
