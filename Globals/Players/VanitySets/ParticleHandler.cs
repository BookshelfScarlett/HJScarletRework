using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Vanity.Arceca;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using rail;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.VanitySets
{
    public partial class ScarletVanityPlayer : ModPlayer
    {
        public float IdleTimer = 0;
        public const int TotalIdleTime = 600;
        public bool InIdleStatement = false;
        public override void PostUpdateEquips()
        {
            if (IdleTimer < TotalIdleTime)
                return;
            if (accVanityID != -1)
            {
                if (accVanityID == ItemType<TairitsuItem>() && !Player.HasProj<TairitsuProj>())
                {
                    
                    int id = ProjectileType<TairitsuProj>();
                    float beginRotation = Player.direction > 0 ? 0 : Pi;
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, id, 0, 0, Player.whoAmI);
                    proj.ai[1] = beginRotation;
                }
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (accVanityID != -1)
            {
                bool isIdle = (Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y)) < 5;
                if (Main.mouseLeft || Main.mouseRight)
                    isIdle = false;
                if (isIdle)
                {
                    if (IdleTimer > TotalIdleTime)
                    {
                        IdleTimer = TotalIdleTime;
                        InIdleStatement = true;
                    }
                    else
                    {
                        IdleTimer += 1;
                    }
                }
                else
                    IdleTimer = 0;
            }
        }

    }
}
