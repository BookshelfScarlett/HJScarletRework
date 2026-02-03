using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Armor;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Placables;
using ContinentOfJourney.Items.Placables.FishingCrate;
using ContinentOfJourney.Items.Rockets;
using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
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
            //天塌下来了这也只能打表
            int[] livingItem = [
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
                ];
            //继续遍历直到没有内容为止
            for (int i = 0; i < livingItem.Length; i++)
            {
                if (item.type != livingItem[i])
                    continue;
                if (line.Name == "ItemName" && line.Mod == "Terraria")
                {
                    LivingRarity.DrawRarity(line);
                    return false;
                }
                
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
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

        }
    }
}
