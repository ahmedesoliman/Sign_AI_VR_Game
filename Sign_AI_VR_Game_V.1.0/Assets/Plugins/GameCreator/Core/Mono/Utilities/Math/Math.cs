namespace GameCreator.Core.Math
{
    public static class Expression
    {
        public static float Evaluate(string expression)
        {
            Parser parser = new Parser(expression);
            return parser.Evaluate();
        }
    }
}