using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.RedDragonKnight;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonHunter
{
    [AutoloadEquip(EquipType.Body)]
    public class DragonHunterBody : HJScarletArmor
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
        public int Crit = 30;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit + "%");
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += Crit;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RedDragonKnightBody>().
                AddIngredient<SunlightGel>(6).
                AddIngredient<EssenceofTime>(6).
                AddIngredient<EssenceofLife>(6).
                AddIngredient<EssenceofMatter>(6).
                AddTile(FinalAnvilTile).
                Register();
        }

    }
}
