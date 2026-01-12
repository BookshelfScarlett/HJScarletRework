using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class RewardsofWarriorBuff : ModBuff
    {
        public override string Texture => HJScarletBuffIcon.Buff_RewardsofWarrior.Path;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += 0.15f + player.HJScarlet().Player_RewardofKingdom.ToInt() * 0.10f;
            Dust d = Dust.NewDustDirect(player.Center, player.width, player.height, DustID.SolarFlare);
            d.scale *= 1.2f;
        }
    }
}
