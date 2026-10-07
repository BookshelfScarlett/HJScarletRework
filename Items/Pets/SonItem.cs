using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class SonItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<SonProj>();
        public override int PetBuffType => BuffType<SonBuff>();
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateWhite);
        }

    }
}
