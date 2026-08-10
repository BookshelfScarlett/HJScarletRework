using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    internal class TheGreatDipperDipper : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public List<int> StarProjIndex = new(7);
        public ref float Osci => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public float Lerp = 0;
        public int AddLerpTime = 0;
        public bool AddStarNow = false;
        public Vector2[] StarPosOffset = new Vector2[7];
        public int ActiveStarCounts = 0;
        public bool AddExtraStar = false;
        public bool ShouldKill = false;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0;
        }
        public void DipperPosList(int dir)
        {
            //北斗七星的相对坐标定义（已放大三倍）
            //摇光(1)
            StarPosOffset[0] = new Vector2(50 * dir, -200);
            //斗柄第二颗
            StarPosOffset[1] = new Vector2(60 * dir, -80);
            //斗柄第三颗
            StarPosOffset[2] = new Vector2(30 * dir, -40);
            //天权，即原点
            StarPosOffset[3] = new Vector2(0, 0);
            //斗勺第二颗
            StarPosOffset[4] = new Vector2(-50 * dir, 4);
            //斗勺第三颗
            StarPosOffset[5] = new Vector2(-70 * dir, 110);
            //斗勺第四颗
            StarPosOffset[6] = new Vector2(10 * dir, 150);
        }
        public override void OnFirstFrame()
        {
            DipperPosList(1);
            for (int i = 0; i < 7; i++)
            {
                Projectile starProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<TheGreatDipperDipperStar>(), 0, 0, Owner.whoAmI);
                StarProjIndex.Add(starProj.whoAmI);
                starProj.ai[2] = Projectile.whoAmI;
                starProj.localAI[1] = i;
            }
            int count = 0;
            if (Owner.HJScarlet().ExecutionListStored.TryGetValue(ItemType<TheGreatDipper>(), out int value))
                count = value;

            ActiveStarCounts = count;
        }
        public override void ProjAI()
        {

            //固定在玩家身后
            UpdateStarPosOffsetPosition();
            UpdateDipperStatement();
            if (ShouldKill)
            {
                return;
            }
            if (Owner.HJScarlet().ExecutionListStored.TryGetValue(ItemType<TheGreatDipper>(), out int value))
                ActiveStarCounts = value;

            UpdateDipperPosition();
            UpdateDipperStarIndex();
            if (Owner.GetExecutionSrike() && Owner.IsHolding<TheGreatDipper>())
            {
                ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center, .75f, 1, 0.17f);
                ShouldKill = true;
                Owner.RemoveExecutionProgress(ItemType<TheGreatDipper>());
                if (Owner.HJScarlet().theGreatDipperBuff)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<TheGreatDipperGalaxyHelper>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                    ((TheGreatDipperGalaxyHelper)proj.ModProjectile).BeginTargetRotation = Owner.ToMouseVector2().ToRotation();
                    ((TheGreatDipperGalaxyHelper)proj.ModProjectile).Flip = Main.rand.NextBool();

                }
                Owner.HJScarlet().theGreatDipperBuff = !Owner.HJScarlet().theGreatDipperBuff;
            }
        }

        public void SetDipperStarBolt()
        {
        }

        public void UpdateStarPosOffsetPosition()
        {
            if (!ShouldKill)
            {
                DipperPosList(Owner.direction);
            }
        }
        public void CreateDustToLine(Vector2 beginPos, Vector2 targetPos)
        {
            if (Main.rand.NextBool(4))
            {
                Vector2 realBegin = beginPos;
                Vector2 realTarget = targetPos;
                Vector2 dir = realBegin.GetNormalVector2(realTarget);
                float length = (beginPos - targetPos).Length();
                Vector2 dustPos = realBegin.ToRandCirclePos(4) + dir * Main.rand.NextFloat(0.1f, length);
                Vector2 vel = dir * Main.rand.NextFloat(0.1f, .3f);
                ECSParticle.HRShinyOrb(dustPos, RandVelTwoPi(0.1f, 0.2f), Color.LightSkyBlue, 40, 1f, .03f);
            }
        }
        public void UpdateDipperStatement()
        {
            if (Owner.HeldItem.type == ItemType<TheGreatDipper>() && !ShouldKill)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.12f);
                Projectile.timeLeft = 30;
                Osci += ToRadians(0.5f);
                float off = (float)Math.Sin(Osci) * 5f;
                Vector2 mountedCenter = Owner.MountedCenter - Owner.direction * 120f * Vector2.UnitX;
                mountedCenter.Y += off;
                Projectile.Center = Vector2.Lerp(Projectile.Center, mountedCenter, 0.15f);
                Projectile.rotation = 0;
            }
            else
            {
                if (!ShouldKill)
                {
                    ShouldKill = true;
                    if (ActiveStarCounts > 0)
                        ScarletSound(HJScarletSounds.Moonlight_Ding, Projectile.Center, .45f, pitch: -.3f);
                }
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, .02f);
            }
        }
        public void UpdateDipperStarIndex()
        {
            for (int i = 0; i < ActiveStarCounts; i++)
            {
                int idx = StarProjIndex[i];
                Projectile star = Main.projectile[idx];
                if (star.active && star is not null && star.type == ProjectileType<TheGreatDipperDipperStar>())
                {
                    //localAI[0]标记为1f用来存放是否允许亮起。
                    star.localAI[0] = ShouldKill ? 0f : 1f;
                }
            }
        }

        public void UpdateDipperPosition()
        {

            float rotation = Projectile.rotation;
            //七星的位置必须得随时更新避免位置跳变导致的违和感，
            for (int i = 0; i < StarProjIndex.Count; i++)
            {
                Projectile star = Main.projectile[StarProjIndex[i]];
                if (star != null && star.active && star.type == ProjectileType<TheGreatDipperDipperStar>())
                {
                    Vector2 offset = StarPosOffset[i].RotatedBy(rotation);
                    star.Center = Projectile.Center + offset;
                    star.rotation = offset.ToRotation();
                }
            }
            if (Main.rand.NextBool(6))
            {
                for (int i = 0; i < ActiveStarCounts - 1; i++)
                {
                    CreateDustToLine(Main.projectile[StarProjIndex[i]].Center, Main.projectile[StarProjIndex[i + 1]].Center);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            SB.EnterShaderArea();
            if (ActiveStarCounts > 1)
            {
                for (int i = 0; i < ActiveStarCounts - 1; i++)
                {
                    Projectile beginProj = Main.projectile[StarProjIndex[i]];
                    Projectile endProj = Main.projectile[StarProjIndex[i + 1]];
                    Vector2 beginPos = beginProj.Center;
                    Vector2 endPos = endProj.Center;
                    DrawTheLine(beginPos, endPos, Color.RoyalBlue, 1f);
                    DrawTheLine(beginPos, endPos, Color.White * .805f, .75f);
                }
            }

            SB.EndShaderArea();
            return false;
        }
        public void DrawTheLine(Vector2 beginPos, Vector2 targetPos, Color c, float thick)
        {
            targetPos -= Main.screenPosition;
            beginPos -= Main.screenPosition;
            Asset<Texture2D> tex = HJScarletTexture.Trail_ManaStreak.Texture;
            Vector2 vec = beginPos.GetNormalVector2(targetPos);
            float length = Vector2.Distance(beginPos, targetPos);
            Vector2 orig = new Vector2(0, tex.Height() / 2f);
            float xScale = (length + 5) / tex.Width();
            float rotation = vec.ToRotation();
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -20);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.05f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(tex.Value, beginPos, null, Color.White, rotation, orig, new Vector2(xScale, .058f * thick), 0, 0);
            Texture2D noiseTex = HJScarletTexture.Noise_BlackGalaxy1.Value;
            length = Vector2.Distance(beginPos, targetPos);
            orig = new Vector2(0, noiseTex.Height / 2f);
            xScale = (length + 2) / noiseTex.Width;
            rotation = vec.ToRotation();
            shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -1.5f);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.05f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();

            Vector2 noisePos = beginPos;
            SB.Draw(noiseTex, noisePos, null, Color.White, rotation, orig, new Vector2(xScale, 0.01f * thick), 0, 0);
            //noiseTex = HJScarletTexture.Noise_BlackGalaxy2.Value;
            //SB.Draw(noiseTex, noisePos, null, Color.White, rotation, orig,new Vector2(xScale,0.01f*thick), 0, 0);
        }
    }
}
