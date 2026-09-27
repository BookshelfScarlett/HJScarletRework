using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class RitualofReposeRest : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => GetInstance<RitualofReposeProj>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public bool VisualKilled
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1 : 0;
        }
        public Vector2 TargetPos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            //ScarletSound(HJScarletSounds.Misc_MagicStaffFire, Owner.Center, .2f, 1, pitch: .4f);

        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.type != NPCID.CultistBossClone && target.type != NPCID.TargetDummy)
                return null;

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return (Timer >= (MaxLandTime) * Projectile.MaxUpdates);
        }
        public float Osci = 0;
        public float MaxLandTime = 40f;
        public override void ProjAI()
        {
            //这里还会再卡住玩家一点时间，不让其投掷安息仪式
            Timer++;
            if (VisualKilled)
            {
                if (Timer < GetSeconds(1))
                {
                }
                else
                {
                    Projectile.Kill();
                }
                return;
            }
            Owner.ControlPlayerArm((Projectile.Center - Owner.Center).ToRotation(), 1);
            Owner.ChangeDir((Projectile.Center.X - Owner.MountedCenter.X > 0).ToDirectionInt());
            float ratios = Clamp((Timer / MaxLandTime * Projectile.MaxUpdates), 0f, 1f);
            Osci -= ToRadians(2f);
            float yMult = (150f * MathF.Sin(Osci) / 9f) + 0f;
            float xPos = TargetPos.X;
            float yPos = TargetPos.Y - yMult;
            Vector2 mountedPos = new(xPos, yPos);

            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, EaseInOutExpo(ratios));
            Projectile.rotation = (-Vector2.UnitY).ToRotation();
            //粒子。
            //安息仪式的主落地射弹
            if (ratios >= 1)
            {
                if (Timer >= (MaxLandTime + 1) * Projectile.MaxUpdates)
                {
                    new ThunderboltParticle(Projectile.Center, 0, 1.25f, Color.Gold, 40, 15f, .75f, new Vector2(0.5f, 1.5f)).Spawn();
                    new ThunderboltParticle(Projectile.Center, 0, 1.15f, Color.LightGoldenrodYellow, 40, 15f, .75f, new Vector2(.5f, 1.5f)).Spawn();
                    ScarletSound(HJScarletSounds.Lightning_QuickHeavy, Projectile.Center, .65f, 1, .3f, pitchVariance: .1f);
                    Projectile.timeLeft = GetSeconds(8) * Projectile.MaxUpdates;
                    Vector2 pos = Projectile.Center;
                    //弹幕
                    for (int i = 0; i < 16; i++)
                    {
                    }
                    for (int i = 0; i < 36; i++)
                    {
                        ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePosEdge(160), 6.5f, RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), Main.rand.Next(60, 70), 1, 0.22f, TwoPi / 36f * i, glowMult: .46f);
                    }
                    for (int i = 0; i < 36; i++)
                    {
                        ECSParticle.ShinyCrossStarSmall(pos.ToRandCirclePos(5), RandVelTwoPi(.2f, 28f), RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(.9f, 1.1f) * 1.18f, 0);
                    }
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 vel = RandVelTwoPi(8f, 21f);
                        ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(6) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 30f, vel, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), 45, 1, vel.ToRotation(), Main.rand.NextFloat(.9f, 1.1f) * .2f, 1);
                    }
                    float glowScale = .63f;
                    ECSParticle.CrossGlow(pos, Color.Gold, 45, 1, glowScale, .3f);
                    ECSParticle.CrossGlow(pos, Color.LightGoldenrodYellow, 45, 1, glowScale * .95f, .3f);
                    ECSParticle.CrossGlow(pos, Color.White, 45, 1, glowScale * .90f, .3f);
                    ECSParticle.Ring(pos, Vector2.Zero, Color.DarkGoldenrod * .8f, 45, 1, 0, .15f, .12f);
                    for (int i = 0; i < 20; i++)
                    {
                        ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(300), Vector2.Zero, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkGoldenrod), 45, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .34f, 2);
                    }
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 32f, 32, RandRotTwoPi);
                    ScreenDarknessSystem.AddScreenDarkness(0.95f, 2, 10, 25, easeOut: EaseInCubic);
                    HashSet<NPC> legalTargetList = [];
                    float searchDistance = 1800f;
                    int count = 0;
                    foreach (var tar in Main.ActiveNPCs)
                    {
                        if (count >= 18)
                        {
                            break;
                        }
                        bool legalTar = tar.CanBeChasedBy() && tar.type != NPCID.TargetDummy;
                        float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                        //别穿墙搜
                        if (legalTar && distPerTar < searchDistance && !legalTargetList.Contains(tar))
                        {
                            legalTargetList.Add(tar);
                            count++;
                            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), tar.Center, Vector2.Zero, ProjectileType<RitualofReposeThunder>(), Projectile.damage, 1, Owner.whoAmI);
                            proj.HJScarlet().CurStoredTarget = tar;
                        }
                    }
                    Timer = 0;
                    VisualKilled = true;
                }
            }
            else
            {
                if (Projectile.IsOutScreen())
                    return;
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(28), Projectile.rotation.ToRotationVector2(), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .42f, 0);
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.rotation.ToRotationVector2(), RandLerpColor(Color.Gold, Color.LightGoldenrodYellow), 45, 1, Projectile.scale * Main.rand.NextFloat(.7f, 1.1f) * .6f);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffType<TheBleachingBuff>(), GetSeconds(2));
        }
        public BlendState BlendState => BlendState.Additive;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (VisualKilled)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int length = Projectile.oldPos.Length;
            Texture2D trailTex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Rectangle curSrc = trailTex.Bounds;
            curSrc.Width = (int)(curSrc.Width * 0.8f);
            Vector2 cutSrcOri = new Vector2(curSrc.Width / 2f, curSrc.Height / 2f);
            Vector2 sharpScale = new Vector2(0.7f, 0.60f);
            //float glowSpreadMult = Clamp(Math.Abs(MathF.Sin((float)Main.timeForVisualEffects / 100f)), 0.35f, 1f);
            float glowSpreadMult = 1;
            float generalGlow = 1;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1f - i / (float)length;
                Vector2 pos = Projectile.Center - Main.screenPosition;
                Color trailColor = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, ratios).ToAddColor(100);
                float scale = Lerp(.44f, 1f, ratios);
                float opacity = Lerp(.51f, .7f, ratios);
                Vector2 sharpPos = pos - new Vector2(15, 0).RotatedBy(Projectile.rotation);
                float trailRot = Projectile.rotation;
                SB.Draw(trailTex, sharpPos, curSrc, trailColor * opacity * glowSpreadMult * generalGlow, trailRot, cutSrcOri, sharpScale * scale, 0, 0);
            }
            //十字光晕
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 glowPos = drawPos + Projectile.rotation.ToRotationVector2() * 25f * Projectile.scale;
            Vector2 glowScale = Projectile.scale * .48f * new Vector2(.75f, 1.35f);

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .65f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .35f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .45f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale * .95f, 0);

            //环状光晕
            glow = HJScarletTexture.Particle_RingShiny.Value;
            glowScale = Projectile.scale * .16f * Vector2.One;

            SB.FastDraw(glow, glowPos, Color.DarkGoldenrod * .45f * generalGlow, Projectile.rotation, glow.Size() / 2f, glowScale, 0);
            SB.FastDraw(glow, glowPos, Color.Gold * .4f * generalGlow, Projectile.rotation + Pi, glow.Size() / 2f, glowScale * .98f, 0);
            SB.FastDraw(glow, glowPos, Color.White * .5f * generalGlow, Projectile.rotation + PiOver2, glow.Size() / 2f, glowScale * .95f, 0);
            //最后再画轨迹
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame || VisualKilled)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);

            float rotFixer = PiOver4;
            float spearRotation = Projectile.rotation + rotFixer;
            SB.FastDraw(projTex, drawPos, Color.White, spearRotation, ori, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
