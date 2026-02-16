using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Buffs.Pets
{
    public class NoneBuff : PetsBuff
    {
        public override bool IsLightPet => true;
        public override void Update(Player player, ref int buffIndex)
        {
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref player.HJScarlet().NonePet, ProjectileType<NoneProj>());
        }
    }
}
