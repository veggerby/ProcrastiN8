using System.Reflection;

using ProcrastiN8.TODOFramework;

namespace ProcrastiN8.Services;

/// <summary>
/// Scans assemblies for <see cref="WontFixAttribute"/> and <see cref="SomedayMaybeAttribute"/> decorations
/// and compiles a formal technical debt report with supporting metrics and editorial commentary.
/// </summary>
/// <remarks>
/// <para>
/// Technical debt is not a failure. It is a strategic investment in future regret.
/// The <see cref="TechnicalDebtCollector"/> provides the audit infrastructure to ensure
/// that debt is properly catalogued, annotated, and then left alone.
/// </para>
/// <para>
/// All debt items are treated with equal gravity. The report is generated synchronously,
/// because adding an async wrapper to a debt-scanning tool would itself constitute technical debt.
/// </para>
/// </remarks>
public sealed class TechnicalDebtCollector
{
    /// <summary>Represents a single catalogued debt item.</summary>
    public sealed record DebtItem(
        string Location,
        string Category,
        string Reason,
        Type DeclaringType,
        MemberInfo Member);

    /// <summary>
    /// Represents the outcome of a debt collection scan.
    /// </summary>
    public sealed class TechnicalDebtReport
    {
        private readonly IReadOnlyList<DebtItem> _items;

        internal TechnicalDebtReport(IReadOnlyList<DebtItem> items, IReadOnlyList<Assembly> scannedAssemblies)
        {
            _items = items;
            ScannedAssemblies = scannedAssemblies;
            GeneratedAt = DateTimeOffset.UtcNow;
        }

        /// <summary>All catalogued debt items.</summary>
        public IReadOnlyList<DebtItem> Items => _items;

        /// <summary>Total number of debt items found. A higher number is not a problem. It is a backlog.</summary>
        public int TotalDebt => _items.Count;

        /// <summary>Number of items bearing the <see cref="WontFixAttribute"/>.</summary>
        public int WontFixCount => _items.Count(i => i.Category == "WontFix");

        /// <summary>Number of items bearing the <see cref="SomedayMaybeAttribute"/>.</summary>
        public int SomedayMaybeCount => _items.Count(i => i.Category == "SomedayMaybe");

        /// <summary>The assemblies that were scanned to produce this report.</summary>
        public IReadOnlyList<Assembly> ScannedAssemblies { get; }

        /// <summary>Timestamp of report generation (for compliance and deniability purposes).</summary>
        public DateTimeOffset GeneratedAt { get; }

        /// <summary>
        /// Returns the debt item with the highest estimated deferral year (the most optimistic).
        /// Returns <c>null</c> if no <see cref="SomedayMaybeAttribute"/> items have an estimated year.
        /// </summary>
        public DebtItem? MostOptimisticItem =>
            _items
                .Where(i => i.Category == "SomedayMaybe")
                .MaxBy(i => ExtractEstimatedYear(i.Member));

        /// <summary>
        /// Produces a formatted technical debt report suitable for presentation to leadership,
        /// who will read the total and not the contents.
        /// </summary>
        public string ToFormattedReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# Technical Debt Report");
            sb.AppendLine();
            sb.AppendLine($"*Generated: {GeneratedAt:yyyy-MM-dd HH:mm:ss UTC}*");
            sb.AppendLine($"*Assemblies scanned: {ScannedAssemblies.Count}*");
            sb.AppendLine();
            sb.AppendLine($"**Total debt items:** {TotalDebt}");
            sb.AppendLine($"- WontFix: {WontFixCount} (resolved by reclassification)");
            sb.AppendLine($"- SomedayMaybe: {SomedayMaybeCount} (aspirational)");
            sb.AppendLine();

            if (TotalDebt == 0)
            {
                sb.AppendLine("*No technical debt detected. Either the codebase is pristine or the annotations are missing.*");
                return sb.ToString();
            }

            var wontFix = _items.Where(i => i.Category == "WontFix").ToList();
            if (wontFix.Count > 0)
            {
                sb.AppendLine("## WontFix Items (officially closed)");
                sb.AppendLine();
                foreach (var item in wontFix)
                {
                    sb.AppendLine($"- **{item.Location}**: {item.Reason}");
                }

                sb.AppendLine();
            }

            var somedayMaybe = _items.Where(i => i.Category == "SomedayMaybe").ToList();
            if (somedayMaybe.Count > 0)
            {
                sb.AppendLine("## SomedayMaybe Items (aspirational)");
                sb.AppendLine();
                foreach (var item in somedayMaybe)
                {
                    sb.AppendLine($"- **{item.Location}**: {item.Reason}");
                }

                sb.AppendLine();
            }

            sb.AppendLine("*This report has been filed. It will be revisited in the next planning cycle.*");
            return sb.ToString();
        }

        private static int ExtractEstimatedYear(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<SomedayMaybeAttribute>();
            return attr?.EstimatedYear ?? 0;
        }
    }

    /// <summary>
    /// Scans the given assemblies and collects all technical debt annotations.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan. If none are provided, the calling assembly is scanned.</param>
    /// <returns>A <see cref="TechnicalDebtReport"/> containing all findings and appropriate commentary.</returns>
    public TechnicalDebtReport Collect(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        var items = new List<DebtItem>();

        foreach (var assembly in assemblies)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.OfType<Type>().ToArray();
            }

            foreach (var type in types)
            {
                CollectFromMember(type, type, items);

                var members = type.GetMembers(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly);

                foreach (var member in members)
                {
                    CollectFromMember(member, type, items);
                }
            }
        }

        return new TechnicalDebtReport(items, assemblies);
    }

    private static void CollectFromMember(MemberInfo member, Type declaringType, List<DebtItem> items)
    {
        foreach (var attr in member.GetCustomAttributes<WontFixAttribute>(inherit: false))
        {
            items.Add(new DebtItem(
                Location: $"{declaringType.Name}.{member.Name}",
                Category: "WontFix",
                Reason: attr.Reason,
                DeclaringType: declaringType,
                Member: member));
        }

        foreach (var attr in member.GetCustomAttributes<SomedayMaybeAttribute>(inherit: false))
        {
            var yearSuffix = attr.HasEstimatedYear ? $" (est. {attr.EstimatedYear})" : " (no estimated year — i.e., never)";
            var reason = string.IsNullOrWhiteSpace(attr.Aspiration)
                ? $"Aspirationally deferred{yearSuffix}."
                : $"{attr.Aspiration}{yearSuffix}";

            items.Add(new DebtItem(
                Location: $"{declaringType.Name}.{member.Name}",
                Category: "SomedayMaybe",
                Reason: reason,
                DeclaringType: declaringType,
                Member: member));
        }
    }
}
