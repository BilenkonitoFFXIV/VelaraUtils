// using System;
//
// namespace VelaraUtils.Utils
// {
//     internal static class CharExtensions
//     {
//         internal static bool IsValidFlag(this char thisChar) =>
//             char.IsLetterOrDigit(thisChar);
//
//         internal static char? Invert(this char thisChar) =>
//             thisChar.IsValidFlag() ?
//                 char.IsLower(thisChar) ?
//                     char.ToUpper(thisChar) :
//                     char.ToLower(thisChar) :
//                 null;
//     }
//
//     internal static class FlagUtils
//     {
//         internal class InvalidFlagException : ArgumentException
//         {
//             internal InvalidFlagException(string? paramName)
//                 : base("Condition flag must be a digit or number.", paramName)
//             {
//             }
//         }
//
//         internal static bool IsValidFlag(char? flag, bool allowNull = false) =>
//             flag?.IsValidFlag() ?? allowNull;
//
//         internal static char? InvertFlag(char? flag) =>
//             flag?.IsValidFlag() ?? false ?
//                 flag.Value.Invert() :
//                 null;
//     }
// }
