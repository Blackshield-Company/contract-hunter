# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

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
