// <copyright file="Program.cs" company="Peter Rosser">
// Copyright (c) Peter Rosser. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeterRosser.GetStoredCredentials
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    using static FormattableString;

    internal class Program
    {
        private const string ArgDecode = "decode";
        private const string ArgHelp = "help";
        private const string ArgHelpAlt = "?";
        private const string ArgHelpAlt2 = "h";
        private const string ArgShowPasswords = "showpasswords";
        private static readonly string[] ArgsHelp = {ArgHelp, ArgHelpAlt, ArgHelpAlt2};

        private static string AddBase64Padding(string text)
        {
            return (text + "==").Substring(0, 3 * text.Length % 4);
        }

        private static string Base64FromWebSafe(string base64)
        {
            if (base64.Contains("-") || base64.Contains("/"))
            {
                return AddBase64Padding(base64);
            }

            return AddBase64Padding(base64.Replace('-', '+').Replace('_', '/'));
        }

        private static string GetPassword(Credential cred, bool decode)
        {
            return decode ? TryDecodePassword(cred) : cred.Password;
        }

        private static bool IsProbablyClearText(string text)
        {
            return !string.IsNullOrEmpty(text) && text.All(c => c >= '!' && c <= 0xa0);
        }

        private static void Main(string[] args)
        {
            var showPasswords = false;
            var decode = false;
            foreach (string arg in args)
            {
                string trimmed = arg.TrimStart('-', '/');
                if (string.Equals(trimmed, ArgShowPasswords, StringComparison.OrdinalIgnoreCase))
                {
                    showPasswords = true;
                }
                else if (string.Equals(trimmed, ArgDecode, StringComparison.OrdinalIgnoreCase))
                {
                    decode = true;
                }
                else if (ArgsHelp.Any(x => string.Equals(trimmed, x, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine(Invariant($@"GetStoredCredentials [OPTIONS]

Options:
-{ArgDecode}
    Attempts to Base64 decode the password value prior to printing it to the
    console. -ShowPasswords must also be set!

-{ArgShowPasswords}
    Prints the values of the passwords that were found to be in the clear

-{ArgHelp}
    This text
"));
                    return;
                }
            }

            IReadOnlyList<Credential> creds = CredentialManager.EnumerateCrendentials();

            var count = 0;
            Console.WriteLine(Invariant($"Checking for credentials stored in the clear"));
            foreach (Credential cred in creds)
            {
                if (IsProbablyClearText(cred.Password))
                {
                    ++count;
                    Warn(Invariant($"Application: {cred.ApplicationName}"));
                    Warn(Invariant($"Type:        {cred.CredentialType}"));
                    Warn(Invariant($"Username:    {cred.UserName}"));
                    if (showPasswords)
                    {
                        Warn(Invariant($"Password:    {GetPassword(cred, decode)}"));
                    }

                    Warn(string.Empty);
                }
            }

            if (count > 0)
            {
                Warn(string.Empty);
                Warn(Invariant($"{count} passwords were probably in the clear."));
                if (!showPasswords)
                {
                    Warn(string.Empty);
                    Warn(Invariant($"Run again with -showpasswords to print the passwords to the console."));
                }
            }
            else
            {
                Success(Invariant($"No credentials were detected to be obviously in the clear. :)"));
            }
        }

        private static void Print(string message, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void Success(string message)
        {
            Print(message, ConsoleColor.Green);
        }

        private static string TryDecodePassword(Credential cred)
        {
            try
            {
                byte[] decoded = Convert.FromBase64String(Base64FromWebSafe(cred.Password));

                foreach (Encoding encoding in new[]
                {
                    Encoding.UTF8,
                    Encoding.Unicode,
                    Encoding.ASCII,
                    Encoding.BigEndianUnicode,
                    Encoding.UTF32,
                    Encoding.UTF7
                })
                {
                    string test = encoding.GetString(decoded);
                    if (IsProbablyClearText(test))
                    {
                        return test;
                    }
                }
            }
            catch (FormatException)
            {
                // not obviously base64-encoded
            }

            return cred.Password;
        }

        private static void Warn(string message)
        {
            Print(message, ConsoleColor.Yellow);
        }
    }
}