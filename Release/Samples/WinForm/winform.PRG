*========================================================================================
* This is the procedure file that is called from a .NET application
*========================================================================================

	MessageBox( Seconds() )


*========================================================================================
* This function is called in WinForm.cs, btnExec
*========================================================================================
Function NetClick( tcName )
	Local lcName
	lcName = "Your name is " + Upper(m.tcName)
Return m.lcName

