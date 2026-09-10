using HJScarletRework.Globals.Executor;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.Executor
{
    public class MoonfireHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Moonfire>();
        public override string Texture => GetInstance<Moonfire>().Texture;
        public override int MinAttackRates => 5;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
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
