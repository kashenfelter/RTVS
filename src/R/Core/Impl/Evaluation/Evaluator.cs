﻿using Microsoft.R.Core.AST.DataTypes;
using Microsoft.R.Core.AST.Definitions;
using Microsoft.R.Core.AST.Evaluation.Definitions;

namespace Microsoft.R.Core.Evaluation
{
    public sealed class CodeEvaluator : ICodeEvaluator
    {
        public RObject Evaluate(IAstNode node)
        {
            IRValueNode rValue = node as IRValueNode;
            if (rValue == null)
            {
                return RNull.Null;
            }

            return RNull.Null;
        }
    }
}
