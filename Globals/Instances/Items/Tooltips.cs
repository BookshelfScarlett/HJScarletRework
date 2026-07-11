using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System;
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
            if (ItemBelongTo != ItemBelong.None)
            {
                string keyPath = Mod.GetLocalizationKey($"ItemBelongTo.{ItemBelongTo}");
                Color color = Color.White;
                switch (ItemBelongTo)
                {
                    case ItemBelong.Developer:
                        color = Color.Red;
                        break;
                    case ItemBelong.Supporter:
                        color = Color.Yellow;
                        break;
                    case ItemBelong.Donator:
                        color = Color.HotPink;
                        break;
                }
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), color);
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
            base.ModifyTooltips(item, tooltips);
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName() && HJScarletConfigClient.Instance.SpecialRarity)
            {
                foreach (var (itemIDs, drawMethods) in _rarityDrawMap)
                {
                    if (itemIDs.Contains(item.type))
                    {
                        drawMethods(line);
                        return false;
                    }
                }
                if (HJScarletList.MiscRarityDrawDictionary.TryGetValue(item.type, out Action<DrawableTooltipLine> value))
                {
                    value(line);
                    return false;
                }
                if (HJScarletList.RareItemRarityDrawDictionary.TryGetValue(item.type, out RareItemRarity.RareType type))
                {
                    RareItemRarity.DrawItemName(line, type);
                    return false;
                }
                if (item.HJScarlet().EnableExecutorVersion)
                {
                    if (HJScarletList.ConvertedItemRarityDrawDictionary.TryGetValue(item.type, out Action<DrawableTooltipLine> value2))
                    {
                        value2(line);
                        return false;
                    }
                }
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
        }
        /// <summary>
        /// 这里每帧都会高频调用，所以该创建哈希表了孩子们。
        /// </summary>
        private static readonly Dictionary<HashSet<int>, Action<DrawableTooltipLine>> _rarityDrawMap = new()
        {
            //寒霜武器的稀有度
            {
                HJScarletList.FrostRarityHashSet,
                FrostRarity.DrawItemName
            },
            //日轮锭的稀有度
            {
                HJScarletList.DisasterRarityHashSet,
                SolarRarity.DrawItemName
            },
            //梦魇锤系列的稀有度
            {
                HJScarletList.NightRarityHashSet,
                NightRarity.DrawRarity
            },
            //神圣
            {
                HJScarletList.HallowedRarityHashSet,
                HallowedRarity.DrawRarity
            },
            //星云
            {
                HJScarletList.NebulaRarityHashSet,
                NebulaRarity.DrawRarityReverse
            },
            //无极绯红
            {
                HJScarletList.ScarletRarityHashSet,
                DisasterRarity.DrawRarity2
            },
            {
                HJScarletList.SunlightRarityHashSet,
                SunlightRarity.DrawItemName
            }
        };

    }
}
