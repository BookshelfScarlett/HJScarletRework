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
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FlybackHandHitClock : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public ref float Timer => ref Projectile.ai[1];
        public override void ExSD()
        {
            //判定会比动画先跑
            Projectile.height = Projectile.width = 250;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.noEnchantmentVisuals = true;
            //初始化的时候总会设置为0
            Projectile.timeLeft = 600;
            Projectile.knockBack = 0;
        }
        public override void AI()
        {
            //Timer++;
            Timer = 1;
            Projectile.timeLeft = 2;
            base.AI();
        }
        public override bool ShouldUpdatePosition() => false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Texture2D ring = HJScarletTexture.Texture_BloomShockwave.Value;
            //我试了很多number
            //嗯
            //第一个环
            float counts = 50;
            for (float i = 0; i < counts; i++)
            {
                float val = (1 - i / counts);
                Color lerpColor = Color.Lerp(Color.LightGoldenrodYellow, Color.LightYellow, Clamp(val, 0, 1)) with { A = 50 } * 0.9f;
                Vector2 pos = Vector2.UnitY.RotatedBy(ToRadians(360 / counts * i));
                SB.Draw(star, Projectile.Center - pos * 80f - Main.screenPosition, null, lerpColor, pos.ToRotation(), star.Size() / 2, new Vector2(0.3f, 1f) * Timer, 0, 0);
            }
            //第二个环
            float counts2 = 67;
            for (float i = 0; i < counts2; i++)
            {
                float val = (1 - i / counts2);
                Color lerpColor = Color.Lerp(Color.LightGoldenrodYellow, Color.LightYellow, Clamp(val, 0, 1)) with { A = 50 } * 0.9f;
                Vector2 pos = Vector2.UnitY.RotatedBy(ToRadians(360 / counts2 * i));
                SB.Draw(star, Projectile.Center - pos * 100f - Main.screenPosition, null, lerpColor, pos.ToRotation(), star.Size() / 2, new Vector2(0.3f, 1f) * Timer, 0, 0);
            }
            SB.EnterShaderArea();
            //SB.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.LightYellow, 0, ring.Size() / 2, 0.45f * Timer, 0, 0);
            SB.Draw(HJScarletTexture.Particle_HRShinyOrbMedium.Value, Projectile.Center - Main.screenPosition, null, Color.LightYellow * 0.8f, 0, HJScarletTexture.Particle_HRShinyOrbMedium.Size() / 2, 5f * Timer, 0, 0);
            //圆球
            //SB.Draw(HJScarletTexture.Particle_OpticalLineGlow.Value, Projectile.Center - Main.screenPosition, null, Color.Gold, 0, HJScarletTexture.Particle_OpticalLineGlow.Origin, Timer * 0.10f, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
