using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.RedDragonKnight
{
    [AutoloadEquip(EquipType.Legs), LegacyName("DragonSlayerLegs")]
    public class RedDragonKnightLegs : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.defense = 30;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public float MoveSpeed = .20f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeed.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeed;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UniversalCube>(4).
                AddIngredient(ItemID.LunarBar, 4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

    }
}
