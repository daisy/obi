; JAWS scripts for Obi 2.5
; Scripts version: 2.5.1 , released on June 4, 2012
; written for JAWS 9.0 and above
; Written by: Avneesh Singh
; Copywrite (c)    2009 The DAISY Consortium

globals  String g_PrevControlClass
globals  Int g_SilentOnPhrases
Globals Int g_ExistingScreenEcho
Globals Int g_IsFirstAppStart

Void Function AutoStartEvent ()
if ( g_IsFirstAppStart == 0 ) Then
	SayString (GetWindowName (GetAppMainWindow (GetFocus ())))
	SayString ("Please use " + GetScriptKeyName ("ToggleEchoOnPhrases") + " to toggle phrase announcement by JAWS speech") ;
	let g_IsFirstAppStart = 1
	let g_SilentOnPhrases = 0
ElIf ( g_SilentOnPhrases == 1 ) Then
	SayString (GetWindowName (GetAppMainWindow (GetFocus())))
	SayString ("JAWS speech for phrases is OFF") 
EndIf
let g_ExistingScreenEcho = GetScreenEcho()
EndFunction

Void Function AutoFinishEvent ()
;SayInteger (g_ExistingScreenEcho  )
var int counter 
let counter = 0 ;
	While (GetScreenEcho () != g_ExistingScreenEcho  && counter <6) 
	ScreenEcho ()
	let counter = counter+1
	EndWhile
;SayInteger(GetScreenEcho ())
EndFunction


Script HotKeyHelp ()
SayString ("Scripts follow all Obi keyboard shortcuts, the only additional shortcut is" + GetScriptKeyName ("ToggleEchoOnPhrases")
 + " for toggling phrase announcement by JAWS speech") 
EndScript

Void Function FocusChangedEvent (handle FocusWindow, handle PrevWindow)
var String ContentViewBlocksClass 
let ContentViewBlocksClass = "WindowsForms10.Window.8.app.0.378734a"
	if	 ( ( IsJAWSversion9to12 () == 1
	|| StringContains (IntToString (GetJFWVersion ()), "13") == 1  
	|| StringContains (IntToString (GetJFWVersion ()), "14") == 1  )
	&&    GetWindowClass (GetFocus ()) == ContentViewBlocksClass  ) Then

		var String strObjectName
		let strObjectName = GetObjectName (0, 0) 
		if ( g_PrevControlClass != ContentViewBlocksClass 
		&& strObjectName == ""  ) Then
			SayString ("Content view ") ;
		EndIf
		If (strObjectName == "" ) Then
			SayStripString () 
		Else
			var int counter
			If ( g_SilentOnPhrases == 1
			&& (GetWindowName (GetFocus ()) =="Waveform1"  || StringContains (strObjectName , "phrase (") == 1 )) Then
				let counter = 0 ;
				While (GetScreenEcho () != 0 && counter <6) 
				ScreenEcho ()
				let counter = counter+1
				EndWhile
			;Saystring ("found")
		Else
			let counter = 0
				While (GetScreenEcho () != g_ExistingScreenEcho  && counter <6) 
				ScreenEcho ()
				let counter = counter+1
				EndWhile
			SayString ( strObjectName)
			EndIf
		EndIf
	Else

		FocusChangedEvent ( FocusWindow, PrevWindow)
	EndIf

let g_PrevControlClass = GetWindowClass (GetFocus ()) 
EndFunction

	Void Function FocusChangedEventEx (handle FocusWindow, handle PrevWindow)
If ( ISJAWSVersion9to12 () == 0 ) Then
	var String strObjectName
	let strObjectName = GetTypeAndTextStringsForWindow (GetFocus())
	var Int counter 
	If ( g_SilentOnPhrases == 1
	&& (GetWindowName (GetFocus ()) =="Waveform1"  || StringContains (strObjectName , "phrase (") == 1 )) Then
		let counter = 0 ;
		While (GetScreenEcho () != 0 && counter <6) 
			ScreenEcho ()
			let counter = counter+1
		EndWhile
	Else
		let counter = 0
			While (GetScreenEcho () != g_ExistingScreenEcho  && counter <6) 
			ScreenEcho ()
			let counter = counter+1
		EndWhile
	FocusChangedEventEx (FocusWindow, PrevWindow)
	EndIf

Else
	FocusChangedEventEx (FocusWindow, PrevWindow)

EndIf

EndFunction

Int Function IsJAWSversion9to12 ()
	if	 ( StringContains (IntToString (GetJFWVersion ()), "9") == 1 
	|| StringContains (IntToString (GetJFWVersion ()), "10") == 1  
	|| StringContains (IntToString (GetJFWVersion ()), "11") == 1  
	|| StringContains (IntToString (GetJFWVersion ()), "12") == 1  ) Then
Return 1
EndIf
Return 0
EndFunction


Void Function SayStripString ()
var String strControlName 
let strControlName =  StringTrimLeadingBlanks (GetControlName ())
let strControlName = StringTrimTrailingBlanks (strControlName )
;SayString(strControlName)
var String strObjectsList
let strObjectsList =  GetListOfObjects (GetFocus (), "=-:")
var String strDiscription  
let strDiscription = StringSegment (strObjectsList, "=-:", 1) 
;let strDiscription = StringReplaceChars (strDiscription, strControlName," ")
let strDiscription = StringReplaceSubstrings (strDiscription, "toolStripContainer1", "" ) ;
If ( g_SilentOnPhrases == 1 
&& (StringContains (IntToString (GetJFWVersion ()), "11") == 1  
	|| StringContains (IntToString (GetJFWVersion ()), "12") == 1  )
) Then
var Int index 
let index = StringContains (strDiscription, "phrase (") 
	If (index == 0 || index  >1)
	SayString ( strDiscription)
	EndIf
Else
	SayString ( strDiscription)
EndIf
EndFunction


Script ToggleContentView ()
If ( GetWindowClass (GetFocus ()) == "WindowsForms10.SysTreeView32.app.0.378734a" ) Then
	TypeCurrentScriptKey ()
	Delay (3)
	StopSpeech ()
Else
	TypeCurrentScriptKey ()
EndIf 

EndScript


Script ToggleEchoOnPhrases ()
If (g_SilentOnPhrases == 0 ) Then
	let g_SilentOnPhrases = 1
	let g_ExistingScreenEcho = GetScreenEcho ()
;SayInteger(GetScreenEcho())
	SayString ("Echo on phrases, OFF")
Else
	let g_SilentOnPhrases = 0
	var Int counter
	let counter = 0 
	While ( GetScreenEcho () != g_ExistingScreenEcho  && counter <6 )
	ScreenEcho ()
	let counter = counter+1
	EndWhile
	SayString ("Echo on phrases, ON")

EndIf
EndScript
