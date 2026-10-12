using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(givePaper), givePaper);
            tag.Add(nameof(firstTimeCraftGaia), firstTimeCraftGaia);
            tag.Add(nameof(ruShiWoWenBanMinionNameTrashList), ruShiWoWenBanMinionNameTrashList);
            tag.Add(nameof(ruShiWoWenBanMinionNameList), ruShiWoWenBanMinionNameList);
            tag.Add(nameof(weaponUpgradePostSon), weaponUpgradePostSon);
            tag.Add(nameof(crystallizeLoreReforgeIndex), crystallizeLoreReforgeIndex);
            tag.Add(nameof(crimsonScytheSlayNPCType), crimsonScytheSlayNPCType);
            tag.Add(nameof(brimstoneHeartKilling), brimstoneHeartKilling);
            ScarletSave(ref tag, giveMagicStorage);
            tag.Add("Scarlet:" + nameof(terrariaRecipeEatenList), terrariaRecipeEatenList);
            tag.Add("Scarlet:"+nameof(terrariaRecipeEatenListTrash), terrariaRecipeEatenListTrash);
            tag.Add("Scarlet:"+nameof(terrariaRecipeNotEatenList), terrariaRecipeNotEatenList);
            tag.Add("Scarlet:"+ nameof(terrariaRecipeNotEatenListTrash), terrariaRecipeNotEatenListTrash);
            tag.Add("Scarlet:" + nameof(terrariaRecipeEatenFoodCounts), terrariaRecipeEatenFoodCounts);
        }
        public override void LoadData(TagCompound tag)
        {
            ruShiWoWenBanMinionNameList = (List<string>)tag.GetList<string>(nameof(ruShiWoWenBanMinionNameList));
            ruShiWoWenBanMinionNameTrashList = (List<string>)tag.GetList<string>(nameof(ruShiWoWenBanMinionNameTrashList));

            brimstoneHeartKilling = tag.GetBool(nameof(brimstoneHeartKilling));
            terraRecipe = tag.GetBool(nameof(terraRecipe));
            givePaper = tag.GetBool(nameof(givePaper));
            firstTimeCraftGaia = tag.GetBool(nameof(firstTimeCraftGaia));
            weaponUpgradePostSon = tag.GetBool(nameof(weaponUpgradePostSon));
            crystallizeLoreReforgeIndex = tag.GetInt(nameof(crystallizeLoreReforgeIndex));
            crimsonScytheSlayNPCType = tag.GetInt(nameof(crimsonScytheSlayNPCType));

            terrariaRecipeNotEatenListTrash= (List<string>)tag.GetList<string>("Scarlet:"+nameof(terrariaRecipeNotEatenListTrash));
            terrariaRecipeNotEatenList= (List<string>)tag.GetList<string>("Scarlet:"+nameof(terrariaRecipeNotEatenList));
            terrariaRecipeEatenList= (List<string>)tag.GetList<string>("Scarlet:"+nameof(terrariaRecipeEatenList));
            terrariaRecipeEatenListTrash= (List<string>)tag.GetList<string>("Scarlet:"+nameof(terrariaRecipeEatenListTrash));
            terrariaRecipeEatenFoodCounts = tag.GetInt("Scarlet:" + nameof(terrariaRecipeEatenFoodCounts));
            ScarletLoadBool(ref tag, ref giveMagicStorage);
        }
        public void ScarletSave(ref TagCompound tag, object value)
        {
            tag.Add("Scarlet:" + nameof(value), value);
        }
        public void ScarletLoadInt(ref TagCompound tag, ref int value)
        {
            value = tag.GetInt("Scarlet:" + nameof(value));

        }
        public void ScarletLoadString(ref TagCompound tag, ref List<string> value)
        {
            value = (List<string>)tag.GetList<string>("Scarlet:" + nameof(value));
        }
        public void ScarletLoadBool(ref TagCompound tag, ref bool value)
        {
            value = tag.GetBool("Scarlet:" + nameof(value));
        }
    }
}
