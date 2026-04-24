using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletArmor : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public virtual bool SetUpArmorSet => false;
        public virtual int[] ArmorSlots => null;
        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            if (SetUpArmorSet)
            {
                if (ArmorSlots.Length < 3)
                    return head.type == ArmorSlots[0] && body.type == ArmorSlots[1];
                else
                    return head.type == ArmorSlots[0] && body.type == ArmorSlots[1] && legs.type == ArmorSlots[2];
            }
            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player)
        {
            if (!SetUpArmorSet)
                return;
            string setBonusPath = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBonus");
            UpdateArmorSetBetter(player, setBonusPath);
        }

        public virtual void UpdateArmorSetBetter(Player player, string setBonusPath) { }
    }
}
