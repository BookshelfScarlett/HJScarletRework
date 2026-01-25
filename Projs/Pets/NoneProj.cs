using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Pets
{
    public class NoneProj : ModProjectile, ILocalizedModType
    {
        public override string LocalizationCategory => "Projs.Friendly.Pets";
        public override string Texture => $"HJScarletRework/Assets/Texture/Pets/Pet_{GetType().Name}";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            if (Owner.dead)
                Owner.HJScarlet().NonePet= false;

            if (Owner.HJScarlet().NonePet)
                Projectile.timeLeft = 2;

            Projectile.Center = new Vector2(Owner.Center.X, Owner.Center.Y - 35f);
            Projectile.light = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, 1);
            return false;
        }
    }
}
