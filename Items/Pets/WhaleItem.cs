using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Pets;
using HJScarletRework.Rarity.RarityShiny;
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
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.RarePets);
        }
        public virtual int PetProjType => 0;
        public virtual int PetBuffType => 0;
        public sealed override void SetDefaults()
        {
            BuffAndProj();
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 50);
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 20;
            Item.UseSound = SoundID.Item2;
            Item.rare = RarityType<RarePets>();
            Item.master = true;
            Item.buffType = PetBuffType;
            Item.shoot = PetProjType;
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.AddBuff(Item.buffType, 3600);
            return true;
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
