using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.Dashes
{
    public class HeartoftheRuinFrostDash : PlayerDashClass
    {
        public override int ImmuneTime(Player player) => 0;
        public override int DashTime(Player player) => 12;
        public override int DashDelay(Player player) => 12;
        public override DashEnum DashOnHitType => DashEnum.Bonk;
        public override DashDamageInfo DashDamageInfo(Player player)
        {
            return new DashDamageInfo(100, 3f, DamageClass.Generic);
        }
        public override float DashSpeed(Player player) => 32f;
        public override float DashEndSpeedMult(Player player) => 0.5f;
        public override void OnDashStart(Player player)
        {
            SoundEngine.PlaySound(HJScarletSounds.Frostwave_LightRelease with { MaxInstances = 0,PitchVariance=.2f });
        }
        public override void OnDashEnd(Player player)
        {
            base.OnDashEnd(player);
        }
        public override void UpdateDash(Player player)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke);
                ECSParticle.SnowCloud(pos2, vel2, color2, 40, 0, .75f, 0.1f * 0.85f);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke);
                ECSParticle.HRShinyOrb(pos2, vel2, color2, 60, .81f, 0.08f, 0.85f);

            }
        }
        public override void OnHitNPC(Player player, NPC target, int DamageDone)
        {
            player.velocity = new Vector2(-player.velocity.X * .75f, -10f);
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with {MaxInstances = 0,PitchVariance = .2f });
            int length = 32;
            for(int i =0;i<length;i++)
            {
                Vector2 pos2 = target.Center.ToRandCirclePos(16f);
                Vector2 vel2 = target.Center.GetNormalVector2(pos2) * Main.rand.NextFloat(1f, 6f);
                Color color2 = RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke);
                ECSParticle.SnowCloud(pos2, vel2, color2, 40, 0, .85f, 0.4f * 0.25f);
            }
            new ShinyRing(target.Center, Vector2.Zero, Color.White,40, 0.15f,opacity:.75f,fadeIn:true).Spawn();
            new ShinyRing(target.Center, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 40, 0.15f,fadeIn:true).SpawnToPriorityNonPreMult();
            for (int i = 0;i<4;i++)
            {
                for (int j =0;j<30;j++)
                {
                    Vector2 pos = target.Center.ToRandCirclePos(3);
                    Vector2 vel = Vector2.UnitX.RotatedBy(i * PiOver2) * Main.rand.NextFloat(-1f, 4.8f);
                    Color color = RandLerpColor(Color.RoyalBlue, Color.LightBlue);
                    float scale = Main.rand.NextFloat(.8f, 1.1f) * 1.1f;
                    int lifeTime = Main.rand.Next(45, 60);
                    new StarShape(pos, vel, color, scale, lifeTime).Spawn();
                }
            }
        }
    }
}
