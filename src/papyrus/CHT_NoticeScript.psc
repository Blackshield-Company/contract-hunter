Scriptname CHT_NoticeScript extends ObjectReference
{Contract Hunter - posted Bounty Notice. Reading it opens the request menu
(same path as the Hunter's Ledger) once the hunter has signed the writ.}

CHT_HunterScript Property Hunter Auto

Event OnRead()
	; Utility.Wait does not advance while menus are open, so the offer appears
	; right after the player closes the notice and inventory.
	Utility.Wait(0.1)
	Hunter.OnNoticeRead()
EndEvent
