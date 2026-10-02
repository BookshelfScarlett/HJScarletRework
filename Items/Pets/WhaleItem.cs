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
        public override int PetProjType => ProjectileType<WhaleProj>();
        public override int PetBuffType => BuffType<WhaleBuff>();
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
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.AddBuff(Item.buffType, 3600);
            return true;
        }
    }
}
