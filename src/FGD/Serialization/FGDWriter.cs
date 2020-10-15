using FGD.AST;
using System;
using System.IO;
using System.Linq;

namespace FGD.Serialization
{
    public class FGDWriter
    {
        public void Write(SyntaxTree syntaxTree, Stream stream)
        {
            if (syntaxTree is null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }

            using var writer = new StreamWriter(stream);

            Write(syntaxTree, writer);
        }

        public void Write(SyntaxTree syntaxTree, TextWriter writer)
        {
            if (syntaxTree is null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }

            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            foreach (var declaration in syntaxTree.Declarations)
            {
                switch (declaration)
                {
                    case EntityClass entityClass:
                        {
                            //TODO: define tokens as constants
                            writer.Write('@');
                            writer.Write(entityClass.Type.ToString());
                            writer.Write(' ');

                            var editorProperties = string.Join(' ', entityClass.EditorProperties.Select(p => p.GetStringRepresentation()));

                            writer.Write(editorProperties);

                            if (editorProperties.Length > 0)
                            {
                                writer.Write(' ');
                            }

                            writer.Write("= {0}", entityClass.Name);

                            if (entityClass.Description.Length > 0)
                            {
                                writer.Write(" : \"{0}\"", entityClass.Description);
                            }

                            if (entityClass.MapProperties.Any())
                            {
                                writer.WriteLine();
                                writer.WriteLine('[');

                                foreach (var mapProperty in entityClass.MapProperties)
                                {
                                    switch (mapProperty)
                                    {
                                        case KeyValueMapProperty keyValue:
                                            {
                                                writer.Write("\t{0}({1})", keyValue.Name, keyValue.Type);

                                                if (keyValue.Description.Length > 0)
                                                {
                                                    writer.Write(" : \"{0}\"", keyValue.Description);
                                                }

                                                if (keyValue.DefaultValue.Length > 0)
                                                {
                                                    //TODO: shouldn't hardcode type handling
                                                    if (keyValue.Type == "string"
                                                        || keyValue.Type == "sprite"
                                                        || keyValue.DefaultValue.Any(char.IsWhiteSpace))
                                                    {
                                                        writer.Write(" : \"{0}\"", keyValue.DefaultValue);
                                                    }
                                                    else
                                                    {
                                                        writer.Write(" : {0}", keyValue.DefaultValue);
                                                    }
                                                }

                                                //TODO: need a better way to identify list types
                                                if (keyValue.Choices.Count > 0)
                                                {
                                                    writer.WriteLine(" =");
                                                    writer.WriteLine("\t[");

                                                    foreach (var choice in keyValue.Choices)
                                                    {
                                                        writer.Write("\t\t{0} : \"{1}\"", choice.Value, choice.Description);

                                                        if (choice.DefaultValue.Length > 0)
                                                        {
                                                            writer.Write(" : {0}", choice.DefaultValue);
                                                        }

                                                        writer.WriteLine();
                                                    }

                                                    writer.WriteLine("\t]");
                                                }
                                                else
                                                {
                                                    writer.WriteLine();
                                                }
                                                break;
                                            }

                                        default: throw new ArgumentException("Map property type not supported", nameof(syntaxTree));
                                    }
                                }

                                writer.Write(']');
                            }
                            else
                            {
                                writer.Write(" []");
                            }

                            writer.WriteLine();
                            writer.WriteLine();

                            break;
                        }

                    default: throw new ArgumentException("Declaration type not supported", nameof(syntaxTree));
                }
            }
        }
    }
}
