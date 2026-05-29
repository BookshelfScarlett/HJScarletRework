using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    //public class MaidHelmetAlter : AlterVanillaArmor
    //{
    //    public override int ApplyArmor => ItemID.MaidHead;
    //    public static int Defense = 8;
    //    public override int[] ArmorSlots => [ApplyArmor,ItemID.MaidShirt,ItemID.MaidPants];
    //    public override bool SetUpArmorSet => true;
    //    public override string SetupName => "Maid";
    //    public override ArmorType Category => ArmorType.Helmet;
    //    public override int DownedConditionID => NPCID.Plantera;
    //    public override void ExUpdateEquipAlter(Item item, Player player)
    //    {
    //        base.ExUpdateEquipAlter(item, player);
    //    }
    //    public override void ExSD(Item item)
    //    {
    //        item.vanity = false;
    //        item.defense = Defense;
    //        item.rare = ItemRarityID.Yellow;
    //    }
    //    public override void UpdateArmorSet(Player player, string set)
    //    {
    //        if (!set.Equals(ArmorSetName))
    //            return;

    //    }
    //    public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
    //    {
    //        base.ExModifyTooltipsAlter(item, tooltips, path);
    //    }
    //}
    //public class MaidChestplateAlter :AlterVanillaArmor
    //{
    //    public override int ApplyArmor => ItemID.MaidShirt;
    //    public static int Defense = 18;
    //    public override int DownedConditionID => NPCID.Plantera;
    //    public override ArmorType Category => ArmorType.Chestplate;
    //    public override string SetupName => "Maid";
    //    public override void ExUpdateEquipAlter(Item item, Player player)
    //    {
    //        base.ExUpdateEquipAlter(item, player);
    //    }
    //    public override void ExSD(Item item)
    //    {
    //        item.vanity = false;
    //        item.defense = Defense;
    //        item.rare = ItemRarityID.Yellow;
    //    }
    //    public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
    //    {
    //        tooltips.AddSwapTooltipValueBossCondition(DownedConditionID);
    //    }
    //}
    //public class MaidLegsAlter : AlterVanillaArmor
    //{
    //    public override int ApplyArmor => ItemID.MaidPants;
    //    public static int Defense = 12;
    //    public override int DownedConditionID => NPCID.Plantera;
    //    public override ArmorType Category => ArmorType.Legs;
    //    public override string SetupName => "Maid";
    //    public override void ExUpdateEquipAlter(Item item, Player player)
    //    {
    //        base.ExUpdateEquipAlter(item, player);
    //    }
    //    public override void ExSD(Item item)
    //    {
    //        item.vanity = false;
    //        item.defense = Defense;
    //        item.rare = ItemRarityID.Yellow;
    //    }
    //    public override void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
    //    {
    //        tooltips.AddSwapTooltipValueBossCondition(DownedConditionID);
    //    }
    //}
}
