using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.SaintChurch
{
    [AutoloadEquip(EquipType.Legs)]
    public class SaintChurchLegs : HJScarletArmor
    {
        public float Damage = .02f;
        public float MoveSpeed = .3f;
        public float RunSpeedAcceleration = 1.25f;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateGolden);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), MoveSpeed.ToPercent(), RunSpeedAcceleration + "x");
        public override void ExSD()
        {
            Item.defense = 10;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.moveSpeed += MoveSpeed;
            player.runAcceleration *= RunSpeedAcceleration;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeepBar>(4).
                AddIngredient(ItemID.Silk, 4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
