*========================================================================================
* Führt eine Anwendung aus und liefert den Errorlevel zurück.
*========================================================================================
LParameter tcBefehlszeile, tcDir, tlNowait

	*--------------------------------------------------------------------------------------
	* API Deklarationen
	*--------------------------------------------------------------------------------------
	DECLARE INTEGER CreateProcess IN kernel32.DLL ;
		String lpApplicationName, ;
		STRING lpCommandLine, ;
		INTEGER lpProcessAttributes, ;
		INTEGER lpThreadAttributes, ;
		INTEGER bInheritHandles, ;
		INTEGER dwCreationFlags, ;
		String lpEnvironment, ;
		String lpCurrentDirectory, ;
		STRING @lpStartupInfo, ;
		STRING @lpProcessInformation
	Declare long GetLastError in Win32API

	*--------------------------------------------------------------------------------------
	* Die Anwendung starten
	*--------------------------------------------------------------------------------------
	Local lcStartupInfo, lcProcessInfo, lnOK
	lcStartupInfo = ;
		BINTOC(68,"RS") + ;
		Replicate(Chr(0),40) + ;
		BinToC(1,"RS") + ;
		BinToC(Iif(m.tlNowait,1,0),"2RS") + ; && SW_HIDE/SW_SHOW
		BinToC(0,"2RS") + ;
		Replicate(Chr(0),16)
	lcProcessInfo = Replicate(Chr(0),16)
	lnOK = CreateProcess( ;
		NULL, ;
		m.tcBefehlszeile, ;
		0, ;
		0, ;
		1, ;
		0x20, ;
		NULL, ;
		m.tcDir, ;
		@lcStartupInfo, ;
		@lcProcessInfo ;
	)

	*--------------------------------------------------------------------------------------
	* Die PROCESSINFO Werte übernehmen
	*--------------------------------------------------------------------------------------
	Local lnProcessHandle, lnThreadHandle
	If m.lnOK == 0
		Return -1
	Else
		lnProcessHandle = CTOBIN(Substr(m.lcProcessInfo,1,4),"RS")
		lnThreadHandle = CTOBIN(Substr(m.lcProcessInfo,5,4),"RS")
	EndIf 

	*--------------------------------------------------------------------------------------
	* Warten bis der Prozess zu Ende ist.
	*--------------------------------------------------------------------------------------
	Declare Long WaitForSingleObject in Win32API Long, Long
	If not m.tlNowait
		WaitForSingleObject( m.lnProcessHandle, -1 )
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Den Rückgabewert ermitteln
	*--------------------------------------------------------------------------------------
	Local lnExitCode
	lnExitCode = -1
	Declare Long GetExitCodeProcess in Win32API ;
	  Long hProcess, ;
  	Long @lpExitCode
	GetExitCodeProcess( m.lnProcessHandle, @lnExitCode )

	*--------------------------------------------------------------------------------------
	* Handle schließen
	*--------------------------------------------------------------------------------------
	Declare CloseHandle in Win32API Long
	CloseHandle( m.lnProcessHandle )
	CloseHandle( m.lnThreadHandle )

Return m.lnExitCode
