using ContinentOfJourney.Items;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.List
{
    public partial class HJScarletList : ModSystem
    {
        public static List<int> ThrownSpearList = [];
        public static List<int> HJSpearList=[];
        public static List<int> MaleNPC = [];
        public static List<int> FemaleNPC = [];
        public static List<int> LegalFoodList = [];
        public override void Load()
        {
            //投矛表单
            foreach (var spear in Mod.GetContent<ThrownSpearClass>())
            {
                ThrownSpearList.Add(spear.Type);
            }
            //而后，再添加旅人归途的一些投矛。如果有的话
            ThrownSpearList.Add(ItemType<DesertScourge>());
            ThrownSpearList.Add(ItemType<Longinus>());
            HJSpearList =
            [
                ItemType<SweetSweetStab>(),
                ItemType<Candlance>(),
                ItemType<WildPointer>(),
                ItemType<SpearOfDarkness>(),
                ItemType<LightBite>(),
                ItemType<DeepTone>(),
                ItemType<Tonbogiri>(),
                ItemType<FlybackHand>(),
                ItemType<GalvanizedHand>(),
                ItemType<Evolution>(),
                ItemType<Dialectics>(),
                ItemType<SpearOfEscape>()
            ];
            MaleNPC =
            [
                NPCID.Merchant,
                NPCID.Demolitionist,
                NPCID.DyeTrader,
                NPCID.GoblinTinkerer,
                NPCID.Angler,
                NPCID.ArmsDealer,
                NPCID.Golfer,
                NPCID.OldMan,
                NPCID.Painter,
                NPCID.TravellingMerchant,
                NPCID.Pirate,
                NPCID.TaxCollector,
                NPCID.Wizard,
                NPCID.SantaClaus,
                NPCID.Clothier,
                NPCID.Cyborg
            ];
            FemaleNPC =
            [
                NPCID.Dryad,
                NPCID.Mechanic,
                NPCID.Nurse,
                NPCID.PartyGirl,
                NPCID.BestiaryGirl,
                NPCID.Steampunker,
                NPCID.Princess,
                NPCID.Stylist
            ];
            LegalFoodList =
            [
                ItemID.Marshmallow,
                ItemID.JojaCola,
                ItemID.Apple,
                ItemID.Apricot,
                ItemID.Banana,
                ItemID.BlackCurrant,
                ItemID.BloodOrange,
                ItemID.Cherry,
                ItemID.Coconut,
                ItemID.Elderberry,
                ItemID.Grapefruit,
                ItemID.Lemon,
                ItemID.Mango,
                ItemID.Peach,
                ItemID.Pineapple,
                ItemID.Plum,
                ItemID.Pomegranate,
                ItemID.Rambutan,
                ItemID.SpicyPepper,
                ItemID.Teacup,
                ItemID.Dragonfruit,
                ItemID.Starfruit,
                ItemID.ChristmasPudding,
                ItemID.CookedFish,
                ItemID.GingerbreadCookie,
                ItemID.SugarCookie,
                ItemID.PumpkinPie,
                ItemID.FroggleBunwich,
                ItemID.AppleJuice,
                ItemID.BunnyStew,
                ItemID.CookedMarshmallow,
                ItemID.GrilledSquirrel,
                ItemID.Lemonade,
                ItemID.PeachSangria,
                ItemID.RoastedBird,
                ItemID.SauteedFrogLegs,
                ItemID.ShuckedOyster,
                ItemID.BowlofSoup,
                ItemID.MonsterLasagna,
                ItemID.PadThai,
                ItemID.Sashimi,
                ItemID.BananaSplit,
                ItemID.CoffeeCup,
                ItemID.CookedShrimp,
                ItemID.Escargot,
                ItemID.Fries,
                ItemID.BananaDaiquiri,
                ItemID.FruitJuice,
                ItemID.LobsterTail,
                ItemID.Pho,
                ItemID.RoastedDuck,
                ItemID.Burger,
                ItemID.Pizza,
                ItemID.Spaghetti,
                ItemID.BloodyMoscato,
                ItemID.MilkCarton,
                ItemID.PinaColada,
                ItemID.SmoothieofDarkness,
                ItemID.TropicalSmoothie,
                ItemID.ChickenNugget,
                ItemID.FriedEgg,
                ItemID.GrubSoup,
                ItemID.IceCream,
                ItemID.SeafoodDinner,
                ItemID.CreamSoda,
                ItemID.Grapes,
                ItemID.Hotdog,
                ItemID.Nachos,
                ItemID.FruitSalad,
                ItemID.PotatoChips,
                ItemID.ShrimpPoBoy,
                ItemID.ChocolateChipCookie,
                ItemID.PrismaticPunch,
                ItemID.ApplePie,
                ItemID.Milkshake,
                ItemID.Steak,
                ItemID.BBQRibs,
                ItemID.Bacon,
                ItemID.GoldenDelight
            ];
        }
        public override void PostSetupContent()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                
                Item item = new Item(i);
                    bool isFood = item.buffType == BuffID.WellFed || item.buffType == BuffID.WellFed2 || item.buffType == BuffID.WellFed3;
                    if (isFood && !LegalFoodList.Contains(item.type))
                        LegalFoodList.Add(item.type);
            }
        }
        public override void Unload()
        {
            ThrownSpearList = null;
            HJSpearList = null;
            LegalFoodList = null;
        }
    }
}
