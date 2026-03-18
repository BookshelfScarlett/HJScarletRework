using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Executor
{
    public class ExecutorGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int FocusTimeReduce = 0;
        public float FocusDamageMult = 0;
        public override GlobalItem Clone(Item from, Item to)
        {
            ExecutorGlobalItem executorGlobalItem = (ExecutorGlobalItem)base.Clone(from, to);
            executorGlobalItem.FocusDamageMult = FocusDamageMult;
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
            FocusDamageMult = 0;
        }
    }
    public class Fake : ExecutorPrefixs
    {
        public override float DamageMult => 0.90f;
        public override int CritAdd => 15;
        public override int ArmorPenetrationAdd => 15;
    }
    public class Digital : ExecutorPrefixs
    {
        public override float UseTimeMult => 0.85f;
        public override int CritAdd => -20;
        public override float FocusDamageMult => 1.15f;
    }
    public class Mysterious : ExecutorPrefixs
    {

        public override int CritAdd => -10;
        public override float UseTimeMult => 0.80f;
    }
    public class Phantasmic : ExecutorPrefixs
    {
        public override float DamageMult => 1.15f;
        public override float FocusDamageMult => 1.10f;
        public override float UseTimeMult => 0.9f;
        public override int ArmorPenetrationAdd => 10;
        public override int CritAdd => 5;
    }
    public class Evolution : ExecutorPrefixs
    {
        public override float DamageMult => 1.10f;
        public override float FocusDamageMult => 1.05f;
        public override float UseTimeMult => 0.95f;
        public override int ArmorPenetrationAdd => 5;
        public override int CritAdd => 3;

    }
    public class Foreigner : ExecutorPrefixs
    {
        public override int ArmorPenetrationAdd => 50;
        public override int CritAdd => 15;
        public override float FocusDamageMult => 1.10f;
    }
    public class Alterego : ExecutorPrefixs
    {
        public override int ArmorPenetrationAdd => 10;
        public override int CritAdd => -10;
        public override float UseTimeMult => 0.90f;
        public override float FocusDamageMult => 0.80f;
    }
    public abstract class ExecutorPrefixs : ModPrefix, ILocalizedModType
    {
        public new string LocalizationCategory => $"DamageClasses.ExecutorDamageClass.Prefixs";
        public virtual float DamageMult => 1f;
        public virtual float UseTimeMult => 1f;
        public virtual int CritAdd => 0;
        public virtual int ArmorPenetrationAdd => 0;
        public virtual float FocusDamageMult => 0f;
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        /// <summary>
        /// 使用CountAsClass会导致射手也能roll代行者的词缀
        /// 这里需要直接等于，反正我们也只有一个代行者伤害类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool CanRoll(Item item) => item.DamageType == ExecutorDamageClass.Instance && item.maxStack == 1 && GetType() != typeof(ExecutorPrefixs);
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = DamageMult;
            useTimeMult = UseTimeMult;
            critBonus = CritAdd;
        }
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= (1f + FocusDamageMult);
        }
        public override void Apply(Item item)
        {
            if(item.DamageType == ExecutorDamageClass.Instance && item.TryGetGlobalItem<ExecutorGlobalItem>(out var result))
            {
                item.ArmorPenetration += ArmorPenetrationAdd;
                result.FocusDamageMult = FocusDamageMult;
            }
        }
        internal const string FocusTimeNameID = "HJScarletRework:PrefixFocusTime";
        internal const string FocusDamageNameID = "HJScarletRework:PrefixFocusDamage";
        internal const string ArmorPenetrationNameID = "HJScarletRework:PrefixArmorPenetration";
        public string FocusTimeValue => Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.Prefixs.FocusTimeLine").ToLangValue();
        public string FocusDamageValue  => Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.Prefixs.FocusDamageLine").ToLangValue();
        public string ArmorPenetrationValue => Mod.GetLocalizationKey("DamageClasses.ExecutorDamageClass.Prefixs.ArmorPenetrationLine").ToLangValue();
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (FocusDamageMult != 0f)
            {
                string inserValue = FocusDamageMult >= 0f ? "+" : "-";
                string realValue = ((FocusDamageMult * 100f) - 100).ToString("N0");
                TooltipLine newLine = new(Mod, FocusDamageNameID, FocusDamageValue.ToFormatValue($"{inserValue}{realValue}"))
                {
                    IsModifier = true,
                    IsModifierBad = FocusDamageMult < 1f
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
                    IsModifierBad = FocusDamageMult < 0f
                };
                yield return newLine;
            }
            yield break;
        }
    }
}
