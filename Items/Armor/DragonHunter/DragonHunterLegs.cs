using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.RedDragonKnight;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonHunter
{
    [AutoloadEquip(EquipType.Legs)]
    public class DragonHunterLegs :HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.defense = 35;
        }
        public float MoveSpeed = .30f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeed.ToPercent());
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
