using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Args;

namespace SafenetSign
{
    [ArgsModel(SwitchDelimiter = "-")]
    public class CommandParameters
    {
        [ArgsMemberSwitch(0)]
        [Required]
        [Description("The certificate thumbprint")]
        public string Thumbprint { get; set; }

        [ArgsMemberSwitch(1)]
        [Required]
        [Description("The name of the key container containing the private key")]
        public string PrivateKeyContainer { get; set; }

        [ArgsMemberSwitch(2)]
        [DefaultValue(CertificateStore.User)]
        [Required]
        [Description("The certificate store to search through (User or Machine)")]
        public CertificateStore Store { get; set; }

        [ArgsMemberSwitch(3)]
        [Required]
        [Description("The PIN protecting the private key")]
        public string Pin { get; set; }

        [ArgsMemberSwitch(4)]
        [Required]
        [Description("The timestamp URL")]
        public string TimestampUrl { get; set; }

        [ArgsMemberSwitch(5)]
        [DefaultValue(SignMode.PE)]
        [Required]
        [Description("The signing mode (PE or APPX)")]
        public SignMode Mode { get; set; }

        [ArgsMemberSwitch(6)]
        [Required]
        [Description("The path to the file to sign")]
        public string Path { get; set; }
    }
}