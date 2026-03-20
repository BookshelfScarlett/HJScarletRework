using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class TerraSpearLaser : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4, 2);
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.friendly = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 4; i++)
            {
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.90f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightGreen, Color.Lerp(Color.LightGreen, Color.Green, rads * 0.7f), (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.5f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, Projectile.oldRot[i], ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DarkGreen.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);
            return false;
        }
    }
    }
