using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [LegacyName("PreciousAim")]
    public class SteadyBreath : HJScarletItemClass
    {
        public float Damage = .20f;
        public int Crit = 10;
        public static float ExtraDamage = 1.1f;
        public static float ChanceToCrit = .35f; 
        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%", ExtraDamage + "x", ChanceToCrit.ToPercent());
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RangedDamageClass>() += Damage;
            player.GetCritChance<RangedDamageClass>() += Crit;
            player.HJScarlet().preciousTargetLevel = 2;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PreciousTarget>().
                AddIngredient<BullseyeBadge>().
                AddIngredient<LivingBar>(15).
                AddTile<FinalAnvil>().
                Register();
        }
    }
}
