using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static HJScarletPlayer HJScarlet(this Player player) => player.GetModPlayer<HJScarletPlayer>();
        public static HJScarletGlobalProjs HJScarlet(this Projectile proj) => proj.GetGlobalProjectile<HJScarletGlobalProjs>();
        public static HJScarletGlobalNPCs HJScarlet(this NPC npc) => npc.GetGlobalNPC<HJScarletGlobalNPCs>();
        public static bool HasProj<T>(this Player player) where T : ModProjectile => HasProj(player, ProjectileType<T>());
        public static bool HasProj(this Player player, int projID) => player.ownedProjectileCounts[projID] > 0;

        /// <summary>
        /// 重载一个out传参，输出你判定的拥有的proj的ID以方便后续可能需要的计算，或者别的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="player"></param>
        /// <param name="ProjID"></param>
        /// <returns></returns>
        public static bool HasProj<T>(this Player player, out int ProjID) where T : ModProjectile
        {
            ProjID = ProjectileType<T>();
            return HasProj<T>(player);
        }
        public static Color RandLerpTo(this Color srcColor, Color endColor) => Color.Lerp(srcColor, endColor, Main.rand.NextFloat(0f, 1f));
        public static bool OutOffScreen(Vector2 pos)
        {
            if (pos.X < Main.screenPosition.X - Main.screenWidth / 2)
                return true;

            if (pos.Y < Main.screenPosition.Y - Main.screenHeight / 2)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth * 1.5f)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight * 1.5f)
                return true;

            return false;
        }
        public static bool OutOffScreen(Vector2 pos, float areamult = 1f)
        {
            float halfwidth = Main.screenWidth / 2;
            float halfheight = Main.screenHeight / 2;
            if (pos.X < Main.screenPosition.X - halfwidth * areamult)
                return true;

            if (pos.Y < Main.screenPosition.Y - halfheight * areamult)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth + halfwidth * areamult)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight + halfheight * areamult)
                return true;

            return false;
        }
        public static Vector2 LocalMouseWorld(this Player player) => player.HJScarlet().SyncedMouseWorld;
        public static bool PressLeftAndRightClick(this Player player)
        {
            return player.HJScarlet().MouseLeft && player.HJScarlet().MouseRight;
        }
        public static bool JustPressLeftClick(this Player player)
        {
            return player.HJScarlet().MouseLeft && !player.HJScarlet().MouseRight;
        }

        public static bool JustPressRightClick(this Player player)
        {
            return !player.HJScarlet().MouseLeft && player.HJScarlet().MouseRight;
        }
        /// <summary>
        /// 快速生成一个简单明了的圆形粒子组
        /// </summary>
        /// <param name="dPos"></param>
        /// <param name="dCounts"></param>
        /// <param name="dScale"></param>
        /// <param name="dType"></param>
        /// <param name="dSpeed"></param>
        /// <param name="dPosOffset"></param>
        /// <param name="dGrav"></param>
        /// <param name="dAlpha"></param>
        public static void CirclrDust(this Vector2 dPos, int dCounts, float dScale, int dType, int dSpeed, float dPosOffset = 0f, bool dGrav = true, int dAlpha = 255)
        {
            float rotArg = 360f / dCounts;
            for (int i = 0; i < dCounts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotArg);
                Vector2 offsetPos = new Vector2(dPosOffset, 0f).RotatedBy(rot);
                Vector2 dVel = new Vector2(dSpeed, 0f).RotatedBy(rot);
                Dust d = Dust.NewDustPerfect(dPos + offsetPos, dType, dVel);
                d.noGravity = dGrav;
                d.velocity = dVel;
                d.scale = dScale;
                d.alpha = dAlpha;
            }
        }
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
        public static void ShimmerEach(this int type, int shimmerType, bool shouldDisableConfig = false)
        {
            if (shouldDisableConfig || HJScarletConfigServer.Instance.EnableSameItemShimmer)
            {
                ItemID.Sets.ShimmerTransformToItem[type] = shimmerType;
                ItemID.Sets.ShimmerTransformToItem[shimmerType] = type;
            }
        }
        public static void ShimmerEach<T>(this int type, bool shouldDisableConfig = false) where T : ModItem => ShimmerEach(type, ItemType<T>(), shouldDisableConfig);
        public static Vector2 GetRandPos() => Main.rand.NextFloat(TwoPi).ToRotationVector2() * 2f;
        public static Vector2 ToMouseVector2(this Player player, Vector2? safeValue = null)
        {
            Vector2 safe = safeValue ?? Vector2.UnitX;
            return (player.LocalMouseWorld() - player.MountedCenter).SafeNormalize(safe);
        }
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float speed = 1f) => srcVel.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(randRads) * Main.rand.NextBool().ToDirectionInt()) * speed;
        public static Vector2 ToRandDirection(this Vector2 srcDir, float randRads) => srcDir.RotatedBy(Main.rand.NextFloat(randRads) * Main.rand.NextBool().ToDirectionInt());

        public static string ToPercent(this float value)
        {
            float value2 = value * 100f;
            return $"{value2}%";
        }
        public static int TerrariaCurrentHour
        {
            get
            {
                //获取当前泰拉时间，即Main.Time
                double time = Main.time;

                //假定当前是晚上，则将时间进行校准，出于某些原因，夜晚的时间必须得直接增加54000来表示
                if (!Main.dayTime)
                    time += 54000D;

                //内部时间单位以“刻”表示，而60刻=1秒，因此这里3600刻为1小时，我们将当前的量拆分
                time /= 3600D;

                //泰拉瑞亚的夜晚起始于晚上7点半，因此需要用这个值去指针
                time -= 19.5;

                //假如过度校准，直接自增24小时，让时钟转移一圈避免负值
                if (time < 0D)
                    time += 24D;

                //而后，我们开始常识性地获取分钟
                int intTime = (int)time;
                double deltaTime = time - intTime;

                //将当前的时间转化为一个实际的分钟值
                deltaTime = (int)(deltaTime * 60D);

                //然后，将24个整数小时转为12小时
                if (intTime > 12)
                    intTime = 12;
                return intTime;
            }
        }
    }
}
