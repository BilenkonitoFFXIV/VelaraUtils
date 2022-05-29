using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using VelaraUtils.Chat;

namespace VelaraUtils.Internal.Payloads;

internal class GenericPayload : Payload
{
    private static readonly TextPayload NullPayload = new("null");

    private delegate byte[] EncodeImplDelegate();
    private delegate void DecodeImplDelegate(BinaryReader reader, long endOfStream);

    private readonly Payload _payload;
    private readonly Type _payloadType;
    private readonly EncodeImplDelegate? _encodeImpl;
    private readonly DecodeImplDelegate? _decodeImpl;

    public override PayloadType Type => _payload.Type;

    public GenericPayload(object? value)
    {
        _payload = value switch
        {
            null => NullPayload,
            bool v => new TextPayload(v ? "true" : "false"),
            char v => new TextPayload(v.ToString()),
            string v => new TextPayload(v),
            byte v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            short v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            ushort v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            int v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            uint v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            long v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            ulong v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            float v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            double v => new TextPayload(v.ToString(CultureInfo.InvariantCulture)),
            ChatItalics v => v == ChatItalics.ON ?
                EmphasisItalicPayload.ItalicsOn :
                EmphasisItalicPayload.ItalicsOff,
            ChatColour v => v == ChatColour.RESET ?
                UIForegroundPayload.UIForegroundOff :
                new UIForegroundPayload((ushort)v),
            ChatGlow v => v == ChatGlow.RESET ?
                UIGlowPayload.UIGlowOff :
                new UIGlowPayload((ushort)v),
            BitmapFontIcon v => new IconPayload(v),
            ItemLink v => v.ToPayload(),
            MapLink v => v.ToPayload(),
            QuestLink v => v.ToPayload(),
            StatusLink v => v.ToPayload(),
            _ => new ObjectPayload(value)
        };
        _payloadType = _payload.GetType();
        _encodeImpl = GetDelegate<EncodeImplDelegate>(nameof(EncodeImpl));
        _decodeImpl = GetDelegate<DecodeImplDelegate>(nameof(DecodeImpl));
    }

    private TDelegate? GetDelegate<TDelegate>(string name) where TDelegate : Delegate =>
        _payloadType
            .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)?
            .CreateDelegate<TDelegate>(_payload);

    protected override byte[] EncodeImpl() =>
        _encodeImpl?.Invoke() ?? Array.Empty<byte>();

    protected override void DecodeImpl(BinaryReader reader, long endOfStream) =>
        _decodeImpl?.Invoke(reader, endOfStream);
}
