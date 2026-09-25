using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Instances.Items;
using System;
using Terraria;
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
                player.itemRotation = player.itemRotation.AngleLerp(targetRotation - MathHelper.ToRadians(rotationOffset) + MathHelper.Pi, rotationSpeed);
            else
                player.itemRotation = player.itemRotation.AngleLerp(targetRotation + MathHelper.ToRadians(rotationOffset), rotationSpeed);
        }
        public static void ApplyPrefixToThis(ref Item item, int prefixID)
        {
            if (item.CanApplyPrefix(prefixID))
                item.Prefix(prefixID);
        }

    }
}
