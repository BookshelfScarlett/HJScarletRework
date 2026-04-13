using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ExsanguinationBulletProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public List<Vector2> PosList = [];
        public List<float> RotList = [];
        public int TotalListCount = 10;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 4;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(30);
            Projectile.timeLeft = 600;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!Projectile.HJScarlet().FirstFrame)
            {
                PosList.Clear();
                RotList.Clear();
                for (int i = 0; i < TotalListCount + 2; i++)
                {
                    PosList.Add(Vector2.Zero);
                    RotList.Add(0);
                }
            }
            PosList.Add(Projectile.Center);
            RotList.Add(Projectile.rotation);
            if (RotList.Count > TotalListCount + 2)
                RotList.RemoveAt(0);
            if (PosList.Count > TotalListCount + 2)
                PosList.RemoveAt(0);
            UpdateParticle();
        }

        private void UpdateParticle()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            //return;
            if (Main.rand.NextBool())
                return;
            if (Main.rand.NextBool(10))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(1), Projectile.velocity / 2f, RandLerpColor(Color.AliceBlue, Color.RoyalBlue), 40, Projectile.rotation,1,0.4f * Main.rand.NextFloat(0.5f,0.9f)).Spawn();
            if (Main.rand.NextBool(10))
                new StarShape(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity * Main.rand.NextFromList(0.8f,1.2f), RandLerpColor(Color.AliceBlue, Color.RoyalBlue),0.5f * Main.rand.NextFloat(0.5f,0.9f),40).Spawn();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0;
            bool hasBuff = Owner.HJScarlet().exsanguinationBuffTime != 0;
            SoundStyle style = hasBuff ? HJScarletSounds.Light_FleshHit with { MaxInstances = 4, Volume = 0.70f } : HJScarletSounds.Light_ShieldHit with { MaxInstances = 4, Volume = 0.70f };
            SoundEngine.PlaySound(style, Projectile.Center);
        }
        public override void OnKill(int timeLeft)
        {
            bool hasBuff = Owner.HJScarlet().exsanguinationBuffTime != 0;
            //初始化。
            if (!Owner.HJScarlet().ExecutionListStored.ContainsKey(ItemType<Exsanguination>()))
                Owner.HJScarlet().ExecutionListStored.TryAdd(ItemType<Exsanguination>(), 0);
            else if (!hasBuff)
                Projectile.AddExecutionTime(ItemType<Exsanguination>());
            if(!hasBuff)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4f), DustID.PlatinumCoin);
                    d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1f, 8f);
                    d.scale = 1.2f;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4f), Main.rand.NextBool(4) ? DustID.PlatinumCoin : DustID.Blood);
                    d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1f, 8f);
                    d.scale = 1.2f;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (PosList.Count < 1 && RotList.Count < 1)
                return false;
            //最顶端绘制一个star。
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Texture2D tex = Projectile.GetTexture();
            Vector2 scale = new Vector2(.6f, 1.4f);
            Vector2 ori = tex.Size() / 2;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.White, Color.White, rads) with { A = 200 }) * 0.9f * Projectile.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / ((float)PosList.Count - 4f), 0f, 1f) * 0.4f;
                Vector2 lerpPos = PosList[i] - Main.screenPosition;
                SB.Draw(tex, lerpPos, null, drawColor, RotList[i] + PiOver2, ori, Projectile.scale * (1f - rads), 0, 0);
            }
                SB.Draw(tex, drawPos, null, Color.White, Projectile.rotation + PiOver2, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
