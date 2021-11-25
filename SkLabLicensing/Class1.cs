using System;
using System.Collections.Generic;
using System.Linq;
using ThinkSharp.Licensing;
using ThinkSharp.Licensing.Helper;
using ThinkSharp.Licensing.Signing;
using DeviceId;

namespace SkLabLicensing
{
    public class LicenseBuilder
    {
        SigningKeyPair RsaKeyPair;
        public KeyValuePair<string, string> GenerateAndSetKeyPair()
        {
            RsaKeyPair = CreateKeyPair();
            return new KeyValuePair<string, string>(RsaKeyPair.PublicKey, RsaKeyPair.PrivateKey);
        }
        public void SetRsaKeyPair(string publicKey, string privateKey)
        {
            RsaKeyPair = new SigningKeyPair(publicKey, privateKey);
        }
        static SigningKeyPair CreateKeyPair()
        {
            return Lic.KeyGenerator.GenerateRsaKeyPair();
        }
        public static string GenerateHardwareID()
        {
            return new DeviceIdBuilder()
               .AddProcessorId()
               .AddProcessorName()
               .AddMotherboardInfo("Manufacturer")
               .AddMotherboardInfo("Product")
               .AddMotherboardInfo("Version")
               .AddMacAddress(true, true)
               .ToString();
        }
        public SignedLicense GenerateLicense(string privateKey, string deviceId, string name, string email, string company)
        {
            return Lic.Builder
                    .WithRsaPrivateKey(privateKey)                                    
                    .WithHardwareIdentifier(deviceId)
                    .WithSerialNumber(SerialNumber.Create("50L"))                   
                    .WithoutExpiration()                                
                    .WithProperty("Name", name)
                    .WithProperty("Company", company)
                    .WithProperty("Email", email)
                    .SignAndCreate();
        }
        public SignedLicense GenerateLicense(string privateKey, string deviceId, string name, string email, string company, DateTime expirationDate)
        {
            return Lic.Builder
                    .WithRsaPrivateKey(privateKey)
                    .WithHardwareIdentifier(deviceId)
                    .WithSerialNumber(SerialNumber.Create("50L"))
                    .ExpiresOn(expirationDate)
                    .WithProperty("Name", name)
                    .WithProperty("Company", company)
                    .WithProperty("Email", email)
                    .SignAndCreate();
        }
        public SignedLicense GenerateLicense(string privateKey, string deviceId, string name, string email, string company, TimeSpan expirationPeriod)
        {
            return Lic.Builder
                    .WithRsaPrivateKey(privateKey)
                    .WithHardwareIdentifier(deviceId)
                    .WithSerialNumber(SerialNumber.Create("50L"))
                    .ExpiresIn(expirationPeriod)
                    .WithProperty("Name", name)
                    .WithProperty("Company", company)
                    .WithProperty("Email", email)
                    .SignAndCreate();
        }

        public bool VerifyLicense(string license, string publicKey, string deviceId)
        {
            try
            {
                var verify = Lic.Verifier
                    .WithRsaPublicKey(publicKey)
                    .WithApplicationCode("50L")
                    .LoadAndVerify(license);

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
