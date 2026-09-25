using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.GunWitch
{
    [AutoloadEquip(EquipType.Head)]
    public class GunWitchHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemType<GunWitchBody>(), ItemType<GunWitchLegs>()];
        public override bool SetUpArmorSet => true;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 8;
        }
        public float Damage = .10f;
        public int Crit = 5;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%");
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;

        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
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
