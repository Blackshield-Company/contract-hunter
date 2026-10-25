namespace ContractHunter.Generator;

/// <summary>All player-facing text in one place.</summary>
internal static class Texts
{
    public const string WritName = "Hunter's Writ";
    public const string Writ =
@"<font face='$HandwrittenFont'>
To whoever is holding this,

The holds pay for outlaws. Dead ones. The jarls post the notices, the couriers carry them, and nobody asks who collects, so long as the right head stops breathing.

Put your mark below and the couriers will know your name. Bounty letters will find you on the road: small notices at first, then the Wanted, and if you live long enough, the Dread.

Every letter has a deadline. Let one lapse and the outlaw learns your face, gets bolder, and hires more blades. Let a Dread target walk, and their friends will come looking for you.

The jarls remember who keeps their roads clean.

- A clerk of the hold courts
</font>";

    public const string IntroCourier =
        "A courier jogs up to you, out of breath. \"Letter for you. From the hold courts, they said. Something about bounties.\"";

    public const string RegisterOffer =
        "The Hunter's Writ has a line at the bottom for your mark.\n\nSign it, and bounty letters will start finding you: Notice, Wanted and Dread targets across Skyrim's holds, each with a deadline.";

    public const string Registered =
        "You sign the writ. The ink is barely dry before you hear the couriers have your name.\n\nYou can now open the Hunter's Ledger (a lesser power) to request bounties, check your open contracts, and see your standing with each hold.";

    public const string BountyCourier =
        "A courier finds you. \"Bounty letter, hunter. Sealed by the hold courts.\" It's in your inventory. You can hold one open bounty per tier.";

    public const string LedgerMenu =
        "HUNTER'S LEDGER\n\nBounties claimed: %.0f\nOutlaws who escaped you: %.0f\nOpen bounties: %.0f / 3";

    public const string RequestMenu =
        "Which notices will you take?\n\nNOTICE: small-time outlaws. 5 days.\nWANTED: gang leaders with a bodyguard. 7 days. Level 10+.\nDREAD: the worst in Skyrim, with two guards. 10 days. Level 20+. Failure has consequences.";

    public const string AbandonMenu =
        "Tear up a bounty letter? It counts as a lapsed contract: the outlaw's notoriety grows and the hold remembers.";

    public const string NoBounty =
        "The couriers come back empty-handed. No outlaw of that kind can be placed right now. Try again later.";

    public sealed record Outlaw(string Name, string Short, FormKeyRef Template);
    public enum FormKeyRef { NoticeNordM, NoticeNordF, NoticeEvenM, NoticeEvenF, NoticeComM, NoticeComF, BossNordM, BossNordF, BossEvenM, BossEvenF, BossComM, BossComF, BossOrcM }

    public static readonly Outlaw[] Notice =
    {
        new("Grisvar Two-Knives", "Grisvar", FormKeyRef.NoticeNordM),
        new("Edda Coldcoin", "Edda", FormKeyRef.NoticeNordF),
        new("Brother Ulf", "Ulf", FormKeyRef.NoticeComM),
        new("Mirri the Magpie", "Mirri", FormKeyRef.NoticeEvenF),
        new("Haldor Stump", "Haldor", FormKeyRef.NoticeEvenM),
        new("Sigrun Wet-Boots", "Sigrun", FormKeyRef.NoticeComF),
        new("Kesh the Lean", "Kesh", FormKeyRef.NoticeEvenM),
        new("Olfrid Half-Ear", "Olfrid", FormKeyRef.NoticeNordM),
    };

    public static readonly Outlaw[] Wanted =
    {
        new("Valga Ironmouth", "Valga", FormKeyRef.BossNordF),
        new("Torsten the Flayer", "Torsten", FormKeyRef.BossNordM),
        new("Ashlyn Black-Brand", "Ashlyn", FormKeyRef.BossEvenF),
        new("Jorund Skullsplitter", "Jorund", FormKeyRef.BossNordM),
        new("Cassia Vane", "Cassia", FormKeyRef.BossComF),
        new("Hroki Stonefist", "Hroki", FormKeyRef.BossComM),
        new("Gromm gro-Bharz", "Gromm", FormKeyRef.BossOrcM),
        new("Dagny Red-Sleeves", "Dagny", FormKeyRef.BossNordF),
    };

    public static readonly Outlaw[] Dread =
    {
        new("Ulfbarn the Butcher", "Ulfbarn", FormKeyRef.BossNordM),
        new("Morwen Gallows-Queen", "Morwen", FormKeyRef.BossEvenF),
        new("Kjeld Ninefingers", "Kjeld", FormKeyRef.BossComM),
        new("Skarn Blackheart", "Skarn", FormKeyRef.BossEvenM),
        new("Thorna the Crow", "Thorna", FormKeyRef.BossComF),
        new("Vigdis the Pale", "Vigdis", FormKeyRef.BossNordF),
    };

    public sealed record Tier(string Key, string Title, int DeadlineDays, string Letter, string StartLog, string Objective);

    public static readonly Tier[] Tiers =
    {
        new("Notice", "Bounty Notice", 5,
@"<font face='$HandwrittenFont'>
BOUNTY NOTICE

By order of the hold courts, a bounty is posted on
<Alias=Target>

last seen near <Alias=ContractLocation>.

Petty robbery, a stabbing or two, and an unpaid tavern tab the size of a mammoth. Bring the outlaw down and the purse is yours.

This notice lapses in 5 days.
</font>",
            "A bounty notice reached me: <Alias=Target>, a small-time outlaw, is hiding near <Alias=ContractLocation>. The notice lapses in 5 days.",
            "Kill <Alias=Target> near <Alias=ContractLocation> (Notice, 5 days)"),
        new("Wanted", "Wanted Poster", 7,
@"<font face='$HandwrittenFont'>
WANTED

<Alias=Target>

Gang leader. Highway murder, extortion and arson. Travels with a hired blade. Last seen near <Alias=ContractLocation>.

A generous purse from the hold courts for proof of death.

This warrant lapses in 7 days.
</font>",
            "A Wanted poster reached me: <Alias=Target>, a gang leader travelling with a bodyguard, is near <Alias=ContractLocation>. The warrant lapses in 7 days.",
            "Kill <Alias=Target> near <Alias=ContractLocation> (Wanted, 7 days)"),
        new("Dread", "Dread Warrant", 10,
@"<font face='$HandwrittenFont'>
DREAD WARRANT

<Alias=Target>

The holds have tried soldiers. The holds have tried mercenaries. They are dead. This one keeps two killers close and has friends who settle debts.

Last seen near <Alias=ContractLocation>.

The richest purse the courts will pay. Do not take this warrant lightly: if it lapses, the outlaw's friends will know who held it.

This warrant lapses in 10 days.
</font>",
            "A Dread Warrant reached me: <Alias=Target>, one of the most dangerous outlaws in Skyrim, is near <Alias=ContractLocation> with two guards. If the warrant lapses in 10 days, their friends will come for me.",
            "Kill <Alias=Target> near <Alias=ContractLocation> (Dread, 10 days)"),
    };

    public const string StageDone = "The outlaw is dead and the hold courts paid the bounty.";
    public const string StageExpired = "The bounty lapsed. The outlaw got away, and is bolder for it.";
    public const string StageAbandoned = "I tore up the bounty letter. The outlaw got away.";
}
