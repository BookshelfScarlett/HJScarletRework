using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonSlayer
{
    [AutoloadEquip(EquipType.Body)]
    public class DragonSlayerBody : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void Load()
        {
            //EquipLoader.AddEquipTexture(Mod, )
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 35;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.rare = RarityType<DisasterRarity>();

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            return true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.lifeRegen += 16;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar, 15).
                AddIngredient(ItemID.FragmentVortex, 15).
                AddIngredient(ItemID.FragmentNebula, 15).
                AddIngredient(ItemID.FragmentStardust, 15).
                AddIngredient(ItemID.LunarBar, 15).
                AddTile(TileID.LunarCraftingStation).
                Register();

        }
    }
}
