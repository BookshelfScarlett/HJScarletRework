using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Rockets;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Items.Weapons.Magic;
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
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
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
            if (line.IsItemName())
            {
                foreach (var (itemIDs, drawMethods) in _rarityDrawMapScarlet)
                {
                    if (itemIDs.Contains(item.type))
                    {
                        drawMethods(line);
                        return false;
                    }
                }
            }
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
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
        private static readonly Dictionary<HashSet<int>, Action<DrawableTooltipLine>> _rarityDrawMapScarlet = new()
        {
            {
                new HashSet<int>
                {
                    ItemType<Corona>(),
                    ItemType<AetherfireSmasher>()
                },
                SolarRarity.DrawItemName
            }
        };
        /// <summary>
        /// 这里每帧都会高频调用，所以该创建哈希表了孩子们。
        /// </summary>
        private static readonly Dictionary<HashSet<int>, Action<DrawableTooltipLine>> _rarityDrawMap = new()
        {
            {
                new HashSet<int>
                {
                    ItemType<Evolution>(),
                    ItemType<ForceANature>(),
                    ItemType<LivingBar>(),
                    ItemType<PillarStaff>(),
                    ItemType<ForestHelmet>(),
                    ItemType<ForestBreastplate>(),
                    ItemType<ForestLeggings>(),
                    ItemType<EntropyReduction>(),
                    ItemType<Virtue>(),
                    ItemType<Lifesaber>(),
                    ItemType<HornofHarvest>(),
                    ItemType<EssenceofLife>(),
                    ItemType<DoctorExpeller>()
                },
                LivingRarity.DrawRarity
            },
            {
                new HashSet<int>
                {
                    ItemType<GalvanizedHand>(),
                    ItemType<FlybackHand>(),
                },
                TimeRarity.DrawRarity
            },
            {
                new HashSet<int>
                {
                    ItemType<Dialectics>(),

                },
                MatterRarity.DrawRarity
            }
        };

    }
}
