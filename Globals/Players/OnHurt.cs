using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if(ShadowCastAcc && Main.rand.NextBool(10))
            {
                return false;
            }
            //星月夜的自活
            if(DesterrennachtAcc && !DesterrannachtImmortal)
            {
                DesterrannachtImmortal = true;
                DesterranTimer = GetSeconds(90);
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            return base.ConsumableDodge(info);
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            //星月夜的自活成功后的闪避，这里应该需要考虑一下……用什么钩子
            if(DesterrannachtImmortal && DesterranImmortalTime < 3)
            {
                Player.AddImmuneTime(ImmunityCooldownID.General, 60);
                DesterranImmortalTime += 1;
                return true;
            }
            return base.FreeDodge(info);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByNPC(npc, ref modifiers);
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByProjectile(proj, ref modifiers);
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if(PreciousTargetAcc && info.Damage > 5)
            {
                PreciousTargetCrtis -= 10;
                if (PreciousTargetCrtis < PreciousCritsMin)
                    PreciousTargetCrtis = PreciousCritsMin;
            }
        }
    }
}
