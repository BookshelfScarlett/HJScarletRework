using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteBounceBall : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 200;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float minScale = 1.9f;
            float maxScale = 2.5f;
            Projectile.velocity.Y += 0.18f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), DustID.Torch, Projectile.velocity * 0.25f);
                d.noGravity = true;
                d.scale *= Main.rand.NextFloat(minScale, maxScale);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            float minScale = 1.6f;
            float maxScale = 2.2f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust.NewDust(Projectile.position, 4, 4, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            Projectile.timeLeft -= 20;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = HJScarletTexture.Particle_ShinyOrb.Value;
            float scale = 1f;
            //绘制残影
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Vector2 projCenter = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                scale *= 0.94f;
                float radius = (float)i / 8;
                Color color = Color.Lerp(Color.OrangeRed, Color.Orange, radius) * 0.5f * (1 - radius);
                Vector2 trailScale = Projectile.scale * new Vector2(1.3f, scale);
                SB.Draw(tex, projCenter, null, color with { A = 50 } * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2f, trailScale, SpriteEffects.None, 0f);
            }
            //绘制火球本身
            float projScale = Projectile.scale * 1.5f;
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 100 }, Projectile.rotation, tex.Size() / 2f, projScale, SpriteEffects.None, 0);
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 100 }, Projectile.rotation, tex.Size() / 2f, projScale / 2f, SpriteEffects.None, 0);
            return false;
        }
    }
}
