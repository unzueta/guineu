*========================================================================================
* XML Parser
*========================================================================================
Define Class CXMLParser as Custom

	oXML = NULL

*========================================================================================
* Es kann direkt beim CREATEOBJECT() ein XML String übergeben werden.
*========================================================================================
Procedure Init( tcXML )
	This.LoadParser()
	If not Empty(m.tcXML)
		This.Requery( m.tcXML )
	EndIf
EndProc

*========================================================================================
* Der XML Parser kann entweder mit einem XML String oder einem Dateinamen beladen werden.
*========================================================================================
Procedure Requery( tcXML )
	Local llOK
	If Left(m.tcXML,1) == "<"
		llOK = This.RequeryString(m.tcXML)
	Else
		llOK = This.RequeryFile(m.tcXML)
	EndIf 
Return m.llOK

*========================================================================================
* Laden des XML Parsers
*========================================================================================
Procedure LoadParser
	this.oXML = CREATEOBJECT("msXML2.domDocument.4.0")
	this.oXML.async = .f.
	this.oXML.ValidateOnParse = .f.
	this.oXML.SetProperty("SelectionLanguage", "XPath")
	this.oXML.SetProperty("NewParser", .t.)
EndProc

*========================================================================================
* Laden eines XML Strings
*========================================================================================
Procedure RequeryString( tcXML )
	Local llOK
	llOK = This.oXML.LoadXML( m.tcXML )
Return m.llOK

*========================================================================================
* Laden einer XML Datei
*========================================================================================
Procedure RequeryFile( tcFile )
	Local llOK
	llOK = This.oXML.Load( m.tcFile )
Return m.llOK


*========================================================================================
* Returns nodes in a collection. 
* 
* If tlResolveReferences is .T. and a node defines an href attribute, the reference node
* is replaced with the one it points to. Java web services frequently use MultiRef nodes
* to serialize linked objects.
*========================================================================================
Procedure GetNodes( tcQuery, toNode, tlResolveReferences )
	
	*--------------------------------------------------------------------------------------
	* Eine leere Collection erzeugen
	*--------------------------------------------------------------------------------------
	Local loCollection
	loCollection = CreateObject("Collection")
	
	*--------------------------------------------------------------------------------------
	* Die Abfrage kann entweder von einem Knoten oder dem Root aus erfolgen.
	*--------------------------------------------------------------------------------------
	Local loNode
	If Vartype(m.toNode) == "O"
		loNode = m.toNode
	Else
		loNode = this.oXML
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Abfrage ausführen und alle Knoten übernehmen
	*--------------------------------------------------------------------------------------
	Local loQuery, lnItem, loItem
	loQuery = loNode.selectNodes( m.tcQuery )
	For lnItem=1 to loQuery.length
		loItem = loQuery.item[m.lnItem-1]
		If tlResolveReferences
			loItem = This.HandleReferenceNode( m.loItem )
		EndIf
		loCollection.Add( m.loItem )
	EndFor
	
Return m.loCollection


*========================================================================================
* Checks if the current node is a reference to another node. In this case, the other node
* is returned.
*========================================================================================
Procedure HandleReferenceNode( toNode )

	*--------------------------------------------------------------------------------------
	* Check if the current node is a reference node
	*--------------------------------------------------------------------------------------
	Local lcRef
	lcRef = This.GetValue("@href",m.toNode)
	If Empty(m.lcRef)
		Return m.toNode
	EndIf
	
	*--------------------------------------------------------------------------------------
	* At this time we can only handle references that point to a location inside the 
	* document. These references start with "#"
	*--------------------------------------------------------------------------------------
	If Left(m.lcRef,1) == "#"
		lcRef = Substr(m.lcRef,2)
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Locate the node with the specified ID
	*--------------------------------------------------------------------------------------
	Local loNode
	loNode = This.GetNode('//*[@id="'+m.lcRef+'"]')
	
Return m.loNode


*========================================================================================
* Returns a single node
*========================================================================================
Procedure GetNode( tcQuery, toNode )
	
	*--------------------------------------------------------------------------------------
	* Die Abfrage kann entweder von einem Knoten oder dem Root aus erfolgen.
	*--------------------------------------------------------------------------------------
	Local loNode
	If Vartype(m.toNode) == "O"
		loNode = m.toNode
	Else
		loNode = this.oXML
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Abfrage ausführen und alle Knoten übernehmen
	*--------------------------------------------------------------------------------------
	Local loResult
	loResult = loNode.selectSingleNode( m.tcQuery )
	
Return m.loResult


*========================================================================================
* Returns nodes in an array
*========================================================================================
Procedure AGetNodes( raNodes, tcQuery, toNode )
	
	*--------------------------------------------------------------------------------------
	* Die Abfrage kann entweder von einem Knoten oder dem Root aus erfolgen.
	*--------------------------------------------------------------------------------------
	Local loNode
	If Vartype(m.toNode) == "O"
		loNode = m.toNode
	Else
		loNode = this.oXML
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Abfrage ausführen und alle Knoten übernehmen
	*--------------------------------------------------------------------------------------
	Local loQuery, lnItem, lnCount
	loQuery = loNode.selectNodes( m.tcQuery )
	lnCount = loQuery.length
	If m.lnCount > 0
		Dimension raNodes[m.lnCount]
		For lnItem=1 to m.lnCount
			raNodes[m.lnItem] = loQuery.item[m.lnItem-1]
		EndFor
	EndIf 
	
Return m.lnCount



*========================================================================================
* Liefert das Ergebnis einer Abfrage
*========================================================================================
Procedure GetValue( tcQuery, toNode, tuDefault )

	*--------------------------------------------------------------------------------------
	* Die Abfrage kann entweder von einem Knoten oder dem Root aus erfolgen.
	*--------------------------------------------------------------------------------------
	Local loNode
	If Vartype(m.toNode) == "O"
		loNode = m.toNode
	Else
		loNode = this.oXML
	EndIf
	
	*--------------------------------------------------------------------------------------
	* Abfrage ausführen
	*--------------------------------------------------------------------------------------
	Local loQuery
	loQuery = loNode.selectSingleNode( m.tcQuery )
	
	*--------------------------------------------------------------------------------------
	* Den Wert ermitteln
	*--------------------------------------------------------------------------------------
	Local lcValue
	lcValue = ""
	Do case
	
	*--------------------------------------------------------------------------------------
	* Der Wert konnte nicht gefunden werden
	*--------------------------------------------------------------------------------------
	Case IsNull(m.loQuery)
	
	*--------------------------------------------------------------------------------------
	* Es handelt sich um eine gewöhnliche Node. Wir laden die erst Node, die nicht leer 
	* ist, um kein Problem bei Zeilenumbrüchen zu haben.
	*--------------------------------------------------------------------------------------
	Case loQuery.NodeType == 1
		Local lnNode
		If loQuery.hasChildNodes
			For lnNode=0 to loQuery.childNodes.length-1
				lcValue =  Nvl(loQuery.childNodes[m.lnNode].nodeValue,"")
				If not Empty(m.lcValue)
					Exit
				EndIf
			EndFor
		EndIf 
	
	*--------------------------------------------------------------------------------------
	* Es handelt sich um einen Attributknoten
	*--------------------------------------------------------------------------------------
	Case loQuery.NodeType == 2
		lcValue = Nvl( loQuery.nodeValue, "" )
	
	*--------------------------------------------------------------------------------------
	* Es handelt sich um einen #TEXT Knoten
	*--------------------------------------------------------------------------------------
	Case loQuery.NodeType == 3
		lcValue = Nvl( loQuery.nodeValue, "" )
	EndCase 
	
	*--------------------------------------------------------------------------------------
	* If we have a default value, we use it to convert the value to a matching type.
	*--------------------------------------------------------------------------------------
	Local luValue
	If Pcount() == 3
		luValue = This.ConvertValue( m.lcValue, m.tuDefault )
	Else
		luValue = m.lcValue
	EndIf
	
Return m.luValue


*========================================================================================
* Converts a character value into a value of the specified type. If the value is empty,
* this function returns the default value.
*========================================================================================
Procedure ConvertValue( tcValue, tuDefault )

	*--------------------------------------------------------------------------------------
	* The conversion routine depends on the type.
	*--------------------------------------------------------------------------------------
	Local luValue, lcValue
	lcValue = m.tcValue
	DO case 

	*--------------------------------------------------------------------------------------
	* Boolean values can be stored as yes/no or true/false
	*--------------------------------------------------------------------------------------
	Case Vartype(m.tuDefault)  == "L"
		lcValue = Lower(Alltrim(m.lcValue))
		DO Case
		Case m.lcValue == "yes"
			luValue = .T.
		Case m.lcValue == "true"
			luValue = .T.
		Case m.lcValue == "no"
			luValue = .F.
		Case m.lcValue == "false"
			luValue = .F.
		Otherwise 
			luValue = m.tuDefault
		EndCase 
	
	*--------------------------------------------------------------------------------------
	* Numeric values
	*--------------------------------------------------------------------------------------
	Case Vartype(m.tuDefault) == "N"
		lcValue = Chrtran(m.lcValue,Chrtran(m.lcValue,"$-1234567890.+Ee",""),"")
		luValue = &lcValue
		If Vartype(m.luValue) == "Y"
			luValue = Mton(m.luValue)
		EndIf 
	
	*--------------------------------------------------------------------------------------
	* We always remove blanks surrounding strings
	*--------------------------------------------------------------------------------------
	Case Vartype(m.tuDefault) == "C"
		luValue = Alltrim(m.lcValue)
	
	*--------------------------------------------------------------------------------------
	* VFP 9 handles XSD datetime values properly
	*--------------------------------------------------------------------------------------
	Case Vartype(m.tuDefault) == "T"
		luValue = Ctot(m.lcValue)
	
	*--------------------------------------------------------------------------------------
	* Ohter data types aren't supported.
	*--------------------------------------------------------------------------------------
	Otherwise 
		luValue = .F.
	EndCase 

Return m.luValue


*========================================================================================
* Liefert den Namen eines Knotens zurück.
*========================================================================================
Procedure GetName( toNode )
	Local lcName
	m.lcName = toNode.nodeName
Return m.lcName


*========================================================================================
* Most ActiveX controls use Unicode instead of ANSI. Since VFP isn't Unicode compatible,
* it supports UTF-8 for ActiveX controls.
*========================================================================================
Procedure SetCharset( tcCharset )

	Do case
	Case Upper(Alltrim(m.tcCharset)) == "UTF-8"
		Comprop( This.oXML, "UTF8", 1 )
	Case Upper(Alltrim(m.tcCharset)) == "ANSI"
		Comprop( This.oXML, "UTF8", 0 )
	EndCase

EndProc

*========================================================================================
* 
*========================================================================================
Procedure SetNamespaces( tcNamespaces )

	This.oXML.setProperty( "SelectionNamespaces", m.tcNamespaces )

EndProc

*========================================================================================
* 
*========================================================================================
Procedure New( tcRootNode  )
	
	Local llOK
	llOK = .T.

	*--------------------------------------------
	* Erstellen....
	*--------------------------------------------
	If m.llOK 
		Local lcXML
		lcXML = '<?xml version="1.0" encoding="Windows-1252"?><'+m.tcRootNode+'/>'

		*--------------------------------------------
		* Datei in Speicher laden (noch nicht physikalisch)
		*--------------------------------------------
		If NOT This.Requery(m.lcXML)
			llOK = .F.
		EndIf
	EndIf

Return m.llOK 

*========================================================================================
* 
*========================================================================================
Procedure Save( tcFileName )

	Local llSaved
	Try
		This.oXML.Save(m.tcFileName)
		llSaved = .T.
	Catch
		*
	Endtry
	
Return m.llSaved


*========================================================================================
* 
*========================================================================================
Procedure SetValue( tcElement, tcValue, tuRoot )

	*--------------------------------------------------------------------------------------
	* Die Rootnode bestimmen. 
	*--------------------------------------------------------------------------------------
	Local loRoot
	loRoot = m.tuRoot

	*--------------------------------------------------------------------------------------
	* Nach einem bestehenden Attribut oder einer Node suchen
	*--------------------------------------------------------------------------------------
	Local loAttribute, loNode, llDone, loValue
	llDone = .F.
	loAttribute = loRoot.selectSingleNode( ;
		JustPath(m.tcElement) + "/@" + JustFname(m.tcElement) ;
	)
	If not IsNull(m.loAttribute)
		loAttribute.Value = Alltrim(m.tcValue)
		llDone = .T.
	Else
		loNode = This.GetNode(m.tcElement,m.loRoot)
		If not IsNull(m.loNode)
			If not loNode.hasChildNodes
				loValue = This.oXML.createNode(3,"","")
				loNode.appendChild( m.loValue )
			EndIf
			llDone = .T.
			loNode.childNodes[0].nodeValue = Alltrim(m.tcValue)
		EndIf 
	EndIf 
	
	*--------------------------------------------------------------------------------------
	* Wenn kein bestehendes Element gefunden wurde, müssen wir die Node erzeugen und darin
	* das gewünschte Attribut anlegen
	*--------------------------------------------------------------------------------------
	If not m.llDone
		loNode = This.NeedNode( JustPath(m.tcElement), m.loRoot )
		loAttribute = This.oXML.createNode(2,JustFname(m.tcElement),"")
		loAttribute.Value = m.tcValue
		loNode.attributes.setNamedItem( m.loAttribute )
		llDone = .T.
	EndIf 
	
	Return m.llDone 

EndProc


*========================================================================================
* 
*========================================================================================
Procedure NeedNode( tcNode, tuRoot )

	*--------------------------------------------------------------------------------------
	* Die Root ermitteln
	*--------------------------------------------------------------------------------------
	Local loRoot
	loRoot = m.tuRoot
	
	*--------------------------------------------------------------------------------------
	* Jede einzelne Node überprüfen. 
	*--------------------------------------------------------------------------------------
	Local lnNode, loNode
	loNode = m.loRoot
	For lnNode = 1 to GetWordCount(m.tcNode,"/")
		loNode = This.GetNode( GetWordNum(m.tcNode,m.lnNode,"/"), m.loRoot )
		If IsNull(loNode)
			loNode = This.AddNode( GetWordNum(m.tcNode,m.lnNode,"/"), m.loRoot )
		EndIf 
		loRoot = m.loNode
	EndFor 
	
Return m.loNode


*========================================================================================
* 
*========================================================================================
Procedure AddNode( tcNode, tuRoot )

	Local loRoot, loNode
	Store .Null. To loRoot, loNode

		loRoot = m.tuRoot
			loNode = this.oXml.createNode(1,m.tcNode,"")
			loRoot.appendChild(m.loNode)

Return m.loNode
	
EndDefine