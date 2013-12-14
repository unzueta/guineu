using Guineu.ObjectEngine;
class FocusManager
{
  public void SetFocusToNextControl(UiControl obj)
  {
    // Find next control
    // Check When event
    // if not move to next
  }


  internal static void Skip(UiControl obj, int skipBy)
  {
    UiControl currentObj = obj;
    for (var i = 0; i < skipBy; i++)
    {
      currentObj = GetNextControl(currentObj);
      if (currentObj == obj)
        break;
    }

		// TODO: Implement pemSetFocus
    //obj.pemSetFocus.Do();
  }

  private static UiControl GetNextControl(UiControl currentObj)
  {
    // find next object
    // check enabled
    // check TabStop
    // check When event

		// TODO: remove this line
  	return currentObj;
  }
}
