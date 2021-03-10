**************
Version 5.4.0.90
**************

Plugin:
- Add support for iOS14 with Googles `SKAdNetwork` identifiers automatically included in
  `Info.plist`.
- Added the RewardedInterstitialAd format. This feature is currently in private beta. Reach out to your account manager to request access.
- Added mock ad views to enable developers to test ad placement and callback logic within the Unity editor.
- Added fix for crash that occurs when attempting to show interstitial when app is closing.
- Added fix for crash that occurs when calling `GetResponseInfo()` on iOS before ad is loaded.

Built and tested with:
- Google Play services 19.5.0
- Google Mobile Ads iOS SDK 7.68.0
- External Dependency Manager for Unity 1.2.161.

**************
Version 5.3.0.91
**************

Plugin:
- Update to invoke callbacks in Unity update loop.
- Add LoadAd(AdRequest adRequest) API to enable loading ads with custom extras.

Built and tested with:
- Google Play services 19.3.0
- Google Mobile Ads iOS SDK 7.64.0
- External Dependency Manager for Unity 1.2.156.

**************
Version 5.3.0.90
**************

Plugin:
- Initial release of Ad Placements open beta.

Built and tested with:
- Google Play services 19.3.0
- Google Mobile Ads iOS SDK 7.63.0
- External Dependency Manager for Unity 1.2.156.
