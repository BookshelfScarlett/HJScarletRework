using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class DescriptionPaper : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
    }
}
