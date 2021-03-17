// <copyright company="Google" file="IMediationTestClient.cs"> Copyright (C) 2017 Google, Inc. </copyright>
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

namespace GoogleMobileAdsMediationTestSuite.Common
{
    using System;
    using GoogleMobileAds.Api;

    public interface IMediationTestClient
    {
        /// <summary>
        /// Occurs when mediation test suite is dismissed.
        /// </summary>
        event EventHandler<EventArgs> OnMediationTestSuiteDismissed;

        /// <summary>
        /// Set the base Ad Request (optionally configured with extras) for the
        /// test suite to use.
        /// </summary>
        AdRequest AdRequest { set; }

        /// <summary>
        /// Show the Mediation Test Tool. Requires Google Mobile Ads application ID.
        /// </summary>
        void Show(string appId);

        /// <summary>
        /// Show the Mediation Test Suite.
        /// </summary>
        void Show();

        /// <summary>
        /// Show the Mediation Test Suite for Ad Manager.
        /// </summary>
        void ShowForAdManager();
    }
}
