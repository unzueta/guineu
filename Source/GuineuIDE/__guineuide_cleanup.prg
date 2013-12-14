*========================================================================================
* Removes all temporary files that Guineu generated
*========================================================================================
LParameter toProject as Project
	*--------------------------------------------------------------------------------------
	* Settings
	*--------------------------------------------------------------------------------------
	Local lcSafety, lcDeleted
	lcSafety = Set("Safety")
	lcDeleted = Set("Deleted")
	Set Safety off
	Set Deleted on

	*--------------------------------------------------------------------------------------
	* Get names. 
	*--------------------------------------------------------------------------------------
	Local lcCore, lcRoot
	lcRoot = JustPath(toProject.Name)
	lcCore = JustStem(toProject.Name)
	
	*--------------------------------------------------------------------------------------
	* Delete C# files
	*--------------------------------------------------------------------------------------
	DeleteFile( Addbs(m.lcRoot) + m.lcCore + ".cs" )
	DeleteFile( Addbs(m.lcRoot) + m.lcCore + ".res" )
	
	*--------------------------------------------------------------------------------------
	* Go through all files
	*--------------------------------------------------------------------------------------
	Local loFile as File, lcFile
	For each loFile in toProject.Files
		Do case 
		Case m.loFile.Type == "K"
			DeleteFile( m.loFile.Name+".fxp" )
			DeleteFile( m.loFile.Name+".prg" )
		Case m.loFile.Type == "V"
			DeleteFile( m.loFile.Name+".fxp" )
			DeleteFile( m.loFile.Name+".prg" )
		EndCase
	EndFor 

	*--------------------------------------------------------------------------------------
	* Restore environment
	*--------------------------------------------------------------------------------------
	If m.lcSafety == "ON"
		Set Safety on
	EndIf
	If m.lcDeleted == "OFF"
		Set Deleted off
	EndIf
	


*========================================================================================
* Deletes a single file if it exists.
*========================================================================================
Procedure DeleteFile(tcFile)
	
	If File(m.tcFile)
		Try
			Erase (m.tcFile)
		Catch
		EndTry 
	EndIf 

EndProc

