namespace Rubicon.Rulesets;

public abstract class RuleSet
{
    /// <summary>
    /// The identifier of this ruleset. Should NEVER be 0.
    /// </summary>
    public abstract uint Id { get; }

    /// <summary>
    /// The name of the ruleset.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The shortened name of this ruleset.
    /// </summary>
    public abstract string ShortName { get; }

    /// <summary>
    /// The version this ruleset is on.
    /// </summary>
    public abstract uint Version { get; }

    /// <summary>
    /// Mainly for Discord RPC, will display this verb while you are playing.
    /// </summary>
    public abstract string PlayingVerb { get; }

    /// <summary>
    /// Checks whether the modifier passed through is compatible with this rule set.
    /// </summary>
    /// <returns>True or false</returns>
    public abstract bool CheckModifierCompatibility(); // TODO: ACTUALLY HAVE A PARAMETER!!!

    /// <summary>
    /// Creates the PlayField associated with this rule set.
    /// </summary>
    /// <returns></returns>
    public abstract PlayField CreatePlayField();
}