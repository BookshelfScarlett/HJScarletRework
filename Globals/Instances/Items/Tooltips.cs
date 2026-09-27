using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Database.Localization;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using HJScarletRework.Rarity.RarityDrawHandler;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public IReadOnlyList<TooltipLine> CacheTooltipLine;
        public bool drawBuffIconAndDetail = false;
        public bool drawBuffIcon = false;
        public string OwnerName = string.Empty;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ItemBelongTo != EnumItemOwner.None)
            {
                string keyPath = Mod.GetLocalizationKey($"ItemBelongTo.{ItemBelongTo}");
                Color color = Color.White;
                switch (ItemBelongTo)
                {
                    case EnumItemOwner.Developer:
                        color = Color.Red;
                        break;
                    case EnumItemOwner.Supporter:
                        color = Color.Lime;
                        break;
                    case EnumItemOwner.Donator:
                        color = Color.Pink;
                        break;
                }
                string value = "<" + keyPath.ToLangValue() + "·" + item.HJScarlet().OwnerName + ">";
                tooltips.CreateTooltipDirect(value, color, LineName: item.HJScarlet().ItemBelongTo + "Name");
            }
            if (HJScarletPlayer.AllWeaponSwapValue.Contains(item.type))
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.CreateTooltipDirect(keyPath.ToLangValue(), Color.Lerp(Color.LawnGreen, Color.LightGreen, 0.5f));
            }
            if (LocalPlayer.HJScarlet().terraRecipe)
            {
                if (HJScarletList.LegalFoodList.Contains(item.type))
                {
                    //表单里有这个内容我们才写这个东西。没有则写另一条
                    string itemName = string.Empty;
                    if (item.type > VanillaMaxItem)
                    {
                        itemName = item.ModItem.FullName;
                    }
                    else
                    {
                        itemName = ItemID.Search.GetName(item.type);
                    }
                    string path = Mod.GetLocalizationKey($"Items.Useable.TerrariaRecipe.");
                    List<string> list = LocalPlayer.HJScarlet().terraRecipeEatenFoodNameList;
                    if (list.Contains(itemName))
                        tooltips.CreateTooltipDirect((path + "Eaten").ToLangValue(), Color.GreenYellow);
                    else
                        tooltips.CreateTooltipDirect((path + "NotEaten").ToLangValue(), Color.SkyBlue);
                }
            }
            //因为各种原因导致的史山
            if (LocalPlayer.HJScarlet().monkExecutor)
            {
                if (item.type == ItemID.MonkStaffT1)
                {
                    string path = Mod.GetLocalizationKey($"Items.Armor.{nameof(MonkHead)}.SleepyOctBuff").ToLangValue();
                    string path2 = Mod.GetLocalizationKey($"Items.Armor.{nameof(ShinobiHead)}.WeaponBuff").ToLangValue();
                    tooltips.CreateTooltipDirect(path2, Color.Bisque, null, "ShinobiBuffTitle");
                    tooltips.CreateTooltipDirect(path, Color.GreenYellow, null, "ShinobiBuff", -1, "20%", "15%", "20%");
                }
                if (item.type == ItemID.MonkStaffT3)
                {
                    string path = Mod.GetLocalizationKey($"Items.Armor.{nameof(MonkHead)}.DragonFuryBuff").ToLangValue();
                    string path2 = Mod.GetLocalizationKey($"Items.Armor.{nameof(ShinobiHead)}.WeaponBuff").ToLangValue();
                    tooltips.CreateTooltipDirect(path2, Color.Bisque, null, "ShinobiBuffTitle");
                    tooltips.CreateTooltipDirect(path, Color.Thistle, null, "ShinobiBuff", -1, "35%", "15%", "200%");
                }
            }
            //强制自动处决/手动处决的字段
            if (ScarletItemIDSets.ForceToAutomaticExecute[item.type])
            {
                int index = tooltips.FindLineIndex("ExecutorWeaponTypeName", Mod.Name);
                string path = Mod.GetLocalizationKey($"ExecutorDamageClass.ForceAutomaticExecution").ToLangValue();
                TooltipLine line = new TooltipLine(Mod, "ForceAutomaticExecution", path)
                {
                    OverrideColor = Color.Pink
                };
                tooltips.Insert(index + 1, line);
            }
            else if (ScarletItemIDSets.ForceToTacticalExecute[item.type])
            {
                int index = tooltips.FindLineIndex("ExecutorWeaponTypeName", Mod.Name);
                string path = Mod.GetLocalizationKey($"ExecutorDamageClass.ForceTacticalExecution").ToLangValue();
                TooltipLine line = new TooltipLine(Mod, "ForceTacticalExecutionLine", path)
                {
                    OverrideColor = Color.Pink
                };
                tooltips.Insert(index + 1, line);
            }
            //标记物品未完成
            if (item.HJScarlet().NotFinished)
            {
                tooltips.CreateTooltip(Mod.GetLocalizationKey("NotFinished"), Color.IndianRed);
            }
            //无主之地系列的武器，红字文本。
            if (item.HJScarlet().borderlandWeapon)
            {
                string path = Mod.GetLocalizationKey($"Weapons.Executor.{item.ModItem.Name}.FlavorTooltip");
                int index = tooltips.FindLineIndex("Tooltip0");
                tooltips.CreateTooltip(path, Color.Lerp(Color.DarkRed, Color.Crimson, 0.82f), LineName: "BorderlandRedLineName", index: index);
            }
            if (item.type == ItemID.PocketMirror)
            {
                int index = tooltips.FindLineIndexLast("Tooltip");
                string path = Mod.GetLocalizationKey($"Database.PocketMirrorModiflication");
                tooltips.CreateTooltip(path, LineName: "PocketMirrorModiflication", color: Color.SkyBlue, index: index + 2, args: Main.LocalPlayer.HJScarlet().pocketMirror.ToString());
            }
            InsertIconInTooltipLine(item, tooltips);
            CacheTooltipLine = tooltips;
        }
        #region 插入图片，但是在Tooltip内
        //匹配的正则表达式
        private static Regex MatchingBuffIcon = new Regex(@"\[(ScarletBuff|ScarletDebuff)\/([^\]]+)\]");
        //匹配中间具体Buff的来源和名称的正则表达式
        private static Regex MatchingSpecificBuff = new Regex(@"([^\/]+)\/([^\/]+)");
        //要绘制的Buff列表
        //话说我们为什么要写这么长一串？
        public List<(float, int, string, Texture2D, string, string, string)> buffs = new();
        public List<string> add = new();
        public void GlobalIconInsert(Item item, List<TooltipLine> tooltips)
        {
            //先画出buff转化的提示文本
            if (drawBuffIconAndDetail)
                tooltips.CreateTooltip(ScarletTextSets.GeneralText_BuffShow, ScarletTextSets.GeneralText_BuffShowColor);
            buffs.Clear();
            //遍历tooltip行，我们开始找匹配的正则表达式
            for (int i = 0; i < tooltips.Count; i++)
            {
                Texture2D texture = null;
                string name = string.Empty;
                string descrip = string.Empty;
                float length = 0;
                string color = string.Empty;
                while (MatchingBuffIcon.Match(tooltips[i].Text).Success)
                {
                    tooltips[i].Text = MatchingBuffIcon.Replace(tooltips[i].Text, match =>
                    {
                        return MatchingSpecificBuff.Replace(match.Groups[2].Value, keys =>
                        {
                            color = match.Groups[1].Value switch
                            {
                                "ScarletBuff" => "EE90EE",
                                "ScarletDebuff" => "AAEEFF",
                                _ => "FFFFFF"
                            };
                            //获取文本长度，方便定位buff的贴图绘制位置
                            length = ChatManager.GetStringSize(FontAssets.MouseText.Value, tooltips[i].Text.Substring(0, match.Index), Vector2.One).X;
                            //判断一遍原版的buff和mod的buff，两者的buffIcon获取区别很大
                            if (keys.Groups[1].Value == "Terraria")
                            {
                                if (BuffID.Search.TryGetId(keys.Groups[2].Value, out int buffID))
                                {
                                    texture = TextureAssets.Buff[buffID].Value;
                                    name = Lang.GetBuffName(buffID);
                                    descrip = Lang.GetBuffDescription(buffID);
                                    buffs.Add((length, i, color, texture, name, descrip, tooltips[i].Name));
                                }
                            }
                            else
                            {
                                if (ModLoader.TryGetMod(keys.Groups[1].Value, out Mod mod))
                                {
                                    if (mod.TryFind(keys.Groups[2].Value, out ModBuff modBuff))
                                    {
                                        //为啥我们要request啊？有没有别的方案？
                                        texture = Request<Texture2D>(modBuff.Texture).Value;
                                        name = modBuff.DisplayName.Value;
                                        descrip = modBuff.Description.Value;
                                        buffs.Add((length, i, color, texture, name, descrip, tooltips[i].Name));
                                    }
                                }
                            }
                            return $"       [c/{color}:{name}]";
                        });
                    }, 1);
                }
            }
            //用于写入具体的buffTooltip
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) && buffs.Count != 0 && drawBuffIconAndDetail)
            {
                if (tooltips.Count < 2)
                    return;
                tooltips.RemoveRange(1, tooltips.Count - 1);
                for (int j = 0; j < buffs.Count; j++)
                {

                    //终于差不多了……加tooltip
                    TooltipLine buffTextNameLine = new TooltipLine(Mod, "ScarletBuffIconName" + j, $"        [c/{buffs[j].Item3}:{buffs[j].Item5}]");
                    TooltipLine buffTextDescripLine = new TooltipLine(Mod, "ScarletBuffDescripName" + j, $"{buffs[j].Item6}");
                    tooltips.Add(buffTextNameLine);
                    tooltips.Add(buffTextDescripLine);
                    buffs[j] = (0, tooltips.Count, buffs[j].Item3, buffs[j].Item4, buffs[j].Item5, buffs[j].Item6, buffTextNameLine.Name);
                    if (add.Contains(buffs[j].Item5))
                    {
                        buffs.Remove(buffs[j]);
                        continue;
                    }
                    else
                        add.Add(buffs[j].Item5);
                }

            }
        }
        public void InsertIconInTooltipLine(Item item, List<TooltipLine> tooltips)
        {
            //是否绘制buffIcon要在物品的sd里面专门打个标记
            //主要是为了略过大部分并不需要画这个东西的鬼玩意，避免每次tooltip跑过来都得清一遍无用内存
            if (drawBuffIconAndDetail || drawBuffIcon)
                GlobalIconInsert(item, tooltips);
        }
        #endregion
        public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            if (!(drawBuffIcon || drawBuffIconAndDetail))
                return;
            for (int i = 0; i < lines.Count; i++)
            {
                foreach (var buf in buffs)
                {
                    if (buf.Item7 == lines[i].Name)
                    {
                        Vector2 pos = new Vector2(lines[i].X + buf.Item1 + 2.5f, lines[i].Y - 5f);
                        Main.spriteBatch.Draw(buf.Item4, pos, null, Color.White, 0, Vector2.Zero, .98f, 0, 0);
                    }
                    if (buf.Item7 != lines[i].Name)
                        continue;
                }
            }
            add.Clear();
        }
        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            if (LocalPlayer.HJScarlet().cycleMadnessCrit > 0 && LocalPlayer.HJScarlet().cycleMadnessLevel > 0)
            {
                if (line.Mod == "Terraria" && line.Name == "CritChance")
                {
                    RarityDrawHelper.DrawCustomTooltipLine(line, Color.White, Color.White, Color.Black, 1f);
                }
            }
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (!HJScarletConfigClient.Instance.SpecialRarity)
                return true;
            if (line.Name == (item.HJScarlet().ItemBelongTo + "Name") && line.Mod == Mod.Name)
            {
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Donator)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Donator);
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Developer)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Developer);
                if (item.HJScarlet().ItemBelongTo == EnumItemOwner.Supporter)
                    RareItemRarity.DrawFlavorTooltipName(line, RareItemRarity.RareType.Support);
                return false;
            }
            if (line.IsItemName())
            {
                if (HJScarletList.ShinyRarityItemDictionary.TryGetValue(item.type, out ShinyRarityType value))
                {
                    RarityDrawHelper.UpdateItemNameParticle(line, value);
                    RarityDrawHelper.UpdateItemNameDraw(line, value);
                    return false;
                }
            }
            if (line.Mod == Mod.Name && line.Name == "FlavorTooltipsName")
            {
                if (HJScarletList.ShinyRarityItemDictionary.TryGetValue(item.type, out ShinyRarityType value))
                {
                    RarityDrawHelper.UpdateFlavorNameDraw(line, value);
                    return false;
                }
            }
            return true;
        }
    }
}
