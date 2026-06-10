using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class ThePunishment : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1f;
        public override int ExecutionProgress => 40;
        public override void ExSSD()
        {
            HJScarletList.HallowedRarityHashSet.Add(Type);
        }

        public override void ExSD()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<ThePunishmentProj>();
            Item.knockBack = 8f;
            Item.width = Item.height = 44;
            Item.damage = 44;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem();
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void AddRecipes()
        {
            if (HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient(ItemID.Pwnhammer).
                    AddRecipeGroup(HJScarletRecipeGroup.AnyMechBossSoul, 5).
                    AddIngredient(ItemID.Diamond, 5).
                    AddIngredient(ItemID.Amethyst, 5).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
            else
            {
                CreateRecipe().
                        AddIngredient(ItemID.Pwnhammer).
                        AddIngredient(ItemID.HallowedBar, 5).
                        AddIngredient(ItemID.Diamond, 5).
                        AddIngredient(ItemID.Amethyst, 5).
                        AddTile(TileID.MythrilAnvil).
                        Register();
            }
        }
    }
}
