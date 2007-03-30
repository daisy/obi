; Scripts for Obi 0.9 - pre alpha Obi Test version
; by: Avneesh Singh


Script AltLeftHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf
EndScript


Script AltRightHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf
EndScript


Script AltUpHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf
EndScript



Script AltDownHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf

EndScript



Script AltShiftLeftHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf
EndScript


Script AltShiftRightHook ()
If IsPCCursor () Then
TypeCurrentScriptKey ()
EndIf
EndScript



Script SayWordOverriden ()
var String BlockName
let BlockName = GetObjectName (1) +" " +  GetObjectDescription (1)
If IsSameScript () Then
	If ( GetWord () == "Audio" 
	&& IsPCCursor () 
	&& BlockName != " "  ) Then
		SpellString ( BlockName)
	Else
		PerformScript SayWord()
	EndIf
Else
	If ( GetWord () == "Audio" 
	&& IsPCCursor () 
	&& BlockName != " " ) Then
		SayString ( BlockName )
	Else
		PerformScript SayWord()

	EndIf
EndIf

EndScript

