using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class TheMarsProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + $"Proj_{nameof(TheMars)}";
        public float RingRotation = 0;
        public List<NPC> LegalTargetList = [];
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Shoot,
            Fade
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            //此处的Timeleft与实际时长无关。下面会用一个计时器手动控制射弹的处死
            Projectile.timeLeft = 9999;
        }
        private float SearchTargetDistance = 800f;
        public override void AI()
        {
            RingRotation += ToRadians(1);
            Projectile.velocity *= 0.967f;
            if (Vector2.Distance(Projectile.Center, Owner.MountedCenter) > 1800f)
                Projectile.Kill();
            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.Fade:
                    DoFade();
                    break;
            }
        }

        public void DoShoot()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = Projectile.Center + Projectile.SafeDirByRot() * 80f + Main.rand.NextVector2Circular(4f, 4f);
                new TurbulenceShinyCube(spawnPos, Projectile.velocity / 2, Color.White.RandLerpTo(Color.Green), 20, Projectile.rotation, 1, 0.28f, randPosMoveValue: 8).Spawn();
            }
            //全局寻找并锁定需要的单位，这里最多锁定6个单位
            if (LegalTargetList.Count < 6)
                GetLegalTarget();
            //如果存在时间超过1秒，进入处死状态
            if(Timer > 10f * Projectile.MaxUpdates)
            {
                AttackType = Style.Fade;
                Projectile.netUpdate = true;
                Timer = 0;
                //这里要干掉额外更新避免意外
                Projectile.extraUpdates = 2;
            }
        }
        private void GetLegalTarget()
        {
            //创建一个链表，搜索附近可能的单位
            float searchDist = SearchTargetDistance;
            foreach (NPC needTar in Main.ActiveNPCs)
            {
                bool legalTarget = needTar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(needTar.Center, Projectile.Center);
                if (legalTarget && distPerTar < searchDist && !LegalTargetList.Contains(needTar))
                {
                    searchDist = distPerTar;
                    //把可用单位甩进去
                    LegalTargetList.Add(needTar);
                }
            }
        }

        public void DoFade()
        {
            //假定我们有实际且可用的单位，为所有单位生成标记射弹
            //这个标记射弹不会有任何视觉效果
            if (LegalTargetList.Count > 0 && Timer == 0f)
            {
                for (int i = 0; i < LegalTargetList.Count; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), LegalTargetList[i].Center, Vector2.Zero, ProjectileType<TheMarsMark>(), 0, 0, Owner.whoAmI);
                    proj.HJScarlet().GlobalTargetIndex = LegalTargetList[i].whoAmI;
                    proj.originalDamage = Projectile.damage;
                }
            }
            Timer++;
            //启用计时器，并在一定时间后直接处死射弹
            if (Timer < 90f)
                return;
            Projectile.Opacity -= 0.1f;
            //为射弹直接提供一个向上的加速度
            if (Projectile.Opacity <= 0f)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            //是的孩子们，为了绘制这把武器的轨迹，这个棱形贴图被绘制了超过120次
            Vector2 projDir = Projectile.SafeDirByRot();
            //绘制两边的轨迹
            DrawEdgeTrail(projDir, drawPos, starShape);
            //填充轨迹颜色
            DrawBackGroundColor();
            //绘制光圈
            DrawCirlceGlow(projDir, drawPos, starShape);
            //在有可用目标的时候，往可用目标上绘制光圈
            //是的没错，光圈绘制是在这个射弹进行的
            if (LegalTargetList.Count > 0)
                DrawMarkCircle(starShape);
            Projectile.DrawGlowEdge(Color.White * Projectile.Opacity, rotFix: PiOver4);
            Projectile.DrawProj(Color.White * Projectile.Opacity, rotFix: PiOver4);
            return false;
        }
        private void DrawMarkCircle(Texture2D starShape)
        {
            for (int i = 0; i < LegalTargetList.Count; i++)
            {
                if (!LegalTargetList[i].CanBeChasedBy() || LegalTargetList[i] == null)
                    continue;
                else
                    DrawTargetCircle(LegalTargetList[i].Center - Main.screenPosition, starShape);
            }
        }

        private void DrawBackGroundColor()
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Rectangle cutSource = star.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            int length = 45;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.SkyBlue, Color.Gray, rads) with { A = 150 }) * 0.7f * Projectile.Opacity * (1 - rads);
                Vector2 trailPos = Projectile.Center - Main.screenPosition - Projectile.SafeDir() * 2.1f * i + Projectile.SafeDir() * 19f;
                SB.Draw(star, trailPos, null, drawColor * Projectile.Opacity, Projectile.rotation, ori, Projectile.scale * new Vector2(1.0f, 0.6f) * 0.5f, 0, 0);
            }
        }

        private void DrawEdgeTrail(Vector2 projDir, Vector2 drawPos, Texture2D starShape)
        {
            Vector2 trailScale = Projectile.scale * new Vector2(0.5f, 1.5f) * 1f * Projectile.Opacity;
            float starDrawTime = 16f;
            for (float j = -1;j <= 2;j += 2)
            {
                for (float i = 0; i < starDrawTime; i++)
                {
                    float rads = (float)i / starDrawTime;
                    Vector2 starPos = drawPos + projDir * 10f + projDir.RotatedBy(PiOver2 * j) * 10f - projDir * 8f * i;
                    Color drawColor = (Color.Lerp(Color.DarkGreen, Color.Silver, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                    //中心高光的颜色
                    Color drawColor2 = (Color.Lerp(Color.AliceBlue, Color.White, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                    SB.Draw(starShape, starPos, null, drawColor, Projectile.rotation - PiOver2, starShape.Size() / 2, trailScale, 0, 0);
                    SB.Draw(starShape, starPos, null, drawColor2, Projectile.rotation - PiOver2, starShape.Size() / 2, trailScale * 0.2f, 0, 0);
                }
            }
        }
        public void DrawTargetCircle(Vector2 drawPos, Texture2D starShape)
        {
            float starDrawTime = 20f;
            for (float i = 0; i < starDrawTime; i++)
            {
                Vector2 argDir = Vector2.UnitY.RotatedBy(ToRadians(360f / starDrawTime * i) + RingRotation) * 23f;
                Vector2 starPos = drawPos + argDir;
                Vector2 scale = Projectile.scale * new Vector2(0.2f, 0.5f) * 0.7f * Projectile.Opacity;
                SB.Draw(starShape, starPos, null, Color.DeepSkyBlue with { A = 0 }, argDir.ToRotation(), starShape.ToOrigin(), scale, 0, 0);
                if (i % 4 == 0)
                {
                    SB.Draw(starShape, starPos, null, Color.White with { A = 0 }, argDir.ToRotation() + PiOver2, starShape.Size() / 2, scale, 0, 0);
                }
            }
        }
        public void DrawCirlceGlow(Vector2 projDir, Vector2 drawPos, Texture2D starShape)
        {
            float starDrawTime = 20f;
            for (float i = 0; i < starDrawTime; i++)
            {
                Vector2 argDir = projDir.RotatedBy(ToRadians(360f / starDrawTime * i) + RingRotation) * 23f;
                Vector2 starPos = drawPos + argDir + projDir * 20f;
                Vector2 scale = Projectile.scale * new Vector2(0.2f, 0.5f) * 0.8f * Projectile.Opacity;
                SB.Draw(starShape, starPos, null, Color.LawnGreen with { A = 0 }, argDir.ToRotation(), starShape.ToOrigin(), scale, 0, 0);
                if(i % 4 == 0)
                {
                    SB.Draw(starShape, starPos, null, Color.White with { A = 0}, argDir.ToRotation() + PiOver2, starShape.Size() / 2, scale, 0, 0);
                }
            }
        }
    }
}
