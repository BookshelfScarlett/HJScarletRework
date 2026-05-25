using HJScarletRework.Globals.List;
using HJScarletRework.Items.Armor.Vanity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void FrameEffects()
        {
            if (accVanityID != -1)
            {
                UpdateVanityItem();
            }

        }
        public override void UpdateVisibleVanityAccessories()
        {
            bool isPausingGame = Main.gamePaused || Main.autoPause;
            if (accVanityID != -1 && isPausingGame)
                UpdateVanityItem();
        }
        public void UpdateVanityItem()
        {
            string name = HJScarletList.VanityItemDictionary[accVanityID];
            //怎么都是特殊情况。
            if(name == nameof(TairitsuItem))
                Player.back = EquipLoader.GetEquipSlot(Mod, name, EquipType.Back);
            Player.legs = EquipLoader.GetEquipSlot(Mod, name, EquipType.Legs);
            Player.body = EquipLoader.GetEquipSlot(Mod, name, EquipType.Body);
            Player.head = EquipLoader.GetEquipSlot(Mod, name, EquipType.Head);
        }
    }
}
