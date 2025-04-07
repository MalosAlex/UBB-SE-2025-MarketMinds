using System;
using System.Collections;
using Microsoft.UI.Xaml.Data;

namespace MarketMinds.Converters
{
    public class FirstImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IEnumerable images)
            {
                foreach (var item in images)
                {
                    var prop = item.GetType().GetProperty("Url");
                    if (prop != null)
                    {
                        string url = prop.GetValue(item)?.ToString();
                        if (!string.IsNullOrEmpty(url))
                        {
                            return url;
                        }
                    }
                }
            }
            // If a converter parameter (fallback) is provided, return it; otherwise, return an empty string
            return parameter?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}