using Dapper;
using System.Data;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
        => parameter.Value = value.ToString(); // store as string in DB

    public override Guid Parse(object value)
        => Guid.TryParse(value.ToString(), out var guid) ? guid : Guid.Empty;
}