using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ComputerUI : UIEnterOut
{
    public override void ResetP()
    {
        base.ResetP();
        World.instance.CM.ClearDescription();
    }

      public override void ResetRP()
    {
        base.ResetRP();
        World.instance.CM.ShowDescription();
    }


}