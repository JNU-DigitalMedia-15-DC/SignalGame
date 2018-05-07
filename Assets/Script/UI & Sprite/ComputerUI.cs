using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ComputerUI : UIFade
{
    public override void ResetT()
    {
        base.ResetT();
        World.instance.CM.ClearDescription();
    }

      public override void ResetRT()
    {
        base.ResetRT();
        //World.instance.CM.UpdateDescription();
        World.instance.CM.InitiateEmail();
    }


}