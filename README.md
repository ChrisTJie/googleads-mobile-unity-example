# Google Mobile Ads Example for Unity
## Features
- Easy to use
- Operation interface
- Ad formats support:
  - `Banner`
  - `Interstitial`
  - `Rewarded`
  - `Rewarded interstitial`
  - (Optional) `Native Ads Advanced (Unified)`
## Environment
- Unity 2020.3.11f1
- External Dependency Manager for Unity v1.2.165
- Google Mobile Ads Unity Plugin v6.0.0
- (Optional) Google Mobile Ads Unity Plugin v5.4.0.90
  - **Contains Native Ads Advanced (Unified) functions**
## Requirements
- Unity 2020 (LTS) and above.
- Google Mobile Ads Unity Plugin v6.0.0
- (Optional) Native ads build of the Unity plugin.
  - **Require native ads build of the Google Mobile Ads Unity plugin to support Native Ads Advanced.**
- ~~Mediation Test Suite plugin.~~
  - **Currently Google Mobile Ads Mediation Test Suite does not support Google Mobile Ads Unity Plugin v6.0.0**

## Installation
1. [Download and Import Package.](https://github.com/ChrisTJie/googleads-mobile-unity-example/releases)
2. Adding an Ads Object to the scene.

![2021-06-02-142858](https://user-images.githubusercontent.com/79248930/120434305-e0500b00-c3ae-11eb-9e18-009fa85ae6e6.jpg)
## Interface
- `Banner`
- `Interstitial`
- `Rewarded`
- `Rewarded interstitial`

![image](https://user-images.githubusercontent.com/79248930/120754013-a7df3700-c53e-11eb-88e7-14871610661c.png)
- (Optional) `Native Ads Advanced (Unified)`

![image](https://user-images.githubusercontent.com/79248930/120755532-aa429080-c540-11eb-8bd9-c3da5a7ee7c7.png)
## Example Code
- Manually request ads.
```csharp
CTJ.Ads.Instance.RequestBanner();
CTJ.Ads.Instance.RequestInterstitial();
CTJ.Ads.Instance.RequestRewarded();
CTJ.Ads.Instance.RequestRewardedInterstitial();
CTJ.Ads.Instance.RequestNative(); // Optional
CTJ.Ads.Instance.ForceRequestNative(); // Optional
```
- Show ads.
```csharp
CTJ.Ads.Instance.ShowBannerAd();
CTJ.Ads.Instance.ShowInterstitialAd();
CTJ.Ads.Instance.ShowRewardedAd();
CTJ.Ads.Instance.ShowRewardedInterstitialAd();
CTJ.Ads.Instance.ShowMediationTestSuite(); // Optional
```
- Destroy ads.
```csharp
CTJ.Ads.Instance.DestroyBannerAd();
CTJ.Ads.Instance.DestroyInterstitialAd();
CTJ.Ads.Instance.DestroyUnifiedNativeAd(); // Optional
```
- (Optional) Initialize native ads.
```csharp
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

    private void Start()
    {
        _NativeObject.SetActive(false);
        CTJ.Ads.Instance.EventAddListener_NativeInitialize(DemoNativeInitialize);
    }

    public void DemoNativeInitialize()
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
```
