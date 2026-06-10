using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Core;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheRuinFrost : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int LifeMax2 = 50;
        public override void SetStaticDefaults()
        {
            HJScarletList.FrostRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.accessory = true;
            Item.expert = true;
            Item.defense = 10;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeMax2);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            player.statLifeMax2 += LifeMax2;
            player.ApplyDash(ScarletContent.DashType<HeartoftheRuinFrostDash>());
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.resistCold = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.breath = 100;
            }
            if (player.statLife <= player.statLifeMax2 / 2)
                player.AddBuff(BuffID.IceBarrier, 5);

            //符合条件就启用圣骑士盾效果
            if (player.statLife > player.statLifeMax2 * 0.25f)
            {
                player.hasPaladinShield = true;
                if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                {
                    int myPlayer = Main.myPlayer;
                    if (Main.player[myPlayer].team == player.team && player.team != 0)
                    {
                        float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                        float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                        if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < 800f)
                            Main.player[myPlayer].AddBuff(BuffID.PaladinsShield, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HeartoftheDistantArk>().
                AddIngredient(ItemID.FrozenShield).
                AddIngredient<EssenceofMatter>(15).
                AddTile<FountainofMatter>().
                Register();
        }
    }
}
