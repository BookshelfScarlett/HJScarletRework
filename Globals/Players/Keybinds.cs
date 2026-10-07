using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.NetCode;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using HJScarletRework.Projs.ParryShield;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public bool CanSwitchWeaponType = false;
        public bool CanRevisual = false;
        public bool CanExecution = false;
        public bool CanWeaponSpecialAbility = false;
        public bool CanArmorAbility = false;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if(HJNetInput.ParryCurrent&&!Player.HasProj<CobaltParryShield>())
            {
                Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<CobaltParryShield>(), 5, 1, Player.whoAmI);
            }
            if (HJNetInput.SkillJustPressed)
            {
                if (defenderEmblemCD == 0 && emblemVanguard)
                {
                    Player.GetImmnue(ImmunityCooldownID.General, GetSeconds(EmblemVanguard.InvinceTime), false);
                    ScarletSound(HJScarletSounds.GrabCharge, Player.Center, instances: 0);
                    defenderEmblemCD = GetSeconds(EmblemVanguard.Cooldown + EmblemVanguard.InvinceTime);
                    for (int i = 0; i < 30; i++)
                        ECSParticle.TurbulenceShinyOrb(Player.Center.ToRandCirclePos(15), 2.4f, Color.White, 120, 1, 0.3f, RandRotTwoPi);
                    new GeneralMountedPlayerParticle(HJScarletTexture.Particle_RingShiny.Value, Color.White, 0, .24f, .8f, GetSeconds(3), Player.whoAmI, BlendStateID.Additive).Spawn();
                }
            }
            if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
            {
                bool tier1 = PiorityTier1();
                if (tier1)
                {
                    return;
                }
                bool tier2 = PiorityTier2();
                if (tier2)
                    return;
            }
        }
        /// <summary>
        /// 第一批优先度梯队：物品切换，代行者手动触发的处刑攻击
        /// </summary>
        /// <returns></returns>
        private bool PiorityTier1()
        {
            bool anyPiorityTier1Active = false;
            //按情况提供硬编码
            //第一批顺序：查看武器转化
            //获取这个列表的第一个值，对比玩家手持的情况，符合则打一个标记出去
            int heldItem = Player.HeldItem.type;
            bool hasValue = WeaponSwapMaps.ContainsKey(heldItem) || ArmorMaps.Contains(heldItem) || WeaponSwapMaps.ContainsValue(heldItem);
            if (!CanSwitchWeaponType && hasValue)
            {
                //打一个标记出去
                CanSwitchWeaponType = true;
                anyPiorityTier1Active = true;
            }
            if (tacticalExecutionInputCache == 0 && tacticalExecution)
            {
                //12帧的输入缓冲
                tacticalExecutionInputCache = 60;
                anyPiorityTier1Active = true;
            }
            if (!CanWeaponSpecialAbility && (heldItem == ItemID.MonkStaffT1 || heldItem == ItemID.MonkStaffT3 || Player.IsHolding<CrimsonScythe>()))
            {
                CanWeaponSpecialAbility = true;
                anyPiorityTier1Active = true;
            }
            return anyPiorityTier1Active;
        }
        /// <summary>
        /// 第二批优先梯队：盔甲技能，部分武器技能
        /// </summary>
        /// <returns></returns>
        private bool PiorityTier2()
        {
            bool anyPiorityTier2Active = false;
            if (!CanArmorAbility)
            {
                CanArmorAbility = true;
                anyPiorityTier2Active = true;
            }
            return anyPiorityTier2Active;
        }
    }
}
