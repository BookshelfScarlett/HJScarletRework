using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TheDisaster : HJScarletWeapon
    {
        public override string Texture => HJScarletItemProj.Item_Disaster.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 75;
            Item.useTime = Item.useAnimation = 30;
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FierySpear>().
                AddIngredient<DisasterBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
