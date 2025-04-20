using FlightRecorder.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlightRecorder.DataExchange.Export
{
    public abstract class ExporterBase<T> where T : class
    {
        protected Dictionary<string, PropertyInfo> Properties { get; private set; } = new();

        protected ExporterBase()
        {
            // Get a list of entity properties marked as exportable, ordering by the column
            // order defined on the export attribute
            IEnumerable<PropertyInfo> properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => Attribute.IsDefined(x, typeof(ExportAttribute)))
                .OrderBy(x => x.GetCustomAttribute<ExportAttribute>()!.Order);

            // Build a dictionary of properties where the key is the column name and the value is
            // the corresponding property information instance
            foreach (PropertyInfo property in properties)
            {
                var attribute = property.GetCustomAttribute<ExportAttribute>();
                if (attribute != null)
                {
                    Properties.Add(attribute.Name, property);
                }
            }
        }
    }
}
