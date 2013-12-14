*========================================================================================
* Converts a form into a prg file
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
	* Generate the classes
	*--------------------------------------------------------------------------------------
	GenerateForm( m.loClass )
	
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

	Local lcClassLib, lcLib
	lcClassLib = ""
	For each lcLib in m.toClass.ClassLibs
		lcClassLib = m.lcClassLib ;
			+[SET CLASSLIB TO "]+m.lcLib+[" ADDITIVE]+Chr(13)+Chr(10)
	EndFor 

	*--------------------------------------------------------------------------------------
	* Generate Parameter list
	*--------------------------------------------------------------------------------------
	Local lcCmdParameter, lcCreateObject
	If Empty(m.toClass.Parameters)
		lcCmdParameter = ""
		lcCreateObject = ""
	Else
		lcCmdParameter = "LParameters " + m.toClass.Parameters
		* (...) Need to parse variables if AS clause is used
		lcCreateObject = ", " + m.toClass.Parameters
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Is there an include file specified?
	*--------------------------------------------------------------------------------------
	Local lcIncludeFile
	If Empty(m.toClass.IncludeFile)
		lcIncludeFile = ""
	Else
		lcIncludeFile = [#INCLUDE "] + m.toClass.IncludeFile + ["]
	EndIf
	
	Local lcCode, lcName
	lcName = m.toClass.Class+Sys(2015)
	Text to m.lcCode noshow textmerge pretext 2
		<<m.lcCmdParameter>>
		Public <<m.lcName>>
		<<m.lcClassLib>>
		<<m.lcName>> = CreateObject("<<m.toClass.Class>>" <<m.lcCreateObject>>)
		If <<m.lcName>>.WindowType == 1
			<<m.lcName>>.Show(1)
		else
			<<m.lcName>>.Show()
		EndIf
		
		<<m.lcIncludeFile>>
		
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
	AddProperty( m.loClass, "Parameters", "" )
	AddProperty( m.loClass, "Properties", "" )
	AddProperty( m.loClass, "Members", "" )
	AddProperty( m.loClass, "Methods", "" )
	AddProperty( m.loClass, "ClassLibs", CreateObject("Collection") )
	AddProperty( m.loClass, "IncludeFile", "" )
	
Return m.loClass


*========================================================================================
* Generates the code
*========================================================================================
Procedure GenerateForm( toForm )

	*--------------------------------------------------------------------------------------
	* Instantiate the generic converter
	*--------------------------------------------------------------------------------------
	Local loConverter
	loConverter = NewObject("GuineuConverter","__guineuide_converter.prg")

	*--------------------------------------------------------------------------------------
	* Create a form class and add it
	*--------------------------------------------------------------------------------------
	Local loForm
	loForm = CreateClass()
	toForm.Add( m.loForm, "form" )
	
	*--------------------------------------------------------------------------------------
	* The first record contains the include file definition, not the form header.
	*--------------------------------------------------------------------------------------
	Go top
	loForm.IncludeFile = _Form.Reserved8

	*--------------------------------------------------------------------------------------
	* Determine base information
	*--------------------------------------------------------------------------------------
	Locate for BaseClass = "form"
	loForm.Class = _Form.ObjName
	loForm.ParentClass = _Form.Class
	loForm.Parameters = StrExtract( ;
		 _Form.Methods ;
		,"PROCEDURE Init"+Chr(13)+Chr(10)+"Lparameters " ;
		,Chr(13) ;
		,1 ;
		,1 ;
	)
	
	*--------------------------------------------------------------------------------------
	* Add property information
	*--------------------------------------------------------------------------------------
	Local loProp, lcProp
	loProp = loConverter.ConvertProperties(_Form.Properties, _Form.BaseClass)
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
	Local lcCode
	Skip
	Scan while Alltrim(Platform) == "WINDOWS"
		lcCode = loConverter.GenerateMember()
		loForm.Members = m.loForm.Members + m.lcCode + Chr(13)+Chr(10)
	EndScan 
	
	*--------------------------------------------------------------------------------------
	* Collect all methods
	*--------------------------------------------------------------------------------------
	Scan for not Empty(Methods)
		loForm.Methods = m.loForm.Methods + loConverter.ConvertMethod() + Chr(13)+ Chr(10)
	EndScan 
	
	*--------------------------------------------------------------------------------------
	* Determine all class libraries.
	*--------------------------------------------------------------------------------------
	Local lcKey
	Scan for not Empty(ClassLoc)
		lcKey = Upper(JustStem(ClassLoc))
		If loForm.ClassLibs.GetKey(m.lcKey) == 0
			loForm.ClassLibs.Add(ClassLoc, m.lcKey)
		EndIf 
	EndScan 

EndProc
