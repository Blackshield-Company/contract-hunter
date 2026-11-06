# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

## [0.2.0] - 2026-11-05

Early beta. Built and checked, but not yet played in game (no Skyrim install is available to the build).

### Changed
- Couriers are **off by default**. The Hunter's Writ is no longer pushed into the player's inventory by a courier message. Find it on steward desks in jarl longhouses, or buy it from merchants who sell books (runtime `AddForm` into vanilla book leveled lists; the plugin does not override those lists). Set `CHT_CourierEnabled` to 1 to restore the old courier writ and letter schedule.
- When courier delivery is on, `CanDeliver` is stricter: Tamriel exterior only (not interiors, Solstheim, Soul Cairn, etc.), not sitting or in a menu/dialogue, and blocked during MQ101 plus a short soft DLC main-quest list (Dawnguard Awakening/Bloodline, Dragonborn MQ01/MQ02/MQ06) so letters do not fire in the wrong worldspace or mid-scene.
- Starting a bounty from the ledger or a posted notice no longer shows the courier popup.

### Added
- **Bounty Notice** book (`CHT_BountyNoticeBook`): reading it (after signing the writ) opens the same request menu as the Hunter's Ledger.
- Global `CHT_CourierEnabled` (default 0).
- Save-compatible with 0.1.4: every previous FormID and EditorID kept; new IDs only from `0x857+`; script properties added only; `CHT_HunterQuest` still start-enabled; `GameLoaded` migration stops courier timers when courier is off; open bounties still `ResumeAfterLoad`; `CHT_BountiesClaimed` unchanged.

### Fixed
- A third party (for example soldiers at a nearby camp) killing an outlaw while the hunter is far away no longer pays the bounty or counts as a lapse. The warrant is withdrawn with a message, with no change to notoriety or standing, and the outlaw can be posted again later. Kills by the player or a teammate, by an unknown killer, or by anyone while the player is close by (within about 58 m) still pay as before.
- Papyrus-log diagnostics: lines starting with `CHT_ ` record when a bounty target first loads and when it dies (killer, player distance, and whether the bounty was paid or voided).

## [0.1.4] - 2026-10-24

Still early beta. Built and checked, but not yet played in game (no Skyrim install is available to the build). Reported by a Nexus tester.

### Fixed
- The "Standing with the holds" screen, the "Bounty claimed" box, the lapsed-bounty box and the jarl's purse message printed a literal backslash-n instead of starting a new line. Papyrus text now takes its line breaks from a real line-feed character, and the build fails if any script string contains a backslash escape. A sweep of every other script, message, book, quest and NPC text found no other occurrences.
- Taking a bounty from the Hunter's Ledger now counts as the current courier letter, so a second letter no longer arrives minutes later because of the timer started at signing. A courier still brings a bounty of a *different* tier when one is due (one open bounty per tier, as before).
- The open-bounty flags are rebuilt from the bounty quests that are actually running every time a save is loaded.

### Changed
- Outlaws sandbox around their spot with a vanilla AI package instead of standing still until you arrive. Untested in game; please report any outlaw that wanders off or behaves oddly.
- The courier message says that one bounty per tier can be open.

## [0.1.3] - 2026-10-11

Still early beta, not yet playtested in game.

### Fixed
- The Hunter's Writ could never arrive if the Helgen quest (MQ101) stayed flagged as running, which happens with some alternate-start and skip-Helgen setups. Once "Before the Storm" has started, Helgen now counts as done. While the writ is waiting, the Papyrus log says why (grep CHT_).

## [Unreleased]

## [0.1.2] - 2026-10-10

Still early beta, not yet playtested in game.

### Fixed
- Wrye Bash could not load the plugin (IndexError "list index out of range"). The first stage's quest log entry was written without its QSDT (log entry flags) subrecord, which every log entry must start with. Every log entry now has one. No gameplay changes.

## [0.1.1] - 2026-10-08

### Fixed
- Bounty letters now name the outlaw and the location. The Letter alias on every bounty quest stores text and uses the stored names, so `<Alias=Target>` and `<Alias=ContractLocation>` resolve instead of printing raw on the page.
- Save/load no longer stops courier letters, bounty deadlines, or avengers. A start-game-enabled load quest restarts them. A deadline already running keeps its remaining time.
- Ships `SEQ/ContractHunter.seq`. On 1.6 the hunter quest does not start without it.

## [0.1.0] - 2026-10-06

Early beta. First public release. It passes every automated check in the build (Papyrus compile, plugin validation, round trip, FOMOD schema) but has not been playtested in-game yet. Please report problems on the Nexus page or on GitHub.

### Added
- The Hunter's Writ: a courier brings it once you're level 3 and past Helgen. Sign it to get the lesser power "Hunter's Ledger".
- 22 named outlaws in three tiers, each with their own bounty: Notice (8, 5-day deadline), Wanted (8, level 10+, one bodyguard, 7 days) and Dread (6, level 20+, two guards and +100 Health, 10 days).
- Bounty letters by courier every 2 in-game days (up to one open bounty per tier), or on request from the ledger. Each letter names the outlaw and a real vanilla location picked by the radiant system.
- Notoriety per outlaw: a lapsed or torn-up bounty raises it (max 3). Escaped outlaws come back with one extra accomplice per point and pay 25% more per point.
- Avengers: letting a Dread warrant lapse sends 2 to 4 Blood-Sworn Avengers after you, outdoors and outside settlements.
- Standing with each of the nine holds (0 to 100): +5/+10/+15 per claimed bounty, the same off for failures. Bounties pay up to 50% more with high standing, and the jarl sends 500 gold at 50.
- Trophies: a Dread Trophy Skull for every Dread target, an Outlaw's Signet from one Wanted target in three.
- Event-driven scripts: deaths and proximity come from alias events, deadlines from single game-time updates. No polling loops.
- FOMOD installer with optional Papyrus sources.
