using HJScarletRework.Globals.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 这个射弹用于高举
    /// </summary>
    public class AbyssalWorldHeldProjAlt : ExecutorHeldProj
    {
        public override string Texture => GetInstance<AbyssalWorldHeldProj>().Texture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            base.ExSD();
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
