using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class JellyfishGroupBuff : ModBuff
    {
        public static int JellyfishGroupBadLifeRegenEnemy = 12;
        public float BlueJellyfishDamage = .10f;
        public int BlueJellyfishCrit = 10;
        public int BlueJellyfishAP = 5;
        public int PinkJellyfishDefense = 4;
        public float PinkJellyfishDR = .04f;
        public int PinkJellyfishLifeRegenSpeed = 2;
        public override LocalizedText Description => base.Description.WithFormatArgs(
            BlueJellyfishDamage.ToPercent(),
            BlueJellyfishCrit + "%",
            "+" + BlueJellyfishAP,
            "+" + PinkJellyfishDefense,
            PinkJellyfishDR.ToPercent(),
            PinkJellyfishLifeRegenSpeed.ToLifeRegenFormat());
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            switch (player.HJScarlet().jellyfishGroupIndex)
            {
                case NPCID.BlueJellyfish:
                    player.GetDamage<GenericDamageClass>() += BlueJellyfishDamage;
                    player.GetCritChance<GenericDamageClass>() += BlueJellyfishCrit;
                    player.GetArmorPenetration<GenericDamageClass>() += BlueJellyfishAP;
                    break;
                case NPCID.GreenJellyfish:
                    player.HJScarlet().infiniteFlightTime = true;
                    player.runAcceleration *= 1.1f;
                    break;
                case NPCID.PinkJellyfish:
                    player.statDefense += PinkJellyfishDefense;
                    player.endurance += PinkJellyfishDR;
                    player.lifeRegen += PinkJellyfishLifeRegenSpeed;
                    break;
            }
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player localPlayer = Main.LocalPlayer;
            HJScarletPlayer modPlayer = localPlayer.HJScarlet();
            int type = modPlayer.jellyfishGroupIndex;
            string value;
            switch (type)
            {
                case NPCID.BlueJellyfish:
                    value = this.GetLocalizedValue("Blue");
                    buffName = buffName + "-" + value;
                    rare = ItemRarityID.Cyan;
                    break;
                case NPCID.GreenJellyfish:
                    value = this.GetLocalizedValue("Green");
                    buffName = buffName + "-" + value;
                    rare = ItemRarityID.Lime;
                    break;
                case NPCID.PinkJellyfish:
                    value = this.GetLocalizedValue("Pink");
                    buffName = buffName + "-" + value;
                    rare = ItemRarityID.Pink;
                    break;
            }
            base.ModifyBuffText(ref buffName, ref tip, ref rare);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.HJScarlet().theJellyfishGroupBuffEnemy = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            return true;
        }

    }
}
