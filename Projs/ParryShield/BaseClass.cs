using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.ParryShield
{
    public abstract class ParryShieldProjClass : ModProjectile, ILocalizedModType
    {
        public override string LocalizationCategory => "Projs.Friendly.ParryShieldProj";
        /// <summary>
        /// 格挡盾直接命中敌人时，允许给敌人击晕的时间，以帧
        /// <br>这个也同样会影响直接命中时的卡肉效果</br>
        /// </summary>
        public virtual int ParryFrame => 15;
        public override string Texture => "HJScarletRework/Assets/Texture/ParryShield/Proj/" + GetType().Name;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.noEnchantments = true;
            ExSD();
        }
        public virtual void ExSD() { }
        public override void AI()
        {
            base.AI();
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
