using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class Bamboomerang : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Item_Bamboomerang.Path;
        public override void ExSD()
        {
            Item.width = Item.height = 46;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.value = 500;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.damage = 20;
            Item.knockBack = 12f;
            Item.shoot = ProjectileType<BamboomerangProj>();
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 3;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodenBoomerang).
                AddIngredient(ItemID.BambooBlock, 20).
                AddIngredient(ItemID.JungleSpores, 5).
                AddTile(TileID.Sawmill).
                Register();
        }
    }
}
