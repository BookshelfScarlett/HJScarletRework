using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Shinobi;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Monk
{

    [AutoloadEquip(EquipType.Legs)]
    public class MonkLegs : HJScarletArmor
    {
        public override void SetStaticDefaults()
        {
            Type.ShimmerEach<ShinobiLegs>();
        }
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 16;
        }
        public int ArmorPenetration = 10;
        public float MoveSpeed = .3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, MoveSpeed.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration<ExecutorDamageClass>() += ArmorPenetration;
            player.moveSpeed += MoveSpeed;
        }

    }
}
