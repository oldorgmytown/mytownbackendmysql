using System;
using System.Security.Cryptography;
using System.Text;

namespace mytown.Helpers
{
    /// <summary>
    /// Generates the X-Cf-Signature header required by Cashfree when calling
    /// APIs from a dynamic IP (i.e. without static-IP whitelisting).
    ///
    /// IMPORTANT: Verification Suite and Payouts use SEPARATE public keys.
    /// Do not use the same PEM for both products - each is generated
    /// independently from its own dashboard section.
    ///
    /// The signature is valid for a short window (Cashfree specifies ~10
    /// minutes for Verification Suite; Payouts tokens generated via
    /// /authorize are valid for 6 minutes). Always generate it fresh,
    /// immediately before the HTTP call - never cache or reuse it.
    /// </summary>
    public static class CashfreeSignatureHelper
    {
        public static string GenerateSignature(string clientId, string publicKeyPem)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("clientId is required to generate a Cashfree signature.", nameof(clientId));

            if (string.IsNullOrWhiteSpace(publicKeyPem))
                throw new InvalidOperationException(
                    "Cashfree public key is not configured. Check CashfreePublicKey / CashfreePayoutPublicKey in configuration.");

            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());

            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string dataToEncrypt = $"{clientId}.{unixTimestamp}";

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToEncrypt);

            // Cashfree requires OAEP padding with SHA1 for both the hash
            // algorithm and MGF1 - do not change this to SHA256 or PKCS1.
            byte[] encrypted = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA1);

            return Convert.ToBase64String(encrypted);
        }
    }
}