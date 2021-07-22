namespace GameCreator.Core.Math
{
    public class NodeNumber : Node
    {
        private readonly float number = 0.0f;

        // INITIALIZERS: --------------------------------------------------------------------------

        public NodeNumber(float number)
        {
            this.number = number;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Evaluate()
        {
            return this.number;
        }
    }
}