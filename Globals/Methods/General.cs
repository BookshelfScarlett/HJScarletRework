using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Instances.Projs;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using System;
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
        public static HJScarletGlobalItem HJScarlet(this Item item) => item.GetGlobalItem<HJScarletGlobalItem>();
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
        public static bool HoldingTools(this Player player, int damageThreshold = 5)
        {
            Item item = player.HeldItem;
            return item.pick > 0 || item.axe > 0 || item.damage < damageThreshold;
        }

        public static bool JustPressRightClick(this Player player)
        {
            return !player.HJScarlet().MouseLeft && player.HJScarlet().MouseRight;
        }
        public static bool GetImmnue(this Player player, int cooldownSlot, int frames, bool blink = false)
        {
            if (!((cooldownSlot < 0) ? (player.immuneTime < frames) : (player.hurtCooldowns[cooldownSlot] < frames)))
            {
                return false;
            }

            player.immune = true;
            player.immuneNoBlink = !blink;
            if (cooldownSlot < 0)
            {
                player.immuneTime = frames;
            }
            else
            {
                player.hurtCooldowns[cooldownSlot] = frames;
            }

            return true;
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
        /// <summary>
        /// 检测一条线段是否与一个轴对齐矩形（AABB）相交，支持线段宽度。
        /// 适用于判定激光、射线等与矩形区域的碰撞。
        /// 注意：目前仅用于“圣神断罪”激光的碰撞检测。
        /// </summary>
        /// <param name="start">线段的起始点。</param>
        /// <param name="end">线段的结束点。</param>
        /// <param name="rect">要检测碰撞的矩形。</param>
        /// <param name="lineWidth">线段的虚拟宽度（像素），用于模拟较粗的碰撞范围。默认值4。</param>
        /// <param name="checkDistance">该参数在当前实现中未使用，保留供将来扩展。默认值8。</param>
        /// <returns>如果线段（或其加粗范围）与矩形相交，或者线段的端点位于矩形内部，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// 检测逻辑包括：
        /// <list type="bullet">
        /// <item><description>起始点或结束点是否在矩形内部；</description></item>
        /// <item><description>调用 <see cref="Collision.CheckAABBvLineCollision"/> 进行带宽度的线段-矩形相交测试。</description></item>
        /// </list>
        /// </remarks>
        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
        public static void ShimmerEach(this int type, int shimmerType)
        {
            ItemID.Sets.ShimmerTransformToItem[type] = shimmerType;
            ItemID.Sets.ShimmerTransformToItem[shimmerType] = type;
        }
        public static void ShimmerFrom(this int type, int shimmerFrom)
        {
            ItemID.Sets.ShimmerTransformToItem[shimmerFrom] = type;
        }
        public static void ShimmerTo(this int type, int shimmerTo)
        {
            ItemID.Sets.ShimmerTransformToItem[type] = shimmerTo;
        }

        public static void ShimmerFrom(this int type, params int[] shimmerFrom)
        {
            foreach (var id in shimmerFrom)
                ItemID.Sets.ShimmerTransformToItem[id] = type;
        }
        public static void ShimmerEach<T>(this int type) where T : ModItem => ShimmerEach(type, ItemType<T>());
        public static Vector2 ToMouseVector2(this Player player, Vector2? safeValue = null)
        {
            Vector2 safe = safeValue ?? Vector2.UnitX;
            return (player.LocalMouseWorld() - player.MountedCenter).SafeNormalize(safe);
        }
        public static Vector2 ToMouseVector2(this Vector2 center, Vector2? safeValue = null)
        {
            Vector2 safe = safeValue ?? Vector2.UnitX;
            return (Main.MouseWorld - center).SafeNormalize(safe);

        }
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float speed = 1f) => srcVel.ToSafeNormalize().RotatedBy(Main.rand.NextFloat(randRads) * Main.rand.NextBool().ToDirectionInt()) * speed;
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float minSpeed, float maxSpeed)
        {
            return srcVel.ToRandVelocity(randRads, Main.rand.NextFloat(minSpeed, maxSpeed));
        }
        public static Vector2 ToSafeNormalize(this Vector2 srcVel, Vector2? what = null) => srcVel.SafeNormalize(what ?? Vector2.UnitX);

        public static string ToPercent(this float value)
        {
            float value2 = value * 100f;
            return $"{(int)value2}%";
        }
        public static int SetAxePower(this int percent) => percent / 5;
        /// <summary>
        /// 将玩家的鼠标向量获取的值限制在1080p屏幕内，类似于天顶剑
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector2 ToClampMouseVector2(this Player player)
        {
            Vector2 mouseWorld = player.LocalMouseWorld();
            mouseWorld.X = mouseWorld.X >= player.MountedCenter.X ? MathF.Min(mouseWorld.X, player.MountedCenter.X + 960f) : MathF.Max(mouseWorld.X, player.MountedCenter.X - 960f);
            mouseWorld.Y = mouseWorld.Y >= player.MountedCenter.Y ? MathF.Min(mouseWorld.Y, player.MountedCenter.Y + 540f) : MathF.Max(mouseWorld.Y, player.MountedCenter.Y - 540f);
            return mouseWorld;
        }
        /// <summary>
        /// 治疗玩家
        /// 重载了一个强制治疗的方法用于强行给玩家治疗
        /// 即绕过原版“Heal”的方法
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="forcedHeal"></param>
        public static void HealDirectly(this Player player, int amount, bool forcedHeal = true)
        {
            if (forcedHeal)
            {
                player.statLife += amount;
                if (Main.myPlayer == player.whoAmI)
                    player.HealEffect(amount);
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
            }
            else
                player.Heal(amount);
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
        public static bool IsItemName(this DrawableTooltipLine line) => line.Name == "ItemName" && line.Mod == "Terraria";
        /// <summary>
        /// 控制玩家的手臂旋转，可选择前臂、后臂或双臂。
        /// </summary>
        /// <param name="player">目标玩家。</param>
        /// <param name="armRot">手臂的目标旋转角度（世界坐标系下的弧度，通常为武器/物品的方向）。</param>
        /// <param name="specificArm">
        /// 指定要控制的手臂：<br/>
        /// -  <c>0</c>（默认）：同时控制前臂和后臂。<br/>
        /// -  <c>&gt;0</c>（正数）：仅控制前臂（CompositeArmFront）。<br/>
        /// -  <c>&lt;0</c>（负数）：仅控制后臂（CompositeArmBack）。
        /// </param>
        /// <param name="customArmRot">
        /// 手臂的本地基础偏移角度（弧度），用于调整手臂默认朝向。实际最终旋转角度 = <paramref name="armRot"/> - <paramref name="customArmRot"/>。<br/>
        /// 默认值为 <see cref="PiOver2"/>（90°），表示手臂默认指向正右方时，需要减去此偏移以获得正确的贴图旋转。
        /// </param>
        public static void ControlPlayerArm(this Player player, float armRot, int specificArm = 0, float customArmRot = PiOver2)
        {
            float armType = Math.Sign(specificArm);
            switch (armType)
            {
                case 1:
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
                case -1:
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
                default:
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRot - customArmRot);
                    break;
            }
        }
        public static Vector2 GetToMouseVector2(this Player player, Vector2 BeginPos)
        {
            Vector2 vector = Main.MouseWorld - BeginPos;
            vector = vector.SafeNormalize(Vector2.UnitX);
            return vector;
        }
        public static Tile GetTileCoord(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return new Tile();

            return Main.tile[x, y];
        }
        public static int ApplyWeaponAttackSpeed(this Player player, Item item, int time, int Min)
        {
            float a = player.GetWeaponAttackSpeed(item);
            float Mult = 1f / a;
            int RealAttack = (int)(time * Mult);
            if (RealAttack < Min)
                return Min;
            else
                return RealAttack;
        }
        public static void SetUpNoUseGraphicItem(this Item item, bool channel = false, bool autoReuse = true)
        {
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = channel;
            item.autoReuse = autoReuse;
        }
        public static bool IsLegalFriendlyProj(this Projectile proj, DamageClass damageClass = null)
        {
            if (damageClass != null)
            {
                return proj.DamageType == damageClass && proj.damage > 5 && proj.friendly && !proj.hostile;
            }
            else
                return proj.damage > 5 && proj.friendly && !proj.hostile;

        }
        public static float ClampOscillation(this Projectile proj, float curOscillation, float speedValue)
        {
            return ClampOscillation(curOscillation, speedValue, proj.MaxUpdates);
        }
        public static float ClampOscillation(float curOscillation, float speedValue, int updates = 1)
        {
            float newOsci = curOscillation + ToRadians(speedValue) / updates;
            if (newOsci >= ToRadians(360f))
                newOsci = ToRadians(-360f);
            return newOsci;
        }
    }
}
