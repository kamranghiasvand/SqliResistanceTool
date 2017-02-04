using System.ComponentModel.DataAnnotations;

namespace SqliResistanceModel
{
    public class FormInputModel
    {
        [Key]
        public int Id { get; set; }
        public InputType Type { get; set; }
        public string Value { get; set; }
        [DataType(DataType.Url)]
        public string Src { get; set; }
        public int Size { get; set; }
        public bool Readonly { get; set; }
        public string Name { get; set; }
        public string Alt { get; set; }
        public bool Checked { get; set; }
        public int Maxlength { get; set; }
        public virtual FormModel Form { get; set; }
        public bool Disabled { get; set; }
    }

    public enum InputType
    {
        Button,
        Checkbox,
        Color,
        Date,
        DatetimeLocal,
        Email,
        File,
        Hidden,
        Image,
        Month,
        Number,
        Password,
        Radio,
        Range,
        Reset,
        Search,
        Submit,
        Tel,
        Text,
        Time,
        Url,
        Week
    }
}
