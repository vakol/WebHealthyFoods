using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace WebHealthyFoods.Utility
{
    
    /**
     * Windows Credential Manager.
     */
    public static class WindowsCredentialManager
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CredRead(string target, int type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern void CredFree([In] IntPtr buffer);

        private const int CRED_TYPE_GENERIC = 1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDENTIAL
        {
            public int Flags;
            public int Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public int CredentialBlobSize;
            public IntPtr CredentialBlob;
            public int Persist;
            public int AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        /**
         * Get password from Windows Credential Manager.
         * target: The name of the credential.
         * pwd: The char array to store the password.
         * return: true if success, false otherwise.
         */
        public static bool GetCredential(string target, out string userName, out char[] pwd)
        {
            userName = null;
            pwd = null;
            IntPtr credPtr;
            if (CredRead(target, CRED_TYPE_GENERIC, 0, out credPtr))
            {
                var cred = (CREDENTIAL)Marshal.PtrToStructure(credPtr, typeof(CREDENTIAL));
                userName = (cred.UserName != IntPtr.Zero ? Marshal.PtrToStringUni(cred.UserName) : null);
                if (userName == null || userName == "")
                {
                    CredFree(credPtr);
                    return false;
                }
                string password = "";
                if (cred.CredentialBlob != IntPtr.Zero)
                {
                    password = Marshal.PtrToStringUni(cred.CredentialBlob, cred.CredentialBlobSize / 2);
                    int pwdLength = password.Length;

                    // Copy password to pwd array.
                    if (pwdLength > 256)
                    {
                        CredFree(credPtr);
                        return false;
                    }
                    pwd = new char[pwdLength];
                    password.CopyTo(0, pwd, 0, pwdLength);
                }
                CredFree(credPtr);
                return true;
            }
            return false;
        }

        /**
         * Entropy for DPAPI.
         */
        private static byte[] entropy = [ 23, 65, 87, 43, 25, 26, 6, 43, 95, 11, 53, 50 ];

        /**
         * Protect secret with DPAPI.
         */
        public static bool Protect(string secret, out string encryptedBase64)
        {
            try
            {
                // Convert string to bytes.
                byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

                // Encrypt with DPAPI (machine scope).
                byte[] encrypted = ProtectedData.Protect(
                    secretBytes,
                    entropy,
                    DataProtectionScope.LocalMachine
                );

                // Convert to Base64 for easy storage.
                encryptedBase64 = Convert.ToBase64String(encrypted);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Protect secret failed: " + e.Message);
                encryptedBase64 = null;
                return false;
            }
        }

        /**
         * Unprotect secret with DPAPI.
         */
        public static bool Unprotect(string encryptedBase64, out char [] decryptedChars)
        {
            try
            {
                // Convert string from Base64 to bytes.
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

                // Decrypt bytes with DPAPI (machine scope).
                byte[] decrypted = ProtectedData.Unprotect(
                    encryptedBytes,
                    entropy,
                    DataProtectionScope.LocalMachine
                );

                // Convert bytes to output character array.
                string descrypotedText = Encoding.UTF8.GetString(decrypted);
                decryptedChars = descrypotedText.ToCharArray();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unprotect secret failed: " + e.Message);
                decryptedChars = null;
                return false;
            }
        }
    }   
 }