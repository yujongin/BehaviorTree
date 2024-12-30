using System;

public class ActionNode : INode
{
    Func<NodeState> onUpdate = null;

    public ActionNode(Func<NodeState> onUpdate)
    {
        this.onUpdate = onUpdate;
    }

    public NodeState Evaluate()
    {
        return onUpdate?.Invoke() ?? NodeState.Failure;
    }
}
