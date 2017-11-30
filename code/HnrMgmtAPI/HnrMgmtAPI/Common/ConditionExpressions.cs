using HnrMgmtAPI.Models.API.Record;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HnrMgmtAPI.Common
{
    public static class ConditionExpressions
    {
        //构建表达式树
        public static Expression<Func<T, bool>> GetConditionExpression<T>(List<ConditionModel> conditions)
        {
            ParameterExpression left = Expression.Parameter(typeof(T), "c");
            Expression expression = Expression.Constant(false);
            foreach (ConditionModel conditionItem in conditions)
            {
                if (typeof(T).GetProperty(conditionItem.fieldName) != null)
                {
                    string fieldName = conditionItem.fieldName;
                    foreach (var optionName in conditionItem.fieldValues)
                    {
                        Expression right = Expression.Call(Expression.Property(left, typeof(T).GetProperty(fieldName)), typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(optionName));
                        expression = Expression.Or(right, expression);
                    }
                }
            }
            Expression<Func<T, bool>> finalExpression = Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { left });
            return finalExpression;
        }
    }
}