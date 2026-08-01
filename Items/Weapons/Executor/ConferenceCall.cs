using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class ConferenceCall : ExecutorWeaponClass
    {
        public static int BulletsPerShot = 5;
        public override int ExecutionProgress => BulletsPerShot * 10;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 100;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.SetUpNoUseGraphicItem(true);
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 20;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
