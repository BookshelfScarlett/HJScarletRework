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

namespace HJScarletRework.Items.Armor.Shinobi
{

    [AutoloadEquip(EquipType.Legs)]
    public class ShinobiLegs : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void ExSD()
        {
            Item.width = Item.height = 40;
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 30;
        }
        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
        }
        
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<ExecutorDamageClass>() += 30f;
            player.moveSpeed += 0.3f;
        }

    }
}
