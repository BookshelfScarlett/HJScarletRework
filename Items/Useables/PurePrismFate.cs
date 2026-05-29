using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Keybinds;
using HJScarletRework.Globals.List;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class PurePrismFate : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public static List<int> _RefusedList =
            [
                ItemID.GoldenKey,
                ItemID.ShadowKey,
                ItemID.CorruptionKey,
                ItemID.CrimsonKey,
                ItemID.JungleKey,
                ItemID.DungeonDesertKey,
                ItemID.FrozenKey,
                ItemID.HallowedKey,
                ItemID.WormFood,
                ItemID.SlimeCrown,
                ItemID.DeerThing,
                ItemID.Abeemination,
                ItemID.BloodySpine,
                ItemID.SuspiciousLookingEye,
                ItemID.QueenSlimeCrystal,
                ItemID.MechanicalEye,
                ItemID.MechanicalWorm,
                ItemID.MechanicalSkull,
                ItemID.LihzahrdPowerCell,
                ItemID.TruffleWorm,
                ItemID.EmpressButterfly,
                ItemID.CelestialSigil,
                ItemID.PumpkinMoonMedallion,
                ItemID.NaughtyPresent,
                ItemType<MetalSpine>(),
                ItemType<UnstableGlobe>(),
                ItemType<CannedSoulofFlight>(),
                ItemType<MaliciousPacket>(),
                ItemType<PurpleFlareGun>()
            ];
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, RareItemRarity.RareType.White);
        }

        public override void ExSD()
        {
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
        }
        public override void HoldItem(Player player)
        {
            Item item = Main.HoverItem;
            if (player.HeldItem.stack < 3)
                return;
            //必须得是材料。必须得没有伤害，必须得不是饰品，必须得什么都不会发射，必须得没有任何Buff提供，必须得可叠加（最大叠加数小于零）
            //必须得不能放置任何墙体
            bool isMate = item.material && item.damage < 1 && !item.accessory && item.shoot == ProjectileID.None && item.buffType == 0 && item.maxStack > 1 && item.createWall == -1;
            if ((isMate && !_RefusedList.Contains(item.type)) || SmeltList.BarType.Contains(item.type))
            {
                if (HJScarletKeybinds.GeneralActionKeybind.JustPressed)
                {
                    int totalStack = 0;
                    for (int i = 0; i < player.HeldItem.stack; i++)
                    {
                        if (i == 300)
                            break;
                        if (i % 3 == 0)
                        {
                            totalStack++;
                        }
                    }
                    player.HeldItem.stack -= (totalStack * 3);
                    Item targetItem = new Item();
                    bool favor = player.HeldItem.favorited;
                    targetItem.SetDefaults(item.type);
                    targetItem.favorited = favor;
                    player.QuickSpawnItemDirect(player.GetSource_FromThis(), targetItem, totalStack);
                    SoundEngine.PlaySound(SoundID.ResearchComplete, player.Center);
                    for (int i = 0; i < 20; i++)
                        new TurbulenceGlowOrb(player.Center.ToRandCirclePos(30), 1.2f, Color.White, 45, 0.1f, RandRotTwoPi).Spawn();
                }
            }
        }
    }
}
