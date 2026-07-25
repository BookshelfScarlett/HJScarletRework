using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.IDSets
{
    [ReinitializeDuringResizeArrays]
    public static class ScarletProjIDSets
    {

        /// <summary>
        /// 如果为<see langword="true"/>，则该射弹会被视为具备神性。主要用于神明类的单位。
        /// <br>由于泰拉瑞亚的特殊性质，如果需要在攻击上做操作，也需要管理对应的射弹</br>
        /// <br>默认集合里，包括了旅人归途中所有的至尊与其下的门徒们的射弹</br>
        /// </summary>

        public static bool[] DivingProjectile = ProjectileID.Sets.Factory.CreateBoolSet();
    }
}
