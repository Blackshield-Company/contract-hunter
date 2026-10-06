using Mutagen.Bethesda.Plugins;

namespace ContractHunter.Generator;

/// <summary>
/// Vanilla Skyrim.esm records this mod links to. Only FormIDs are referenced;
/// no Bethesda data or assets are copied into the plugin or the repo.
/// FormIDs were looked up with the Mutagen.Bethesda.FormKeys.SkyrimSE package.
/// </summary>
internal static class Vanilla
{
    public static readonly ModKey Skyrim = ModKey.FromFileName("Skyrim.esm");
    private static FormKey K(uint id) => new(Skyrim, id);

    public static readonly FormKey PlayerRef = K(0x000014);
    public static readonly FormKey NordRace = K(0x013746);
    public static readonly FormKey ClassWarrior1H = K(0x013176);
    public static readonly FormKey CsHumanMelee = K(0x068849);
    public static readonly FormKey MQ101 = K(0x03372B);

    // Leveled bandits, split by gender so names match
    public static readonly FormKey LCharBanditMeleeAny = K(0x03DECD);
    public static readonly FormKey LCharBanditMissile = K(0x01E770);
    public static readonly FormKey LCharBanditMeleeNordM = K(0x01A320);
    public static readonly FormKey LCharBanditMeleeNordF = K(0x01A321);
    public static readonly FormKey LCharBanditMeleeEvenTonedM = K(0x01A323);
    public static readonly FormKey LCharBanditMeleeEvenTonedF = K(0x01A322);
    public static readonly FormKey LCharBanditMeleeCommonerM = K(0x01A319);
    public static readonly FormKey LCharBanditMeleeCommonerF = K(0x01A31E);
    public static readonly FormKey LCharBanditBossNordM = K(0x01A341);
    public static readonly FormKey LCharBanditBossNordF = K(0x01A340);
    public static readonly FormKey LCharBanditBossEvenTonedM = K(0x01A33F);
    public static readonly FormKey LCharBanditBossEvenTonedF = K(0x01A33B);
    public static readonly FormKey LCharBanditBossCommonerM = K(0x01A33A);
    public static readonly FormKey LCharBanditBossCommonerF = K(0x01A32F);
    public static readonly FormKey LCharBanditBossOrcM = K(0x01B0EA);

    // Holds (for per-hold standing)
    public static readonly FormKey HaafingarHoldLocation = K(0x016770);
    public static readonly FormKey ReachHoldLocation = K(0x016769);
    public static readonly FormKey HjaalmarchHoldLocation = K(0x01676E);
    public static readonly FormKey WhiterunHoldLocation = K(0x016772);
    public static readonly FormKey FalkreathHoldLocation = K(0x01676F);
    public static readonly FormKey PaleHoldLocation = K(0x01676D);
    public static readonly FormKey WinterholdHoldLocation = K(0x01676B);
    public static readonly FormKey EastmarchHoldLocation = K(0x01676A);
    public static readonly FormKey RiftHoldLocation = K(0x01676C);

    // Location data
    public static readonly FormKey LocTypeBanditCamp = K(0x0130DF);
    public static readonly FormKey LocTypeDungeon = K(0x0130DB);
    public static readonly FormKey LocTypeMilitaryFort = K(0x0130E7);
    public static readonly FormKey LocTypeCity = K(0x013168);
    public static readonly FormKey LocTypeTown = K(0x013166);
    public static readonly FormKey LocTypeSettlement = K(0x013167);
    public static readonly FormKey LocTypeDwelling = K(0x0130DC);
    public static readonly FormKey LocTypeHabitation = K(0x039793);
    public static readonly FormKey LocTypePlayerHouse = K(0x0FC1A3);
    public static readonly FormKey RefTypeMapMarker = K(0x10F63C);
    public static readonly FormKey CompanionsRadiantAllowedHolds = K(0x0F9073);

    // Items, keywords, sounds, art
    public static readonly FormKey Gold001 = K(0x00000F);
    public static readonly FormKey VendorItemBook = K(0x0937A2);
    public static readonly FormKey VendorItemClutter = K(0x0914E9);
    public static readonly FormKey VendorItemJewelry = K(0x08F95A);
    public static readonly FormKey ITMNoteUp = K(0x0C7A54);
    public static readonly FormKey ITMGenericUpSD = K(0x03C7BA);
    public static readonly FormKey ITMGenericDownSD = K(0x03C7B9);
    public static readonly FormKey InvArtNote01 = K(0x097788);
    public static readonly FormKey InvArtNote02 = K(0x01541B);
    public static readonly FormKey EquipVoice = K(0x025BEE);
    public static readonly FormKey EquipEitherHand = K(0x013F44);

    // Magic effects
    public static readonly FormKey AbFortifyHealth = K(0x0A1A3F);

    // Meshes (by path)
    public const string MeshNote01 = @"Clutter\Books\Note01.nif";
    public const string MeshNote02 = @"Clutter\Books\Note02.nif";
    public const string MeshGoldRing = @"Armor\AmuletsandRings\GoldRingGO.nif";
    public const string MeshSkull = @"Clutter\Bones\HumanSkullFull.nif";
}
