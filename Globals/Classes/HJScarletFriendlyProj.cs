using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletProj : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual ClassCategory Category => ClassCategory.Typeless;
        public static string ProjPath => $"HJScarletRework/Assets/Texture/Projs/";
        public override string Texture => ProjPath + GetType().Name;
        public new string LocalizationCategory => $"Projs.Friendly.{Category}";
        public bool PerformanceMode = HJScarletConfigClient.Instance.PerformanceMode;
        public virtual Vector2 TileHitbox => Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
            ExSD();
        }
        public virtual void OnFirstFrame() { }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (!TileHitbox.Equals(Vector2.Zero))
            {
                width = (int)TileHitbox.X;
                height = (int)TileHitbox.Y;
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public virtual void ProjAI() { }

        public virtual void ExSD() { }
        private DamageClass SetDamageClass
        {
            get
            {
                return Category switch
                {
                    ClassCategory.Melee => DamageClass.Melee,
                    ClassCategory.Ranged => DamageClass.Ranged,
                    ClassCategory.Magic => DamageClass.Magic,
                    ClassCategory.Summon => DamageClass.Summon,
                    ClassCategory.Executor => ExecutorDamageClass.Instance,
                    _ => DamageClass.Generic,
                };
            }
        }
    }
    public abstract class HJScarletFriendlyProj : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual ClassCategory Category { get; }
        public new string LocalizationCategory => $"Projs.Friendly.{Category}";
        public static string ProjPath => $"HJScarletRework/Assets/Texture/Projs/Proj_";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
            ExSD();
        }
        public virtual void OnFirstFrame() { }
        public virtual void ProjAI() { }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
        private DamageClass SetDamageClass
        {
            get
            {
                return Category switch
                {
                    ClassCategory.Melee => DamageClass.Melee,
                    ClassCategory.Ranged => DamageClass.Ranged,
                    ClassCategory.Magic => DamageClass.Magic,
                    ClassCategory.Summon => DamageClass.Summon,
                    ClassCategory.Executor => ExecutorDamageClass.Instance,
                    _ => DamageClass.Generic,
                };
            }
        }
        public virtual void ExSD() { }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
    }
}
