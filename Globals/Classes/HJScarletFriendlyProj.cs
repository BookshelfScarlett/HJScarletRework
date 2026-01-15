using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletFriendlyProj : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual ClassCategory Category { get; }
        public new string LocalizationCategory => $"Projs.Friendly.{Category}";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
            ExSD();
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
                    _ => DamageClass.Generic,
                };
            }
        }
        public virtual void ExSD() { }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
    }
}
