using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Projs.Pets;
using Terraria.ID;

namespace HJScarletRework.Items.Pets
{
    public class SonItem : HJScarletPetItem
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateWhite);
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
