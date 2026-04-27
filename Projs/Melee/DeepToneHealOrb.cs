using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneHealOrb : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public ref float Timer => ref Projectile.ai[0];
        public bool IsInter = false;
        public int BounceTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = GetSeconds(3);
        }
        private float SearchDistance = 460f;
        private int TotalBounceTime = 2;
        private float KillDistance = 1800f;
        public bool UseHeal = false;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Green);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > 15f)
            {
                float homingSpeed = Clamp(Timer - 15f, 5f, 12f);
                if (!UseHeal)
                {
                    if (Projectile.GetTargetSafe(out NPC target, true, SearchDistance))
                        Projectile.HomingTarget(target.Center, -1, homingSpeed + 5f, 20f);
                    else
                        Projectile.Kill();
                }
                else
                {
                    if (!IsInter)
                    {
                        Projectile.HomingTarget(Owner.Center, -1, homingSpeed, 5f);
                        if (Projectile.IntersectOwnerByDistance(30f))
                        {
                            Owner.Heal(Main.rand.Next(1, 3));
                            IsInter = true;
                        }
                    }
                    else
                    {
                        Projectile.velocity *= 0.001f;
                        Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.2f);
                        Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.10f);
                        if (Projectile.Opacity < 0.02f)
                            Projectile.Kill();
                    }
                }

            }
            else
            {
                Projectile.velocity *= 0.96f;
            }
            //粒子
            if (Projectile.IsOutScreen() || Main.rand.NextFloat() > Projectile.Opacity)
                return;
            if (Main.rand.NextBool(3))
            {
                //new SmokeParticle(Projectile.Center.ToRandCirclePosEdge(3f), Projectile.velocity / 8f, RandLerpColor(Color.DarkOliveGreen, Color.DarkSeaGreen), 40, RandRotTwoPi, 1, Main.rand.NextFloat(0.2f, 0.4f) * 0.255f, true).Spawn();

            }
            if (Main.rand.NextBool(6))
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(3), Projectile.velocity / 9f * Main.rand.NextFloat(0.5f, 1.3f), RandLerpColor(Color.DarkGreen, Color.DarkSeaGreen), 40, 0, 1, Main.rand.NextFloat(0.7f, 0.95f) * 0.45f, false).Spawn();
            }

            //距离玩家过远时处死
            //if (Projectile.TooAwayFromOwner(KillDistance))
            //Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(BuffID.OnFire, GetSeconds(5));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                new TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16f), 1.2f, RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 20, 0.5f, RandRotTwoPi).Spawn();
            }
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            Color baseColor = Color.DarkSeaGreen;
            Color targetColor = Color.DarkOliveGreen;
            //绘制残影
            float oriScale = 0.45f;
            float scale = 1f;
            int length = 15;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.925f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(baseColor, targetColor, (1 - rads)).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.40f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }

            SB.Draw(projTex, projPos, null, Color.DarkSeaGreen.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.75f, 0, 0);
            return false;
        }
        public override bool? CanDamage()
        {
            return Timer > 20f;
        }
    }
}
