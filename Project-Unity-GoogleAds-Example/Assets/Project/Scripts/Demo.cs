using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    [SerializeField] private GameObject _AdChoicesLogo;

    private void Start()
    {
        CTJ.Ads.RegisterAdChoicesLogo(_AdChoicesLogo);
    }
}
