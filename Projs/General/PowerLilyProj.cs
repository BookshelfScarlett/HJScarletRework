using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.General
{
    public class PowerLilyProj : HJScarletProj
    {
        public override ClassCategory Category =>  ClassCategory.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override bool ShouldUpdatePosition()
        {
            return base.ShouldUpdatePosition();
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
