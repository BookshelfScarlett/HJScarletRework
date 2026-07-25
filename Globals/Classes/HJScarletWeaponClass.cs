using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletWeapon : ModItem, ILocalizedModType
    {
        public virtual EnumDamageClass Category { get; }
        public new string LocalizationCategory => $"Weapons.{Category}";
        public override string Texture => $"HJScarletRework/Assets/Texture/Items/Weapons/{GetType().Name}";
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.DamageType = GetDamageClass;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public virtual void ExSD() { }
        private DamageClass GetDamageClass
        {
            get
            {
                return Category switch
                {
                    EnumDamageClass.Melee => DamageClass.Melee,
                    EnumDamageClass.Ranged => DamageClass.Ranged,
                    EnumDamageClass.Magic => DamageClass.Magic,
                    EnumDamageClass.Summon => DamageClass.Summon,
                    _ => DamageClass.Generic,
                };
            }
        }
    }
}
