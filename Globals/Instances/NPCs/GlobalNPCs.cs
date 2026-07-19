using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public partial class HJScarletGlobalNPCs : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool DeepToneThrownMark = false;
        public bool Dialectics_Mark = false;
        public int Dialectics_Timer = 0;
        public int Dialectics_HitTime = 0;
        public int blackKeyDefensesReduces = 0;
        public bool terraFlamethrowerDebuff = false;
        public int terraFlamethrowerDropDamage = 0;
        public List<int> StabList = [];
        public bool isBeingStabByLavaFlow = false;
        public int isBeingStabByLavaFlowExecution = 0;
        public bool isBeingStabByContainedBlast = false;
        public int isBeingStabByContainedStick = 0;
        public float miscCounter = 0;
        public override void ResetEffects(NPC npc)
        {
            isBeingStabByLavaFlow = false;
            isBeingStabByContainedBlast = false;
            terraFlamethrowerDebuff = false;
            terraFlamethrowerDropDamage = 0;
            if (isBeingStabByContainedStick > 0)
                isBeingStabByContainedStick--;
            if (isBeingStabByContainedStick > 0)
                isBeingStabByLavaFlowExecution--;
        }
        public override void PostAI(NPC npc)
        {
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
            miscCounter++;
            if (miscCounter > 300)
                miscCounter = 0;
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            base.DrawEffects(npc, ref drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
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
