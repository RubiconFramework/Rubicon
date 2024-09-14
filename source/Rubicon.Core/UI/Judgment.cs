using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// A control node that activates upon the player hitting a note, showing their type of rating.
/// </summary>
[GlobalClass]
public partial class Judgment : Control
{
    /// <summary>
    /// The texture to show when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    [Export] public Texture2D PerfectTexture;

    /// <summary>
    /// The texture to show when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    [Export] public Texture2D GreatTexture;

    /// <summary>
    /// The texture to show when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    [Export] public Texture2D GoodTexture;

    /// <summary>
    /// The texture to show when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    [Export] public Texture2D BadTexture;

    /// <summary>
    /// The texture to show when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    [Export] public Texture2D MissTexture;

    /// <summary>
    /// How much to scale the judgment graphics by.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    [ExportGroup("Materials"), Export] public Material PerfectMaterial; // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    [Export] public Material GreatMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    [Export] public Material GoodMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    [Export] public Material BadMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    [Export] public Material MissMaterial;

    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    public virtual void Play(HitType type)
    {
        Play(type, Vector2.Zero);
    }

    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    /// <param name="offset">A Vector2 that offsets the position</param>
    public virtual void Play(HitType type, Vector2 offset)
    {
        // Makes the judgment anchor at the center probably
        Play(type, 0.5f, 0.5f, 0.5f, 0.5f, offset);
    }
    
    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="pos">Where to place the judgment, in pixels.</param>
    public virtual void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        AnchorLeft = anchorLeft;
        AnchorTop = anchorTop;
        AnchorRight = anchorRight;
        AnchorBottom = anchorBottom;
    }

    /// <summary>
    /// Get a judgment texture based on the rating.
    /// </summary>
    /// <param name="type">The rating</param>
    /// <returns>The Texture2D associated with the Judgment</returns>
    protected Texture2D GetJudgmentTexture(HitType type)
    {
        switch (type)
        {
            default:
                return PerfectTexture;
            case HitType.Great:
                return GreatTexture;
            case HitType.Good:
                return GoodTexture;
            case HitType.Bad:
                return BadTexture;
            case HitType.Miss:
                return MissTexture;
        }
    }

    /// <summary>
    /// Get a judgment material based on the rating.
    /// </summary>
    /// <param name="type">The rating</param>
    /// <returns>The Material associated with the Judgment</returns>
    protected Material GetJudgmentMaterial(HitType type)
    {
        switch (type)
        {
            default:
                return PerfectMaterial;
            case HitType.Great:
                return GreatMaterial;
            case HitType.Good:
                return GoodMaterial;
            case HitType.Bad:
                return BadMaterial;
            case HitType.Miss:
                return MissMaterial;
        }
    }
}

/*
   ::::::::::::::::::::::::::::::::::::::::::::::+@@@@:::::::::::::::::::::::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::::::::::::::@@@@@@@@@@@#:@@@@@@@@:::::::::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::::::::@@@@@@@@@@@@@@@@@@@@@@@@@@@::::::::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::::::::*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@::::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::::@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::%@@@@@@@%@@@@@@@@@@@@@@@@@@@%@@@@@@@@-:::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::+%@@@@@%%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::-#@@%@@@@@@@@@@@%%@@@@@@@@@@@@@@@@@@@@@@@@-:::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::=#%@@@@@@@@@@@@@@%@@@@@@@@@@@@@@@@@@@@@@@@@%+-:::::::::::::::::::::::::
   :::::::::::::::::::::::::::-**%@@@@@@@@@@@@@%%@@@@@@@%%%%@@@@@@@@@@@@@@@@--:::::::::::::::::::::::::
   ::::::::::::::::::::::::::::-@@@@@%%@@@%@@%%@@@@@@%##***#%@@@@@@@@@@@@@@@%%:::::::::::::::::::::::::
   ::::::::::::::::::::::::::-=*@@@@%@@@@@@@%%%%%%%@#*+====+#@@@@@@@@@@@%@@%=-=-:::::::::::::::::::::::
   ::::::::::::::::::::::::::*-*@%@@@@@@@@%@%%%%%#%**==----=+%@@@@@@@@@@@%%@#-:::::::::::::::::::::::::
   ::::::::::::::::::::::::::-*@%%@@@@@@%%@%%%%%#@+==--:----=*@#@@@@@@@@@@@@@@-::::::::::::::::::::::::
   ::-----------::-------:-::*@@@@%@@@@@@@%%%%%*+=----::--==++*##%@@@@@@@@@@@%=-:::::-:-::-----------::
   ------------------------#@@@%@@%@@@@@@%%%%@%%%**=----=+*%%%#%%#%%@@@@@@@@@#+*-::--------------------
   ---------------------------%@@%@@@%@@##+=--=++**+===+###+====*%%%@@@@@@@@%+-------------------------
   ------------------------%+@@%@@%@@%@*++#@=@+@%#*+-:=***@*@@#@%##%@@@@@@@@@+%-:----------------------
   -------------------------+--#@@@%%@%====-++++++==---*++=+**##%**#@@@@@@@@%#%------------------------
   ----------------------------+=@#@@@#=----===-====-:-++====+**+=+*@@@@@@@%%=-------------------------
   ----------------------------+==%@@@==----------==-:=+*+=========*#@@%@@*-=+-------------------------
   ----------------------------#--#%%@+==----:-:-===-:-+**========+*#@@@@%+----------------------------
   ------------------------------+%*%@*===------==--::-++*+=====+*#*%@@@#*+----------------------------
   -------------------------------%@*%*=====---==+*@=+*%%%=====+**#*%%#*@-=----------------------------
   -------------------------------%-#@@========--==++*=*+++==++***##@#*-==-----------------------------
   -------------------------------===@@@+=====-=-===-=-=+++++++**##*%+=--------------------------------
   --------------------------------=@=@@@========+##*+*#%#***+*###@#-----------------------------------
   ----------------------------=====#*@*@++======++=+=++*#****###=@%-----------------------------------
   --=====-==-===-=-===---=--------=-**=%+++======*#%%%%#****###%==++==---=-------=====-=====--====--==
   ==============================------=*+++%+=====-===++*++*#@%%@=----==============================-=
   ====================================@@+=++#%==-=-====+++#@%@%%@@#===================================
   =================================#@@@%+=+++*##*+++***##@@%%%%%%@@@%=================================
   ==============================#@@%@@%++===+++##%%%%%@@@@%%%####%@@@@@%==============================
   ============================@@@@@@@@#*++====+*+*##%%%@@%%##*##%%@@@@@@@@============================
   =========================*@@@@@@@@@@#+=======++++*+*####*****###*@@@@@@@@@#=========================
   ===================#@@@@@@@@@@@@@@@##+========++++++**++*********@@@@@%@@@@@@@@@*===================
   ==============-@@%@@%@%@@@@@@@@%@@@#*===========+=++++++**++++++*@@@@@@%@@@@@@@@@*@@@-==============
   ==========@@%%@@@@%%@@@@@@@%%@@@@@@#*==========+=++++++*++++++++*@@@@@@@@%@@@@@%@%@@@@@@@*==========
   ===@@%@@@@%@@@@@@%%@@@%@@%%%%%%@@@@#++++=======+++++++=++++++===+#@@@@@@%%%%@@@@@@%@@@@@@@@@@@@@+===
   ==-@@@@@@%%@@@@@%%@%%@@@@%%%%%%%%@@%+=--------==++==+===========+*@%@@%%@%%%%%@@%@@%@@@@@@@@@@@@@===
   ==@%%%@@%%%@@@@%%@@@@@@@%%%%%%%%@@@%+=----------=====---========+*@@%%%%%%%%%%%@%%@@%@@@@@@@@@@%%@==
   +@%%%%%%%%%%%@%%:=:@@%%%%%%%%#@@@@%%+=---------======---========+*@@@@@%%%%%%%%%%:+-@%@@@@@@@@@%@@@=
   @%%%%%%@%%%%%%#%%*@@%%%%%%%@@@%@@@%%+==-------=======---=---====+:@@@@@@@%%%%%%@@@%#%#@@@@@@@@%%@@@@
   %%%%%#%@@@@@%%%@@@**%%%@@%@%%%%%%%%:+=---------====--------=====*:@@@%%%%%@%%%%%%@@@@@@@@@@@@@%%@@%@
   %%%@%%%@%@@@%%%%@@@@@#%%%%%%%##%%@%:+==--------===----------====::@@%%%%%%%%%@*@@@@@@@@@@@@@@@%@@@%@
   %##%*%%%%@@@%%%%%%%%%%%%%%#%####%@%-.==----------------------==.::@%%%%%%%%%#%%%@*@@@@@@@@@@@@%@@@%@
   @##%%%%%%@@@%%%%#%*%%%%%%########%%+..==---------------------=:.:.@%%%%%%%%%%%%%%#%%%@@@@@@@@@%@@@%@
   @*#*#%%%%@@@@%#:.%%##%%%##***###%%%:...=---------------------...:.@%%%%%%%%%%%%%%%#=:*%@@@@@@@@@@%%@
   %#%##@@%%@@@##...####%%%%####*###%%.....===--=====---------=.....#@%%%%%%%%%###%%%%..:#%*@@@@@@@@%@@
   #%*%#%@%@@@@@%%#*###%%%%%###**###%%%......=========-----==.......@@%%%%%%%%%####%%%%%%%@@@@@@@@@%@@@
   ##%@%@@@@@@@@%%@#+##%%%%%%#*****####-........+++++=====-.......::@@%%%%%%%%%*#####%%%#@@@@@@@@@@@@@@
   ###%@@@@@@@@@%%@@%*-#%%%%%#*******#%-............................@@@%%%%%%%##**##%%*@@@@@@@@@@@@@@@@
   ##%@%@@@@@@@@%%@@@@#####%%%#******##=..................:........@@@@%%%%%%%***+%=@@@@@@@@@@@@@@@@@@@
   #%%@@@@@@@@@@%%@@@@@@%###%%%##******%...........................@@@%%##%%%%##+##*@@@@@@@@@@@@@@@@@@@
   @%*%%@@@@@@@@%@@@@@@@@#%%%#%%%%##**#%-..........................@@%##*#%%%##%#%@@@@@@@@@@@@@@@@@@@@@
*/