namespace Unity.Extension;

/// <summary>
/// Represents the method that compares two objects of different types
/// </summary>
/// <typeparam name="TLeft"><see cref="Type"/> of the object on the left</typeparam>
/// <typeparam name="TRight"><see cref="Type"/> of the object on the right</typeparam>
/// <typeparam name="TOut"><see cref="Type"/> of the output</typeparam>
/// <param name="left">Object to compare on the left</param>
/// <param name="right">Object to compare on the right</param>
/// <returns>Result of the comparison</returns>
public delegate TOut Comparison<in TLeft, in TRight, out TOut>(TLeft left, TRight right);
