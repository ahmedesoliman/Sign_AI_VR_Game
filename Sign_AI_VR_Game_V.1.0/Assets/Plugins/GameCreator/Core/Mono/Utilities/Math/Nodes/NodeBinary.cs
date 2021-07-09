namespace GameCreator.Core.Math
{
    using System;

    public class NodeBinary : Node
    {
        private readonly Node lhs;
        private readonly Node rhs;
        private readonly Func<float, float, float> op;

        // INITIALIZER: ---------------------------------------------------------------------------

        public NodeBinary(Node lhs, Node rhs, Func<float, float, float> op)
        {
            this.lhs = lhs;
            this.rhs = rhs;
            this.op = op;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Evaluate()
        {
            float lhsVal = this.lhs.Evaluate();
            float rhsVal = this.rhs.Evaluate();

            return this.op(lhsVal, rhsVal);
        }
    }
}