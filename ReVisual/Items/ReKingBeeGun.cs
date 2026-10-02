using ContinentOfJourney.Items;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace HJScarletRework.ReVisual.Items
{
    public class ReKingBeeGun : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<KingBeeGun>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualKingBeeGun= !vp.reVisualKingBeeGun;
            item.noUseGraphic = vp.reVisualKingBeeGun;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.GetModPlayer<ReVisualPlayer>().reVisualKingBeeGun)
            {
                item.noUseGraphic = true;
            }
            base.UpdateInventory(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<ReVisualPlayer>().reVisualKingBeeGun)
            {
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<ReVisualRecoilProj>(), 0, 0, player.whoAmI);
                if (proj.ModProjectile is ReVisualRecoilProj holdout)
                {
                    holdout.SetUpHoldoutData(ApplyItem, 6.5f, item.useAnimation, new Vector2(10, -2.5f));
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

    }
}
