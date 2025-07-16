using System.Net;
using System.Net.Mail;

namespace Backend
{
    public class EmailService
    {
        private readonly string _correoOrigen = "huecasya@gmail.com"; // TU CORREO
        private readonly string _contrasenaApp = "xrys pksm kubc sysn"; // CONTRASEÑA DE APP

        public async Task EnviarCodigoAsync(string destinatario, string codigo)
        {
            var mensaje = new MailMessage();
            mensaje.From = new MailAddress(_correoOrigen, "HuecasYa");
            mensaje.To.Add(destinatario);
            mensaje.Subject = "Código de verificación";
            mensaje.Body = $"Tu código es: {codigo}";
            mensaje.IsBodyHtml = false;

            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_correoOrigen, _contrasenaApp);

            await smtp.SendMailAsync(mensaje);
        }
    }
}
