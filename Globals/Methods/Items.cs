using HJScarletRework.Globals.Instances.Items;
using Terraria;
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
        public static void SetUpItemShoot(this Item item, int shootID, float itemShootSpeed, float knocback = 3f)
        {
            item.shoot = shootID;
            item.shootSpeed = itemShootSpeed;
            item.knockBack = knocback;
        }
        public static void SetUpItemShoot<T>(this Item item, float itemShootSpeed, float knocback = 3f) where T :ModProjectile
        {
            item.shoot = ProjectileType<T>();
            item.shootSpeed = itemShootSpeed;
            item.knockBack = knocback;
        }
        public static bool IsLegal(this Item item)
        {
            return !item.IsAir && item is not null;
        }

    }
}
