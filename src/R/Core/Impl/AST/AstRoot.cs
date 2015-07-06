﻿using Microsoft.Languages.Core.Text;
using Microsoft.R.Core.AST.Definitions;
using Microsoft.R.Core.AST.Evaluation.Definitions;
using Microsoft.R.Core.AST.Scopes;
using Microsoft.R.Core.Evaluation;
using Microsoft.R.Core.Parser;

namespace Microsoft.R.Core.AST
{
    public sealed class AstRoot : AstNode
    {
        public ITextProvider TextProvider { get; private set; }

        public TextRangeCollection<TokenNode> Comments { get; private set; }

        public ICodeEvaluator CodeEvaluator { get; private set; }

        public AstRoot(ITextProvider textProvider) :
            this(textProvider, new CodeEvaluator())
        {
        }

        public AstRoot(ITextProvider textProvider, ICodeEvaluator codeEvaluator)
        {
            this.TextProvider = textProvider;
            this.Comments = new TextRangeCollection<TokenNode>();
            this.CodeEvaluator = codeEvaluator;
        }

        #region IAstNode
        public override AstRoot Root
        {
            get { return this; }
        }
        #endregion

        public override bool Parse(ParseContext context, IAstNode parent)
        {
            // Remove comments from the token stream
            context.RemoveCommentTokens();

            GlobalScope globalScope = new GlobalScope();
            return globalScope.Parse(context, this);
        }
    }
}
