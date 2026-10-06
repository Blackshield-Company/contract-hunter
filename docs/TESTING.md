# In-game test checklist (v0.1.0, early beta)

v0.1.0 ships as an early beta. It passed every automated build check but hasn't been played in-game yet. This checklist is for players and testers who want to confirm everything works, and for the next round of fixes.

Open the console with `~`. Commands that take a quest or global accept the EditorID directly. `player.additem` and `player.addspell` need a full FormID, which depends on your load order, so look it up first:

```
help "Hunter's Writ" 4         -> e.g. BOOK: (FE01482F) 'Hunter's Writ'
```

ESL-flagged plugins show up as `FE` + a 3-digit slot (`xxx`) + the record's own 3 digits. The record IDs are:

| Record | Local ID | Full FormID |
|---|---|---|
| Hunter's Writ (book) | 82F | `FExxx82F` |
| Hunter's Ledger (lesser power) | 82D | `FExxx82D` |
| Bounty letters: Notice, Wanted, Dread | 830, 831, 832 | `FExxx830` … |
| Outlaw's Signet, Dread Trophy Skull | 833, 834 | `FExxx833`, `FExxx834` |
| Blood-Sworn Avenger (leveled list) | 84D | `FExxx84D` |
| Bounty quests: Notice 01–08 | 801 to 808 | `CHT_BountyNotice01` … |
| Bounty quests: Wanted 01–08 | 809 to 810 | `CHT_BountyWanted01` … |
| Bounty quests: Dread 01–06 | 811 to 816 | `CHT_BountyDread01` … |

Globals: `CHT_Registered`, `CHT_OpenNotice`, `CHT_OpenWanted`, `CHT_OpenDread`, `CHT_OpenCount`, `CHT_BountiesClaimed`, `CHT_EscapedOutlaws`, and standing `CHT_StandingHaafingar`, `CHT_StandingReach`, `CHT_StandingHjaalmarch`, `CHT_StandingWhiterun`, `CHT_StandingFalkreath`, `CHT_StandingPale`, `CHT_StandingWinterhold`, `CHT_StandingEastmarch`, `CHT_StandingRift`.

**Turn on Papyrus logging before testing.** Add this to `Skyrim.ini` (under Proton: `.../compatdata/489830/pfx/drive_c/users/steamuser/Documents/My Games/Skyrim Special Edition/Skyrim.ini`):

```ini
[Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

The log is written to `.../My Games/Skyrim Special Edition/Logs/Script/Papyrus.0.log`. After each session, run `grep -n "CHT_" Papyrus.0.log` and report anything it finds.

---

## 0. Plugin loads
- [ ] `ContractHunter.esp` is enabled and the game reaches the main menu.
- [ ] Load a save. `show CHT_Registered` prints `0.00`. `sqv CHT_HunterQuest` shows the quest running.
- [ ] With Blackshield Company and/or Bounties of Oblivion also installed, all plugins load and `show BSC_Joined` / `show BOO_Bound` still work.

## 1. The writ
- [ ] Level 3+ character, past Helgen. Within about 30 seconds the courier message appears and the **Hunter's Writ** is in your inventory.
  - Faster: `set CHT_IntroMinLevel to 1`.
  - Bypass: `player.additem FExxx82F 1`.
- [ ] Drop or sell the writ without signing, then wait 24 in-game hours. It comes back.
- [ ] Read it and close the inventory. *Sign the writ / Not now* appears. *Not now* closes it; reading again offers it again.
- [ ] *Sign the writ*: the registered message appears, **Hunter's Ledger** is in Magic > Powers, and `show CHT_Registered` is `1.00`.

## 2. Courier letters
- [ ] About 6 in-game hours after signing (`set timescale to 2000` for a few seconds, then back to `20`), "A courier finds you…" appears and a **Bounty Notice** letter is in your inventory.
- [ ] The letter names an outlaw and a real location (no `<Alias=…>` text left over). The journal (Side Quests) shows "Bounty Notice: <name>" with a map marker on that location.
- [ ] Another letter arrives roughly every 2 days until one bounty of each allowed tier is open. Nothing arrives once all allowed tiers are open.
- [ ] At level 20+, a Dread warrant can arrive by courier (about one letter in three).

## 3. Ledger menus
- [ ] Cast the power. The ledger shows claimed / escaped / open counts and *Request a bounty / Tear up a bounty letter / Standing with the holds / Close*. With no open bounties, *Tear up* is hidden. With 3 open, *Request* is hidden.
- [ ] *Request a bounty* hides tiers already open. Below level 10, *Wanted* shows a "won't hand Wanted posters" notification; same for *Dread* below 20. *Back* returns to the ledger.
- [ ] *Standing with the holds* lists all nine holds at 0.
- [ ] Reading the writ again after signing opens the same ledger.

## 4. Each tier
For each tier, start a bounty, travel to the marker, kill the target. `sqv CHT_Bounty<Tier><NN>` shows alias fills (`ContractLocation`, `MapMarker`, `Target`, `Letter`, bodyguards). Fast path: `sqv`, `prid <Target id>`, `moveto player`, `kill`.

- [ ] **Notice**: the outlaw (name on the crosshair) is near the location's map marker, hostile, no bodyguards. Killing them shows the "Bounty claimed" box (gold, standing +5 with that hold) and the journal completes.
- [ ] **Wanted**: the outlaw has one bandit bodyguard. Payout includes +10 standing; about one in three also gives an **Outlaw's Signet**.
- [ ] **Dread**: the outlaw has two guards (melee and archer) and *Dread Reputation* (+100 Health) in their active effects once loaded. Payout includes +15 standing and a **Dread Trophy Skull**.
- [ ] If no location fits (rare), "The couriers come back empty-handed…" appears and nothing starts.
- [ ] Killing the target before reaching the marker (e.g. with `kill` from far away) still counts.

## 5. Deadlines, notoriety, avengers
- [ ] Start a Notice bounty, then let it lapse (`set timescale to 20000` for about 15 seconds, then back to `20`, or wait 5 days). The "A bounty lapsed. The outlaw slipped away." box shows their new notoriety (1), the hold loses 5 standing, and the journal shows it failed.
- [ ] Request Notice bounties until that outlaw comes back (escaped outlaws are offered first half the time). They now have one extra accomplice, and the payout says "+25% for an outlaw who had escaped before".
- [ ] *Tear up a bounty letter* on an open bounty counts the same as a lapse.
- [ ] Let a Dread warrant lapse. Within a few in-game hours, while outdoors and outside a city/town/home, "Blood-sworn avengers have tracked you down!" appears and 2 to 4 avengers attack. They don't appear while you're in a city, town, settlement, inn or home.

## 6. Standing rewards
- [ ] `set CHT_StandingWhiterun to 45`, claim a Wanted or Dread bounty in Whiterun Hold. Standing reaches 50+ and the jarl's 500-gold gift message appears. It doesn't repeat at higher standing.
- [ ] Payout gold is higher with high standing than at 0 for the same tier and level.

## 7. Persistence
- [ ] Save during an active bounty, quit to desktop, reload. The deadline still triggers, and killing the target still pays out.
- [ ] Save with pending avengers, reload. They still arrive.

## Stuck? Reset commands
```
stopquest CHT_BountyNotice01      (or any CHT_Bounty<Tier><NN>; then open the ledger once to resync)
set CHT_Registered to 1           (then player.addspell FExxx82D to get the power)
```
