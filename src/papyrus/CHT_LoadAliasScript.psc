Scriptname CHT_LoadAliasScript extends ReferenceAlias
{Contract Hunter - on the load-watcher quest's player alias. Papyrus drops
update registrations on every save load. This alias is what puts them back.}

CHT_HunterScript Property Hunter Auto

Event OnInit()
	Hunter.GameLoaded()
EndEvent

Event OnPlayerLoadGame()
	Hunter.GameLoaded()
EndEvent
