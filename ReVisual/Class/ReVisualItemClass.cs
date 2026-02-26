using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.ReVisual.Class
{
    public abstract class ReVisualItemClass : GlobalItem
    {
        public virtual int ApplyItem => -1;
        public sealed override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ApplyItem;
        }
        public sealed override bool InstancePerEntity => true;
        public sealed override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.CreateTooltip(Mod.GetLocalizationKey("SwitchWeapon.Visual"), Color.LightGray);
            ExModifyTooltips(item, tooltips);
        }
        public virtual void ExModifyTooltips(Item item, List<TooltipLine> tooltips) { }
        public override void HoldItem(Item item, Player player)
        {
            //只允许在本地玩家执行
            if(Main.myPlayer == player.whoAmI)
            {
                var vp = player.GetModPlayer<ReVisualPlayer>();
                if (Main.mouseMiddle)
                {
                    if (Main.mouseMiddleRelease)
                    {
                        SoundEngine.PlaySound(SoundID.ResearchComplete, player.Center);
                    for (int i = 0; i < 30; i++)
                        new TurbulenceShinyOrb(player.Center.ToRandCirclePosEdge(30f), 1.2f, Color.White, 120, 0.4f, RandRotTwoPi).Spawn();
                        ExHoldItem(item, player, vp);
                    }
                }
            }
        }

        public virtual void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {

        }
    }
}
