using System.Reflection;
using ClosedXML.Excel;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Common.Exceptions;
using Microsoft.AspNetCore.Http;

public static class ExcelMapper
{
    public static async Task<List<T>> ExtractAsync<T>(IFormFile file)
        where T : new()
    {
        if (file == null || file.Length == 0)
            throw new CommonException(ResponseMessage.ExcelFileEmpty);

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name.ToLower(), p => p);

        // Read header
        var headerRow = worksheet.Row(1);
        var columnMap = new Dictionary<int, PropertyInfo>();

        foreach (var cell in headerRow.CellsUsed())
        {
            var headerName = cell.GetString().Trim().ToLower();

            if (properties.TryGetValue(headerName, out var property))
            {
                columnMap[cell.Address.ColumnNumber] = property;
            }
        }

        var result = new List<T>();

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var obj = new T();

            foreach (var column in columnMap)
            {
                var cell = row.Cell(column.Key);
                var property = column.Value;

                if (cell.IsEmpty())
                    continue;

                try
                {
                    object? value = ConvertCellValue(cell, property.PropertyType);
                    property.SetValue(obj, value);
                }
                catch
                {
                    throw new CommonException(ResponseMessage.CellValueConvertionFailed);
                }
            }

            result.Add(obj);
        }

        return result;
    }

    private static object? ConvertCellValue(IXLCell cell, Type targetType)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType.IsEnum)
            return Enum.Parse(underlyingType, cell.GetString(), true);

        if (underlyingType == typeof(DateTime))
            return cell.GetDateTime();

        if (underlyingType == typeof(int))
            return cell.GetValue<int>();

        if (underlyingType == typeof(decimal))
            return cell.GetValue<decimal>();

        if (underlyingType == typeof(bool))
            return cell.GetValue<bool>();

        if (underlyingType == typeof(List<string>))
        {
            var raw = cell.GetString().Trim();

            return raw
                .Split([ ',', ';', '\n' ], StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        return Convert.ChangeType(cell.GetValue<string>().Trim(), underlyingType);
    }
}

