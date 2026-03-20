using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class JungleMadnessLeafs : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.Leaf);
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public enum LeafState
        {
            Normal,
            Execution
        }
        public NPC LastHitTarget = null;
        public LeafState AIStatement = LeafState.Normal;
        public float InitSpeed = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.timeLeft = GetSeconds(3);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.width = Projectile.height = 12;
        }
        public override void OnFirstFrame()
        {
            if (Projectile.GetTargetSafe(out NPC target, false))
                LastHitTarget = target;
            InitSpeed = Projectile.velocity.Length();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (target.Equals(LastHitTarget) && Timer < 30)
                return false;
            return base.CanHitNPC(target);
        }
        public override void ProjAI()
        {
            Timer++;
            UpdateParticles();
            switch (AIStatement)
            {
                case LeafState.Normal:
                    DoNormalState();
                    break;
                case LeafState.Execution:
                    DoExecutionState();
                    break;
            }
            UpdateFrameAnimation();
        }

        private void UpdateParticles()
        {
            if (Projectile.IsOutScreen())
                return;
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass);
                d.scale = Main.rand.NextFloat(0.8f, 1.15f);
                d.noGravity = true;

        }

        private void UpdateFrameAnimation()
        {
            Projectile.frameCounter += 1;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;

            }

        }

        public void DoNormalState()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public void DoExecutionState()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(7f - Timer * 0.02f));
            if (Projectile.velocity.LengthSquared() > InitSpeed * InitSpeed)
                Projectile.velocity *= 0.90f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frames = Projectile.GetTexture().Frame(1, 5, 0, Projectile.frame);
            Vector2 origin = frames.Size() / 2;
            Texture2D tex = Projectile.GetTexture();
            int length = Projectile.oldPos.Length - 3;

            for (int i = length - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 trailingDrawPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float faded = 1 - i / (float)length;
                faded *= faded;
                Color trailColor = lightColor * faded;
                Main.spriteBatch.Draw(tex, trailingDrawPos, frames, trailColor, Projectile.oldRot[i], origin, Projectile.scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, Projectile.Center - Main.screenPosition + ToRadians(60 * i).ToRotationVector2() * 2f, frames, Color.White.ToAddColor(), Projectile.rotation, origin, Projectile.scale, 0, 0);
            SB.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, frames, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
