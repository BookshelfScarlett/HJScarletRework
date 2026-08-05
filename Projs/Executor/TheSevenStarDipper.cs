using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// <see cref="TheSevenStarStar"/>武器的北斗七星挂载弹
    /// </summary>
    public class TheSevenStarDipper : HJScarletProj
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
        public override void OnFirstFrame()
        {
            //北斗七星的相对坐标定义
            //摇光(1)
            StarPosOffset[0] = new Vector2(25, -100);
            //斗柄第二颗
            StarPosOffset[1] = new Vector2(30, -40);
            //斗柄第三颗
            StarPosOffset[2] = new Vector2(15, -20);
            //天权，即原点
            StarPosOffset[3] = new Vector2(0, 0);
            //斗勺第二颗
            StarPosOffset[4] = new Vector2(-25, 2);
            //斗勺第三颗
            StarPosOffset[5] = new Vector2(-35, 55);
            //斗勺第四颗
            StarPosOffset[6] = new Vector2(5, 75);
            //在这里直接创建七个链接星星，一开始为隐形状态
            for (int i = 0; i < 7; i++)
            {
                Projectile starProj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<TheSevenStarDipperStar>(), 0, 0, Owner.whoAmI);
                StarProjIndex.Add(starProj.whoAmI);
                starProj.ai[2] = Projectile.whoAmI;
                starProj.localAI[1] = i;
            }
            int count = 0;
            if (Owner.HJScarlet().ExecutionListStored.TryGetValue(ItemType<TheSevenStar>(), out int value))
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
            if (Owner.HJScarlet().ExecutionListStored.TryGetValue(ItemType<TheSevenStar>(), out int value))
                ActiveStarCounts = value;

            UpdateDipperPosition();
            UpdateDipperStarIndex();
            if (Owner.GetExecutionSrike() && Owner.IsHolding<TheSevenStar>())
            {
                ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center, .75f, 1, 0.17f);
                ShouldKill = true;
                Owner.RemoveExecutionProgress(ItemType<TheSevenStar>());
            }

        }

        public void SetDipperStarBolt()
        {
        }

        public void UpdateStarPosOffsetPosition()
        {
            if (!ShouldKill)
            {
                //北斗七星的相对坐标定义
                //摇光(1)
                StarPosOffset[0] = new Vector2(25 * Owner.direction, -100);
                //斗柄第二颗
                StarPosOffset[1] = new Vector2(30 * Owner.direction, -40);
                //斗柄第三颗
                StarPosOffset[2] = new Vector2(15 * Owner.direction, -20);
                //天权，即原点
                StarPosOffset[3] = new Vector2(0, 0);
                //斗勺第二颗
                StarPosOffset[4] = new Vector2(-25 * Owner.direction, 2);
                //斗勺第三颗
                StarPosOffset[5] = new Vector2(-35 * Owner.direction, 55);
                //斗勺第四颗
                StarPosOffset[6] = new Vector2(5 * Owner.direction, 75);
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
            if (Owner.HeldItem.type == ItemType<TheSevenStar>() && !ShouldKill)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1.01f, 0.12f);
                Projectile.timeLeft = 30;
                Osci += ToRadians(0.5f);
                float off = (float)Math.Sin(Osci) * 5f;
                Vector2 mountedCenter = Owner.MountedCenter - Owner.direction * 80f * Vector2.UnitX;
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
                if (star.active && star is not null && star.type == ProjectileType<TheSevenStarDipperStar>())
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
                if (star != null && star.active && star.type == ProjectileType<TheSevenStarDipperStar>())
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
            if(ActiveStarCounts > 1)
            {
                for(int i =0;i<ActiveStarCounts - 1;i++)
                {
                    Projectile beginProj = Main.projectile[StarProjIndex[i]];
                    Projectile endProj = Main.projectile[StarProjIndex[i+1]];
                    Vector2 beginPos = beginProj.Center;
                    Vector2 endPos = endProj.Center;
                    DrawTheLine(beginPos, endPos, Color.RoyalBlue, 1f);
                    DrawTheLine(beginPos, endPos, Color.White*.805f, .75f);
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
            float xScale = length / tex.Width();
            float rotation = vec.ToRotation();
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -20);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(tex.Value, beginPos, null, Color.White, rotation, orig, new Vector2(xScale, .0351f * thick), 0, 0);
        }

    }
}
