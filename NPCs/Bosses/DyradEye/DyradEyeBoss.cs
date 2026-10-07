using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.NPCs.Bosses.DyradEye
{
    [AutoloadBossHead]
    public class DyradEyeBoss : ModNPC
    {
        public enum AttackStyle
        {
            JustSpawn,
            BeginnerMode,
            Spinning,
            Charge,
            SmoothCharge
        }
        public ref float Timer => ref NPC.ai[0];
        public AttackStyle State
        {
            get => (AttackStyle)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        public override string Texture => AssetHandler.DyradEye.Texture;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 16;
        }
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 112;
            NPC.damage = 25;
            NPC.defense = 16;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = Item.buyPrice(1);
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.npcSlots = 10;
            NPC.lavaImmune = true;
            NPC.friendly = false;
        }
        public override void AI()
        {
            if(!NPC.IsLegalTarget())
            {
                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];
            Timer++;
            if(player.dead)
            {
                NPC.active = false;
                return;

            }
            switch (State)
            {
                case AttackStyle.JustSpawn:
                    DoAI_JustSpawn(player);
                    break;
                case AttackStyle.BeginnerMode:
                    DoAI_BeginnerMode(player);
                    break;
            }

            base.AI();
        }
        /// <summary>
        /// 树妖眼具备一个新手教程
        /// <br>需要引入空归的核心格挡机制</br>
        /// </summary>
        /// <param name="player"></param>
        public void DoAI_BeginnerMode(Player player)
        {
        }

        /// <summary>
        /// 刚生成的逻辑
        /// <br>考虑到可能使用祭坛，这里最后会重置一遍AI</br>
        /// </summary>
        public void DoAI_JustSpawn(Player player)
        { 
        }
    }
}
