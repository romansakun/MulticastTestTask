using System;

namespace Infrastructure.LogicUtility
{
    public interface IBuildNode
    {
        Type NodeType {get;}
        IBuildNode Next {get; }
        void DirectTo (IBuildNode next);
    }
}