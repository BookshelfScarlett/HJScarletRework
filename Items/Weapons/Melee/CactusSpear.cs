using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class CactusSpear : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override bool HasLegendary => false;
        public override void ExSD()
        {
            Item.width = Item.height = 44;
            Item.damage = 15;
            Item.useTime = Item.useAnimation = 40;
            Item.shoot = ProjectileType<CactusSpearProj>();
            Item.shootSpeed = 11f;
            Item.rare = ItemRarityID.Green;
            Item.value = 500;
        }
        public override Color MainTooltipColor => Color.LightGreen;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Cactus, 16).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
