using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BlazingSun : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
