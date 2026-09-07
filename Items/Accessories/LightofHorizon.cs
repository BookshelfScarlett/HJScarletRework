using ContinentOfJourney.Buffs;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Accessories.GrazeBadge;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Materials;
using HJScarletRework.Globals.Database.Enums;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class LightofHorizon : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override bool AllowPrefix(int pre)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Matter);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            if (Main.netMode != 2)
            {
                ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(300, 12f, 4f);
            }
        }
        public override bool CanReforge() => false;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.expert = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.empressBrooch)
            {
                player.moveSpeed += .075f;
                player.jumpSpeedBoost += 1.8f;
                player.runAcceleration *= 1.75f;
            }
            player.noKnockback = true;
            //十字盾的减益免疫
            player.HJScarlet().ankhShieldImmnue = true;
            //无限水下呼吸，跳过原版的检测避免在ftw淹死
            player.HJScarlet().infiniteBreath = true;
            //无限飞行时间
            player.HJScarlet().infiniteFlightTime = true;
            player.HJScarlet().LightofHorizon = true;
            //泰拉闪耀靴的各种效果
            player.HJScarlet().terraSparkBoostImmnue = true;
            player.GrazeBadge().deceleration = 4f;
            //下方继承视界的效果
            player.desertBoots = true;
            player.accRunSpeed = 12f;
            player.moveSpeed += 0.2f;
            player.autoJump = true;
            player.jumpSpeedBoost += 1.6f;
            player.noFallDmg = true;
            player.flowerBoots = true;
            player.fairyBoots = true;
            player.ignoreWater = true;
            player.accFlipper = true;
            player.buffImmune[BuffType<VulnerableBuff>()] = true;

            if (player.whoAmI == Main.myPlayer)
            {
                player.DoBootsEffect(player.DoBootsEffect_PlaceFlowersOnTile);
            }

            player.buffImmune[67] = true;

            if (player.mount.Type == -1 && player.wingTime > 0f && player.controlUp && player.controlJump && player.velocity.Y > -15f)
            {
                player.velocity.Y -= 0.8f;
            }

            if (player.mount.Type == -1 && player.wingTime > 0f && player.controlDown && player.controlJump)
            {
                player.velocity.Y += (5.08f - player.velocity.Y) / 8f;
            }
        }
        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {

            }

            return base.WingUpdate(player, inUse);
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            if (player.mount.Type == -1 && player.wingTime > 0f && player.controlDown && player.controlJump)
            {
                speed *= 1.25f;
                acceleration *= 8f;
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.4f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2f;
            constantAscend = 0.135f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkyofHorizon>().
                AddIngredient<Altitude>().
                AddIngredient<Horizon>().
                AddIngredient(ItemID.ArcticDivingGear).
                AddIngredient<EssenceofDeath>(15).
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();

            CreateRecipe().
                AddIngredient<SkyofHorizon>().
                AddIngredient<Altitude>().
                AddIngredient<Horizon>().
                AddIngredient(ItemID.ArcticDivingGear).
                AddIngredient<EssenceofDeath>(15).
                AddIngredient<FinalBar>().
                AddCondition(HJScarletCraftingConditions.InMultiplayer).
                DisableDecraft().
                AddTile(FinalAnvilTile).
                Register();

        }

    }
}
