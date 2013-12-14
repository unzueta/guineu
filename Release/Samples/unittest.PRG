*========================================================================================
* This simple unit test suite validates the result of various function calls against the
* behavior of VFP 9. 
*========================================================================================
Clear
Test_ICASE()
Test_CLOSE_DATABASE()
Test_GetEnv()
Test_ADir()
Test_VarType()
Test_Empty()
Test_JustDrive()
Test_StrTran()
Test_ALen()
Test_DriveType()
Test_APrinters()
Test_CMonth() 
MessageBox("Done")


*========================================================================================
* Test for ICASE
*========================================================================================
Procedure Test_ICASE()
	assertEquals( "equal", ICase(1=1,"one"), "one" )
	assertEquals( "not equal", IsNull(ICase(1=2,"one")), .T. )
	assertEquals( "not equal, but default", ICase(1=2,"one","two"), "two" )
	assertEquals( "non existing variable", ICase(1=1,"one",m.DoesntExist), "one" )
	assertEquals( "null", ICase(.NULL.,"one","two"), "two" )
EndProc 

*========================================================================================
* Test for CLOSE DATABASE
*========================================================================================
Procedure Test_CLOSE_DATABASE
	Local lcPath
	lcPath = Strtran( Addbs(JustPath(Sys(16))), "PROCEDURE TEST_CLOSE_DATABASE ", "" )
	Select 1
	Use (m.lcPath+"unittest_data.dbf")
	assertEquals( "opening test table", Used("unittest_data"), .T. )
	Select 2
	Create Cursor curTest (cTest I)
	assertEquals( "creating cursor", Used("curtest"), .T. )
	Select 3
	Close Databases
	Close Databases all
	assertEquals( "closing table", Used(1), .F. )
	assertEquals( "closing cursor", Used(2), .F. )
	assertEquals( "switch to work area 1", Select(), 1 )
EndProc


*========================================================================================
* Test for GetEnv
*========================================================================================
Procedure Test_GetEnv
	assertEquals( "existing variable", GetEnv("windir"), "C:\Windows" )
	assertEquals( "case insensitive", GetEnv("WINDIR"), "C:\Windows" )
	assertEquals( "not existing variable", GetEnv("doesn't exist"), "" )
EndProc


*========================================================================================
* Test for ADIR()
*========================================================================================
Procedure Test_ADir()
	Local la[1], lnFiles
	lnFiles = ADir(la,Addbs(GetEnv("windir"))+"*.*")
	assertEquals( "all files", m.lnFiles>0, .T. )
	assertEquals( "plus directories", ;
		ADir(la,Addbs(GetEnv("windir"))+"*.*","D")>m.lnFiles, .T. )
EndProc


*========================================================================================
* Test for VARTYPE()
*========================================================================================
Procedure Test_VarType
	Local lnVal
	lnVal = 0
	assertEquals( "integer variables", Vartype(m.lnVal), "N" )
EndProc

*========================================================================================
* Test for ALEN()
*========================================================================================
Procedure Test_Alen
	Local la[1]
	assertEquals( "#1,0", Alen(la,0), 1)
	assertEquals( "#1,1", Alen(la,1), 1)
	assertEquals( "#1,2", Alen(la,2), 0)

	Local la[1,2]
	assertEquals( "#2,0", Alen(la,0), 2)
	assertEquals( "#2,1", Alen(la,1), 1)
	assertEquals( "#2,2", Alen(la,2), 2)

	Local la[2,1]
	assertEquals( "#3,0", Alen(la,0), 2)
	assertEquals( "#3,1", Alen(la,1), 2)
	assertEquals( "#3,2", Alen(la,2), 1)
EndProc

*========================================================================================
* Test for EMPTY()
*========================================================================================
Procedure Test_Empty()
	assertEquals( "Strings", Empty(""), .T. )
	assertEquals( "numbers", Empty(0), .T. )
EndProc 

*========================================================================================
* Test for STRTRAN()
*========================================================================================
Procedure Test_StrTran
	AssertEquals( "#1", "aa", strtran("1","1","aa"))
	AssertEquals( "#2", Strtran("H'l","'","''"), "H''l" )
EndProc

*========================================================================================
* Test for JUSTDRIVE()
*========================================================================================
Procedure Test_JustDrive
	assertEquals( "empty string", JustDrive(""), "" )
 	* assertError( "null", [JustDrive(NULL)], 11 )
	assertEquals( "null type", Vartype(JustDrive(""),.F.), "C" )
	assertEquals( "no drive", JustDrive("xx"), "" )
	assertEquals( "colon", JustDrive("c:\test"), "c:" )
	assertEquals( "UNC", JustDrive("\\srv\vol\test"), "" )
EndProc

*========================================================================================
* Test for DRIVETYPE()
*
* C: is a hard disk
* D: is a CD/ROM drive
* E: is a USB stick
* Y: is a network drive
*========================================================================================
Procedure Test_DriveType()
	assertEquals( "local hard disc", DriveType("C:"), 3)
	assertEquals( "CD/ROM drive", DriveType("D:"), 5)
	assertEquals( "network drive", DriveType("Y:"), 4)
	assertEquals( "USB stick", DriveType("E:"), 2)
	assertEquals( "no type", DriveType("no"), 1)
	* assertError( "empty parameter", "DRIVETYPE([])", 202 )
	* assertError( "empty parameter", "DRRIVETYPE(NULL)", 11 )
	assertEquals( "long path", DriveType("C:\test"), 3)
	assertEquals( "root path", DriveType("C:\"), 3)
EndProc

*========================================================================================
* Test for APRINTER()
*========================================================================================
Procedure Test_APrinters
	Local laPrt[1], lnCnt
	lnCnt = APrinters(laPrt)
	assertEquals( "default", lnCnt*2 = Alen(laPrt), .T. )
	lnCnt = APrinters(laPrt,1)
	assertEquals( "extended", lnCnt*5 = Alen(laPrt), .T. )
	* assertError( "invalid flag", "APrinters(laPrt,2)", 11 )
	If Version(4) > "09.10"
		lnCnt = APrinters(laPrt,2)
		assertEquals( "basic", lnCnt = Alen(laPrt), .T. )
	EndIf 
EndProc



*========================================================================================
*TEST: CMonth() 
*========================================================================================
FUNCTION Test_CMonth() 
LOCAL ldStart
ldStart = DATE(2000,1,1)
assertEquals( "January invalid", 	CMONTH(DATE(2000,1,1)) , "January")
assertEquals( "February invalid", 	CMONTH(DATE(2000,2,1)) , "February")
assertEquals( "March invalid", 		CMONTH(DATE(2000,3,1)) , "March")
assertEquals( "April invalid", 		CMONTH(DATE(2000,4,1)) , "April")
assertEquals( "May invalid", 		CMONTH(DATE(2000,5,1)) , "May")
assertEquals( "June invalid", 		CMONTH(DATE(2000,6,1)) , "June")
assertEquals( "July invalid", 		CMONTH(DATE(2000,7,1)) , "July")
assertEquals( "August invalid", 	CMONTH(DATE(2000,8,1)) , "August")
assertEquals( "September invalid", 	CMONTH(DATE(2000,9,1)) , "September")
assertEquals( "October invalid", 	CMONTH(DATE(2000,10,1)) , "October")
assertEquals( "November invalid", 	CMONTH(DATE(2000,11,1)) , "November")
assertEquals( "December invalid", 	CMONTH(DATE(2000,12,1)) , "December")
assertEquals( "Paramter=.NULL.", 	CMONTH(.NULL.) , .NULL.)
assertEquals( "Parameter=DateTime", CMONTH(DATETIME(2000,1,1)), "January")

*========================================================================================
* Unittest routines
*========================================================================================
Procedure AssertEquals( tcMsg, tuVal1, tuVal2 )
	Do case
	Case IsNull(m.tuVal1) and IsNull(m.tuVal2)
		Return
	Case IsNull(m.tuVal1) and not IsNull(m.tuVal2)
		Failed( m.tcMsg, Program(Program(-1)-1) )
	Case not IsNull(m.tuVal1) and IsNull(m.tuVal2)
		Failed( m.tcMsg, Program(Program(-1)-1) )
	Case m.tuVal1 == m.tuVal2
		Return
	Otherwise 
		Failed( m.tcMsg, Program(Program(-1)-1) )
	EndCase
EndProc

Procedure Failed( tcMsg, tcPrg )
	Local lcPrg
	If Empty(m.tcPrg)
		lcPrg = Program(Program(-1))
	Else
		lcPrg = m.tcPrg
	EndIf
	? m.tcPrg, ": ", m.tcMsg
EndProc
