using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField] private RawImage _RawImage;

    [SerializeField] private GameObject _AdChoicesLogo;
    [SerializeField] private GameObject _Advertiser;
    [SerializeField] private GameObject _Body;
    [SerializeField] private GameObject _CallToAction;
    [SerializeField] private GameObject _Headline;
    [SerializeField] private GameObject _Icon;

    private void Start()
    {
        CTJ.Ads.RegisterAdChoicesLogo(_AdChoicesLogo);
        CTJ.Ads.RegisterAdvertiser(_Advertiser);
        CTJ.Ads.RegisterBody(_Body);
        CTJ.Ads.RegisterCallToAction(_CallToAction);
        CTJ.Ads.RegisterHeadline(_Headline);
        CTJ.Ads.RegisterIcon(_Icon);
    }

    private void Update()
    {

    }

    // 自定義函式
    public void Test()
    {
        CTJ.Logger.Log("外部調用成功");
    }
}