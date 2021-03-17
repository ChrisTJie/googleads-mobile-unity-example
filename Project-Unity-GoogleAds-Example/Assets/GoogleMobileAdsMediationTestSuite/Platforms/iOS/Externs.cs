// <copyright company="Google" file="Externs.cs"> Copyright (C) 2017 Google, Inc. </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_IOS

namespace GoogleMobileAdsMediationTestSuite.iOS
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Externs used by the iOS component.
    /// </summary>
    internal class Externs
    {
        #region Common externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUShowMediationTestSuiteWithAppID(string appID, IntPtr mediationClient);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUShowMediationTestSuite(IntPtr mediationClient);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUShowMediationTestSuiteForAdManager(IntPtr mediationClient);

        [DllImport("__Internal")]
        internal static extern void GADUMRelease(IntPtr obj);

        [DllImport("__Internal")]
        internal static extern void GADUMSetMediationClientCallback(
            IntPtr client,
            MediationTestClient.GADUMediationTestSuiteDidDismissScreenCallback screenDismissedCallback);

        [DllImport("__Internal")]
        internal static extern void GADUMSetAdRequest(IntPtr request);

        #endregion
    }
}

#endif
