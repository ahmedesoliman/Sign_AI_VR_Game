namespace GameCreator.Core
{
    using GameCreator.Core.Math;

    public static class ExpressionEvaluator
    {
        public static float Evaluate(string expression)
        {
            return Expression.Evaluate(expression);
        }
    }
}