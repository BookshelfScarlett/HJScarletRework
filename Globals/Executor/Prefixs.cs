using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace HJScarletRework.Globals.Executor
{
    public class ExecutorGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int DefenseBuff = 0;
        public float ExecutionDamageMult = 1;
        public float CritDamageMult = 0;
        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            return base.ChoosePrefix(item, rand);
        }
        public override GlobalItem Clone(Item from, Item to)
        {
            ExecutorGlobalItem executorGlobalItem = (ExecutorGlobalItem)base.Clone(from, to);
            executorGlobalItem.ExecutionDamageMult = ExecutionDamageMult;
            return executorGlobalItem;
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.DamageType == ExecutorDamageClass.Instance;
        }
        public override void HoldItem(Item item, Player player)
        {
            player.HJScarlet().critDamageExecutor += CritDamageMult;
            player.statDefense += DefenseBuff;

        }
        public override void PreReforge(Item item)
        {
            DefenseBuff = 0;
            ExecutionDamageMult = 0;
            CritDamageMult = 0;
        }
    }
    public class Phantasmic : ExecutorPrefixs
    {
        public override float DamageMult => 1.12f;
        public override float CritDamageAdd => .10f;
        public override int CritAdd => 5;
        public override float KnockbackMult => 1.1f;
    }
    public class Evolutional : ExecutorPrefixs
    {
        public override float DamageMult => 1.05f;
        public override float CritDamageAdd => .05f;
        public override int CritAdd => 5;
        public override float KnockbackMult => 1.05f;
    }
    public class Digital : ExecutorPrefixs
    {
        public override float DamageMult => 1.10f;
        public override float CritDamageAdd => .10f;
    }

    public class Mysterious : ExecutorPrefixs
    {
        public override int CritAdd => 10;
        public override float CritDamageAdd => .10f;
        public override float KnockbackMult => .90f;
    }
    public class Foreigner : ExecutorPrefixs
    {
        public override int CritAdd => 30;
        public override float CritDamageAdd => -0.5f;
        public override float ExecutionDamageMult => .75f;
        public override float KnockbackMult => 1f;
    }
    public class Alterego : ExecutorPrefixs
    {
        public override int CritAdd => -30;
        public override float CritDamageAdd => .50f;
        public override float ExecutionDamageMult => 0.25f;
        public override float KnockbackMult => 1f;
    }
    public class Fake : ExecutorPrefixs
    {
        public override float DamageMult => 0.90f;
        public override int CritAdd => -10;
        public override float ExecutionDamageMult => 0.90f;
        public override float KnockbackMult => .90f;
        public override int DefenseAdded => -10;
    }

    public abstract class ExecutorPrefixs : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => $"ExecutorDamageClass.Prefixs";
        /// <summary>
        /// 伤害加成
        /// </summary>
        public virtual float DamageMult => 1f;
        /// <summary>
        /// 使用时间加成
        /// </summary>
        public virtual float UseTimeMult => 1f;
        /// <summary>
        /// 击退力加成
        /// </summary>
        public virtual float KnockbackMult => 1f;
        /// <summary>
        /// 暴击加成
        /// </summary>
        public virtual int CritAdd => 0;
        /// <summary>
        /// 防御加成
        /// </summary>
        public virtual int DefenseAdded => 0;
        /// <summary>
        /// 处决伤害加成
        /// </summary>
        public virtual float ExecutionDamageMult => 1f;
        /// <summary>
        /// 暴击伤害加成
        /// </summary>
        public virtual float CritDamageAdd => 0;
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        /// <summary>
        /// 使用CountAsClass会导致射手也能roll代行者的词缀
        /// 这里需要直接等于，反正我们也只有一个代行者伤害类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool CanRoll(Item item) => item.DamageType.CountsAsClass<ExecutorDamageClass>();
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = DamageMult;
            useTimeMult = UseTimeMult;
            critBonus = CritAdd;
            knockbackMult = KnockbackMult;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + DamageMult;
        }
        public override void Apply(Item item)
        {
            if (item.DamageType == ExecutorDamageClass.Instance && item.TryGetGlobalItem<ExecutorGlobalItem>(out var result))
            {
                result.DefenseBuff += DefenseAdded;
                result.ExecutionDamageMult = ExecutionDamageMult;
                result.CritDamageMult = CritDamageAdd;
            }
        }
        internal const string FocusDamageNameID = "HJScarletRework:PrefixExecutionDamage";
        internal const string ArmorPenetrationNameID = "HJScarletRework:PrefixArmorPenetration";
        internal const string CritDamageID = "HJScarletRework:PrefixCritDamage";
        public string ExecutionDamageValue => Mod.GetLocalizationKey("ExecutorDamageClass.Prefixs.ExecutionDamageLine").ToLangValue();
        public string ArmorPenetrationValue => Mod.GetLocalizationKey("ExecutorDamageClass.Prefixs.ArmorPenetrationLine").ToLangValue();
        public string CritDamageValue => Mod.GetLocalizationKey("ExecutorDamageClass.Prefixs.CritDamageLine").ToLangValue();
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (ExecutionDamageMult != 1f)
            {
                string realValue = (ExecutionDamageMult * 100f - 100).ToString("N0");
                string inserValue = ExecutionDamageMult >= 1f ? "+" : string.Empty;
                TooltipLine newLine = new(Mod, FocusDamageNameID, ExecutionDamageValue.ToFormatValue($"{inserValue}{realValue}"))
                {
                    IsModifier = true,
                    IsModifierBad = ExecutionDamageMult < 1f
                };
                yield return newLine;
            }
            if (CritDamageAdd != 0f)
            {
                string mult = (CritDamageAdd * 100f).ToString("N0");
                string insertValue = (CritDamageAdd * 100f) > 0 ? "+" : string.Empty;
                string realValue = $"{insertValue}{mult}%";

                TooltipLine newLine = new(Mod, CritDamageID, CritDamageValue.ToFormatValue(realValue))
                {
                    IsModifier = true,
                    IsModifierBad = CritDamageAdd < 0f
                };
                yield return newLine;


            }

            if (DefenseAdded != 0)
            {
                string insertValue = DefenseAdded > 0 ? "+" : string.Empty;
                string realValue = $"{insertValue}{DefenseAdded}";

                TooltipLine newLine = new(Mod, ArmorPenetrationNameID, ArmorPenetrationValue.ToFormatValue(realValue))
                {
                    IsModifier = true,
                    IsModifierBad = DefenseAdded < 0f
                };
                yield return newLine;
            }
            yield break;
        }
    }
}
