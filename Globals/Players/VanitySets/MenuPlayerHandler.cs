using HJScarletRework.Globals.List;
using HJScarletRework.Items.Vanity.Arceca;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players.VanitySets
{
    public abstract class MenuVanityPlayer : ModPlayer
    {
        public virtual int VanityItemType => -1;
        public bool Equipped = false;
        public override void ResetEffects()
        {
            Equipped = false;
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(Equipped), Equipped);
        }
        public override void LoadData(TagCompound tag)
        {
            Equipped = tag.GetBool(nameof(Equipped));
        }
        public override void FrameEffects()
        {
            bool equip = false;
            if (Main.gameMenu)
            {
                GetArmorSlotItem(ref equip);
            }
            if (equip && VanityItemType != -1)
            {
                string name = HJScarletList.VanityItemDictionary[VanityItemType];
                Player.legs = EquipLoader.GetEquipSlot(Mod, name, EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, name, EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, name, EquipType.Head);
                ExArmor();
            }
        }
        public virtual void ExArmor()
        {

        }
        public void GetArmorSlotItem(ref bool equip)
        {
            foreach (Item item in Player.armor)
            {
                if (item.type == VanityItemType)
                {
                    equip = true;
                    break;
                }
            }
        }
    }
    public class TairitsuPlayer : MenuVanityPlayer
    {
        public override int VanityItemType => ItemType<TairitsuItem>();
    }
    public class HikariPlayer : MenuVanityPlayer
    {
        public override int VanityItemType => ItemType<HikariItem>();
    }
    //public class CantonesePlayer : MenuVanityPlayer
    //{
    //    public override int VanityItemType => ItemType<CantoneseGirlItem>();
    //}
    //public class LeafMaidPlayer : MenuVanityPlayer
    //{
    //    public override int VanityItemType => ItemType<LeafMaidItem>();
    //}
    //public class RedDragonPlayer : MenuVanityPlayer
    //{
    //    public override int VanityItemType => ItemType<RedDragonItem>();
    //}
}
