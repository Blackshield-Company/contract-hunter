using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using static ContractHunter.Generator.Vanilla;

namespace ContractHunter.Generator;

internal sealed class Builder
{
    private readonly SkyrimMod _mod;
    private readonly FormIdRegistry _ids;
    private static readonly SkyrimRelease R = SkyrimRelease.SkyrimSE;

    // Quest DNAM form version written by the SE Creation Kit (Mutagen defaults to 255).
    private const byte VanillaQuestFormVersion = 65;

    // Alias layout shared by every bounty quest
    private const int AliasLocation = 0, AliasMapMarker = 1, AliasTarget = 2, AliasPlayer = 3, AliasLetter = 4, AliasFirstGuard = 5;

    /// <summary>Script properties deliberately left unfilled ("EditorID:Script.Property"), for the validator.</summary>
    public HashSet<string> ExpectedUnfilled { get; } = new();

    public Builder(SkyrimMod mod, FormIdRegistry ids)
    {
        _mod = mod;
        _ids = ids;
    }

    private FormKey Id(string editorId) => _ids.Get(editorId);
    private static FormLink<T> L<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);

    private static FormKey Template(Texts.FormKeyRef r) => r switch
    {
        Texts.FormKeyRef.NoticeNordM => LCharBanditMeleeNordM,
        Texts.FormKeyRef.NoticeNordF => LCharBanditMeleeNordF,
        Texts.FormKeyRef.NoticeEvenM => LCharBanditMeleeEvenTonedM,
        Texts.FormKeyRef.NoticeEvenF => LCharBanditMeleeEvenTonedF,
        Texts.FormKeyRef.NoticeComM => LCharBanditMeleeCommonerM,
        Texts.FormKeyRef.NoticeComF => LCharBanditMeleeCommonerF,
        Texts.FormKeyRef.BossNordM => LCharBanditBossNordM,
        Texts.FormKeyRef.BossNordF => LCharBanditBossNordF,
        Texts.FormKeyRef.BossEvenM => LCharBanditBossEvenTonedM,
        Texts.FormKeyRef.BossEvenF => LCharBanditBossEvenTonedF,
        Texts.FormKeyRef.BossComM => LCharBanditBossCommonerM,
        Texts.FormKeyRef.BossComF => LCharBanditBossCommonerF,
        Texts.FormKeyRef.BossOrcM => LCharBanditBossOrcM,
        _ => throw new ArgumentOutOfRangeException(nameof(r)),
    };

    private sealed record TierSpec(Texts.Tier Text, Texts.Outlaw[] Outlaws, Level Level, FormKey[] LocTypes, FormKey[] Guards);

    // ----------------------------------------------------------------------------------------
    public void Build()
    {
        var questHunter = Id("CHT_HunterQuest");
        var tiers = new[]
        {
            new TierSpec(Texts.Tiers[0], Texts.Notice, Level.Medium, new[] { LocTypeBanditCamp, LocTypeDungeon }, Array.Empty<FormKey>()),
            new TierSpec(Texts.Tiers[1], Texts.Wanted, Level.Hard, new[] { LocTypeDungeon, LocTypeMilitaryFort }, new[] { LCharBanditMeleeAny }),
            new TierSpec(Texts.Tiers[2], Texts.Dread, Level.VeryHard, new[] { LocTypeDungeon, LocTypeMilitaryFort, LocTypeBanditCamp }, new[] { LCharBanditMeleeAny, LCharBanditMissile }),
        };
        // Reserve quest FormIDs first, in a stable order.
        var bountyQuests = tiers.SelectMany(t => t.Outlaws.Select((o, i) => Id($"CHT_Bounty{t.Text.Key}{i + 1:00}"))).ToArray();

        // --- Globals ---------------------------------------------------------------------------
        var gRegistered = Global("CHT_Registered", 0);
        var gOpen = tiers.Select(t => Global($"CHT_Open{t.Text.Key}", 0)).ToArray();
        var gOpenCount = Global("CHT_OpenCount", 0);
        var gClaimed = Global("CHT_BountiesClaimed", 0);
        var gEscaped = Global("CHT_EscapedOutlaws", 0);
        var gIntroMin = Global("CHT_IntroMinLevel", 3);
        var gWantedMin = Global("CHT_WantedMinLevel", 10);
        var gDreadMin = Global("CHT_DreadMinLevel", 20);
        var gPayMult = Global("CHT_PayMult", 1.0f);
        var gInterval = Global("CHT_LetterIntervalDays", 2.0f);
        var holds = new (string key, FormKey loc)[]
        {
            ("Haafingar", HaafingarHoldLocation), ("Reach", ReachHoldLocation), ("Hjaalmarch", HjaalmarchHoldLocation),
            ("Whiterun", WhiterunHoldLocation), ("Falkreath", FalkreathHoldLocation), ("Pale", PaleHoldLocation),
            ("Winterhold", WinterholdHoldLocation), ("Eastmarch", EastmarchHoldLocation), ("Rift", RiftHoldLocation),
        };
        var gStanding = holds.Select(h => Global($"CHT_Standing{h.key}", 0)).ToArray();

        // --- Magic -----------------------------------------------------------------------------
        var mgefLedger = Add(new MagicEffect(Id("CHT_LedgerEffect"), R)
        {
            EditorID = "CHT_LedgerEffect",
            Name = "Hunter's Ledger",
            Flags = MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoMagnitude | MagicEffect.Flag.NoArea
                    | MagicEffect.Flag.Painless | MagicEffect.Flag.NoHitEvent | MagicEffect.Flag.HideInUI,
            MagicSkill = ActorValue.None, ResistValue = ActorValue.None, SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.Script, ActorValue = ActorValue.None },
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = "Open the Hunter's Ledger.",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("CHT_LedgerEffectScript", Vmad.Obj("Hunter", questHunter))),
        });
        var powerLedger = Add(new Spell(Id("CHT_PowerLedger"), R)
        {
            EditorID = "CHT_PowerLedger",
            Name = "Hunter's Ledger",
            Description = "Open the Hunter's Ledger: request bounties, check open contracts, and see your standing with each hold.",
            Type = SpellType.LesserPower,
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            EquipmentType = L<IEquipTypeGetter>(EquipVoice).AsNullable(),
            Effects = { Effect(mgefLedger.FormKey, 0, 0) },
        });
        var abDread = Add(new Spell(Id("CHT_AbDreadOutlaw"), R)
        {
            EditorID = "CHT_AbDreadOutlaw",
            Name = "Dread Reputation",
            Description = "This outlaw has survived everything the holds sent.",
            Type = SpellType.Ability,
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            EquipmentType = L<IEquipTypeGetter>(EquipEitherHand).AsNullable(),
            Effects = { Effect(AbFortifyHealth, 100, 0) },
        });

        // --- Items -----------------------------------------------------------------------------
        Book Letter(string edid, string name, string text, string mesh, FormKey invArt, string? script) => Add(new Book(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            ObjectBounds = Bounds(-8, -5, 0, 8, 5, 0),
            Model = new Model { File = mesh },
            BookText = text,
            PickUpSound = L<ISoundDescriptorGetter>(ITMNoteUp).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
            Type = Book.BookType.NoteOrScroll,
            Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
            Value = 0,
            Weight = 0,
            InventoryArt = L<IStaticGetter>(invArt).AsNullable(),
            Description = "",
            VirtualMachineAdapter = script == null ? null : Vmad.Adapter(Vmad.Script(script, Vmad.Obj("Hunter", questHunter))),
        });
        var writ = Letter("CHT_HuntersWrit", Texts.WritName, Texts.Writ, MeshNote01, InvArtNote01, "CHT_WritScript");
        var letters = tiers.Select(t => Letter($"CHT_Letter{t.Text.Key}", t.Text.Title, t.Text.Letter, MeshNote02, InvArtNote02, null)).ToArray();

        MiscItem Trophy(string edid, string name, string mesh, ObjectBounds b, uint value, float weight, FormKey vendorKw) => Add(new MiscItem(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            ObjectBounds = b,
            Model = new Model { File = mesh },
            PickUpSound = L<ISoundDescriptorGetter>(ITMGenericUpSD).AsNullable(),
            PutDownSound = L<ISoundDescriptorGetter>(ITMGenericDownSD).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(vendorKw) },
            Value = value,
            Weight = weight,
        });
        var trophySignet = Trophy("CHT_TrophySignet", "Outlaw's Signet", MeshGoldRing, Bounds(-1, -1, 0, 1, 1, 0), 150, 0.1f, VendorItemJewelry);
        var trophySkull = Trophy("CHT_TrophySkull", "Dread Trophy Skull", MeshSkull, Bounds(-5, -8, -8, 5, 8, 8), 400, 5, VendorItemClutter);

        // --- Actors ----------------------------------------------------------------------------
        var allButName = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats | NpcConfiguration.TemplateFlag.Factions
            | NpcConfiguration.TemplateFlag.SpellList | NpcConfiguration.TemplateFlag.AIData | NpcConfiguration.TemplateFlag.AIPackages
            | NpcConfiguration.TemplateFlag.ModelAnimation | NpcConfiguration.TemplateFlag.Inventory | NpcConfiguration.TemplateFlag.Script
            | NpcConfiguration.TemplateFlag.DefPackList | NpcConfiguration.TemplateFlag.AttackData | NpcConfiguration.TemplateFlag.Keywords;

        var outlawNpcs = tiers.SelectMany(t => t.Outlaws.Select((o, i) =>
            Add(TemplatedNpc($"CHT_Outlaw{t.Text.Key}{i + 1:00}", o.Name, o.Short, Template(o.Template), allButName)))).ToArray();
        var avenger = Add(TemplatedNpc("CHT_DreadAvenger", "Blood-Sworn Avenger", "Avenger", LCharBanditMeleeAny, allButName));
        var avengerArcher = Add(TemplatedNpc("CHT_DreadAvengerArcher", "Blood-Sworn Avenger", "Avenger", LCharBanditMissile, allButName));
        var lvlAvenger = Add(new LeveledNpc(Id("CHT_LCharDreadAvenger"), R)
        {
            EditorID = "CHT_LCharDreadAvenger",
            Flags = LeveledNpc.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer,
            Entries = new[] { avenger, avenger, avengerArcher }.Select(n => new LeveledNpcEntry
            {
                Data = new LeveledNpcEntryData { Level = 1, Count = 1, Reference = L<INpcSpawnGetter>(n.FormKey) },
            }).ToExtendedList(),
        });

        // --- Messages --------------------------------------------------------------------------
        var msgIntro = Msg("CHT_MsgIntroCourier", Texts.IntroCourier);
        var msgRegister = Msg("CHT_MsgRegisterOffer", Texts.RegisterOffer, Btn("Sign the writ"), Btn("Not now"));
        var msgRegistered = Msg("CHT_MsgRegistered", Texts.Registered);
        var msgCourier = Msg("CHT_MsgBountyCourier", Texts.BountyCourier);
        var msgLedger = Msg("CHT_MsgLedgerMenu", Texts.LedgerMenu,
            Btn("Request a bounty", Cond.Global(gOpenCount, CompareOperator.LessThan, 3)),
            Btn("Tear up a bounty letter", Cond.Global(gOpenCount, CompareOperator.GreaterThan, 0)),
            Btn("Standing with the holds"),
            Btn("Close"));
        var msgRequest = Msg("CHT_MsgRequestMenu", Texts.RequestMenu,
            Btn("Notice", Cond.Global(gOpen[0], CompareOperator.EqualTo, 0)),
            Btn("Wanted", Cond.Global(gOpen[1], CompareOperator.EqualTo, 0)),
            Btn("Dread", Cond.Global(gOpen[2], CompareOperator.EqualTo, 0)),
            Btn("Back"));
        var msgAbandon = Msg("CHT_MsgAbandonMenu", Texts.AbandonMenu,
            Btn("Tear up the Notice", Cond.Global(gOpen[0], CompareOperator.EqualTo, 1)),
            Btn("Tear up the Wanted poster", Cond.Global(gOpen[1], CompareOperator.EqualTo, 1)),
            Btn("Tear up the Dread Warrant", Cond.Global(gOpen[2], CompareOperator.EqualTo, 1)),
            Btn("Back"));
        var msgNoBounty = Msg("CHT_MsgNoBounty", Texts.NoBounty);

        // --- Controller quest ------------------------------------------------------------------
        var hunter = Add(new Quest(questHunter, R)
        {
            EditorID = "CHT_HunterQuest",
            Name = "Contract Hunter",
            Flags = Quest.Flag.StartGameEnabled,
            Priority = 50,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        hunter.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("CHT_HunterScript",
            Vmad.Obj("CHT_Registered", gRegistered), Vmad.ObjList("OpenFlags", gOpen), Vmad.Obj("CHT_OpenCount", gOpenCount),
            Vmad.Obj("CHT_BountiesClaimed", gClaimed), Vmad.Obj("CHT_EscapedOutlaws", gEscaped),
            Vmad.Obj("CHT_IntroMinLevel", gIntroMin), Vmad.Obj("CHT_WantedMinLevel", gWantedMin), Vmad.Obj("CHT_DreadMinLevel", gDreadMin),
            Vmad.Obj("CHT_PayMult", gPayMult), Vmad.Obj("CHT_LetterIntervalDays", gInterval),
            Vmad.ObjList("Standing", gStanding),
            Vmad.ObjList("Holds", holds.Select(h => h.loc).ToArray()),
            Vmad.ObjList("Bounties", bountyQuests),
            Vmad.Obj("MQ101", MQ101), Vmad.Obj("MQ102", MQ102),
            Vmad.Obj("CHT_HuntersWrit", writ.FormKey), Vmad.Obj("CHT_PowerLedger", powerLedger.FormKey),
            Vmad.Obj("CHT_TrophySignet", trophySignet.FormKey), Vmad.Obj("CHT_TrophySkull", trophySkull.FormKey),
            Vmad.Obj("CHT_LCharDreadAvenger", lvlAvenger.FormKey), Vmad.Obj("Gold001", Gold001),
            Vmad.ObjList("SafeLocationTypes", LocTypeCity, LocTypeTown, LocTypeSettlement, LocTypeDwelling, LocTypeHabitation, LocTypePlayerHouse),
            Vmad.Obj("CHT_MsgIntroCourier", msgIntro), Vmad.Obj("CHT_MsgRegisterOffer", msgRegister),
            Vmad.Obj("CHT_MsgRegistered", msgRegistered), Vmad.Obj("CHT_MsgBountyCourier", msgCourier),
            Vmad.Obj("CHT_MsgLedgerMenu", msgLedger), Vmad.Obj("CHT_MsgRequestMenu", msgRequest),
            Vmad.Obj("CHT_MsgAbandonMenu", msgAbandon), Vmad.Obj("CHT_MsgNoBounty", msgNoBounty)));

        var questLoad = Id("CHT_LoadQuest");
        var load = Add(new Quest(questLoad, R)
        {
            EditorID = "CHT_LoadQuest",
            Name = "Contract Hunter",
            Flags = Quest.Flag.StartGameEnabled,
            Priority = 40,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
            NextAliasID = 1,
        });
        load.Aliases.Add(new QuestAlias
        {
            ID = 0,
            Type = QuestAlias.TypeEnum.Reference,
            Name = "Player",
            ForcedReference = new FormLinkNullable<IPlacedGetter>(PlayerRef),
        });
        load.VirtualMachineAdapter = Vmad.QuestAdapter()
            .WithAliasScripts(questLoad, 0, Vmad.Script("CHT_LoadAliasScript", Vmad.Obj("Hunter", questHunter)));

        // --- Bounty quests, one per named outlaw -----------------------------------------------
        int index = 0;
        for (int t = 0; t < tiers.Length; t++)
        {
            var tier = tiers[t];
            for (int i = 0; i < tier.Outlaws.Length; i++, index++)
            {
                var fk = bountyQuests[index];
                var edid = $"CHT_Bounty{tier.Text.Key}{i + 1:00}";
                var q = Add(new Quest(fk, R)
                {
                    EditorID = edid,
                    Name = $"{tier.Text.Title}: {tier.Outlaws[i].Name}",
                    Priority = 55,
                    Type = Quest.TypeEnum.SideQuest,
                    QuestFormVersion = VanillaQuestFormVersion,
                });
                q.Stages.Add(new QuestStage { Index = 0, Flags = QuestStage.Flag.StartUpStage });
                q.Stages.Add(new QuestStage { Index = 10, LogEntries = { new QuestLogEntry { Flags = 0, Entry = tier.Text.StartLog } } });
                q.Stages.Add(new QuestStage { Index = 100, LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.CompleteQuest, Entry = Texts.StageDone } } });
                q.Stages.Add(new QuestStage { Index = 150, Flags = QuestStage.Flag.ShutDownStage, LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.FailQuest, Entry = Texts.StageExpired } } });
                q.Stages.Add(new QuestStage { Index = 200, Flags = QuestStage.Flag.ShutDownStage, LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.FailQuest, Entry = Texts.StageAbandoned } } });

                var loc = new QuestAlias
                {
                    ID = AliasLocation, Type = QuestAlias.TypeEnum.Location, Name = "ContractLocation",
                    Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText | QuestAlias.Flag.AllowCleared,
                };
                loc.Conditions.Add(Cond.InCurrentLocFormList(CompanionsRadiantAllowedHolds));
                for (int k = 0; k < tier.LocTypes.Length; k++)
                    loc.Conditions.Add(Cond.LocationHasKeyword(tier.LocTypes[k], or: k < tier.LocTypes.Length - 1));
                loc.Conditions.Add(Cond.LocationHasRefType(RefTypeMapMarker));
                q.Aliases.Add(loc);
                q.Aliases.Add(new QuestAlias
                {
                    ID = AliasMapMarker, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
                    Location = new LocationAliasReference { AliasID = AliasLocation, RefType = L<ILocationReferenceTypeGetter>(RefTypeMapMarker).AsNullable() },
                });
                q.Aliases.Add(new QuestAlias
                {
                    ID = AliasTarget, Type = QuestAlias.TypeEnum.Reference, Name = "Target",
                    Flags = QuestAlias.Flag.StoresText,
                    CreateReferenceToObject = new CreateReferenceToObject
                    {
                        Object = L<ISkyrimMajorRecordGetter>(outlawNpcs[index].FormKey), AliasID = AliasMapMarker,
                        Create = CreateReferenceToObject.CreateEnum.At, Level = tier.Level,
                    },
                });
                q.Aliases.Add(new QuestAlias
                {
                    ID = AliasPlayer, Type = QuestAlias.TypeEnum.Reference, Name = "Player",
                    ForcedReference = new FormLinkNullable<IPlacedGetter>(PlayerRef),
                });
                q.Aliases.Add(new QuestAlias
                {
                    ID = AliasLetter, Type = QuestAlias.TypeEnum.Reference, Name = "Letter",
                    // A created note does not inherit the quest's stored names unless this
                    // alias both stores text and uses it. Without that, the page shows
                    // the literal <Alias=Target> / <Alias=ContractLocation> tags.
                    Flags = QuestAlias.Flag.StoresText | QuestAlias.Flag.UsesStoredText,
                    CreateReferenceToObject = new CreateReferenceToObject
                    {
                        Object = L<ISkyrimMajorRecordGetter>(letters[t].FormKey), AliasID = AliasPlayer,
                        Create = CreateReferenceToObject.CreateEnum.In, Level = Level.Easy,
                    },
                });
                for (int g = 0; g < tier.Guards.Length; g++)
                {
                    q.Aliases.Add(new QuestAlias
                    {
                        ID = (uint)(AliasFirstGuard + g), Type = QuestAlias.TypeEnum.Reference, Name = $"Bodyguard{g + 1}",
                        Flags = QuestAlias.Flag.Optional,
                        CreateReferenceToObject = new CreateReferenceToObject
                        {
                            Object = L<ISkyrimMajorRecordGetter>(tier.Guards[g]), AliasID = AliasMapMarker,
                            Create = CreateReferenceToObject.CreateEnum.At, Level = Level.Medium,
                        },
                    });
                }
                q.NextAliasID = (uint)q.Aliases.Count;
                q.Objectives.Add(new QuestObjective
                {
                    Index = 10,
                    DisplayText = tier.Text.Objective,
                    Targets = { new QuestObjectiveTarget { AliasID = AliasTarget } },
                });
                q.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("CHT_BountyScript",
                        Vmad.Obj("Hunter", questHunter),
                        Vmad.Int("Tier", t),
                        Vmad.Int("OutlawIndex", index),
                        Vmad.Float("DeadlineDays", tier.Text.DeadlineDays),
                        Vmad.Alias("ContractLocation", fk, AliasLocation),
                        Vmad.Alias("MapMarker", fk, AliasMapMarker),
                        Vmad.Alias("Target", fk, AliasTarget),
                        Vmad.Obj("Accomplices", LCharBanditMeleeAny),
                        Vmad.Obj("CHT_AbDreadOutlaw", abDread.FormKey)))
                    .WithAliasScripts(fk, AliasTarget, Vmad.Script("CHT_TargetAliasScript"));
            }
        }
    }

    // ----------------------------------------------------------------------------------------
    // Record helpers
    // ----------------------------------------------------------------------------------------

    private T Add<T>(T record) where T : SkyrimMajorRecord
    {
        _mod.GetTopLevelGroup<T>().Add(record);
        return record;
    }

    private FormKey Global(string edid, float value)
    {
        var g = new GlobalFloat(Id(edid), R) { EditorID = edid, Data = value };
        _mod.Globals.Add(g);
        return g.FormKey;
    }

    private static Effect Effect(FormKey mgef, float magnitude, int duration) => new()
    {
        BaseEffect = new FormLinkNullable<IMagicEffectGetter>(mgef),
        Data = new EffectData { Magnitude = magnitude, Area = 0, Duration = duration },
    };

    private static ObjectBounds Bounds(short x1, short y1, short z1, short x2, short y2, short z2)
        => new() { First = new P3Int16(x1, y1, z1), Second = new P3Int16(x2, y2, z2) };

    private static MessageButton Btn(string text, params Condition[] conditions)
    {
        var b = new MessageButton { Text = text };
        b.Conditions.AddRange(conditions);
        return b;
    }

    private FormKey Msg(string edid, string text, params MessageButton[] buttons)
    {
        if (buttons.Length > 10) throw new InvalidOperationException($"{edid}: a message box holds at most 10 buttons");
        var m = new Message(Id(edid), R)
        {
            EditorID = edid,
            Description = text,
            Flags = Message.Flag.MessageBox,
            INAM = new byte[4],
        };
        m.MenuButtons.AddRange(buttons);
        _mod.Messages.Add(m);
        return m.FormKey;
    }

    private Npc TemplatedNpc(string edid, string name, string shortName, FormKey template, NpcConfiguration.TemplateFlag flags) => new(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ShortName = shortName,
        ObjectBounds = Bounds(-22, -14, 0, 22, 14, 128),
        Template = new FormLinkNullable<INpcSpawnGetter>(template),
        Race = L<IRaceGetter>(NordRace),
        Class = L<IClassGetter>(ClassWarrior1H),
        CombatStyle = L<ICombatStyleGetter>(CsHumanMelee).AsNullable(),
        Configuration = new NpcConfiguration
        {
            Flags = NpcConfiguration.Flag.AutoCalcStats,
            TemplateFlags = flags,
            Level = new PcLevelMult { LevelMult = 1 },
            CalcMinLevel = 1,
            CalcMaxLevel = 0,
            SpeedMultiplier = 100,
            DispositionBase = 35,
        },
        AIData = new AIData { Aggression = Aggression.Aggressive, Confidence = Confidence.Average, Assistance = Assistance.HelpsAllies, EnergyLevel = 50 },
        PlayerSkills = new PlayerSkills(),
        Height = 1,
        Weight = 50,
    };
}
