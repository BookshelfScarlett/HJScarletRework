using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using Microsoft.Extensions.Logging.Abstractions;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheSevenStarDipperStar : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.Particle_GlowStar.Path;
        public enum State
        {
            Following,
            Lanuching
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int ParentProjIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }
        public bool ShoulGlowUp
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }
        public bool ShouldKill = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0;
            Projectile.scale = 0;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public void UpdateGlowUp()
        {
            if (Projectile.Opacity == 0)
            {
                ScarletSound(HJScarletSounds.Moonlight_Ding, Projectile.Center, 0.4f, 1, Projectile.localAI[1] * .03f);
                for (int i = 0; i < 3; i++)
                {
                    Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .05f + i * 0.032f, Projectile.whoAmI, Vector2.Zero, true).Spawn();
                }
                for (int i = 0; i < 20; i++)
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(4), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
            }
            Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.12f);
            Projectile.scale = Projectile.Opacity;
            if (Main.rand.NextBool(17))
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(4), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
        }
        public override void ProjAI()
        {
            if (!ShouldKill)
            {
                if (ShoulGlowUp)
                {
                    UpdateGlowUp();
                }
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.12f);
                Projectile.scale = Lerp(Projectile.scale, 0f, 0.12f);
            }

            if (AttackState == State.Following)
            {
                if (Owner.GetExecutionSrike() && Owner.IsHolding<TheSevenStar>())
                {
                    AttackState = State.Lanuching;
                }
                Projectile proj = Main.projectile[ParentProjIndex];
                //这里只管存货，实际的控制由父射弹管理
                if ((proj != null && proj.active && proj.type == ProjectileType<TheSevenStarDipper>()) && Owner.HeldItem.type == ItemType<TheSevenStar>())
                {
                    Projectile.timeLeft = 30;
                }
                else if (!ShouldKill)
                {
                    if (ShoulGlowUp)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                            new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .05f + i * 0.032f, Projectile.whoAmI, Vector2.Zero, true).Spawn();
                        }
                        for (int i = 0; i < 10; i++)
                            ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(4), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
                    }
                    ShouldKill = true;
                    Projectile.netUpdate = true;
                }
            }
            else if (AttackState == State.Lanuching)
            {
                Vector2 vec = Projectile.Center.GetNormalVector2(Main.MouseWorld);
                int damage = (int)Owner.GetTotalDamage<ExecutorDamageClass>().ApplyTo(34);
                Projectile bolt = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + vec.ToSafeNormalize() * 10f, vec * 17f, ProjectileType<TheSevenStarBolt>(), damage, 2f, Owner.whoAmI);
                float centerGlowScale = .12f;
                ECSParticle.CrossGlow(Projectile.Center, Color.SkyBlue, 45, 1, centerGlowScale);
                ECSParticle.CrossGlow(Projectile.Center, Color.LightSkyBlue, 45, 1, centerGlowScale * .98f);
                ECSParticle.CrossGlow(Projectile.Center, Color.White, 45, 1, centerGlowScale * .96f);

                for (int i = 0; i < 3; i++)
                {
                    Color color = RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue);
                    new NoiseShockRing(Projectile.Center, Vector2.Zero, color, 45, 1f, .05f + i * 0.032f, Projectile.whoAmI, Vector2.Zero, true).Spawn();
                }
                for (int i = 0; i < 10; i++)
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePosEdge(4), Main.rand.NextFloat(1.2f, 2.4f) * .24f, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .043f);
                AttackState = (State)(-1);
            }
            else
            {
                ShouldKill = true;
            }
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //实际开始绘制的星星。
            Texture2D tex = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = Color.Lerp(Color.LightSkyBlue, Color.SkyBlue, Projectile.localAI[1] / 6f);
            float generalScale = 1.32f * Projectile.Opacity;
            Vector2 scale = new Vector2(1.72f, 1.72f) * .024f * generalScale;
            Vector2 orig = tex.Size() / 2;
            SB.EnterShaderArea();
            for (int i = 0; i < 2; i++)
                SB.Draw(tex, pos, null, c*Projectile.Opacity, PiOver2 * i, orig, scale, 0, 0);
            Texture2D orb = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            SB.Draw(orb, pos, null, Color.White * .4f, 0, orb.Size() / 2f, .125f * generalScale, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
