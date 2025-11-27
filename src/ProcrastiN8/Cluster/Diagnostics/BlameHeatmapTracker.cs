using System.Collections.Concurrent;

namespace ProcrastiN8.Cluster.Diagnostics;

/// <summary>
/// Tracks blame assignment across the cluster — which nodes have deferred the most work.
/// Essential for post-mortem analysis and finger-pointing ceremonies.
/// </summary>
public sealed class BlameHeatmapTracker
{
    private readonly ConcurrentDictionary<string, BlameRecord> _blameRecords = new();
    private readonly object _lock = new();

    /// <summary>
    /// Records a deferral against a node.
    /// </summary>
    /// <param name="nodeId">The node that deferred work.</param>
    /// <param name="reason">The excuse provided.</param>
    public void RecordDeferral(string nodeId, string? reason = null)
    {
        _blameRecords.AddOrUpdate(
            nodeId,
            _ => new BlameRecord { DeferralCount = 1, LastDeferralReason = reason },
            (_, existing) =>
            {
                existing.DeferralCount++;
                existing.LastDeferralReason = reason ?? existing.LastDeferralReason;
                existing.LastDeferralTime = DateTimeOffset.UtcNow;
                return existing;
            });
    }

    /// <summary>
    /// Records a migration against both source and destination nodes.
    /// </summary>
    /// <param name="fromNodeId">Source node.</param>
    /// <param name="toNodeId">Destination node.</param>
    /// <param name="reason">Reason for migration.</param>
    public void RecordMigration(string fromNodeId, string toNodeId, string? reason = null)
    {
        _blameRecords.AddOrUpdate(
            fromNodeId,
            _ => new BlameRecord { MigrationsSent = 1, LastMigrationReason = reason },
            (_, existing) =>
            {
                existing.MigrationsSent++;
                existing.LastMigrationReason = reason ?? existing.LastMigrationReason;
                return existing;
            });

        _blameRecords.AddOrUpdate(
            toNodeId,
            _ => new BlameRecord { MigrationsReceived = 1 },
            (_, existing) =>
            {
                existing.MigrationsReceived++;
                return existing;
            });
    }

    /// <summary>
    /// Records a timeout against a node.
    /// </summary>
    /// <param name="nodeId">The node that timed out.</param>
    public void RecordTimeout(string nodeId)
    {
        _blameRecords.AddOrUpdate(
            nodeId,
            _ => new BlameRecord { TimeoutCount = 1 },
            (_, existing) =>
            {
                existing.TimeoutCount++;
                return existing;
            });
    }

    /// <summary>
    /// Gets the current blame heatmap.
    /// </summary>
    /// <returns>A dictionary of node IDs to total blame scores.</returns>
    public IReadOnlyDictionary<string, int> GetHeatmap()
    {
        return _blameRecords.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.CalculateBlameScore());
    }

    /// <summary>
    /// Gets detailed blame records for all nodes.
    /// </summary>
    /// <returns>A dictionary of node IDs to blame records.</returns>
    public IReadOnlyDictionary<string, BlameRecord> GetDetailedRecords()
    {
        return _blameRecords.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Gets the node with the highest blame score.
    /// </summary>
    /// <returns>The most blameworthy node ID, or null if no records exist.</returns>
    public string? GetMostBlameworthy()
    {
        var records = _blameRecords.ToList();
        if (records.Count == 0)
        {
            return null;
        }

        return records
            .OrderByDescending(kvp => kvp.Value.CalculateBlameScore())
            .First()
            .Key;
    }

    /// <summary>
    /// Gets the top N most blameworthy nodes.
    /// </summary>
    /// <param name="count">Number of nodes to return.</param>
    /// <returns>List of node IDs ordered by blame score.</returns>
    public IReadOnlyList<string> GetTopBlameworthy(int count)
    {
        return _blameRecords
            .OrderByDescending(kvp => kvp.Value.CalculateBlameScore())
            .Take(count)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    /// <summary>
    /// Clears a node's blame record (e.g., after redemption).
    /// </summary>
    /// <param name="nodeId">The node to clear.</param>
    public void ClearBlame(string nodeId)
    {
        _blameRecords.TryRemove(nodeId, out _);
    }

    /// <summary>
    /// Clears all blame records.
    /// </summary>
    public void ClearAll()
    {
        _blameRecords.Clear();
    }
}

/// <summary>
/// Individual blame record for a node.
/// </summary>
public sealed class BlameRecord
{
    /// <summary>Gets or sets the number of deferrals.</summary>
    public int DeferralCount { get; set; }

    /// <summary>Gets or sets the number of migrations sent.</summary>
    public int MigrationsSent { get; set; }

    /// <summary>Gets or sets the number of migrations received.</summary>
    public int MigrationsReceived { get; set; }

    /// <summary>Gets or sets the number of timeouts.</summary>
    public int TimeoutCount { get; set; }

    /// <summary>Gets or sets the last deferral reason.</summary>
    public string? LastDeferralReason { get; set; }

    /// <summary>Gets or sets the last migration reason.</summary>
    public string? LastMigrationReason { get; set; }

    /// <summary>Gets or sets the last deferral time.</summary>
    public DateTimeOffset LastDeferralTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Calculates the total blame score for this node.
    /// </summary>
    /// <returns>The blame score.</returns>
    public int CalculateBlameScore()
    {
        // Deferrals are worth 1 point each
        // Migrations sent are worth 2 points each (you made someone else deal with it)
        // Timeouts are worth 3 points each (you completely failed)
        // Migrations received actually reduce blame by 0.5 points (you're helping!)
        return DeferralCount + (MigrationsSent * 2) + (TimeoutCount * 3) - (MigrationsReceived / 2);
    }
}
