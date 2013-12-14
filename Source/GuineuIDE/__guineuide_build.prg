*========================================================================================
* Generates C# files for the specified project
*========================================================================================
LParameter toProject as Project, tcMode, tcRoot, toParameter 

	*--------------------------------------------------------------------------------------
	* Settings
	*--------------------------------------------------------------------------------------
	Local lcSafety, lcDeleted
	lcSafety = Set("Safety")
	lcDeleted = Set("Deleted")
	Set Safety off
	Set Deleted on

	*--------------------------------------------------------------------------------------
	* Get names. The C# files share the name of the project file
	*--------------------------------------------------------------------------------------
	Local lcCore, lcCSFile, lcResFile, lcExe, lcRoot, lcDLL
	
	
	If Type( "m.toParameter" ) = "O" And Not IsNull( m.toParameter ) 
		m.lcRoot = m.toParameter.GetDDir() 
		lcCore = m.toParameter.cCore
	Else 
		lcRoot = JustPath(toProject.Name)
		lcCore = JustStem(toProject.Name)
	EndIF
	lcCSFile = Addbs(m.lcRoot) + m.lcCore + ".cs" 
	lcResFile = Addbs(m.lcRoot) + m.lcCore + ".res" 
	If m.tcMode = "webgui"
		lcEXE = Addbs(m.lcRoot) + "bin\" + m.lcCore + ".dll" 
	Else 
		lcEXE = Addbs(m.lcRoot) + m.lcCore + ".exe" 
	EndIf
	lcDLL = Addbs(m.tcRoot)
	
*!*		*--------------------------------------------------------------------------------------
*!*		* Get names. The C# files share the name of the project file
*!*		*--------------------------------------------------------------------------------------
*!*		Local lcCore, lcCSFile, lcResFile, lcExe, lcRoot, lcDLL
*!*		lcCore = JustStem(toProject.Name)
*!*		lcCSFile = ForceExt(m.toProject.Name,"cs")ener
*!*		lcResFile = ForceExt(m.toProject.Name,"res")
*!*		lcEXE = ForceExt(m.toProject.Name,"exe")
*!*		lcRoot = JustPath(toProject.Name)
*!*		lcDLL = Addbs(m.tcRoot)

	
	*--------------------------------------------------------------------------------------
	* Get the main program. Visual FoxPro ensures that the file is embedded.
	*--------------------------------------------------------------------------------------
	Local lcMain, loMain
	loMain = m.toProject.Files(m.toProject.MainFile)
	Do case
	Case m.loMain.Type = "K"
		lcMain = forceext(JustFname(m.toProject.MainFile),".scx.fxp")
	Case m.loMain.Type = "P"
		lcMain = forceext(JustFname(m.toProject.MainFile),"fxp")
	Otherwise
		lcMain = forceext(JustFname(m.toProject.MainFile),"fxp")
	EndCase
	
	*--------------------------------------------------------------------------------------
	* Branch according to the mode
	*--------------------------------------------------------------------------------------
	Local lcOption, loClasses
	Do case
	Case m.tcMode = "winform"
		GenerateWinFormCS( m.lcCSFile, m.lcCore, m.lcMain )
		GenerateWinFormRes( m.lcResFile, m.lcCore, m.lcExe, m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesDesktop( m.lcDLL, m.lcRoot )
		lcOption = ""
	Case m.tcMode = "console"
		GenerateConsoleCS( m.lcCSFile, m.lcCore, m.lcMain )
		GenerateConsoleRes( m.lcResFile, m.lcCore, m.lcExe, m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesDesktop( m.lcDLL, m.lcRoot )
		lcOption = ""
	Case m.tcMode = "compact"
		GenerateCompactCS( m.lcCSFile, m.lcCore, m.lcMain )
		GenerateCompactRes( m.lcResFile, m.lcCore, m.lcExe, m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesCompact( m.lcDLL, m.lcRoot )
		lcOption = "/noconfig"
	Case m.tcMode = "activex"
		loClasses = GetClassCollection( m.toProject.Files, m.toProject.Name, m.lcCore )
		GenerateActiveXCS( m.lcCSFile, m.lcCore, m.loClasses )
		GenerateActiveXRes( m.lcResFile, m.lcCore, ForceExt(m.lcExe,"DLL"), m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesDesktop( m.lcDLL, m.lcRoot )
		lcOption = ""	
	Case m.tcMode = "lib"
		loClasses = GetClassCollection( m.toProject.Files, m.toProject.Name, m.lcCore )
		GenerateLibCS( m.lcCSFile, m.lcCore, m.loClasses )
		GenerateLibRes( m.lcResFile, m.lcCore, ForceExt(m.lcExe,"DLL"), m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesDesktop( m.lcDLL, m.lcRoot )
		lcOption = ""	
	Case m.tcMode = "spt"
		GenerateSPTCS( m.lcCSFile, m.lcCore, m.loMain.Name, m.lcMain )
		GenerateSPTRes( m.lcResFile, m.lcCore, ForceExt(m.lcExe,"DLL"), m.lcRoot, m.toProject.Files )
		GenerateSPTScript(ForceExt(m.lcExe,"SQL"),m.lcCore,ForceExt(m.lcExe,"DLL"), m.loMain.Name)
		CopyRuntimeFilesSPT( m.lcDLL, m.lcRoot )
		lcOption = ""	
	Case m.tcMode = "webgui"
		If not Directory(Addbs(m.lcRoot)+"bin",1)
			Md (Addbs(m.lcRoot)+"bin")
		EndIf
		GenerateWebGUICS( m.lcCSFile, m.lcCore, m.lcMain )
		GenerateWebGUIConfig( m.lcRoot, m.lcCore )
		GenerateWebGUIRes( m.lcResFile, m.lcCore, m.lcExe, m.lcRoot, m.toProject.Files )
		CopyRuntimeFilesWebGUI( m.lcDLL, Addbs(m.lcRoot)+"bin" )
		lcOption = ""
	EndCase
	
	*--------------------------------------------------------------------------------------
	* Does the project have an icon file?
	*--------------------------------------------------------------------------------------
	If not Empty(m.toProject.Icon)
		lcOption = m.lcOption + [ /win32icon:"]+m.toProject.Icon+["]
	EndIf 
	
	*--------------------------------------------------------------------------------------
	* Execute the C# compiler to generate the EXE or DLL
	*--------------------------------------------------------------------------------------
	CompileCSC( m.lcResFile, m.lcOption )
	
	*--------------------------------------------------------------------------------------
	* Register the control if necessary
	*--------------------------------------------------------------------------------------
	If m.tcMode == "activex"
		Register( ForceExt(m.lcExe,"DLL") )
		ChangeToHKCU( ForceExt(m.lcExe,"REG"), m.loClasses )
		regedit(ForceExt(m.lcExe,"REG") )
		GenerateHTML( ForceExt(m.lcExe,"DLL"), ForceExt(m.lcExe,"HTM"), m.loClasses)
	EndIf 
	
	*--------------------------------------------------------------------------------------
	* 
	*--------------------------------------------------------------------------------------
	If m.tcMode = "webgui"
		GenerateWebGUIHTML( ForceExt(m.lcExe,"HTM") )
	EndIf	
	
	*--------------------------------------------------------------------------------------
	* Restore environment
	*--------------------------------------------------------------------------------------
	If m.lcSafety == "ON"
		Set Safety on
	EndIf
	If m.lcDeleted == "OFF"
		Set Deleted off
	EndIf
	
Return


*========================================================================================
* 
*========================================================================================
Procedure Register( tcDLL)

	Local lcPath, lcCmd
	lcPath = Addbs(GetEnv("windir"))+"Microsoft.NET\Framework\v2.0.50727\"
	lcCmd = m.lcPath+[REGASM.EXE "] + m.tcDLL+ [" /regfile /silent /codebase]
	_cliptext = m.lcCmd
	__guineuide_execute( m.lcCmd, JustPath(m.tcDLL) )

EndProc

*========================================================
* Replaces HKEY_CLASSES_ROOT with HKEY_CURRENT_USER\
* Software\Classes in all .reg files.
*========================================================
Procedure ChangeToHKCU(tcFile, toClasses)

	Local lcFile
	lcFile = FileToStr(m.tcFile)
	lcFile = Strtran( ;
		m.lcFile, ;
		"HKEY_CLASSES_ROOT", ;
		"HKEY_CURRENT_USER\Software\Classes" ;
	)
	
	Local loClass
	For each loClass in toClasses FOXOBJECT 
		lcFile = m.lcFile + Chr(13)+Chr(10) + Chr(13)+Chr(10) + ;
			"[HKEY_CURRENT_USER\Software\Classes\CLSID\{"+loClass.Guid+"}\Control]"
	EndFor

	StrToFile( m.lcFile, m.tcFile )

EndProc

*========================================================================================
* 
*========================================================================================
Procedure RegEdit( tcReg)

	Local lcCmd
	lcCmd =[REGEDIT.EXE /s "] + m.tcReg+ ["]
	_cliptext = m.lcCmd
	__guineuide_execute( m.lcCmd, JustPath(m.tcReg) )

EndProc


*========================================================================================
* Generate and run an HTML file
*========================================================================================
Procedure GenerateHTML( tcCodebase, tcFile, toClasses )

	Local lcText, loClass
	lcText = "<html><body>"
	For each loClass in m.toClasses FOXOBJECT 
		Text to m.lcText noshow TEXTMERGE  ADDITIVE 
			<div style="Height: 200px;">
			<p>ActiveX control <<loClass.ProgID>>:</p>
			<OBJECT CLASSID="clsid:<<m.loClass.Guid>>" CODEBASE="<<m.tcCodebase>>" >
			</div>
		EndText
	EndFor 
	lcText = m.lcText + "</body></html>"

	StrToFile(m.lcText,m.tcFile)
	ShellEx( "open", m.tcFile)

EndProc



*========================================================================================
* Executes the C# compiler
*========================================================================================
Procedure CompileCSC( tcResponseFile, tcOption )

	Local lcPath, lcCmd
	lcPath = Addbs(GetEnv("windir"))+"Microsoft.NET\Framework\v2.0.50727\"
	lcCmd = m.lcPath+[CSC.EXE ]+m.tcOption+[ @"] + m.tcResponseFile + ["]
	_cliptext = m.lcCmd
	__guineuide_execute( m.lcCmd, JustPath(m.tcResponseFile) )

EndProc


*========================================================================================
* Copies the Guineu runtime libraries
*========================================================================================
Procedure CopyRuntimeFilesDesktop( tcSource, tcDest )

	If Upper(Alltrim(Addbs(m.tcSource))) != Upper(Alltrim(Addbs(m.tcDest)))
		Copy File (m.tcSource+"Guineu.Runtime.Desktop.dll") to (m.tcDest)
	EndIf

EndProc

*========================================================================================
* Copies the Guineu runtime libraries
*========================================================================================
Procedure CopyRuntimeFilesWebGUI( tcSource, tcDest )

	If Upper(Alltrim(Addbs(m.tcSource))) != Upper(Alltrim(Addbs(m.tcDest)))
		Copy File (m.tcSource+"Guineu.Runtime.Desktop.dll") to (m.tcDest)
	EndIf
	
	Local laFile[1], lnFile, lcSDK
	lcSDK = Addbs(GetEnv("ProgramFiles"))+"Gizmox\Visual WebGui\SDK20"
	For lnFile = 1 to ADir(laFile,m.lcSDK+"\*.*")
		Copy File ;
			(Addbs(GetEnv("ProgramFiles"))+"Gizmox\Visual WebGui\SDK20\"+laFile[m.lnFile,1]) ;
			to (m.tcDest)
	EndFor 

EndProc

*========================================================================================
* Copies the Guineu runtime libraries
*========================================================================================
Procedure CopyRuntimeFilesSPT( tcSource, tcDest )

	If Upper(Alltrim(Addbs(m.tcSource))) != Upper(Alltrim(Addbs(m.tcDest)))
		Copy File (m.tcSource+"Guineu.sql.dll") to (m.tcDest)
	EndIf

EndProc


*========================================================================================
* Copies the Guineu runtime libraries for the Windows Mobile version
*========================================================================================
Procedure CopyRuntimeFilesCompact( tcSource, tcDest )

	If Upper(Alltrim(Addbs(m.tcSource))) != Upper(Alltrim(Addbs(m.tcDest)))
		Copy File (m.tcSource+"Guineu.Runtime.Mobile.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.Phone.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.Windows.Forms.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.Net.Mail.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.WindowsCE.dll") to (m.tcDest)
		Copy File (m.tcSource+"OpenNETCF.WindowsCE.Messaging.dll") to (m.tcDest)
	EndIf

EndProc


*========================================================================================
* Generate the CS file for a WinForm application
*========================================================================================
Procedure GenerateWinFormCS( tcCSFile, tcCore, tcMain )

	Local lcCode
	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2
		using System;
		using Guineu;
		using System.Windows.Forms;
		 
		namespace <<m.tcCore>>
		{
			class main
			{
				static void Main(string[] args)
				{
					Application.EnableVisualStyles();
					GuineuInstance.InitInstance();
					GuineuInstance.Do("<<m.tcMain>>", args);
				}
			}
		}	
	EndText 
	StrToFile( m.lcCode, m.tcCSFile, 0 )
	
EndProc


*========================================================================================
* Generate the Response file for a WinForm application
*========================================================================================
Procedure GenerateWinFormRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:winexe
		/reference:Guineu.Runtime.Desktop.dll
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc


*========================================================================================
* Generate the CS file for a Windows Mobile application
*========================================================================================
Procedure GenerateCompactCS( tcCSFile, tcCore, tcMain )

	Local lcCode
	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2
		using System;
		using Guineu;
		using System.Windows.Forms;
		using System.Reflection;
		using System.IO;
 
		namespace <<m.tcCore>>
		{
			class main
			{
				static void Main(string[] args)
				{
					GuineuInstance.InitInstance();
					if (!GuineuInstance.FileMgr.SupportCurrentDirectory)
					{
						GuineuInstance.FileMgr.CurrentDirectory = Path.GetDirectoryName(
							Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName
						);
					}
					GuineuInstance.Do("<<m.tcMain>>", args);
				}
			}
		}	
	EndText 
	StrToFile( m.lcCode, m.tcCSFile, 0 )
	
EndProc


*========================================================================================
* Generate the Response file for a Windows Mobile application
*========================================================================================
Procedure GenerateCompactRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes, lcSDK
	lcSDK = GetCompactSDK()
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:exe
		/reference:Guineu.Runtime.Mobile.dll
		/nostdlib
		/out:"<<m.tcExe>>"
		/r:"<<m.lcSDK>>\System.dll"
		/r:"<<m.lcSDK>>\mscorlib.dll"
		/r:"<<m.lcSDK>>\System.Windows.Forms.dll"
		/r:"<<m.lcSDK>>\System.drawing.dll"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc


*========================================================================================
* Returns the path to the Windows Mobile SDK. Right now we check multiple known locations
*========================================================================================
Procedure GetCompactSDK

	*--------------------------------------------------------------------------------------
	* Standard installation path for the SDK
	*--------------------------------------------------------------------------------------
	Local lcSDK
	lcSDK = Addbs(GetEnv("ProgramFiles"))+"Microsoft.NET\SDK\CompactFramework\v2.0\WindowsCE"
	If File(m.lcSDK+"\mscorlib.dll")
		Return m.lcSDK
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Visual Studio, multiple variations
	*--------------------------------------------------------------------------------------
	lcSDK = Addbs(GetEnv("ProgramFiles"))+"Microsoft Visual Studio 8\SmartDevices\SDK\CompactFramework\2.0\v1.0\WindowsCE"
	If File(m.lcSDK+"\mscorlib.dll")
		Return m.lcSDK
	EndIf
	lcSDK = Addbs(GetEnv("ProgramFiles"))+"Microsoft Visual Studio 8\SmartDevices\SDK\CompactFramework\2.0\v2.0\WindowsCE"
	If File(m.lcSDK+"\mscorlib.dll")
		Return m.lcSDK
	EndIf
	lcSDK = Addbs(GetEnv("ProgramFiles"))+"Microsoft Visual Studio 9.0\SmartDevices\SDK\CompactFramework\2.0\v2.0\WindowsCE"
	If File(m.lcSDK+"\mscorlib.dll")
		Return m.lcSDK
	EndIf
	
Return m.lcSDK


*========================================================================================
* Generate the CS file for a console application
*========================================================================================
Procedure GenerateConsoleCS( tcCSFile, tcCore, tcMain )

	Local lcCode
	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2
		using System;
		using Guineu;
		 
		namespace <<m.tcCore>>
		{
			class main
			{
				static void Main(string[] args)
				{
					GuineuInstance.InitInstance();
					GuineuInstance.Do("<<m.tcMain>>", args);
				}
			}
		}	
	EndText 
	StrToFile( m.lcCode, m.tcCSFile, 0 )
	
EndProc


*========================================================================================
* Generate the Response file for a Console application
*========================================================================================
Procedure GenerateConsoleRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:exe
		/reference:Guineu.Runtime.Desktop.dll
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc


*========================================================================================
* 
*========================================================================================
Procedure GetResourceList( toFiles, tcRoot )

	Local loFile as File, lcFile, lcRes
	lcRes = ""
	For each loFile in toFiles
		lcFile = GetFileName(m.loFile)
		If not Empty(m.lcFile)
			lcFile = relativepathto(m.tcRoot, m.lcFile)
			If Left(m.lcFile,2) == ".\"
				lcFile = Substr(m.lcFile,3)
			EndIf
			lcRes = m.lcRes + "/resource:" + m.lcFile + Chr(13)+Chr(10)
		EndIf 
	EndFor 

Return m.lcRes


*========================================================================================
* Returns the file name of the file to embed.
*========================================================================================
Procedure GetFileName( toFile as File )

	Local lcFile
	Do case
	
	*--------------------------------------------------------------------------------------
	* Excluded files
	*--------------------------------------------------------------------------------------
	Case m.toFile.Exclude
		lcFile = ""
		
	*--------------------------------------------------------------------------------------
	* Program files
	*--------------------------------------------------------------------------------------
	Case m.toFile.Type == "P"
		If File(m.toFile.Name)
			Clear Program (m.toFile.Name)
			Compile (m.toFile.Name)
		EndIf 
		lcFile = ForceExt( m.toFile.Name, "fxp" )
		
	*--------------------------------------------------------------------------------------
	* Forms are converted into a PRG
	*--------------------------------------------------------------------------------------
	Case m.toFile.Type == "K"
		lcFile = m.toFile.Name+".fxp"
		If HasChanged( m.toFile.Name, m.lcFile )
			Do __guineuide_ConvertSCX with m.toFile.Name
		EndIf 
		
	*--------------------------------------------------------------------------------------
	* VCXes are converted into a PRG
	*--------------------------------------------------------------------------------------
	Case m.toFile.Type == "V"
		Do __guineuide_ConvertVCX with m.toFile.Name
		lcFile = m.toFile.Name+".fxp"
		
	*--------------------------------------------------------------------------------------
	* Other files.
	*--------------------------------------------------------------------------------------
	Case m.toFile.Type == "x"
		lcFile = m.toFile.Name

	*--------------------------------------------------------------------------------------
	* free tables
	*--------------------------------------------------------------------------------------
	Case m.toFile.Type == "D"
		lcFile = m.toFile.Name

	*--------------------------------------------------------------------------------------
	* In all other cases exclude file
	*--------------------------------------------------------------------------------------
	Otherwise 
		lcFile = ""
	EndCase 
	
	*--------------------------------------------------------------------------------------
	* Check if the file exists
	*--------------------------------------------------------------------------------------
	If not File(m.lcFile)
		lcFile = ""
		* (...) Error message
	EndIf 

Return m.lcFile


*========================================================================================
* Returns the relative path
*========================================================================================
PROCEDURE relativepathto
LParameter tcFrom, tcTo

	*--------------------------------------------------------------------------------------
	* API Deklaration
	*--------------------------------------------------------------------------------------
	Declare LONG PathRelativePathTo in shlwapi.Dll ;
		String @pszPath, ;
		String pszFrom, ;
			Long dwAttrFrom, ;
		String pszTo, ;
		Long dwAttrTo
	#DEFINE FILE_ATTRIBUTE_DIRECTORY 0x00000010  
			
	*--------------------------------------------------------------------------------------
	* Der relative Pfad wird durch die shlwapi.DLL ermittelt. Diese Funktion steht ab
	* Version 4.71 zur Verf³gung.
	*--------------------------------------------------------------------------------------
	Local lcPath, lnOK
	lcPath = Space(260)
	lnOK = PathRelativePathTo( ;
		@lcPath, ;
		RemoveBackslash(m.tcFrom), FILE_ATTRIBUTE_DIRECTORY, ;
		RemoveBackslash(m.tcTo), FILE_ATTRIBUTE_DIRECTORY ;
	)
	If m.lnOK == 0
		lcPath = ""
	Else
		If Chr(0) $ m.lcPath
			lcPath = Left( m.lcPath, At(Chr(0),m.lcPath)-1 )
		EndIf
	EndIf

Return m.lcPath


*========================================================================================
* Entfernt von einem Pfad alle am Ende befindlichen Backslash, falls welche vorhanden 
* sein sollte. Erfordert shlwapi.dll Version 4.71 (IE4) oder h÷her.
*========================================================================================
Procedure RemoveBackslash
LParameter tcPath

	*--------------------------------------------------------------------------------------
	* Backslash entfernen. Da PathRemoveBackslash nur den letzten Backslash entfernt 
	* (getestet unter Windows XP SP2), rufen wir die Funktion in einer Schleife auf, bis
	* sich der R³ckgabewert nicht mehr õndert.
	*--------------------------------------------------------------------------------------
	Local lcPath, lcPrevPath
	Declare Long PathRemoveBackslash in shlwapi.dll String @lpszPath
	lcPath = m.tcPath
	Do while .T.
		lcPrevPath = m.lcPath
		lcPath = m.lcPath + Chr(0)
		PathRemoveBackslash( @lcPath )
		lcPath = Left( m.lcPath, At(Chr(0),m.lcPath)-1 )
		If m.lcPath == m.lcPrevPath
			Exit
		EndIf
	EndDo
	
Return m.lcPath


*========================================================================================
* Generates the program required for an ActiveX control
*========================================================================================
Procedure GenerateActiveXCS( tcCSFile, tcCore, toClasses )

	*--------------------------------------------------------------------------------------
	* Template for the header
	*--------------------------------------------------------------------------------------
	Local lcCode
	Text to m.lcCode TEXTMERGE NOSHOW Pretext 2
		using System;
		using System.Collections.Generic;
		using System.ComponentModel;
		using System.Data;
		using System.Drawing;
		using System.Text;
		using System.Windows.Forms;
		using System.Runtime.InteropServices;
		using System.Reflection;
		using Microsoft.Win32;

		namespace <<m.tcCore>>
		{
	EndText 
	
	*--------------------------------------------------------------------------------------
	* For each class generate an interface an a class
	*--------------------------------------------------------------------------------------
	Local loClass
	For each loClass in toClasses FoxObject
		lcCode = m.lcCode + GenerateActiveXClassCS(m.loClass)
	EndFor
	
	*--------------------------------------------------------------------------------------
	* End
	*--------------------------------------------------------------------------------------
	lcCode = m.lcCode + Chr(13)+Chr(10) + "}"

	StrToFile( m.lcCode, m.tcCSFile, 0 )
EndProc 

*========================================================================================
* 
*========================================================================================
Procedure GenerateActiveXClassCS( toClass )

	*--------------------------------------------------------------------------------------
	* Generate a GUID for the class and the interface
	*--------------------------------------------------------------------------------------
	Local lcCode
	lcCode = ""
		Text to m.lcCode ADDITIVE NOSHOW TEXTMERGE PRETEXT 2
			[Guid("<<m.toClass.IID>>")]
			[ComVisible(true)]
			public interface <<m.toClass.CSName>>Interface
			{
			}

			[Guid("<<m.toClass.Guid>>")]
			[ProgId("<<m.toClass.ProgID>>")]
			[ComVisible(true)]
			[ClassInterface(ClassInterfaceType.None)]
			public class <<m.toClass.CSName>>: Guineu.Interop.GuineuHost, <<m.toClass.CSName>>Interface
			{
				public <<m.toClass.CSName>>()
					: base("<<m.toClass.File>>", "<<m.toClass.ClassName>>")
				{
					InitializeComponent();
				}
				#region based on code (c) Morgan Skinner, 2001
				[ComRegisterFunction()]
				public static void RegisterClass(string key)
				{
					StringBuilder sb = new StringBuilder(key);
					sb.Replace(@"HKEY_CLASSES_ROOT\", "");
					RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);
					RegistryKey ctrl = k.CreateSubKey("Control");
					ctrl.Close();
					RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
					inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
					inprocServer32.Close();
					k.Close();
				}
				[ComUnregisterFunction()]
				public static void UnregisterClass(string key)
				{
					StringBuilder sb = new StringBuilder(key);
					sb.Replace(@"HKEY_CLASSES_ROOT\", "");
					RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);
					k.DeleteSubKey("Control", false);
					RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
					k.DeleteSubKey("CodeBase", false);
					k.Close();
				}
				#endregion
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
			}
			EndText
			
		Return m.lccode


*========================================================================================
* Generate ActiveX res
*========================================================================================
Procedure GenerateActiveXRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:library
		/reference:Guineu.Runtime.Desktop.dll
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc


*========================================================================================
* Generates the program required for a library
*========================================================================================
Procedure GenerateLibCS( tcCSFile, tcCore, toClasses )

	*--------------------------------------------------------------------------------------
	* Template for the header
	*--------------------------------------------------------------------------------------
	Local lcCode
	Text to m.lcCode TEXTMERGE NOSHOW Pretext 2
		using System;
		using System.Collections.Generic;
		using System.ComponentModel;
		using System.Data;
		using System.Drawing;
		using System.Text;
		using System.Windows.Forms;
		using System.Runtime.InteropServices;
		using System.Reflection;
		using Microsoft.Win32;

		namespace <<m.tcCore>>
		{
	EndText 
	
	*--------------------------------------------------------------------------------------
	* For each class generate an interface an a class
	*--------------------------------------------------------------------------------------
	Local loClass
	For each loClass in toClasses FoxObject
		lcCode = m.lcCode + GenerateLibClassCS(m.loClass)
	EndFor
	
	*--------------------------------------------------------------------------------------
	* End
	*--------------------------------------------------------------------------------------
	lcCode = m.lcCode + Chr(13)+Chr(10) + "}"

	StrToFile( m.lcCode, m.tcCSFile, 0 )
EndProc 

*========================================================================================
* 
*========================================================================================
Procedure GenerateLibClassCS( toClass )

	*--------------------------------------------------------------------------------------
	* Generate a GUID for the class and the interface
	*--------------------------------------------------------------------------------------
	Local lcCode
	lcCode = ""
		Text to m.lcCode ADDITIVE NOSHOW TEXTMERGE PRETEXT 2
			public class <<m.toClass.CSName>>: Guineu.Interop.GuineuHost
			{
				public <<m.toClass.CSName>>()
					: base("<<m.toClass.File>>", "<<m.toClass.ClassName>>")
				{
					InitializeComponent();
				}
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
			}
			EndText
			
		Return m.lccode


*========================================================================================
* Generate Lib res
*========================================================================================
Procedure GenerateLibRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:library
		/reference:Guineu.Runtime.Desktop.dll
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

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


*========================================================================================
* Generate the CS file for a stored procedure
*========================================================================================
Procedure GenerateSPTCS( tcCSFile, tcCore, tcPrg, tcMain )

	Local lcCode
	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2
using System;
using System.Collections.Generic;
using System.Text;
using Guineu;
using System.Reflection;
using Guineu.Data;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;


	public class <<m.tcCore>>
	{
	EndText 
	
	*--------------------------------------------------------------------------------------
	* Obtain a list of all procedures
	*--------------------------------------------------------------------------------------
	Local laProc[1], lnProc, lcType, laCode[1], lcConvert
	For m.lnProc = 1 to AProcInfo(laProc,m.tcPrg)

		ALines(laCode,FileToStr(m.tcPrg))
		lcType = Alltrim(upper(StrExtract(laCode[laProc[m.lnProc,2]],"AS","",1,1)))
		Do case
		Case m.lcType == "STRING"
			lcType = "String"
			lcConvert = "ToString(null)"
		Case m.lcType == "NUMBER"
			lcType = "Double"
			lcConvert = "ToNumber()"
		EndCase 
				

	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2 ADDITIVE 
	
		[SqlFunction(DataAccess = DataAccessKind.Read)]
		static public <<m.lcType>> <<laProc[m.lnProc,1]>>()
		{
			GuineuInstance.InitInstance(Assembly.GetExecutingAssembly());
			GuineuInstance.Connections.Engine = new ContextConnectionEngine();
			CompiledProgram code = new CompiledProgram("<<m.tcMain>>");
			CodeBlock procedure = code.Locate("<<laProc[m.lnProc,1]>>");
			Variant retVal = GuineuInstance.Context.ExecuteInNewContext(procedure, null);
			return retVal.<<m.lcConvert>>;
		}
EndText 
	EndFor
		
	lcCode = m.lcCode + "}"
	StrToFile( m.lcCode, m.tcCSFile, 0 )
	
EndProc
*========================================================================================
* Generate ActiveX res
*========================================================================================
Procedure GenerateSPTRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:library
		/reference:Guineu.sql.dll
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
		
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc

*========================================================================================
* 
*========================================================================================
Procedure GenerateSPTScript( tcFile, tcCore, tcDLL, tcPrg )
Local lcText
lcText = ""

	Local laProc[1], lnProc, lcType, laCode[1]
	For m.lnProc = 1 to AProcInfo(laProc,m.tcPrg)

			Text to m.lcText noshow textmerge  ADDITIVE 

			Drop Function <<laProc[m.lnProc,1]>>
			go

			EndText 
		EndFor 
		

Text to m.lcText noshow textmerge  ADDITIVE 

drop Assembly "<<m.tcCore>>"
go
CREATE ASSEMBLY <<m.tcCore>> 
	FROM '<<m.tcDLL>>'
	WITH PERMISSION_SET = UNSAFE;
go

EndText 

	Local laProc[1], lnProc, lcType, laCode[1]
	For m.lnProc = 1 to AProcInfo(laProc,m.tcPrg)

		ALines(laCode,FileToStr(m.tcPrg))
		lcType = Alltrim(upper(StrExtract(laCode[laProc[m.lnProc,2]],"AS","",1,1)))
		Do case
		Case m.lcType == "STRING"
			lcType = "nVarchar(254)"
		Case m.lcType == "NUMBER"
			lcType = "float"
		EndCase 


Text to m.lcText noshow textmerge ADDITIVE 

Create Function <<laProc[m.lnProc,1]>>()
	Returns <<m.lcType>>
	External Name "<<m.tcCore>>"."<<m.tcCore>>"."<<laProc[m.lnProc,1]>>"
go

EndText 
EndFor 

StrToFile(m.lcText,m.tcFile)
EndProc


*========================================================================================
* Returns a collection of classes in the project.
*========================================================================================
Procedure GetClassCollection( toFiles, tcProject, tcCore )

	Local loFile, loClasses
	loClasses = CreateObject("Collection")
	For each loFile in toFiles
		If m.loFile.Type == "V"
			GetClassesOfLibrary( m.loClasses, m.loFile.Name, tcProject, m.tcCore )
		EndIf 
	EndFor 	

Return m.loClasses


*========================================================================================
* Ermittelt alle Klassen in einer Klassenbibliothek und erstellt entsprechende Objekte.
*========================================================================================
Procedure GetClassesOfLibrary( toClasses, tcFile, tcProject, tcCore )
	
	*--------------------------------------------------------------------------------------
	* Modified project flag
	*--------------------------------------------------------------------------------------
	Local llSave
	llSave = .F.
	
	*--------------------------------------------------------------------------------------
	* Get a list of all classes
	*--------------------------------------------------------------------------------------
	Local laClass[1], lnCount
	lnCount = AVcxClasses( laClass, m.tcFile )
	
	*--------------------------------------------------------------------------------------
	* Den XML Parser mit den Angaben des Projektes laden.
	*--------------------------------------------------------------------------------------
	Local loXML, lcFile
	loXML = NewObject("CXMLParser","__guineuide_xmlparser.prg")
	lcFile = m.tcProject + ".xml"
	If File(m.lcFile)
		loXML.Requery(m.lcFile)
	Else
		loXML.New("project")
		llSave = .T.
	EndIf 
	
	*--------------------------------------------------------------------------------------
	* Iterate through all classes
	*--------------------------------------------------------------------------------------
	Local loClass, lnClass
	For lnClass = 1 to m.lnCount
		loClass = GetClass( tcCore, tcFile, laClass[m.lnClass,1], m.loXML, @llSave )
		If not IsNull(m.loClass)
			toClasses.Add( loClass, loClass.Name )
		EndIf
	EndFor 
	
	*--------------------------------------------------------------------------------------
	* Save project
	*--------------------------------------------------------------------------------------
	If m.llSave
		loXML.Save(m.lcFile)
	EndIf

EndProc


*========================================================================================
* 
*========================================================================================
Procedure GetClass( tcCore, tcFile, tcName, toXML, rlSave )

	*--------------------------------------------------------------------------------------
	* Generate the internal name
	*--------------------------------------------------------------------------------------
	Local lcName, lcProgID
	lcName = Lower(JustStem(m.tcFile)) + "." + Lower(m.tcName)
	lcProgID = Lower(m.tcCore) + "." + Lower(m.tcName)

	*--------------------------------------------------------------------------------------
	* Create empty object
	*--------------------------------------------------------------------------------------
	Local loClass
	loClass = CreateObject("Empty")
	AddProperty( m.loClass, "Name", m.lcName )
	AddProperty( m.loClass, "ProgID", m.lcProgID )
	AddProperty( m.loClass, "CSName", Lower(m.tcName) )
	AddProperty( m.loClass, "ClassName", Upper(m.tcName) )
	AddProperty( m.loClass, "File", JustFname(m.tcFile)+".fxp" )
	AddProperty( m.loClass, "GUID", "" )
	AddProperty( m.loClass, "IID", "" )
	
	*--------------------------------------------------------------------------------------
	* Check for existing values.
	*--------------------------------------------------------------------------------------
	loClass.GUID = m.toXML.GetValue("/project/"+m.lcName+"/@guid")
	If Empty(m.loClass.GUID)
		loClass.GUID = getGUID()
		toXML.SetValue( "guid", loClass.Guid, toXML.NeedNode(m.lcName,toXML.GetNode("/project")) )
		rlSave = .T.
	EndIf	
	loClass.IID = m.toXML.GetValue("/project/"+m.lcName+"/@iid")
	If Empty(m.loClass.IID)
		loClass.IID = getGUID()
		toXML.SetValue( "iid", loClass.IID, toXML.NeedNode(m.lcName,toXML.GetNode("/project")) )
		rlSave = .T.
	EndIf	
	
Return m.loClass


*========================================================================================
* GUID erzeugen
*========================================================================================
Procedure GetGUID

	Local lcGUID
	Declare Integer CoCreateGuid in OLE32.DLL String@
	lcGUID = Space(16)
	CoCreateGuid(@lcGUID)
	lcGUID = Strconv( m.lcGUID, 15 )
	lcGUID = Left(m.lcGUID,8)+"-"+Substr(m.lcGUID,9,4)+"-"+ ;
		Substr(m.lcGUID,13,4)+"-"+Substr(m.lcGUID,17,4)+"-"+Right(m.lcGUID,12)

Return m.lcGUID


*========================================================================================
* Generate the CS for a ASP.NET application
*========================================================================================
Procedure GenerateWebGUICS( tcCSFile, tcCore, tcMain )

	Local lcCode
	Text to lcCode NOSHOW TEXTMERGE PRETEXT 2
		using System;
		using Guineu;

		namespace <<m.tcCore>>
		{
			public class index : Guineu.Gui.WebGUI.WebGUIScreen
			{
				public index() : base()
				{
					GuineuInstance.InitInstance();
					GuineuInstance.Do("<<m.tcMain>>");
				}
			}
		}
	EndText 
	StrToFile( m.lcCode, m.tcCSFile, 0 )
	
EndProc


*========================================================================================
* Generate the Response file for a WebGUIapplication
*========================================================================================
Procedure GenerateWebGUIRes( tcResFile, tcCore, tcExe, tcRoot, toFiles )

	Local lcRes, lcWebGUI
	lcWebGUI = Addbs(GetEnv("ProgramFiles"))+"Gizmox\Visual WebGui\SDK20"
	Text to lcRes NOSHOW TEXTMERGE PRETEXT 2
		/target:library
		/reference:bin\Guineu.Runtime.Desktop.dll
		/r:"<<m.lcWebGUI>>\Gizmox.WebGUI.Common.dll"
		/r:"<<m.lcWebGUI>>\Gizmox.WebGUI.Forms.dll"
		/r:"<<m.lcWebGUI>>\Gizmox.WebGUI.Server.dll"
		/out:"<<m.tcExe>>"
		<<m.tcCore>>.cs
				
	EndText
	lcRes = m.lcRes + GetResourceList(m.toFiles,m.tcRoot)
	StrToFile( m.lcRes, m.tcResFile, 0 )

EndProc


*========================================================================================
* 
*========================================================================================
Procedure GenerateWebGUIConfig( tcDest, tcCore )

Local lcFile
Text to m.lcFile noshow TEXTMERGE
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    
	<configSections>
		<section name="WebGUI" type="Gizmox.WebGUI.Common.Configuration.ConfigHandler, Gizmox.WebGUI.Common, Version=2.0.5701.0, Culture=neutral, PublicKeyToken=263fa4ef694acff6" />
	</configSections>
	 
	<WebGUI>
	 
		<!--  WEBGUI AUTHENTICATION MANAGEMENT
			This section defines the application authentication mode.
			Example: 
				<Authentication Mode="Dialog" Type="Ifn.W2.Web.POC.Forms.Logon, Ifn.W2.Web.POC" />
			Example Help: 
				Mode:
					Dialog - Authentication is done in a popup window
				Type:
					A full type name to handle logon (This class should inherit from the LogonForm)
		-->
		<!--Authentication Mode="Main" Type="Gizmox.WebGUI.Sample.Forms.Logon, Gizmox.WebGUI.Sample" /-->
		
		
		
		
		<!--  WEBGUI APPLICATION MANAGEMENT
			This section maps pages to application forms.
			Example: 
				<Application Code="MainForm" Type="MyCode.MainForm, MyCode"/>.
			Example Help: 
				The current application definition maps the full qualified class name to the 
				specifiec page code. This means that the link http://[host]/MainForm.wgx will
				be handles by the givven application form.
		-->
		<Applications>
			<Application Code="index" Type="<<m.tcCore>>.index, <<m.tcCore>>"/>
		</Applications>
		
		<!--  WEBGUI CONTROL MANAGEMENT 
			This section registers controls so that the webgui server can expose there resources.
		-->
		<Controls>
			<!-- <Control Type="Gizmox.WebGUI.Forms.Catalog.Controls.WinPanel, Gizmox.WebGUI.Forms.Catalog"/> -->
		</Controls>
		
		<!--  WEBGUI THEMES MANAGEMENT
			Selected - The currently selected scheme
		-->
		<Themes Selected="Default">
			<!-- <Theme Name="MyTheme" Assembly="MyThemeAssembly" /> -->
		</Themes>
		
		<!--  WebGUI RESOURCE MANAGEMENT
			Icons		- This maps the icon directory (Absolute\Relative path).
			Images		- This maps the images directory (Absolute\Relative path).
			Generated	- This maps the generated resources directory and is requiered for using custom controls (Absolute\Relative path).
			Data		- This maps the data directory which should contain data related files (Absolute\Relative path)  
		-->
		<Directories>
			<Directory Code="Icons"		Path="Resources\Icons" />
			<Directory Code="Images"	Path="Resources\Images" />
			<Directory Code="Generated"	Path="Resources\Generated" />
			<Directory Code="UserData"	 Path="Resources\UserData" />
		</Directories>
		
		<!-- WEBGUI STATIC RESOURCES MANAGMENT
			The mechanism should be used in a deployment scenario to boost performance. The static
			resoruces mode creates a directory named "Route" and with in it files that are accessed
			directly using the web server instead of dynamicly generated files. You should check that
			there the site can write to that directory.
		-->
		<StaticResources Mode="Off"/>
		
		<!--
			WEBGUI PRIVATE VERSION
			Adds the private version key to the caching key. This key provides a mechanism to invalidate
			both client and server caching. You should use this key when you are delpoying a theme or	
			a new custom control. The server and client caching are per site.
		-->
		<PrivateVersion Value="1"/>
		
		
		<!--
			WEBGUI PRELOADING
			Chaning the Mode to "On" enables icon preloading mechanism. This mainly prevents IE
			loading the same image multiple times.
		-->
		<IconsPreloading Mode="Off"/> 
		
	</WebGUI>

	<system.diagnostics>
		<switches>
			<!--
			Provides a mechanism for monitor an application using a debug messages viewer or
			in debug time in the output window. Remember to turn turn this feature off in 
			a deplyment scenario.
			0 - Disabled
			1 - Gives error messages
			2 - Gives errors and warnings
			3 - Gives more detailed error information
			4 - Gives verbose trace information
			-->
			<add name="VWG_TracingSwitch" value="0" />
			<!--
			Performance tracing limited to this threshold
			-->
			<add name="VWG_TracingThresholdSwitch" value="10" />
			<!--
			Disables the caching of Visual WebGui resources
			-->
			<add name="VWG_DisableCachingSwitch" value="0"/>
			<!--
			Disables client sources obscuring (Mainly for debugging and developing purposes)
			0 - Disabled
			1 - Enabled
			-->
			<add name="VWG_DisableObscuringSwitch" value="0" />
		</switches>
	</system.diagnostics>
	
  <system.web>

	<httpHandlers>
		
			<!--  WebGUI ROUTER HANDLER
				This http handler routes the requests to Modules/Icons/Images/Css/Xslt/Resoureces.
				Client events are sinked through this router as well.
			-->
			<add verb="*" path="*.wgx" type="Gizmox.WebGUI.Server.Router,Gizmox.WebGUI.Server,Version=2.0.5701.0,Culture=neutral,PublicKeyToken=3de6eb684226c24d" />
			
	</httpHandlers>
    <!--  DYNAMIC DEBUG COMPILATION
          Set compilation debug="true" to enable ASPX debugging.  Otherwise, setting this value to
          false will improve runtime performance of this application. 
          Set compilation debug="true" to insert debugging symbols (.pdb information)
          into the compiled page. Because this creates a larger file that executes
          more slowly, you should set this value to true only when debugging and to
          false at all other times. For more information, refer to the documentation about
          debugging ASP.NET files.
    -->
    <compilation 
         defaultLanguage="c#"
         debug="true"
    />

    <!--  CUSTOM ERROR MESSAGES
          Set customErrors mode="On" or "RemoteOnly" to enable custom error messages, "Off" to disable. 
          Add <error> tags for each of the errors you want to handle.

          "On" Always display custom (friendly) messages.
          "Off" Always display detailed ASP.NET error information.
          "RemoteOnly" Display custom (friendly) messages only to users not running 
           on the local Web server. This setting is recommended for security purposes, so 
           that you do not display application detail information to remote clients.
    -->
    <customErrors 
    mode="RemoteOnly" 
    /> 

    <!--  AUTHENTICATION 
          This section sets the authentication policies of the application. Possible modes are "Windows", 
          "Forms", "Passport" and "None"

          "None" No authentication is performed. 
          "Windows" IIS performs authentication (Basic, Digest, or Integrated Windows) according to 
           its settings for the application. Anonymous access must be disabled in IIS. 
          "Forms" You provide a custom form (Web page) for users to enter their credentials, and then 
           you authenticate them in your application. A user credential token is stored in a cookie.
          "Passport" Authentication is performed via a centralized authentication service provided
           by Microsoft that offers a single logon and core profile services for member sites.
    -->
    <authentication mode="Windows" /> 

	<!--  AUTHORIZATION 
          This section sets the authorization policies of the application. You can allow or deny access
          to application resources by user or role. Wildcards: "*" mean everyone, "?" means anonymous 
          (unauthenticated) users.
    -->

    <authorization>
        <allow users="*" /> <!-- Allow all users -->
            <!--  <allow     users="[comma separated list of users]"
                             roles="[comma separated list of roles]"/>
                  <deny      users="[comma separated list of users]"
                             roles="[comma separated list of roles]"/>
            -->
    </authorization>

    <!--  APPLICATION-LEVEL TRACE LOGGING
          Application-level tracing enables trace log output for every page within an application. 
          Set trace enabled="true" to enable application trace logging.  If pageOutput="true", the
          trace information will be displayed at the bottom of each page.  Otherwise, you can view the 
          application trace log by browsing the "trace.axd" page from your web application
          root. 
    -->
    <trace
        enabled="false"
        requestLimit="10"
        pageOutput="false"
        traceMode="SortByTime"
		localOnly="true"
    />

    <!--  SESSION STATE SETTINGS
          By default ASP.NET uses cookies to identify which requests belong to a particular session. 
          If cookies are not available, a session can be tracked by adding a session identifier to the URL. 
          To disable cookies, set sessionState cookieless="true".
    -->
    <sessionState 
            mode="InProc"
            stateConnectionString="tcpip=127.0.0.1:42424"
            sqlConnectionString="data source=127.0.0.1;Trusted_Connection=yes"
            cookieless="false" 
            timeout="20" 
    />

    <!--  GLOBALIZATION
          This section sets the globalization settings of the application. 
    -->
    <globalization 
            requestEncoding="utf-8" 
            responseEncoding="utf-8" 
   />
   
 </system.web>

</configuration>
EndText 

	StrToFile( m.lcFile, Addbs(m.tcDest)+"web.config", 0 )
	

EndProc


*========================================================================================
* Generate and run an HTML file
*========================================================================================
Procedure GenerateWebGUIHTML(tcFile)

	Local lcText
	Text to m.lcText NOSHOW 
		<html><head>
			<meta http-equiv="refresh" content="0;url=http://localhost/g/index.wgx">
		</head>
		</html>
	EndText

	StrToFile(m.lcText,m.tcFile)
	ShellEx( "open", m.tcFile)

EndProc


*========================================================================================
* 
*========================================================================================
Procedure HasChanged( tcSource, tcDest )

	*--------------------------------------------------------------------------------------
	* If the file is missing we need to create it in any case
	*--------------------------------------------------------------------------------------
	Local laSource[1], laDest[1]
	If ADir(laSource, m.tcSource) == 0
		Return .T.
	EndIf
	If ADir(laDest, m.tcDest) == 0
		Return .T.
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Otherwise compare the date and time
	*--------------------------------------------------------------------------------------
	Do case
	Case laSource[3] < laDest[3]
		Return .F.
	Case laSource[3] > laDest[3]
		Return .T.
	Otherwise 
		If laSource[4] <= laDest[4]
			Return .F.
		Else
			Return .T.
		EndIf
	EndCase

EndProc
