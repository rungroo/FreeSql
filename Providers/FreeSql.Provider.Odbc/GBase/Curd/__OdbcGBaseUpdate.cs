﻿using FreeSql.Internal;
using FreeSql.Internal.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSql.Odbc.GBase
{

    class OdbcGBaseUpdate<T1> : Internal.CommonProvider.UpdateProvider<T1> where T1 : class
    {

        public OdbcGBaseUpdate(IFreeSql orm, CommonUtils commonUtils, CommonExpression commonExpression, object dywhere)
            : base(orm, commonUtils, commonExpression, dywhere)
        {
        }

        public override int ExecuteAffrows() => base.SplitExecuteAffrows(500, 3000);
        public override List<T1> ExecuteUpdated() => base.SplitExecuteUpdated(500, 3000);

        protected override List<T1> RawExecuteUpdated()
        {
            throw new NotImplementedException();
        }

        protected override void ToSqlCase(StringBuilder caseWhen, ColumnInfo[] primarys)
        {
            if (_table.Primarys.Length == 1)
            {
                caseWhen.Append(_commonUtils.QuoteReadColumn(_table.Primarys.First().Attribute.MapType, _commonUtils.QuoteSqlName(_table.Primarys.First().Attribute.Name)));
                return;
            }
            caseWhen.Append("(");
            var pkidx = 0;
            foreach (var pk in _table.Primarys)
            {
                if (pkidx > 0) caseWhen.Append(" || ");
                caseWhen.Append(_commonUtils.QuoteReadColumn(pk.Attribute.MapType, _commonUtils.QuoteSqlName(pk.Attribute.Name))).Append("::varchar");
                ++pkidx;
            }
            caseWhen.Append(")");
        }

        protected override void ToSqlWhen(StringBuilder sb, ColumnInfo[] primarys, object d)
        {
            if (_table.Primarys.Length == 1)
            {
                sb.Append(_commonUtils.FormatSql("{0}", _table.Primarys.First().GetMapValue(d)));
                return;
            }
            sb.Append("(");
            var pkidx = 0;
            foreach (var pk in _table.Primarys)
            {
                if (pkidx > 0) sb.Append(" || ");
                sb.Append(_commonUtils.FormatSql("{0}", pk.GetMapValue(d))).Append("::varchar");
                ++pkidx;
            }
            sb.Append(")");
        }

        protected override void ToSqlCaseWhenEnd(StringBuilder sb, ColumnInfo col)
        {
            if (_noneParameter == false) return;
            var dbtype = _commonUtils.CodeFirst.GetDbInfo(col.Attribute.MapType)?.dbtype;
            if (dbtype == null) return;

            sb.Append("::").Append(dbtype);
        }

#if net40
#else
        public override Task<int> ExecuteAffrowsAsync() => base.SplitExecuteAffrowsAsync(500, 3000);
        public override Task<List<T1>> ExecuteUpdatedAsync() => base.SplitExecuteUpdatedAsync(500, 3000);
        
        protected override Task<List<T1>> RawExecuteUpdatedAsync()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
