using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Materials
{
    public class DisasterEssence : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Materials;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
