Procedure GetSeconds as Number
Return Seconds()

Procedure GetNames as String

	* Get all contacts in the current context
	Local lnHandle
	lnHandle = SQLStringConnect("")
	SQLExec(m.lnHandle,"Select * from Customers where ContactName like 'A%'")

	* Create comma separated list
	Local lcName
	lcName = ""
	Scan 
		If not Empty(m.lcName)
			lcName = m.lcName + ", "
		EndIf
		lcName = m.lcName + Alltrim(ContactName)
	EndScan
	
	* Disconnect
	SQLDisconnect(m.lnHandle)
	
Return m.lcName
