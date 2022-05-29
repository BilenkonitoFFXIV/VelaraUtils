// using System;
// using VelaraUtils.Utils;
//
// namespace VelaraUtils.Attributes
// {
//     [AttributeUsage(AttributeTargets.Method)]
//     internal class ConditionAttribute : Attribute
//     {
//         public char Flag { get; }
//         public char? InverseFlag { get; }
//         public string Name { get; }
//         public string Description { get; }
//
//         public ConditionAttribute(char flag, string name = "", string description = "")
//             : this(flag, flag.Invert(), name, description)
//         {
//         }
//
//         public ConditionAttribute(char flag, char? inverseFlag, string name = "", string description = "")
//         {
//             if (!flag.IsValidFlag()) throw new FlagUtils.InvalidFlagException(nameof(flag));
//             Flag = flag;
//
//             if (!FlagUtils.IsValidFlag(inverseFlag, true)) throw new FlagUtils.InvalidFlagException(nameof(inverseFlag));
//             InverseFlag = inverseFlag == flag ?
//                 flag.Invert() :
//                 inverseFlag;
//
//             Name = name;
//             Description = description;
//         }
//     }
// }
