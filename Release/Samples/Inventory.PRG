*========================================================================================
* Sample inventory application
*========================================================================================

	Local loForm
	loForm = CreateObject("frmInventory")
	loForm.Show(1)


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
		backcolor = RGB(0,0,255), ;
		forecolor = RGB(255,255,0), ;
		Caption = "Please scan a barcode or enter"
		
	Add Object lblIntro2 as Label with ;
		Left = 5, ;
		Top = 25, ;
		width = 230, ;
		backcolor = RGB(0,0,255), ;
		forecolor = RGB(255,255,0), ;
		Caption = "the article code"
		
	Add Object lblArticle as Label with ;
		Left = 5, ;
		Top = 58, ;
		Caption = "EAN/UPC:"
		
	
	Add Object txtArticle as Textbox with ;
		Left = 65, ;
		Top = 55, ;
		Width = 170
		
	Add Object lblQty as Label with ;
		Left = 5, ;
		Top = 88, ;
		Caption = "Quantity:"
		
	Add Object txtQty	as Textbox with ;
		Left = 65, ;
		Top = 85, ;
		backcolor = RGB(0,0,255), ;
		forecolor = RGB(255,255,0), ;
		Width = 90
	
	Add Object cmdSave as Commandbutton with ;
		Left = 175, ;
		Top = 235, ;
		Width = 60, ;
		Height = 27, ;
		Caption = "Save"
	
	Add Object cmd1 as Commandbutton with ;
		Left = 65, ;
		Top = 115, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "1"
	
	Add Object cmd2 as Commandbutton with ;
		Left = 105, ;
		Top = 115, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "2"
	
	Add Object cmd3 as Commandbutton with ;
		Left = 145, ;
		Top = 115, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "3"
	
	Add Object cmd4 as Commandbutton with ;
		Left = 65, ;
		Top = 155, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "4"
	
	Add Object cmd5 as Commandbutton with ;
		Left = 105, ;
		Top = 155, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "5"
	
	Add Object cmd6 as Commandbutton with ;
		Left = 145, ;
		Top = 155, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "6"
	
	Add Object cmd7 as Commandbutton with ;
		Left = 65, ;
		Top = 195, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "7"
	
	Add Object cmd8 as Commandbutton with ;
		Left = 105, ;
		Top = 195, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "8"
	
	Add Object cmd9 as Commandbutton with ;
		Left = 145, ;
		Top = 195, ;
		Height = 35, ;
		Width = 35, ;
		Caption = "9"
	
*========================================================================================
* Saves the current item into a file.
*========================================================================================
Procedure cmdSave.Click
	Local lcFile
	If File("inventory.txt")
		lcFile = FileToStr("inventory.txt")
	Else 
		lcFile = ""
	EndIf
	lcFile = m.lcFile + ;
		Alltrim(Thisform.txtArticle.Value)+Chr(9) + ;
		Alltrim(Thisform.txtQty.Value)+Chr(13)+Chr(10)
	StrToFile(m.lcFile,"inventory.txt")
	Thisform.txtArticle.Value = ""
	Thisform.txtQty.Value = ""
*	Thisform.txtArticle.SetFocus()
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd1.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd2.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd3.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd4.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd5.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd6.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd7.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd8.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc

*========================================================================================
* Add one digit to the current quantity
*========================================================================================
Procedure cmd9.Click
	Thisform.txtQty.Value = Alltrim(Thisform.txtQty.Value) + This.Caption
EndProc


EndDefine 