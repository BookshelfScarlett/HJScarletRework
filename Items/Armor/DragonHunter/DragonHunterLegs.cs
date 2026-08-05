using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.DragonSlayer;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Armor.DragonHunter
{
    public class DragonHunterLegs :HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.defense = 40;
        }
        public float MoveSpeed = .30f;
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeed;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RedDragonKnightLegs>().
                AddIngredient<SunlightGel>(2).
                AddIngredient<EssenceofTime>(2).
                AddIngredient<EssenceofLife>(2).
                AddIngredient<EssenceofMatter>(2).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
