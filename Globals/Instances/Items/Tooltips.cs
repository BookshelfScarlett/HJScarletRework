using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public IReadOnlyList<TooltipLine> CacheTooltipLine;
        public string OwnerName = string.Empty;
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
                        color = Color.Lime;
                        break;
                    case EnumItemOwner.Donator:
                        color = Color.Pink;
                        break;
                }
                string value = "<" + keyPath.ToLangValue() + "·" + item.HJScarlet().OwnerName + ">";
                tooltips.QuickAddTooltipDirect(value, color, LineName: item.HJScarlet().ItemBelongTo + "Name");
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
            //因为各种原因导致的史山
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
                    tooltips.QuickAddTooltipDirect(path, Color.Thistle, null, "ShinobiBuff", "35%", "15%", "200%");
                }
            }
            //强制自动处决/手动处决的字段
            if (ScarletItemIDSets.ForceToAutomaticExecute[item.type])
            {
                int index = tooltips.FindLineIndex("ExecutorWeaponTypeName", Mod.Name);
                string path = Mod.GetLocalizationKey($"ExecutorDamageClass.ForceAutomaticExecution").ToLangValue();
                TooltipLine line = new TooltipLine(Mod, "ForceAutomaticExecution", path)
                {
                    OverrideColor = Color.Pink
                };
                tooltips.Insert(index + 1, line);
            }
            else if (ScarletItemIDSets.ForceToTacticalExecute[item.type])
            {
                int index = tooltips.FindLineIndex("ExecutorWeaponTypeName", Mod.Name);
                string path = Mod.GetLocalizationKey($"ExecutorDamageClass.ForceTacticalExecution").ToLangValue();
                TooltipLine line = new TooltipLine(Mod, "ForceTacticalExecutionLine", path)
                {
                    OverrideColor = Color.Pink
                };
                tooltips.Insert(index + 1, line);
            }
            //标记物品未完成
            if (item.HJScarlet().NotFinished)
            {
                tooltips.CreateTooltip(Mod.GetLocalizationKey("NotFinished"), Color.IndianRed);
            }
            //无主之地系列的武器，红字文本。
            if (item.HJScarlet().borderlandWeapon)
            {
                string path = Mod.GetLocalizationKey($"Weapons.Executor.{item.ModItem.Name}.FlavorTooltip");
                int index = tooltips.FindLineIndex("Tooltip0");
                tooltips.CreateTooltip(path, Color.Lerp(Color.DarkRed, Color.Crimson, 0.82f), LineName: "BorderlandRedLineName", index: index);
            }
            if(item.type == ItemID.PocketMirror)
            {
                int index = tooltips.FindLineIndexLast("Tooltip");
                string path = Mod.GetLocalizationKey($"Database.PocketMirrorModiflication");
                tooltips.CreateTooltip(path, LineName: "PocketMirrorModiflication",color:Color.SkyBlue  , index: index + 2, args: Main.LocalPlayer.HJScarlet().pocketMirror.ToString());
            }
            CacheTooltipLine = tooltips;
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (!HJScarletConfigClient.Instance.SpecialRarity)
                return true;
            if (line.Name == (item.HJScarlet().ItemBelongTo + "Name") && line.Mod == Mod.Name)
            {
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Donator)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Donator);
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Developer)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Developer);
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Supporter)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Support);
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
    }
}
