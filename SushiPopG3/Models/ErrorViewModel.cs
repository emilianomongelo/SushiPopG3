namespace SushiPopG3.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public const string CampoRequerido = "{0} es un campo requerido";
        public const string CaracteresMax = "{0} no debe superar los {1} caracteres";
        public const string CaracteresMin = "{0} debe superar los {1} caracteres";
        public const string IgualCantidadCaracteres = "{0} debe contener {1] caracteres";
        public const string FormatoMail = "Debe tener formato de email";
        public const string MensajeError = "Error";
    }
}