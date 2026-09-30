using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Pets
{
    public class SquidItem : HJScarletPetItem
    {
        public override int PetProjType => ProjectileType<SquidProj>();
        public override int PetBuffType => BuffType<SquidBuff>();
    }
}
