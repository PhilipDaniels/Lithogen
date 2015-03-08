using System;
using System.Globalization;
using System.IO;

namespace Lithogen.Core
{
    /// <summary>
    /// VS Code Analysis will not actually recognise the generic helper method
    /// (and possibly sometimes non-generics) as having checked parameters, so
    /// CA1062 warnings will not go away. By introducing this dummy attribute
    /// we can fool the code analysis engine, and stop it producing false
    /// warnings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ValidatedNotNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Provides utility methods for validating arguments to methods.
    /// </summary>
    public static class ArgumentValidators
    {
        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// </summary>
        /// <typeparam name="T">Generic type of the argument.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfNull<T>([ValidatedNotNull] this T parameter, string parameterName)
        {
            return parameter.ThrowIfNull(parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// </summary>
        /// <typeparam name="T">Generic type of the argument.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfNull<T>([ValidatedNotNull] this T parameter, string parameterName, string message)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName, message);

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// Throws an <code>ArgumentException</code> if <paramref name="parameter"/> is the empty string.
        /// </summary>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static string ThrowIfNullOrEmpty([ValidatedNotNull] this string parameter, string parameterName)
        {
            return parameter.ThrowIfNullOrEmpty(parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// Throws an <code>ArgumentException</code> if <paramref name="parameter"/> is the empty string.
        /// </summary>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static string ThrowIfNullOrEmpty([ValidatedNotNull] this string parameter, string parameterName, string message)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName, message);
            if (parameter.Length == 0)
                throw new ArgumentException(message, parameterName);

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// Throws an <code>ArgumentException</code> if <paramref name="parameter"/> is the empty string or whitespace.
        /// </summary>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static string ThrowIfNullOrWhiteSpace([ValidatedNotNull] this string parameter, string parameterName)
        {
            return parameter.ThrowIfNullOrWhiteSpace(parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// Throws an <code>ArgumentException</code> if <paramref name="parameter"/> is the empty string or whitespace.
        /// </summary>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static string ThrowIfNullOrWhiteSpace([ValidatedNotNull] this string parameter, string parameterName, string message)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName, message);
            if (parameter.Trim().Length == 0)
                throw new ArgumentException(message, parameterName);

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is less than <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfLessThan<T>([ValidatedNotNull] this T parameter, T value, string parameterName)
            where T : IComparable<T>
        {
            return parameter.ThrowIfLessThan(value, parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is less than <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfLessThan<T>([ValidatedNotNull] this T parameter, T value, string parameterName, string message)
            where T : IComparable<T>
        {
            if (parameter.CompareTo(value) < 0)
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "Parameter {0} cannot be less than {1} but {2} supplied.", parameterName, value, parameter);
                if (message != null)
                    msg += " " + message;
                throw new ArgumentOutOfRangeException(parameterName, msg);
            }

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is less than 
        /// or equal to <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfLessThanOrEqualTo<T>([ValidatedNotNull] this T parameter, T value, string parameterName)
            where T : IComparable<T>
        {
            return parameter.ThrowIfLessThanOrEqualTo(value, parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is less than
        /// or equal to <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfLessThanOrEqualTo<T>([ValidatedNotNull] this T parameter, T value, string parameterName, string message)
            where T : IComparable<T>
        {
            if (parameter.CompareTo(value) <= 0)
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "Parameter {0} cannot be less than or equal to {1} but {2} supplied.", parameterName, value, parameter);
                if (message != null)
                    msg += " " + message;
                throw new ArgumentOutOfRangeException(parameterName, message);
            }

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is more than
        /// <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfMoreThan<T>([ValidatedNotNull] this T parameter, T value, string parameterName)
            where T : IComparable<T>
        {
            return parameter.ThrowIfMoreThan(value, parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is more than
        /// <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfMoreThan<T>([ValidatedNotNull] this T parameter, T value, string parameterName, string message)
            where T : IComparable<T>
        {
            if (parameter.CompareTo(value) > 0)
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "Parameter {0} cannot be more than {1} but {2} supplied.", parameterName, value, parameter);
                if (message != null)
                    msg += " " + message;
                throw new ArgumentOutOfRangeException(parameterName, msg);
            }

            return parameter;
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is more than
        /// or equal to <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfMoreThanOrEqualTo<T>([ValidatedNotNull] this T parameter, T value, string parameterName)
            where T : IComparable<T>
        {
            return parameter.ThrowIfMoreThanOrEqualTo(value, parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentOutOfRangeException</code> if <paramref name="parameter"/> is more than
        /// or equal to <paramref name="value."/>
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="value">The value to compare against.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfMoreThanOrEqualTo<T>([ValidatedNotNull] this T parameter, T value, string parameterName, string message)
        where T : IComparable<T>
        {
            if (parameter.CompareTo(value) >= 0)
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "Parameter {0} cannot be more than or equal to {1} but {2} supplied.", parameterName, value, parameter);
                if (message != null)
                    msg += " " + message;
                throw new ArgumentOutOfRangeException(parameterName, msg);
            }

            return parameter;
        }

        /// <summary>
        /// Throws a <code>ArgumentNullException</code> if <paramref name="path"/> is null.
        /// Throws a <code>ArgumentOutOfRangeException</code> if <paramref name="path"/> is whitespace.
        /// Throws a <code>DirectoryNotFoundException</code> if the directory <paramref name="path"/> does not exist.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="path"/> if no exception is thrown.</returns>
        public static string ThrowIfDirectoryDoesNotExist([ValidatedNotNull] this string path, string parameterName)
        {
            path.ThrowIfNullOrWhiteSpace(parameterName, "path must be specified.");

            if (!Directory.Exists(path))
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "The directory {0} does not exist.", path);
                throw new DirectoryNotFoundException(msg);
            }

            return path;
        }

        /// <summary>
        /// Throws a <code>ArgumentNullException</code> if <paramref name="path"/> is null.
        /// Throws a <code>ArgumentOutOfRangeException</code> if <paramref name="path"/> is whitespace.
        /// Throws a <code>FileNotFoundException</code> if the file <paramref name="path"/> does not exist.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="path"/> if no exception is thrown.</returns>
        public static string ThrowIfFileDoesNotExist([ValidatedNotNull] this string path, string parameterName)
        {
            path.ThrowIfNullOrWhiteSpace(parameterName, "path must be specified.");

            if (!File.Exists(path))
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "The file {0} does not exist.", path);
                throw new FileNotFoundException(msg, path);
            }

            return path;
        }

        /// <summary>
        /// Throws a <code>ArgumentException</code> if <typeparamref name="T"/> is not an enumerated type.
        /// Throws a <code>ArgumentOutOfRangeException</code> if <paramref name="enumerand"/> is not a valid value within <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="enumerand">The value of the enumeration.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="enumerand"/> if no exception is thrown.</returns>
        public static T ThrowIfInvalidEnumerand<T>([ValidatedNotNull] this T enumerand, string parameterName)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("The type " + enumType.FullName + " is not an enumerated type.");
            }

            if (!Enum.IsDefined(enumType, enumerand))
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "The value {0} is not valid for enumeration {1}.", enumerand, enumType.FullName);
                throw new ArgumentOutOfRangeException(msg);
            }

            return enumerand;
        }
    }
}
