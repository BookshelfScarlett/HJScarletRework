using ContinentOfJourney.Items;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using HJScarletRework.Core.Keybinds;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.ExecutorAlter;
using ContinentOfJourney;

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
        private static readonly List<int> ArmorMaps = new List<int>()
        {
            ItemID.RuneHat,
            ItemID.RuneRobe,
            ItemID.CowboyHat,
            ItemID.CowboyJacket,
            ItemID.CowboyPants,
            ItemID.FloretProtectorHelmet,
            ItemID.FloretProtectorChestplate,
            ItemID.FloretProtectorLegs,
            ItemID.RainHat,
            ItemID.RainCoat,
            ItemID.FishCostumeMask,
            ItemID.FishCostumeShirt,
            ItemID.FishCostumeFinskirt
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
            if (!CanSwitchWeaponType)
                return;
            if (!HJScarletKeybinds.GeneralActionKeybind.JustReleased)
                return;
            CanSwitchWeaponType = false;
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
            if (SwitchArmorType(ownerItemType))
                return;
            
        }
        public bool SwitchArmorType(int item)
        {
            if (!ArmorMaps.Contains(item))
                return false;
            //这里实际上没什么办法，只能这样打表
            switch (item)
            {
                case ItemID.RuneHat:
                    AlterArmorType(item, 14, false);
                    break;
                case ItemID.RuneRobe:
                    AlterArmorType(item, 22, false);
                    break;
                case ItemID.CowboyHat:
                    AlterArmorType(item, CowboyHelmet.Defense, false, ItemRarityID.Orange);
                    break;
                case ItemID.CowboyJacket:
                    AlterArmorType(item, CowboyChestplate.Defense, false, ItemRarityID.Orange);
                    break;
                case ItemID.CowboyPants:
                    AlterArmorType(item, CowboyHelmet.Defense, false, ItemRarityID.Orange);
                    break;
                case ItemID.RainHat:
                    AlterArmorType(item, RaincoatHelmet.Defense, false);
                    break;
                case ItemID.RainCoat:
                    AlterArmorType(item, RaincoatChestplate.Defense, false);
                    break;
                case ItemID.FishCostumeMask:
                    AlterArmorType(item, FishCostumeHelmet.Defense, false);
                    break;
                case ItemID.FishCostumeShirt:
                    AlterArmorType(item, FishCostumeChestplate.Defense, false);
                    break;
                case ItemID.FishCostumeFinskirt:
                    AlterArmorType(item, FishCostumeLegs.Defense, false);
                    break;
            }
            if (DownedBossSystem.downedLifeGod)
            {
                switch (item)
                {
                    case ItemID.FloretProtectorHelmet:
                        AlterArmorType(item, FloretProtectorHelmetAlter.Defense, false, ItemRarityID.Red);
                        break;
                    case ItemID.FloretProtectorChestplate:
                        AlterArmorType(item, FlorectProtectorChestplateAlter.Defense, false, ItemRarityID.Red);
                        break;
                    case ItemID.FloretProtectorLegs:
                        AlterArmorType(item, FlorectProtectorLegsAlter.Defense, false, ItemRarityID.Red);
                        break;
                }
            }
        return true;
        }
        private void AlterArmorType(int targetArmor, int defense = 0, bool vanity = true, int rarityID = -1)
        {
            Item targetItem = new Item();
            bool favor = Player.HeldItem.favorited;
            bool alterVersion = Player.HeldItem.HJScarlet().EnableExecutorVersion;
            if (alterVersion)
            {
                targetItem.SetDefaults(targetArmor);
            }
            else
            {
                targetItem.SetDefaults(targetArmor);
                targetItem.vanity = vanity;
                targetItem.HJScarlet().EnableExecutorVersion = true;
                targetItem.defense = defense;
                targetItem.favorited = favor;
                if(rarityID != -1)
                    targetItem.rare = rarityID;
            }
            Player.inventory[Player.selectedItem] = targetItem;
            SoundEngine.PlaySound(SoundID.ResearchComplete, Player.Center);
            for (int i = 0; i < 20; i++)
                new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();

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
            if (!targetWeapon.CanApplyPrefix(PrefixID.Legendary) && heldPrefix == PrefixID.Legendary)
            {
                heldPrefix = PrefixID.Godly;
            }
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
