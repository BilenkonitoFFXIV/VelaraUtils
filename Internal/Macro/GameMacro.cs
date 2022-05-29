using System;
using System.Collections.Generic;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using JetBrains.Annotations;
using VelaraUtils.Chat;

namespace VelaraUtils.Internal.Macro;

using Macro = RaptureMacroModule.Macro;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public unsafe class GameMacro
{
    protected Macro* Ptr;
    public Macro* Value => Ptr;
    public bool IsValid => (IntPtr)Ptr != IntPtr.Zero;

    public uint IconId
    {
        get => Ptr->IconId;
        set => Ptr->IconId = value;
    }

    public uint Key
    {
        get => Ptr->Unk;
        set => Ptr->Unk = value;
    }

    public string Name
    {
        get => Ptr->Name.ToString();
        set
        {
            fixed (byte* cStr = Encoding.UTF8.GetBytes(value + "\0"))
                Ptr->Name.SetString(cStr);
        }
    }

    public string this[int i]
    {
        get
        {
            if (i is < 0 or > 14) throw new ArgumentOutOfRangeException(nameof(i));
            return Ptr->Line[i]->ToString();
        }
        set
        {
            if (i is < 0 or > 14) throw new ArgumentOutOfRangeException(nameof(i));
            fixed (byte* cStr = Encoding.UTF8.GetBytes(value + "\0"))
                Ptr->Line[i]->SetString(cStr);
        }
    }

    public int LineCount =>
        (int)RaptureMacroModule.Instance->GetLineCount(Ptr);

    protected GameMacro() : this((Macro*)IntPtr.Zero)
    {
    }

    protected GameMacro(Macro* ptr)
    {
        Ptr = ptr;
    }

    public static implicit operator GameMacro(Macro* other) => new(other);
    public static implicit operator Macro*(GameMacro other) => other.Ptr;

    public static GameMacro Get(uint set, uint index) =>
        new(RaptureMacroModule.Instance->GetMacro(set, index));

    public void ReplaceLines(IReadOnlyList<string> lines)
    {
        for (int i = 0; i < Math.Min(lines.Count, 15); i++)
            this[i] = lines[i];
    }

    public void AppendLines(IReadOnlyList<string> lines)
    {
        for (int i = 0; i < Math.Min(lines.Count, 15 - LineCount); i++)
            this[i] = lines[i];
    }

    public bool Execute()
    {
        // if (LineCount > 0) RaptureShellModule.Instance->ExecuteMacro(Ptr);
        return LineCount > 0 && VelaraUtils.MacroManager.Execute(this);
    }

    public IReadOnlyList<IReadOnlyList<object>> ToChatMessage()
    {
        if (!IsValid)
            return new List<List<object>> { new() { ChatColour.ERROR, "Invalid ", GetType().Name, ChatColour.RESET } };

        List<List<object>> messageData = new()
        {
            new List<object>
            {
                ChatColour.WHITE,
                "   - IconId: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                IconId.ToString(),
                ChatColour.RESET
            },
            new List<object>
            {
                ChatColour.WHITE,
                "   - Key: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                Key.ToString(),
                ChatColour.RESET
            },
            new List<object>
            {
                ChatColour.WHITE,
                "   - Name: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                Name,
                ChatColour.RESET
            },
            new List<object>
            {
                ChatColour.WHITE,
                "   - LineCount: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                LineCount,
                ChatColour.RESET
            },
            new List<object>
            {
                ChatColour.WHITE,
                "   - Lines:",
                ChatColour.RESET
            }
        };

        for (int i = 0; i < Math.Min(LineCount, 15); i++)
            messageData.Add(new List<object>
            {
                ChatColour.WHITE,
                "       - ",
                i.ToString(),
                ": ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                this[i],
                ChatColour.RESET
            });

        return messageData;
    }
}
