namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class KeyValueChoiceModel
    {
        public string Value { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public KeyValueChoiceModel(string value, string description, string defaultValue)
        {
            Value = value;
            Description = description;
            DefaultValue = defaultValue;
        }
    }
}
