using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Easy.Common
{
    public class LicenseException : Exception
    {
        public LicenseException(string message) : base(message) { }
    }




    /// <summary>
    /// Public Code API to register commercial license for ServiceStack.
    /// </summary>
    public static class Licensing
    {
        public static void RegisterLicense(string licenseKeyText)
        {
            LicenseUtils.RegisterLicense(licenseKeyText);
        }

        public static void RegisterLicenseFromFile(string filePath)
        {
            if (!filePath.FileExists())
                throw new LicenseException("License file does not exist: " + filePath).Trace();

            var licenseKeyText = filePath.ReadAllText();
            LicenseUtils.RegisterLicense(licenseKeyText);
        }

        public static void RegisterLicenseFromFileIfExists(string filePath)
        {
            if (!filePath.FileExists())
                return;

            var licenseKeyText = filePath.ReadAllText();
            LicenseUtils.RegisterLicense(licenseKeyText);
        }
    }

    public class LicenseKey
    {
        public string Ref { get; set; }
        public string Name { get; set; }
        public EnumLicenseType Type { get; set; }
        public string Hash { get; set; }
        public DateTime Expiry { get; set; }
    }

    /// <summary>
    /// Internal Utilities to verify licensing
    /// </summary>
    public static class LicenseUtils
    {
        public const string RuntimePublicKey = "<RSAKeyValue><Modulus>nkqwkUAcuIlVzzOPENcQ+g5ALCe4LyzzWv59E4a7LuOM1Nb+hlNlnx2oBinIkvh09EyaxIX2PmaY0KtyDRIh+PoItkKeJe/TydIbK/bLa0+0Axuwa0MFShE6HdJo/dynpODm64+Sg1XfhICyfsBBSxuJMiVKjlMDIxu9kDg7vEs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public const string LicensePublicKey = "<RSAKeyValue><Modulus>w2fTTfr2SrGCclwLUkrbH0XsIUpZDJ1Kei2YUwYGmIn5AUyCPLTUv3obDBUBFJKLQ61Khs7dDkXlzuJr5tkGQ0zS0PYsmBPAtszuTum+FAYRH4Wdhmlfqu1Z03gkCIo1i11TmamN5432uswwFCVH60JU3CpaN97Ehru39LA1X9E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string ContactDetails = " Please see servicestack.net or contact team@servicestack.net for more details.";

        static LicenseUtils()
        {
            PclExport.Instance.RegisterLicenseFromConfig();
        }

        private static bool hasInit;
        public static void Init()
        {
            hasInit = true; //Dummy method to init static constructor
        }

        public static class ErrorMessages
        {
            private const string UpgradeInstructions = " Please see https://servicestack.net to upgrade to a commercial license or visit https://github.com/ServiceStackV3/ServiceStackV3 to revert back to the free ServiceStack v3.";
            internal const string ExceededTextTypes = "The free-quota limit on '{0} ServiceStack.Text Types' has been reached." + UpgradeInstructions;
            internal const string ExceededRedisTypes = "The free-quota limit on '{0} Redis Types' has been reached." + UpgradeInstructions;
            internal const string ExceededRedisRequests = "The free-quota limit on '{0} Redis requests per hour' has been reached." + UpgradeInstructions;
            internal const string ExceededOrmLiteTables = "The free-quota limit on '{0} OrmLite Tables' has been reached." + UpgradeInstructions;
            internal const string ExceededServiceStackOperations = "The free-quota limit on '{0} ServiceStack Operations' has been reached." + UpgradeInstructions;
            internal const string ExceededAdminUi = "The Admin UI is a commerical-only premium feature." + UpgradeInstructions;
            internal const string ExceededPremiumFeature = "Unauthorized use of a commerical-only premium feature." + UpgradeInstructions;
            public const string UnauthorizedAccessRequest = "Unauthorized access request of a licensed feature.";
        }

        public static class FreeQuotas
        {
            public const int ServiceStackOperations = 10;
            public const int TypeFields = 20;
            public const int TextTypes = 20;
            public const int RedisTypes = 20;
            public const int RedisRequestPerHour = 6000;
            public const int OrmLiteTables = 10;
            public const int PremiumFeature = 0;
        }

        public static void AssertEvaluationLicense()
        {
            if (DateTime.UtcNow > new DateTime(2013, 12, 31))
                throw new LicenseException("The evaluation license for this software has expired. " +
                    "See https://servicestack.net to upgrade to a valid license.").Trace();
        }

        private static LicenseKey __activatedLicense;
        public static void RegisterLicense(string licenseKeyText)
        {
            string cutomerId = null;
            try
            {
                var parts = licenseKeyText.SplitOnFirst('-');
                cutomerId = parts[0];

                LicenseKey key;
                using (new AccessToken(EnumLicenseFeature.Text))
                {
                    key = PclExport.Instance.VerifyLicenseKeyText(licenseKeyText);
                }

                var releaseDate = Env.GetReleaseDate();
                if (releaseDate > key.Expiry)
                    throw new LicenseException("This license has expired on {0} and is not valid for use with this release."
                        .Fmt(key.Expiry.ToString("d")) + ContactDetails).Trace();

                __activatedLicense = key;
            }
            catch (Exception ex)
            {
                if (ex is LicenseException)
                    throw;

                var msg = "This license is invalid." + ContactDetails;
                if (!string.IsNullOrEmpty(cutomerId))
                    msg += " The id for this license is '{0}'".Fmt(cutomerId);

                throw new LicenseException(msg).Trace();
            }
        }

        public static void RemoveLicense()
        {
            __activatedLicense = null;
        }

        public static EnumLicenseFeature ActivatedLicenseFeatures()
        {
            return __activatedLicense != null ? __activatedLicense.GetLicensedFeatures() : EnumLicenseFeature.None;
        }

        public static void ApprovedUsage(EnumLicenseFeature licenseFeature, EnumLicenseFeature requestedFeature,
            int allowedUsage, int actualUsage, string message)
        {
            var hasFeature = (requestedFeature & licenseFeature) == requestedFeature;
            if (hasFeature)
                return;

            if (actualUsage > allowedUsage)
                throw new LicenseException(message.Fmt(allowedUsage)).Trace();
        }

        public static bool HasLicensedFeature(EnumLicenseFeature feature)
        {
            var licensedFeatures = ActivatedLicenseFeatures();
            return (feature & licensedFeatures) == feature;
        }

        public static void AssertValidUsage(EnumLicenseFeature feature, EnumQuotaType quotaType, int count)
        {
            var licensedFeatures = ActivatedLicenseFeatures();
            if ((EnumLicenseFeature.All & licensedFeatures) == EnumLicenseFeature.All) //Standard Usage
                return;

            if (AccessTokenScope != null)
            {
                if ((feature & AccessTokenScope.tempFeatures) == feature)
                    return;
            }

            //Free Quotas
            switch (feature)
            {
                case EnumLicenseFeature.Text:
                    switch (quotaType)
                    {
                        case EnumQuotaType.Types:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.TextTypes, count, ErrorMessages.ExceededTextTypes);
                            return;
                    }
                    break;

                case EnumLicenseFeature.Redis:
                    switch (quotaType)
                    {
                        case EnumQuotaType.Types:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.RedisTypes, count, ErrorMessages.ExceededRedisTypes);
                            return;
                        case EnumQuotaType.RequestsPerHour:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.RedisRequestPerHour, count, ErrorMessages.ExceededRedisRequests);
                            return;
                    }
                    break;

                case EnumLicenseFeature.OrmLite:
                    switch (quotaType)
                    {
                        case EnumQuotaType.Tables:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.OrmLiteTables, count, ErrorMessages.ExceededOrmLiteTables);
                            return;
                    }
                    break;

                case EnumLicenseFeature.ServiceStack:
                    switch (quotaType)
                    {
                        case EnumQuotaType.Operations:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.ServiceStackOperations, count, ErrorMessages.ExceededServiceStackOperations);
                            return;
                    }
                    break;

                case EnumLicenseFeature.Admin:
                    switch (quotaType)
                    {
                        case EnumQuotaType.PremiumFeature:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.PremiumFeature, count, ErrorMessages.ExceededAdminUi);
                            return;
                    }
                    break;

                case EnumLicenseFeature.Premium:
                    switch (quotaType)
                    {
                        case EnumQuotaType.PremiumFeature:
                            ApprovedUsage(licensedFeatures, feature, FreeQuotas.PremiumFeature, count, ErrorMessages.ExceededPremiumFeature);
                            return;
                    }
                    break;
            }

            throw new LicenseException("Unknown Quota Usage: {0}, {1}".Fmt(feature, quotaType)).Trace();
        }

        public static EnumLicenseFeature GetLicensedFeatures(this LicenseKey key)
        {
            switch (key.Type)
            {
                case EnumLicenseType.Free:
                    return EnumLicenseFeature.Free;

                case EnumLicenseType.Indie:
                case EnumLicenseType.Business:
                case EnumLicenseType.Enterprise:
                    return EnumLicenseFeature.All;

                case EnumLicenseType.TextIndie:
                case EnumLicenseType.TextBusiness:
                    return EnumLicenseFeature.Text;

                case EnumLicenseType.OrmLiteIndie:
                case EnumLicenseType.OrmLiteBusiness:
                    return EnumLicenseFeature.OrmLiteSku;

                case EnumLicenseType.RedisIndie:
                case EnumLicenseType.RedisBusiness:
                    return EnumLicenseFeature.RedisSku;
            }
            throw new ArgumentException("Unknown License Type: " + key.Type).Trace();
        }

        public static LicenseKey ToLicenseKey(this string licenseKeyText)
        {
            licenseKeyText = Regex.Replace(licenseKeyText, @"\s+", "");
            var parts = licenseKeyText.SplitOnFirst('-');
            var refId = parts[0];
            var base64 = parts[1];
            var jsv = Convert.FromBase64String(base64).FromUtf8Bytes();
            var key = jsv.FromJsv<LicenseKey>();

            if (key.Ref != refId)
                throw new LicenseException("The license '{0}' is not assigned to CustomerId '{1}'.".Fmt(base64)).Trace();

            return key;
        }

        public static string GetHashKeyToSign(this LicenseKey key)
        {
            return "{0}:{1}:{2}:{3}".Fmt(key.Ref, key.Name, key.Expiry.ToString("yyyy-MM-dd"), key.Type);
        }

        public static Exception GetInnerMostException(this Exception ex)
        {
            //Extract true exception from static intializers (e.g. LicenseException)
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        [ThreadStatic]
        private static AccessToken AccessTokenScope;
        private class AccessToken : IDisposable
        {
            private readonly AccessToken prevToken;
            internal readonly EnumLicenseFeature tempFeatures;
            internal AccessToken(EnumLicenseFeature requested)
            {
                prevToken = AccessTokenScope;
                AccessTokenScope = this;
                tempFeatures = requested;
            }

            public void Dispose()
            {
                AccessTokenScope = prevToken;
            }
        }

        static class _approved
        {
            internal static readonly HashSet<string> __tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ServiceStack.ServiceClientBase+AccessToken",
                "ServiceStack.RabbitMq.RabbitMqProducer+AccessToken",
                "ServiceStack.Messaging.RedisMessageQueueClient+AccessToken",
                "ServiceStack.Messaging.RedisMessageProducer+AccessToken",
            };

            internal static readonly HashSet<string> __dlls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ServiceStack.Client.dll",
                "ServiceStack.RabbitMq.dll",
                "<Unknown>"
            };
        }

        public static IDisposable RequestAccess(object accessToken, EnumLicenseFeature srcFeature, EnumLicenseFeature requestedAccess)
        {
            var accessType = accessToken.GetType();

            if (srcFeature != EnumLicenseFeature.Client || requestedAccess != EnumLicenseFeature.Text || accessToken == null)
                throw new LicenseException(ErrorMessages.UnauthorizedAccessRequest).Trace();

            if (accessType.Name == "AccessToken" && accessType.GetAssembly().ManifestModule.Name.StartsWith("<")) //Smart Assembly
                return new AccessToken(requestedAccess);

            if (!_approved.__tokens.Contains(accessType.FullName))
            {
                var errorDetails = " __token: '{0}', Assembly: '{1}'".Fmt(
                    accessType.Name,
                    accessType.GetAssembly().ManifestModule.Name);

                throw new LicenseException(ErrorMessages.UnauthorizedAccessRequest + errorDetails).Trace();
            }

            PclExport.Instance.VerifyInAssembly(accessType, _approved.__dlls);

            return new AccessToken(requestedAccess);
        }
    }
}
