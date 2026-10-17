using HJScarletRework.Core.SeperateVisualEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Globals.Graphics.SeperateVisual
{
    public class EoCParryDashEffect :SeperateVisualBehavior
    {
        public override BlendState BlendState => BlendState.AlphaBlend;
        public override void Update()
        {
            base.Update();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Draw()
        {
            string path = GetType().Namespace + "." + GetType().Name;
            Texture2D tex = Request<Texture2D>(path.Replace('.', '/')).Value;
            //Rectangle frame = tex.Frame(1,3, )
            base.Draw();
        }
    }
}
