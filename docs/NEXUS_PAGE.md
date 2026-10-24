# Nexus Mods page: Contract Hunter

Early beta: v0.1.3 fixes bounty letters printing raw `<Alias=…>` tags. Exact upload-form values are in [NEXUS_FIELDS.md](NEXUS_FIELDS.md).

## Page settings

- **Game:** Skyrim Special Edition
- **Mod name:** Contract Hunter
- **Version:** 0.1.3 (early beta)
- **Category:** Quests and Adventures
- **Suggested tags:** Quests, Lore-Friendly, Bounty, Gameplay, Radiant, ESL, No SKSE, Scripted
- **Requirements:** none.
- **Permissions (suggested):** source on GitHub under MIT, so allow modification and conversions with credit. Don't allow re-uploads of the archive itself.
- **File to upload:** `ContractHunter-0.1.3.zip` (FOMOD), main file, named "Contract Hunter".

## Description (paste into the BBCode editor)

```bbcode
[center][size=6][b]CONTRACT HUNTER[/b][/size]
[size=3][i]A name, a place, a deadline. Bring them down before they slip away.[/i][/size]

[size=3][color=#e0a030][b]EARLY BETA (v0.1.3)[/b]: bounty letters now name the outlaw and the location. v0.1.3 printed the raw alias tags. Please report bugs in the Bugs tab or on GitHub, ideally with your Papyrus log.[/color][/size][/center]

[line]

[size=4][b]Summary[/b][/size]
A courier finds you with the [b]Hunter's Writ[/b]. Sign it, and bounty letters start arriving: each one names a [b]real outlaw[/b] hiding at a [b]real place[/b] in Skyrim, with a deadline. Bring them down for gold and the gratitude of the hold. Let them get away and they come back bolder, and a lapsed [b]Dread warrant[/b] sends avengers after you. Solo bounty hunting, no follower company required.

[size=4][b]Features[/b][/size]
[list]
[*][b]22 named outlaws[/b], each with their own bounty, from Grisvar Two-Knives to Ulfbarn the Butcher. They spawn at vanilla bandit camps, dungeons and forts picked by the radiant system, and their name shows on the crosshair.
[*][b]Three tiers[/b]:
[list]
[*][b]Notice[/b]: small-time outlaws. 5-day deadline.
[*][b]Wanted[/b] (level 10+): gang leaders with a bodyguard. 7 days.
[*][b]Dread[/b] (level 20+): the worst in Skyrim, with two guards and extra health. 10 days.
[/list]
[*][b]Courier letters[/b] every couple of in-game days (one open bounty per tier at most), or request one any time from the [b]Hunter's Ledger[/b] lesser power.
[*][b]Notoriety[/b]: let a bounty lapse (or tear up the letter) and that outlaw's notoriety rises, up to 3. When they resurface, they bring an extra accomplice per point, and their price goes up 25% per point.
[*][b]Avengers[/b]: let a Dread warrant lapse and 2 to 4 Blood-Sworn Avengers track you down on the road.
[*][b]Standing with every hold[/b], 0 to 100. Claimed bounties raise it, failures lower it. High standing pays up to 50% more, and at 50 the jarl sends a purse of 500 gold.
[*][b]Rewards[/b]: gold that scales with tier and level, a Dread Trophy Skull for every Dread target, and the occasional Outlaw's Signet from Wanted targets.
[*]Event-driven scripts with no polling loops. All vanilla assets, so retextures apply.
[/list]

[size=4][b]Requirements[/b][/size]
[list]
[*]Skyrim Special Edition or Anniversary Edition (1.5.97 or 1.6.x). Works on Steam under Proton on Linux.
[*]Nothing else. No SKSE, no SkyUI, no DLC, no Creation Club content.
[/list]

[size=4][b]Installation[/b][/size]
Install with [b]Mod Organizer 2[/b] or [b]Vortex[/b] (FOMOD installer) and enable [b]ContractHunter.esp[/b]. It's ESL-flagged, so it doesn't take a full load-order slot. Safe to add mid-playthrough. The writ arrives at level 3, after Helgen (alternate-start friendly).

[size=4][b]Compatibility[/b][/size]
[list]
[*]No edits to vanilla records, cells or navmesh. Everything is new records. Skyrim.esm is the only master.
[*]Uses the vanilla radiant system to pick locations, so other radiant quests won't send you to a location a bounty has reserved, and the reverse.
[*]Fully compatible with [b]Blackshield Company[/b] and [b]Daedric Ledger: Bounties of Oblivion[/b] (same author). All three can run at the same time.
[*]Vanilla bounties (jarl and innkeeper quests) are untouched.
[/list]

[size=4][b]Uninstalling[/b][/size]
As with any scripted mod, don't remove it mid-playthrough. If you must, tear up your open bounties first.

[size=4][b]Known issues / early beta limits[/b][/size]
[list]
[*]Not playtested in-game yet. Found something? Please post it in the Bugs tab.
[*]No voice acting. Letters come by courier message rather than a walking courier NPC, and there are no notice boards (that would need world edits).
[*]Avengers appear near you rather than walking in from a distance. They never appear indoors or in cities, towns or homes.
[*]If no eligible location is free, the couriers come back empty-handed. Try again later.
[*]Settings are console globals for now: [code]set CHT_PayMult to 1.0 / set CHT_LetterIntervalDays to 2 / set CHT_IntroMinLevel to 3 / set CHT_WantedMinLevel to 10 / set CHT_DreadMinLevel to 20[/code]
[/list]

[size=4][b]Roadmap[/b][/size]
Capture-alive contracts, more outlaws and locations, notice boards as an optional patch, and an MCM.

[size=4][b]Source[/b][/size]
Open source (MIT): [url=https://github.com/Blackshield-Company/contract-hunter]github.com/Blackshield-Company/contract-hunter[/url]

[size=4][b]Credits[/b][/size]
[list]
[*][url=https://github.com/Mutagen-Modding/Mutagen]Mutagen[/url] by Noggog and contributors, which generates the plugin from code.
[*][url=https://github.com/russo-2025/papyrus-compiler]papyrus-compiler[/url] by russo-2025, the open-source Papyrus compiler.
[*][url=https://github.com/GandaG/fomod-schema]FOMOD schema[/url] by GandaG.
[*]Bethesda Game Studios for Skyrim.
[/list]
```

## Screenshots to capture in-game

Take these at 16:9, with the HUD hidden for scenery shots (`tm` in the console toggles menus).

1. **Hero shot.** Your character standing over a Dread outlaw at a fort, the trophy skull in hand. This goes on the mod tile.
2. **The writ.** The Hunter's Writ open in the book view.
3. **Courier.** The "A courier finds you…" box.
4. **A letter.** A Wanted Poster or Dread Warrant open, naming the outlaw and location.
5. **Journal.** The bounty in Side Quests with its map marker.
6. **The ledger.** The Hunter's Ledger menu.
7. **The target.** A named outlaw with the name on the crosshair, bodyguards nearby.
8. **Payout.** The "Bounty claimed" box with standing change.
9. **Escaped.** The lapsed-bounty box showing notoriety.
10. **Avengers.** Blood-Sworn Avengers attacking on a road.
11. **Standing.** The hold standing list.
