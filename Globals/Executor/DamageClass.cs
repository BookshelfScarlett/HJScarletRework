using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Executor
{
    public class ExecutorDamageClass : DamageClass, ILocalizedModType
    {
        internal static ExecutorDamageClass Instance;
        public override void Load()
        {
            Instance = this;
        }
        public override void Unload()
        {
            Instance = null;
        }
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Ranged)
                return new StatInheritanceData
                (
                    damageInheritance: 1.0f,
                    critChanceInheritance: 1.0f,
                    attackSpeedInheritance: 1.0f,
                    armorPenInheritance: 10.0f,
                    knockbackInheritance: 0.5f
                );
            else if (damageClass == Generic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            if (damageClass == Ranged)
                return true;
            return false;
        }
        public override bool UseStandardCritCalcs => true;
        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            return true;
        }
    }
}
