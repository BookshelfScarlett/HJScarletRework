using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class PandorasBurgerHeldProj : HJScarletFloatingBook
    {
        public override int OriginalItemID => ItemType<PandorasBurger>();
        public override string Texture => GetInstance<PandorasBurger>().Texture;
        public override void HandleLeftAttack()
        {
            if (Timer < Projectile.MaxUpdates * 20)
                return;
            Timer = 0;
            for (int i = 0; i < 3; i++)
            {
                Item item = ContentSamples.ItemsByType[HJScarletList.PandorasBurgerWeaponList[Main.rand.Next(0, HJScarletList.PandorasBurgerWeaponList.Count)]];
                var src = new EntitySource_ItemUse_WithAmmo(Owner, item, AmmoID.None);
                Vector2 vel = Projectile.Center.GetNormalVector2(Main.MouseWorld);
                ItemLoader.Shoot(item, Owner, src, Projectile.Center, vel * item.shootSpeed, item.shoot, Projectile.originalDamage, item.knockBack);
            }
        }
    }
}
