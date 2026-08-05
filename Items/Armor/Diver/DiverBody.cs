using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{
    [AutoloadEquip(EquipType.Body)]
    public class DiverBody : HJScarletArmor
    {
        public float CritDamage = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.defense = 8;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
        }
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
    }
}
