# GetStoredCredentials
Console application to check for credentials stored in the clear in Windows Credential Manager.

## Implementation details
Enumerates the credentials using the Windows Credential Manager API, and checks for any that have a Password value
with only plaintext ASCII characters. If a password was encrypted properly (using the API's Protect mechanism), it will
contain Unicode characters; otherwise, it is probably not encrypted.

By default, the program will *not* print the passwords to the screen. If you want to check the passwords to see if they
are, in fact, in the clear, and not just Base64-encoded encrypted by some external process, pass the -ShowPasswords
parameter.

To attempt Base64 decoding, pass the -Decode parameter, and the program will do its best.

# Syntax
GetStoredCredentials [OPTIONS]

Options:
-Decode
    Attempts to Base64 decode the password value prior to printing it to the console. -ShowPasswords must also be set!

-ShowPasswords
    Prints the values of the passwords that were found to be in the clear
    
-help
    This text
