using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Items.Armor.ExecutorAlter
{
    public abstract class AlterVanillaArmor : GlobalItem
    {
        public override void SaveData(Item item, TagCompound tag)
        {
            if (item.type != ApplyArmor)
                return;
            tag.Add("HJScarlet:AlterVersion", item.HJScarlet().EnableExecutorVersion);
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            if (item.type != ApplyArmor)
                return;
            item.HJScarlet().EnableExecutorVersion = tag.GetBool("HJScarlet:AlterVersion");
        }
        public virtual string SetupName { get; }
        public enum ArmorType
        {
            Helmet,
            Chestplate,
            Legs
        }
        public virtual ArmorType Category => ArmorType.Helmet;
        public virtual int ApplyArmor => -1;
        public sealed override bool InstancePerEntity => true;
        /// <summary>
        /// Shorthand
        /// </summary>
        public string ArmorSetName => $"HJScarletRework:{SetupName}Set";
        public string ArmorLocalization => Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.");
        public virtual bool SetUpArmorSet => false;
        public virtual int[] ArmorSlots => null;
        public virtual int DownedConditionID => -1;
        public sealed override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ApplyArmor;
        }
        public override void PostUpdate(Item item)
        {
        }
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (!SetUpArmorSet)
            {
                return base.IsArmorSet(head, body, legs);
            }
            else
            {
                if (ArmorSlots.Length != 0)
                {
                    if (ArmorSlots.Length < 3)
                    {
                        bool correctedArmor = false;
                        correctedArmor = head.type == ArmorSlots[0] && body.type == ArmorSlots[1];
                        if (correctedArmor)
                        {
                            bool corretedState = head.HJScarlet().EnableExecutorVersion && body.HJScarlet().EnableExecutorVersion;
                            if (corretedState)
                                return ArmorSetName;
                        }
                    }
                    else
                    {
                        bool correctedArmor = false;
                        correctedArmor = head.type == ArmorSlots[0] && body.type == ArmorSlots[1] && legs.type == ArmorSlots[2];
                        if (correctedArmor)
                        {
                            bool corretedState = head.HJScarlet().EnableExecutorVersion && body.HJScarlet().EnableExecutorVersion && legs.HJScarlet().EnableExecutorVersion;
                            if (corretedState)
                                return ArmorSetName;
                        }

                    }
                }
                return base.IsArmorSet(head, body, legs);
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type != ApplyArmor)
                return;
            string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
            tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lerp(Color.LawnGreen, Color.LightGreen, 0.5f));
            if (item.HJScarlet().EnableExecutorVersion)
            {
                int flavorTooltipIndex = tooltips.FindIndex(line => line.Name == "Defense" && line.Mod == "Terraria");
                string armorCategory = Mod.GetLocalizationKey($"Items.Armor.{SetupName}Executor.");
                string path = $"{armorCategory}{Category}Tooltip".ToLangValue();
                TooltipLine tooltipLine = new TooltipLine(Mod, "HJscarletReworkName", path);
                tooltips.Insert(flavorTooltipIndex + 1, tooltipLine);
                ExModifyTooltipsAlter(item, tooltips, armorCategory);
            }
            if (DownedConditionID != -1)
            {
                tooltips.AddSwapTooltipValueBossCondition(DownedConditionID);
            }
        }
        public virtual void ExModifyTooltipsAlter(Item item, List<TooltipLine> tooltips, string path)
        {
        }
        public override void UpdateEquip(Item item, Player player)
        {
            if (item.type != ApplyArmor)
                return;
            if (!item.HJScarlet().EnableExecutorVersion)
                return;
            ExUpdateEquipAlter(item, player);
        }
        public virtual void ExUpdateEquipAlter(Item item, Player player) { }
        /// <summary>
        /// 一个快捷的检查是否为整套盔甲的方法
        /// 会自动过是否为代行者版本的判定
        /// </summary>
        /// <returns></returns>
        public bool LegalArmorSet(Item head, Item body, Item legs, int helmet, int chestplate, int legging)
        {
            bool correctedArmor = head.type == helmet && body.type == chestplate && legs.type == legging;
            bool correctedState = head.HJScarlet().EnableExecutorVersion && body.HJScarlet().EnableExecutorVersion && legs.HJScarlet().EnableExecutorVersion;
            return correctedArmor && correctedState;
        }

    }
}
