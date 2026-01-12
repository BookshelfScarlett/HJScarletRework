using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class EvolutionEnergyPortal : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public bool SpawnEvolutionArrow = false;
        public bool StopSpawnArrow = false;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {

        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 0;
            Projectile.damage = 0;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.82f;
            Timer+=1f;
            //除非附近有可用单位，否则这个传送门不会为您生成任何箭矢
            //额外的，如果进化矛本身直接命中了敌对单位，则所有的箭矢都会直接攻击他
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1200f) && SpawnEvolutionArrow && !StopSpawnArrow)
            {
                //每个传送门都只生成1根箭矢
                Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Vector2 toRandDir = dir.RotatedBy(Main.rand.NextFloat(ToRadians(5) * Main.rand.NextBool().ToDirectionInt()));
                Vector2 vel =  toRandDir * Main.rand.NextFloat(10f, 12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center - dir * 10f, vel, ProjectileType<EvolutionArrow>(),Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                //生成时，附带粒子
                DrawArrowSpawnDust();
                //生成结束后让传送门缓慢消失
                StopSpawnArrow = true;
            }
            else if(StopSpawnArrow || Timer >60f)
            {
                Projectile.Opacity -= 0.1f;
            }
        }

        private void DrawArrowSpawnDust()
        {
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.Green, 40, 1f, 0.2f).Spawn();
            new CrossGlow(Projectile.Center, Vector2.Zero, Color.White, 40, 1f, 0.1f).Spawn();
            int count = 36;
            for (int i = 1; i <= count; i++)
            {
                //生成一组圆环圈
                Vector2 dir = -Vector2.UnitY.RotatedBy(ToRadians(360 / count * i));
                Vector2 vel =  dir * 1.05f;
                //new ShinyOrbParticle(Projectile.Center + dir * 1.2f, vel, Color.DarkGreen.RandLerpTo(Color.PaleGreen), 40, 0.65f, glowCenter: false).SpawnToPriority();
                new TurbulenceShinyOrb(Projectile.Center + dir * 1.2f, 1.2f, Color.DarkGreen.RandLerpTo(Color.PaleGreen), 40, 0.12f, dir.ToRotation()).SpawnToPriority();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D crossGlow = HJScarletTexture.Particle_CrossGlow.Value;
            Texture2D circle = HJScarletTexture.Texture_SoftCircleEdge.Value;
            //底图处理
            SB.Draw(HJScarletTexture.Texture_BloomShockwave.Value, drawPos, null, Color.LimeGreen, 0, HJScarletTexture.Texture_BloomShockwave.Origin, 0.02f * Projectile.scale * Projectile.Opacity, 0, 0);
            //光圈，叠加
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //绘制辉光
            SB.Draw(circle, drawPos, null, Color.LawnGreen, 0, circle.Size() /2, 0.12f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.Green, Projectile.rotation, crossGlow.Size() / 2, 0.17f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.White, Projectile.rotation, crossGlow.Size() / 2, 0.08f * Projectile.scale * Projectile.Opacity, 0, 0);
            SB.End();
            SB.BeginDefault();
            return true;
        }
    }
}
