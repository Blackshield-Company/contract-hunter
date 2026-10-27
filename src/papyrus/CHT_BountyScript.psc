Scriptname CHT_BountyScript extends Quest
{Contract Hunter - one bounty on one named outlaw. Event driven: the target's
death and 3D load come from CHT_TargetAliasScript, the deadline is a single
game-time update. No polling.}

CHT_HunterScript Property Hunter Auto
Int Property Tier Auto
{0 Notice, 1 Wanted, 2 Dread}
Int Property OutlawIndex Auto
Float Property DeadlineDays Auto
LocationAlias Property ContractLocation Auto
ReferenceAlias Property MapMarker Auto
ReferenceAlias Property Target Auto
LeveledActor Property Accomplices Auto
{Spawned next to the target, one per point of notoriety.}
Spell Property CHT_AbDreadOutlaw Auto

Int Property OBJ_MAIN = 10 AutoReadOnly
Int Property STAGE_STARTED = 10 AutoReadOnly
Int Property STAGE_DONE = 100 AutoReadOnly
Int Property STAGE_EXPIRED = 150 AutoReadOnly
Int Property STAGE_ABANDONED = 200 AutoReadOnly
Int Property STAGE_VOIDED = 160 AutoReadOnly
; Kill credit radius, in game units (4096 units is about 58 metres, roughly
; bow range). Inside it the player counts as part of the fight.
Float Property NEAR_UNITS = 4096.0 AutoReadOnly

Bool finished = False
Bool extrasSpawned = False
Int extraGuards = 0
Float deadlineAt = 0.0

; Called by CHT_HunterScript after a save load. The deadline registration does not survive one.
Function ResumeAfterLoad()
	If finished || !IsRunning()
		Return
	EndIf
	Float now = Utility.GetCurrentGameTime()
	If deadlineAt <= 0.0
		deadlineAt = now + DeadlineDays
	EndIf
	Float hours = (deadlineAt - now) * 24.0
	If hours < 0.05
		hours = 0.05
	EndIf
	RegisterForSingleUpdateGameTime(hours)
EndFunction

; Called by CHT_HunterScript right after Start() succeeds.
Function Begin(Int aiNotoriety)
	finished = False
	extrasSpawned = False
	extraGuards = aiNotoriety
	ObjectReference marker = MapMarker.GetReference()
	If marker
		marker.AddToMap()
	EndIf
	SetStage(STAGE_STARTED)
	SetObjectiveDisplayed(OBJ_MAIN)
	deadlineAt = Utility.GetCurrentGameTime() + DeadlineDays
	RegisterForSingleUpdateGameTime(DeadlineDays * 24.0)
	If aiNotoriety > 0
		Debug.Notification("This outlaw has escaped hunters before (notoriety " + aiNotoriety + "). Expect company.")
	EndIf
	; OnLoad won't fire if the target is already loaded near the player.
	Actor victim = Target.GetActorReference()
	If victim && victim.Is3DLoaded()
		TargetLoaded()
	EndIf
EndFunction

Function TargetLoaded()
	If extrasSpawned || finished
		Return
	EndIf
	Actor victim = Target.GetActorReference()
	If !victim || victim.IsDead()
		Return
	EndIf
	extrasSpawned = True
	; Diagnostic only, so a Papyrus log shows what the target was doing on arrival.
	Actor player = Game.GetPlayer()
	String dist = "unknown"
	If player.Is3DLoaded() && victim.Is3DLoaded()
		dist = "" + player.GetDistance(victim)
	EndIf
	Debug.Trace("CHT_ target loaded: tier " + Tier + ", outlaw " + OutlawIndex + ", in combat " + victim.IsInCombat() + ", player distance " + dist)
	If Tier == 2
		victim.AddSpell(CHT_AbDreadOutlaw, False)
	EndIf
	Int i = 0
	While i < extraGuards
		victim.PlaceAtMe(Accomplices)
		i += 1
	EndWhile
EndFunction

; Kill credit. The bounty is voided only when someone else clearly killed the
; outlaw while the player was far away (for example soldiers at a nearby camp).
; Player-allied killers (followers, mercenaries, summons) standing near the
; player still count, because the player is near. Anything ambiguous (no
; killer, the player, a teammate, the victim itself, or the player within
; NEAR_UNITS) returns False and the bounty is paid as before.
Bool Function IsThirdPartyKill(Actor akKiller, Actor akVictim)
	Actor player = Game.GetPlayer()
	If !akKiller || akKiller == player || akKiller == akVictim
		Return False
	EndIf
	If akKiller.IsPlayerTeammate()
		Return False
	EndIf
	; A victim that is not 3D loaded counts as far from the player.
	If akVictim && akVictim.Is3DLoaded() && player.Is3DLoaded() && player.GetDistance(akVictim) <= NEAR_UNITS
		Return False
	EndIf
	Return True
EndFunction

Function TargetDied(Actor akKiller = None)
	If finished || !IsRunning()
		Return
	EndIf
	Actor victim = Target.GetActorReference()
	Actor player = Game.GetPlayer()
	String killerId = "none"
	If akKiller
		killerId = "" + akKiller.GetFormID()
		If akKiller == player
			killerId += " (player)"
		ElseIf akKiller.IsPlayerTeammate()
			killerId += " (teammate)"
		EndIf
	EndIf
	String dist = "unknown"
	If victim && victim.Is3DLoaded() && player.Is3DLoaded()
		dist = "" + player.GetDistance(victim)
	EndIf
	Debug.Trace("CHT_ target died: tier " + Tier + ", outlaw " + OutlawIndex + ", killer " + killerId + ", player distance " + dist)
	If IsThirdPartyKill(akKiller, victim)
		Debug.Trace("CHT_ target died: third-party kill far from the player, bounty voided")
		finished = True
		UnregisterForUpdateGameTime()
		FailAllObjectives()
		Hunter.BountyVoided(OutlawIndex, Tier)
		SetStage(STAGE_VOIDED)
		Stop()
		Return
	EndIf
	Debug.Trace("CHT_ target died: kill credited, bounty paid")
	finished = True
	UnregisterForUpdateGameTime()
	CompleteAllObjectives()
	Hunter.BountyClaimed(OutlawIndex, Tier, ContractLocation.GetLocation())
	SetStage(STAGE_DONE)
	Stop()
EndFunction

; The deadline.
Event OnUpdateGameTime()
	If finished || !IsRunning()
		Return
	EndIf
	Fail(False)
EndEvent

Function Abandon()
	If finished
		Return
	EndIf
	Fail(True)
EndFunction

Function Fail(Bool abAbandoned)
	finished = True
	UnregisterForUpdateGameTime()
	FailAllObjectives()
	Hunter.BountyFailed(OutlawIndex, Tier, ContractLocation.GetLocation(), abAbandoned)
	If abAbandoned
		SetStage(STAGE_ABANDONED)
	Else
		SetStage(STAGE_EXPIRED)
	EndIf
	Stop()
EndFunction
