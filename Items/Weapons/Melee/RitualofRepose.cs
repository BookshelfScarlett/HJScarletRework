using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.List;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class RitualofRepose : ThrownSpearClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override bool NotHomewardJourneySpear => true;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Hallowed);
        }
        public override void ExSD()
        {
            Item.damage = 1546;
        }
        public override Color MainTooltipColor => Color.LightGoldenrodYellow;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient<EssenceofNothingness>(10).
                AddIngredient<EssenceofDeath>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
