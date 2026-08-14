using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        /// <summary>
        /// 计算防御力加成，返回增加的防御力数值
        /// <br><paramref name="multiplier"/>为比率，如果低于1则返回0</br>
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="multiplier"></param>
        /// <param name="noClamp">是否不进行 clamp 操作</param>
        /// <returns></returns>
        public static int DefenseMultiplier(this Player owner, float multiplier, bool noClamp = false)
        {
            float ratios = multiplier - 1f;
            if (ratios <= 0f && !noClamp)
                ratios = 0f;
            return (int)(owner.statDefense * ratios);
        }
        public static bool IsHolding<T>(this Player player) where T : ModItem => IsHolding(player, ItemType<T>());
        public static bool IsHolding(this Player player, int itemID) => player.HeldItem.type == itemID;
        public static bool IsInInventory(this Player player) => Main.hoverItemName != "";
        //public static int CountsProj()
    }
}
