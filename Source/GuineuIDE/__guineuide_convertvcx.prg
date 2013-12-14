*========================================================================================
* Converts a class library into a prg file
*========================================================================================
LParameter tcForm

	*--------------------------------------------------------------------------------------
	* Save environment
	*--------------------------------------------------------------------------------------
	Local lnSelect, lcSafety
	lnSelect = Select()
	lcSafety = Set("Safety")
	Set Safety off
	
	*--------------------------------------------------------------------------------------
	* Open the form
	*--------------------------------------------------------------------------------------
	If not File(m.tcForm)
		Return
	EndIf
	Use (m.tcForm) Alias _Form Again In Select("_Form")
	Select _Form
	
	*--------------------------------------------------------------------------------------
	* Storage for the class definition
	*--------------------------------------------------------------------------------------
	Local loClass
	loClass = CreateObject("Collection")
	
	*--------------------------------------------------------------------------------------
	* Get a list of classes
	*--------------------------------------------------------------------------------------
	Local laRecNo[1]
	laRecNo = 0
	Select Recno() ;
		from _Form ;
		where Platform == "WINDOWS " and Empty(Parent) ;
		into ARRAY laRecNo
	
	*--------------------------------------------------------------------------------------
	* Generate the classes
	*--------------------------------------------------------------------------------------
	Local lnClass
	For lnClass = 1 to Alen(laRecNo)
		GenerateForm( m.loClass, laRecNo[m.lnClass] )
	EndFor 
	
	*--------------------------------------------------------------------------------------
	* Generate the code from the class definition
	*--------------------------------------------------------------------------------------
	Local lcCode, loSingle
	lcCode = ""
	For each loSingle in loClass FOXOBJECT 
		lcCode = m.lcCode + GenerateCode(m.loSingle) + Chr(13) + Chr(10)
	EndFor 
	
	*--------------------------------------------------------------------------------------
	* Compile the form
	*--------------------------------------------------------------------------------------
	StrToFile( m.lcCode, m.tcForm+".prg", 0 )
	Compile (m.tcForm+".prg")
	
	*--------------------------------------------------------------------------------------
	* Close the form
	*--------------------------------------------------------------------------------------
	Use in Select("_Form")
	
	*--------------------------------------------------------------------------------------
	* Restore environment
	*--------------------------------------------------------------------------------------
	Select (m.lnSelect)
	If m.lcSafety == "ON"
		Set Safety on
	EndIf
	
Return 


*========================================================================================
* 
*========================================================================================
Procedure GenerateCode( toClass )

	*--------------------------------------------------------------------------------------
	* Is there an include file specified?
	*--------------------------------------------------------------------------------------
	Local lcIncludeFile
	If Empty(m.toClass.IncludeFile)
		lcIncludeFile = ""
	Else
		lcIncludeFile = [#INCLUDE "] + m.toClass.IncludeFile + ["]
	EndIf
	
	Local lcCode
	Text to m.lcCode noshow textmerge pretext 2
		<<m.lcIncludeFile	>>
		Define Class <<m.toClass.Class>> as <<m.toClass.ParentClass>>
			<<m.toClass.Properties>>
			<<m.toClass.Members>>
			<<m.toClass.Methods>>
		EndDefine 
	EndText 
	
Return m.lcCode


*========================================================================================
* Returns an empty class object
*========================================================================================
Procedure CreateClass

	Local loClass
	loClass = CreateObject("Empty")
	AddProperty( m.loClass, "Class", "" )
	AddProperty( m.loClass, "Parentclass", "" )
	AddProperty( m.loClass, "Properties", "" )
	AddProperty( m.loClass, "Members", "" )
	AddProperty( m.loClass, "Methods", "" )
	AddProperty( m.loClass, "IncludeFile", "" )
	
Return m.loClass


*========================================================================================
* Generates the code
*========================================================================================
Procedure GenerateForm( toForm, tnRecNo )

	*--------------------------------------------------------------------------------------
	* Instantiate the generic converter
	*--------------------------------------------------------------------------------------
	Local loConverter
	loConverter = NewObject("GuineuConverter","__guineuide_converter.prg")

	*--------------------------------------------------------------------------------------
	* Locate class in library
	*--------------------------------------------------------------------------------------
	Locate RECORD m.tnRecNo
	
	*--------------------------------------------------------------------------------------
	* Check that the class has a name. In one case Alejandro F. found a problem when the
	* name appears to be empty.
	*--------------------------------------------------------------------------------------
	Assert not Empty(_Form.ObjName)
	Assert not IsNull(_Form.ObjName)
	Assert Vartype(_Form.ObjName) == "C"
	If not Vartype(_Form.ObjName) == "C" or IsNull(_Form.ObjName) or Empty(_Form.ObjName)	
		Return
	EndIf

	*--------------------------------------------------------------------------------------
	* Create a form class and add it
	*--------------------------------------------------------------------------------------
	Local loForm
	loForm = CreateClass()
	
	*--------------------------------------------------------------------------------------
	* Determine base information
	*--------------------------------------------------------------------------------------
	toForm.Add( m.loForm, _Form.ObjName )
	loForm.Class = _Form.ObjName
	loForm.ParentClass = _Form.Class
	loForm.IncludeFile = _Form.Reserved8
	
	*--------------------------------------------------------------------------------------
	* Add property information
	*--------------------------------------------------------------------------------------
	Local loProp, lcProp
	loProp = loConverter.ConvertProperties(_Form.Properties, _Form.BaseClass )
	For each lcProp in loProp
		loForm.Properties = loForm.Properties + m.lcProp + Chr(13) + Chr(10)
	EndFor 
	
	*--------------------------------------------------------------------------------------
	* Add arrays
	*--------------------------------------------------------------------------------------
	Local loProp, lcProp
	loProp = loConverter.ConvertArrays(_Form.Reserved3)
	For each lcProp in loProp
		loForm.Properties = loForm.Properties + m.lcProp + Chr(13) + Chr(10)
	EndFor 
	
	*--------------------------------------------------------------------------------------
	* Add properties that have no value in Properties and protected/hidden elements.
	*--------------------------------------------------------------------------------------
	* (...)
	
	*--------------------------------------------------------------------------------------
	* Add all contained objects
	*--------------------------------------------------------------------------------------
	Local lcCode, lnLast
	Skip
	lnLast = m.tnRecNo
	Scan while Alltrim(Platform) == "WINDOWS"
		lcCode = loConverter.GenerateMember()
		loForm.Members = m.loForm.Members + m.lcCode + Chr(13)+Chr(10)
		lnLast = Recno()
	EndScan 
	
	*--------------------------------------------------------------------------------------
	* Collect all methods
	*--------------------------------------------------------------------------------------
	Scan for not Empty(Methods) and Between(Recno(),m.tnRecNo,m.lnLast)
		loForm.Methods = m.loForm.Methods + loConverter.ConvertMethod() + Chr(13)+ Chr(10)
	EndScan 

EndProc

