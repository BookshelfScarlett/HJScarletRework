using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Items.Accessories
{
    public class SakurabaEmmaSound : ModSystem
    {
        public static SoundStyle HitSound;
        public static SoundStyle HitSound_Kiang;
        public static SoundStyle HitSound_Heavy;
        public override void Load()
        {
            HitSound = InstallEmmaSound("Emma_Hit",4);
            HitSound_Heavy = InstallEmmaSound("Emma_HitHeavy", 3);
            HitSound_Kiang = new SoundStyle(Path + "Emma_Kiang") with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
        }
        private string Path = HJScarletSounds.SoundsPath + "SakurabaEmmaSounds/";
        public SoundStyle InstallEmmaSound(string name, int vari) => new SoundStyle(Path + name, numVariants: vari) with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest };
    }
    public class SakurabaEmmaPlayer : ModPlayer
    {
        public bool vanityEquipped = false;
        public bool JustKiang = false;
        public float Timer = 0;
        public override void LoadData(TagCompound tag)
        {
            vanityEquipped = tag.GetBool(nameof(vanityEquipped));
            JustKiang = tag.GetBool(nameof(JustKiang));
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(vanityEquipped), vanityEquipped);
            tag.Add(nameof(JustKiang), JustKiang);
        }
        public override void ResetEffects()
        {
            vanityEquipped = false;
        }
        //Timer不要直接设定为0
        public override void UpdateDead() => Timer = 1;
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if(!ShouldDisable)
            {
                hurtInfo.SoundDisabled = true;
                Disable();
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!ShouldDisable)
            {
                hurtInfo.SoundDisabled = true;
                Disable();
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers) => ModifySound(ref modifiers);
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) => ModifySound(ref modifiers);
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) => ModifySound(ref modifiers);

        private void ModifySound(ref Player.HurtModifiers modifiers)
        {
            if (!ShouldDisable)
            {
                modifiers.DisableSound();
                Disable();
            }

        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (vanityEquipped)
            {
                genDust = false;
                if (!ShouldDisable)
                {
                    SoundStyle sound = JustKiang ? SakurabaEmmaSound.HitSound_Kiang : SakurabaEmmaSound.HitSound_Heavy;
                    SoundEngine.PlaySound(sound, Player.Center);
                }
                //落樱和散发粒子，修改为这些。
                new CrossGlow(Player.Center, Color.Pink, 30, 1f, 0.13f, false).Spawn();
                for (int i = 0; i < 15; i++)
                {
                    Vector2 spawnPos = Player.Center.ToRandCirclePos(36f);
                    new Petal(spawnPos, Vector2.UnitY * Main.rand.NextFloat(1.1f, 1.3f), RandLerpColor(Color.HotPink, Color.LightPink), 120, RandRotTwoPi, 0.8f, Main.rand.NextFloat(0.08f, 0.1f), 0.3f).Spawn();
                    new TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(4), 0.2f, RandLerpColor(Color.HotPink, Color.LightPink), 120, 0.22f, RandRotTwoPi).Spawn();

                }
            }
            //当然仍然可以杀死玩家。
            return true;
        }
        public override void PostUpdate()
        {
            if (Main.mouseMiddle && Main.HoverItem.type == ItemType<SakurabaEmma>() && Main.playerInventory)
            {
                if (!Main.mouseMiddleRelease)
                    return;
                SoundStyle playSound = !JustKiang ? SakurabaEmmaSound.HitSound_Kiang : SakurabaEmmaSound.HitSound;
                SoundEngine.PlaySound(playSound, Player.Center);
                JustKiang = !JustKiang;
            }
        }
        private bool ShouldDisable
        {
            get
            {
                //出于某些原因如果真的有神人玩家选择加载了樱羽艾玛死亡音效mod，则禁用这个物品的所有自定义音效
                //还有这里的中文变量名纯故意的

                bool 草艾玛 = ModLoader.HasMod("Sounds_SakurabaEma");
                if (草艾玛)
                    return true;
                if (!vanityEquipped)
                    return true;
                return false;
            }

        }
        private bool Disable()
        {
            //如果玩家出于某种原因想听艾玛狗叫的话。
            //kiang
            if (JustKiang)
            {
                SoundEngine.PlaySound(SakurabaEmmaSound.HitSound_Kiang, Player.Center);
                return true;
            }
            SoundStyle hitSound = Main.rand.NextBool() ? SakurabaEmmaSound.HitSound : SakurabaEmmaSound.HitSound_Kiang;
            SoundEngine.PlaySound(hitSound, Player.Center);
            return true;
        }
        public override void UpdateVisibleVanityAccessories()
        {
            //只有暂停的时候下面才会调用这个方法
            bool isPaused = Main.gamePaused || Main.autoPause;
            //重写这段是为了开启自动暂停的时候，也能绘制需要的时装
            if(vanityEquipped && isPaused)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Head);
            }
        }
        public override void FrameEffects()
        {
            //这里只能通过遍历所有玩家原版盔甲栏的方式来寻找需要的时装物品。
            //如果玩家佩戴了其他的饰品栏，那么……嗯，随便吧反正
            bool equip = false;
            if (Main.gameMenu)
            {
                foreach (Item item in Player.armor)
                {
                    if (item.type == ItemType<SakurabaEmma>())
                    {
                        equip = true;
                        break;
                    }
                }
            }
            if (vanityEquipped || equip)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, nameof(SakurabaEmma), EquipType.Head);
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (Timer > 0f)
                Timer--;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Vector2 spawnPos = Player.direction > 0 ? new Vector2(drawInfo.Position.X + 2, drawInfo.Position.Y + 2) : new Vector2(drawInfo.Position.X + 15, drawInfo.Position.Y + 2);
            if (Timer <= 0f)
                DrawGlow(spawnPos);
        }
        public void DrawGlow(Vector2 spawnPos)
        {
            new CrossGlow(spawnPos, Color.Pink, 30, 1, 0.10f).Spawn();
            for (int i = 0; i < 3; i++)
            {
                new Petal(spawnPos, Vector2.UnitY * Main.rand.NextFloat(1.1f, 1.3f), RandLerpColor(Color.HotPink, Color.LightPink), 120, RandRotTwoPi, 0.8f, Main.rand.NextFloat(0.08f, 0.1f), 0.3f).Spawn();
                new TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(3), 0.2f, RandLerpColor(Color.HotPink, Color.LightPink), 120, 0.22f, RandRotTwoPi).Spawn();
            }
            Timer = 120f;

        }
    }
    public class SakurabaEmma : HJScarletItems
    {
        //没有理由给这个东西敲词缀，说实话
        public override bool AllowPrefix(int pre) => false;
        internal static string ItemPath = "HJScarletRework/Assets/Texture/Player/";
        public override string Texture => ItemPath + "Acc_SakurabaEmma";
        public override ItemCategory LocalCategory => ItemCategory.Accessories;
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, ItemPath + "EmmaHead", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, ItemPath + "EmmaBody", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, ItemPath + "EmmaLegs", EquipType.Legs, this);
            }
        }
        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
        public override bool ConsumeItem(Player player) => false;
        public int Timer = 0;
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = RarityType<SakuraRarity>();
            Item.vanity = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.ReplaceAllTooltip(Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Tooltip"), Color.LightPink);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                SakuraRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.Pink with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.UltraBright);
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = Request<Texture2D>(Texture).Value;
            //描边。
            for (int i = 0; i < 8; i++)
            {
                spriteBatch.Draw(tex, position + ToRadians(60f * i).ToRotationVector2() * 2.1f, frame, Color.LightPink.ToAddColor(), 0f, origin, scale, 0, 0);
            }
            //本身
            spriteBatch.Draw(tex, position, frame, Color.White, 0, origin, scale, 0, 0);
            return false;
        }
        public override void UpdateVanity(Player player)
        {
            //整理一下这里的代码
            //如果开启自动暂停的情况下，这里需要手动绘制一遍
            player.GetModPlayer<SakurabaEmmaPlayer>().vanityEquipped = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SakurabaEmmaPlayer>().vanityEquipped = !hideVisual;
        }
    }
}
