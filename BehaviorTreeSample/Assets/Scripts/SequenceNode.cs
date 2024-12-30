using System.Collections.Generic;

public class SequenceNode : INode
{
    List<INode> childs;

    public SequenceNode(List<INode> childs)
    {
        this.childs = childs;
    }

    public NodeState Evaluate()
    {
        if (childs == null || childs.Count == 0)
        {
            return NodeState.Failure;
        }

        foreach (INode child in childs)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    continue;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Running:
                    return NodeState.Running;
            }
        }

        return NodeState.Success;
    }
}
