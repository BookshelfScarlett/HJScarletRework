using HJScarletRework.Buffs.Pets;
using HJScarletRework.Projs.Pets;
using HJScarletRework.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Pets
{
    public class WhaleItem : HJScarletPetItem
    {
        public override void BuffAndProj()
        {
            Item.DefaultToVanitypet(ProjectileType<WhaleProj>(), BuffType<WhaleBuff>());
        }
            
        public override void ExSD()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
        }
    }
    public abstract class HJScarletPetItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pet";
        public override string Texture => $"HJScarletRework/Assets/Texture/Pets/Pet_{GetType().Name}";
        //封住这个sd避免误重写
        public sealed override void SetDefaults()
        {
            ExSD();
            BuffAndProj();
            Item.rare = RarityType<RarePets>();
            Item.value = Item.sellPrice(gold: 50);
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.AddBuff(Item.buffType, 3600);
            return true;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (!CustomTooltipDraw(line, ref yOffset))
            {
                if (line.Name == "ItemName" && line.Mod == "Terraria")
                {
                    RarePets.DrawCustomTooltipLine(line);
                    return false;
                }
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        /// <summary>
        /// 返回真以做掉通用的tooltip方法
        /// </summary>
        /// <param name="line"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public virtual bool CustomTooltipDraw(DrawableTooltipLine line, ref int yOffset) => false;
        public virtual void ExSD() { }
        /// <summary>
        /// 复写这个属性，为宠物物品提供相对应的buff和proj名
        /// </summary>
        public virtual void BuffAndProj() { }
    }
}
