using HJScarletRework.Globals.Enums;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletWeapon : ModItem,ILocalizedModType
    {
        public virtual ClassCategory Category { get; }
        public new string LocalizationCategory => $"Weapons.{Category}";
        public override void SetDefaults()
        {
            Item.DamageType = GetDamageClass;
            ExSD();
        }
        public virtual void ExSD() { }
        private DamageClass GetDamageClass
        {
            get
            {
                return Category switch
                {
                    ClassCategory.Melee => DamageClass.Melee,
                    ClassCategory.Ranged => DamageClass.Ranged,
                    ClassCategory.Magic => DamageClass.Magic,
                    ClassCategory.Summon => DamageClass.Summon,
                    _ => DamageClass.Generic,
                };
            }
        }
    }
}
