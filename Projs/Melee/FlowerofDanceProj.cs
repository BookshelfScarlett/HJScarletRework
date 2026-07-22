using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FlowerofDanceProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => GetInstance<FlowerofDance>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(14);
        }
        public Vector2 DrawOffset = Vector2.Zero;
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        { 
            Shoot,
            AngleTo,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new(2);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 1000;
        }
        public NPC CurTarget = null;
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.AngleTo:
                    DoAngleTo();
                    break;
                case State.Homing:
                    DoHoming();
                    break;
            }

        }

        public void DoShoot()
        {

            float ratios = Timer / (Projectile.MaxUpdates * 25);
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1000, canPassWall: false) && ratios < .15f)
            {
                CurTarget = target;
            }
            if (ratios > .15f)
            {
                if (CurTarget.IsLegal())
                {
                    Vector2 vec = Projectile.Center.GetNormalVector2(CurTarget.Center);
                    Projectile.rotation += Lerp(0.20f, 0.01f, EaseInOutQuad((1 - ratios) / (1 - .15f))) * (Projectile.velocity.X > 0).ToDirectionInt();
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

            }
                Projectile.velocity *= 0.95f;
            Timer++;
            if (ratios >= .99f)
            {
                AttackState = State.AngleTo;
                Timer = 0;
                Projectile.netUpdate = true;
            }
            if (Projectile.IsOutScreen())
                return;
            if (ratios > Main.rand.NextFloat())
                return;
            if (Main.rand.NextBool(3))
                return;
            if (Main.rand.NextBool(9))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity / 4f, RandLerpColor(Color.Aquamarine, Color.SkyBlue), 40, 1f, 0.48f * (1- ratios));
            if (Main.rand.NextBool(10))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity / 4f, RandLerpColor(Color.Aquamarine, Color.SkyBlue), 40, 1f, 0.48f);
            if (Main.rand.NextBool(21))
                ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePos(6),-Vector2.UnitY.RotateRandom(PiOver4) * 10f,RandLerpColor(Color.SkyBlue,Color.Aquamarine),50,1,RandRotTwoPi,0.125f*Main.rand.NextFloat(.85f,1.1f),1.92f,true,fullBright:true,blendState:BlendState.AlphaBlend);
        }
        public void DoAngleTo()
        {
            if (CurTarget.IsLegal())
            {
                if (Projectile.ai[1] == 1 && Timer == 0)
                {
                    SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 1, Volume = .7f, Pitch = .5f }, Projectile.Center);
                    //for (int i = 0; i < 22; i++)
                }
                Vector2 vec = Projectile.Center.GetNormalVector2(CurTarget.Center);
                if(Main.rand.NextBool(20))
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16, 32), Main.rand.NextFloat(1.2f, 3.4f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 45, 1f, Main.rand.NextFloat(.85f, 1.15f) * 0.181f, glowMult: 0.45f);
                Timer++;
                float ratios = Timer / (Projectile.MaxUpdates * 15);
                Projectile.rotation = Projectile.rotation.AngleTowards(vec.ToRotation(), (1f));
                if (ratios > 0.5f)
                {
                    Projectile.scale = Lerp(1.21f, 1f, EaseInOutQuad((1 - ratios) / 0.5f));
                }
                else
                    Projectile.scale = Lerp(1f, 1.21f, EaseInOutQuad(ratios / 0.5f));
                    Projectile.HomingTarget(CurTarget.Center, -1, Lerp(1f, 16f, ratios), Lerp(0f, 25f, ratios), Lerp(0f, 5f, ratios));
                if (ratios > .99f)
                {
                    Projectile.netUpdate = true;
                    AttackState = State.Homing;
                    Timer = 0;
                    Projectile.extraUpdates = 3;
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16, 32), Main.rand.NextFloat(1.2f, 3.4f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 45, 1f, Main.rand.NextFloat(.85f, 1.15f) * 0.181f, glowMult: 0.45f);
                }
                for (int i = 0; i < 16; i++)
                {
                    float args = TwoPi / 16f * i;
                    ECSParticle.ShinyCrossStarECS(Projectile.Center, args.ToRotationVector2() * 4f, RandLerpColor(Color.SkyBlue, Color.Aquamarine), 45, 1f, Main.rand.NextFloat(.85f, 1.15f) * 0.381f);
                }
                Projectile.Kill();
            }
        }

        public void DoHoming()
        {
            if (CurTarget.IsLegal() && Projectile.damage > 0)
            {
                Projectile.HomingTarget(CurTarget.Center, -1, 16, 0);
            }
            else
            {
                if (Projectile.velocity.LengthSquared() > 5f)
                {
                    Projectile.timeLeft = 50;
                }
                Projectile.velocity *= .95f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            float ratios = Clamp(Projectile.timeLeft / 50f, 0, 1);
            if (ratios < Main.rand.NextFloat())
                return;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool(9))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity / 4f, RandLerpColor(Color.Aquamarine, Color.SkyBlue), 40, 1f, 0.48f * EaseInBack(ratios));
            if (Main.rand.NextBool(9))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity / 4f, RandLerpColor(Color.Aquamarine, Color.SkyBlue), 40, 1f, 0.48f * ratios);
            if (Main.rand.NextBool(21))
                ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePos(6), -Vector2.UnitY.RotateRandom(PiOver4) * 10f, RandLerpColor(Color.SkyBlue, Color.Aquamarine), 30, 1, RandRotTwoPi, 0.125f * Main.rand.NextFloat(.85f, 1.1f), 0.92f, true, fullBright: true, blendState: BlendState.AlphaBlend);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type != ProjectileType<FlowerofDanceFlower>())
                    continue;
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.ai[0] < 65f)
                    continue;
                proj.ai[1] = 1;
                if (target.IsLegal())
                    ((FlowerofDanceFlower)proj.ModProjectile).CurTarget = target;
            }
            if (Projectile.ai[2] == 1)
            {
                for (int i = 0; i < 1; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, -Vector2.UnitY.ToRandVelocity(ToRadians(30f), 12, 16f), ProjectileType<FlowerofDanceFlower>(), 0, Projectile.knockBack, Projectile.owner);
                    proj.originalDamage = Projectile.damage;
                    proj.timeLeft = 500;
                }
            }
            if(AttackState == State.Shoot)
            {
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16, 32), Main.rand.NextFloat(1.2f, 3.4f), RandLerpColor(Color.SkyBlue, Color.Aquamarine), 45, 1f, Main.rand.NextFloat(.85f, 1.15f) * 0.181f, glowMult: 0.45f);
                }
                for (int i = 0; i < 16; i++)
                {
                    float args = TwoPi / 16f * i;
                    ECSParticle.ShinyCrossStarECS(Projectile.Center, args.ToRotationVector2() * 4f, RandLerpColor(Color.SkyBlue, Color.Aquamarine), 45, 1f, Main.rand.NextFloat(.85f, 1.15f) * 0.381f);
                }

                Projectile.timeLeft = 2;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition + DrawOffset;
            Vector2 ori = tex.ToOrigin();
            float scale = Projectile.scale * 0.45f;
            float rotFixer = PiOver4;
            float timeLeftProgress = Clamp(Projectile.timeLeft / 50f, 0, 1);
            int length = (int)((Projectile.oldPos.Length - 4) *(timeLeftProgress));
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float rad = Clamp(Timer / (Projectile.MaxUpdates * 25), 0f, 0.25f) / 0.25f;

            if (AttackState != State.AngleTo)
            {
                rad = 1;
                for (int i = 0; i < length; i++)
                {
                    float rads = (float)i / length;
                    if (AttackState != State.Shoot)
                        rads = 0;
                    Color drawColor = (Color.Lerp(Color.MediumAquamarine, Color.LightSkyBlue, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * (1 - rad);
                    SB.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter() + DrawOffset, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(1f, 1.5f), 0, 0);
                }
                DrawDaggerTrail(length - 2, rotFixer, scale, tex, ori);
            }
            if (Projectile.damage != 0)
                for (int i = 0; i < 16; i++)
                {
                    SB.Draw(tex, pos + (TwoPi / 16f * i).ToRotationVector2() * 2 * rad * EaseInBack(timeLeftProgress), null, Color.White.ToAddColor() * EaseInBack(timeLeftProgress), Projectile.rotation + rotFixer, ori, scale, 0, 0);
                }
            tex.ApplyMeltShader(Color.White,   1- EaseInOutExpo(timeLeftProgress));
            SB.Draw(tex, pos, null, Color.White * rad * timeLeftProgress, Projectile.rotation + rotFixer, ori, scale, 0, 0);
            SB.EndShaderArea();
            return false;
        }

        private void DrawDetailLine(Color c)
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_Lightning4.Texture;
            Vector2 offset = Projectile.SafeDirByRot() * 0f * 0.45f;
            Vector2 mountedPos = Projectile.Center - offset;
            Vector2 mountedPos2 = Projectile.Center + offset;
            float BeamLength = (mountedPos - Owner.MountedCenter).Length();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            //轨迹
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -40);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            float rot = (mountedPos - Owner.MountedCenter).ToRotation();
            Vector2 startPos = Owner.MountedCenter - Main.screenPosition;
            float width = 0.25f * Projectile.scale;
            Vector2 beamScale = new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), width);
            SB.Draw(value.Value, startPos, null, c, rot, orig, beamScale, 0, 0);
            beamScale = new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), width * 0.85f);
            SB.Draw(value.Value, startPos, null, Color.White * 0.5f, rot, orig, beamScale, 0, 0);
            //边框
        }

        public void DrawDaggerTrail(int length, float rotFixer, float scale,Texture2D tex, Vector2 ori)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2 + DrawOffset;
                float rot = Projectile.oldRot[i] + rotFixer;
                float ratios = i / (float)length;
                Color c = Color.Lerp(Color.MediumAquamarine, Color.LightSkyBlue, ratios);
                float colorLerp = Lerp(105, 0, ratios);
                float opac = Lerp(.95f, 0.45f, ratios) * Clamp(Projectile.velocity.Length(), 0, 1);
                float oldScale = Lerp(scale * .95f, scale * .10f, ratios);
                c = c.ToAddColor((byte)colorLerp) * opac;
                for (int j = 0; j < 4; j++)
                {
                    SB.Draw(tex, oldPos + (TwoPi / 4f * j).ToRotationVector2() * 2, null, c.ToAddColor(200), rot, ori, oldScale, 0, 0);
                }
                SB.Draw(tex, oldPos, null, c, rot, ori, oldScale, 0, 0);
            }

        }
    }
}