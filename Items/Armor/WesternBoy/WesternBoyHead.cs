using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.WesternBoy
{

    [AutoloadEquip(EquipType.Head)]
    public class WesternBoyHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemType<WesternBoyBody>(), ItemType<WesternBoyLegs>()];
        public override bool SetUpArmorSet => true;
        public static int RevolerDamage = 40;
        public float Damage = .05f;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void ExSD()
        {
            Item.defense = 5;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + this.GetLocalizedValue("SetBonus");
            player.HJScarlet().cowboyExecutor = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 10).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 5).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
