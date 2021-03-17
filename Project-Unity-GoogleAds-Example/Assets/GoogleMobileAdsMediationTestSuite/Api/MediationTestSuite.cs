// <copyright company="Google" file="MediationTestSuite.cs"> Copyright (C) 2017 Google, Inc. </copyright>
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

namespace GoogleMobileAdsMediationTestSuite.Api
{
    using System;
    using System.Reflection;
    using GoogleMobileAds.Api;
    using GoogleMobileAdsMediationTestSuite;
    using GoogleMobileAdsMediationTestSuite.Common;

    /// <summary>
    /// Enables interactions with the GoogleMobileAdsMediationTestSuite.
    /// </summary>
    public class MediationTestSuite
    {
        private readonly IMediationTestClient client;
        private static MediationTestSuite instance = new MediationTestSuite();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediationTestSuite"/> class.
        /// </summary>
        private MediationTestSuite()
        {
            this.client = GetMediationTestClient();
            this.client.OnMediationTestSuiteDismissed += this.HandleMediationTestSuiteDismissed;
        }

        /// <summary>
        /// Set this event to be notified when the MediationTestSuite is dimissed.
        /// </summary>
        public static event EventHandler<EventArgs> OnMediationTestSuiteDismissed;

        private static MediationTestSuite Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Shows the mediation test suite.
        /// </summary>
        public static void Show()
        {
            MediationTestSuite.Instance.CallShow();
        }

        /// <summary>
        /// Shows the mediation test suite for Ad Manager.
        /// </summary>
        public static void ShowForAdManager()
        {
            MediationTestSuite.Instance.CallShowForAdManager();
        }

        /// <summary>
        /// Shows the mediation test suite.
        /// </summary>
        /// <param name="appId">AdMob App ID for the app that's making this call</param>
        [System.Obsolete("Use MediationTestSuite.Show() instead", false)]
        public static void Show(string appId)
        {
            MediationTestSuite.Instance.CallShow(appId);
        }

        /// <summary>
        /// Set the base Ad Request (optionally configured with extras) for the test suite to use.
        /// Note that this should only be called when the ad request is configured and before
        /// showing the test suite. Any modifications made to the request after setting or showing
        /// the test suite will not apply.
        /// </summary>
        public static AdRequest AdRequest {
            set
            {
                MediationTestSuite.Instance.AdRequestImpl = value;
            }
        }

        private static IMediationTestClient GetMediationTestClient()
        {
            return MediationTestSuiteClientFactory.MediationTestSuiteInstance();
        }

        private void HandleMediationTestSuiteDismissed(object sender, EventArgs args)
        {
            if (MediationTestSuite.OnMediationTestSuiteDismissed != null)
            {
                MediationTestSuite.OnMediationTestSuiteDismissed(this, args);
            }
        }

        private void CallShow(string appId)
        {
            this.client.Show(appId);
        }

        private void CallShow()
        {
            this.client.Show();
        }

        private void CallShowForAdManager()
        {
            this.client.ShowForAdManager();
        }

        private AdRequest AdRequestImpl {
            set
            {
                this.client.AdRequest = value;
            }
        }
    }
}
