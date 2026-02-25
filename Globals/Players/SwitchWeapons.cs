using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        //这里是硬打表，而且本质上我们也确实只需要硬打表……
        private static readonly Dictionary<int, int> WeaponSwapMaps = new Dictionary<int, int>()
        {
            { ItemType<CandLanceThrown>(), ItemType<Candlance>() },
            { ItemType<DeepToneThrown>(), ItemType<DeepTone>() },
            { ItemType<DialecticsThrown>(), ItemType<Dialectics>() },
            { ItemType<FlybackHandThrown>(), ItemType<FlybackHand>() },
            { ItemType<EvolutionThrown>(), ItemType<Evolution>()  },
            { ItemType<SpearofDarknessThrown>(), ItemType<SpearOfDarkness>() },
            { ItemType<SweetStabThrown>(), ItemType<SweetSweetStab>() },
            { ItemType<WildPointerThrown>(), ItemType<WildPointer>() },
            { ItemType<TonbogiriThrown>(), ItemType<Tonbogiri>() },
            { ItemType<GalvanizedHandThrown>(), ItemType<GalvanizedHand>() },
            { ItemType<SpearofEscapeThrown>(),  ItemType<SpearOfEscape>() },
            { ItemType<LightBiteThrown>() , ItemType<LightBite>() }
        };
        //一个额外的工具方法，用于反向字典映射
        private static int GetReverseWeapon(int curType)
        {
            foreach (var (key, value) in WeaponSwapMaps)
            {
                if (value == curType)
                    return key;
            }
            return -1;
        }
        public void SwitchWeaponSystem()
        {
            //不过背包判定，因为下面的搜字典的判定相对占用高
            int ownerItemType = Player.HeldItem.type;
            //按下鼠标中键进行切换
            if (!Main.mouseMiddle)
                return;

            if (!Main.mouseMiddleRelease)
                return;
            //替换武器
            if (WeaponSwapMaps.TryGetValue(ownerItemType, out int targetWeapons))
            {
                ReplaceWeaponsOnNeed(targetWeapons, false);
                return;
            }
            int reverseWeapon = GetReverseWeapon(ownerItemType);
            if (reverseWeapon != -1)
            {
                ReplaceWeaponsOnNeed(reverseWeapon, true);
                return;
            }
        }
        private void ReplaceWeaponsOnNeed(int targetWeapons, bool alterPrefix)
        {
            Item targetWeapon = new Item();
            int heldPrefix = Player.HeldItem.prefix;
            bool favor = Player.HeldItem.favorited;
            if (alterPrefix)
            {
                if (heldPrefix == PrefixID.Demonic || heldPrefix == PrefixID.Godly)
                    heldPrefix = PrefixID.Legendary;
            }
            else if (heldPrefix == PrefixID.Legendary || heldPrefix == PrefixID.Godly)
                heldPrefix = PrefixID.Godly;

            targetWeapon.SetDefaults(targetWeapons);
            if (!targetWeapon.CanApplyPrefix(PrefixID.Legendary))
                heldPrefix = PrefixID.Godly;
            //继承词缀
            targetWeapon.Prefix(heldPrefix);
            targetWeapon.favorited = favor; 
            //直接……干掉玩家的物品
            Player.inventory[Player.selectedItem] = targetWeapon;
            SoundEngine.PlaySound(SoundID.ResearchComplete, Player.Center);
            for (int i = 0; i < 20; i++)
                new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
        }
    }
}
