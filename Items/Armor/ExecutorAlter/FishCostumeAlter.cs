using HJScarletRework.Globals.Methods;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public class FishCostumeHelmet : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeMask;
        public static int Defense = 6;
        public override string SetupName => "FishCostume";
        public override bool SetUpArmorSet => true;
        public override int[] ArmorSlots => [ItemID.FishCostumeMask, ItemID.FishCostumeShirt,ItemID.FishCostumeFinskirt];
        public override void ExSD(Item item)
        {
            item.defense = Defense; 
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (!set.Equals(ArmorSetName))
                return;
            string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.SetBonus");
            player.setBonus += "\n" + armorCategory.ToLangValue();
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
    public class FishCostumeChestplate : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeShirt;
        public static int Defense = 12;
        public override string SetupName => "FishCostume";
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
    public class FishCostumeLegs : AlterVanillaArmor
    {
        public override int ApplyArmor => ItemID.FishCostumeFinskirt;
        public static int Defense = 4;
        public override string SetupName => "FishCostume";
        public override void ExSD(Item item)
        {
            item.defense = Defense;
        }
        public override void ExUpdateEquipAlter(Item item, Player player)
        {
            base.ExUpdateEquipAlter(item, player);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }

    }
}
