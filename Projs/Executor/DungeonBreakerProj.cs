using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonBreakerProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<DungeonBreaker>().Texture;
        public AnimationStruct Helper = new AnimationStruct(3);
        public enum State
        {
            Shoot,
            Bounce,
            Return
        }
        public enum ProjAniSlot : int
        {
            ShootSlot,
            ReturnSlot
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 32;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 6;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 48;
            Helper.MaxProgress[2] = 60;
            Helper.MaxProgress[1] = 15;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.FinalUpdateNextBool())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(32), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, RandRotTwoPi, 1f, 0.5f, false, 0.2f).Spawn();
            if (Projectile.FinalUpdateNextBool())
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(32), Projectile.velocity.ToRandVelocity(ToRadians(15f), 2.4f), RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, .5f).Spawn();
            if (Projectile.FinalUpdateNextBool())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple);
                d.scale *= Main.rand.NextFloat(0.75f, 1.2f);
            }
        }

        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Bounce:
                    DoBounce();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
            Projectile.spriteDirection = Projectile.direction = Math.Sign(Projectile.velocity.X);
        }

        public void DoShoot()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.MeetMaxUpdatesFrame(Timer, 15))
            {
                Vector2 dir = (Owner.Center - Projectile.Center).ToSafeNormalize();
                Projectile.velocity = dir.RotatedBy(ToRadians(30) * Main.rand.NextBool().ToDirectionInt()) * 24f;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Pitch = -0.5f}, Projectile.Center);
                GenerateBackDust(-1);
                UpdateToNextState(State.Return);
            }
        }
        public void DoReturn()
        {
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Helper.UpdateAniState(1);
            Projectile.rotation += 0.2f;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 12f);
            if (Projectile.IntersectOwnerByDistance(50))
            {
                if (Projectile.HJScarlet().AddFocusHit)
                    Owner.HJScarlet().ExecutionTime += 1;
                Projectile.Kill();
            }
        }
        public void DoBounce()
        {

            Projectile.tileCollide = false;
            if (!Helper.IsDone[0])
            {
                Projectile.rotation = Projectile.SpeedAffectRotation();
                Projectile.velocity *= 0.923f;

                if (Projectile.velocity.LengthSquared() < 10 * 10)
                    Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2.5f) * -Projectile.spriteDirection);
                Helper.UpdateAniState(0);
            }
            else
            {
                Vector2 velDir = (Owner.Center - Projectile.Center).ToSafeNormalize();
                Projectile.velocity = velDir * 8f;
                Projectile.timeLeft = GetSeconds(5);
                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0, Pitch = -0.15f }, Projectile.Center);
                UpdateToNextState(State.Return);
                GenerateBackDust(-1);
            }
        }
        public void UpdateToNextState(State id)
        {
            Projectile.netUpdate = true;
            AttackType = id;
            Timer *= 0;
        }
        public void GenerateBackDust(int reverse)
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(16);
                Vector2 dir = Projectile.velocity.ToRandVelocity(ToRadians(30f), 0f, 8f) * reverse;
                new ShinyOrbParticle(spawnPos, dir, RandLerpColor(Color.RoyalBlue, Color.MidnightBlue), 40, 0.8f).Spawn();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool updateToBounce = AttackType == State.Shoot || (AttackType == State.Return && !Helper.IsDone[1]);
            if (updateToBounce)
            {
                BounceUpdate();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<DungeonBreakerShockwave>(), Projectile.damage, Owner.whoAmI);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteEffects se = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotSe = Projectile.spriteDirection < 0 ? PiOver2 : 0;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, Projectile.Center + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.RoyalBlue.ToAddColor(), Projectile.rotation + PiOver4 + rotSe, tex.ToOrigin(), Projectile.scale, se, 0);
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                float ratios = ((float)i / (Projectile.oldPos.Length - 1));
                Color lerpColor = Color.Lerp(Color.White, Color.Blue, ratios);
                SB.Draw(tex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, lerpColor * (1 - ratios), Projectile.oldRot[i] + PiOver4 + rotSe, tex.ToOrigin(), Projectile.scale, se, 0);
            }

            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool updateToBounce = AttackType == State.Shoot;
            if (updateToBounce)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<DungeonBreakerShockwave>(), Projectile.damage, Owner.whoAmI);
                BounceUpdate();
                BreakTiles();
            }
            return false;
        }
        public void BounceUpdate()
        {
            Vector2 dir = (Owner.Center - Projectile.Center).ToSafeNormalize();
            Projectile.velocity = dir.RotatedBy(ToRadians(30) * Main.rand.NextBool().ToDirectionInt()) * 24f;
            GenerateBackDust(-1);
            SoundEngine.PlaySound(HJScarletSounds.Hammer_LightHit with { Pitch = -0.2f }, Projectile.Center);
            UpdateToNextState(State.Bounce);
        }

        private void BreakTiles()
        {
            List<int> tileTypeList =
                    [
                        TileID.CrackedPinkDungeonBrick,
                        TileID.CrackedGreenDungeonBrick,
                        TileID.CrackedBlueDungeonBrick,
                        TileID.Spikes
                    ];
            for (int i = -15; i < 15; i++)
            {
                Point pointToCheck = new Vector2(Projectile.Center.X + i, Projectile.Center.Y + i).ToTileCoordinates();
                Tile CurTiles = HJScarletMethods.GetTileCoord(pointToCheck.X, pointToCheck.Y);

                if (!CurTiles.HasTile || !tileTypeList.Contains(CurTiles.TileType))
                    continue;

                if (!WorldGen.CanKillTile(pointToCheck.X, pointToCheck.Y))
                    continue;

                WorldGen.KillTile(pointToCheck.X, pointToCheck.Y);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, pointToCheck.X, pointToCheck.Y);
            }
        }
    }
}
