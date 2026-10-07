using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Items.Pets
{
    public class WatcherItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<WatcherProj>();
        public override int PetBuffType => BuffType<WatcherBuff>();
    }
}
