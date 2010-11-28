; JAWS scripts for Obi 1.2
; Scripts version: 1.2.1 , released on November 29, 2010
; written for JAWS 9.0 and above
; Written by: Avneesh Singh
; Copywrite (c)    2009 The DAISY Consortium


globals  String g_PrevControlClass

Void Function FocusChangedEvent (handle FocusWindow, handle PrevWindow)
var String ContentViewBlocksClass 
let ContentViewBlocksClass = "WindowsForms10.Window.8.app.0.378734a"
	if	 ( ( StringContains (IntToString (GetJFWVersion ()), "9") == 1 
	|| StringContains (IntToString (GetJFWVersion ()), "10") == 1  )
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
			SayString ( strObjectName)
		EndIf
	Else
		FocusChangedEvent ( FocusWindow, PrevWindow)
	EndIf
let g_PrevControlClass = GetWindowClass (GetFocus ()) 
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
SayString ( strDiscription)
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
