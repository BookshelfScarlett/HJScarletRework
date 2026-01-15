using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using static HJScarletRework.Projs.Ranged.AetherfireSmasherName;

namespace HJScarletRework.Projs.Ranged
{
    public class Aetherfireball : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 8;
            Projectile.height = 4;
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public bool CanHome = false;
        public override bool? CanDamage() => CanHome;
        public override void AI()
        {
            float minScale = 1.9f;
            float maxScale = 2.5f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), PickTagDust, Projectile.velocity * 0.25f);
                d.noGravity = true;
                d.scale *= Main.rand.NextFloat(minScale, maxScale);
            }
            StealthAI();
        }
        private void StealthAI()
        {
            ref float Timer = ref Projectile.ai[0];
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!CanHome)
            {
                Projectile.velocity.Y += 0.22f;
                if (Timer > 30)
                {
                    CanHome = true;
                    Projectile.timeLeft = 120;
                    Timer = 0;
                }
            }
            if (CanHome)
            {
                if (Timer < 30)
                    Timer++;
                if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, true))
                    Projectile.HomingTarget(target.Center, 1800f, 18f + Timer / 5f, 15f);
                else
                    Projectile.velocity.Y += 0.18f;
            } 
        }
        private short PickTagDust
        {
            get 
            {
                short Pick = Owner.name.SelectedName() switch
                {
                    NameType.TrueScarlet => DustID.CrimsonTorch,
                    NameType.WutivOrChaLost => DustID.YellowTorch,
                    NameType.Emma => DustID.HallowedTorch,
                    NameType.SherryOrAnnOrKino => DustID.UnusedWhiteBluePurple,
                    NameType.Shizuku => DustID.IceTorch,
                    NameType.Hanna => DustID.JungleTorch,
                    NameType.SerratAntler => DustID.PurpleTorch,
                    _ => DustID.Torch
                };
                return Pick;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.LightCoral;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = Color.LightGoldenrodYellow;
                    targetColor = Color.Yellow;
                    break;
                //银九 - 粉
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;

                //聚胶 - 紫
                case NameType.SerratAntler:
                    targetColor = Color.White;
                    baseColor = Color.DarkViolet;
                    break;
                case NameType.Hanna:
                    baseColor = Color.LimeGreen;
                    targetColor = Color.White;
                    break;
                default:
                    targetColor = Color.Orange;
                    baseColor = new Color(255, 215, 100);
                    break;
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
                Dust.NewDust(Projectile.position, 4, 4, PickTagDust, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }
        //抄的地狱飞剑
        //todo:换一个更好的火球贴图，这个现在是个占位符
        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Texture2D tex = HJScarletTexture.Particle_ShinyOrb.Value;
            float scale = 1f;
            //绘制残影
            for (int i = 1; i < 8; i++)
            {
                Vector2 projCenter = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                scale *= 0.94f;
                float radius = (float)i / 8;
                Color color = Color.Lerp(baseColor, targetColor, radius) * 0.5f * (1 - radius);
                Vector2 trailScale = Projectile.scale * new Vector2(1.5f, scale) * 1.5f;
                SB.Draw(tex, projCenter , null, color with { A = 50 } * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2f, trailScale, SpriteEffects.None, 0f);
                SB.Draw(tex, projCenter , null, targetColor  with { A = 50 } * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2f, trailScale * 0.40f, SpriteEffects.None, 0f);
            }
            //绘制火球本身
            float projScale = Projectile.scale * 1.5f;
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, baseColor with { A = 100 }, Projectile.rotation, tex.Size() / 2f,  projScale , SpriteEffects.None, 0);
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 100}, Projectile.rotation, tex.Size() / 2f, projScale /2f, SpriteEffects.None, 0);
            return false;
        }
    }
}
