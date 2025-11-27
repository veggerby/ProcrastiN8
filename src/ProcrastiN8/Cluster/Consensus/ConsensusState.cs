namespace ProcrastiN8.Cluster.Consensus;

/// <summary>
/// Represents the state of a consensus round in the Global Moving Target Clock protocol.
/// </summary>
public enum ConsensusState
{
    /// <summary>No consensus round is active.</summary>
    Idle,

    /// <summary>A proposal has been made and is awaiting votes.</summary>
    Proposing,

    /// <summary>Votes are being collected.</summary>
    Voting,

    /// <summary>Consensus has been reached (for now).</summary>
    Committed,

    /// <summary>Consensus failed — epoch will shift unpredictably.</summary>
    Failed
}
