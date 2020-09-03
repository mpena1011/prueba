using ConsultaFEI;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using System.Globalization;
using System.Xml;

public class EmailJob : IJob
{
    public List<clasEntityDocument> cabeceras = null;
    public string ruta_error = System.AppDomain.CurrentDomain.BaseDirectory + "Info\\log_email.log";
    public string mensaje = String.Empty;
    /// <summary>
    /// Metodo que realiza el envio del emailsegun el periodo especificado en el JobScheduler
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
        var REmail = GetValueRegistro("email");
        var ENombre = GetValueRegistro("nombre");
        var ECliente = GetValueRegistro("smtp_cliente");
        var EPuerto = GetValueRegistro("smtp_puerto");
        var EUsuario = GetValueRegistro("smtp_usuario");
        var EPassword = GetValueRegistro("smtp_clave");

        clasEntityDocument c = new clasEntityDocument();
        try
        {
            cabeceras = c.cs_pxBuscarDocumentosNotSend();
            if (cabeceras != null)
            {
                foreach (var cabecera in cabeceras)
                {//generar xml y pdf para envio
                    try
                    {
                        if (cabecera.Cs_Email_Cliente != "")
                        {                           
                            string[] rutas = Documento.getRutas(cabecera.Cs_pr_Document_Id);
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient(ECliente);
                            //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía

                            mail.From = new MailAddress(REmail, ENombre, Encoding.UTF8);
                            //Aquí ponemos el asunto del correo
                            mail.Subject = "Documentos electronicos";
                            //Aquí ponemos el mensaje que incluirá el correo

                            string cuerpo = "";
                            cuerpo += "Estimado cliente: <br>"; 
                            cuerpo += "Por la presente le comunicamos que la empresa LPF SERVICIOS INTEGRALES S.A.C. con RUC 20521180006, <br>";
                            cuerpo += "emisora de comprobantes electronicos le ha emitido el siguiente comprobante <br><br>";
                            cuerpo += " <b>Tipo de comprobante</b>: " + getTipoDoc(cabecera.Cs_tag_InvoiceTypeCode) + " <br> <b>Documento</b>:" + cabecera.Cs_tag_ID + " <br> <b>Fecha</b>: " + cabecera.Cs_tag_IssueDate + " <br>";
                           // cuerpo += "<br><br>Le informamos ademas que a traves del portal de contasic.org puede realizar la consulta de sus documentos.";
                            cuerpo += "<br><br>Atentamente.";
                            cuerpo += "<br>Area de facturación";
                            mail.Body = cuerpo;

                            mail.BodyEncoding = System.Text.Encoding.UTF8;
                            mail.IsBodyHtml = true;
                            //Especificamos a quien enviaremos el Email ->deberia venir desde base de datos FEI.
                            mail.To.Add(cabecera.Cs_Email_Cliente);
                            //Para enviar archivos adjuntos tenemos que especificar la ruta en donde se encuentran
                            mail.Attachments.Add(new Attachment(GetStreamFile(rutas[0]), Path.GetFileName(rutas[0]), "application/pdf"));
                            mail.Attachments.Add(new Attachment(GetStreamFile(rutas[1]), Path.GetFileName(rutas[1])));
                            //Configuracion del SMTP
                            SmtpServer.Port = Convert.ToInt32(EPuerto); //Puerto que utiliza mail para sus servicios
                                                                        //Especificamos las credenciales con las que enviaremos el mail
                            SmtpServer.Credentials = new System.Net.NetworkCredential(EUsuario, EPassword);
                            SmtpServer.EnableSsl = true;
                            SmtpServer.Send(mail);

                            c.cs_pxActualizarEstado("1", cabecera.Cs_pr_Document_Id);
                        }

                    }
                    catch (Exception ex)
                    {
                        mensaje = ex.ToString();
                        bool flag = !File.Exists(ruta_error);
                        if (flag)
                        {
                            File.Create(ruta_error).Close();
                        }
                        TextWriter tw = new StreamWriter(ruta_error, true);
                        tw.WriteLine("<>" + DateTime.Now.ToString() + ": Ocurrencia: " + mensaje);
                        tw.Close();
                    }
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            mensaje = ex.ToString();
            bool flag = !File.Exists(ruta_error);
            if (flag)
            {
                File.Create(ruta_error).Close();
            }
            TextWriter tw = new StreamWriter(ruta_error, true);
            tw.WriteLine("<>" + DateTime.Now.ToString() + ": Ocurrencia: " + mensaje);
            tw.Close();
        }
        

    }
    /// <summary>
    /// Obtiene el tipo de documento segun el codigo.
    /// </summary>
    /// <param name="codigo"></param>
    /// <returns>Cadena del tipo documento.</returns>
    public string getTipoDoc(string codigo)
    {

        string retorno = String.Empty;
        switch (codigo)
        {
            case "01":
                retorno = "Factura Electronica";
                break;
            case "03":
                retorno = "Boleta de Venta Electronica";
                break;
            case "07":
                retorno = "Nota de Credito Electronica";
                break;
            case "08":
                retorno = "Nota de Debito Electronica";
                break;

        }

        return retorno;
    }
    /// <summary>
    /// Obtiene el asociado a una clave en la tabla registro. 
    /// </summary>
    /// <param name="clave"></param>
    /// <returns>Valor asociado a la clave.</returns>
    public string GetValueRegistro(string clave)
    {
        string valor = string.Empty; ;     
        try
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM AspRegistros WHERE (clave LIKE '%" + clave + "%')", con);
            cmd.ExecuteNonQuery();
            var reader = cmd.ExecuteReader();
            reader.Read();
            var s = reader["valor"];
            con.Close();

            if (s != null || s.ToString() != "")
            {
                valor = s.ToString();
            }
            else
            {
                valor = "";
            }
        }
        catch (Exception)
        {          
            valor = "";
        }
        return valor;
    }
    /// <summary>
    /// Obtiene el Stream de un archivo.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>Stream:secuencia de datos</returns>
    public Stream GetStreamFile(string filePath)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            MemoryStream memStream = new MemoryStream();
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

            return memStream;
        }
    }

}