using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public abstract class FlightRecorderEntityBase
    {
        /// <summary>
        /// Return the properties of the entity
        /// </summary>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            StringBuilder builder = new();

            // Use reflection to iterate over all the properties of the type
            foreach (var propertyInfo in GetType().GetProperties())
            {
                // Get the value, substituting an indicator for NULL
                var value = propertyInfo.GetValue(this);
                if (value == null)
                {
                    value = "null";
                }

                // Separate from previous properties with a ","
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                // Add this property name and value
                builder.Append(propertyInfo.Name);
                builder.Append(" = ");
                builder.Append(value);
            }

            return builder.ToString();
        }
    }
}