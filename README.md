# Contract Hunter

A Skyrim Special Edition mod about solo bounty hunting. Couriers bring you bounty letters naming real outlaws hiding at real places. Deadlines matter: let a target slip away and they come back bolder, and a lapsed Dread warrant sends avengers after you. The jarls remember who keeps their roads clean.

**Status: v0.1.1, early beta.** It passes every automated check in the build (Papyrus compile, plugin validation, byte-exact round trip, FOMOD schema). v0.1.0 had not been playtested; a player confirmed bounty letters printed the raw `<Alias=…>` tags, and 0.1.1 fixes that. See [docs/TESTING.md](docs/TESTING.md).

- Game: Skyrim Special Edition / Anniversary Edition 1.6.x (also fine on 1.5.97). Works on Windows and on Linux through Proton.
- Requirements: none beyond the base game. No SKSE, no SkyUI, no DLC, no Creation Club content.
- Plugin: `ContractHunter.esp`, ESL-flagged, Skyrim.esm is the only master. No vanilla records edited.
- Compatible with [Blackshield Company](https://github.com/synthalorian/blackshield-company) and [Daedric Ledger: Bounties of Oblivion](https://github.com/synthalorian/bounties-of-oblivion): separate prefixes (`CHT_`), records and globals, and all three reserve radiant locations through the vanilla system so they never share a target location.

## What it does

| Feature | How it works |
|---|---|
| Registration | At level 3, past Helgen, a courier hands you the **Hunter's Writ**. Sign it to get the lesser power **Hunter's Ledger**. |
| Bounty letters | Every 2 in-game days a courier brings a new letter, or ask for one in the ledger. Each letter names one outlaw and a vanilla location picked by the radiant system. Up to one open bounty per tier. |
| Tiers | **Notice**: small-time outlaws, 5 days. **Wanted** (level 10+): gang leaders with a bodyguard, 7 days. **Dread** (level 20+): the worst, two guards and +100 Health, 10 days. |
| Named outlaws | 22 of them, each with a fixed name and their own bounty, on vanilla bandit templates (gender-matched). Killed outlaws stay dead until a whole tier is cleared out. |
| Notoriety | A lapsed or torn-up bounty raises that outlaw's notoriety (max 3). When they come back (escaped outlaws are offered first half the time), they bring one extra accomplice per point and pay 25% more per point. |
| Avengers | Let a Dread warrant lapse and 2 to 4 Blood-Sworn Avengers track you down within a few hours, outdoors and outside cities, towns and homes. |
| Hold standing | Each of the nine holds tracks your standing (0 to 100) in a global. +5/+10/+15 per Notice/Wanted/Dread bounty claimed there, the same off for failures. Bounties pay `standing / 2` % more. At 50 the jarl sends 500 gold. |
| Rewards | Gold scaled to tier and level. A Dread Trophy Skull for every Dread target, an Outlaw's Signet from one Wanted target in three. |

There is no new voice acting and no world edits: everything arrives by courier message and letter, and the ledger is a lesser power.

## Install

Grab `ContractHunter-<version>.zip` from [Releases](https://github.com/synthalorian/contract-hunter/releases) (FOMOD installer).

- **Mod Organizer 2**: *Install a new mod from an archive*, pick the zip, enable `ContractHunter.esp`.
- **Vortex**: drag the zip into the Mods page, install and enable it.
- **Manual (Linux/Proton)**: copy everything inside `00 Core/` into `.../steamapps/common/Skyrim Special Edition/Data/`, then add `*ContractHunter.esp` to `.../steamapps/compatdata/489830/pfx/drive_c/users/steamuser/AppData/Local/Skyrim Special Edition/Plugins.txt`.

Safe to add to an existing save. Don't uninstall mid-playthrough.

## Console tunables

```
set CHT_IntroMinLevel to 3        (level before the writ arrives)
set CHT_WantedMinLevel to 10      (level for Wanted posters)
set CHT_DreadMinLevel to 20       (level for Dread warrants)
set CHT_LetterIntervalDays to 2   (days between courier letters)
set CHT_PayMult to 1.0            (multiplier on bounty gold)
```

## Build from source

`./build.sh` on Linux builds the release zip from this repo: Papyrus is compiled with the open-source [papyrus-compiler](https://github.com/russo-2025/papyrus-compiler), the plugin is generated from C# with [Mutagen](https://github.com/Mutagen-Modding/Mutagen) and validated (ESL range, masters, links, script properties, script base types, alias scripts, quest text tags, byte-exact round trip), and the FOMOD is schema-checked and zipped reproducibly. Needs .NET SDK 9+, python3, curl and optionally xmllint. See [tools/modkit/README.md](tools/modkit/README.md).

`src/plugin/formids.json` pins every record's FormID. Never edit or reuse entries.

## Layout

```
src/papyrus/      Papyrus sources (.psc)
src/plugin/       Mutagen plugin generator + pinned FormIDs
fomod/            FOMOD installer config
docs/             Testing checklist, design notes, Nexus page and upload fields
tools/modkit/     Shared tooling for the mod series
build.sh          One-command build
```

## Credits and licenses

- Code in this repo: MIT (see `LICENSE`).
- [Mutagen](https://github.com/Mutagen-Modding/Mutagen) by Noggog and contributors (GPL-3.0), used as a build tool, not redistributed.
- [papyrus-compiler](https://github.com/russo-2025/papyrus-compiler) by russo-2025 (MIT), downloaded at build time.
- FOMOD schema from [GandaG/fomod-schema](https://github.com/GandaG/fomod-schema) (MIT).
- Skyrim and its assets belong to Bethesda Softworks. This repo contains no Bethesda assets or script sources; the plugin references vanilla records by FormID and vanilla meshes by path.
