using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class CycleMadness : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.rare = ItemRarityID.Purple;
            Item.defense = 5;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
