namespace Common.Types;

public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Value = new();

    [System.Diagnostics.Contracts.Pure]
    public override int GetHashCode() =>
        0;

    [System.Diagnostics.Contracts.Pure]
    public override bool Equals(object? obj) =>
        obj is Unit;

    [System.Diagnostics.Contracts.Pure]
    public override string ToString() =>
        "()";

    [System.Diagnostics.Contracts.Pure]
    public bool Equals(Unit other) =>
        true;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator ==(Unit lhs, Unit rhs) =>
        true;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator !=(Unit lhs, Unit rhs) =>
        false;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator >(Unit lhs, Unit rhs) =>
        false;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator >=(Unit lhs, Unit rhs) =>
        true;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator <(Unit lhs, Unit rhs) =>
        false;

    [System.Diagnostics.Contracts.Pure]
    public static bool operator <=(Unit lhs, Unit rhs) =>
        true;

    /// <summary>
    /// Provide an alternative value to unit
    /// </summary>
    /// <typeparam name="T">Alternative value type</typeparam>
    /// <param name="anything">Alternative value</param>
    /// <returns>Alternative value</returns>
    [System.Diagnostics.Contracts.Pure]
    public T Return<T>(T anything) => anything;

    /// <summary>
    /// Provide an alternative value to unit
    /// </summary>
    /// <typeparam name="T">Alternative value type</typeparam>
    /// <param name="anything">Alternative value</param>
    /// <returns>Alternative value</returns>
    [System.Diagnostics.Contracts.Pure]
    public T Return<T>(Func<T> anything) => anything();

    /// <summary>
    /// Always equal
    /// </summary>
    [System.Diagnostics.Contracts.Pure]
    public int CompareTo(Unit other) =>
        0;

    [System.Diagnostics.Contracts.Pure]
    public static Unit operator +(Unit a, Unit b) =>
        Value;
}