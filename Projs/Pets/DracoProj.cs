using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Pets
{
    public class DracoProj : ScarletFloatingPet
    {
        public override int PetFrames => 6;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = true;
        }
        public override void SimplePetFunction()
        {
            SimplePetAnimation(15f);
            if (Owner.dead)
                Owner.HJScarlet().WatcherPet = false;
            if (Owner.HJScarlet().WatcherPet)
                Projectile.timeLeft = 2;
        }
        public override void GetPetSpriteState(out bool FaceLeft, out bool ShouldFiip)
        {
            FaceLeft = true;
            ShouldFiip = true;
        }
        public override Vector2 GetIdlePos(Player player)
        {
            return base.GetIdlePos(player);
        }
        public override void GetIdleState(out float MinDistance, out float SpeedMult, out float FloatingMult)
        {
            base.GetIdleState(out MinDistance, out SpeedMult, out FloatingMult);
        }

    }
}
