using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.NPCs
{
    public partial class HJScarletGlobalNPCs : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Dialectics_Mark = false;
        public int Dialectics_Timer = 0;
        public int Dialectics_HitTime = 0;
        public bool terraFlamethrowerDebuff = false;
        public List<int> StabList = [];
        public int isBeingStabByLavaFlowExecution = 0;
        public bool isBeingStabByContainedBlast = false;
        public int isBeingStabByContainedStick = 0;
        public int isPotraitTimer = 0;
        public int isBeingShadowCast = 0;
        public bool refluxChain = false;
        public float potraityDoT = 0;
        public float miscCounter = 0;
        public int StopNpcTime = 0;
        public Vector2 PostSpeed = Vector2.Zero;
        public bool absoluteZeroBuffEnemy = false;
        public bool theBleachingBuffEnemy = false;
        public int isUnderPreciousTargetCross = 2;


        public override void ResetEffects(NPC npc)
        {
            isBeingStabByContainedBlast = false;
            terraFlamethrowerDebuff = false;
            absoluteZeroBuffEnemy = false;
            theBleachingBuffEnemy = false;
            refluxChain = false;
            if (isBeingShadowCast > 0)
                isBeingShadowCast--;
            if (isUnderPreciousTargetCross > 0)
                isUnderPreciousTargetCross--;
            if (isBeingStabByContainedStick > 0)
                isBeingStabByContainedStick--;
            if (isBeingStabByLavaFlowExecution > 0)
                isBeingStabByLavaFlowExecution = 0;
            if (StopNpcTime > 0)
                StopNpcTime--;
            if (isPotraitTimer > 0)
            {
                isPotraitTimer = 0;
            }
            if (isPotraitTimer == 0)
            {
                potraityDoT = 0;
            }
            miscCounter++;
            if (miscCounter > 300)
                miscCounter = 0;

        }
        public override void PostAI(NPC npc)
        {
            if (absoluteZeroBuffEnemy)
            {
                npc.velocity *= AbsoluteZeroBuff.BadMoveSpeedEnemy;
            }
            if (StopNpcTime > 0)
                npc.velocity *= 0.1f;
            if (StopNpcTime == 0 && PostSpeed != Vector2.Zero)
            {
                npc.velocity = PostSpeed;
                PostSpeed = Vector2.Zero;
            }
            if (terraFlamethrowerDebuff)
            {
                if (miscCounter % 10 == 0 && npc.damage != 0)
                {
                }
            }

            if (Dialectics_HitTime > 5)
                Dialectics_HitTime = 5;

            if (Dialectics_Timer > 0)
                Dialectics_Timer--;

            if (Dialectics_Timer <= 0)
            {
                Dialectics_Mark = false;
                Dialectics_HitTime = 0;
            }
            if (isBeingShadowCast > 0)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC spread = Main.npc[i];

                    if (i != npc.whoAmI && spread != null && spread.active && !spread.townNPC && !spread.friendly && spread.lifeMax > 5 && Vector2.Distance(npc.Center, spread.Center) < 500 && spread.HJScarlet().isBeingShadowCast < 1)
                    {
                        spread.HJScarlet().isBeingShadowCast = isBeingShadowCast;
                    }
                }
            }
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (absoluteZeroBuffEnemy)
            {
                ApplyDoT(ref npc, AbsoluteZeroBuff.BadLifeRegenEnemy);
                damage = AbsoluteZeroBuff.BadLifeRegenEnemy;
            }
            if (theBleachingBuffEnemy)
            {
                ApplyDoT(ref npc, TheBleachingBuff.BadLifeRegenEnemy);
                damage = TheBleachingBuff.BadLifeRegenEnemy;
            }
            if (isBeingShadowCast > 0)
            {
                ApplyDoT(ref npc, 100);
                damage = 20;
                Vector2 pos = npc.ToRandRec();
                ECSParticle.ShrinkParticle(pos, npc.velocity / 4f, Color.Black, 45, 1, 0, 0.4f, 1, blendstate: BlendState.NonPremultiplied);
            }
            if (npc.lifeRegen < 0)
            {
                if (isPotraitTimer > 0)
                {
                    npc.lifeRegen = (int)(npc.lifeRegen * potraityDoT);
                    damage = (int)(damage * potraityDoT);
                }
            }
        }
        public void ApplyDoT(ref NPC npc, int DoTCount)
        {
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= DoTCount * 2;
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {

            if (absoluteZeroBuffEnemy)
            {
                drawColor = Color.Lerp(drawColor, Color.SkyBlue, .5f) with { A = 255 };
                Vector2 randRec = npc.ToRandRec();
            }
            if (theBleachingBuffEnemy)
            {
            }
            if (isBeingShadowCast > 0)
            {
                drawColor = Color.Lerp(drawColor, Color.Black, .40f);
            }
            base.DrawEffects(npc, ref drawColor);
        }
        public bool IsOnShader = false;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (isBeingShadowCast > 0)
            {

            }
            //if (theBleachingBuffEnemy)
            //{
            //    spriteBatch.EnterShaderArea(BlendState.AlphaBlend);
            //    Effect shader = HJScarletShader.StandardFlowShader;
            //    shader.Parameters["LaserTextureSize"].SetValue(npc.frame.Size());
            //    shader.Parameters["targetSize"].SetValue(new Vector2(npc.frame.Width, npc.frame.Height));
            //    shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 0f);
            //    shader.Parameters["uColor"].SetValue(Color.White.ToVector4() * .5f);
            //    shader.Parameters["uFadeoutLength"].SetValue(0f);
            //    shader.Parameters["uFadeinLength"].SetValue(0f);
            //    shader.CurrentTechnique.Passes[0].Apply();
            //    //HJScarletMethods.ApplyAlphaCut(new Vector4(0f, 0f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 0f, 0), new Vector2(1f, 1f), Color.Black);
            //    IsOnShader = true;
            //}
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsOnShader)
            {
                spriteBatch.EndShaderArea();
                IsOnShader = false;
            }
            if (terraFlamethrowerDebuff)
            {
                spriteBatch.EnterShaderArea();
                Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
                spriteBatch.Draw(ring, npc.Center - screenPos, null, Color.LimeGreen.ToAddColor(), 0, ring.ToOrigin(), 0.5f, 0, 0);
                spriteBatch.EndShaderArea();
            }
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}
