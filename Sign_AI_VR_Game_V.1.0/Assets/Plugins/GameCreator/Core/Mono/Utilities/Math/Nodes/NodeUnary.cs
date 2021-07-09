namespace GameCreator.Core.Math
{
    using System;

    public class NodeUnary : Node
    {
        private readonly Node rhs;
        private readonly Func<float, float> op;

        // INITIALIZER: ---------------------------------------------------------------------------

        public NodeUnary(Node rhs, Func<float, float> op)
        {
            this.rhs = rhs;
            this.op = op;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Evaluate()
        {
            float rhsVal = this.rhs.Evaluate();
            return this.op(rhsVal);
        }
    }
}