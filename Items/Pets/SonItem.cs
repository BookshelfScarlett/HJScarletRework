using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.List;
using HJScarletRework.Projs.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Pets
{
    public class SonItem : HJScarletPetItem
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<SonProj>(), BuffType<SonBuff>());
        }

        public override void ExSD()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuPetItem);
        }

    }
}
