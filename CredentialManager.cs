using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace PeterRosser.GetStoredCredentials
{
    internal static class CredentialManager
    {
        public static IReadOnlyList<Credential> EnumerateCrendentials()
        {
            var result = new List<Credential>();
            bool ret = 0 != NativeMethods.CredEnumerate(null, 0, out int count, out IntPtr pCredentials);
            int lastError = Marshal.GetLastWin32Error();
            if (ret)
            {
                for (var n = 0; n < count; n++)
                {
                    IntPtr credential = Marshal.ReadIntPtr(pCredentials, n * Marshal.SizeOf(typeof(IntPtr)));
                    result.Add(ReadCredential((CREDENTIAL) Marshal.PtrToStructure(credential, typeof(CREDENTIAL))));
                }
            }
            else
            {
                throw new Win32Exception(lastError);
            }

            return result;
        }

        private static Credential ReadCredential(CREDENTIAL credential)
        {
            string applicationName = Marshal.PtrToStringUni(credential.TargetName);
            string userName = Marshal.PtrToStringUni(credential.UserName);
            string secret = null;
            if (credential.CredentialBlob != IntPtr.Zero)
            {
                secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int) credential.CredentialBlobSize / 2);
            }

            return new Credential(credential.Type, applicationName, userName, secret);
        }

        private static class NativeMethods
        {
            [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.U4)]
            public static extern uint CredEnumerate(string filter, int flag,
                [MarshalAs(UnmanagedType.I4)] out int count,
                out IntPtr pCredentials);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        // ReSharper disable once InconsistentNaming
        private struct CREDENTIAL
        {
            public readonly uint Flags;
            public readonly CredentialType Type;
            public readonly IntPtr TargetName;
            public readonly IntPtr Comment;
            public readonly FILETIME LastWritten;
            public readonly uint CredentialBlobSize;
            public readonly IntPtr CredentialBlob;
            public readonly uint Persist;
            public readonly uint AttributeCount;
            public readonly IntPtr Attributes;
            public readonly IntPtr TargetAlias;
            public readonly IntPtr UserName;
        }
    }
}