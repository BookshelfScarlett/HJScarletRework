using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Placables;
using ContinentOfJourney.Items.Placables.FishingCrate;
using ContinentOfJourney.Items.Rockets;
using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool EnableCritDamage = false;
        public float CritsDamageBonus = 0f;
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria" && HJScarletConfigClient.Instance.SpecialRarity)
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
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (HJScarletCategoryList.HJSpearList.Contains(item.type))
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lime);
            }
            base.ModifyTooltips(item, tooltips);
        }
        private void DrawSpecialRarityName(Item item, DrawableTooltipLine line, ref int y)
        {
        }

        public override void HoldItem(Item item, Player player)
        {
            if (EnableCritDamage)
            {
                player.HJScarlet().GeneralCrtiDamageAdd += CritsDamageBonus;
            }
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            return base.IsArmorSet(head, body, legs);
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch(item.type)
            {
                case ItemID.GolemBossBag:
                    itemLoot.AddLoot<DisasterEssence>(1, 10, 20);
                    break;
            }
            if(!Main.masterMode)
            {
                return;                

            }
            if (item.type == ItemType<ScarabBeliefTreasureBag>())
                itemLoot.AddLoot<SacarbWings>(10);
        }
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.Spear).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 12).
                AddTile(TileID.Anvils).
                Register();

            Recipe.Create(ItemID.SunStone).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                AddIngredient<DisasterEssence>(50).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.Amber, 15).
                AddIngredient<DisasterEssence>(50).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemType<AncientBlessing>()).
                AddIngredient<CrimsonRune>().
                AddIngredient(ItemID.CelestialShell).
                AddIngredient<TankOfThePastJungle>(5).
                DisableDecraft().
                AddTile(TileID.TinkerersWorkbench).
                Register();

        }
    }
}
