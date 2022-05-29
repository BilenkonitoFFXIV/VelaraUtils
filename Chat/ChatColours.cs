namespace VelaraUtils.Chat;

public enum ChatItalics : ushort
{
    RESET = 0,
    ON = 1
}

public enum ChatColourKey : ushort
{
    NONE = 0,
    WHITE = 1,
    GREY = 4,
    LAVENDER = 9,
    RED = 17,
    YELLOW = 25,
    LIGHTBLUE = 34,
    BLUE = 37,
    LIGHTGREEN = 43,
    GREEN = 45,
    PURPLE = 48,
    INDIGO = 49,
    TEAL = 52,
    BROWN = 54,
    CYAN = 57,
    BRIGHTGREEN = 60,
    ORANGE = 65,
    PALEGREEN = 67,
}
public enum ChatColour : ushort
{
    NONE = ChatColourKey.NONE,
    WHITE = ChatColourKey.WHITE,
    GREY = ChatColourKey.GREY,
    LAVENDER = ChatColourKey.LAVENDER,
    RED = ChatColourKey.RED,
    YELLOW = ChatColourKey.YELLOW,
    LIGHTBLUE = ChatColourKey.LIGHTBLUE,
    BLUE = ChatColourKey.BLUE,
    LIGHTGREEN = ChatColourKey.LIGHTGREEN,
    GREEN = ChatColourKey.GREEN,
    PURPLE = ChatColourKey.PURPLE,
    INDIGO = ChatColourKey.INDIGO,
    TEAL = ChatColourKey.TEAL,
    BROWN = ChatColourKey.BROWN,
    CYAN = ChatColourKey.CYAN,
    BRIGHTGREEN = ChatColourKey.BRIGHTGREEN,
    ORANGE = ChatColourKey.ORANGE,
    PALEGREEN = ChatColourKey.PALEGREEN,

    RESET = NONE,
    ERROR = RED,
    TRUE = GREEN,
    FALSE = RED,
    CONDITION_FAILED = ORANGE,
    CONDITION_PASSED = GREEN,
    HIGHLIGHT_FAILED = YELLOW,
    HIGHLIGHT_PASSED = BRIGHTGREEN,
    PREFIX = LAVENDER,
    OUTGOING_TEXT = TEAL,
    QUIET = GREY,
    HELP_TEXT = PALEGREEN,
    USAGE_TEXT = LIGHTBLUE,
    HIGHLIGHT = CYAN,
    DEBUG = BROWN,
    JOB = INDIGO,
    COMMAND = PURPLE
}
public enum ChatGlow : ushort
{
    NONE = ChatColourKey.NONE,
    WHITE = ChatColourKey.WHITE,
    GREY = ChatColourKey.GREY,
    LAVENDER = ChatColourKey.LAVENDER,
    RED = ChatColourKey.RED,
    YELLOW = ChatColourKey.YELLOW,
    LIGHTBLUE = ChatColourKey.LIGHTBLUE,
    BLUE = ChatColourKey.BLUE,
    LIGHTGREEN = ChatColourKey.LIGHTGREEN,
    GREEN = ChatColourKey.GREEN,
    PURPLE = ChatColourKey.PURPLE,
    INDIGO = ChatColourKey.INDIGO,
    TEAL = ChatColourKey.TEAL,
    BROWN = ChatColourKey.BROWN,
    CYAN = ChatColourKey.CYAN,
    BRIGHTGREEN = ChatColourKey.BRIGHTGREEN,
    ORANGE = ChatColourKey.ORANGE,
    PALEGREEN = ChatColourKey.PALEGREEN,

    RESET = NONE,
    FALSE = RED,
    TRUE = GREEN,
    CONDITION_FAILED = YELLOW,
    CONDITION_PASSED = BRIGHTGREEN,
}
