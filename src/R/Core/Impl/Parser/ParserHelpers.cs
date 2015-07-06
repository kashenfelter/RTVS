﻿using System.Diagnostics;
using Microsoft.Languages.Core.Tokens;
using Microsoft.R.Core.AST;
using Microsoft.R.Core.AST.Definitions;
using Microsoft.R.Core.AST.Scopes;
using Microsoft.R.Core.AST.Scopes.Definitions;
using Microsoft.R.Core.Tokens;

namespace Microsoft.R.Core.Parser
{
    public sealed partial class RParser
    {
        public static TokenNode ParseKeyword(ParseContext context, IAstNode parent)
        {
            TokenStream<RToken> tokens = context.Tokens;

            Debug.Assert(tokens.CurrentToken.TokenType == RTokenType.Keyword);

            TokenNode keyword = new TokenNode();
            keyword.Parse(context, parent);

            return keyword;
        }

        public static TokenNode ParseToken(ParseContext context, IAstNode parent)
        {
            TokenStream<RToken> tokens = context.Tokens;
            TokenNode node = new TokenNode();

            node.Parse(context, parent);
            return node;
        }

        public static TokenNode ParseOpenBraceSequence(ParseContext context, IAstNode parent)
        {
            TokenStream<RToken> tokens = context.Tokens;

            RParser.ParseKeyword(context, parent);

            if (tokens.CurrentToken.TokenType == RTokenType.OpenBrace)
            {
                TokenNode openBrace = new TokenNode();
                openBrace.Parse(context, parent);

                return openBrace;
            }
            else
            {
                context.Errors.Add(new ParseError(ParseErrorType.OpenBraceExpected, tokens.CurrentToken));
            }

            return null;
        }

        public static TokenNode ParseCloseBraceSequence(ParseContext context, IAstNode parent)
        {
            TokenStream<RToken> tokens = context.Tokens;

            if (tokens.CurrentToken.TokenType == RTokenType.CloseBrace)
            {
                return RParser.ParseToken(context, parent);
            }

            context.Errors.Add(new ParseError(ParseErrorType.CloseBraceExpected, tokens.CurrentToken));
            return null;
        }

        public static IScope ParseScope(ParseContext context, IAstNode parent, bool allowsSimpleScope)
        {
            TokenStream<RToken> tokens = context.Tokens;
            IScope scope;

            if (tokens.CurrentToken.TokenType == RTokenType.OpenCurlyBrace)
            {
                scope = new Scope(string.Empty);
                if (scope.Parse(context, parent))
                {
                    return scope;
                }
            }
            else if (allowsSimpleScope)
            {
                // Try simple on-line scope as in 'for(...) statement # comment'
                scope = new SimpleScope();
                if (scope.Parse(context, parent))
                {
                    return scope;
                }
            }
            else
            {
                context.Errors.Add(new ParseError(ParseErrorType.OpenCurlyBraceExpected, tokens.CurrentToken));
            }

            return null;
        }

        public static RTokenType GetTerminatingTokenType(RTokenType openingTokenType)
        {
            switch (openingTokenType)
            {
                case RTokenType.OpenSquareBracket:
                    return RTokenType.CloseSquareBracket;

                case RTokenType.OpenDoubleSquareBracket:
                    return RTokenType.CloseDoubleSquareBracket;

                case RTokenType.OpenCurlyBrace:
                    return RTokenType.CloseCurlyBrace;

                case RTokenType.OpenBrace:
                    return RTokenType.CloseBrace;

                default:
                    Debug.Assert(false, "Unable to determine closing token type");
                    break;
            }

            return RTokenType.Unknown;
        }

        public static bool IsScopeSeparator(RTokenType tokenType)
        {
            if (tokenType == RTokenType.OpenCurlyBrace || tokenType == RTokenType.CloseCurlyBrace)
            {
                return true;
            }

            return false;
        }
    }
}
