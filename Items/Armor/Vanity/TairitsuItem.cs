using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityParticles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Items.Armor.Vanity
{
    public abstract class MenuVanityPlayer : ModPlayer
    {
        public virtual int VanityItemType => -1;
        public bool Equipped = false;
        public override void ResetEffects()
        {
            Equipped = false;
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(Equipped), Equipped);
        }
        public override void LoadData(TagCompound tag)
        {
            Equipped = tag.GetBool(nameof(Equipped));
        }
        public override void FrameEffects()
        {
            bool equip = false;
            if (Main.gameMenu)
            {
                GetArmorSlotItem(ref equip);
            }
            if (equip && VanityItemType != -1)
            {
                string name = HJScarletList.VanityItemDictionary[VanityItemType];
                Player.legs = EquipLoader.GetEquipSlot(Mod, name, EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, name, EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, name, EquipType.Head);
                ExArmor();
            }
        }
        public virtual void ExArmor()
        {

        }
        public void GetArmorSlotItem(ref bool equip)
        {
            foreach (Item item in Player.armor)
            {
                if (item.type == VanityItemType)
                {
                    equip = true;
                    break;
                }
            }
        }
    }
    public abstract class AccVanityItem : HJScarletItemClass
    {
        public static string ItemPath => "HJScarletRework/Assets/Texture/Items/Vanity/";
        public virtual string VanityName => string.Empty;
        public string VanityPrefix => ItemPath + VanityName;
        public override string AssetPath => AssetHandler.Vanity;
        public override bool AllowPrefix(int pre) => false;
        public override bool ConsumeItem(Player player) => false;
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.Red;
            Item.vanity = true;
            Item.accessory = true;
        }
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Head", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Body", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Legs", EquipType.Legs, this);
            ExLoad();

        }

        public virtual void ExLoad()
        {
        }
        public override void SetStaticDefaults()
        {

            HJScarletList.VanityItemDictionary.Add(Type, GetType().Name);
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
        public virtual void ExModifyTooltip(List<TooltipLine> tooltips) { }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
                player.HJScarlet().accVanityID = Type;
        }
        public override void UpdateVanity(Player player)
        {
            player.HJScarlet().accVanityID = Type;
        }
    }
    public class TairitsuItem : AccVanityItem
    {
        public override string VanityName => "Tairitsu";
        public override void ExLoad()
        {
            EquipLoader.AddEquipTexture(Mod, $"{VanityPrefix}Hair", EquipType.Back, this);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                TairitsuEffect.DrawItemName(line);
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Umbrella).
                AddIngredient(ItemID.UlyssesButterfly).
                AddTile(TileID.Loom).
                Register();
        }
    }
    public class TairitsuPlayer : MenuVanityPlayer
    {
        public override int VanityItemType => ItemType<TairitsuItem>();
    }

    public class TairitsuEffect : ModRarity
    {
        public static List<RaritySparkle> RaritySparkles = [];
        public static void DrawItemName(DrawableTooltipLine line)
        {
            PostDrawRarity(ref RaritySparkles, line);
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.RoyalBlue, Color.Lerp(Color.White, Color.DeepSkyBlue, 0.65f), Color.Black, 1);
        }

        public static void PostDrawRarity(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, RandLerpColor(Color.DeepSkyBlue, Color.Black), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
            RarityDrawHelper.UpdateTooltipParticles(tooltipLine, ref particleList);
        }
        public static Vector2 GetParticlePosition(DrawableTooltipLine line)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            return Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
        }
    }

}
