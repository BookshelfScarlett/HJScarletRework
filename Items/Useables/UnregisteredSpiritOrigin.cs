using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class UnregisteredSpiritOrigin : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void ExSD()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 1;
        }
        public override void HoldItem(Player player)
        {
            if (player.HeldItem.type == Type && Main.playerInventory)
            {
                Item item = Main.HoverItem;
                //必须得有伤害，必须得是武器
                bool isWeapon = item.damage > 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0;
                //必须得有宝藏袋一名
                bool isTreasureBag = item.Name.Contains("TreasureBag");
                if (isWeapon || isTreasureBag)
                {
                    if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                    {
                        Item targetItem = new Item();
                        bool favor = player.HeldItem.favorited;
                        targetItem.SetDefaults(item.type);
                        targetItem.favorited = favor;

                        player.inventory[player.selectedItem] = item;
                        SoundEngine.PlaySound(SoundID.ResearchComplete, player.Center);
                        for (int i = 0; i < 20; i++)
                            new TurbulenceGlowOrb(player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
                    }
                }

            }
        }
    }
}
