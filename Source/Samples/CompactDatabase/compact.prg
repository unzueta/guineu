*========================================================================================
* Sample for accessing the SQL Compact Engine. 
*
* Compile this project into a Console application. It's designed to work on the desktop.
* When running on a PDA, you must either install a Console for the PDA or replace the 
* ? commands with some other form of output.
*
* REQUIREMENTS: You should copy the Northwind sample database into your project folder.
*               The sample database is installed with Microsoft SQL Server Compact 
*               Edition into the following directory by default:
*
*               "C:\Program Files\Microsoft SQL Server Compact Edition\v3.1\SDK\Samples"
*
*========================================================================================

	*--------------------------------------------------------------------------------------
	* activate COMPACT Engine 
	*--------------------------------------------------------------------------------------
	Local lcConnStr, lcDatabase
	Sys(8004,"compact")
	lcDatabase = FullPath("northwind.sdf")
	lcConnStr = "Data Source = "+m.lcDatabase
	
	*--------------------------------------------------------------------------------------
	* If the database doesn't exist, create an empty one.
	*--------------------------------------------------------------------------------------
	Local llNew
	If File(m.lcDatabase)
		llNew = .F.
	Else
		llNew = .T.
		Sys(8005,"create",m.lcConnStr)
	EndIf
	
	*--------------------------------------------------------------------------------------
	* connect
	*--------------------------------------------------------------------------------------
	Local lnHandle, laErr[1]
	lcConnStr = "Data Source = "+FullPath("northwind.sdf")
	lnHandle = SQLStringConnect(m.lcConnStr)
	If m.lnHandle < 0 and AError( laErr ) > 0
		If laErr[5] == 2
			Sys(8005,"upgrade",m.lcConnStr)
			lnHandle = SQLStringConnect(m.lcConnStr)
		EndIf 
	EndIf
	
	*--------------------------------------------------------------------------------------
	* If we have a new database, we create some test data
	*--------------------------------------------------------------------------------------
	If m.llNew and m.lnHandle > 0
		SQLExec( m.lnHandle, ;
			"CREATE TABLE Customers (Company_Name national character(20), " + ;
			"Contact_Name national character(20), City national character(20))" ;
		)
		SQLExec( m.lnHandle, ;
			"INSERT INTO Customers (Company_Name, Contact_Name, City) Values (" + ;
			"'Foxpert','Christof Wollenhaupt','Norderstedt')" ;
		)
	EndIf 

	*--------------------------------------------------------------------------------------
	* execute query
	*--------------------------------------------------------------------------------------
	Local lcCmd
	lcCmd = "Select * from Customers order by City"
	If SQLExec(m.lnHandle,m.lcCmd,"customers")
		For t=1 to Fcount()
			? Field(t)
		EndFor 
		Scan
			? Company_Name, Contact_Name, City
		endscan
	EndIf 
	
	*--------------------------------------------------------------------------------------
	* close connection
	*--------------------------------------------------------------------------------------
	SQLDisconnect(m.lnHandle)
	

