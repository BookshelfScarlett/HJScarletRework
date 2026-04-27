using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

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

        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
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
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            Color baseColor = Color.DarkOrange;
            Color targetColor = Color.OrangeRed;
            //绘制残影
            float oriScale = 0.70f;
            float scale = 1f;
            int length = 10;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.925f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(baseColor, targetColor, (1 - rads)).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            return false;
        }
    }
}
