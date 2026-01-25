using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Pets
{
    public class WhaleProj : ScarletFlyingPet
    {
        public override int PetFrames => 4;
        public override void ExSD()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
        }
        public override void GetPetFlyingState(out float FlyingSpeed, out float FlyingIneritia, out bool ShouldSpeedUp, out float SpeedUpThreshold)
        {
            FlyingSpeed = 12f;
            FlyingIneritia = 60f;
            ShouldSpeedUp = true;
            SpeedUpThreshold = 600f;
        }
        public override void GetPetIdleState(out Vector2 FlyingOffset, out float FlyingArea, out float FlyingDrag, out bool ShouldFlyRotate)
        {
            base.GetPetIdleState(out FlyingOffset, out FlyingArea, out FlyingDrag, out ShouldFlyRotate);
        }
        public override void SimplePetFunction()
        {
            SimplePetAnimation(15f);
            if (Owner.dead)
                Owner.HJScarlet().WhalePet= false;

            if (Owner.HJScarlet().WhalePet)
                Projectile.timeLeft = 2;
        }
    }
}
