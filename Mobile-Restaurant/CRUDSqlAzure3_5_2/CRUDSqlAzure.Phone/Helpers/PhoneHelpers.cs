// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using Microsoft.Phone.Shell;
    using SL.Phone.Federation.Utilities;

    public static class PhoneHelpers
    {
        private const long ExpirationBuffer = 10;

        public static bool IsValid(this RequestSecurityTokenResponseStore rstrStore)
        {
            var epoch = (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
            if ((rstrStore != null) && rstrStore.ContainsValidRequestSecurityTokenResponse())
            {
                return rstrStore.RequestSecurityTokenResponse.expires > epoch + ExpirationBuffer;
            }

            return false;
        }

        public static T GetApplicationState<T>(string key)
        {
            if (!PhoneApplicationService.Current.State.ContainsKey(key))
            {
                return default(T);
            }

            return (T)PhoneApplicationService.Current.State[key];
        }

        public static void SetApplicationState(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                PhoneApplicationService.Current.State.Remove(key);
            }

            PhoneApplicationService.Current.State.Add(key, value);
        }

        public static void RemoveApplicationState(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
            {
                PhoneApplicationService.Current.State.Remove(key);
            }
        }

        public static T GetIsolatedStorageSetting<T>(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (!isolatedStorage.ContainsKey(key))
            {
                return default(T);
            }

            return (T)isolatedStorage[key];
        }

        public static void SetIsolatedStorageSetting(string key, object value)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (isolatedStorage.ContainsKey(key))
            {
                isolatedStorage.Remove(key);
            }

            isolatedStorage.Add(key, value);
        }
        
        public static void RemoveIsolatedStorageSetting(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (isolatedStorage.ContainsKey(key))
            {
                isolatedStorage.Remove(key);
            }
        }
    }
}