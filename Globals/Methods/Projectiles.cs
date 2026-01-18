using ContinentOfJourney.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static Vector2 PosToCenter(this Projectile proj) => proj.Size / 2 - Main.screenPosition;
        public static Vector2 ToOrigin(this Texture2D tex) => tex.Size() / 2;
        public static void GetHeldProjDrawState(this Projectile proj, float rotFix, out Texture2D projTex, out Vector2 drawPos, out Vector2 drawOri, out float projRot, out SpriteEffects projSP)
        {
            projTex = proj.GetTexture();
            drawPos = proj.Center - Main.screenPosition;
            Player player = Main.player[proj.owner];
            projRot = proj.rotation + (player.direction == -1 ? Pi : 0f) + rotFix * player.direction;

            drawOri = projTex.Size() / 2;

            projSP = player.direction * player.gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
        public static void GetProjDrawData(this Projectile proj, out Texture2D projTex, out Vector2 drawPos, out Vector2 ori)
        {
            projTex = proj.GetTexture();
            drawPos = proj.Center - Main.screenPosition;
            ori = projTex.Size() / 2;
        }
        /// <summary>
        /// 轨迹设置，一般情况下默认使用模式2
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="length"></param>
        /// <param name="mode"></param>
        public static void ToTrailSetting(this Projectile proj, int length = 4, int mode = 2)
        {
            ProjectileID.Sets.TrailingMode[proj.type] = mode;
            ProjectileID.Sets.TrailCacheLength[proj.type] = length;
        }
        /// <summary>
        /// 将proj的vel转化为单位向量
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static Vector2 SafeDir(this Projectile proj) => proj.velocity.SafeNormalize(Vector2.UnitX);
        /// <summary>
        /// 将proj的rot转化为单位向量
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static Vector2 SafeDirByRot(this Projectile proj) => proj.rotation.ToRotationVector2();
        public static Vector2 SafeDirByRot(this Projectile proj, float rotDegree) => proj.rotation.ToRotationVector2().RotatedBy(ToRadians(rotDegree));
        public static Texture2D GetTexture(this Projectile proj) => TextureAssets.Projectile[proj.type].Value;
        public static void GetTexture(this Projectile proj, out Texture2D projTex, out Vector2 orig, out Vector2 drawPos)
        {
            projTex = TextureAssets.Projectile[proj.type].Value;
            orig = projTex.Size() / 2;
            drawPos = proj.Center - Main.screenPosition;
        }
        /// <summary>
        /// 获取一个单位，这里优先判定输入的NPC索引
        /// 如果需要直接忽略npc索引。输入-1
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="target"></param>
        /// <param name="targetIndex"></param>
        /// <param name="searchSecondTarget"></param>
        /// <param name="searchDistance"></param>
        /// <returns></returns>
        public static bool GetTargetSafe(this Projectile proj, out NPC target, int targetIndex, bool searchSecondTarget = true, float searchDistance = 600f)
        {
            target = null;
            if (targetIndex != -1)
            {
                target = Main.npc[targetIndex];
                if (target.CanBeChasedBy(proj) && target != null)
                    return true;
                else if (searchSecondTarget)
                {
                    target = proj.FindClosestTarget(searchDistance);
                    if (target != null && target.CanBeChasedBy(proj))
                        return true;
                }
            }
            else if (searchSecondTarget)
            {
                target = proj.FindClosestTarget(searchDistance);
                if (target != null && target.CanBeChasedBy(proj))
                    return true;
            }
            return target != null;
        }
        /// <summary>
        /// 获取一个单位，这里优先判定输入的NPC索引
        /// 直接使用模组内的GlobalTargetIndex
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="target"></param>
        /// <param name="targetIndex"></param>
        /// <param name="searchSecondTarget"></param>
        /// <param name="searchDistance"></param>
        /// <returns></returns>
        public static bool GetTargetSafe(this Projectile proj, out NPC target, bool searchSecondTarget = true, float searchDistance = 600f)
        {
            target = null;
            if (proj.HJScarlet().GlobalTargetIndex!= -1)
            {
                target = Main.npc[proj.HJScarlet().GlobalTargetIndex];
                if (target.CanBeChasedBy(proj) && target != null)
                    return true;
                else if (searchSecondTarget)
                {
                    target = proj.FindClosestTarget(searchDistance);
                    if (target != null && target.CanBeChasedBy(proj))
                        return true;
                }
            }
            else if (searchSecondTarget)
            {
                target = proj.FindClosestTarget(searchDistance);
                if (target != null && target.CanBeChasedBy(proj))
                    return true;
            }
            return target != null;
        }

        /// <summary>
        /// 用于搜索距离射弹最近的npc单位，并返回NPC实例。通常情况下与上方的追踪方法配套
        /// 这个方法会同时实现穿墙、数组、boss优先度的搜索。不过只能用于射弹。但也足够
        /// 这里Boss优先度的实现逻辑是如果我们但凡搜索到一个Boss，就把这个Boss临时存储，在返回实例的时候优先使用
        /// </summary>
        /// <param name="p">射弹</param>
        /// <param name="maxDist">最大搜索距离</param>
        /// <param name="bossFirst">boss优先度，这个还没实现好逻辑，所以填啥都没用（</param>
        /// <param name="ignoreTiles">穿墙搜索, 默认为</param>
        /// <param name="arrayFirst">数组优先, 这个将会使射弹优先针对数组内第一个单位,默认为否</param>
        /// <returns>返回一个NPC实例</returns>
        public static NPC FindClosestTarget(this Projectile p, float maxDist, bool bossFirst = false, bool ignoreTiles = true, bool arrayFirst = false)
        {
            //bro我真的要遍历整个NPC吗？
            float distStoraged = maxDist;
            NPC tryGetBoss = null;
            NPC acceptableTarget = null;
            bool alreadyGetBoss = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float exDist = npc.width + npc.height;

                //单位不可被追踪 或者 超出索敌距离则continue
                if (Vector2.Distance(p.Center, npc.Center) > distStoraged + exDist)
                    continue;

                if (!npc.active || npc.friendly || npc.lifeMax < 5 || !npc.CanBeChasedBy(p.Center, false))
                    continue;

                //补: 如果优先搜索Boss单位, 且附近至少有一个。我们直接存储这个Boss单位
                //已经获取到的会被标记，使其不会再跑一遍搜索.
                if (npc.boss && bossFirst && !alreadyGetBoss)
                {
                    tryGetBoss = npc;
                    alreadyGetBoss = true;
                }

                //搜索符合条件的敌人, 准备返回这个NPC实例
                float curNpcDist = Vector2.Distance(npc.Center, p.Center);
                if (curNpcDist < distStoraged && (ignoreTiles || Collision.CanHit(p.Center, 1, 1, npc.Center, 1, 1)))
                {
                    distStoraged = curNpcDist;
                    acceptableTarget = npc;
                    if (tryGetBoss != null & bossFirst)
                        acceptableTarget = tryGetBoss;
                    //如果是数组优先，直接在这返回实例
                    if (arrayFirst)
                        return acceptableTarget;
                }
            }
            //返回这个NPC实例
            return acceptableTarget;
        }
        public static NPC FindClosestTarget(this Projectile p, float maxDist, Vector2 center, bool bossFirst = false, bool ignoreTiles = true, bool arrayFirst = false)
        {
            //bro我真的要遍历整个NPC吗？
            float distStoraged = maxDist;
            NPC tryGetBoss = null;
            NPC acceptableTarget = null;
            bool alreadyGetBoss = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float exDist = npc.width + npc.height;

                //单位不可被追踪 或者 超出索敌距离则continue
                if (Vector2.Distance(center, npc.Center) > distStoraged + exDist)
                    continue;

                if (!npc.active || npc.friendly || npc.lifeMax < 5 || !npc.CanBeChasedBy(p.Center, false))
                    continue;

                //补: 如果优先搜索Boss单位, 且附近至少有一个。我们直接存储这个Boss单位
                //已经获取到的会被标记，使其不会再跑一遍搜索.
                if (npc.boss && bossFirst && !alreadyGetBoss)
                {
                    tryGetBoss = npc;
                    alreadyGetBoss = true;
                }

                //搜索符合条件的敌人, 准备返回这个NPC实例
                float curNpcDist = Vector2.Distance(npc.Center, center);
                if (curNpcDist < distStoraged && (ignoreTiles || Collision.CanHit(center, 1, 1, npc.Center, 1, 1)))
                {
                    distStoraged = curNpcDist;
                    acceptableTarget = npc;
                    if (tryGetBoss != null & bossFirst)
                        acceptableTarget = tryGetBoss;
                    //如果是数组优先，直接在这返回实例
                    if (arrayFirst)
                        return acceptableTarget;
                }
            }
            //返回这个NPC实例
            return acceptableTarget;
        }

        /// <summary>
        /// 用于跟踪指定地点的方法
        /// 只会跟踪你传进去的目标
        /// </summary>
        /// <param name="proj">射弹</param>
        /// <param name="target">射弹目标</param>
        /// <param name="distRequired">最大范围</param>
        /// <param name="speed">射弹速度</param>
        /// <param name="inertia">惯性</param>
        /// <param name="maxAngleChage">角度限制，默认为空. </param>
        public static void HomingTarget(this Projectile proj, Vector2 target, float distRequired, float speed, float inertia, float? maxAngleChage = null)
        {
            if (distRequired > 0 && Vector2.Distance(proj.Center, target) > distRequired)
                return;
            //开始追踪target
            Vector2 home = (target - proj.Center).SafeNormalize(Vector2.UnitY);
            Vector2 velo = (proj.velocity * inertia + home * speed) / (inertia + 1f);
            //这里给了一个角度限制
            if (maxAngleChage.HasValue)
            {
                float curAngle = proj.velocity.ToRotation();
                float tarAngle = velo.ToRotation();
                float angleDiffer = WrapAngle(tarAngle - curAngle);
                //转弧度
                float maxRadians = ToRadians(maxAngleChage.Value);
                if (Math.Abs(angleDiffer) > maxRadians)
                {
                    float clampedAngle = curAngle + Math.Sign(angleDiffer) * maxRadians;
                    float setSpeed = velo.Length();
                    velo = new Vector2((float)Math.Cos(clampedAngle), (float)Math.Sin(clampedAngle)) * setSpeed;
                }
            }
            //设定速度
            proj.velocity = velo;
        }
        /// <summary>
        /// 预制发光边缘
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="color"></param>
        /// <param name="drawTime"></param>
        /// <param name="posMove"></param>
        /// <param name="rotFix"></param>
        public static void DrawGlowEdge(this Projectile proj, Color color, int drawTime = 8, float posMove = 2f, float rotFix = 0f)
        {
            //绘制发光边缘
            for (int i = 0; i < drawTime; i++)
                Main.spriteBatch.Draw(proj.GetTexture(), proj.Center - Main.screenPosition + ToRadians(i * 60f).ToRotationVector2() * posMove, null, color with { A = 0 }, proj.rotation + rotFix, proj.GetTexture().Size() / 2, proj.scale, 0, 0f);
        }
        /// <summary>
        /// 预制射弹绘制
        /// </summary>
        /// <param name="proj"></param>

        public static void DrawProj(this Projectile proj, Color color, int drawTime = 4, float offset = 0.7f, float rotFix = 0, bool useOldPos = false)
        {
            Texture2D tex = proj.GetTexture();
            Vector2 orig = tex.Size() / 2;
            Vector2 drawPos = proj.Center - Main.screenPosition;
            int drawLength = drawTime > proj.oldPos.Length ? proj.oldPos.Length : drawTime;
            for (int i = 1; i < drawLength; i++)
            {
                if (proj.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 trailingDrawPos = useOldPos ? proj.oldPos[i] + proj.PosToCenter() : drawPos - proj.velocity * i * offset;
                float faded = 1 - i / (float)drawLength;
                //平方放缩
                faded = MathF.Pow(faded, 2);
                Color trailColor = color * faded;
                Main.spriteBatch.Draw(tex, trailingDrawPos, null, trailColor, proj.oldRot[i] + rotFix, orig, proj.scale, 0, 0);
            }
            //直接绘制主射弹位于最顶层
            Main.spriteBatch.Draw(tex, drawPos, null, color, proj.rotation + rotFix, orig, proj.scale, 0, 0.1f);
        }
        /// <summary>
        /// 将TargetIndex转化为NPC。这里直接调用的模组内部的GlobalTargetIndex字段
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ToHJScarletNPC(this Projectile proj, out NPC target)
        {
            if (proj.HJScarlet().GlobalTargetIndex == -1)
            {
                target = null;
                return false;
            }
            target = Main.npc[proj.HJScarlet().GlobalTargetIndex];
            return true;
        }
        /// <summary>
        /// 将TargetIndex转化为NPC。这里直接调用的模组内部的GlobalTargetIndex字段
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static NPC ToHJScarletNPC(this Projectile proj)
        {
            if (proj.HJScarlet().GlobalTargetIndex == -1)
                return null;
            return Main.npc[proj.HJScarlet().GlobalTargetIndex];
        }
        /// <summary>
        /// 我也不知道为什么我要写这种拓展
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Projectile NewProjectileDirect<T>(this Projectile proj, IEntitySource src, Vector2 position, Vector2 velocity, int damage, float kb, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2=0) where T : ModProjectile
        {
            return Projectile.NewProjectileDirect(src,position,velocity,ProjectileType<T>(),damage,kb,owner,ai0,ai1,ai2);
        }
        public static void ExpandHitboxBy(this Projectile projectile, int width, int height)
        {
            projectile.position = projectile.Center;
            projectile.width = width;
            projectile.height = height;
            projectile.position -= projectile.Size * 0.5f;
        }
        public static void ExpandHitboxBy(this Projectile projectile, int newSize)
        {
            projectile.ExpandHitboxBy(newSize, newSize);
        }
        public static void ExpandHitboxBy(this Projectile projectile, float expandRatio)
        {
            projectile.ExpandHitboxBy((int)((float)projectile.width * expandRatio), (int)((float)projectile.height * expandRatio));
        }
        public static bool TooAwayFromOwner(this Projectile proj, float distance = 1800f)
        {
            Player owner = Main.player[proj.owner];
            return Vector2.Distance(owner.MountedCenter, proj.Center) > distance;
        }
    }
}
