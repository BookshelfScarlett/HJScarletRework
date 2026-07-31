using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaperLegs : HJScarletArmor
    {
        public float MoveSpeed = .25f;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeed.ToPercent());

        public override void ExSD()
        {
            Item.defense = 45;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeed;
            player.aggro += 500;
        }
    }
}
