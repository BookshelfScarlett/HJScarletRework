using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using ReLogic.Content;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class BlazingSunFireball : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public AnimationStruct Helper = new AnimationStruct(3);
        public ref float Timer => ref Projectile.ai[0];
        public int Direction
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(20);
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 20;
            base.OnFirstFrame();
        }
        public static int BeamLength = 1200;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer < ShootTimer)
                return false;
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.Zero) * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 24f, ref _);
        }
        public int ShootTimer = 5;
        public override void ProjAI()
        {
            if (Owner.HeldItem.type != ItemType<BlazingSun>() && Projectile.Opacity <= .8f)
            {
                Projectile.Kill();
                return;
            }
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                float ratios = EaseOutBack(Helper.GetAniProgress(0));
                Vector2 targetPos = Owner.MountedCenter - Vector2.UnitY * Direction * 50f * ratios;
                //Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, .2f);
                Projectile.Center = targetPos;
            }
            else
            {
                if (Owner.channel)
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, .2f);
                    Vector2 targetPos = Owner.MountedCenter - Vector2.UnitY.RotatedBy(Projectile.rotation) * Direction * 50f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, .4f);
                    Projectile.rotation = Projectile.Center.GetNormalVector2(Main.MouseWorld).ToRotation();

                    if (Projectile.Opacity >= 1f)
                    {
                        if (Timer == 0)
                        {
                            ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, 0.84f, 1, Projectile.localAI[1] * .03f);
                            InitThisBeam();
                        }
                        Timer++;
                        if (Timer > ShootTimer)
                            Timer = ShootTimer;
                        if (Main.rand.NextBool(7))
                            ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePos(5), -Vector2.UnitY * 0.94f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, 1, 0.24f, 0);
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 dir = Projectile.SafeDirByRot() * Main.rand.Next(0, BeamLength);
                            Vector2 pos = Projectile.Center + dir;
                            if (HJScarletMethods.OutOffScreen(pos))
                                continue;

                            //ECSParticle.HRShinyOrb(pos, dir.ToRandVelocity(ToRadians(15f), 1f, 9f), Color.Orange, 40, 1, 0.14f, 0.5f);
                            ECSParticle.TurbulenceShinyOrb(pos, 0.42f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, 1, 0.12f, glowMult: .5f);
                        }
                    }
                }
                else
                {
                    Timer -= 2f;
                    if (Timer <= 0)
                        Timer = 0;
                    //Vector2 targetPos = Owner.MountedCenter - Vector2.UnitY * Direction * 50f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.MountedCenter, .14f);
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.3f);
                    if (Projectile.Opacity <= 0.12f)
                        Projectile.Opacity = 0;
                }
            }
        }
        public void InitThisBeam()
        {
            float centerGlowScale = .362f;
            ECSParticle.CrossGlow(Projectile.Center, Color.DarkOrange, 45, 1, centerGlowScale);
            ECSParticle.CrossGlow(Projectile.Center, Color.OrangeRed, 45, 1, centerGlowScale * .98f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 45, 1, centerGlowScale * .96f);

            for (int i = 0; i < 6; i++)
            {
                Color color = RandLerpColor(Color.OrangeRed, Color.Orange);
                new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .1f + i * 0.06f, Projectile.whoAmI, Vector2.Zero, true).Spawn();
            }
            for (int i = 0; i < 20; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(18), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.Orange, Color.OrangeRed), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .1f);
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(6), Projectile.SafeDirByRot().ToRandVelocity(ToRadians(15f), 1.2f, 12.8f), RandLerpColor(Color.Orange, Color.OrangeRed), 45, RandRotTwoPi, 1, 0.2f, blendstate: BlendState.Additive);
            }
            for (int i = 0; i < 320; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot() * Main.rand.Next(0, BeamLength);
                Vector2 pos = Projectile.Center + dir;
                if (HJScarletMethods.OutOffScreen(pos))
                    continue;
                ECSParticle.TurbulenceShinyOrb(pos, 0.42f, RandLerpColor(Color.Orange, Color.OrangeRed), 40, 1, 0.12f, glowMult: .5f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            HJScarletMethods.EndShaderAreaPixel();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D orb = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            SB.EnterShaderArea();
            Vector2 glowSize = new Vector2(1f, 1f) * Projectile.scale * .45f;
            SB.FastDraw(orb, center, Color.Orange * Projectile.Opacity, Projectile.rotation, orb.Size() / 2f, glowSize, 0);
            SB.FastDraw(orb, center, Color.White * .85f * Projectile.Opacity, Projectile.rotation, orb.Size() / 2f, glowSize * .75f, 0);
            orb = HJScarletTexture.Particle_SharpTear;
            glowSize = new Vector2(1f, 1f) * Projectile.scale * .947f * (Timer / (float)ShootTimer);
            SB.FastDraw(orb, center, Color.Orange, Projectile.rotation, orb.Size() / 2f, glowSize, 0);
            SB.FastDraw(orb, center, Color.Orange, Projectile.rotation + PiOver2, orb.Size() / 2f, glowSize, 0);
            SB.FastDraw(orb, center, Color.White, Projectile.rotation, orb.Size() / 2f, glowSize * .85f, 0);
            SB.FastDraw(orb, center, Color.White, Projectile.rotation + PiOver2, orb.Size() / 2f, glowSize * .85f, 0);
            DrawBeam(SB, Color.Lerp(Color.Orange, Color.Red, 0.5f), 0.12f * Projectile.scale);
            DrawBeam(SB, Color.Lerp(Color.OrangeRed, Color.Orange, 0.55f), 0.10f * Projectile.scale);
            DrawBeam(SB, Color.Lerp(Color.Orange, Color.White, 0.62f), 0.08f * Projectile.scale);
            DrawBeam(SB, Color.White, 0.05f * Projectile.scale);
            SB.EndShaderArea();

            return false;
        }
        public void DrawBeam(SpriteBatch sb, Color color, float height)
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_ManaStreak.Texture;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -70);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            sb.Draw(value.Value, Projectile.Center - Main.screenPosition - Projectile.SafeDirByRot() * 5f, null, Color.White, Projectile.rotation, orig, new Vector2(xScale * Clamp(Timer / (float)(ShootTimer), 0f, 1f), height * 0.59f), 0, 0);
        }

    }
}
