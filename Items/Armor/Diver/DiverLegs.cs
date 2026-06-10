using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Diver
{
    [AutoloadEquip(EquipType.Legs)]
    public class DiverLegs : HJScarletArmor
    {
        public override void ExSD()
        {
            Item.defense = 45;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .25f;
            player.aggro += 500;
        }
    }
}
