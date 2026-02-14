using ContinentOfJourney.Buffs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class LifeBalloon : HJScarletItems
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void SetDefaults()
        {
            Item.width = Item.height = 30;
            Item.accessory = true;
            Item.value = 50;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 100;
            player.buffImmune[BuffType<PlagueBuff>()] = true;
            player.HJScarlet().LifeBalloonAcc = true;
            player.GetJumpState<LifeBalloonExJump>().Enable();
        }
    }
    public class LifeBalloonExJump : ExtraJump
    {
        public override Position GetDefaultPosition()
        {
            return new After(BlizzardInABottle);
        }
        public override float GetDurationMultiplier(Player player)
        {
            // Each successive jump has weaker power
            return player.HJScarlet().LifeBalloonAccJumps switch 
            {
                1 => 1.2f,
                2 => 1.5f,
                3 => 1.8f,
                4 => 3.9f,
                _ => 0f
            };
        }
        public override void OnRefreshed(Player player)
        {
            // Reset the jump counter
            player.HJScarlet().LifeBalloonAccJumps = 4;
        }
        public override void UpdateHorizontalSpeeds(Player player)
        {
            base.UpdateHorizontalSpeeds(player);
        }
        public override void OnStarted(Player player, ref bool playSound)
        {
            // Get the jump counter
            ref int jumps = ref player.HJScarlet().LifeBalloonAccJumps;

            // Spawn rings of fire particles
            int offsetY = player.height;
            if (player.gravDir == -1f)
                offsetY = 0;

            offsetY -= 16;

            Vector2 center = player.Top + new Vector2(0, offsetY);

            if (jumps == 3)
            {
                const int numDusts = 40;
                for (int i = 0; i < numDusts; i++)
                {
                    (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                    float amplitudeX = cos * (player.width + 10) / 2f;
                    float amplitudeY = sin * 6;

                    Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.5f, Scale: 1f);
                    dust.noGravity = true;
                }
            }
            else if (jumps == 2)
            {
                const int numDusts = 30;
                for (int i = 0; i < numDusts; i++)
                {
                    (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                    float amplitudeX = cos * (player.width + 2) / 2f;
                    float amplitudeY = sin * 5;

                    Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.35f, Scale: 1f);
                    dust.noGravity = true;
                }
            }
            else
            {
                const int numDusts = 24;
                for (int i = 0; i < numDusts; i++)
                {
                    (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians(i * 360 / numDusts));

                    float amplitudeX = cos * (player.width - 4) / 2f;
                    float amplitudeY = sin * 3;

                    Dust dust = Dust.NewDustPerfect(center + new Vector2(amplitudeX, amplitudeY), DustID.BlueFlare, -player.velocity * 0.2f, Scale: 1f);
                    dust.noGravity = true;
                }
            }

            // Play a different sound
            playSound = false;

            float pitch = jumps switch
            {
                1 => 0.5f,
                2 => 0.1f,
                3 => -0.2f,
                _ => 0
            };

            SoundEngine.PlaySound(SoundID.Item8 with { Pitch = pitch, PitchVariance = 0.04f }, player.Bottom);

            // Decrement the jump counter
            jumps--;

            // Allow the jump to be used again while the jump counter is > 0
            if (jumps > 0)
                player.GetJumpState(this).Available = true;
        }

    }
}
