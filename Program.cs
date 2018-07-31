// <copyright file="Program.cs" company="Peter Rosser">
// Copyright (c) Peter Rosser. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace PeterRosser.GetStoredCredentials
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    using static FormattableString;

    internal class Program
    {
        private const string ArgShowPasswords = "showpasswords";
        private const string ArgHelp = "help";
        private const string ArgHelpAlt = "?";
        private const string ArgHelpAlt2 = "h";
        private static readonly string[] ArgsHelp = {ArgHelp, ArgHelpAlt, ArgHelpAlt2};


        private static void Main(string[] args)
        {
            var printPasswords = false;
            foreach (string arg in args)
            {
                string trimmed = arg.TrimStart('-', '/');
                if (string.Equals(trimmed, ArgShowPasswords, StringComparison.OrdinalIgnoreCase))
                {
                    printPasswords = true;
                }
                else if (ArgsHelp.Any(x => string.Equals(trimmed, x, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine(Invariant($@"GetStoredCredentials [OPTIONS]

Options:
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
                if (cred.Password != null && cred.Password.All(c => c >= '!' && c <= 0xa0))
                {
                    ++count;

                    Warn(Invariant(
                        $"{cred.ApplicationName} ({cred.CredentialType}), Username=\"{cred.UserName}\", Password=\"{(printPasswords ? cred.Password : Invariant($"[REDACTED]"))}\""));
                }
            }

            if (count > 0)
            {
                Warn(string.Empty);
                Warn(Invariant($"{count} passwords were probably in the clear."));
                if (!printPasswords)
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

        private static void Warn(string message)
        {
            Print(message, ConsoleColor.Yellow);
        }

        private static void Success(string message)
        {
            Print(message, ConsoleColor.Green);
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
    }
}