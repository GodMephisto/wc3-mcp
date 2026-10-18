---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/Optional.cs"
ref: HEAD
---

﻿using System.Diagnostics.CodeAnalysis;

namespace vJassMainJBlueprint.Utils
{
    /// <summary>
    /// A class that wraps a generic type to represent an optional value, which may or may not be present.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class Optional<T>
    {
        /// <summary>
        /// Gets an empty Optional instance.
        /// </summary>
        public static Optional<T> Empty => new();

        private readonly T? value;

        // Private constructors to ensure controlled creation via static methods.
        private Optional(T? value)
        {
            this.value = value;
        }

        private Optional()
        {
            this.value = default;
        }

        /// <summary>
        /// Creates an Optional instance containing the provided value.
        /// </summary>
        /// <param name="value">The value to wrap, can be null.</param>
        /// <returns>An Optional instance containing the provided value.</returns>
        public static Optional<T> Of(T? value)
        {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Returns the value contained in this Optional, or throws an exception if the value is null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the value is null.</exception>
        /// <returns>The contained value.</returns>
        public T GetValue()
        {
            if (value == null)
            {
                throw new InvalidOperationException("Optional value is null.");
            }
            return value;
        }

        /// <summary>
        /// Transforms the value using the provided mapping function.
        /// If the value is null, an empty Optional is returned.
        /// </summary>
        /// <typeparam name="U">The target type of the mapping function.</typeparam>
        /// <param name="mapperFunction">The function to apply to the value.</param>
        /// <returns>An Optional containing the transformed value, or empty if the original value was null.</returns>
        public Optional<U> Map<U>(Func<T, U> mapperFunction)
        {
            return value == null ? Optional<U>.Empty : Optional<U>.Of(mapperFunction(value));
        }

        /// <summary>
        /// Filters the Optional using a predicate. If the predicate is not met or the value is null, an empty Optional is returned.
        /// </summary>
        /// <param name="predicate">A predicate function to test the value.</param>
        /// <returns>An Optional containing the value if it satisfies the predicate, otherwise an empty Optional.</returns>
        public Optional<T> Filter(Predicate<T> predicate)
        {
            return value == null || !predicate(value) ? Empty : this;
        }

        /// <summary>
        /// Casts the value to the specified type if possible. 
        /// Returns an Optional containing the casted value, or an empty Optional if the cast fails.
        /// </summary>
        /// <typeparam name="U">The target type for the cast.</typeparam>
        /// <returns>An Optional containing the casted value, or empty if the cast fails.</returns>
        public Optional<U> Cast<U>()
        {
            if (value is U castedValue)
            {
                return Optional<U>.Of(castedValue);
            }
            return Optional<U>.Empty;
        }

        /// <summary>
        /// If the value is present, performs the provided action on it.
        /// </summary>
        /// <param name="action">The action to perform on the value, if present.</param>
        public void IfPresent(DisallowNullAction<T> action)
        {
            if (value != null)
            {
                action(value);
            }
        }

        /// <summary>
        /// A delegate representing an action to be performed on a value that is guaranteed not to be null.
        /// </summary>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="x">The non-null value to perform the action on.</param>
        public delegate void DisallowNullAction<V>([DisallowNull] V x);

        /// <summary>
        /// Returns the value, or throws the provided exception if the value is null.
        /// </summary>
        /// <param name="exception">The exception to throw if the value is null.</param>
        /// <returns>The value.</returns>
        /// <exception cref="Exception">Throws the provided exception if the value is null.</exception>
        public T OrElseThrow(Exception exception)
        {
            if (value == null)
            {
                throw exception;
            }
            return value;
        }

        /// <summary>
        /// Returns the value if present, or the provided default value if the value is null.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the value is null.</param>
        /// <returns>The value or the default value.</returns>
        public T? OrElse(T? defaultValue)
        {
            return value ?? defaultValue;
        }
    }
}
