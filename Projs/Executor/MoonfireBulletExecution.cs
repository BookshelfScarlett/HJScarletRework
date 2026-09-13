using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MoonfireBulletExecution : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public ref float JustShootTimer => ref Projectile.ai[1];
        public List<NPC> TargetList = [];
        public float HomingFrame = 3f;
        public float BeginUsingChasingFrame = 20f;
        public int OriginalPenetrate = 0;
        public int BounceTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(22);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.penetrate = 15;
            Projectile.SetupImmnuity(-1);
            Projectile.MaxUpdates = 9;
            Projectile.timeLeft = GetSeconds(8) * Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.HJScarlet().ExecutionStrikeManual= true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public override void OnFirstFrame()
        {
            OriginalPenetrate = Projectile.penetrate;
            ScarletSound(HJScarletSounds.ASMD_ExecutionFire, Projectile.Center, .25f, 1, .53f, pitchVariance: .1f);
        }
        public override bool? CanHitNPC(NPC target)
        {
            NPC curTar = Projectile.HJScarlet().CurStoredTarget;
            if (!curTar.IsLegal())
                return null;
            if (curTar.Equals(target))
                return null;
            return false;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3() * 2);
            if (JustShootTimer > Projectile.MaxUpdates * BeginUsingChasingFrame)
            {
                if (Timer > Projectile.MaxUpdates * HomingFrame)
                {
                    ref NPC chaseTarget = ref Projectile.HJScarlet().CurStoredTarget;
                    //追踪处理
                    if (!chaseTarget.IsLegal())
                    {
                        if (Projectile.GetTargetSafe(out NPC target, true, 1000f) && !TargetList.Contains(target))
                        {

                            Projectile.velocity = Projectile.Center.GetNormalVector2(target.Center) * 18f;
                            chaseTarget = target;
                        }
                    }
                    else if (chaseTarget.IsLegal() && !TargetList.Contains(chaseTarget))
                    {
                        Projectile.HomingTarget(chaseTarget.Center, -1, 18f, 0f);
                    }
                    else
                    {
                        //这里其实略微有点危险，相当于告诉玩家现在单位已经打满了
                        //如果可见范围内仍然没有合格的目标，且当前可穿透数量大于1个，我们会手动清除targetList，从而重新搜寻目标
                        if (Projectile.penetrate >= 1)
                        {
                            //reset
                            Projectile.ResetLocalNPCHitImmunity();
                            TargetList.Clear();
                        }
                        chaseTarget = null;
                    }
                }
                else
                {
                    Timer++;
                    if (Timer == Projectile.MaxUpdates * HomingFrame && Projectile.HJScarlet().CurStoredTarget.IsLegal())
                    {
                        Vector2 pos = Projectile.Center;
                        for (int i = 0; i < 12; i++)
                        {
                            ECSParticle.GlowSquare(pos, RandVelTwoPi(.2f, 4.6f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(35, 45), 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 1f, 0, Main.rand.NextFloat(-.1f, 0.1f), 0.9f);
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 4.6f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * .68f, .2f);
                        }
                        float glowScale = .15f;
                        ECSParticle.CrossGlow(pos, Color.DarkGreen, 45, 1, glowScale, .3f);
                        ECSParticle.CrossGlow(pos, Color.LimeGreen, 45, 1, glowScale * .95f, .3f);
                        ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);
                        for (int i = 0; i < 3; i++)
                        {
                            ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(66), Vector2.Zero, RandLerpColor(Color.LimeGreen, Color.Green), 45, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .24f, 2);
                        }
                        Projectile.velocity = Projectile.Center.GetNormalVector2(Projectile.HJScarlet().CurStoredTarget.Center) * 18f;
                    }
                }
                if (!Projectile.IsOutScreen())
                {
                    if (Main.rand.NextBool())
                    {
                        float scale = Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * 1f;
                        Vector2 pos = Projectile.Center.ToRandCirclePos(8);
                        ECSParticle.GlowSquare(pos, Projectile.velocity / 6f, RandLerpColor(Color.LimeGreen, Color.Lime), 45, 1, RandRotTwoPi, scale, 0, Main.rand.NextFloat(-.05f, .05f), 0.9f);
                    }
                    if (Main.rand.NextBool())
                    {
                        float scale = Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .3f;
                        Vector2 pos = Projectile.Center.ToRandCirclePos(8);
                        ECSParticle.ShinyCrossStarECS(pos, Projectile.velocity / 6f, RandLerpColor(Color.LightGreen, Color.LimeGreen), 40, 1, scale, 0.2f);
                    }
                    if (Main.rand.NextBool(3))
                        ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(5), Vector2.Zero, RandLerpColor(Color.LimeGreen, Color.Green), 45, 1, Projectile.rotation + PiOver2, Main.rand.NextFloat(.9f, 1.1f) * .54f, 3);
                }
            }
            else
            {
                JustShootTimer++;
                Projectile.velocity *= .96f;
                if (JustShootTimer == Projectile.MaxUpdates * BeginUsingChasingFrame)
                {
                    ScarletSound(HJScarletSounds.Lightning_QuickHeavy, Projectile.Center, .85f, 1, .93f, pitchVariance: .1f);
                    Projectile.penetrate = OriginalPenetrate;
                    Projectile.timeLeft = GetSeconds(8) * Projectile.MaxUpdates;
                    Vector2 pos = Projectile.Center;
                    Projectile.ResetLocalNPCHitImmunity();
                    for (int i = 0; i < 26; i++)
                    {
                        ECSParticle.GlowSquare(pos, RandVelTwoPi(.2f, 14f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(35, 45), 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 1.2f, 0, Main.rand.NextFloat(-.1f, 0.1f), 0.9f);
                    }
                    for (int i = 0; i < 26; i++)
                    {
                        ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 18f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * .88f, .2f);
                    }
                    float glowScale = .3f;
                    ECSParticle.CrossGlow(pos, Color.DarkGreen, 45, 1, glowScale, .3f);
                    ECSParticle.CrossGlow(pos, Color.LimeGreen, 45, 1, glowScale * .95f, .3f);
                    ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);
                    ECSParticle.Ring(pos, Vector2.Zero, Color.LimeGreen * .8f, 45, 1, 0, .15f, .12f);
                    for (int i = 0; i < 8; i++)
                    {
                        ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(66), Vector2.Zero, RandLerpColor(Color.LimeGreen, Color.Green), 45, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .34f, 2);
                    }
                    if(Projectile.penetrate >8)
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 32f, 32, RandRotTwoPi);
                    else
                        ScreenShakeSystem.AddScreenShakes(Projectile.Center, 16f, 16, RandRotTwoPi);
                    if(Projectile.penetrate > 8)
                    ScreenDarknessSystem.AddScreenDarkness(0.90f, 5, 20, 30, easeOut: EaseInCubic);
                }
            }
        }
        public void HitParticle()
        {
            Vector2 pos = Projectile.Center;
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(pos, RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(35, 45), 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 1f, 0, Main.rand.NextFloat(-.08f, 0.09f), 0.9f);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 8f), RandLerpColor(Color.LimeGreen, Color.Lime), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * .88f, .2f);
            }
            float glowScale = .3f;
            ECSParticle.CrossGlow(pos, Color.DarkGreen, 45, 1, glowScale, .3f);
            ECSParticle.CrossGlow(pos, Color.LimeGreen, 45, 1, glowScale * .95f, .3f);
            ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);

        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate == 0)
                return;
            ScarletSound(HJScarletSounds.Lightning_Strike, Projectile.Center, .75f, 1, .3f, pitchVariance: .1f);
            HitParticle();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScarletSound(HJScarletSounds.Lightning_Strike, Projectile.Center, .75f, 1, .3f, pitchVariance: .1f);
            HitParticle();
            if (JustShootTimer <= Projectile.MaxUpdates * BeginUsingChasingFrame)
                return;
            Timer = 0; 
            if (!TargetList.Contains(target))
                TargetList.Add(target);
            float searchDistance = 1200f * 1200f;
            List<NPC> legalTargetList = [];
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = !tar.Equals(target) && tar.CanBeChasedBy();
                float distPerTar = Vector2.DistanceSquared(tar.Center, Projectile.Center);
                if (legalTar && distPerTar < searchDistance && !TargetList.Contains(tar) && Collision.CanHitLine(tar.Center, tar.width, tar.height, Projectile.Center, Projectile.width, Projectile.height))
                {
                    searchDistance = distPerTar;
                    legalTargetList.Add(tar);
                }
            }
            //穿透数小于4，我们再击杀这个射弹
            if (legalTargetList.Count <= 0)
            {
                if (Projectile.penetrate <= 1)
                    Projectile.timeLeft = 2;
                return;
            }
            Projectile.timeLeft = Projectile.MaxUpdates * GetSeconds(1);
            legalTargetList.Reverse();
            int maxIndex = Math.Min(legalTargetList.Count, 2);
            NPC targetThatShouldChase = legalTargetList[Main.rand.Next(0, maxIndex)];
            Projectile.HJScarlet().CurStoredTarget = targetThatShouldChase;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            DeepGlow.SubmitCustomGlow(() =>
            {
                SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.LimeGreen, 1f,1f,0.78f);
            });
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Noise_HeavyAura.Texture, Color.LimeGreen, 0.25f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.15f, offsetHeight: 1.1f);
            SB.EnterShaderArea();
            //绘制残影
            Projectile.SetCrossStar(1.2f, Projectile.rotation, Color.Green);
            SB.EndShaderArea();
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
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f*offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 8) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 14 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

    }
}
