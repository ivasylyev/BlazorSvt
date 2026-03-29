namespace BlazorSvt.Utils;

using Dapper;
using System.Data;

public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    // Как записывать DateOnly в базу
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToDateTime(new TimeOnly(0, 0));
        parameter.DbType = DbType.Date;
    }

    // Как считывать DateOnly из базы
    public override DateOnly Parse(object value)
    {
        if (value is DateTime dateTime)
            return DateOnly.FromDateTime(dateTime);

        return DateOnly.Parse(value.ToString());
    }
}
