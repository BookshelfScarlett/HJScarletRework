using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class TheMarsSpark : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<TheMars>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public enum Style
        {
            Spawn,
            ToTarget
        }
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int ParentProjectileIndex = -1;
        public bool DoStrike = false;
        public NPC AvaiTar = null;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 6;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 0;
        }
        public override void AI()
        {
            Projectile.rotation += ToRadians(1f);
            //锁住射弹位置
            if (Projectile.GetTargetSafe(out NPC target, false))
            {
                //这里的Timer只会用于生成第一个射弹，然后消失
                Timer++;
                if (Timer < 20f)
                    return;

                Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center, 0.1f);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPos = Projectile.Center + Vector2.UnitY.RotatedByRandom(TwoPi) * 150f;
                    Vector2 dir = (target.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, dir * Main.rand.NextFloat(6f, 11f), ProjectileType<TheMarsPhantom>(), Projectile.originalDamage, 1.5f, Owner.whoAmI);
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                }
            }
            Projectile.Kill();

        }
        public override bool? CanDamage() => false;
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public void DrawCirlceGlow(Vector2 projDir, Vector2 drawPos, Texture2D starShape)
        {
            float starDrawTime = 20f;
            for (float i = 0; i < starDrawTime; i++)
            {
                Vector2 argDir = projDir.RotatedBy(ToRadians(360f / starDrawTime * i) + Projectile.rotation) * 23f;
                Vector2 starPos = drawPos + argDir;
                Vector2 scale = Projectile.scale * new Vector2(0.2f, 0.5f) * 0.7f;
                SB.Draw(starShape, starPos, null, Color.LawnGreen with { A = 0 }, argDir.ToRotation(), starShape.ToOrigin(), scale, 0, 0);
                if (i % 4 == 0)
                {
                    SB.Draw(starShape, starPos, null, Color.White with { A = 0 }, argDir.ToRotation() + PiOver2, starShape.Size() / 2, scale, 0, 0);
                }
            }
        }
    }
}
