*========================================================================================
* Generic converter class
*========================================================================================
Define Class GuineuConverter as Custom

*========================================================================================
* Converts a method memo field
*========================================================================================
Procedure ConvertMethod

	Local lcCode, lcName
	lcCode = Chr(13)+Chr(10) + _Form.Methods
	If not Empty(_Form.Parent)
		lcName = StrExtract(_Form.Parent,".","")
		If Empty(m.lcName)
			lcName = _Form.ObjName
		Else
			lcName = m.lcName+[.]+_Form.ObjName
		EndIf
		lcCode = Strtran( ;
			m.lcCode, ;
			Chr(13)+Chr(10)+"PROCEDURE ", ;
			Chr(13)+Chr(10)+"PROCEDURE " + m.lcName+ "." ;
		)
	EndIf

Return m.lcCode


*========================================================================================
* Generates the code for a single member
*========================================================================================
Procedure GenerateMember

	Local lcCode, loProp, lnProp, lcName
	lcName = StrExtract(_Form.Parent,".","")
	If Empty(m.lcName)
		lcName = _Form.ObjName
	Else
		lcName = ["]+m.lcName+[.]+_Form.ObjName+["]
	EndIf
	lcCode = "ADD OBJECT "+m.lcName+" as " + _Form.Class
	loProp = This.ConvertProperties( _Form.Properties, _Form.BaseClass )
	If m.loProp.Count > 0
		lcCode = m.lcCode + " WITH "
		For lnProp=1 to loProp.Count
			If m.lnProp > 1
				lcCode = m.lcCode + ", "
			EndIf
			lcCode = m.lcCode + loProp.Item[m.lnProp]
		EndFor 
	EndIf 
	
Return m.lcCode

*====================================================================
* Liest alle Eigenschaften im aktuellen PROPERTIES Feld im aktuellen
* Datensatz in ein zweidimensionalas Array. Die erste Spalte enthält
* den Eigenschaftennamen, die zweite den gespeicherten Wert.
*====================================================================
PROCEDURE propertiestoarray
LParameter raProperties, tcProperties

	External Array raProperties
	
	*-----------------------------------------------------------------
	* Assertions
	*-----------------------------------------------------------------
	Assert PCount() == 2
	Assert Type("ALen(raProperties)") == "N"
	Assert Vartype(m.tcProperties) == "C"

	*-----------------------------------------------------------------
	* 
	*-----------------------------------------------------------------
	Local lcProperties
	lcProperties = m.tcProperties

	*--------------------------------------------------------------------------------------
	* 
	*--------------------------------------------------------------------------------------
	Local lcName, lcContent, lnLen, lnCount
	lnCount = 0
	Do while Len(m.lcProperties) > 0
		lcName = StrExtract(m.lcProperties,""," = ")
		If Empty(m.lcName)
			Exit
		EndIf
		lcProperties = Substr(m.lcProperties,Len(m.lcName)+3+1)
		If Left(m.lcProperties,517) == Replicate(Chr(1),517)
			lcProperties = Substr(m.lcProperties,517+1)
			lnLen = Val(Left(m.lcProperties,8))
			lcContent = Replicate(Chr(1),517) + Left(m.lcProperties,m.lnLen+8)
			lcProperties = Substr(m.lcProperties,8+1+m.lnLen+2)
		Else
			lcContent = StrExtract(m.lcProperties,"",Chr(13)+Chr(10))
			lcProperties = Substr(m.lcProperties,Len(m.lcContent)+2+1)
		EndIf
		lnCount = m.lnCount + 1
		Dimension raProperties[m.lnCount,2]
		raProperties[m.lnCount,1] = m.lcName
		raProperties[m.lnCount,2] = m.lcContent
	EndDo 

Return m.lnCount


*========================================================================================
* Returns a collection of property values as valid VFP expressions.
*========================================================================================
Procedure ConvertProperties( tcProp, tcClass )

	*--------------------------------------------------------------------------------------
	* Read properties into an array
	*--------------------------------------------------------------------------------------
	Local laProp[1], lnCnt
	lnCnt = This.PropertiesToArray( @laProp, m.tcProp )
	
	*--------------------------------------------------------------------------------------
	* Collection for properties
	*--------------------------------------------------------------------------------------
	Local loProp
	loProp = CreateObject("Collection")
	
	*--------------------------------------------------------------------------------------
	* Go through all properties and parse them indivdually
	*--------------------------------------------------------------------------------------
	Local lnProp, lcProp
	For lnProp = 1 to m.lnCnt
		lcProp = This.ConvertProperty( laProp[m.lnProp,1], laProp[m.lnProp,2], m.tcClass )
		If not Empty(m.lcProp)
			loProp.Add( m.lcProp )
		EndIf 
	EndFor 
	
Return m.loProp


*========================================================================================
* Returns a collection of array properties as valid DIMENSION commands
*========================================================================================
Procedure ConvertArrays( tcProp )

	*--------------------------------------------------------------------------------------
	* Read properties into an array
	*--------------------------------------------------------------------------------------
	Local laProp[1], lnCnt
	lnCnt = ALines(laProp, m.tcProp, Chr(10))
	
	*--------------------------------------------------------------------------------------
	* Collection for properties
	*--------------------------------------------------------------------------------------
	Local loProp
	loProp = CreateObject("Collection")
	
	*--------------------------------------------------------------------------------------
	* Go through all properties and parse them indivdually. Arrays start with ^
	*--------------------------------------------------------------------------------------
	Local lnProp, lcProp, lcSize, lcName, lcDef
	For lnProp = 1 to m.lnCnt
		If Left(laProp[m.lnProp], 1) == "^"
			lcDef = Substr(laProp[m.lnProp],2)
			lcName = StrExtract (m.lcDef, "", "[")
			lcSize = StrExtract (m.lcDef, "[", "]")
			If Right(m.lcSize,2) == ",0"
				lcSize = GetWordNum(m.lcSize,1,",")
			EndIf
			lcProp = "DIMENSION " + m.lcName + "[" + m.lcSize + "]"
			loProp.Add( m.lcProp )
		EndIf 
	EndFor 
	
Return m.loProp


*========================================================================================
* Converts an individual property
*========================================================================================
Procedure ConvertProperty( tcName, tcValue, tcBaseClass )

	*--------------------------------------------------------------------------------------
	* Get the actual property name if the name consists of object names, as well.
	*--------------------------------------------------------------------------------------
	Local lcName
	If "." $ m.tcName
		lcName = Substr( m.tcName,Rat(".",m.tcName)+1)
	Else
		lcName = m.tcName
	EndIf

	Do case	
	*--------------------------------------------------------------------------------------
	* Skip properties that are only internally used
	*--------------------------------------------------------------------------------------
	case m.lcName == "Name" ;
		or m.lcName == "DoCreate"
		Return ""
	
	*--------------------------------------------------------------------------------------
	* Properties with expressions don't need any special treatment
	*--------------------------------------------------------------------------------------
	Case Left(m.tcValue,1) == "("
		Return m.tcName + " = " + m.tcValue
	
	*--------------------------------------------------------------------------------------
	* Some native properties don't store string value as string. This is true for all 
	* properties that contain paths to images.
	*--------------------------------------------------------------------------------------
	Case m.lcName == "Picture" ;
	  or m.lcName == "Icon" ;
	  or m.lcName == "OLEDragPicture" ;
	  or m.lcName == "DragIcon" ;
	  or m.lcName == "Picture" ;
	  or m.lcName == "DownPicture" ;
	  or m.lcName == "DisabledPicture" ;
	  or m.lcName == "MouseIcon" ;
	  or m.lcName == "MemberClassLibrary"
		Return m.tcName + [ = "] + Strtran(m.tcValue,"..\","") + ["]
	
	*--------------------------------------------------------------------------------------
	* The Value property can be any type. Strings are not stored as strings, though.
	*--------------------------------------------------------------------------------------
	Case m.lcName == "Value" and This.IsStringValue(m.tcValue)
		Return m.tcName + [ = "] + Strtran(m.tcValue,"..\","") + ["]
	
	*--------------------------------------------------------------------------------------
	* Native color properties store the RGB values
	*--------------------------------------------------------------------------------------
	Case m.lcName == "ForeColor" ;
		or m.lcName == "BackColor" ;
		or m.lcName == "FillColor" ;
		or m.lcName == "GridLineColor" ;
		or m.lcName == "BorderColor" ;
		or m.lcName == "DisabledBackColor" ;
		or m.lcName == "DisabledForeColor" ;
		or m.lcName == "DisabledItemBackColor" ;
		or m.lcName == "DisabledItemForeColor" ;
		or m.lcName == "HighlightBackColor" ;
		or m.lcName == "HighlightForeColor" ;
		or m.lcName == "ItemBackColor" ;
		or m.lcName == "ItemForeColor" ;
		or m.lcName == "SelectedBackColor" ;
		or m.lcName == "SelectedForeColor" ;
		or m.lcName == "SelectedItemBackColor" ;
		or m.lcName == "SelectedItemForeColor"
		Return m.tcName + " = Rgb(" + m.tcValue + ")"
	
	*--------------------------------------------------------------------------------------
	* Built-in properties don't require any conversion. We cannot use PEMSTATUS() to de-
	* tect built-in properties, because this won't work with nested objects like 
	* "Page1.Caption" when the baseclass is a pageframe. Therefore we rely on VFP using
	* Upper case characters for its own properties and lower case for everything else.
	* However, VFP sometimes stores a user-defined property name in all upper case, as 
	* well, so we treat those just like lower-case.
	*
	* There's an exception with string values. VFP uses a series of "", '' and [] as 
	* delimiters. A string with " in the value is enclosed with ''. If both characters are
	* contained, VFP uses []. However, if all three string delimiters are part of the 
	* string, VFP still uses []. This produces invalid syntax
	*--------------------------------------------------------------------------------------
	Case IsUpper(Left(m.lcName,1)) and not m.lcName == Upper(m.lcName)
		Local lcValue
		lcValue = m.tcValue
		If Left(m.lcValue,1) == "[" and Right(m.lcValue,1) == "]" and Len(m.lcValue)>2
			lcValue = Substr(m.lcValue, 2, Len(m.lcValue)-2)
			lcValue = "[" + Strtran(m.lcValue,"]","]+Chr(93)+[") + "]"
		EndIf 
		Return m.tcName + " = " + m.lcValue
		
	*--------------------------------------------------------------------------------------
	* Long property values can only be strings. However, they can contain binary data that
	* needs to be encoded into CHR() lines.
	*--------------------------------------------------------------------------------------
	Case Left(m.tcValue,517) == Replicate(Chr(1),517)
		Local lcData, lnChar, lcLine, lnCode
		lcData = Substr(m.tcValue,517+8+1)
		lcLine = m.lcName + [ = "]
		For lnChar = 1 to Len(m.lcData)
			lnCode = Asc(Substr(m.lcData,m.lnChar,1)) 
			If m.lnCode < 32 or m.lnCode == Asc(["])
				lcLine = m.lcLine ;
					+ ["+Chr(] + Transform(m.lnCode) + [)+"]
			Else
				lcLine = m.lcLine + Substr(m.lcData,m.lnChar,1)
			EndIf
		EndFor
		lcLine = m.lcLine + ["]
		Return m.lcLine
		
	*--------------------------------------------------------------------------------------
	* User defined boolean properties
	*--------------------------------------------------------------------------------------
	Case m.tcValue == ".T." or m.tcValue == ".F." or m.tcValue == ".NULL."
		Return m.tcName + " = " + m.tcValue
		
	*--------------------------------------------------------------------------------------
	* User defined date or datetime properties
	*--------------------------------------------------------------------------------------
	Case Left(m.tcValue,1) == "{" and Right(m.tcValue,1) == "}"
		Return m.tcName + " = " + m.tcValue
		
	*--------------------------------------------------------------------------------------
	* Numeric values start with a digit or a minus sign. Everything else is either a
	* digit or a decimal point.
	*--------------------------------------------------------------------------------------
	Case     (IsDigit(Left(m.tcValue,1)) or Left(m.tcValue,1) == "-") ;
	     and Empty(Chrtran(Substr(m.tcValue,2),"1234567890.","")) ;
	     and Occurs(".",m.tcValue) <= 1
		Return m.tcName + " = " + m.tcValue
		
	*--------------------------------------------------------------------------------------
	* Every other property must be a user-defined string property.
	*--------------------------------------------------------------------------------------
	Otherwise 
		Return m.tcName + [ = "] + Strtran(m.tcValue,["],["+Chr(34)+"]) + ["]
	EndCase
	
EndProc


*========================================================================================
* Returns .T. when the value is a string
*========================================================================================
Procedure IsStringValue(tcValue)

	Do case
	Case Left(m.tcValue,1) == "="
		Return .F.
	Case IsDigit(m.tcValue) 
		Return .F.
	Case m.tcValue == ".F."
		Return .F.
	Case m.tcValue == ".T."
		Return .F.
	Case m.tcValue == ".NULL."
		Return .F.
	Otherwise 
		Return .T.
	EndCase

EndProc

	
EndDefine 

