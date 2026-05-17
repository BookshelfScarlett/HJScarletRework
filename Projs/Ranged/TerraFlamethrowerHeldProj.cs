using HJScarletRework.Globals.Classes;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.Ranged
{
    public class TerraFlamethrowerHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<TerraFlamethrower>().Texture + "Alt";
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
