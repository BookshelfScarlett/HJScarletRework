using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class RuShiWoWen : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public static int Cooldown = 30;
        public static int MinMinionSelected = 10;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Donator);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Cooldown, HJScarletList.SummonWeaponList.Count - MinMinionSelected);
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().ItemBelongTo = Globals.Enums.EnumItemOwner.Donator;
        }
        public override void RightClick(Player player)
        {
            //ref List<int> list = ref player.HJScarlet().powerLilyBanMinionHashSet;
            //if (list.Count > 0)
            //{
            //    list.RemoveAt(list.Count - 1);
            //}
        }
        public override bool ConsumeItem(Player player) => false;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().powerLily = true;
        }
        public IReadOnlyList<TooltipLine> CacheTooltip;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            HJScarletPlayer modPlayer = Main.LocalPlayer.HJScarlet();
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
            {
                //获取表单与对应的名字。
                List<int> foodList = new(modPlayer.powerLilyBanMinionHashSet);
                //直接遍历一个表单
                //这里表单是严格11对应的，理论来说不会出现问题，大概
                string combineValue = null;
                string supply = "\n";
                int line = 1;
                if (foodList.Count != 0)
                    combineValue = $"{line}-";
                else
                {
                    combineValue = string.Empty;
                    supply = string.Empty;
                }
                for (int i = 0; i < foodList.Count; i++)
                {

                    string perInstance = $"[i:{foodList[i]}]";
                    //将其放进这个列表里合并起来
                    combineValue += $"{perInstance}";
                    //如果i每次都%8==0,新开一行
                    if ((i + 1) % 10 == 0)
                    {
                        //过一个判定看这个值是不是超过了列表数
                        //这样就不会新开一个什么都没有的行，因为写法上的问题
                        if (i + 1 < foodList.Count)
                        {
                            line++;
                            combineValue += $"\n{line}-";
                        }
                    }
                }
                //最后使用replace的插值字符串。
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey($"BanMinionList"), null, supply+ combineValue, foodList.Count);

            }
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().drawUseableItemIcon = Type;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.IsAir || item is null)
                    continue;
                if (HJScarletList.SummonWeaponList.Contains(item.type) && !player.HJScarlet().powerLilyBanMinionHashSet.Contains(item.type))
                {
                    item.HJScarlet().purePrismLegalTarget = true;
                }
            }

        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(line);
        }
    }
}
