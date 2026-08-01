using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.ReVisual.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class MonocleBulletExecution : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 10;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(-1);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation(); 
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            int maxHit = Monocle.ExecutionPenetrate;
            float ratios = Utils.GetLerpValue(0, maxHit, Projectile.numHits, true);
            float damageMult = Lerp(Monocle.ExecutionDamageMult, 0.75f, ratios);
            modifiers.SourceDamage *= damageMult;
            modifiers.SetCrit();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public void RenderPixelated(SpriteBatch spriteBatch)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
