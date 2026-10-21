using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.GlowMushroom
{

    [AutoloadEquip(EquipType.Head)]
    public class GlowMushroomHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemType<GlowMushroomBody>(), ItemType<GlowMushroomLegs>()];
        public override bool SetUpArmorSet => true;
        public int Crit = 5;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void ExSD()
        {
            Item.defense = 2;
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit + "%");
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + this.GetLocalizedValue("SetBonus");
            player.HJScarlet().raincoatExecutor = true;
            if (player.velocity.LengthSquared() > 2.5f * 2.5f && player.miscCounter % 3 == 0)
            {
                Dust d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.GlowingMushroom, player.velocity.X / 8f, player.velocity.Y / 8f);
                d.noGravity = true;
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += Crit;
            player.AddBuff(BuffID.NightOwl, 60);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GlowingMushroom, 15).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
