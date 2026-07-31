using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.General
{
    /// <summary>
    /// 这个伞用射弹做，最主要是为了画刀光和消失特效
    /// </summary>
    public class TairitsuProj : HJScarletProj, IPixelatedRenderer
    {
        public AnimationStruct Helper = new AnimationStruct(4);
        public List<Vector2> OldAimPos = [];
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1.3f;
        public float Width = 1.3f;
        public int AttackSpeed = 160;
        public ref float Timer => ref Projectile.ai[0];
        public ref float BeingTargetRotation => ref Projectile.ai[1];
        public int TotalProgressDead;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = TotalProgressDead= (int)(AttackSpeed * 1.65f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * 1.65f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * .6f);
            Helper.MaxProgress[3] = (int)(AttackSpeed * .95f);
            TargetRotation = 0;
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            if (OldAimPos.Count > 100)
                OldAimPos.RemoveAt(0);
        }
        public bool InitSound = false;
        public void UpdateAnimation()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                {
                    float offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
                    Vector2 pos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, offset - PiOver2);
                    ECSParticle.CrossGlow(pos, Color.SkyBlue, 40, 1, 0.35f, 0.5f);
                    ECSParticle.CrossGlow(pos, Color.White, 40, 1, 0.32f, 0.5f);
                }
                Helper.UpdateAniState(0);
                UpdateBeginAnimation();
            }
            else if (!Helper.IsDone[1])
            {
                InitSound = false;
                Helper.UpdateAniState(1);
                UpdateMidAnimation();

            }
            else
            {
                //实际上，静止不动的情况下我也不知道为什么要管这个
                //0~1
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);
                float omega = Main.GlobalTimeWrappedHourly / 2f;
                float sinValue = (float)Math.Sin(omega) + 0f;
                float ratios = ((float)Math.Abs(sinValue));

                float rotValueChange = Lerp(ToRadians(-8f), ToRadians(5f), ratios);
                Vector2 skyDir = (-Vector2.UnitY).RotatedBy(rotValueChange);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, skyDir,0.23f);
                float targetRotaiton = Projectile.velocity.ToRotation();
                float currentRotation = Projectile.rotation;
                float value = WrapAngle(targetRotaiton - currentRotation);

                float innerRot = Lerp(Projectile.rotation, currentRotation + value, .012f);
                float targetRot = Lerp(innerRot,currentRotation + value,0.012f);
                Projectile.rotation = Lerp(targetRot, currentRotation + value, 0.014f);
                bool killUp = (Math.Abs(Owner.velocity.X) + Math.Abs(Owner.velocity.Y)) > 5f || (Main.mouseLeft && Main.mouseLeftRelease) || (Main.mouseRight && Main.mouseRightRelease);
                if( killUp && !ShouldKill)
                {
                    ShouldKill = true;
                }
                if (ShouldKill)
                {
                    float progress = (Clamp(Timer / TotalProgressDead, 0, 1));
                    Timer++;
                    if (progress >= 0.3f && !InitSound)
                    {
                        InitSound = true;
                        ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center,pitch:-.4f);

                        //for (int i = 0; i < 16; i++)
                        //{
                        //    float offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
                        //    Vector2 pos = Owner.GetBackHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation- PiOver2);
                        //    pos += Projectile.rotation.ToRotationVector2() * 20f;
                        //    ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePos(6), 1.2f, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 80, 1, Main.rand.NextFloat(0.8f, 1.2f) * .07f);
                        //}
                        //for (int i = 0; i < 8; i++)
                        //{
                        //    float offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
                        //    Vector2 pos = Owner.GetBackHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation- PiOver2);
                        //    pos += Projectile.rotation.ToRotationVector2() * 20f;
                        //    ECSParticle.LiliesPetal(pos, (-Vector2.UnitY).ToRandVelocity(ToRadians(15), 2, 4), RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1, RandRotTwoPi, 0.05f, 0.2f, true, 1, true);
                        //}

                    }
                    if(progress== 1f)
                    {
                        Projectile.Kill();
                    }

                }
            }
        }
        public void UpdateBeginAnimation()
        {
            float curAniPro = Helper.GetAniProgress(0);
            float pro = EaseInOutQuad(curAniPro);
            if (pro > .05f && !InitSound)
            {
                InitSound = true;
                ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center);
                for (int i = 0; i < 16; i++)
                {
                    float offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
                    Vector2 pos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, offset - PiOver2);
                    ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePos(6), 1.2f, RandLerpColor(Color.SkyBlue, Color.RoyalBlue), 80, 1, Main.rand.NextFloat(0.8f, 1.2f) * .07f);
                }
                for (int i = 0; i < 8; i++)
                {
                    float offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
                    Vector2 pos = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, offset - PiOver2);
                    ECSParticle.LiliesPetal(pos, (-Vector2.UnitY).ToRandVelocity(ToRadians(15), 2, 4), RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1, RandRotTwoPi, 0.05f, 0.2f, true, 1, true);
                }
            }
            float beginAngle = -110 * Flip.ToDirectionInt();
            float endAngle = -100 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
        }
        public Vector2 FinalTarPos = Vector2.Zero;
        public void UpdateMidAnimation()
        {
            float curAniPro = Helper.GetAniProgress(1);
            float pro = EaseInOutQuad(curAniPro);
            float beginAngle = -100 * Flip.ToDirectionInt();
            float endAngle = 75 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, pro);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(Width, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1f;
            FinalTarPos = tarPos;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (pro < 0.02f)
                return;
            //下面基本上是粒子生成了。
            float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle + (0 * (Flip).ToDirectionInt()), Owner.direction, pro);
            Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(Width, Height, 1f);
            Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * 1.4f;
            Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 30;
            OldAimPos.Add(slashPosFinal);

            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 30 * 1.4f, Main.rand.NextFloat(0.6f, 1f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1, 0.2f);
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 30 * 1.4f, Main.rand.NextFloat(0.6f, 1f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                ECSParticle.LiliesPetal(pos, vel, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, 1,RandRotTwoPi, 0.05f,0.2f,true,1,true);
            }

        }
        public void UpdatePlayerState()
        {
            if (!Helper.IsDone[1])
            {
                Projectile.velocity = TargetRotation.ToRotationVector2();
            }
            Projectile.position.Y += Owner.gfxOffY;
            float offset = 0;
            if (!Helper.IsDone[1])
            {
                Owner.ChangeDir(Projectile.direction);
                Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            }
            else
            {
                int flipSide = ((Main.MouseWorld.X - Owner.Center.X) > 0).ToDirectionInt();
                Owner.ChangeDir(flipSide);
                Projectile.spriteDirection = Flip.ToDirectionInt() * flipSide;
                //让伞盖过对立的头顶
            }
            if (Helper.IsDone[1] && Helper.IsDone[0])
                Owner.heldProj = Projectile.whoAmI;
            offset = Projectile.spriteDirection < 0 ? PiOver2 + ToRadians(30) : Pi + PiOver2 - ToRadians(30);
            Owner.ControlPlayerArm(Projectile.rotation + offset, 1);
            Owner.ControlPlayerArm(Projectile.rotation, -1);
        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            if (Owner.dead || Main.mouseRight)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }
        public override bool? CanDamage() => false;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            //给刀画刀光的来了
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D texture =  HJScarletTexture.Texture_SwordSlash.Value;
            Effect effect = HJScarletShader.AlphaFade;
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.21f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            effect.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            effect.CurrentTechnique.Passes[0].Apply();
            DrawSlash(texture, Color.DeepSkyBlue* 0.55f, 0.95f);
            DrawSlash(texture, Color.LightBlue* 0.40f, 0.50f);
            effect.Parameters["uFadeoutLeftLength"].SetValue(0.1f);
            effect.Parameters["uFadeinRigtLength"].SetValue(0.05f);
            DrawSlash(texture, Color.Lerp(Color.SkyBlue, Color.White, 0.760f) * 0.75f, 0.85f, 0.95f);
            DrawSlash(texture, Color.Lerp(Color.LightBlue, Color.White, 0.790f) * 0.75f, 0.50f, 0.95f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(.1f, .1f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 1.395f, 0), new Vector2(1, 2), Color.White);
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.LightSkyBlue, 0.60f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.White, 0.45f);

            HJScarletMethods.EndShaderAreaPixel();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f, float beginMult = 1f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] * beginMult + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
        public bool ShouldKill = false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            //byd你这伞让我吃了半口大的
            int offset = 2;
            //确保对立与伞的位置不会离得太远
            Vector2 pos = Projectile.Center - Main.screenPosition - new Vector2(2, 2).RotatedBy(Projectile.rotation);
            Texture2D tex = Projectile.GetTexture();
            if (Helper.IsDone[1] && Helper.IsDone[0])
            {
                tex = Request<Texture2D>(Texture + "Alt").Value;
            }
            Rectangle texFrame = tex.Frame();
            Vector2 ori = texFrame.BottomLeft() + new Vector2(offset, -(offset + 4));

            float rotOffset = 30;
            float rot = Projectile.rotation + ToRadians(rotOffset);
            SpriteEffects se = SpriteEffects.FlipHorizontally;
            if (Projectile.spriteDirection > 0)
            {
                //确保对立能拿到这把伞的把柄
                ori = texFrame.BottomRight() - new Vector2(offset, 4);
                //确保对立的伞拿的角度是对的
                rot = Projectile.rotation - ToRadians(rotOffset) - Pi;
                //确保对立的伞翻转是对的
                se = SpriteEffects.None;
            }

            float progress = Helper.GetAniProgress(0);
            float meltPro = 1- EaseInOutQuad(progress);
            if(ShouldKill)
            {

                    progress = (Clamp(Timer / TotalProgressDead, 0, 1));
                //progress =Utils.GetLerpValue(0, (float)TotalProgressDead, Timer, true);
                meltPro = EaseInOutExpo(progress);
            }
            HJScarletMethods.ApplyMeltShader(tex, Color.SkyBlue, meltPro);
            //最后画出来
            SB.Draw(tex, pos, null, Color.White, rot, ori, Projectile.scale, se, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
