using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class NoneItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<NoneProj>();
        public override int PetBuffType => BuffType<NoneBuff>();
    }
}
