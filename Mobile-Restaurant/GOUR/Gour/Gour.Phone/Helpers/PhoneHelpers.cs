namespace Gour.Phone.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Net;
    using Microsoft.Phone.Shell;

    public static class PhoneHelpers
    {
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

        public static bool ContainsApplicationState(string key)
        {
            return PhoneApplicationService.Current.State.ContainsKey(key);
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