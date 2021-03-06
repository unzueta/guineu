*========================================================================================
* This sample demonstrates the Signature control. With the signature control you can
* capture customer signatures electronically, for instance, when delivering packages,
* when signing receipts, etc.
*========================================================================================

	*--------------------------------------------------------------------------------------
	* The Signature control is an additional class. We need to enable it.
	*--------------------------------------------------------------------------------------
	Sys(8008, "Signature")
	
	*--------------------------------------------------------------------------------------
	* Ensure this sample is working on hi-res devices, too.
	*--------------------------------------------------------------------------------------
	LOCAL lnScale
	lnScale = SYSMETRIC(2)/320
	SYS(8006, m.lnScale)

	*--------------------------------------------------------------------------------------
	* Now we can run our sample form
	*--------------------------------------------------------------------------------------
	Do Form Signature
	


