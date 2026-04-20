using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class HerbBagBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HJScarlet().protectorPlantID == -1)
                return;
            switch (player.HJScarlet().protectorPlantID)
            {
                case ItemID.Daybloom:
                    player.lifeRegen += 8;
                    player.statDefense += 30;
                    Lighting.AddLight(player.Center, TorchID.White);
                    UpdateDaybloomParticle(player);
                    break;
                case ItemID.Moonglow:
                    player.endurance += 0.12f;
                    player.aggro -= 1000;
                    player.HJScarlet().protectorMoonglow = true;
                    UpdateMoonglowParticle(player);
                    break;
                case ItemID.Deathweed:
                    player.GetDamage<ExecutorDamageClass>() += 0.20f;
                    player.GetCritChance<ExecutorDamageClass>() += 20f;
                    player.aggro += 500;
                    UpdateDeathWeedParticle(player);
                    break;
                case ItemID.Fireblossom:
                    if (Collision.LavaCollision(player.Center, player.width, player.height))
                    {
                        player.GetDamage<ExecutorDamageClass>() += 0.35f;
                        player.GetCritChance<ExecutorDamageClass>() += 35f;
                    }
                    UpdateFireblossomParticle(player);
                    break;
                case ItemID.Waterleaf:
                    player.luck += 50;
                    UpdateWaterleafParticle(player);
                    break;
                case ItemID.Blinkroot:
                    player.pickSpeed -= 0.50f;
                    UpdateBlinkrootParticle(player);
                    break;
                case ItemID.Shiverthorn:
                    player.HJScarlet().protectorShiver = true;
                    UpdateShiverthornParticle(player);
                    break;
            }
        }

        private void UpdateBlinkrootParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkGoldenrod, Color.Gold), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkGoldenrod, Color.Gold), 0.65f, 40).Spawn();
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkGoldenrod, Color.Gold), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

        }

        private void UpdateWaterleafParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 0.65f, 40).Spawn();
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkSeaGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();
        }

        private void UpdateFireblossomParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.75f).Spawn();
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkOrange, Color.Black), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

        }

        private void UpdateDeathWeedParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.DarkViolet, Color.Purple), 0.65f, 40).Spawn();
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

        }

        private void UpdateMoonglowParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 0.65f, 40).Spawn();
            if (Main.rand.NextBool())
                new KiraStar(spawnPos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 41, 0, 1, 0.051f * 0.75f, useAlt: true).Spawn();

        }

        private void UpdateShiverthornParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 0.65f, 40).Spawn();
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.24f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, RandRotTwoPi, 1f, 0.20f).SpawnToNonPreMult();

        }

        private void UpdateDaybloomParticle(Player player)
        {
            Vector2 spawnPos = player.Center + RandVelTwoPi(10f, 28f);
            Vector2 dir = -Vector2.UnitY;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool())
                new ShinyOrbParticle(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.Lime, Color.LimeGreen), 40, 0.65f).Spawn();
            if (Main.rand.NextBool())
                new StarShape(spawnPos - dir * 10f, dir * Main.rand.NextFloat(0.7f, 1.84f), RandLerpColor(Color.Lime, Color.LimeGreen), 0.65f, 40).Spawn();
        }
    }
}
