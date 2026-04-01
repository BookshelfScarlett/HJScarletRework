using ContinentOfJourney.NPCs.Boss_PriestessRod;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class ClimaticHawstringMinion : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public float Osci = 0;
        public bool Reverse = false;
        public List<Vector2> PosList = [];
        public AnimationStruct Helper = new(3);
        public bool ShouldKillRods = false;
        public enum State
        {
            Idle,
            Attack,
            AFK
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float AFKTimer => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10000;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 40;
            Helper.MaxProgress[1] = 30;
            Helper.MaxProgress[2] = 30;
        }
        public override void ProjAI()
        {
            UpdateProjGeneralState();
            bool shouldAttack = GetTargetNearMouse(out NPC target) && PlayerIsAttacking && Helper.IsDone[0];
            if (!ShouldKillRods)
            {
                AttackState = shouldAttack ? State.Attack : State.Idle;
                //如果检测到玩家处于这个状态过久的话，我们会让飞棍重新飞回天空
                if (AttackState == State.Idle)
                {
                    AFKTimer++;
                    if (AFKTimer > GetSeconds(10))
                        AttackState = State.AFK;
                }
                AFKTimer *= (!PlayerIsAttacking).ToInt();
            }
            //简单查看玩家状态
            //在不满足条件的情况下，无论怎么样我们都强行把飞棍执行一个飞走的动画
            if(Owner.HeldItem.type != ItemType<ClimaticHawstring>() && !ShouldKillRods)
            {
                if (AttackState != State.AFK)
                    SoundEngine.PlaySound(SoundID.Item44 with { MaxInstances = 1, Pitch = -0.25f }, Owner.Center);
                ShouldKillRods = true;
                //此处直接启用afk的动作
                AttackState = State.AFK;
            }
            //这里会有一个afk的timer
            switch(AttackState)
            {
                case State.Idle:
                    DoIdle();
                    break;
                case State.Attack:
                    DoAttack(target);
                    break;
                case State.AFK:
                    DoAFK();
                    break;
            }
        }
        public void UpdateProjGeneralState()
        {
            Projectile.timeLeft = 2;
            PosList.Add(Projectile.Center);
            if (PosList.Count > 4)
                PosList.RemoveAt(0);
            Projectile.spriteDirection = Math.Sign((Projectile.Center.X - Owner.MountedCenter.X));
            HJScarletMethods.AddFrames(Projectile, 3, 6);
            Helper.UpdateAniState(0);

        }

        public void DoAFK()
        {
            //这里的悬挂路径用的世界差值
            Osci = 0;
            float mountedX = Owner.MountedCenter.X;
            float mountedY = Owner.MountedCenter.Y  - 1500f - 30f * Reverse.ToInt();
            Vector2 mountedPos = new Vector2(mountedX, mountedY);
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.02f);
            Projectile.rotation = Projectile.rotation.AngleLerp((-Vector2.UnitX).ToRotation(), 0.2f);
            //击杀射弹。
            if (ShouldKillRods)
            {
                Helper.UpdateAniState(2);
                if (Helper.IsDone[2])
                    Projectile.Kill();
            }
        }

        public void DoAttack(NPC target)
        {
            Osci += ToRadians(1f);
            AFKTimer = 0;
            //这里的悬挂路径用的世界差值
            Vector2 dir = (Owner.MountedCenter - target.Center).ToSafeNormalize().RotatedBy(ToRadians((30f + (float)Math.Sin(Osci)) * Reverse.ToDirectionInt()));
            Vector2 mountedPos = dir * 150f + target.Center;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.2f);
            Projectile.rotation = Projectile.rotation.AngleLerp((Owner.direction *(Projectile.Center - target.Center)).ToRotation(), 0.2f);
            Helper.UpdateAniState(1);
            
            if (Helper.IsDone[1])
            {
                Timer++;
                if (Timer > 10)
                {
                    Vector2 vel = -Owner.direction *(Projectile.rotation.ToRotationVector2()).ToRandVelocity(0f, 12f, 16f);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileType<ClimaticHawstringBeam>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj.rotation = vel.ToRotation();
                    proj.stopsDealingDamageAfterPenetrateHits = true;
                    proj.penetrate = 1;
                    Timer = 0;
                    SoundEngine.PlaySound(SoundID.Item158 with { MaxInstances = 0 }, Projectile.Center);
                }
            }
        }

        public void DoIdle()
        {
            Helper.IsDone[1] = false;
            Helper.Progress[1] = 0;
            Timer = 0;
            Osci += ToRadians(1f);
            //这里的悬挂路径用的世界差值
            float mountedX = Owner.MountedCenter.X;
            float mountedY = Owner.MountedCenter.Y - (20f * (float)Math.Sin(Osci)) - 60f - 30f * Reverse.ToInt();
            Vector2 mountedPos = new Vector2(mountedX, mountedY);
            float flyValue = Helper.IsDone[0] ? 0.2f : 0.12f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, flyValue);
            Projectile.rotation = Projectile.rotation.AngleLerp((-Vector2.UnitX).ToRotation(), 0.2f);
        }

        public bool GetTargetNearMouse(out NPC target)
        {
            target = null;
            float searchDist = 500f * 500f;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (!npc.IsLegal())
                    continue;
                float curDistance = Vector2.DistanceSquared(npc.Center , Main.MouseWorld);
                if (curDistance < searchDist)
                {
                    target = npc;
                    searchDist = curDistance;
                }
            }
            return target != null;
        }
        public bool PlayerIsAttacking
        {
            get
            {
                return Owner.channel && Owner.HeldItem.type == ItemType<ClimaticHawstring>();
            }
        }
        public void AddFrame()
        {
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;

            }
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>(GetInstance<PriestessRod_Minion>().Texture).Value;
            Rectangle frames = tex.Frame(1, 6, 0, Projectile.frame);
            Vector2 origin = frames.Size() / 2;
            SpriteEffects spriteEffect = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, Projectile.Center - Main.screenPosition, frames, Color.White, Projectile.rotation, origin, 1f, spriteEffect, 0);
            return true;
        }
    }
}
