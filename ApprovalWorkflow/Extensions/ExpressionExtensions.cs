using System.Linq.Expressions;

namespace ApprovalSystem.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two expressions into a single expression using the AND clause
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>A single expression which is an expression of both <paramref name="first"/> AND <paramref name="second"/></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // Replace parameters in the second expression with the first's parameter
            // so they have matching parameters when compiling
            var left = new ReplaceParameterVisitor(first.Parameters[0], parameter)
                .Visit(first.Body);
            var right = new ReplaceParameterVisitor(second.Parameters[0], parameter)
                .Visit(second.Body);

            var body = Expression.AndAlso(left, right);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            public ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
