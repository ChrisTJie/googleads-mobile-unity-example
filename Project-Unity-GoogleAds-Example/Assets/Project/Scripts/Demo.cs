using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField] private GameObject _AdChoicesLogo;
    [SerializeField] private GameObject _Advertiser;
    [SerializeField] private GameObject _Body;
    [SerializeField] private GameObject _CallToAction;
    [SerializeField] private GameObject _Headline;
    [SerializeField] private GameObject _Icon;

    // 自定義函式
    public void Test()
    {
        _AdChoicesLogo.GetComponent<RawImage>().texture = CTJ.Ads.GetAdChoicesLogo;
        _Advertiser.GetComponent<Text>().text = CTJ.Ads.GetAdvertiser;
        _Body.GetComponent<Text>().text = CTJ.Ads.GetBody;
        _CallToAction.GetComponent<Text>().text = CTJ.Ads.GetCallToAction;
        _Headline.GetComponent<Text>().text = CTJ.Ads.GetHeadline;
        _Icon.GetComponent<RawImage>().texture = CTJ.Ads.GetIcon;
        CTJ.Ads.RegisterAdChoicesLogo(_AdChoicesLogo);
        CTJ.Ads.RegisterAdvertiser(_Advertiser);
        CTJ.Ads.RegisterBody(_Body);
        CTJ.Ads.RegisterCallToAction(_CallToAction);
        CTJ.Ads.RegisterHeadline(_Headline);
        CTJ.Ads.RegisterIcon(_Icon);
    }
}