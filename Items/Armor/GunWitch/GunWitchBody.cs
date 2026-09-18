using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.GunWitch
{
    [AutoloadEquip(EquipType.Body)]
    public class GunWitchBody : HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 22;
        }
        public int Crit = 25;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit+"%");
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += Crit;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DefenderMedal, 15).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
        }
    }
}
