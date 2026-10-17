using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class DiverItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<DiverProj>();
        public override int PetBuffType => BuffType<DiverBuff>();
    }
}
