*========================================================================================
* Visual FoxPro IDE extension for Guineu
*========================================================================================

	*--------------------------------------------------------------------------------------
	* Check version
	*--------------------------------------------------------------------------------------
	If Version(4) < "09.00"
		MessageBox("GuineuIDE requires Microsoft Visual FoxPro 9.0")
		Return 
	ENDIF

	*--------------------------------------------------------------------------------------
	* Run IDE menu
	*--------------------------------------------------------------------------------------
	Local loGuineu
	loGuineu = CreateObject("GuineuIDE")
	loGuineu.DefineMenu()


*========================================================================================
* 
*========================================================================================
Define Class GuineuIDE as Custom

	cPath = ""
	
*========================================================================================
* Determine the path of the application, because we need it hardcoded in various menu
* options.
*========================================================================================
Procedure Init
	This.cPath = Sys(16,Program(-1)-1)
	_Screen.AddProperty("__Guineu_Path",This.cPath)
EndProc


*========================================================================================
* Creates the Guineu menu.
*========================================================================================
Procedure DefineMenu

	*--------------------------------------------------------------------------------------
	* Pad definition
	*--------------------------------------------------------------------------------------
	Local lcScript
	lcScript = [ExecScript("Do UpdateMenu in (_Screen.__Guineu_Path) with ]+;
		[_Screen.__Guineu_Path"+Chr(13)+Chr(10)+"Return .F.")]
	DEFINE PAD padGuineu OF _MSYSMENU PROMPT "Guineu" After _mprog 	KEY ALT+G, "" ;
		Skip for &lcScript
	ON PAD padGuineu OF _MSYSMENU ACTIVATE POPUP popGuineu 

	*--------------------------------------------------------------------------------------
	* Popup definiton
	*--------------------------------------------------------------------------------------
	DEFINE POPUP popGuineu MARGIN RELATIVE SHADOW COLOR SCHEME 4
	DEFINE BAR 2 OF popGuineu PROMPT "\<Build" 
	DEFINE BAR 3 OF popGuineu PROMPT "\<Clean up" ;
		skip for Type("_VFP.ActiveProject") != "O"
	Define Bar 30 of popGuineu Prompt "\-"
	Define Bar 40 of popGuineu Prompt "\<About"

	*--------------------------------------------------------------------------------------
	* Popup actions
	*--------------------------------------------------------------------------------------
	ON BAR 2 OF popGuineu ACTIVATE POPUP popGuineuBuild
	ON SELECTION BAR 3 OF popGuineu DO CleanUpProject in (_Screen.__Guineu_Path)
	This.DefineBuildMenu()
	
EndProc 


*========================================================================================
* Create the build menu
*========================================================================================
Procedure DefineBuildMenu

	*--------------------------------------------------------------------------------------
	* Popup definition
	*--------------------------------------------------------------------------------------
	DEFINE POPUP popGuineuBuild MARGIN RELATIVE SHADOW COLOR SCHEME 4 
	DEFINE BAR 1 OF popGuineuBuild PROMPT "Windows \<Mobile Application" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 2 OF popGuineuBuild PROMPT "Managed \<Application (EXE)" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 3 OF popGuineuBuild PROMPT "\<Console Application" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 4 OF popGuineuBuild PROMPT "ASP.\<NET Application" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 30 OF popGuineuBuild PROMPT "\-"
	DEFINE BAR 40 OF popGuineuBuild PROMPT "Active\<X Control" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 41 OF popGuineuBuild PROMPT "Managed \<Library" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 50 OF popGuineuBuild PROMPT "\-"
	DEFINE BAR 51 OF popGuineuBuild PROMPT "SQL 2005 Stored \<Procedure" ;
		skip for Type("_VFP.ActiveProject") != "O"
	DEFINE BAR 52 OF popGuineuBuild PROMPT "\<Windows Service" skip for .T.

	*--------------------------------------------------------------------------------------
	* Popup actions
	*--------------------------------------------------------------------------------------
	ON SELECTION BAR 1 OF popGuineuBuild DO BuildCompact in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 2 OF popGuineuBuild DO BuildWinform in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 3 OF popGuineuBuild DO BuildConsole in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 4 OF popGuineuBuild DO BuildWebGUI in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 40 OF popGuineuBuild DO BuildActiveX in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 41 OF popGuineuBuild DO BuildLib in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path
	ON SELECTION BAR 51 OF popGuineuBuild DO BuildSPT in (_Screen.__Guineu_Path) with _Screen.__Guineu_Path

EndProc 

EndDefine 


*========================================================================================
* Checks for certain conditions and updates the menu accordingly.
*========================================================================================
Procedure UpdateMenu(tcPath)

	*--------------------------------------------------------------------------------------
	* If this code is execute during initialization we cancel out
	*--------------------------------------------------------------------------------------
	If not Popup("popGuineu")
		Return
	EndIf 
	rlSkip = .F.
	
	*--------------------------------------------------------------------------------------
	* If there's an active project, we embed the name of the Project file in some menu
	* options.
	*--------------------------------------------------------------------------------------
	If Type("_Vfp.ActiveProject") == "O"
		DEFINE BAR 2 OF popGuineu PROMPT "\<Build "+JustFName(_vfp.ActiveProject.Name)
		DEFINE BAR 3 OF popGuineu PROMPT "\<Clean up "+JustFName(_vfp.ActiveProject.Name)
	Else 
		DEFINE BAR 2 OF popGuineu PROMPT "\<Build" 
	EndIf 

	*--------------------------------------------------------------------------------------
	* After the user built a project, we put the last option into the menu as a shortcut.
	*--------------------------------------------------------------------------------------
	Local lcTarget, lcCmd
	lcTarget = ""
	If     PemStatus(_Screen,"__guineuide_lastbuild",5) ;
	   and not Empty(_Screen.__guineuide_lastbuild)
		Do case
		Case _Screen.__guineuide_lastbuild == "compact"
			lcTarget = "Windows Mobile Application"
			lcCmd = "BuildCompact"
		Case _Screen.__guineuide_lastbuild == "winform"
			lcTarget = "Managed \<Application (EXE)"
			lcCmd = "BuildWinform"
		Case _Screen.__guineuide_lastbuild == "console"
			lcTarget = "Console Application"
			lcCmd = "BuildConsole"
		Case _Screen.__guineuide_lastbuild == "webgui"
			lcTarget = "ASP.NET Application"
			lcCmd = "BuildWebGUI"
		Case _Screen.__guineuide_lastbuild == "activex"
			lcTarget = "ActiveX Control"
			lcCmd = "BuildActiveX"
		Case _Screen.__guineuide_lastbuild == "lib"
			lcTarget = "Managed Library"
			lcCmd = "BuildLib"
		Case _Screen.__guineuide_lastbuild == "spt"
			lcTarget = "SQL 2005 Stored Procedure"
			lcCmd = "BuildSPT"
		EndCase 
	EndIf 

	*--------------------------------------------------------------------------------------
	* Define the menu action
	*--------------------------------------------------------------------------------------
	If Empty(m.lcTarget)
		Release Bar 1 of popGuineu
	Else
		Define Bar 1 of popGuineu Prompt "Build " + m.lcTarget before 2
		ON SELECTION BAR 1 OF popGuineu do &lcCmd in "&tcPath" with "&tcPath"
	EndIf 				

EndProc


*========================================================================================
* Creates a windows mobile application
*========================================================================================
Procedure BuildCompact( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the compact EXE
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "compact", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","compact")
	EndIf
	
	*--------------------------------------------------------------------------------------
	* If a device emulator stored state file exists that has the same name as the project,
	* run the device emulator.
	*
	* (...) Make this configurable.
	*--------------------------------------------------------------------------------------
	Local lcDESS
	If Vartype(m.loProject) == "O"
		lcDESS = ForceExt( m.loProject.Name, "DESS" )
		If File(m.lcDESS)
			ShellEx( "open", m.lcDESS )
		EndIf 
	EndIf 

EndProc


*========================================================================================
* Create a WinForm application
*========================================================================================
Procedure BuildWinform( tcPath )

	*------------------------------------------------------------------
	* (PS)
	*------------------------------------------------------------------
	Local  llOk 
	m.llOk = .T. 
	
	If m.llOk 
		m.llOk = Type("_VFP.Activeproject") == "O"
	EndIf 
	
 	If m.llOk 
 		Local loParameter 
 		m.loParameter = getBuildParameter( "Winform" ) 
 		m.llOk = Not IsNull( m.loParameter ) 
 	EndIf 
	
	*--------------------------------------------------------------------------------------
	* Build the compact EXE
	*--------------------------------------------------------------------------------------
	If m.llOk 
		Local loProject
		loProject = _VFP.Activeproject
		*------------------------------------------------------------------
		* (..) Das ist ganz seltsam und führt zu einem Fehler, später 
		* (..) in der weiteren Verarbeitung 
		*------------------------------------------------------------------
		m.llOK = Type( "m.loProject.MainFile" ) = "C" ;
				AND Not IsNull( m.loProject.MainFile )
	EndIf 
	
	If m.llOK 	
		Do __guineuide_Build with m.loProject, "winform", JustPath(m.tcPath), m.loParameter 
		_Screen.AddProperty("__guineuide_lastbuild","winform")
	EndIf

	*------------------------------------------------------------------
	* Erase intermediate files 
	*------------------------------------------------------------------
	If m.llOK And m.loParameter.lEraseIFile	
		EraseIFiles( m.loParameter ) 
	EndIf 

	*--------------------------------------------------------------------------------------
	* Open the folder with the EXE
	*
	* (...) Make this configurable in a small dialog
	*--------------------------------------------------------------------------------------
	If m.llOk And m.loParameter.lShowFolder
		ShellEx( "open", m.loParameter.GetDDir())
	EndIf
	
EndProc


*========================================================================================
* Create a Console application
*========================================================================================
Procedure BuildConsole( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the console EXE
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "console", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","console")
	EndIf

	*--------------------------------------------------------------------------------------
	* Open CMD.EXE in the correct directory
	*
	* (...) Make this configurable in a small dialog
	*--------------------------------------------------------------------------------------
	If Vartype(m.loProject) == "O"
		__guineuide_execute( "CMD.EXE", _VFP.Activeproject.HomeDir, .T. )
	EndIf
	
EndProc


*========================================================================================
* Create a Console application
*========================================================================================
Procedure BuildWebGUI( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the console EXE
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "webgui", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","webgui")
	EndIf

	
EndProc


*========================================================================================
* Creates an ActiveX control
*========================================================================================
Procedure BuildActiveX( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the ActiveX DLL
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "activex", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","activex")
	EndIf
	
EndProc


*========================================================================================
* Creates a Lib
*========================================================================================
Procedure BuildLib( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the Lib DLL
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "lib", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","lib")
	EndIf
	
EndProc


*========================================================================================
* Create a SPT application
*========================================================================================
Procedure BuildSPT( tcPath )

	*--------------------------------------------------------------------------------------
	* Build the SPT DLL
	*--------------------------------------------------------------------------------------
	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Build with m.loProject, "spt", JustPath(m.tcPath)
		_Screen.AddProperty("__guineuide_lastbuild","spt")
	EndIf

	*--------------------------------------------------------------------------------------
	* Open the SQL script
	*
	* (...) Make this configurable in a small dialog
	*--------------------------------------------------------------------------------------
	If Vartype(m.loProject) == "O"
		ShellEx("open",ForceExt(m.loProject.Name,"SQL"))
	EndIf
	
EndProc


*========================================================================================
* Removes all temporary Guineu files from the project directory
*========================================================================================
Procedure CleanUpProject

	Local loProject
	If Type("_VFP.Activeproject") == "O"
		loProject = _VFP.Activeproject
		Do __guineuide_Cleanup with m.loProject
	EndIf

EndProc

	

*========================================================================================
* 
*========================================================================================
Procedure ShellEx( Befehl, Programm )

		DECLARE INTEGER ShellExecute IN SHELL32.DLL ;
		INTEGER nWinHandle,;
		STRING cOperation,;   
		STRING cFileName,;
		STRING cParameters,;
		STRING cDirectory,;
		INTEGER nShowWindow
		
RETURN ShellExecute(0,m.Befehl,m.Programm,"",NULL,3)


*====================================================================
* (PS) Starten eines Dialogs, um die Einstellungen abzufragen 
*====================================================================
Function getBuildParameter (tcType) 
Local lcType 
m.lcType = Upper(Alltrim(Evl(m.tcType,"")))

Local loParameter, loDialog 
Do Case 
Case m.lcType == "WINFORM"
	m.loParameter = CreateObject( "ParameterWinform" ) 
	m.loDialog = NewObject( "__guineu_frmbuildwinform","__guineuide_dialog",.NULL.,m.loParameter ) 
Otherwise 
	m.loParameter = CreateObject( "ParameterBase" ) 
	m.loDialog = NewObject( "__guineu_frmbuildbase","__guineuide_dialog",.NULL.,m.loParameter ) 
EndCase 
		

If Vartype( m.loDialog ) = "O" 
	m.loDialog.Show(1) 
	*------------------------------------------------------------------
	* (PS) Rückgabe auswerten 
	*------------------------------------------------------------------
	If m.loDialog.nExitState <> 1 
		m.loParameter = .NULL. 
	Else 
		If Not m.loParameter.Validate() 
			MessageBox( m.loParameter.cLastError,0+16,"Error" ) 
			m.loParameter = .NULL.
		EndIf 			
	EndIf 	
Else 
	m.loParameter = .Null.
EndIf 

Return m.loParameter

*====================================================================
* (PS) Parameter(Grund)Klasse 
*====================================================================
Define Class ParameterBase AS Custom
	lShowCError		= .F. 
	lEraseIFile		= .F.
	cEraseIFile 	= ""
	cBuildCMDOutput	= ""
	lDDir			= .F.
	cDDir			= ""
	cPath 			= Sys(16,Program(-1)-1)
	cLastError		= "" 
	cCore			= "guineu" + Sys(2015) 
	
	*====================================================================
	* (PS)
	*====================================================================
	Function Init() 
	This.SetDefault() 
	Return 
	
	*====================================================================
	* (PS)
	*====================================================================
	Function SetDefault
	Local lcPath 
	If Type("_VFP.Activeproject") == "O"
		This.cPath 	= Addbs(JustPath(_vfp.ActiveProject.Name)) 
		This.cCore 	= JustStem(_vfp.ActiveProject.Name)
	EndIf 

	This.lShowCError		= .F. 
	This.lEraseIFile		= .T.
	This.cEraseIFile 		= "*.CS,*.RES" 
	This.cBuildCMDOutput	= This.cPath + "projekt.cmd" 
	This.lDDir				= .F.
	This.cDDir				= This.cPath + "bin"
	Return 
	
	*====================================================================
	* (PS)
	*====================================================================
	Function Validate
	LParameters tcParameter 
	Local lcParameter 
	m.lcParameter =  Upper(Alltrim( Evl(m.tcParameter,"") ))
	
	Local llValidateAll 
	m.llValidateAll =  Empty( m.lcParameter ) 
	
	Local llOK 
	llOk = .T. 
		
	*------------------------------------------------------------------
	* Parameter.cDDir 
	*------------------------------------------------------------------
	If m.llOK 
		If m.llValidateAll OR m.lcParameter = "CDDIR" 
			m.llOK = This.Validate_cDDir() 
		EndIf 
	EndIf 	
	
	*------------------------------------------------------------------
	* Parameter.cCore 
	*------------------------------------------------------------------
	If m.llOK 
		If m.llValidateAll OR m.lcParameter = "CCORE" 
			This.cCore = Alltrim(This.cCore) 
			If Empty( This.cCore ) 
				If Type("_VFP.Activeproject") == "O"
					This.cCore 	= JustStem(_vfp.ActiveProject.Name)
				Else 
					This.cCore 	= "guineu" + Sys(2015) 
				EndIf 
			EndIf 
		EndIf 
	EndIf 
	
	Return m.llOK 	

	*====================================================================
	* (PS)
	*====================================================================
	Function Validate_cddir
	Local llOK 
	m.llOK = .T. 
	If This.lDDir 
		*------------------------------------------------------------------
		* (PS) Innerhalb des Projektverzeichnisses immer das Unterverzeichnis 
		* (PS) "bin"
		*------------------------------------------------------------------
		If Upper(Alltrim(Addbs(this.cPath))) == Upper(Alltrim(Addbs(this.cDDir))) 
			This.cDDir = Addbs(This.cDDir) + "bin"
		EndIf 
		*------------------------------------------------------------------
		* (PS) Verzeichnis bei Bedarf erstellen. 
		*------------------------------------------------------------------
		If not Directory( This.cDDir ) 
			Try 
				Md (This.cDDir) 
			Catch 
				m.llOK = .F. 
				This.cLastError = "Can't create " + This.cDDir
			EndTry 
		EndIf 
	Else 
		*------------------------------------------------------------------
		* (PS) 
		*------------------------------------------------------------------
		This.cDDir = This.cPath 
	EndIf 	

	*------------------------------------------------------------------
	* (PS) Am Ende kein BS 
	*------------------------------------------------------------------
	This.cDDir = Addbs(This.cDDir) 
	This.cDDir = Left(This.cDDir,Len(This.cDDir)-1)
	
	*------------------------------------------------------------------
	* (PS) ermitteln des relativen Pfades 
	*------------------------------------------------------------------
	This.cDDir = Sys(2014,This.cDDir, This.cPath ) 
	
	Return m.llOK 
	*====================================================================
	* (PS)
	*====================================================================
	Function GetDDIr 
	Local lcReturn 
	Do Case 
	Case At(":",This.cDDir) > 0 
		m.lcReturn = This.cDDir 
		
	Case At("\\",This.cDDir) > 0 
		m.lcReturn = This.cDDir  
		
	Otherwise 
		m.lcReturn = FullPath(Addbs(This.cPath) + This.cDDir) 
	EndCase 
	
	Return m.lcReturn 
EndDefine 

*====================================================================
* (PS) Parameter(Winform)Klasse 
*====================================================================
Define Class ParameterWinform AS ParameterBase
	lShowFolder				= .F.
	*====================================================================
	* 
	*====================================================================
	Function SetDefault() 
	This.lShowFolder 		= .T. 
	Return DoDefault()  
EndDefine 

*====================================================================
* (PS) Aufräumen nach einem Build 
*====================================================================
Procedure EraseIFiles( toParameter ) 
Local lcFile, lcFiles, laFiles[1], lcERRMess
m.lcFiles = Chrtran( toParameter.cEraseIFile, ";",",") 
m.lcFile = "" 
m.lcERRMess = "" 

Local lcDDir, lcDeleteFile 
m.lcDDir = m.toParameter.GetDDir() 

If Directory( m.lcDDir ) 
	ALines( m.laFiles,m.lcFiles,1+2,",") 
	For Each lcFile IN laFiles 
		m.lcDeleteFile = Addbs( m.lcDDir ) + Alltrim(m.lcFile) 
		Try 
			Delete File (m.lcDeleteFile)
		Catch To loError 
			m.lcERRMess = m.lcERRMess + Chr(13) + Chr(10) + m.loError.Message 
		EndTry 
	EndFor 
EndIf 

If Not Empty( m.lcERRMess ) 
	MessageBox( m.lcERRMess, 16 , "Error") 
EndIf 

Return 

