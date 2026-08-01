using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public IReadOnlyList<TooltipLine> CacheTooltipLine;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ItemBelongTo != EnumItemOwner.None)
            {
                string keyPath = Mod.GetLocalizationKey($"ItemBelongTo.{ItemBelongTo}");
                Color color = Color.White;
                switch (ItemBelongTo)
                {
                    case EnumItemOwner.Developer:
                        color = Color.Red;
                        break;
                    case EnumItemOwner.Supporter:
                        color = Color.Yellow;
                        break;
                    case EnumItemOwner.Donator:
                        color = Color.HotPink;
                        break;
                }
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), color, LineName: item.HJScarlet().ItemBelongTo + "Name");
            }
            if (HJScarletPlayer.AllWeaponSwapValue.Contains(item.type))
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lerp(Color.LawnGreen, Color.LightGreen, 0.5f));
            }
            if (LocalPlayer.HJScarlet().terraRecipe)
            {
                if (HJScarletList.LegalFoodList.Contains(item.type))
                {
                    //表单里有这个内容我们才写这个东西。没有则写另一条
                    string path = Mod.GetLocalizationKey($"Items.Useable.TerrariaRecipe.");
                    List<int> list = LocalPlayer.HJScarlet().terraRecipe_EatenFoodList;
                    if (list.Contains(item.type))
                        tooltips.QuickAddTooltipDirect((path + "Eaten").ToLangValue(), Color.GreenYellow);
                    else
                        tooltips.QuickAddTooltipDirect((path + "NotEaten").ToLangValue(), Color.SkyBlue);
                }
            }
            if (LocalPlayer.HJScarlet().monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT1)
                {
                    string path = Mod.GetLocalizationKey($"Items.Armor.{nameof(MonkHead)}.SleepyOctBuff").ToLangValue();
                    string path2 = Mod.GetLocalizationKey($"Items.Armor.{nameof(ShinobiHead)}.WeaponBuff").ToLangValue();
                    tooltips.QuickAddTooltipDirect(path2, Color.Bisque, null, "ShinobiBuffTitle");
                    tooltips.QuickAddTooltipDirect(path, Color.GreenYellow, null, "ShinobiBuff", "20%", "15%", "20%");
                }
                if (item.type == ItemID.MonkStaffT3)
                {
                    string path = Mod.GetLocalizationKey($"Items.Armor.{nameof(MonkHead)}.DragonFuryBuff").ToLangValue();
                    string path2 = Mod.GetLocalizationKey($"Items.Armor.{nameof(ShinobiHead)}.WeaponBuff").ToLangValue();
                    tooltips.QuickAddTooltipDirect(path2, Color.Bisque, null, "ShinobiBuffTitle");
                    tooltips.QuickAddTooltipDirect(path, Color.Thistle, null, "ShinobiBuff", "50%", "15%", "200%");
                }
            }
            if (item.HJScarlet().ForceTacticalExecution && HJScarletList.IsExecutorWeaponDictionaty.ContainsKey(item.type))
            {
                int index = 0;
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Name == "ExecutorWeaponTypeName" && tooltips[i].Mod == Mod.Name)
                    {
                        index = i;
                        break;
                    }
                }
                string path = Mod.GetLocalizationKey($"ExecutorDamageClass.ForceTacticalExecution").ToLangValue();
                TooltipLine line = new TooltipLine(Mod, "ForceTacticalExecutionLine", path)
                {
                    OverrideColor = Color.Pink
                };
                tooltips.Insert(index + 1, line);
            }
            if(item.HJScarlet().NotFinished)
            {
                tooltips.CreateTooltip(Mod.GetLocalizationKey("NotFinished"),Color.IndianRed);
            }
            base.ModifyTooltips(item, tooltips);
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (!HJScarletConfigClient.Instance.SpecialRarity)
                return true;
            if (line.Name == (item.HJScarlet().ItemBelongTo + "Name") && line.Mod == Mod.Name)
            {
                RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Donator);
                return false;
            }
            if (line.IsItemName())
            {
                if (HJScarletList.ShinyRarityItemDictionary.TryGetValue(item.type, out ShinyRarityType value))
                {
                    RarityDrawHelper.UpdateItemNameParticle(line, value);
                    RarityDrawHelper.UpdateItemNameDraw(line, value);
                    return false;
                }
            }
            if (line.Mod == Mod.Name && line.Name == "FlavorTooltipsName")
            {
                if (HJScarletList.ShinyRarityItemDictionary.TryGetValue(item.type, out ShinyRarityType value))
                {
                    RarityDrawHelper.UpdateFlavorNameDraw(line, value);
                    return false;
                }
            }
            return true;
        }
        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
        }
    }
}
