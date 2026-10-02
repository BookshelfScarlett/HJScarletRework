using ContinentOfJourney.Items;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Terraria;
using Terraria.DataStructures;

namespace HJScarletRework.ReVisual.Items
{
    public class ReCrusadersCrossbow:ReVisualItemClass
    {
        public override int ApplyItem => ItemType<CrusadersCrossbow>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualCrusadersCrossbow= !vp.reVisualCrusadersCrossbow;
            item.noUseGraphic = vp.reVisualCrusadersCrossbow;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.GetModPlayer<ReVisualPlayer>().reVisualCrusadersCrossbow)
            {
                item.noUseGraphic = true;
            }
            base.UpdateInventory(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<ReVisualPlayer>().reVisualCrusadersCrossbow)
            {
                for (int i = 0; i < 16; i++)
                {
                    ECSParticle.ShinyCrossStarECS(position.ToRandCirclePos(6), velocity.ToRandVelocity(ToRadians(10), 1, 10), RandLerpColor(Color.Goldenrod, Color.LightGoldenrodYellow), 40,
                        1, Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
                }
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
