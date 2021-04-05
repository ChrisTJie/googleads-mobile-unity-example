using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    [SerializeField] private GameObject _NativeObject;
    [SerializeField] private GameObject _AdChoicesLogo;
    [SerializeField] private GameObject _Advertiser;
    [SerializeField] private GameObject _Body;
    [SerializeField] private GameObject _CallToAction;
    [SerializeField] private GameObject _Headline;
    [SerializeField] private GameObject _Icon;

    private void Start() => _NativeObject.SetActive(false);

    public void DemoInitializeEventFunction()
    {
        _AdChoicesLogo.GetComponent<RawImage>().texture = CTJ.Ads.Instance.GetAdChoicesLogo;
        _Advertiser.GetComponent<Text>().text = CTJ.Ads.Instance.GetAdvertiser;
        _Body.GetComponent<Text>().text = CTJ.Ads.Instance.GetBody;
        _CallToAction.GetComponent<Text>().text = CTJ.Ads.Instance.GetCallToAction;
        _Headline.GetComponent<Text>().text = CTJ.Ads.Instance.GetHeadline;
        _Icon.GetComponent<RawImage>().texture = CTJ.Ads.Instance.GetIcon;
        CTJ.Ads.Instance.RegisterAdChoicesLogo(_AdChoicesLogo);
        CTJ.Ads.Instance.RegisterAdvertiser(_Advertiser);
        CTJ.Ads.Instance.RegisterBody(_Body);
        CTJ.Ads.Instance.RegisterCallToAction(_CallToAction);
        CTJ.Ads.Instance.RegisterHeadline(_Headline);
        CTJ.Ads.Instance.RegisterIcon(_Icon);
        _NativeObject.SetActive(true);
    }
}