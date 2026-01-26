using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneShockwave : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int MoutedProjIndex = -1;
        public override void ExSD()
        {
            Projectile.netImportant = true;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 24;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }
        //行为总体上和旅人本身的那个没什么区别
        //但是这里附带了标记功能
        public override void AI()
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 180f)
            {
                Projectile.ai[1] = 180f;
            }
            Projectile.ai[0] += 1f;
            //获取主人射弹
            
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 vector = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI / 2.0));
            float collisionPoint = 0f;
            Vector2 vector2 = Projectile.Center + vector.RotatedBy(-Math.PI) * (Projectile.ai[1] / 100f) * 200f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), vector2, vector2 + vector * (Projectile.ai[1] / 100f) * 400f, 32f, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)((float)Projectile.damage * 0.7f);
            //过命中检测而非AI检测
            if (MoutedProjIndex == -1)
            {
                Projectile.Kill();
                return;
            }

            Projectile Parent = Main.projectile[MoutedProjIndex];
            if (Parent != null && Parent.active && Parent.type == ProjectileType<DeepToneMinion>())
            {
                //将攻击的单位加入到这个链表里面                
                //如果链表内已经超过了最大6个可用单位，跳出去不再添加
                if (((DeepToneMinion)Parent.ModProjectile).LegalTargetList.Count < 6)
                {
                    //注意过一下是否添加了重复的单位
                    if (!((DeepToneMinion)Parent.ModProjectile).LegalTargetList.Contains(target))
                        ((DeepToneMinion)Parent.ModProjectile).LegalTargetList.Add(target);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D value = Request<Texture2D>("ContinentOfJourney/Images/GiantSlash").Value;
            float num = Clamp(Projectile.ai[0] / 80f, 0f, 1f);
            if (Projectile.timeLeft < 30)
            {
                num = Clamp(Projectile.timeLeft / 30f, 0f, 1f);
            }

            SB.Draw(value, Projectile.Center - Main.screenPosition, null, Color.White * 0.66f * num, Projectile.velocity.ToRotation(), new Vector2(value.Width * 3 / 4, value.Height / 2), 1.2f * new Vector2(0.5f, Projectile.ai[1] / 100f), SpriteEffects.None,0);
            SB.Draw(value, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * num, Projectile.velocity.ToRotation(), new Vector2(value.Width * 3 / 4, value.Height / 2), new Vector2(0.5f, Projectile.ai[1] / 100f), SpriteEffects.None,0);
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

    }
}
