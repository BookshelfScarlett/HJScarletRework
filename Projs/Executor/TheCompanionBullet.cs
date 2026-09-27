using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    internal class TheCompanionBullet : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(22);
        }

        public override void ExSD()
        {
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 2;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300 * 3;
            Projectile.SetupImmnuity(30);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
                for (int i = 0; i < 3; i++)
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(6) + Projectile.velocity / i, 0.62f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .1f, glowMult: .25f);
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate == 0)
                return;
            ECSParticle.ShinyCrossStarSmall(Projectile.Center, Projectile.SafeDir() * .1f, Color.LightGoldenrodYellow, 40, 1, 1f, 0);
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(12), 0.62f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .1f, glowMult: .25f);

            }
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                modifiers.SetCrit();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && Projectile.HJScarlet().ExecutionStrike && Projectile.numHits < 1)
            {

                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = Projectile.oldVelocity.ToSafeNormalize() * Main.rand.NextFloat(0f, 10) * Main.rand.NextBool().ToDirectionInt();
                    Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f);
                    new SmokeParticle(spawnpos, vel, RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                    if (Main.rand.NextBool())

                        new SmokeParticle(spawnpos, Projectile.oldVelocity.ToSafeNormalize().RotatedByRandom(Pi) * Main.rand.NextFloat(0.2f, 8f), RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.75f), Color.OrangeRed), 40, RandRotTwoPi, 1f, 0.30f * Main.rand.NextFloat(0.75f, 1.1f), true).SpawnToPriority();
                }
                for (int j = 0; j < 30; j++)
                {
                    Vector2 dir = -Projectile.SafeDirByRot();
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePos(20f) + dir * Main.rand.NextFloat(0f, 12f), dir * 12f * Main.rand.NextFloat(), RandLerpColor(Color.Orange, Color.OrangeRed), 50, RandRotTwoPi, 1, 0.7f, false).Spawn();
                }
                for (int i = 0; i < 16; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                    Vector2 vel = RandVelTwoPi(1f, 7.9f);
                    new HRShinyOrb(pos, vel, RandLerpColor((Color.Lerp(Color.Red, Color.Orange, 0.5f)), Color.OrangeRed), 40, 0.12f).Spawn();
                    new HRShinyOrb(pos, vel, Color.White, 40, 0.12f * 0.5f).Spawn();
                }
                ScarletSound(HJScarletSounds.Misc_GunHit, Projectile.Center, volume: .65f, pitch: .3f);
                int count = 242;
                float seartchDist = 400;
                for (int i = 0; i < count; i++)
                {
                    float args = TwoPi / (float)count * i;
                    Vector2 argsVec = args.ToRotationVector2();
                    Vector2 startPos = Projectile.Center + argsVec * seartchDist * .9f;
                    ECSParticle.ShinyCrossStarECS(startPos, argsVec, Color.Orange, 45, 1, .6f, .2f);
                }

                //int count = 66;
                //float seartchDist = 400;
                //for(int i =0;i<count;i++)
                //{
                //    float args = TwoPi / count;
                //    Vector2 argsVec = args.ToRotationVector2();
                //    Vector2 startPos = Projectile.Center + argsVec * seartchDist * .9f;
                //    Dust d = Dust.NewDustPerfect(startPos, DustID.Torch);
                //    d.velocity = argsVec * 2f;
                //    d.noGravity = true;
                //}
                target.AddBuff(BuffID.OnFire3, GetSeconds(2));
                HashSet<NPC> legalTargetList = [];
                count = 0;
                foreach (var tar in Main.ActiveNPCs)
                {
                    if (count >= 18)
                    {
                        break;
                    }
                    bool legalTar = tar.CanBeChasedBy() && tar.type != NPCID.TargetDummy;
                    float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                    //别穿墙搜
                    if (legalTar && distPerTar < seartchDist && !legalTargetList.Contains(tar))
                    {
                        legalTargetList.Add(tar);
                        tar.AddBuff(BuffID.OnFire3, GetSeconds(3));
                        count++;
                    }
                }
            }
            else
                target.AddBuff(BuffID.Oiled, GetSeconds(2));
            ECSParticle.ShinyCrossStarSmall(Projectile.Center, Projectile.SafeDir() * .1f, Color.LightGoldenrodYellow, 40, 1, 1f, 0);
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(12), 0.62f, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .1f, glowMult: .25f);

            }

            Projectile.AddExecutionTimeImmediate(ItemType<TheCompanion>());

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                ////这里是强行使用ex98拼凑出来的子弹效果
                Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
                Rectangle frame = tex.Frame();
                Vector2 ori = tex.Size() / 2;

                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkOrange, 1f, 1f, 0.78f);
                SB.EnterShaderArea();
                DrawTrails(HJScarletTexture.Noise_HeavyAura.Texture, Color.OrangeRed, 0.25f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.15f, offsetHeight: 1.1f);
                SB.EnterShaderArea();
                //绘制残影
                Texture2D orb = HJScarletTexture.Texture_BloodStain.Value;
                Vector2 orbScale = new Vector2(1f, 1.25f) * .5f * Projectile.scale;
                float rot = Projectile.rotation;
                SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.DarkOrange, rot, orb.Size() / 2f, orbScale, 0, 0);
                SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.White, rot, orb.Size() / 2f, orbScale * .75f, 0, 0);
                SB.EndShaderArea();
            }
            else
            {
                ////这里是强行使用ex98拼凑出来的子弹效果
                Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
                Rectangle frame = tex.Frame();
                Vector2 ori = tex.Size() / 2;

                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.DarkGoldenrod, 1f, 1f, 0.78f);
                SB.EnterShaderArea();
                DrawTrails(HJScarletTexture.Noise_HeavyAura.Texture, Color.Goldenrod, 0.25f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.15f, offsetHeight: 1.1f);
                SB.EnterShaderArea();
                //绘制残影
                Texture2D orb = HJScarletTexture.Texture_BloodStain.Value;
                Vector2 orbScale = new Vector2(1f, 1.25f) * .5f * Projectile.scale;
                float rot = Projectile.rotation;
                SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.DarkGoldenrod, rot, orb.Size() / 2f, orbScale, 0, 0);
                SB.FastDraw(orb, Projectile.Center - Main.screenPosition, Color.White, rot, orb.Size() / 2f, orbScale * .75f, 0, 0);
                SB.EndShaderArea();
            }
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)

        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 270f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 10) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 10 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

    }
}
