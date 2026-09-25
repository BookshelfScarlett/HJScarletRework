using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class DuskfallBait : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.MaxUpdates = 4;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public BlendState BlendState => BlendState.AlphaBlend;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {

        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
