using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class EmblemFirearm : HJScarletItemClass
    {
        public float CritDamage = .30f;
        public int Crit = 20;
        public static int MaxSecondsBuff = 15;

        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent(), Crit + "%");
        public override void SetStaticDefaults()
        {
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var usPlayer = player.HJScarlet();
            if (player.HeldItem.CheckExecuteTypes(ExecutorWeaponType.Firearm) || player.HeldItem.CheckExecuteTypes(ExecutorWeaponType.Misc))
            {
                if (usPlayer.emblemFirearmTimer < GetSeconds(MaxSecondsBuff))
                    usPlayer.emblemFirearmTimer += 1;
                float ratios = Utils.GetLerpValue(0, GetSeconds(MaxSecondsBuff), usPlayer.emblemFirearmTimer, true);
                usPlayer.critDamageExecutor += Lerp(0, .5f, ratios);

                player.HJScarlet().emblemFirearm = true;
                player.GetCritChance<ExecutorDamageClass>() += Crit;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.FragmentVortex, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
