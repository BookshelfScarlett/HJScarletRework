using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Pets;
using Terraria;

namespace HJScarletRework.Buffs.Pets
{
    public class DracoBuff : PetsBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref player.HJScarlet().dracoPet, ProjectileType<DracoProj>());
        }

    }
}
