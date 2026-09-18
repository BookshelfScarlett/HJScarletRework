using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodThornCrown : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateWhite);
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
            ArmorIDs.Head.Sets.IsTallHat[Item.headSlot] = true;
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrownofSilveryLight>(15).
                AddIngredient<TankOfThePastHallow>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
