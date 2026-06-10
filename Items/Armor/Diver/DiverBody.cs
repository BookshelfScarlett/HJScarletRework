using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{
    [AutoloadEquip(EquipType.Body)]
    public class DiverBody : HJScarletArmor
    {
        public override void ExSD()
        {
            Item.defense = 60;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += 0.15f;
            player.aggro += 500;
        }
    }
}
