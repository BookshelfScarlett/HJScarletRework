using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class CombatSlot : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.accessory = true;
            Item.value = Item.buyPrice(1, 50, 0, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().combatSlot = true;
        }
    }
}
