using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Monk
{
    [AutoloadEquip(EquipType.Body)]
    public class MonkBody:HJScarletArmor 
    {
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 22;
        }
        
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += 30;
        }
    }
}
