using HJScarletRework.Globals.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class WeHaveBookshelfBook : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Item, ItemID.Book);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            base.ExSD();
        }
    }
}
