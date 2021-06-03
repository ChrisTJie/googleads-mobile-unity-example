using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private void Start()
    {
        CTJ.Logger.Log(CTJ.Ads.Instance.GetGDPRApplicable);
    }
}