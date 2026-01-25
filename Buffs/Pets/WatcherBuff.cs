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
    public class WatcherBuff : PetsBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref player.HJScarlet().WatcherPet, ProjectileType<WatcherProj>());
        }

    }
}
