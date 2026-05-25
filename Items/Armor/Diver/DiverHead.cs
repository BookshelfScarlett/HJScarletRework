using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{
    public abstract class HJScarletArmor : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public virtual bool SetUpArmorSet => false;
        public virtual int[] ArmorSlots => null;
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
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

    [AutoloadEquip(EquipType.Head)]
    public class DiverHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemType<DiverBody>(),ItemType<DiverLegs>()];
        public override bool SetUpArmorSet => true;
        public override void ExSD()
        {
            Item.defense = 5;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            base.UpdateArmorSetBetter(player, setBonusPath);
        }
        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
        }
    }
}
