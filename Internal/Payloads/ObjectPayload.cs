using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace VelaraUtils.Internal.Payloads;

internal class ObjectTypeFormatter : IFormatProvider, ICustomFormatter
{
    public object? GetFormat(Type? formatType) =>
        formatType == typeof(ICustomFormatter) ?
            this :
            null;

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (arg is not Type type)
        {
            try
            {
                return HandleOtherFormats(format, arg);
            }
            catch (FormatException e)
            {
                throw new FormatException($"The format of '{format}' is invalid.", e);
            }
        }

        string fmt = string.IsNullOrWhiteSpace(format) ?
            string.Empty :
            format.ToUpper();

        bool addName = false;
        bool addGenArgs = false;
        bool addGenArgsPretty = false;
        while (fmt.Length > 0)
        {
            char c = fmt[0];
            switch (c)
            {
                case 'N':
                    addName = true;
                    break;
                case 'G':
                    addGenArgs = true;
                    break;
                case 'P':
                    addGenArgsPretty = true;
                    break;
            }
            fmt = fmt[1..];
        }

        StringBuilder sb = new StringBuilder();

        if (addName)
            sb.Append(type.Name);

        if (!addGenArgs || !type.IsGenericType)
            return sb.ToString().Trim();

        Type[] genericArguments = type.GetGenericArguments();
        string genArgStr = string.Join(',', genericArguments.Select(o => o.Name));
        sb.Append(addName ?
            addGenArgsPretty ?
                $"<{genArgStr}>" :
                $"`{genericArguments.Length}[{genArgStr}]" :
            genArgStr);

        return sb.ToString().Trim();
    }

    private static string HandleOtherFormats(string? format, object? arg) => arg switch
    {
        null => string.Empty,
        IFormattable formattable => formattable.ToString(format, CultureInfo.CurrentCulture),
        _ => arg.ToString() ?? string.Empty
    };
}

internal class ObjectPayload : Payload, ITextProvider
{
    public object? Value { get; set; }
    public string Text => string.Format(new ObjectTypeFormatter(), "({0:NGP}): {1}", Value?.GetType() ?? typeof(object), Value?.ToString() ?? "null");
    public override PayloadType Type => PayloadType.RawText;

    public ObjectPayload() { }

    public ObjectPayload(object? obj)
    {
        Value = obj;
    }

    protected override byte[] EncodeImpl() => Encoding.UTF8.GetBytes(Text);
    protected override void DecodeImpl(BinaryReader reader, long endOfStream) { }
}
