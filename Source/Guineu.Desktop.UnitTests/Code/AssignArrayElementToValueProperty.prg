*========================================================================================
* Sample inventory application
*========================================================================================

	Local loForm
	loForm = CreateObject ("frmInventory")
	loForm.Show()
	loForm.cmdSave.Click ()

Return loForm.txtName.Value == "Lurchi"

*========================================================================================
* This form lets the user enter an article number or scan a barcode, then specify a
* quantity. All data is stored in a text file.
*========================================================================================
Define Class frmInventory as Form
	Width = 240
	Height = 268
	Caption = "Inventory"
	
	Add Object lblIntro as Label with ;
		Left = 5, ;
		Top = 8, ;
		width = 230, ;
		Caption = "Zaubername"		
		
	Add Object txtName as Textbox with ;
		Left = 65, ;
		Top = 85, ;
		Width = 90
	
	Add Object cmdSave as Commandbutton with ;
		Left = 175, ;
		Top = 235, ;
		Width = 60, ;
		Height = 27, ;
		Caption = "Zack"
		
*========================================================================================
* Saves the current item into a file.
*========================================================================================
Procedure cmdSave.Click

   DIMENSION aTest[2]
   aTest[1]="Luchs"
   aTest[2]="Lurchi"
   Thisform.txtName.Value = aTest[2]

EndProc



EndDefine