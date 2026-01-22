using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class SodomsDisasterBoom : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public float TheScale = 0.8f; 
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            //无限穿透，但是存续时间非常短
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0f;
            Projectile.scale = 1f;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                int dustCount = 25;
                for (int i = 0; i < dustCount; ++i)
                {
                    float randX = Main.rand.NextFloat(-10f, 10f);
                    float randY = Main.rand.NextFloat(-10f, 10f);
                    float randVelocity = Main.rand.NextFloat(5f, 10f);
                    float speed = (float)Math.Sqrt((double)(randX * randX + randY * randY));
                    speed = randVelocity / speed;
                    randX *= speed;
                    randY *= speed;
                    Vector2 vel = new Vector2(randX, randY) * Main.rand.NextFloat(0.24f, 0.28f);
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                    new ShinyOrbParticle(pos, vel, RandLerpColor(Color.Red, Color.DarkRed), 30, 1f).Spawn();
                    new ShinyOrbParticle(pos, vel, Color.White, 30, 0.4f).Spawn();
                }
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.Black, 30, 1f, 0.4f).Spawn();
                new CrossGlow(Projectile.Center, Vector2.Zero, Color.DarkRed, 30, 1f, 0.4f).Spawn();
                for (int i = 0; i < 5; i++)
                {
                    Color Firecolor = RandLerpColor(Color.Black, Color.DarkRed);
                    new Fire(Projectile.Center, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
