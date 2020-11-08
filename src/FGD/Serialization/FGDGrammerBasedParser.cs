using FGD.AST;
using FGD.Grammar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FGD.Serialization
{
    public class FGDGrammerBasedParser
    {
        public FGDGrammar Grammar { get; }

        public FGDGrammerBasedParser(FGDGrammar grammar)
        {
            Grammar = grammar;
        }

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

        private Declaration ParseDeclaration(Token token, FGDLexer lexer)
        {
            if (Grammar.Declarations.TryGetValue(token.Text, out var declarationGrammar))
            {
                switch (declarationGrammar.Type)
                {
                    case DeclarationType.EntityClass: return ParseEntityClass(declarationGrammar, lexer);
                }
            }

            throw new FGDParseException($"Unknown declaration type \'{token.Text}\'", token.Line, token.Column);
        }

        private EntityClass ParseEntityClass(DeclarationGrammar declarationGrammar, FGDLexer lexer)
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

            var type = Enum.Parse<EntityClassType>(declarationGrammar.Name);

            return new EntityClass(type, entityClassName, description, editorProperties, mapProperties);
        }

        private Token ParseEditorProperties(FGDLexer lexer, Token nextToken, List<EditorProperty> editorProperties)
        {
            while (nextToken.Type == TokenType.UnquotedString)
            {
                var keyName = nextToken.Text;

                if (!Grammar.EditorProperties.TryGetValue(keyName, out var editorProperty))
                {
                    throw new FGDParseException(
                        $"While parsing declaration: unknown editor property \'{nextToken.Text}\'", nextToken.Line, nextToken.Column);
                }

                nextToken = lexer.GetNextToken();

                EnsureValidToken(TokenType.ParenthesisOpen, "'('", nextToken);

                nextToken = lexer.GetNextToken();

                EditorPropertyParameterGrammar? variadicParameter = null;

                var parameters = new List<EditorPropertyParameter>();

                //Set of parameters
                //Parameters are delimited by commas
                for (var parameterIndex = 0; !(variadicParameter is null) || (parameterIndex < editorProperty.Parameters.Length); ++parameterIndex)
                {
                    var parameterGrammar = variadicParameter ?? editorProperty.Parameters[parameterIndex];

                    if (parameterGrammar.IsOptional && nextToken.Type == TokenType.ParenthesisClose)
                    {
                        break;
                    }

                    if (parameterGrammar.IsVariadic)
                    {
                        variadicParameter = parameterGrammar;
                    }

                    if (parameterGrammar.Type == DataType.Text || parameterGrammar.Type == DataType.FilePath)
                    {
                        //Single quoted string
                        EnsureValidToken(TokenType.QuotedString, "quoted string", nextToken);

                        parameters.Add(new EditorPropertyParameter(nextToken.Text, true));

                        nextToken = lexer.GetNextToken();
                    }
                    else
                    {
                        //One or more unquoted strings
                        EnsureValidToken(TokenType.UnquotedString, "unquoted string", nextToken);

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

        private void ParseMapProperties(FGDLexer lexer, Token nextToken, List<MapProperty> mapProperties)
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

                if (!Grammar.KeyValueTypes.TryGetValue(valueType, out var dataType))
                {
                    throw new FGDParseException(
                            $"While parsing declaration: unknown keyvalue data type \'{nextToken.Text}\'", nextToken.Line, nextToken.Column);
                }

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

                    switch (dataType.DataType)
                    {
                        case DataType.Text:
                        case DataType.FilePath:
                        case DataType.EntityClassReference:
                        case DataType.Float1:
                        case DataType.Float2:
                        case DataType.Float3:
                        case DataType.Float4:
                        //Multiple integers are formatted using quotes
                        case DataType.Int2:
                        case DataType.Int3:
                        case DataType.Int4:
                            {
                                EnsureValidToken(TokenType.QuotedString, "quoted default value", nextToken);
                                break;
                            }

                        case DataType.Int1:
                            {
                                EnsureValidToken(TokenType.UnquotedString, "unquoted default value", nextToken);
                                break;
                            }

                        default:
                            throw new FGDParseException(
                                $"While parsing declaration: unknown keyvalue data type, got \'{nextToken.Text}\'", nextToken.Line, nextToken.Column);
                    }

                    defaultValue = nextToken.Text;

                    if (dataType.DataType >= DataType.Float1 && dataType.DataType <= DataType.Float4)
                    {
                        foreach (var value in defaultValue.Split(' ', '\t'))
                        {
                            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                            {
                                throw new FGDParseException(
                                    $"While parsing declaration: Default value \'{nextToken.Text}\' is not a floating point value",
                                    nextToken.Line, nextToken.Column);
                            }
                        }
                    }
                    else if (dataType.DataType >= DataType.Int1 && dataType.DataType <= DataType.Int4)
                    {
                        foreach (var value in defaultValue.Split(' ', '\t'))
                        {
                            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                            {
                                throw new FGDParseException(
                                    $"While parsing declaration: Default value \'{nextToken.Text}\' is not an integer value", nextToken.Line, nextToken.Column);
                            }
                        }
                    }
                    //TODO: validate filepaths

                    nextToken = lexer.GetNextToken();
                }

                var choices = new List<KeyValueChoice>();

                if (dataType.ListType != KeyValueListType.None)
                {
                    //List of choices/flags
                    if (nextToken.Type == TokenType.Assignment)
                    {
                        nextToken = lexer.GetNextToken();

                        EnsureValidToken(TokenType.BracketOpen, "'['", nextToken);

                        nextToken = lexer.GetNextToken();

                        //Parse choices/flags
                        //Choices are formatted as: value : "description"
                        //Flags are formatted as: value : "description" : default_value (0|1)
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

                            if (dataType.ListType == KeyValueListType.FlagList)
                            {
                                if (nextToken.Type == TokenType.Colon)
                                {
                                    nextToken = lexer.GetNextToken();

                                    EnsureValidToken(TokenType.UnquotedString, "choice description", nextToken);

                                    flagDefaultValue = nextToken.Text;

                                    if (!int.TryParse(flagDefaultValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var flagDefaultValueInt)
                                        || (flagDefaultValueInt != 0 && flagDefaultValueInt != 1))
                                    {
                                        throw new FGDParseException(
                                            $"While parsing declaration: Default flag value \'{nextToken.Text}\' is not a valid value",
                                            nextToken.Line, nextToken.Column);
                                    }

                                    nextToken = lexer.GetNextToken();
                                }
                            }

                            choices.Add(new KeyValueChoice(choiceValue, choiceDescription, flagDefaultValue));
                        }

                        nextToken = lexer.GetNextToken();
                    }
                }

                mapProperties.Add(new KeyValueMapProperty(keyName, valueType, keyDescription, defaultValue, choices));
            }
        }
    }
}
