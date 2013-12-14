*========================================================================================
* This sample requires a standard installation of MS SQL Server. It uses SPT to connect
* to the server and lists all customers.
*========================================================================================

	*--------------------------------------------------------------------------------------
	* Retrieve records from the SQL server
	*--------------------------------------------------------------------------------------
	Local lnHandle
	lnHandle = SQLStringConnect( ;
		"DRIVER=SQL Server;SERVER=(local);Trusted_Connection=YES;Database=Northwind" ;
	)
	If m.lnHandle = -1
		MessageBox("Cannot connect to SQL server.")
		Return
	EndIf 
	SQLExec(m.lnHandle,"Select * from Customers","Customers") 
	SQLDisconnect(m.lnHandle)

	*--------------------------------------------------------------------------------------
	* Print them all
	*--------------------------------------------------------------------------------------
	Scan
		? Customers.Customerid + ": " + customers.CompanyName
	Endscan
