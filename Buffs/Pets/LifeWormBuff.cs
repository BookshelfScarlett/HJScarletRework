using HJScarletRework.Projs.Pets;

namespace HJScarletRework.Buffs.Pets
{
    public class LifeWormBuff : PetsBuff
    {
        public override int PetProjType => ProjectileType<LifeWormProj>();
    }
}
