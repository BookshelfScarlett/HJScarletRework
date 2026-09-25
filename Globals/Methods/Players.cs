using HJScarletRework.Buffs;
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
        public static bool CanUseHoldout(this Player player, int itemID) => !player.dead && !player.CCed && player.IsHolding(itemID);
        public static bool CanUseHoldout<T>(this Player player) where T : ModItem => !player.dead && !player.CCed && player.IsHolding(ItemType<T>());
        public static bool IsInInventory(this Player player) => Main.hoverItemName != "";
        public static bool IsInwater(this Player player) => Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
        /// <summary>
        /// 计算玩家对指定伤害类型的基础伤害加成比例（例如返回 0.25 表示伤害提升了 25%）。
        /// </summary>
        /// <param name="player">要计算的玩家实例</param>
        /// <param name="originalDamage">基础伤害值（通常为物品或弹幕的原始伤害）</param>
        /// <param name="damageClass">伤害类型（如近战、远程、魔法等）</param>
        /// <returns>额外伤害比例，即实际伤害相对于原始伤害的增幅（0 表示无加成，0.5 表示加成 50%）。</returns>
        public static float GetDamageBonusRatio(this Player player, int originalDamage, DamageClass damageClass) => (player.GetTotalDamage(damageClass).ApplyTo(originalDamage) - originalDamage) / (float)originalDamage;
        /// <summary>
        /// 计算玩家对指定伤害类型的基础伤害加成比例（例如返回 0.25 表示伤害提升了 25%）。
        /// 这是一个重载方案，可以让你输入指定的目标伤害，无视damageclass的加成规则
        /// </summary>
        /// <param name="player">要计算的玩家实例</param>
        /// <param name="originalDamage">基础伤害值（通常为物品或弹幕的原始伤害）</param>
        /// <param name="damageClass">伤害类型（如近战、远程、魔法等）</param>
        /// <returns>额外伤害比例，即实际伤害相对于原始伤害的增幅（0 表示无加成，0.5 表示加成 50%）。</returns>

        public static float GetDamageBonusRatio(int targetDamage, int originalDamage) => ((float)targetDamage - originalDamage) / (float)originalDamage;
        public static void ApplyNoKnockbackBuff(this Player player, int frame) => player.AddBuff(BuffType<AntiKnockbackBuff>(), frame);
    }
}
