using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.ReVisual.Class;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static void SetUpRarityPrice(this Item item, int rarityID)
        {
            item.rare = rarityID;
            item.value = HJScarletShopPrice.ConvertedToValue(rarityID);
        }
        public static void SetUpItemUseTime(this Item item, int useStyle, int itemUseTime, int? itemUseAnimation = null)
        {
            item.useStyle = useStyle;
            item.useTime = itemUseTime;
            item.useAnimation = itemUseAnimation ?? itemUseTime;
        }
        public static void SetUpItemShoot<T>(this Item item, float itemShootSpeed, float knocback = 3f) where T : ModProjectile
        {
            item.shoot = ProjectileType<T>();
            item.shootSpeed = itemShootSpeed;
            item.knockBack = knocback;
        }
        public static bool IsLegal(this Item item)
        {
            return !item.IsAir && item is not null;
        }
        public static bool IsTool(this Item item)
        {
            return item.IsLegal() && (item.pick > 0 || item.axe > 0 || item.hammer > 0);
        }
        public static bool IsWeapon(this Item item)
        {
            return (!item.IsTool() && (item.damage > 0 || item.type == ItemID.CoinGun) || (ScarletItemIDSets.CountAsWeapon[item.type]));
        }
        public static bool IsExecutorWeapon(this Item item)
        {
            if (!item.IsLegal())
                return false;
            return HJScarletList.ExecuteRequests.ContainsKey(item.type);
        }
        /// <summary>
        /// 让原版的手持也可以像手持弹幕一样旋转<br/>
        /// 随便找一个每帧调用的方法调用即可<br/>
        /// </summary>
        public static void NoHeldProjUpdateAim(this Player player, float rotationOffset = 0f, float rotationSpeed = 1f)
        {
            player.ChangeDir(Math.Sign((player.LocalMouseWorld() - player.Center).X));

            Vector2 aimVect = player.LocalMouseWorld() - player.Center;
            aimVect.SafeNormalize(Vector2.UnitX);

            float targetRotation = aimVect.ToRotation();

            if (player.LocalMouseWorld().X < player.Center.X)
                player.itemRotation = player.itemRotation.AngleLerp(targetRotation - ToRadians(rotationOffset) + Pi, rotationSpeed);
            else
                player.itemRotation = player.itemRotation.AngleLerp(targetRotation + ToRadians(rotationOffset), rotationSpeed);
        }
        public static void HoldoutProjUpdateAim(this Item item, Projectile proj, float recoilPower, Vector2? offset = null)
        {
            if (proj.ModProjectile is ReVisualRecoilProj holdout)
            {
                holdout.SetUpHoldoutData(item.type, recoilPower, item.useAnimation, offset ?? Vector2.Zero);
            }
        }
        public static void ApplyPrefixToThis(ref Item item, int prefixID)
        {
            if (item.CanApplyPrefix(prefixID))
                item.Prefix(prefixID);
        }
        public static int ScarletSpawnItem(IEntitySource source, Vector2 pos, Rectangle rec, int item, int stack = 1)
        {
            int number = Item.NewItem(source, (int)pos.X, (int)pos.Y, rec.Width, rec.Height, item, stack, noBroadcast: false, -1);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
            return number;
        }
        /// <summary>
        /// 正确设置玩家手持物品的旋转与位置，替你完成那些烦人的数学计算，因为原版在这方面的处理完全不统一！
        /// 以下所有计算都假设玩家朝右。所有翻转相关的逻辑都在本方法内部自动处理。
        /// Credit from Calamity
        /// </summary>
        /// <param name="player">要设置持握样式的玩家</param>
        /// <param name="desiredRotation">期望的物品旋转角度</param>
        /// <param name="desiredPosition">期望的物品位置</param>
        /// <param name="spriteSize">物品精灵图的大小（用于计算）</param>
        /// <param name="rotationOriginFromCenter">旋转原点相对于精灵图中心的偏移</param>
        /// <param name="noSandstorm">是否禁用沙尘暴跳跃时产生的旋转特效</param>
        /// <param name="flipAngle">角度是否应随玩家朝向翻转，还是直接旋转 180 度</param>
        /// <param name="stepDisplace">行走动画中物品是否应随玩家身高变化产生位移？</param>
        public static void CleanHoldStyle(Player player, float desiredRotation, Vector2 desiredPosition, Vector2 spriteSize, Vector2? rotationOriginFromCenter = null, bool noSandstorm = false, bool flipAngle = false, bool stepDisplace = true)
        {
            if (noSandstorm)
                player.sandStorm = false;

            // 因为 Vector2.Zero 不是编译期常量，无法直接作为默认参数
            if (rotationOriginFromCenter == null)
                rotationOriginFromCenter = Vector2.Zero;

            Vector2 origin = rotationOriginFromCenter.Value;
            // 翻转原点的 X 坐标，因为玩家朝左时精灵图会被水平翻转。
            origin.X *= player.direction;
            // 另外，如果玩家处于反向重力，也要翻转原点的 Y 坐标。
            origin.Y *= player.gravDir;

            player.itemRotation = desiredRotation;

            if (flipAngle)
                player.itemRotation *= player.direction;
            else if (player.direction < 0)
                player.itemRotation += Pi;

            // 这可以把物品锚定在其精灵图左侧中部，围绕该点旋转
            //Vector2 consistentLeftAnchor = (player.itemRotation).ToRotationVector2() * -10f * player.direction;

            // 这会把物品锚定在其精灵图中心，围绕中心旋转。
            Vector2 consistentCenterAnchor = player.itemRotation.ToRotationVector2() * (spriteSize.X / -2f - 10f) * player.direction;

            // 这会把物品平移，使其改为围绕设定的原点旋转
            Vector2 consistentAnchor = consistentCenterAnchor - origin.RotatedBy(player.itemRotation);

            // 精灵图还需要再偏移其大小的一半。
            Vector2 offsetAgain = spriteSize * -0.5f;

            Vector2 finalPosition = desiredPosition + offsetAgain + consistentAnchor;

            // 处理玩家行走时因身高变化带来的额外偏移
            if (stepDisplace)
            {
                int frame = player.bodyFrame.Y / player.bodyFrame.Height;
                if ((frame > 6 && frame < 10) || (frame > 13 && frame < 17))
                {
                    finalPosition -= Vector2.UnitY * 2f;
                }
            }

            player.itemLocation = finalPosition + new Vector2(spriteSize.X * 0.5f, 0);
        }
    }
}
