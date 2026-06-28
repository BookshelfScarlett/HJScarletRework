using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class StarofHope : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 20;
        public override WeaponCategory WeaponCategory => WeaponCategory.Minion;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 2;
            Item.UseSound = SoundID.Item152;
            Item.damage = 20;
            Item.shootSpeed = 12f;
        }
    }
}
