using ContinentOfJourney.Buffs;
using ContinentOfJourney.Items.Pylons;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using rail;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class DuskfallProj : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override string Texture => GetInstance<Duskfall>().Texture;
        public int PreIvalA = 1;
        public ref float Timer => ref Projectile.ai[0];
        public ref float RandMap => ref Projectile.ai[1];
        public bool AcceptIval
        {
            get => Projectile.ai[2] == 1;
            set => Projectile.ai[2] = value ? 1 : 0;
        }
        public List<float> OldRotList = [];
        public ref float SpriteRotation => ref Projectile.localAI[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 4;
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override Vector2 TileHitbox => new(16);
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            if(Projectile.HJScarlet().CurStoredTarget.IsLegal())
            Projectile.tileCollide = false;
            return false;
        }
        public override void OnFirstFrame()
        {
            RandMap = Main.rand.NextFloat(.3f, .61f);
            base.OnFirstFrame();
        }
        public override void OnSpawn(IEntitySource source)
        {
        }
        public static bool isThrowingItem(Item item, out bool Knife, bool excludeStake = false)
        {
            Knife = false;

            if ((!item.CountsAsClass(DamageClass.Ranged) || !item.consumable) && !item.CountsAsClass(DamageClass.Throwing))
            {
                if (item.type == 1835)
                {
                    return !excludeStake;
                }

                return false;
            }

            return true;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            OldRotList.Add(Projectile.rotation);
            if (OldRotList.Count > Projectile.oldPos.Length)
                OldRotList.RemoveAt(0);
            SpriteRotation += .2f * (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = SpriteRotation;
            Timer++;
            //if (AcceptIval)
            {
                NPC CurTarget = Projectile.HJScarlet().CurStoredTarget;
                if (CurTarget.IsLegal()&&Timer>Projectile.MaxUpdates*15)
                {
                    float speedValue = Projectile.velocity.Length();
                    float rotation = Projectile.velocity.ToRotation();
                    float angleTo = Projectile.AngleTo(CurTarget.Center);
                    float dist = Projectile.Distance(CurTarget.Center);
                    float r = dist * RandMap / (float)Math.Abs(Math.Sin(rotation - angleTo));
                    if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(CurTarget.Center)) < 0)
                    {
                        r = Clamp(r, 1, 240);
                    }
                    Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(rotation - angleTo)) * speedValue / r);
                    if (Projectile.velocity.LengthSquared() < 11f * 11f)
                        Projectile.velocity *= 1.01f;
                    else
                        Projectile.velocity *= 0.9f;
                }
                else
                {
                    if (Projectile.GetTargetSafe(out NPC tar, true, 1200f, true))
                    {
                        Projectile.HJScarlet().CurStoredTarget = tar;
                    }
                }
            }
            var instance = Projectile.GetGlobalProjectile<CoJGlobalProjectile_Instance>().iValA;
            if (Main.rand.NextBool(6))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(24), DustID.OrangeTorch);
                d.velocity = Projectile.velocity / 4f;
                d.scale = 1f;
                d.noGravity = true;
                //ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePos(24), Projectile.velocity / 4f, RandLerpColor(Color.Orange, Color.OrangeRed), 45, 1, .38f, 0);
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(24);
                ECSParticle.TurbulenceShinyOrb(pos, 1.5f, RandLerpColor(Color.Orange,Color.OrangeRed), 45, 1, .1f, glowMult: .4f);
                ECSParticle.ShrinkParticle(pos, Projectile.velocity / 4f, RandLerpColor(Color.Orange, Color.OrangeRed), 45, .51f, Projectile.velocity.ToRotation(), .6f, 1);
                //ECSParticle.ShrinkParticle(pos, Projectile.velocity / 4f, Color.White, 45, 0.75f, Projectile.velocity.ToRotation(), .20f, 1);
            }
            if (Projectile.IsMe())
            {
                if (PreIvalA != instance && !AcceptIval)
                {
                    AcceptIval = true;
                    Projectile.netUpdate = true;
                    float speed = Projectile.velocity.Length();
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDir().RotatedBy(ToRadians(15f) * i) * speed, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        ((DuskfallProj)proj.ModProjectile).AcceptIval = true;
                    }
                }
            }
            if (AcceptIval)
            {
                if (Main.rand.NextBool(3))
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(16), Projectile.velocity / 6f, RandLerpColor(Color.OrangeRed, Color.Orange), 45, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .4f, false, blendstate: BlendState.Additive);
            }
            PreIvalA = instance;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Timer = Main.rand.Next(-10,16);
            RandMap = Main.rand.NextFloat(.3f, .61f);
            Projectile.tileCollide = true;
            target.AddBuff(BuffType<SolarBurntBuff>(), 60);
                        for (int i = 0; i < 24; i++)
            {
                Vector2 vel = (TwoPi / 24f * i).ToRotationVector2() * 8f * Main.rand.NextFloat(0f, 1f);

                Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 2f;
                Color color = RandLerpColor(Color.Lerp(Color.Orange, Color.Red, 0.50f), Color.Orange);
                float scale = 0.40f * Main.rand.NextFloat(0.55f, 1.1f);
                ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f), scale, true, BlendState.Additive);
            }
            for (int j = 0; j < 15; j++)
            {
                Vector2 dir = RandVelTwoPi(.1f, 4.9f);
                Vector2 pos = Projectile.Center.ToRandCirclePos(3f) + dir * Main.rand.NextFloat(0f, 3f);
                ECSParticle.ShinyCrossStarECS(pos, dir, RandLerpColor(Color.Orange, Color.OrangeRed), Main.rand.Next(15, 50), 1f, 1f * Main.rand.NextFloat(.7f, .9f), .2f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .99f * Main.rand.NextFloat(.6f, 1f), .2f);
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(.1f, 4.9f);
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Lerp(Color.Red, Color.Orange, .5f), Color.OrangeRed), Main.rand.Next(15, 50), 1f, .15f * Main.rand.NextFloat(.6f, 1f), .5f);
            }
            ScarletSound(SoundID.DD2_BetsyFireballImpact, Projectile.Center, 0.5f, 1, .35f);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public BlendState BlendState => BlendState.Additive;
        public ScarletDrawLayer LayerToRenderTo => ScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (!Projectile.HJScarlet().FirstFrame||!AcceptIval)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DarkOrange);
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, Color.OrangeRed);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.White);
            HJScarletMethods.EndShaderAreaPixel();


        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            if(AcceptIval)
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //if (AcceptIval)
            {
                int length = Projectile.oldPos.Length-5;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = (1f - i / (float)length);
                Vector2 oldpos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color c = Color.Lerp(Color.Orange, Color.DarkOrange, ratios).ToAddColor(10);
                float oldscale = Lerp(.05f, 1f, ratios);
                float opa = Lerp(.05f, 1f, ratios) * Projectile.Opacity;
                Vector2 sharpScale = new Vector2(1f, 1f);
                Vector2 sharpPos = oldpos - new Vector2(0, 0).RotatedBy(Projectile.oldRot[i]);
                for (int j = -1; j < 2; j += 2)
                {
                    float oldRot = Projectile.oldRot[i];
                    SB.FastDraw(tex, sharpPos, c.ToAddColor(50) * opa, oldRot, tex.Size()/2f, sharpScale * oldscale, 0);
                    SB.FastDraw(tex, sharpPos, Color.White.ToAddColor(10) * opa, oldRot, tex.Size()/2f, sharpScale * oldscale * .5f, 0);
                }
            }
                for (int i = 0; i < 8; i++)
                    SB.FastDraw(tex, drawPos + ((TwoPi / 8f) * i).ToRotationVector2() * 2f, Color.White.ToAddColor(), Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0);

            }

            SB.FastDraw(tex, drawPos, Color.White, Projectile.rotation, tex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 170f*offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length - 1) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 50 * multipleSize * Projectile.scale), OldRotList[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }

    }
}
