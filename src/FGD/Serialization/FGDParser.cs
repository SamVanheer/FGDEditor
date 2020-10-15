using FGD.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FGD.Serialization
{
    public sealed class FGDParser
    {
        /// <summary>
        /// Parses a given stream of text into a syntax tree
        /// </summary>
        /// <param name="stream"></param>
        public SyntaxTree Parse(Stream stream)
        {
            using var reader = new StreamReader(stream);

            return Parse(reader);
        }

        public SyntaxTree Parse(TextReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var lexer = new FGDLexer(reader)
            {
                //Skip whitespace automatically
                SkipWhitespace = true,
                SkipComments = true
            };

            var declarations = new List<Declaration>();

            for (var token = lexer.GetNextToken(); token.IsValid; token = lexer.GetNextToken())
            {
                if (token.Type == TokenType.Declaration)
                {
                    declarations.Add(ParseDeclaration(token, lexer));
                }
            }

            return new SyntaxTree(declarations);
        }

        private static void EnsureValidToken(TokenType expectedType, string expectedMessage, Token token)
        {
            if (token.Type != expectedType)
            {
                throw new FGDParseException($"While parsing declaration: expected {expectedMessage}, got \'{token.Text}\'", token.Line, token.Column);
            }
        }

        private static Declaration ParseDeclaration(Token token, FGDLexer lexer)
        {
            switch (token.Text)
            {
                case nameof(EntityClassType.BaseClass):
                case nameof(EntityClassType.PointClass):
                case nameof(EntityClassType.SolidClass):
                    {
                        var nextToken = lexer.GetNextToken();

                        //Optional editor properties
                        var editorProperties = new List<EditorProperty>();

                        nextToken = ParseEditorProperties(lexer, nextToken, editorProperties);

                        EnsureValidToken(TokenType.Assignment, "'='", nextToken);

                        nextToken = lexer.GetNextToken();

                        EnsureValidToken(TokenType.UnquotedString, "entity class name", nextToken);

                        var entityClassName = nextToken.Text;

                        nextToken = lexer.PeekNextToken();

                        var description = string.Empty;

                        if (nextToken.Type == TokenType.Colon)
                        {
                            lexer.GetNextToken();

                            nextToken = lexer.GetNextToken();

                            EnsureValidToken(TokenType.QuotedString, "entity description", nextToken);

                            description = nextToken.Text;

                            nextToken = lexer.PeekNextToken();
                        }

                        var mapProperties = new List<MapProperty>();

                        ParseMapProperties(lexer, nextToken, mapProperties);

                        var type = Enum.Parse<EntityClassType>(token.Text);

                        return new EntityClass(type, entityClassName, description, editorProperties, mapProperties);
                    }

                default: throw new FGDParseException($"Unknown declaration \'{token.Text}\'", token.Line, token.Column);
            }
        }

        private static Token ParseEditorProperties(FGDLexer lexer, Token nextToken, List<EditorProperty> editorProperties)
        {
            while (nextToken.Type == TokenType.UnquotedString)
            {
                var keyName = nextToken.Text;

                nextToken = lexer.GetNextToken();

                EnsureValidToken(TokenType.ParenthesisOpen, "'('", nextToken);

                nextToken = lexer.GetNextToken();

                var parameters = new List<EditorPropertyParameter>();

                //Set of parameters
                //Parameters are delimited by commas
                //Parameters are one or more unquoted strings, or a single quoted string
                while (nextToken.Type != TokenType.ParenthesisClose)
                {
                    if (nextToken.Type == TokenType.QuotedString)
                    {
                        //Single quoted string
                        parameters.Add(new EditorPropertyParameter(nextToken.Text, true));

                        nextToken = lexer.GetNextToken();
                    }
                    else if (nextToken.Type == TokenType.UnquotedString)
                    {
                        //One or more unquoted strings
                        var parameter = new StringBuilder(nextToken.Text);

                        nextToken = lexer.GetNextToken();

                        while (nextToken.Type == TokenType.UnquotedString)
                        {
                            parameter
                                .Append(' ')
                                .Append(nextToken.Text);

                            nextToken = lexer.GetNextToken();
                        }

                        parameters.Add(new EditorPropertyParameter(parameter.ToString(), false));
                    }

                    if (nextToken.Type == TokenType.Comma)
                    {
                        nextToken = lexer.GetNextToken();
                    }
                    else
                    {
                        EnsureValidToken(TokenType.ParenthesisClose, "',' or ')'", nextToken);
                    }
                }

                editorProperties.Add(new EditorProperty(keyName, parameters));

                nextToken = lexer.GetNextToken();
            }

            return nextToken;
        }

        private static void ParseMapProperties(FGDLexer lexer, Token nextToken, List<MapProperty> mapProperties)
        {
            lexer.GetNextToken();

            EnsureValidToken(TokenType.BracketOpen, "'['", nextToken);

            nextToken = lexer.GetNextToken();

            //Parse map properties
            while (nextToken.Type != TokenType.BracketClose)
            {
                EnsureValidToken(TokenType.UnquotedString, "key name", nextToken);

                var keyName = nextToken.Text;

                nextToken = lexer.GetNextToken();

                EnsureValidToken(TokenType.ParenthesisOpen, "'('", nextToken);

                nextToken = lexer.GetNextToken();

                EnsureValidToken(TokenType.UnquotedString, "value type", nextToken);

                var valueType = nextToken.Text;

                nextToken = lexer.GetNextToken();

                EnsureValidToken(TokenType.ParenthesisClose, "')'", nextToken);

                nextToken = lexer.GetNextToken();

                //Optional description
                //TODO: is this really optional?
                var keyDescription = string.Empty;

                if (nextToken.Type == TokenType.Colon)
                {
                    nextToken = lexer.GetNextToken();

                    EnsureValidToken(TokenType.QuotedString, "key description", nextToken);

                    keyDescription = nextToken.Text;

                    nextToken = lexer.GetNextToken();
                }

                //Optional default value
                var defaultValue = string.Empty;

                if (nextToken.Type == TokenType.Colon)
                {
                    nextToken = lexer.GetNextToken();

                    if (nextToken.Type != TokenType.QuotedString && nextToken.Type != TokenType.UnquotedString)
                    {
                        throw new FGDParseException(
                            $"While parsing declaration: expected default value, got \'{nextToken.Text}\'", nextToken.Line, nextToken.Column);
                    }

                    defaultValue = nextToken.Text;

                    nextToken = lexer.GetNextToken();
                }

                //Optional list of choices/flags
                //TODO: verify that the value type supports this
                var choices = new List<KeyValueChoice>();

                if (nextToken.Type == TokenType.Assignment)
                {
                    nextToken = lexer.GetNextToken();

                    EnsureValidToken(TokenType.BracketOpen, "'['", nextToken);

                    nextToken = lexer.GetNextToken();

                    //Parse choices/flags
                    //Choices are formatted as: value : "description"
                    //Flags are formatted as: value : "description" : default_value (0|1)
                    //TODO: verify that the correct syntax is being used for the given type
                    while (nextToken.Type != TokenType.BracketClose)
                    {
                        EnsureValidToken(TokenType.UnquotedString, "choice value", nextToken);

                        var choiceValue = nextToken.Text;

                        nextToken = lexer.GetNextToken();

                        EnsureValidToken(TokenType.Colon, "':'", nextToken);

                        nextToken = lexer.GetNextToken();

                        EnsureValidToken(TokenType.QuotedString, "choice description", nextToken);

                        var choiceDescription = nextToken.Text;

                        nextToken = lexer.GetNextToken();

                        var flagDefaultValue = string.Empty;

                        if (nextToken.Type == TokenType.Colon)
                        {
                            nextToken = lexer.GetNextToken();

                            EnsureValidToken(TokenType.UnquotedString, "choice description", nextToken);

                            flagDefaultValue = nextToken.Text;

                            nextToken = lexer.GetNextToken();
                        }

                        choices.Add(new KeyValueChoice(choiceValue, choiceDescription, flagDefaultValue));
                    }

                    nextToken = lexer.GetNextToken();
                }

                mapProperties.Add(new KeyValueMapProperty(keyName, valueType, keyDescription, defaultValue, choices));
            }
        }
    }
}
