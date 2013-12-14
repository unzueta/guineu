*========================================================================================
* Tests raw speed of loops not using "m."
*========================================================================================

	Local lnStart, lnCount
	lnStart = Seconds()
	lnCount = 0
	Do While lnCount < 5000000
		lnCount = lnCount + 1
	EndDo
	? Seconds()-lnstart, "sec. for 5,000,000 iteration"
