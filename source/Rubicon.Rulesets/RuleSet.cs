namespace Rubicon.Rulesets;

/// <summary>
/// A base ruleset for any Rubicon ruleset
/// </summary>
public partial class RuleSet : RefCounted
{
    /// <summary>
    /// The unique identifier of this ruleset.
    /// </summary>
    public virtual string Uid { get; }

    /// <summary>
    /// The name of the ruleset.
    /// </summary>
    public virtual string Name { get; }

    /// <summary>
    /// The shortened name of this ruleset.
    /// </summary>
    public virtual string ShortName { get; }

    /// <summary>
    /// The version this ruleset is on.
    /// </summary>
    public virtual uint Version { get; }

    /// <summary>
    /// Mainly for Discord RPC, will display this verb while you are playing.
    /// </summary>
    public virtual string PlayingVerb { get; }

    /// <summary>
    /// Checks whether the modifier passed through is compatible with this rule set.
    /// </summary>
    /// <returns>True or false</returns>
    public virtual bool CheckModifierCompatibility() => false; // TODO: ACTUALLY HAVE A PARAMETER!!!

    /// <summary>
    /// Creates the PlayField associated with this rule set.
    /// </summary>
    /// <returns></returns>
    public virtual PlayField CreatePlayField() => null;
}