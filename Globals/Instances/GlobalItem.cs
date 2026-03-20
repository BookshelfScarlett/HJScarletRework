using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Rockets;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public bool CanDrawIcon = false;
        public float CritsDamageBonus = 0f;
        private int GhostTimer = 0;
        private int GhostFrame = 0;
        public override void UpdateInventory(Item item, Player player)
        {
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawIcon)
            {
                //在UpdateInventory内更新帧图的绘制，因为tooltip的draw实际上只会执行一次
                GhostTimer++;
                if (GhostTimer > 5)
                {
                    GhostFrame++;
                    GhostTimer = 0;
                }
                if (GhostFrame >= 16)
                    GhostFrame = 1;
            }
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (HJScarletConfigClient.Instance.DrawIcon &&CanDrawIcon)
            {
                Vector2 iconPosition = position + new Vector2(8f, 8f);
                float iconScale = 0.35f;
                Rectangle rect = new(0, GhostFrame * 44, 46, 42);
                Vector2 recorigin = new(23, 21);
                spriteBatch.Draw(HJScarletTexture.ScarletGhost.Value, iconPosition, rect, Color.White, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
            }
        }
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
                    ItemType<Apocalypse>()

                },
                MatterRarity.DrawRarity
            }
        };
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (HJScarletList.HJSpearList.Contains(item.type))
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lime);
            }
            base.ModifyTooltips(item, tooltips);
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
                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.AddLoot<AzureFrostmark>(4);
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
            if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargoWiltas))
                return;
            Recipe.Create(ItemType<AzureFrostmark>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyIceCrate, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
