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
        public int FocusTimeReduce = 0;
        public float ExecutionDamageMult = 1;
        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            return base.ChoosePrefix(item, rand);
        }
        public override GlobalItem Clone(Item from, Item to)
        {
            ExecutorGlobalItem executorGlobalItem = (ExecutorGlobalItem)base.Clone(from, to);
            executorGlobalItem.ExecutionDamageMult = ExecutionDamageMult;
            executorGlobalItem.FocusTimeReduce = FocusTimeReduce;
            return executorGlobalItem;
        }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.DamageType == ExecutorDamageClass.Instance;
        }
        public override void PreReforge(Item item)
        {
            FocusTimeReduce = 0;
            ExecutionDamageMult = 0;
        }
    }
    public class Fake : ExecutorPrefixs
    {
        public override float DamageMult => 0.50f;
        public override int CritAdd => -15;
        public override float ExecutionDamageMult => 1f;
    }
    public class Digital : ExecutorPrefixs
    {
        public override float DamageMult => 1.10f;
        public override int ArmorPenetrationAdd => 5;
        public override float ExecutionDamageMult => 1f;
    }
    public class Mysterious : ExecutorPrefixs
    {
        public override int CritAdd => 10;
        public override int ArmorPenetrationAdd => 10;
    }
    public class Phantasmic : ExecutorPrefixs
    {
        public override float DamageMult => 1.10f;
        public override float ExecutionDamageMult => 1f;
        public override float UseTimeMult => 0.95f;
        public override int ArmorPenetrationAdd => 10;
        public override int CritAdd => 5;
    }
    public class Evolutional : ExecutorPrefixs
    {
        public override float DamageMult => 1.05f;
        public override float ExecutionDamageMult => 1;
        public override float UseTimeMult => 1f;
        public override int ArmorPenetrationAdd => 5;
        public override int CritAdd => 3;

    }
    public class Foreigner : ExecutorPrefixs
    {
        public override int ArmorPenetrationAdd => 30;
        public override int CritAdd => 30;
        public override float ExecutionDamageMult => 1f;
        public override float UseTimeMult => 1.5f;
    }
    public class Alterego : ExecutorPrefixs
    {
        public override int ArmorPenetrationAdd => 15;
        public override int CritAdd => -30;
        public override float UseTimeMult => 0.70f;
        public override float ExecutionDamageMult => 0.50f;
    }
    public abstract class ExecutorPrefixs : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => $"DamageClasses.ExecutorDamageClass.Prefixs";
        public virtual float DamageMult => 1f;
        public virtual float UseTimeMult => 1f;
        public virtual int CritAdd => 0;
        public virtual int ArmorPenetrationAdd => 0;
        public virtual float ExecutionDamageMult => 1f;
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        /// <summary>
        /// 使用CountAsClass会导致射手也能roll代行者的词缀
        /// 这里需要直接等于，反正我们也只有一个代行者伤害类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool CanRoll(Item item) => item.DamageType == ExecutorDamageClass.Instance;
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = DamageMult;
            useTimeMult = UseTimeMult;
            critBonus = CritAdd;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + DamageMult;
        }
        public override void Apply(Item item)
        {
            if (item.DamageType == ExecutorDamageClass.Instance && item.TryGetGlobalItem<ExecutorGlobalItem>(out var result))
            {
                item.ArmorPenetration += ArmorPenetrationAdd;
                result.ExecutionDamageMult = ExecutionDamageMult;
            }
        }
        internal const string FocusDamageNameID = "HJScarletRework:PrefixExecutionDamage";
        internal const string ArmorPenetrationNameID = "HJScarletRework:PrefixArmorPenetration";
        public string ExecutionDamageValue => Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.Prefixs.ExecutionDamageLine").ToLangValue();
        public string ArmorPenetrationValue => Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.Prefixs.ArmorPenetrationLine").ToLangValue();
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

            if (ArmorPenetrationAdd != 0)
            {
                string insertValue = ArmorPenetrationAdd > 0 ? "+" : string.Empty;
                string realValue = $"{insertValue}{ArmorPenetrationAdd}";

                TooltipLine newLine = new(Mod, ArmorPenetrationNameID, ArmorPenetrationValue.ToFormatValue(realValue))
                {
                    IsModifier = true,
                    IsModifierBad = ArmorPenetrationAdd< 0f
                };
                yield return newLine;
            }
            yield break;
        }
    }
}
