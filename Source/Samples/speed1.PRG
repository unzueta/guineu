*========================================================================================
* Tests raw speed of loops using "m."
*========================================================================================

	Local lnStart, lnCount
	lnStart = Seconds()
	lnCount = 0
	Do While m.lnCount < 5000000
		lnCount = m.lnCount + 1
	EndDo
	? Seconds()-lnstart, "sec. for 5,000,000 iteration"
