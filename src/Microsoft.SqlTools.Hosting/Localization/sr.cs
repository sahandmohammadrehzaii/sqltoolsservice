// WARNING:
// This file was generated by the Microsoft DataWarehouse String Resource Tool 5.0.0.0
// from information in sr.strings
// DO NOT MODIFY THIS FILE'S CONTENTS, THEY WILL BE OVERWRITTEN
//
namespace Microsoft.SqlTools.Hosting
{
    using System;
    using System.Reflection;
    using System.Resources;
    using System.Globalization;

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR
    {
        protected SR()
        { }

        public static CultureInfo Culture
        {
            get
            {
                return Keys.Culture;
            }
            set
            {
                Keys.Culture = value;
            }
        }


        public static string CredentialsServiceInvalidCriticalHandle
        {
            get
            {
                return Keys.GetString(Keys.CredentialsServiceInvalidCriticalHandle);
            }
        }

        public static string CredentialsServicePasswordLengthExceeded
        {
            get
            {
                return Keys.GetString(Keys.CredentialsServicePasswordLengthExceeded);
            }
        }

        public static string CredentialsServiceTargetForDelete
        {
            get
            {
                return Keys.GetString(Keys.CredentialsServiceTargetForDelete);
            }
        }

        public static string CredentialsServiceTargetForLookup
        {
            get
            {
                return Keys.GetString(Keys.CredentialsServiceTargetForLookup);
            }
        }

        public static string CredentialServiceWin32CredentialDisposed
        {
            get
            {
                return Keys.GetString(Keys.CredentialServiceWin32CredentialDisposed);
            }
        }

        public static string ServiceAlreadyRegistered
        {
            get
            {
                return Keys.GetString(Keys.ServiceAlreadyRegistered);
            }
        }

        public static string MultipleServicesFound
        {
            get
            {
                return Keys.GetString(Keys.MultipleServicesFound);
            }
        }

        public static string IncompatibleServiceForExtensionLoader
        {
            get
            {
                return Keys.GetString(Keys.IncompatibleServiceForExtensionLoader);
            }
        }

        public static string ServiceProviderNotSet
        {
            get
            {
                return Keys.GetString(Keys.ServiceProviderNotSet);
            }
        }

        public static string ServiceNotFound
        {
            get
            {
                return Keys.GetString(Keys.ServiceNotFound);
            }
        }

        public static string ServiceNotOfExpectedType
        {
            get
            {
                return Keys.GetString(Keys.ServiceNotOfExpectedType);
            }
        }

        public static string HostingUnexpectedEndOfStream
        {
            get
            {
                return Keys.GetString(Keys.HostingUnexpectedEndOfStream);
            }
        }

        public static string HostingHeaderMissingColon
        {
            get
            {
                return Keys.GetString(Keys.HostingHeaderMissingColon);
            }
        }

        public static string HostingHeaderMissingContentLengthHeader
        {
            get
            {
                return Keys.GetString(Keys.HostingHeaderMissingContentLengthHeader);
            }
        }

        public static string HostingHeaderMissingContentLengthValue
        {
            get
            {
                return Keys.GetString(Keys.HostingHeaderMissingContentLengthValue);
            }
        }

        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class Keys
        {
            static ResourceManager resourceManager = new ResourceManager("Microsoft.SqlTools.Hosting.Localization.SR", typeof(SR).GetTypeInfo().Assembly);

            static CultureInfo _culture = null;


            public const string CredentialsServiceInvalidCriticalHandle = "CredentialsServiceInvalidCriticalHandle";


            public const string CredentialsServicePasswordLengthExceeded = "CredentialsServicePasswordLengthExceeded";


            public const string CredentialsServiceTargetForDelete = "CredentialsServiceTargetForDelete";


            public const string CredentialsServiceTargetForLookup = "CredentialsServiceTargetForLookup";


            public const string CredentialServiceWin32CredentialDisposed = "CredentialServiceWin32CredentialDisposed";


            public const string ServiceAlreadyRegistered = "ServiceAlreadyRegistered";


            public const string MultipleServicesFound = "MultipleServicesFound";


            public const string IncompatibleServiceForExtensionLoader = "IncompatibleServiceForExtensionLoader";


            public const string ServiceProviderNotSet = "ServiceProviderNotSet";


            public const string ServiceNotFound = "ServiceNotFound";


            public const string ServiceNotOfExpectedType = "ServiceNotOfExpectedType";


            public const string HostingUnexpectedEndOfStream = "HostingUnexpectedEndOfStream";


            public const string HostingHeaderMissingColon = "HostingHeaderMissingColon";


            public const string HostingHeaderMissingContentLengthHeader = "HostingHeaderMissingContentLengthHeader";


            public const string HostingHeaderMissingContentLengthValue = "HostingHeaderMissingContentLengthValue";


            private Keys()
            { }

            public static CultureInfo Culture
            {
                get
                {
                    return _culture;
                }
                set
                {
                    _culture = value;
                }
            }

            public static string GetString(string key)
            {
                return resourceManager.GetString(key, _culture);
            }

        }
    }
}
