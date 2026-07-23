using ContinentOfJourney.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class CycleMadnessStar : HJScarletProj
    {
        public enum State
        {
            Floating,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Osci => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new(3);
        public int CurLifeTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.SetUpHeldProj(2);
            Projectile.Opacity = 0;
        }
        public override void OnFirstFrame()
        {
            CurLifeTime = Projectile.timeLeft;
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.2f);
            switch(AttackState)
            {
                case State.Floating:
                    DoFloating();
                    break;
                case State.Homing:
                    DoHoming();
                    break;
            }
        }

        public void DoHoming()
        {
            Projectile.timeLeft = CurLifeTime;
            Projectile.rotation = Projectile.rotation.AngleTowards((Projectile.Center.GetNormalVector2(Owner.Center)).ToRotation(),.2f);
            float maxtimer = 30;
            float progress = Clamp(Timer / maxtimer,0,1);
            Timer++;


        }

        public void DoFloating()
        {
            float distance = (Projectile.Center - Owner.Center).LengthSquared();
            float searchDist = 150;

            Projectile.scale = Lerp(Projectile.scale, 1.01f, 0.2f);
            Projectile.velocity *= .86f;
            Osci += ToRadians(2.5f);
            Vector2 floatingProgress = Projectile.Center + Vector2.UnitY * (int)(Math.Sin(Osci) * 5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, floatingProgress, 0.08f);
            CurLifeTime = Projectile.timeLeft;
            if(distance < (searchDist * searchDist) && Projectile.scale > 1.0f)
            {
                AttackState = State.Homing;
            }

        }

        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
