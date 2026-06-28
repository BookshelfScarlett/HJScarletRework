using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Accessories
{
    public class PowerLily : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override string Texture => HJScarletTexture.Specific_DialectCube.Path;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().ItemBelongTo = Globals.Enums.ItemBelong.Donator;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
    }
}
