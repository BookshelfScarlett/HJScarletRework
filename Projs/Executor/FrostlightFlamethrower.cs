using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightFlamethrower :ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<Frostlight>();
        public override string Texture => GetInstance<FrostlightHeldProj>().Texture;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime, 20);
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public ref float HeldAnimationHelper => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.SetUpHeldProj(10);
            Projectile.netImportant = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.AI();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public void RenderPixelated(SpriteBatch sb)
        {

        }

    }
}
