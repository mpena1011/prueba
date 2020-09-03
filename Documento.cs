using ConsultaFEI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

/// <summary>
/// Descripción breve de Documento
/// </summary>
public class Documento
{
    // ENCAPSULADORES
    public string Codigo { get; set; }
    public string Imagen { get; set; }
    public string Nombre { get; set; }
    public string TextoRepresentacionImpresa { get; set; }


    // CONSTRUCTORES SIN PARAMETROS

    public Documento()
    {
    }

    // CONSTRUCTORES CON PARAMETROS

    public Documento(string codigo, string urlImagen, string nombre, string texto)
    {
        this.Codigo = codigo;
        this.Imagen = urlImagen;
        this.Nombre = nombre;
        this.TextoRepresentacionImpresa = texto;
    }
 
    // ARRAY DE RUTA Y METODO 
    public static string[] getRutas(string idDocumento)
    {
        clasEntityDocument cabecera;
        var idDoc = idDocumento;
        var url = "";
        var doc_serie = "";
        var doc_correlativo = "";
        string[] rutas = new string[4];
        rutas[0] = "";
        rutas[1] = "";
        rutas[2] = "";
        rutas[3] = "";
        try
        {
            cabecera = new clasEntityDocument();
            cabecera.cs_fxObtenerUnoPorId(idDoc);

            if (cabecera != null)
            {

                string[] partes = cabecera.Cs_tag_ID.Split('-');
                DateTime dt = DateTime.ParseExact(cabecera.Cs_tag_IssueDate, "yyyy-MM-dd", null);
                doc_serie = partes[0];
                doc_correlativo = partes[1];


                // CREACION DE PDF
                string newFile = HostingEnvironment.MapPath("~/PDF/" + cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID + "_" + doc_serie + "_" + doc_correlativo + ".pdf");
                string newFileServer = "/PDF/" + cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID + "_" + doc_serie + "_" + doc_correlativo + ".pdf";

                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }

                //CREACION DE XML           
                string newxml = HostingEnvironment.MapPath("~/PDF/" + cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID + "_" + doc_serie + "_" + doc_correlativo + ".xml");
                string newXmlServer = "/PDF/" + cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID + "_" + doc_serie + "_" + doc_correlativo + ".xml";
                if (File.Exists(newxml))
                {
                    File.Delete(newxml);
                }

                // SACAR EL CP29
                System.IO.StreamWriter objWrite;
                objWrite = new System.IO.StreamWriter(newxml);
                objWrite.Write(cabecera.Cs_pr_XML);
                objWrite.Close();
                File.SetAttributes(newxml, FileAttributes.Normal);

                // LEER EL XML Y FIRMA DEL XML
                XmlDocument xmlDocument = new XmlDocument();
                var textXml = cabecera.Cs_pr_XML;               
                textXml = textXml.Replace("cbc:", "");
                textXml = textXml.Replace("cac:", "");
                textXml = textXml.Replace("sac:", "");
                textXml = textXml.Replace("ext:", "");
                textXml = textXml.Replace("ds:", "");
                xmlDocument.LoadXml(textXml);

                var signatureValue = xmlDocument.GetElementsByTagName("SignatureValue")[0].InnerText;
                var digestValue = xmlDocument.GetElementsByTagName("DigestValue")[0].InnerText;

                string InvoiceTypeCode = String.Empty;
                XmlNodeList InvoiceTypeCodeXml = xmlDocument.GetElementsByTagName("InvoiceTypeCode");
                if (InvoiceTypeCodeXml.Count > 0)
                {
                    InvoiceTypeCode = xmlDocument.GetElementsByTagName("InvoiceTypeCode")[0].InnerText;
                }
                else
                {
                    InvoiceTypeCode = cabecera.Cs_tag_InvoiceTypeCode;

                }

                // LLAMAR LOS DATOS PARA ARMA EL PDF

                string IssueDate = xmlDocument.GetElementsByTagName("IssueDate")[0].InnerText;
                string DocumentCurrencyCode = xmlDocument.GetElementsByTagName("DocumentCurrencyCode")[0].InnerText;
                string ASPCustomerAssignedAccountID = ""; //no esta
                string ASPAdditionalAccountID = ""; //no esta
                string ASPStreetName = ""; // no esta
                string ASPRegistrationName = "";
                //nuevo
                string ASPAddressTypeCode = "";
                string ASPLine = "";
                string ASPPartyIdentification = "";
                //NUEVO
                string ASPRegistrationAddress = "";
                //
                string CbcNote = "";
                //
                string ACPCustomerAssignedAccountID = "";
                string ACPAdditionalAccountID = "";
                string ACPDescription = ""; //no esta
                string ACPRegistrationName = "";
                //nuevo
                string ACPParty = "";
                string ACPPartyIdentification = "";
                string ACPId = "";
                string ACPSchemeId = "";
                string ACPAddressTypeCode = "";
                string ACPAddressLine = "";
                string ACPRegistrationAddress = "";

                //string ttTaxableAmount = "";
                //
                string DReferenceID = "";
                string DResponseCode = "";
                string DDescription = "";
                string LMTChargeTotalAmount = "";//no esta
                string LMTPayableAmount = "";
                //nuevo
                string LMTLineExtensionAmount = "";
                string LMTTaxInclusiveAmount = "";
                string LMTAllowanceTotalAmount = "";
                string LMTPrepaidAmount = "";

                //
                string op_detraccion = "0.00";

                string porcentaje_detraccion = "";
                string cuenta_nacion1 = "";

                string ttName = "";
                string ttGrati = "";
                string op_inafecta = "0.00";
                string op_gratuita = "0.00";
                string op_gravada = "0.00";
                //nuevo - mp
                string BRCondicion = "";
                string direccionad = "";
                //string ORordencompra = "";
                //string DDRguiaremi = "";
                //string BRcuentasbancarias = "";
                //string BRobservacion = "";


                //string valor_operacion = string.Empty; //

                string uubbll = "";


                var info_general = Documento.getByTipo(InvoiceTypeCode);

                Document doc = new Document(PageSize.A4);
                // Indicamos donde vamos a guardar el documento
                PdfWriter writer = PdfWriter.GetInstance(doc,
                                            new FileStream(newFile, FileMode.Create));

                // Le colocamos el título y el autor
                // Esto no será visible en el documento
                doc.AddTitle("Documento Electronico");
                doc.AddCreator("Contasis");

                // Abrimos el archivo
                doc.Open();
                // Creamos el tipo de Font que vamos utilizar
                iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _TitleFontN = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font _TitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 15, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _HeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font _HeaderFontMin = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font _clienteFontBold = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font _clienteFontBoldMin = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                iTextSharp.text.Font _clienteFontContent = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _clienteFontContentMinFooter = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                iTextSharp.text.Font _clienteFontBoldContentMinFooter = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                PdfPTable tblPrueba = new PdfPTable(5);
                tblPrueba.WidthPercentage = 100;



                //TABLA header left
                PdfPTable tblHeaderLeft = new PdfPTable(1);
                tblHeaderLeft.WidthPercentage = 100;


                // Creamos la imagen y le ajustamos el tamaño
                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(HostingEnvironment.MapPath(info_general.Imagen));
                imagen.BorderWidth = 0;
                imagen.Alignment = Element.ALIGN_RIGHT;
                float percentage = 0.0f;
                percentage = 290 / imagen.Width;
                imagen.ScalePercent(80);

                // Insertamos la imagen en el documento

                PdfPCell logo = new PdfPCell(imagen);
                logo.BorderWidth = 0;
                logo.BorderWidthBottom = 0;
                logo.Border = 0;

                tblHeaderLeft.AddCell(logo);
                //condicion
                uubbll = xmlDocument.GetElementsByTagName("UBLVersionID").Item(0).InnerText.ToString();
                if (uubbll == "2.0")
                {
                    //codigo 2.0

                    //get accounting supplier party
                    XmlNodeList AccountingSupplierParty = xmlDocument.GetElementsByTagName("AccountingSupplierParty");
                    foreach (XmlNode dat in AccountingSupplierParty)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var caaid = xmlDocumentinner.GetElementsByTagName("CustomerAssignedAccountID");
                        if (caaid.Count > 0)
                        {
                            ASPCustomerAssignedAccountID = caaid.Item(0).InnerText;
                        }
                        var aacid = xmlDocumentinner.GetElementsByTagName("AdditionalAccountID");
                        if (aacid.Count > 0)
                        {
                            ASPAdditionalAccountID = aacid.Item(0).InnerText;
                        }
                        var stname = xmlDocumentinner.GetElementsByTagName("StreetName");
                        if (stname.Count > 0)
                        {
                            ASPStreetName = stname.Item(0).InnerText;
                        }
                        var regname = xmlDocumentinner.GetElementsByTagName("RegistrationName");
                        if (regname.Count > 0)
                        {
                            ASPRegistrationName = regname.Item(0).InnerText;
                        }
                    }
                    //get accounting supplier party
                    XmlNodeList AccountingCustomerParty = xmlDocument.GetElementsByTagName("AccountingCustomerParty");
                    foreach (XmlNode dat in AccountingCustomerParty)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var caaid = xmlDocumentinner.GetElementsByTagName("CustomerAssignedAccountID");
                        if (caaid.Count > 0)
                        {
                            ACPCustomerAssignedAccountID = caaid.Item(0).InnerText;
                        }
                        var aacid = xmlDocumentinner.GetElementsByTagName("AdditionalAccountID");
                        if (aacid.Count > 0)
                        {
                            ACPAdditionalAccountID = aacid.Item(0).InnerText;
                        }
                        var descr = xmlDocumentinner.GetElementsByTagName("Description");
                        if (descr.Count > 0)
                        {
                            ACPDescription = descr.Item(0).InnerText;
                        }
                        var regname = xmlDocumentinner.GetElementsByTagName("RegistrationName");
                        if (regname.Count > 0)
                        {
                            ACPRegistrationName = regname.Item(0).InnerText;
                        }
                    }
                    XmlNodeList DiscrepancyResponse = xmlDocument.GetElementsByTagName("DiscrepancyResponse");
                    foreach (XmlNode dat in DiscrepancyResponse)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var refid = xmlDocumentinner.GetElementsByTagName("ReferenceID");
                        if (refid.Count > 0)
                        {
                            DReferenceID = refid.Item(0).InnerText;
                        }
                        var respcode = xmlDocumentinner.GetElementsByTagName("ResponseCode");
                        if (respcode.Count > 0)
                        {
                            DResponseCode = respcode.Item(0).InnerText;
                        }
                        var descr = xmlDocumentinner.GetElementsByTagName("Description");
                        if (descr.Count > 0)
                        {
                            DDescription = descr.Item(0).InnerText;
                        }

                    }

                    XmlNodeList LegalMonetaryTotal = null;

                    if (InvoiceTypeCode == "08")
                    {
                        LegalMonetaryTotal = xmlDocument.GetElementsByTagName("RequestedMonetaryTotal");
                    }
                    else
                    {
                        LegalMonetaryTotal = xmlDocument.GetElementsByTagName("LegalMonetaryTotal");
                    }

                    foreach (XmlNode dat in LegalMonetaryTotal)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var cta = xmlDocumentinner.GetElementsByTagName("ChargeTotalAmount");
                        if (cta.Count > 0)
                        {
                            LMTChargeTotalAmount = cta.Item(0).InnerText;
                        }
                        var pam = xmlDocumentinner.GetElementsByTagName("PayableAmount");
                        if (pam.Count > 0)
                        {
                            LMTPayableAmount = pam.Item(0).InnerText;
                        }
                    }

                    List<clasEntityDocument_AdditionalComments> Lista_additional_coments = new List<clasEntityDocument_AdditionalComments>();
                    clasEntityDocument_AdditionalComments adittionalComents;
                    XmlNodeList datosCabecera = xmlDocument.GetElementsByTagName("DatosCabecera");
                    foreach (XmlNode dat in datosCabecera)
                    {
                        var NodosHijos = dat.ChildNodes;
                        for (int z = 0; z < NodosHijos.Count; z++)
                        {
                            adittionalComents = new clasEntityDocument_AdditionalComments();
                            adittionalComents.Cs_pr_TagNombre = NodosHijos.Item(z).LocalName;
                            adittionalComents.Cs_pr_TagValor = NodosHijos.Item(z).ChildNodes.Item(0).InnerText;
                            Lista_additional_coments.Add(adittionalComents);
                        }
                    }

                    //comentarios contenido
                    var teclaf8 = " ";//comment1
                    var teclavtrlm = " ";//commnet2
                    var cuentasbancarias = " ";//comment 3
                    string CondicionVentaXML = string.Empty;
                    string CondicionPagoXML = string.Empty;
                    string VendedorXML = string.Empty;
                    foreach (var itemm in Lista_additional_coments)
                    {
                        if (itemm.Cs_pr_TagNombre == "CondPago")
                        {
                            CondicionPagoXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "Vendedor")
                        {
                            VendedorXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "Condicion")
                        {
                            CondicionVentaXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "DatEmpresa")
                        {
                            cuentasbancarias = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "TeclaF8")
                        {
                            teclaf8 = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "TeclasCtrlM")
                        {
                            teclavtrlm = itemm.Cs_pr_TagValor;
                        }
                    }

                    string sucursal = string.Empty;
                    string[] sucursalpartes = cuentasbancarias.Split('*');
                    if (sucursalpartes.Length > 0)
                    {
                        sucursal = sucursalpartes[0];
                    }

                    //tabla info empresa
                    PdfPTable tblInforEmpresa = new PdfPTable(1);
                    tblInforEmpresa.WidthPercentage = 100;
                    PdfPCell NameEmpresa = new PdfPCell(new Phrase(ASPRegistrationName, _HeaderFont));
                    NameEmpresa.BorderWidth = 0;
                    NameEmpresa.Border = 0;
                    tblInforEmpresa.AddCell(NameEmpresa);

                    var pa = new Paragraph();
                    pa.Font = _clienteFontBoldMin;
                    pa.Add("Dirección:AV. ALMIRANTE MIGUEL GRAU NRO. 093 DPTO. C INT. 102 (COSTADO BANCO DE LA NACION) LIMA - LIMA - BARRANCO \n");
                    //pa.Add(sucursal);

                    PdfPCell EstaticoEmpresa = new PdfPCell(pa);
                    EstaticoEmpresa.BorderWidth = 0;
                    EstaticoEmpresa.Border = 0;
                    tblInforEmpresa.AddCell(EstaticoEmpresa);

                    PdfPCell celdaInfoEmpresa = new PdfPCell(tblInforEmpresa);
                    celdaInfoEmpresa.Border = 0;
                    tblHeaderLeft.AddCell(celdaInfoEmpresa);
                    // PdfPCell blanco = new PdfPCell();
                    // blanco.Border = 0;





                    //tabla para info ruc
                    PdfPTable tblInforRuc = new PdfPTable(1);
                    tblInforRuc.WidthPercentage = 100;

                    PdfPCell TituRuc = new PdfPCell(new Phrase("R.U.C. " + ASPCustomerAssignedAccountID, _TitleFontN));
                    TituRuc.BorderWidthTop = 0.75f;
                    TituRuc.BorderWidthBottom = 0.75f;
                    TituRuc.BorderWidthLeft = 0.75f;
                    TituRuc.BorderWidthRight = 0.75f;
                    TituRuc.HorizontalAlignment = Element.ALIGN_CENTER;
                    TituRuc.PaddingTop = 10f;
                    TituRuc.PaddingBottom = 10f;

                    PdfPCell TipoDoc = new PdfPCell(new Phrase(info_general.Nombre, _TitleFontN));
                    TipoDoc.BorderWidthLeft = 0.75f;
                    TipoDoc.BorderWidthRight = 0.75f;
                    TipoDoc.HorizontalAlignment = Element.ALIGN_CENTER;
                    TipoDoc.PaddingTop = 10f;
                    TipoDoc.PaddingBottom = 10f;

                    PdfPCell SerieDoc = new PdfPCell(new Phrase("N° " + cabecera.Cs_tag_ID, _TitleFont));
                    SerieDoc.BorderWidthBottom = 0.75f;
                    SerieDoc.BorderWidthRight = 0.75f;
                    SerieDoc.BorderWidthLeft = 0.75f;
                    SerieDoc.BorderWidthTop = 0.75f;
                    SerieDoc.HorizontalAlignment = Element.ALIGN_CENTER;
                    SerieDoc.PaddingTop = 10f;
                    SerieDoc.PaddingBottom = 10f;

                    PdfPCell blanco2 = new PdfPCell(new Paragraph(" "));
                    blanco2.Border = 0;
                    tblInforRuc.AddCell(TituRuc);
                    //tblInforRuc.AddCell(blanco2);
                    tblInforRuc.AddCell(TipoDoc);
                    //tblInforRuc.AddCell(blanco2);
                    tblInforRuc.AddCell(SerieDoc);
                    tblInforRuc.AddCell(blanco2);

                    PdfPCell infoRuc = new PdfPCell(tblInforRuc);
                    infoRuc.Colspan = 2;
                    infoRuc.BorderWidth = 0;

                    PdfPCell celdaHeaderLeft = new PdfPCell(tblHeaderLeft);
                    celdaHeaderLeft.Border = 0;
                    celdaHeaderLeft.Colspan = 3;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(celdaHeaderLeft);
                    // tblPrueba.AddCell(blanco);
                    tblPrueba.AddCell(infoRuc);

                    doc.Add(tblPrueba);

                    PdfPTable tblBlanco = new PdfPTable(1);
                    tblBlanco.WidthPercentage = 100;
                    PdfPCell blanco3 = new PdfPCell((new Paragraph(" ")));
                    blanco3.Border = 0;

                    tblBlanco.AddCell(blanco3);

                    doc.Add(tblBlanco);

                    //Informacion cliente
                    PdfPTable tblInfoCliente = new PdfPTable(10);
                    tblInfoCliente.WidthPercentage = 100;



                    // Llenamos la tabla con información del cliente
                    PdfPCell cliente = new PdfPCell(new Phrase("Cliente:", _clienteFontBoldMin));
                    cliente.BorderWidth = 0;
                    cliente.Colspan = 1;

                    PdfPCell clNombre = new PdfPCell(new Phrase(ACPRegistrationName, _clienteFontContentMinFooter));
                    clNombre.BorderWidth = 0;
                    clNombre.Colspan = 5;

                    PdfPCell fecha = new PdfPCell(new Phrase("Fecha de Emision:", _clienteFontBoldMin));
                    fecha.BorderWidth = 0;
                    fecha.Colspan = 2;

                    var fechaString = dt.ToString("dd") + " de " + dt.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-ES")) + " " + dt.ToString("yyyy");
                    PdfPCell clFecha = new PdfPCell(new Phrase(fechaString.ToUpper(), _clienteFontContentMinFooter));
                    clFecha.BorderWidth = 0;
                    clFecha.Colspan = 2;

                    // Añadimos las celdas a la tabla
                    tblInfoCliente.AddCell(cliente);
                    tblInfoCliente.AddCell(clNombre);
                    tblInfoCliente.AddCell(fecha);
                    tblInfoCliente.AddCell(clFecha);

                    PdfPCell direccion = new PdfPCell(new Phrase("Direccion:", _clienteFontBoldMin));
                    direccion.BorderWidth = 0;
                    direccion.Colspan = 1;

                    PdfPCell clDireccion = new PdfPCell(new Phrase(ACPDescription, _clienteFontContentMinFooter));
                    clDireccion.BorderWidth = 0;
                    clDireccion.Colspan = 5;


                    /*En caso sea nota de credito o debito*/
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        PdfPCell condicionVenta = new PdfPCell(new Phrase("Documento que modifica:", _clienteFontBoldMin));
                        condicionVenta.BorderWidth = 0;
                        condicionVenta.Colspan = 2;


                        PdfPCell clCondicionVenta = new PdfPCell(new Phrase(DReferenceID, _clienteFontContentMinFooter));
                        clCondicionVenta.BorderWidth = 0;
                        clCondicionVenta.Colspan = 2;

                        tblInfoCliente.AddCell(direccion);
                        tblInfoCliente.AddCell(clDireccion);
                        tblInfoCliente.AddCell(condicionVenta);
                        tblInfoCliente.AddCell(clCondicionVenta);
                    }
                    else
                    {
                        NumLetra monedaLetras = new NumLetra();
                        var monedaLetra = monedaLetras.getMoneda(DocumentCurrencyCode);
                        PdfPCell moneda = new PdfPCell(new Phrase("Moneda:", _clienteFontBoldMin));
                        moneda.BorderWidth = 0;
                        moneda.Colspan = 2;

                        PdfPCell clMoneda = new PdfPCell(new Phrase(monedaLetra.ToUpper(), _clienteFontContentMinFooter));
                        clMoneda.BorderWidth = 0;
                        clMoneda.Colspan = 2;

                        /* PdfPCell condicionVenta = new PdfPCell(new Phrase("Condicion Venta:", _clienteFontBoldMin));
                         condicionVenta.BorderWidth = 0;
                         condicionVenta.Colspan = 2;


                         PdfPCell clCondicionVenta = new PdfPCell(new Phrase("", _clienteFontContentMinFooter));
                         clCondicionVenta.BorderWidth = 0;
                         clCondicionVenta.Colspan = 2;
                         */
                        tblInfoCliente.AddCell(direccion);
                        tblInfoCliente.AddCell(clDireccion);
                        tblInfoCliente.AddCell(moneda);
                        tblInfoCliente.AddCell(clMoneda);

                    }


                    // Añadimos las celdas a la tabla de info cliente


                    var docName = getTipoDocIdentidad(ACPAdditionalAccountID);
                    PdfPCell ruc = new PdfPCell(new Phrase(docName + " N°:", _clienteFontBoldMin));
                    ruc.BorderWidth = 0;
                    ruc.Colspan = 1;

                    PdfPCell clRUC = new PdfPCell(new Phrase(ACPCustomerAssignedAccountID, _clienteFontContentMinFooter));
                    clRUC.BorderWidth = 0;
                    clRUC.Colspan = 5;
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        NumLetra monedaLetras1 = new NumLetra();
                        var monedaLetra_ = monedaLetras1.getMoneda(DocumentCurrencyCode);
                        PdfPCell moneda_ = new PdfPCell(new Phrase("Moneda:", _clienteFontBoldMin));
                        moneda_.BorderWidth = 0;
                        moneda_.Colspan = 2;

                        PdfPCell clMoneda_ = new PdfPCell(new Phrase(monedaLetra_.ToUpper(), _clienteFontContentMinFooter));
                        clMoneda_.BorderWidth = 0;
                        clMoneda_.Colspan = 2;
                        tblInfoCliente.AddCell(ruc);
                        tblInfoCliente.AddCell(clRUC);
                        tblInfoCliente.AddCell(moneda_);
                        tblInfoCliente.AddCell(clMoneda_);
                    }
                    else
                    {  //NumLetra monedaLetras = new NumLetra();
                       //  var monedaLetra_ = monedaLetras.getMoneda(cabecera.Cs_tag_DocumentCurrencyCode);
                        PdfPCell moneda_ = new PdfPCell(new Phrase("Condicion de Venta", _clienteFontBoldMin));
                        moneda_.BorderWidth = 0;
                        moneda_.Colspan = 2;

                        PdfPCell clMoneda_ = new PdfPCell(new Phrase(CondicionVentaXML, _clienteFontContentMinFooter));
                        clMoneda_.BorderWidth = 0;
                        clMoneda_.Colspan = 2;
                        tblInfoCliente.AddCell(ruc);
                        tblInfoCliente.AddCell(clRUC);
                        tblInfoCliente.AddCell(moneda_);
                        tblInfoCliente.AddCell(clMoneda_);

                    }

                    // Añadimos las celdas a la tabla inf

                    /*En caso sea nota de credito o debito*/
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {

                        PdfPCell motivomodifica = new PdfPCell(new Phrase("Motivo", _clienteFontBoldMin));
                        motivomodifica.BorderWidth = 0;
                        motivomodifica.Colspan = 1;

                        PdfPCell clmotivomodifica = new PdfPCell(new Phrase(DDescription, _clienteFontContentMinFooter));
                        clmotivomodifica.BorderWidth = 0;
                        clmotivomodifica.Colspan = 5;

                        clasEntityDocument doc_modificado = new clasEntityDocument();
                        string fechaModificado = doc_modificado.cs_pxBuscarFechaDocumento(DReferenceID);
                        PdfPCell docmodifica = new PdfPCell(new Phrase("Fecha Doc. Modificado:", _clienteFontBoldMin));
                        docmodifica.BorderWidth = 0;
                        docmodifica.Colspan = 2;

                        PdfPCell cldocmodifica = new PdfPCell(new Phrase(fechaModificado, _clienteFontContentMinFooter));
                        cldocmodifica.BorderWidth = 0;
                        cldocmodifica.Colspan = 2;

                        tblInfoCliente.AddCell(motivomodifica);
                        tblInfoCliente.AddCell(clmotivomodifica);
                        tblInfoCliente.AddCell(docmodifica);
                        tblInfoCliente.AddCell(cldocmodifica);

                    }
                    else
                    {
                        PdfPCell motivomodifica = new PdfPCell(new Phrase(" ", _clienteFontBoldMin));
                        motivomodifica.BorderWidth = 0;
                        motivomodifica.Colspan = 1;

                        PdfPCell clmotivomodifica = new PdfPCell(new Phrase(" ", _clienteFontContentMinFooter));
                        clmotivomodifica.BorderWidth = 0;
                        clmotivomodifica.Colspan = 5;


                        PdfPCell docmodifica = new PdfPCell(new Phrase("Vendedor:", _clienteFontBoldMin));
                        docmodifica.BorderWidth = 0;
                        docmodifica.Colspan = 2;

                        PdfPCell cldocmodifica = new PdfPCell(new Phrase(VendedorXML, _clienteFontContentMinFooter));
                        cldocmodifica.BorderWidth = 0;
                        cldocmodifica.Colspan = 2;

                        tblInfoCliente.AddCell(motivomodifica);
                        tblInfoCliente.AddCell(clmotivomodifica);
                        tblInfoCliente.AddCell(docmodifica);
                        tblInfoCliente.AddCell(cldocmodifica);

                    }

                    /*------------------------------------*/
                    doc.Add(tblInfoCliente);
                    doc.Add(tblBlanco);

                    PdfPTable tblInfoComprobante = new PdfPTable(11);
                    tblInfoComprobante.WidthPercentage = 100;


                    // Llenamos la tabla con información
                    PdfPCell colCodigo = new PdfPCell(new Phrase("Item", _clienteFontBoldMin));
                    colCodigo.BorderWidthBottom = 0.75f;
                    colCodigo.BorderWidthLeft = 0.75f;
                    colCodigo.BorderWidthRight = 0.75f;
                    colCodigo.BorderWidthTop = 0.75f;
                    colCodigo.Colspan = 1;
                    colCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colCantidad = new PdfPCell(new Phrase("Cantidad", _clienteFontBoldMin));
                    colCantidad.BorderWidthBottom = 0.75f;
                    colCantidad.BorderWidthLeft = 0;
                    colCantidad.BorderWidthRight = 0.75f;
                    colCantidad.BorderWidthTop = 0.75f;
                    colCantidad.Colspan = 1;
                    colCantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                    /*PdfPCell colUnidadMedida= new PdfPCell(new Phrase("Und Medida", _clienteFontBoldMin));
                    colUnidadMedida.BorderWidth = 0.75f;
                    colUnidadMedida.Colspan = 1;
                    colUnidadMedida.HorizontalAlignment = Element.ALIGN_CENTER;*/

                    PdfPCell colDescripcion = new PdfPCell(new Phrase("Descripcion", _clienteFontBoldMin));
                    colDescripcion.BorderWidthBottom = 0.75f;
                    colDescripcion.BorderWidthLeft = 0;
                    colDescripcion.BorderWidthRight = 0.75f;
                    colDescripcion.BorderWidthTop = 0.75f;
                    colDescripcion.Colspan = 7;
                    colDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colPrecUnit = new PdfPCell(new Phrase("Valor Unitario (Sin IGV)", _clienteFontBoldMin));
                    colPrecUnit.BorderWidthBottom = 0.75f;
                    colPrecUnit.BorderWidthLeft = 0;
                    colPrecUnit.BorderWidthRight = 0.75f;
                    colPrecUnit.BorderWidthTop = 0.75f;
                    colPrecUnit.Colspan = 1;
                    colPrecUnit.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colImporte = new PdfPCell(new Phrase("Valor Total (Sin IGV)", _clienteFontBoldMin));
                    colImporte.BorderWidthBottom = 0.75f;
                    colImporte.BorderWidthLeft = 0;
                    colImporte.BorderWidthRight = 0.75f;
                    colImporte.BorderWidthTop = 0.75f;
                    colImporte.Colspan = 1;
                    colImporte.HorizontalAlignment = Element.ALIGN_CENTER;

                    // Añadimos las celdas a la tabla
                    tblInfoComprobante.AddCell(colCodigo);
                    tblInfoComprobante.AddCell(colCantidad);
                    // tblInfoComprobante.AddCell(colUnidadMedida);
                    tblInfoComprobante.AddCell(colDescripcion);
                    tblInfoComprobante.AddCell(colPrecUnit);
                    tblInfoComprobante.AddCell(colImporte);

                    //impuestos globales

                    List<clasEntityDocument_TaxTotal> Lista_tax_total = new List<clasEntityDocument_TaxTotal>();
                    clasEntityDocument_TaxTotal taxTotal;
                    XmlNodeList nodestaxTotal = xmlDocument.GetElementsByTagName("TaxTotal");
                    foreach (XmlNode dat in nodestaxTotal)
                    {
                        string nodoPadre = dat.ParentNode.LocalName;
                        if (nodoPadre == "Invoice" || nodoPadre == "DebitNote" || nodoPadre == "CreditNote")
                        {
                            taxTotal = new clasEntityDocument_TaxTotal();
                            XmlDocument xmlDocumentTaxtotal = new XmlDocument();
                            xmlDocumentTaxtotal.LoadXml(dat.OuterXml);
                            XmlNodeList taxAmount = xmlDocumentTaxtotal.GetElementsByTagName("TaxAmount");
                            if (taxAmount.Count > 0)
                            {
                                taxTotal.Cs_tag_TaxAmount = taxAmount.Item(0).InnerText;
                            }
                            XmlNodeList subtotal = xmlDocumentTaxtotal.GetElementsByTagName("TaxSubtotal");
                            if (subtotal.Count > 0)
                            {
                                XmlDocument xmlDocumentTaxSubtotal = new XmlDocument();
                                xmlDocumentTaxSubtotal.LoadXml(subtotal.Item(0).OuterXml);

                                var subTotalAmount = xmlDocumentTaxSubtotal.GetElementsByTagName("TaxAmount");
                                if (subTotalAmount.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxAmount = subTotalAmount.Item(0).InnerText;
                                }
                                var subTotalID = xmlDocumentTaxSubtotal.GetElementsByTagName("ID");
                                if (subTotalID.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID = subTotalID.Item(0).InnerText;
                                }


                                var subTotalName = xmlDocumentTaxSubtotal.GetElementsByTagName("Name");
                                if (subTotalName.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_Name = subTotalName.Item(0).InnerText;
                                }
                                var subTotalTaxTypeCode = xmlDocumentTaxSubtotal.GetElementsByTagName("TaxTypeCode");
                                if (subTotalTaxTypeCode.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_TaxTypeCode = subTotalTaxTypeCode.Item(0).InnerText;
                                }

                            }
                            Lista_tax_total.Add(taxTotal);

                        }
                    }



                    string imp_IGV = "";
                    string imp_ISC = "";
                    string imp_OTRO = "";

                    foreach (var ress in Lista_tax_total)
                    {

                        if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "1000")
                        {//IGV
                            imp_IGV = Convert.ToString(ress.Cs_tag_TaxAmount);

                        }
                        else if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "2000")
                        {//isc
                            imp_ISC = Convert.ToString(ress.Cs_tag_TaxAmount);

                        }
                        else if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "9999")
                        {
                            imp_OTRO = Convert.ToString(ress.Cs_tag_TaxAmount);

                        }

                    }

                    //Additional Monetary Total
                    List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal> Lista_additional_monetary = new List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal>();
                    List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty> Lista_additional_property = new List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty>();

                    XmlNodeList additionalInformation = xmlDocument.GetElementsByTagName("AdditionalInformation");
                    foreach (XmlNode dat in additionalInformation)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);
                        clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal adittionalMonetary;

                        XmlNodeList LIST1 = xmlDocumentinner.GetElementsByTagName("AdditionalMonetaryTotal");
                        for (int ii = 0; ii < LIST1.Count; ii++)
                        {
                            adittionalMonetary = new clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal();

                            var ss = LIST1.Item(ii);
                            XmlDocument xmlDocumentinner1 = new XmlDocument();
                            xmlDocumentinner1.LoadXml(ss.OuterXml);

                            var id = xmlDocumentinner1.GetElementsByTagName("ID");
                            if (id.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_Id = id.Item(0).InnerText;
                                if (id.Item(0).Attributes.Count > 0)
                                {
                                    adittionalMonetary.Cs_tag_SchemeID = id.Item(0).Attributes.GetNamedItem("schemeID").Value;
                                }
                            }

                            var percent = xmlDocumentinner1.GetElementsByTagName("Percent");
                            if (percent.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_Percent = percent.Item(0).InnerText;
                            }
                            var payableAmount = xmlDocumentinner1.GetElementsByTagName("PayableAmount");
                            if (payableAmount.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_PayableAmount = payableAmount.Item(0).InnerText;
                                /*** if (payableAmount.Item(0).Attributes.Count > 0)
                                 {
                                     adittionalMonetary. = payableAmount.Item(0).Attributes.GetNamedItem("currencyID").Value;
                                 }****/
                            }
                            Lista_additional_monetary.Add(adittionalMonetary);

                        }
                        clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty adittionalProperty;
                        XmlNodeList LIST2 = xmlDocumentinner.GetElementsByTagName("AdditionalProperty");
                        for (int iii = 0; iii < LIST2.Count; iii++)
                        {
                            adittionalProperty = new clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty();

                            var ss = LIST2.Item(iii);
                            XmlDocument xmlDocumentinner1 = new XmlDocument();
                            xmlDocumentinner1.LoadXml(ss.OuterXml);

                            var id = xmlDocumentinner1.GetElementsByTagName("ID");
                            if (id.Count > 0)
                            {
                                adittionalProperty.Cs_tag_ID = id.Item(0).InnerText;
                            }

                            var value = xmlDocumentinner1.GetElementsByTagName("Value");
                            if (value.Count > 0)
                            {
                                adittionalProperty.Cs_tag_Value = value.Item(0).InnerText;
                            }
                            var name = xmlDocumentinner1.GetElementsByTagName("Name");
                            if (name.Count > 0)
                            {
                                adittionalProperty.Cs_tag_Name = name.Item(0).InnerText;
                            }
                            Lista_additional_property.Add(adittionalProperty);
                        }
                    }
                    //Additional

                    var cuenta_nacion = "";
                    try
                    {
                        foreach (var it in Lista_additional_property)
                        {
                            if (it.Cs_tag_ID == "3001")
                            {
                                cuenta_nacion = it.Cs_tag_Value;
                            }
                        }

                    }
                    catch (Exception)
                    {
                        cuenta_nacion = "";
                    }

                    string op_gravada1 = "0.00";
                    string op_inafecta1 = "0.00";
                    string op_exonerada = "0.00";
                    string op_gratuita1 = "0.00";
                    string op_detraccion1 = "0.00";
                    string porcentaje_detraccion1 = "";
                    string total_descuentos = "0.00";
                    string op_percepcion = "0.00";
                    string tipo_op = "0";

                    foreach (var ress in Lista_additional_monetary)
                    {
                        if (ress.Cs_tag_Id == "1001")
                        {
                            op_gravada1 = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1002")
                        {
                            op_inafecta1 = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1003")
                        {
                            op_exonerada = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "2005")
                        {
                            total_descuentos = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1004")
                        {
                            op_gratuita1 = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "2003")
                        {
                            op_detraccion1 = Convert.ToString(ress.Cs_tag_PayableAmount);
                            porcentaje_detraccion1 = Convert.ToString(ress.Cs_tag_Percent);
                        }
                        else if (ress.Cs_tag_Id == "2001")
                        {
                            op_percepcion = Convert.ToString(ress.Cs_tag_PayableAmount);
                            tipo_op = Convert.ToString(ress.Cs_tag_SchemeID);
                        }

                    }
                    /* seccion de items ------ añadir items*/
                    var numero_item = 0;
                    double sub_total = 0.00;

                    List<clasEntityDocument_Line> Lista_items;
                    List<clasEntityDocument_Line_TaxTotal> Lista_items_taxtotal;
                    clasEntityDocument_Line item;
                    XmlNodeList nodeitem;
                    if (InvoiceTypeCode == "07")
                    {
                        nodeitem = xmlDocument.GetElementsByTagName("CreditNoteLine");

                    }
                    else if (InvoiceTypeCode == "08")
                    {

                        nodeitem = xmlDocument.GetElementsByTagName("DebitNoteLine");

                    }
                    else
                    {
                        nodeitem = xmlDocument.GetElementsByTagName("InvoiceLine");
                    }
                    // XmlNodeList nodeitem = xmlDocument.GetElementsByTagName("InvoiceLine");
                    // Dictionary<string, List<clasEntityDocument_Line_Description>> dictionary = new Dictionary<string, List<clasEntityDocument_Line_Description>>();
                    List<clasEntityDocument_Line_Description> Lista_items_description;
                    List<clasEntityDocument_Line_PricingReference> Lista_items_princingreference;
                    clasEntityDocument_Line_Description descripcionItem;

                    var total_items = nodeitem.Count;

                    int i = 0;
                    foreach (XmlNode dat in nodeitem)
                    {
                        i++;
                        numero_item++;
                        var valor_unitario_item = "";
                        var valor_total_item = "";
                        string condition_price = "";
                        Lista_items = new List<clasEntityDocument_Line>();
                        Lista_items_description = new List<clasEntityDocument_Line_Description>();
                        Lista_items_princingreference = new List<clasEntityDocument_Line_PricingReference>();
                        Lista_items_taxtotal = new List<clasEntityDocument_Line_TaxTotal>();
                        item = new clasEntityDocument_Line();
                        XmlDocument xmlItem = new XmlDocument();
                        xmlItem.LoadXml(dat.OuterXml);

                        XmlNodeList ItemDetail = xmlItem.GetElementsByTagName("Item");
                        if (ItemDetail.Count > 0)
                        {
                            foreach (XmlNode items in ItemDetail)
                            {
                                XmlDocument xmlItemItem = new XmlDocument();
                                xmlItemItem.LoadXml(items.OuterXml);
                                XmlNodeList taxItemIdentification = xmlItemItem.GetElementsByTagName("ID");
                                if (taxItemIdentification.Count > 0)
                                {
                                    item.Cs_tag_Item_SellersItemIdentification = taxItemIdentification.Item(0).InnerText;
                                }
                                XmlNodeList taxItemDescription = xmlItemItem.GetElementsByTagName("Description");
                                int j = 0;
                                foreach (XmlNode description in taxItemDescription)
                                {
                                    j++;
                                    descripcionItem = new clasEntityDocument_Line_Description();
                                    descripcionItem.Cs_pr_Document_Line_Id = j.ToString();
                                    /* if (description.HasChildNodes)
                                     {
                                         descripcionItem.Cs_tag_Description = description.FirstChild.InnerText.Trim();
                                     }
                                     else
                                     {*/
                                    descripcionItem.Cs_tag_Description = description.InnerText.Trim();
                                    //   }

                                    Lista_items_description.Add(descripcionItem);

                                }
                                j = 0;
                            }
                            //dictionary[i.ToString()] = Lista_items_description;
                        }


                        XmlNodeList ID = xmlItem.GetElementsByTagName("ID");
                        if (ID.Count > 0)
                        {
                            item.Cs_tag_InvoiceLine_ID = ID.Item(0).InnerText;
                        }

                        XmlNodeList InvoicedQuantity;
                        if (InvoiceTypeCode == "07")
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("CreditedQuantity");

                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }
                        }
                        else if (InvoiceTypeCode == "08")
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("DebitedQuantity");
                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }
                        }
                        else
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("InvoicedQuantity");
                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }

                        }


                        XmlNodeList LineExtensionAmount = xmlItem.GetElementsByTagName("LineExtensionAmount");
                        if (LineExtensionAmount.Count > 0)
                        {
                            item.Cs_tag_LineExtensionAmount_currencyID = LineExtensionAmount.Item(0).InnerText;
                        }
                        clasEntityDocument_Line_PricingReference lines_pricing_reference;
                        XmlNodeList PricingReference = xmlItem.GetElementsByTagName("PricingReference");
                        if (PricingReference.Count > 0)
                        {
                            XmlDocument xmlItemItem = new XmlDocument();
                            xmlItemItem.LoadXml(PricingReference.Item(0).OuterXml);
                            XmlNodeList AlternativeConditionPrice = xmlItemItem.GetElementsByTagName("AlternativeConditionPrice");
                            foreach (XmlNode itm in AlternativeConditionPrice)
                            {
                                XmlDocument xmlItemPricingReference = new XmlDocument();
                                xmlItemPricingReference.LoadXml(itm.OuterXml);
                                lines_pricing_reference = new clasEntityDocument_Line_PricingReference();
                                XmlNodeList PriceAmount = xmlItemPricingReference.GetElementsByTagName("PriceAmount");
                                if (PriceAmount.Count > 0)
                                {
                                    lines_pricing_reference.Cs_tag_PriceAmount_currencyID = PriceAmount.Item(0).InnerText;
                                }
                                XmlNodeList PriceTypeCode = xmlItemPricingReference.GetElementsByTagName("PriceTypeCode");
                                if (PriceTypeCode.Count > 0)
                                {
                                    lines_pricing_reference.Cs_tag_PriceTypeCode = PriceTypeCode.Item(0).InnerText;
                                }
                                Lista_items_princingreference.Add(lines_pricing_reference);
                            }


                        }
                        clasEntityDocument_Line_TaxTotal taxTotalItem;
                        XmlNodeList TaxTotal = xmlItem.GetElementsByTagName("TaxTotal");
                        if (TaxTotal.Count > 0)
                        {
                            foreach (XmlNode taxitem in TaxTotal)
                            {
                                taxTotalItem = new clasEntityDocument_Line_TaxTotal();
                                XmlDocument xmlItemTaxtotal = new XmlDocument();
                                xmlItemTaxtotal.LoadXml(taxitem.OuterXml);
                                XmlNodeList taxItemAmount = xmlItemTaxtotal.GetElementsByTagName("TaxAmount");
                                if (taxItemAmount.Count > 0)
                                {
                                    taxTotalItem.Cs_tag_TaxAmount_currencyID = taxItemAmount.Item(0).InnerText;
                                }
                                XmlNodeList itemsubtotal = xmlItemTaxtotal.GetElementsByTagName("TaxSubtotal");
                                if (itemsubtotal.Count > 0)
                                {
                                    XmlDocument xmlItemTaxSubtotal = new XmlDocument();
                                    xmlItemTaxSubtotal.LoadXml(itemsubtotal.Item(0).OuterXml);

                                    var subTotalAmount = xmlItemTaxSubtotal.GetElementsByTagName("TaxAmount");
                                    if (subTotalAmount.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxAmount_currencyID = subTotalAmount.Item(0).InnerText;
                                    }
                                    var subTotalID = xmlItemTaxSubtotal.GetElementsByTagName("ID");
                                    if (subTotalID.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID = subTotalID.Item(0).InnerText;
                                    }
                                    var subTotalName = xmlItemTaxSubtotal.GetElementsByTagName("Name");
                                    if (subTotalName.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_Name = subTotalName.Item(0).InnerText;
                                    }
                                    var subTotalTaxTypeCode = xmlItemTaxSubtotal.GetElementsByTagName("TaxTypeCode");
                                    if (subTotalTaxTypeCode.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_TaxTypeCode = subTotalTaxTypeCode.Item(0).InnerText;
                                    }

                                }
                                Lista_items_taxtotal.Add(taxTotalItem);
                            }
                        }

                        XmlNodeList Price = xmlItem.GetElementsByTagName("Price");
                        if (Price.Count > 0)
                        {
                            XmlDocument xmlItemPrice = new XmlDocument();
                            xmlItemPrice.LoadXml(Price.Item(0).OuterXml);
                            XmlNodeList PriceAmount = xmlItemPrice.GetElementsByTagName("PriceAmount");
                            if (PriceAmount.Count > 0)
                            {
                                item.Cs_tag_Price_PriceAmount = PriceAmount.Item(0).InnerText;
                            }
                        }

                        if (op_gratuita1 != "0.00")
                        {
                            foreach (var itm in Lista_items_princingreference)
                            {
                                if (itm.Cs_tag_PriceTypeCode == "02")
                                {
                                    condition_price = itm.Cs_tag_PriceAmount_currencyID;
                                }
                            }
                        }
                        var text_detalle = "";
                        foreach (var det_it in Lista_items_description)
                        {
                            text_detalle += det_it.Cs_tag_Description + " \n";
                        }
                        PdfPCell itCodigo = new PdfPCell(new Phrase(numero_item.ToString(), _clienteFontContentMinFooter));
                        itCodigo.Colspan = 1;
                        if (numero_item == total_items & op_detraccion1 == "0.00")
                        {
                            itCodigo.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itCodigo.BorderWidthBottom = 0.75f;
                        }
                        itCodigo.BorderWidthLeft = 0.75f;
                        itCodigo.BorderWidthRight = 0.75f;
                        itCodigo.BorderWidthTop = 0;
                        itCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                        PdfPCell itCantidad = new PdfPCell(new Phrase(item.Cs_tag_invoicedQuantity, _clienteFontContentMinFooter));
                        itCantidad.Colspan = 1;
                        if (numero_item == total_items & op_detraccion1 == "0.00")
                        {
                            itCantidad.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itCantidad.BorderWidthBottom = 0.75f;
                        }

                        itCantidad.BorderWidthLeft = 0;
                        itCantidad.BorderWidthRight = 0.75f;
                        itCantidad.BorderWidthTop = 0;
                        itCantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                        /* PdfPCell itUnidadMedida = new PdfPCell(new Phrase(item.Cs_tag_InvoicedQuantity_unitCode, _clienteFontContentMinFooter));
                         itUnidadMedida.Colspan = 1;
                         if (numero_item == total_items & op_detraccion1 == "0.00")
                         {
                             itUnidadMedida.BorderWidthBottom = 0.75f;

                         }
                         else
                         {
                             itUnidadMedida.BorderWidthBottom = 0.75f;
                         }

                         itUnidadMedida.BorderWidthLeft = 0;
                         itUnidadMedida.BorderWidthRight = 0.75f;
                         itUnidadMedida.BorderWidthTop = 0;
                         itUnidadMedida.HorizontalAlignment = Element.ALIGN_CENTER;*/

                        PdfPCell itDescripcion = new PdfPCell(new Phrase(text_detalle, _clienteFontContentMinFooter));
                        itDescripcion.Colspan = 7;
                        if (numero_item == total_items & op_detraccion1 == "0.00")
                        {
                            itDescripcion.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itDescripcion.BorderWidthBottom = 0.75f;
                        }

                        itDescripcion.BorderWidthLeft = 0;
                        itDescripcion.BorderWidthRight = 0.75f;
                        itDescripcion.BorderWidthTop = 0;
                        itDescripcion.PaddingBottom = 5f;
                        itDescripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                        if (op_gratuita1 != "0.00")
                        {
                            valor_unitario_item = condition_price;
                        }
                        else
                        {
                            valor_unitario_item = item.Cs_tag_Price_PriceAmount;
                        }

                        PdfPCell itPrecUnit = new PdfPCell(new Phrase(double.Parse(valor_unitario_item, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContentMinFooter));
                        itPrecUnit.Colspan = 1;
                        if (numero_item == total_items & op_detraccion1 == "0.00")
                        {
                            itPrecUnit.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itPrecUnit.BorderWidthBottom = 0.75f;
                        }

                        itPrecUnit.BorderWidthLeft = 0;
                        itPrecUnit.BorderWidthRight = 0.75f;
                        itPrecUnit.BorderWidthTop = 0;
                        itPrecUnit.HorizontalAlignment = Element.ALIGN_CENTER;


                        if (op_gratuita1 != "0.00")
                        {
                            if (valor_unitario_item == "")
                            {
                                valor_unitario_item = "0.00";
                            }
                            double valor_total_item_1 = double.Parse(valor_unitario_item, CultureInfo.InvariantCulture) * double.Parse(item.Cs_tag_invoicedQuantity, CultureInfo.InvariantCulture);
                            valor_total_item = valor_total_item_1.ToString();
                        }
                        else
                        {
                            valor_total_item = item.Cs_tag_LineExtensionAmount_currencyID;
                        }
                        PdfPCell itImporte = new PdfPCell(new Phrase(double.Parse(valor_total_item, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContentMinFooter));
                        itImporte.Colspan = 1;
                        if (numero_item == total_items & op_detraccion1 == "0.00")
                        {
                            itImporte.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itImporte.BorderWidthBottom = 0.75f;
                        }

                        itImporte.BorderWidthLeft = 0;
                        itImporte.BorderWidthRight = 0.75f;
                        itImporte.BorderWidthTop = 0;
                        itImporte.HorizontalAlignment = Element.ALIGN_CENTER;

                        //sub_total += Double.Parse(item.Cs_tag_LineExtensionAmount_currencyID);
                        // sub_total += double.Parse(item.Cs_tag_LineExtensionAmount_currencyID, CultureInfo.InvariantCulture);
                        // Añadimos las celdas a la tabla
                        tblInfoComprobante.AddCell(itCodigo);
                        tblInfoComprobante.AddCell(itCantidad);
                        // tblInfoComprobante.AddCell(itUnidadMedida);
                        tblInfoComprobante.AddCell(itDescripcion);
                        tblInfoComprobante.AddCell(itPrecUnit);
                        tblInfoComprobante.AddCell(itImporte);
                    }


                    if (op_detraccion1 != "0.00")
                    {
                        //agregar mensaje

                        PdfPCell celda_blanco = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco.Colspan = 1;
                        celda_blanco.BorderWidthBottom = 0.75f;
                        celda_blanco.BorderWidthLeft = 0;
                        celda_blanco.BorderWidthRight = 0.75f;
                        celda_blanco.BorderWidthTop = 0;

                        PdfPCell celda_blanco_right = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco_right.Colspan = 1;
                        celda_blanco_right.BorderWidthBottom = 0.75f;
                        celda_blanco_right.BorderWidthLeft = 0;
                        celda_blanco_right.BorderWidthRight = 0.75f;
                        celda_blanco_right.BorderWidthTop = 0;

                        PdfPCell celda_blanco_left = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco_left.Colspan = 1;
                        celda_blanco_left.BorderWidthBottom = 0.75f;
                        celda_blanco_left.BorderWidthLeft = 0.75f;
                        celda_blanco_left.BorderWidthRight = 0.75f;
                        celda_blanco_left.BorderWidthTop = 0;

                        var parrafo = new Paragraph();
                        parrafo.Font = _clienteFontContentMinFooter;
                        parrafo.Add("Operación sujeta al Sistema de Pago de Obligaciones Tributarias con el Gobierno Central \n");
                        parrafo.Add("SPOT " + porcentaje_detraccion1 + "% " + cuenta_nacion + " \n");

                        PdfPCell celda_parrafo = new PdfPCell(parrafo);
                        celda_parrafo.Colspan = 7;
                        celda_parrafo.BorderWidthBottom = 0.75f;
                        celda_parrafo.BorderWidthLeft = 0;
                        celda_parrafo.BorderWidthRight = 0.75f;
                        celda_parrafo.BorderWidthTop = 0;
                        celda_parrafo.PaddingTop = 10f;
                        celda_parrafo.HorizontalAlignment = Element.ALIGN_CENTER;

                        tblInfoComprobante.AddCell(celda_blanco_left);
                        tblInfoComprobante.AddCell(celda_blanco);
                        //tblInfoComprobante.AddCell(celda_blanco);
                        tblInfoComprobante.AddCell(celda_parrafo);
                        tblInfoComprobante.AddCell(celda_blanco);
                        tblInfoComprobante.AddCell(celda_blanco_right);

                    }
                    /* ------end items------*/
                    doc.Add(tblInfoComprobante);
                    doc.Add(tblBlanco);



                    if (InvoiceTypeCode == "03" | InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                        tblInfoOperacionesGratuitas.WidthPercentage = 100;

                        PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase(" ", _clienteFontContentMinFooter));
                        infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                        infoTotalOpGratuitas.BorderWidthRight = 0;
                        infoTotalOpGratuitas.Colspan = 5;
                        infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                        PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontBoldMin));
                        infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                        infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                        infoTotalOpGratuitasLabel.Colspan = 3;
                        infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_RIGHT;

                        var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                        PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita1, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                        infoTotalOpGratuitasVal.Colspan = 2;
                        infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                        doc.Add(tblInfoOperacionesGratuitas);

                        doc.Add(tblBlanco);
                        if (InvoiceTypeCode == "03")
                        {
                            /*----------- Monto total en letras --------------*/
                            NumLetra totalLetras = new NumLetra();
                            PdfPTable tblInfoMontoTotal = new PdfPTable(10);

                            tblInfoMontoTotal.WidthPercentage = 100;

                            PdfPCell infoTotal = new PdfPCell(new Phrase("SON: " + totalLetras.Convertir(LMTPayableAmount, true, DocumentCurrencyCode), _clienteFontContent));
                            infoTotal.BorderWidth = 0.75f;
                            infoTotal.Colspan = 7;
                            infoTotal.HorizontalAlignment = Element.ALIGN_LEFT;

                            tblInfoMontoTotal.AddCell(infoTotal);


                            PdfPTable tbl_monto_total1 = new PdfPTable(2);
                            tbl_monto_total1.WidthPercentage = 100;


                            var monedaDatos2 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell labelMontoTotal1 = new PdfPCell(new Phrase("IMPORTE TOTAL:", _clienteFontBold));
                            labelMontoTotal1.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell valueMontoTotal1 = new PdfPCell(new Phrase(monedaDatos2.CurrencySymbol + " " + double.Parse(LMTPayableAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            valueMontoTotal1.HorizontalAlignment = Element.ALIGN_RIGHT;

                            tbl_monto_total1.AddCell(labelMontoTotal1);
                            tbl_monto_total1.AddCell(valueMontoTotal1);

                            PdfPCell contenedor = new PdfPCell(tbl_monto_total1);
                            contenedor.Colspan = 3;
                            contenedor.Border = 0;
                            contenedor.PaddingLeft = 10f;
                            tblInfoMontoTotal.AddCell(contenedor);
                            doc.Add(tblInfoMontoTotal);
                            /*-------------End Monto Total----------------*/
                            doc.Add(tblBlanco);
                        }


                    }
                    else
                    {

                        if (op_gratuita1 != "0.00")
                        {
                            /*Monto de Transferencia Gratuita*/

                            PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                            tblInfoOperacionesGratuitas.WidthPercentage = 100;

                            PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase("TRANSFERENCIA GRATUITA DE UN BIEN Y/O SERVICIO PRESTADO GRATUITAMENTE", _clienteFontContentMinFooter));
                            infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                            infoTotalOpGratuitas.BorderWidthRight = 0;
                            infoTotalOpGratuitas.Colspan = 6;
                            infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                            PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontContentMinFooter));
                            infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                            infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                            infoTotalOpGratuitasLabel.Colspan = 3;
                            infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_CENTER;

                            var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita1, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                            infoTotalOpGratuitasVal.Colspan = 1;
                            infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                            doc.Add(tblInfoOperacionesGratuitas);

                            doc.Add(tblBlanco);
                        }
                        else
                        {


                            PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                            tblInfoOperacionesGratuitas.WidthPercentage = 100;

                            PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase(" ", _clienteFontContentMinFooter));
                            infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                            infoTotalOpGratuitas.BorderWidthRight = 0;
                            infoTotalOpGratuitas.Colspan = 5;
                            infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                            PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontBoldMin));
                            infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                            infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                            infoTotalOpGratuitasLabel.Colspan = 3;
                            infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_RIGHT;

                            var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita1, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                            infoTotalOpGratuitasVal.Colspan = 2;
                            infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                            doc.Add(tblInfoOperacionesGratuitas);
                            doc.Add(tblBlanco);
                        }
                    }



                    /*----------- CASO BOLETA SOLO MONTO TOTAL --------------*/
                    if (InvoiceTypeCode == "03")
                    {
                        /*  PdfPTable tblMontoTotal = new PdfPTable(10);
                          tblMontoTotal.WidthPercentage = 100;

                          PdfPCell monto_blanco = new PdfPCell(new Phrase(" ", _clienteFontContent));
                          monto_blanco.Border = 0;
                          monto_blanco.Colspan = 6;
                          tblMontoTotal.AddCell(monto_blanco);

                          PdfPTable tbl_monto_total = new PdfPTable(2);
                          tbl_monto_total.WidthPercentage = 100;
                          var monedaDatos1 = GetCurrencySymbol(cabecera.Cs_tag_DocumentCurrencyCode);
                          PdfPCell labelMontoTotal = new PdfPCell(new Phrase("IMPORTE TOTAL:", _clienteFontBold));
                          labelMontoTotal.HorizontalAlignment = Element.ALIGN_LEFT;
                          PdfPCell valueMontoTotal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + cabecera.Cs_tag_LegalMonetaryTotal_PayableAmount_currencyID, _clienteFontContent));
                          valueMontoTotal.HorizontalAlignment = Element.ALIGN_RIGHT;

                          tbl_monto_total.AddCell(labelMontoTotal);
                          tbl_monto_total.AddCell(valueMontoTotal);

                          PdfPCell monto_total = new PdfPCell(tbl_monto_total);
                          monto_total.Border = 0;
                          monto_total.Colspan = 4;
                          tblMontoTotal.AddCell(monto_total);

                          doc.Add(tblMontoTotal);*/
                    }
                    /*-------------End Monto Total----------------*/

                    //FOOTER
                    PdfPTable tblInfoFooter = new PdfPTable(10);
                    tblInfoFooter.WidthPercentage = 100;

                    //comentarios
                    PdfPTable tblInfoComentarios = new PdfPTable(1);
                    tblInfoComentarios.WidthPercentage = 100;

                    //tblInfoComentarios.TotalWidth = 144f;
                    //tblInfoComentarios.LockedWidth = true;

                    PdfPCell tituComentarios = new PdfPCell(new Phrase("Observaciones:", _clienteFontBold));
                    tituComentarios.Border = 0;
                    tituComentarios.HorizontalAlignment = Element.ALIGN_LEFT;
                    tituComentarios.PaddingBottom = 5f;
                    if (InvoiceTypeCode == "03")
                    {
                        //cuando es boleta
                        tituComentarios.PaddingTop = -15f;
                    }
                    else
                    {
                        tituComentarios.PaddingTop = -5f;
                    }

                    tblInfoComentarios.AddCell(tituComentarios);



                    var comentarios_string = teclaf8 + " " + teclavtrlm;

                    PdfPCell contComentarios = new PdfPCell(new Phrase(teclavtrlm, _clienteFontContentMinFooter));
                    contComentarios.BorderWidth = 0.75f;
                    contComentarios.PaddingBottom = 5f;
                    contComentarios.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tblInfoComentarios.AddCell(contComentarios);

                    /* if (cabecera.Cs_tag_InvoiceTypeCode != "03")
                     {*/
                    PdfPCell tituDatos = new PdfPCell(new Phrase("DATOS:", _clienteFontBold));
                    tituDatos.Border = 0;
                    tituDatos.HorizontalAlignment = Element.ALIGN_LEFT;
                    tituDatos.PaddingBottom = 5f;
                    tblInfoComentarios.AddCell(tituDatos);


                    /* TABLA PARA NRO ORDEN PEDIDO Y CUENTAS BANCARIAS*/
                    PdfPTable tblOrdenCuenta = new PdfPTable(11);
                    tblOrdenCuenta.WidthPercentage = 100;
                    PdfPCell labelOrden = new PdfPCell(new Phrase("Nº Orden de Pedido:", _clienteFontBoldContentMinFooter));
                    labelOrden.Colspan = 2;
                    labelOrden.Border = 0;
                    labelOrden.HorizontalAlignment = Element.ALIGN_LEFT;
                    PdfPCell valueOrden = new PdfPCell(new Phrase(teclaf8, _clienteFontContent));
                    valueOrden.Colspan = 9;
                    valueOrden.Border = 0;
                    valueOrden.HorizontalAlignment = Element.ALIGN_LEFT;
                    tblOrdenCuenta.AddCell(labelOrden);
                    tblOrdenCuenta.AddCell(valueOrden);

                    PdfPCell labelCuentas = new PdfPCell(new Phrase("Ctas Bancarias:", _clienteFontBoldContentMinFooter));
                    labelCuentas.Colspan = 2;
                    labelCuentas.Border = 0;
                    labelCuentas.HorizontalAlignment = Element.ALIGN_LEFT;

                    var pdat = new Paragraph();
                    pdat.Font = _clienteFontContentMinFooter;
                    pdat.Add(cuentasbancarias);
                    PdfPCell valueCuentas = new PdfPCell(pdat);
                    valueCuentas.Colspan = 9;
                    valueCuentas.Border = 0;
                    valueCuentas.HorizontalAlignment = Element.ALIGN_LEFT;

                    tblOrdenCuenta.AddCell(labelCuentas);
                    tblOrdenCuenta.AddCell(valueCuentas);

                    tblInfoComentarios.AddCell(tblOrdenCuenta);

                    PdfPCell cellBlanco = new PdfPCell(new Phrase("", _clienteFontBoldContentMinFooter));
                    cellBlanco.Border = 0;

                    tblInfoComentarios.AddCell(cellBlanco);
                    // }
                    /*PdfPCell contDatos = new PdfPCell(pdat);
                    contDatos.BorderWidth = 0.75f;
                    contDatos.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tblInfoComentarios.AddCell(contDatos);
                    */

                    //resumen 
                    PdfPTable tblInfoResumen = new PdfPTable(4);
                    tblInfoResumen.WidthPercentage = 100;

                    //tblInfoResumen.TotalWidth = 144f;
                    //tblInfoResumen.LockedWidth = true;
                    sub_total += double.Parse(op_gravada1, CultureInfo.InvariantCulture);

                    if (InvoiceTypeCode != "03")
                    {
                        // moneda

                        var monedaDatos = GetCurrencySymbol(DocumentCurrencyCode);
                        string output_subtotal = "";


                        if (op_gratuita1 == "0.00")
                        {
                            output_subtotal = sub_total.ToString("#,0.00", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            output_subtotal = "0.00";
                        }

                        PdfPCell resItem6 = new PdfPCell(new Phrase("Sub Total", _clienteFontBold));
                        resItem6.Colspan = 2;
                        resItem6.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue6 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + output_subtotal, _clienteFontContent));
                        resvalue6.Colspan = 2;
                        resvalue6.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem6);
                        tblInfoResumen.AddCell(resvalue6);

                        PdfPCell resItem7 = new PdfPCell(new Phrase("Otros Cargos", _clienteFontBold));
                        resItem7.Colspan = 2;
                        resItem7.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue7 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(LMTChargeTotalAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue7.Colspan = 2;
                        resvalue7.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem7);
                        tblInfoResumen.AddCell(resvalue7);

                        PdfPCell resItem8 = new PdfPCell(new Phrase("Descuento Global", _clienteFontBold));
                        resItem8.Colspan = 2;
                        resItem8.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue8 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(total_descuentos, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue8.Colspan = 2;
                        resvalue8.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem8);
                        tblInfoResumen.AddCell(resvalue8);

                        PdfPCell resItem1 = new PdfPCell(new Phrase("Operaciones Gravadas", _clienteFontBold));
                        resItem1.Colspan = 2;
                        resItem1.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue1 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_gravada1, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue1.Colspan = 2;
                        resvalue1.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem1);
                        tblInfoResumen.AddCell(resvalue1);

                        PdfPCell resItem2 = new PdfPCell(new Phrase("Operaciones Inafectas", _clienteFontBold));
                        resItem2.Colspan = 2;
                        resItem2.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue2 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_inafecta1, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue2.Colspan = 2;
                        resvalue2.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem2);
                        tblInfoResumen.AddCell(resvalue2);

                        PdfPCell resItem3 = new PdfPCell(new Phrase("Operaciones Exoneradas", _clienteFontBold));
                        resItem3.Colspan = 2;
                        resItem3.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue3 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_exonerada, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue3.Colspan = 2;
                        resvalue3.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem3);
                        tblInfoResumen.AddCell(resvalue3);

                        if (imp_IGV != "")
                        {
                            PdfPCell resItem4_1 = new PdfPCell(new Phrase("IGV", _clienteFontBold));
                            resItem4_1.Colspan = 2;
                            resItem4_1.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_1 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(imp_IGV, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue4_1.Colspan = 2;
                            resvalue4_1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_1);
                            tblInfoResumen.AddCell(resvalue4_1);
                        }
                        /*if (imp_ISC != "")
                        {
                            PdfPCell resItem4_2 = new PdfPCell(new Phrase("ISC", _clienteFontBold));
                            resItem4_2.Colspan = 2;
                            resItem4_2.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_2 = new PdfPCell(new Phrase(imp_ISC, _clienteFontContent));
                            resvalue4_2.Colspan = 2;
                            resvalue4_2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_2);
                            tblInfoResumen.AddCell(resvalue4_2);
                        }
                        if (imp_OTRO != "")
                        {
                            PdfPCell resItem4_3 = new PdfPCell(new Phrase("Otros tributos", _clienteFontBold));
                            resItem4_3.Colspan = 2;
                            resItem4_3.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_3 = new PdfPCell(new Phrase(imp_OTRO, _clienteFontContent));
                            resvalue4_3.Colspan = 2;
                            resvalue4_3.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_3);
                            tblInfoResumen.AddCell(resvalue4_3);
                        }*/
                        string importeString = "IMPORTE TOTAL:";
                        if (op_percepcion != "0.00")
                        {
                            importeString = "TOTAL:";
                        }
                        else
                        {
                            importeString = "IMPORTE TOTAL:";

                        }

                        PdfPCell resItem5 = new PdfPCell(new Phrase(importeString, _clienteFontBold));
                        resItem5.Colspan = 2;
                        resItem5.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue5 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(LMTPayableAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue5.Colspan = 2;
                        resvalue5.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tblInfoResumen.AddCell(resItem5);
                        tblInfoResumen.AddCell(resvalue5);

                        if (op_percepcion != "0.00")
                        {
                            PdfPCell resItem51 = new PdfPCell(new Phrase("PERCEPCION:", _clienteFontBold));
                            resItem51.Colspan = 2;
                            resItem51.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue51 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_percepcion, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue51.Colspan = 2;
                            resvalue51.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem51);
                            tblInfoResumen.AddCell(resvalue51);

                            double new_total = Convert.ToDouble(LMTPayableAmount, CultureInfo.CreateSpecificCulture("en-US")) + Convert.ToDouble(op_percepcion, CultureInfo.CreateSpecificCulture("en-US"));

                            PdfPCell resItem52 = new PdfPCell(new Phrase("TOTAL VENTA:", _clienteFontBold));
                            resItem52.Colspan = 2;
                            resItem52.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue52 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + new_total.ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue52.Colspan = 2;
                            resvalue52.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem52);
                            tblInfoResumen.AddCell(resvalue52);


                        }



                        PdfPCell resItem9 = new PdfPCell(new Phrase("", _clienteFontBold));
                        resItem9.Colspan = 2;
                        resItem9.Border = 0;
                        resItem9.PaddingBottom = 0f;
                        resItem9.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue9 = new PdfPCell(new Phrase("", _clienteFontContent));
                        resvalue9.Colspan = 2;
                        resvalue9.Border = 0;
                        resvalue9.PaddingBottom = 0f;
                        resvalue9.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tblInfoResumen.AddCell(resItem9);
                        tblInfoResumen.AddCell(resvalue9);


                    }
                    //lado izquierdo
                    PdfPCell tblInfoFooterLeft = new PdfPCell(tblInfoComentarios);
                    if (InvoiceTypeCode != "03")
                    {
                        tblInfoFooterLeft.Colspan = 6;
                        tblInfoFooterLeft.PaddingRight = 10f;
                    }
                    else
                    {
                        tblInfoFooterLeft.Colspan = 10;
                        tblInfoFooterLeft.PaddingRight = 0;
                    }

                    tblInfoFooterLeft.Border = 0;

                    tblInfoFooter.AddCell(tblInfoFooterLeft);
                    //lado derecho

                    PdfPCell tblInfoFooterRight = new PdfPCell(tblInfoResumen);
                    tblInfoFooterRight.Colspan = 4;
                    tblInfoFooterRight.Border = 0;
                    tblInfoFooter.AddCell(tblInfoFooterRight);


                    doc.Add(tblInfoFooter);
                    doc.Add(tblBlanco);
                    if (InvoiceTypeCode != "03")
                    {
                        /*----------- Monto total en letras --------------*/
                        NumLetra totalLetras = new NumLetra();
                        PdfPTable tblInfoMontoTotal = new PdfPTable(1);
                        tblInfoMontoTotal.WidthPercentage = 100;
                        PdfPCell infoTotal = new PdfPCell(new Phrase("SON: " + totalLetras.Convertir(LMTPayableAmount, true, DocumentCurrencyCode), _clienteFontContent));
                        infoTotal.BorderWidth = 0.75f;
                        infoTotal.HorizontalAlignment = Element.ALIGN_LEFT;
                        tblInfoMontoTotal.AddCell(infoTotal);
                        doc.Add(tblInfoMontoTotal);
                        /*-------------End Monto Total----------------*/
                        doc.Add(tblBlanco);
                    }

                    PdfPTable tblFooter = new PdfPTable(10);
                    tblFooter.WidthPercentage = 100;
                    tblFooter.SpacingBefore = 5;

                    var p = new Paragraph();
                    p.Font = _clienteFontBold;
                    if (op_percepcion != "0.00")
                    {
                        string tipoOperacion = Documento.getTipoOperacion(tipo_op);
                        p.Add("Incorporado al regimen de agentes de Percepcion de IGV - " + tipoOperacion + " (D.S 091-2013) 01/02/2014 \n\n");
                    }
                    p.Add(digestValue + "\n\n");
                    p.Add(info_general.TextoRepresentacionImpresa);
                    p.Add("Puede consultar su comprobante en cpecontasiscorp.com/ConsultaLPFServiciosIntegrales/ \n");

                    PdfPCell DataHash = new PdfPCell(new Phrase(digestValue, _clienteFontBold));
                    DataHash.Border = 0;
                    DataHash.Colspan = 6;
                    DataHash.HorizontalAlignment = Element.ALIGN_CENTER;
                    // DataHash.PaddingTop = 5f;                

                    PdfPCell campo1 = new PdfPCell(p);
                    campo1.Colspan = 6;
                    campo1.Border = 0;
                    campo1.PaddingTop = 0f;
                    campo1.HorizontalAlignment = Element.ALIGN_CENTER;

                    //codigo de barras                               
                    //var hash = new clsNegocioXML();
                    //var hash_obtenido=hash.cs_fxHash(cabecera.Cs_pr_Document_Id);

                    Dictionary<EncodeHintType, object> ob = new Dictionary<EncodeHintType, object>() {
                                {EncodeHintType.ERROR_CORRECTION,ErrorCorrectionLevel.Q }
                            };


                    var textQR = ASPCustomerAssignedAccountID + " | " + InvoiceTypeCode + " | " + doc_serie + "-" + doc_correlativo + " | " + imp_IGV + " | " + LMTPayableAmount + " | " + IssueDate + " | " + ACPAdditionalAccountID + " | " + ACPCustomerAssignedAccountID + " |";

                    BarcodeQRCode qrcode = new BarcodeQRCode(textQR, 400, 400, ob);

                    iTextSharp.text.Image qrcodeImage = qrcode.GetImage();

                    /* BarcodePDF417 barcod = new BarcodePDF417();
                     barcod.SetText(cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID+" | "+ cabecera.Cs_tag_InvoiceTypeCode+" | "+ doc_serie+" | "+doc_correlativo+" | "+ impuestos_globales.Cs_tag_TaxSubtotal_TaxAmount+" | "+ cabecera.Cs_tag_LegalMonetaryTotal_PayableAmount_currencyID+" | "+ cabecera.Cs_tag_IssueDate+" | "+cabecera.Cs_tag_AccountingCustomerParty_AdditionalAccountID+" | "+cabecera.Cs_tag_AccountingCustomerParty_CustomerAssignedAccountID+" | "+ digestValue + " | "+signatureValue+" |");
                     barcod.ErrorLevel = 5;
                     barcod.Options = BarcodePDF417.PDF417_FORCE_BINARY;

                     iTextSharp.text.Image imagePDF417 = barcod.GetImage();*/
                    //qrcodeImage.ScaleAbsolute(100f, 90f);
                    PdfPCell blanco12 = new PdfPCell();
                    // blanco12.Image = qrcodeImage;
                    blanco12.AddElement(new Chunk(qrcodeImage, 55f, -65f));
                    blanco12.Border = 0;
                    blanco12.PaddingTop = 15f;
                    blanco12.Colspan = 4;


                    PdfPCell blanco121 = new PdfPCell(new Paragraph(" "));
                    blanco121.Border = 0;
                    blanco121.Colspan = 4;

                    tblFooter.AddCell(campo1);
                    tblFooter.AddCell(blanco12);
                    //tblFooter.AddCell(campo1);
                    // tblFooter.AddCell(blanco121);

                    doc.Add(tblFooter);


                    doc.Close();
                    File.SetAttributes(newFile, FileAttributes.Normal);
                    writer.Close();

                    url = newFileServer;
                    rutas[0] = newFile;
                    rutas[1] = newxml;
                    rutas[2] = newFileServer;
                    rutas[3] = newXmlServer;
            
        }
                ///////////////////////////////////////////////////////////////////////////
                else /*if (uubbll == "2.1")*/
                {

                    //get accounting supplier party
                    XmlNodeList AccountingSupplierParty = xmlDocument.GetElementsByTagName("AccountingSupplierParty");//emisor
                    foreach (XmlNode dat in AccountingSupplierParty)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        //var caaid = xmlDocumentinner.GetElementsByTagName("CustomerAssignedAccountID");
                        //if (caaid.Count > 0)
                        //{
                        //    ASPCustomerAssignedAccountID = caaid.Item(0).InnerText;
                        //}
                        //var aacid = xmlDocumentinner.GetElementsByTagName("AdditionalAccountID");
                        //if (aacid.Count > 0)
                        //{
                        //    ASPAdditionalAccountID = aacid.Item(0).InnerText;
                        //}
                        //var stname = xmlDocumentinner.GetElementsByTagName("StreetName");
                        //if (stname.Count > 0)
                        //{
                        //    ASPStreetName = stname.Item(0).InnerText;
                        //}
                        var regname = xmlDocumentinner.GetElementsByTagName("RegistrationName");
                        if (regname.Count > 0)
                        {
                            ASPRegistrationName = regname.Item(0).InnerText;
                        }
                        //NUEVO
                        var atc = xmlDocumentinner.GetElementsByTagName("AddressTypeCode");
                        if (atc.Count > 0)
                        {
                            ASPAddressTypeCode = atc.Item(0).InnerText;
                        }

                        var ln = xmlDocumentinner.GetElementsByTagName("Line");
                        if (ln.Count > 0)
                        {
                            ASPLine = ln.Item(0).InnerText;
                        }

                        var ppii = xmlDocumentinner.GetElementsByTagName("PartyIdentification");
                        if (ppii.Count > 0)
                        {
                            ASPPartyIdentification = ppii.Item(0).InnerText;
                        }
                        var smid = xmlDocumentinner.GetElementsByTagName("schemeID");
                        if (smid.Count > 0)
                        {
                            ACPSchemeId = smid.Item(0).InnerText;
                        }
                        //SE AGREGO MP
                        var rraddd = xmlDocumentinner.GetElementsByTagName("RegistrationAddress");
                        if (rraddd.Count > 0)
                        {
                            ASPRegistrationAddress = rraddd.Item(0).InnerText;
                        }
                        //MP
                    }

                    ////////////////////
                    //get accounting supplier party
                    //yo
                    //clasEntityDocument_Line_TaxTotal accountingCustomerPartyItem;//



                    #region Datos del Cliente
                    XmlNodeList AccountingCustomerParty = xmlDocument.GetElementsByTagName("AccountingCustomerParty");


                    foreach (XmlNode dat in AccountingCustomerParty)
                    {
                        //accountingCustomerPartyItem = new clasEntityDocument_Line_TaxTotal();//
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);


                        var partyy = xmlDocumentinner.GetElementsByTagName("Party");
                        if (partyy.Count > 0)
                        {
                            ACPParty = partyy.Item(0).InnerText;
                        }

                        var ppii = xmlDocumentinner.GetElementsByTagName("PartyIdentification");
                        if (ppii.Count > 0)
                        {
                            ACPPartyIdentification = ppii.Item(0).InnerText;
                        }
                        //se agrego MP
                        var iiddd = xmlDocumentinner.GetElementsByTagName("ID");
                        if (iiddd.Count > 0)
                        {
                            ACPId = iiddd.Item(0).InnerText;
                        }

                        if (iiddd.Count > 0)
                        {
                            ACPSchemeId = iiddd.Item(0).Attributes.Item(1).InnerText;
                        }
                        ///////////////MP

                        var caaid = xmlDocumentinner.GetElementsByTagName("CustomerAssignedAccountID");
                        if (caaid.Count > 0)
                        {
                            ACPCustomerAssignedAccountID = caaid.Item(0).InnerText;
                        }
                        var aacid = xmlDocumentinner.GetElementsByTagName("AdditionalAccountID");
                        if (aacid.Count > 0)
                        {
                            ACPAdditionalAccountID = aacid.Item(0).InnerText;
                        }
                        var descr = xmlDocumentinner.GetElementsByTagName("Description");
                        if (descr.Count > 0)
                        {
                            ACPDescription = descr.Item(0).InnerText;
                        }
                        var regname = xmlDocumentinner.GetElementsByTagName("RegistrationName");
                        if (regname.Count > 0)
                        {
                            ACPRegistrationName = regname.Item(0).InnerText;
                        }
                        //NUEVO                    

                        var adtc = xmlDocumentinner.GetElementsByTagName("AddressTypeCode");
                        if (adtc.Count > 0)
                        {
                            ACPAddressTypeCode = adtc.Item(0).InnerText;
                        }

                        var adle = xmlDocumentinner.GetElementsByTagName("AddressLine");
                        if (adle.Count > 0)
                        {
                            ACPAddressLine = adle.Item(0).InnerText;
                        }

                        var rradd = xmlDocumentinner.GetElementsByTagName("RegistrationAddress");
                        if (rradd.Count > 0)
                        {
                            ACPRegistrationAddress = rradd.Item(0).InnerText;
                        }

                    }
                    #endregion

                    XmlNodeList DiscrepancyResponse = xmlDocument.GetElementsByTagName("DiscrepancyResponse");
                    foreach (XmlNode dat in DiscrepancyResponse)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var refid = xmlDocumentinner.GetElementsByTagName("ReferenceID");
                        if (refid.Count > 0)
                        {
                            DReferenceID = refid.Item(0).InnerText;
                        }
                        var respcode = xmlDocumentinner.GetElementsByTagName("ResponseCode");
                        if (respcode.Count > 0)
                        {
                            DResponseCode = respcode.Item(0).InnerText;
                        }
                        var descr = xmlDocumentinner.GetElementsByTagName("Description");
                        if (descr.Count > 0)
                        {
                            DDescription = descr.Item(0).InnerText;
                        }

                    }

                    XmlNodeList LegalMonetaryTotal = null;

                    if (InvoiceTypeCode == "08")
                    {
                        LegalMonetaryTotal = xmlDocument.GetElementsByTagName("RequestedMonetaryTotal");
                    }
                    else
                    {
                        LegalMonetaryTotal = xmlDocument.GetElementsByTagName("LegalMonetaryTotal");
                    }

                    foreach (XmlNode dat in LegalMonetaryTotal)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);

                        var cta = xmlDocumentinner.GetElementsByTagName("ChargeTotalAmount");
                        if (cta.Count > 0)
                        {
                            LMTChargeTotalAmount = cta.Item(0).InnerText;
                        }
                        var pam = xmlDocumentinner.GetElementsByTagName("PayableAmount");
                        if (pam.Count > 0)
                        {
                            LMTPayableAmount = pam.Item(0).InnerText;
                        }
                        //nuevo -MP
                        var lea = xmlDocumentinner.GetElementsByTagName("LineExtensionAmount");
                        if (lea.Count > 0)
                        {
                            LMTLineExtensionAmount = lea.Item(0).InnerText;
                        }

                        var tia = xmlDocumentinner.GetElementsByTagName("TaxInclusiveAmount");
                        if (tia.Count > 0)
                        {
                            LMTTaxInclusiveAmount = tia.Item(0).InnerText;
                        }

                        var ata = xmlDocumentinner.GetElementsByTagName("AllowanceTotalAmount");
                        if (ata.Count > 0)
                        {
                            LMTAllowanceTotalAmount = ata.Item(0).InnerText;
                        }

                        var paa = xmlDocumentinner.GetElementsByTagName("PrepaidAmount");
                        if (paa.Count > 0)
                        {
                            LMTPrepaidAmount = paa.Item(0).InnerText;
                        }
                        //MP                                          

                    }

                    List<clasEntityDocument_AdditionalComments> Lista_additional_coments = new List<clasEntityDocument_AdditionalComments>();
                    clasEntityDocument_AdditionalComments adittionalComents;
                    XmlNodeList datosCabecera = xmlDocument.GetElementsByTagName("DatosCabecera");
                    foreach (XmlNode dat in datosCabecera)
                    {
                        var NodosHijos = dat.ChildNodes;
                        for (int z = 0; z < NodosHijos.Count; z++)
                        {
                            adittionalComents = new clasEntityDocument_AdditionalComments();
                            adittionalComents.Cs_pr_TagNombre = NodosHijos.Item(z).LocalName;
                            adittionalComents.Cs_pr_TagValor = NodosHijos.Item(z).ChildNodes.Item(0).InnerText;
                            Lista_additional_coments.Add(adittionalComents);
                        }
                    }

                    //comentarios contenido
                    var teclaf8 = " ";//comment1
                    var teclavtrlm = " ";//commnet2
                    var cuentasbancarias = " ";//comment 3
                    string CondicionVentaXML = string.Empty;
                    string CondicionPagoXML = string.Empty;
                    string VendedorXML = string.Empty;
                    string GuiaRemision = string.Empty;

                    foreach (var itemm in Lista_additional_coments)
                    {
                        if (itemm.Cs_pr_TagNombre == "CondPago")
                        {
                            CondicionPagoXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "Vendedor")
                        {
                            VendedorXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "Condicion")
                        {
                            CondicionVentaXML = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "DatEmpresa")
                        {
                            cuentasbancarias = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "TeclaF8")
                        {
                            teclaf8 = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "TeclasCtrlM")
                        {
                            teclavtrlm = itemm.Cs_pr_TagValor;
                        }
                        if (itemm.Cs_pr_TagNombre == "NumGuias")
                        {
                            GuiaRemision = itemm.Cs_pr_TagValor;
                        }

                    }

                    string sucursal = string.Empty;
                    string[] sucursalpartes = cuentasbancarias.Split('*');
                    if (sucursalpartes.Length > 0)
                    {
                        sucursal = sucursalpartes[0];
                    }

                    //MP-NUEVO
                    XmlNodeList DespatchDocumentReference = xmlDocument.GetElementsByTagName("DespatchDocumentReference");
                    foreach (XmlNode ddrr in DespatchDocumentReference)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(ddrr.OuterXml);

                        var ididid = xmlDocumentinner.GetElementsByTagName("ID");
                        if (ididid.Count > 0)
                        {
                            GuiaRemision = ididid.Item(0).InnerText; //  DDRguiaremi
                        }
                    }

                    //MP
                    //tabla info empresa
                    PdfPTable tblInforEmpresa = new PdfPTable(1);
                    tblInforEmpresa.WidthPercentage = 100;
                    PdfPCell NameEmpresa = new PdfPCell(new Phrase(ASPRegistrationName, _HeaderFont));
                    NameEmpresa.BorderWidth = 0;
                    NameEmpresa.Border = 0;
                    tblInforEmpresa.AddCell(NameEmpresa);

                    var pa = new Paragraph();
                    pa.Font = _clienteFontBoldMin;
                    pa.Add("Dirección: AV. ALMIRANTE MIGUEL GRAU NRO. 093 DPTO. C INT. 102 (COSTADO BANCO DE LA NACION) LIMA - LIMA - BARRANCO \n");
                    //pa.Add(sucursal);

                    PdfPCell EstaticoEmpresa = new PdfPCell(pa);
                    EstaticoEmpresa.BorderWidth = 0;
                    EstaticoEmpresa.Border = 0;
                    tblInforEmpresa.AddCell(EstaticoEmpresa);

                    PdfPCell celdaInfoEmpresa = new PdfPCell(tblInforEmpresa);
                    celdaInfoEmpresa.Border = 0;
                    tblHeaderLeft.AddCell(celdaInfoEmpresa);
                    // PdfPCell blanco = new PdfPCell();
                    // blanco.Border = 0;


                    //tabla para info ruc
                    PdfPTable tblInforRuc = new PdfPTable(1);
                    tblInforRuc.WidthPercentage = 100;

                    PdfPCell TituRuc = new PdfPCell(new Phrase("R.U.C. " + ASPPartyIdentification/*ASPCustomerAssignedAccountID*/, _TitleFontN));//yooooooo
                    TituRuc.BorderWidthTop = 0.75f;
                    TituRuc.BorderWidthBottom = 0.75f;
                    TituRuc.BorderWidthLeft = 0.75f;
                    TituRuc.BorderWidthRight = 0.75f;
                    TituRuc.HorizontalAlignment = Element.ALIGN_CENTER;
                    TituRuc.PaddingTop = 10f;
                    TituRuc.PaddingBottom = 10f;

                    PdfPCell TipoDoc = new PdfPCell(new Phrase(info_general.Nombre, _TitleFontN));
                    TipoDoc.BorderWidthLeft = 0.75f;
                    TipoDoc.BorderWidthRight = 0.75f;
                    TipoDoc.HorizontalAlignment = Element.ALIGN_CENTER;
                    TipoDoc.PaddingTop = 10f;
                    TipoDoc.PaddingBottom = 10f;

                    PdfPCell SerieDoc = new PdfPCell(new Phrase("N° " + cabecera.Cs_tag_ID, _TitleFont));
                    SerieDoc.BorderWidthBottom = 0.75f;
                    SerieDoc.BorderWidthRight = 0.75f;
                    SerieDoc.BorderWidthLeft = 0.75f;
                    SerieDoc.BorderWidthTop = 0.75f;
                    SerieDoc.HorizontalAlignment = Element.ALIGN_CENTER;
                    SerieDoc.PaddingTop = 10f;
                    SerieDoc.PaddingBottom = 10f;

                    PdfPCell blanco2 = new PdfPCell(new Paragraph(" "));
                    blanco2.Border = 0;


                    tblInforRuc.AddCell(TituRuc);
                    //tblInforRuc.AddCell(blanco2);
                    tblInforRuc.AddCell(TipoDoc);
                    //tblInforRuc.AddCell(blanco2);
                    tblInforRuc.AddCell(SerieDoc);
                    tblInforRuc.AddCell(blanco2);

                    PdfPCell infoRuc = new PdfPCell(tblInforRuc);
                    infoRuc.Colspan = 2;
                    infoRuc.BorderWidth = 0;

                    PdfPCell celdaHeaderLeft = new PdfPCell(tblHeaderLeft);
                    celdaHeaderLeft.Border = 0;
                    celdaHeaderLeft.Colspan = 3;

                    // Añadimos las celdas a la tabla
                    tblPrueba.AddCell(celdaHeaderLeft);
                    // tblPrueba.AddCell(blanco);
                    tblPrueba.AddCell(infoRuc);

                    doc.Add(tblPrueba);

                    PdfPTable tblBlanco = new PdfPTable(1);
                    tblBlanco.WidthPercentage = 100;
                    PdfPCell blanco3 = new PdfPCell((new Paragraph(" ")));
                    blanco3.Border = 0;

                    tblBlanco.AddCell(blanco3);

                    doc.Add(tblBlanco);

                    //Informacion cliente
                    PdfPTable tblInfoCliente = new PdfPTable(10);
                    tblInfoCliente.WidthPercentage = 100;

                    // Llenamos la tabla con información del cliente
                    PdfPCell cliente = new PdfPCell(new Phrase("Cliente:", _clienteFontBoldMin));
                    cliente.BorderWidth = 0;
                    cliente.Colspan = 1;

                    PdfPCell clNombre = new PdfPCell(new Phrase(ACPRegistrationName, _clienteFontContentMinFooter));
                    clNombre.BorderWidth = 0;
                    clNombre.Colspan = 5;

                    PdfPCell fecha = new PdfPCell(new Phrase("Fecha de Emision:", _clienteFontBoldMin));
                    fecha.BorderWidth = 0;
                    fecha.Colspan = 2;

                    var fechaString = dt.ToString("dd") + " de " + dt.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-ES")) + " " + dt.ToString("yyyy");
                    PdfPCell clFecha = new PdfPCell(new Phrase(fechaString.ToUpper(), _clienteFontContentMinFooter));
                    clFecha.BorderWidth = 0;
                    clFecha.Colspan = 2;

                    // Añadimos las celdas a la tabla
                    tblInfoCliente.AddCell(cliente);
                    tblInfoCliente.AddCell(clNombre);
                    tblInfoCliente.AddCell(fecha);
                    tblInfoCliente.AddCell(clFecha);

                    //////////////////////////////////////////////////////////

                    string Nameee = string.Empty;
                    if (xmlDocument.InnerXml.Contains("BuyerReference"))//si es que esta la etiqueta
                    {
                        //if (InvoiceTypeCode == "01" | InvoiceTypeCode == "03")
                        //{ }
                        Nameee = xmlDocument.GetElementsByTagName("BuyerReference").Item(0).InnerText;

                        if (Nameee.Length > 0)
                        {
                            BRCondicion = xmlDocument.GetElementsByTagName("BuyerReference").Item(0).InnerText;

                            string[] cuentass = BRCondicion.Split('~');
                            if (cuentass.Length > 0)
                            {

                                int n = 2;
                                foreach (var item_ in cuentass)
                                {
                                    if (item_.Contains("DatosEmpresa"))
                                    {
                                        string xml_ = item_.Replace("DatosEmpresa: ", "");
                                        string[] bancos = xml_.Split('\r', '\n');

                                        foreach (var _var in bancos)
                                        {
                                            if (_var != "")
                                                cuentasbancarias = cuentasbancarias + _var + "\n";
                                            n++;
                                        }

                                    }
                                }
                            }

                            string[] condicionn = BRCondicion.Split(':', '~');

                            if (condicionn.Length > 0)
                            {
                                int n = 0;
                                foreach (var item_ in condicionn)
                                {
                                    if (item_.Contains("Condicion"))
                                    {
                                        CondicionVentaXML = condicionn[n + 1];//concatenado
                                    }
                                    n++;
                                }
                            }

                            string[] observacionn = BRCondicion.Split(':', '~');

                            if (observacionn.Length > 0)
                            {
                                int n = 0;
                                foreach (var item_ in observacionn)
                                {
                                    if (item_.Contains("Adicional_2"))
                                    {
                                        teclavtrlm = observacionn[n + 1];//concatenado
                                    }
                                    n++;
                                }
                            }

                            string[] direcciondos = BRCondicion.Split(':', '~');

                            if (direcciondos.Length > 0)
                            {
                                int n = 0;
                                foreach (var item_ in observacionn)
                                {
                                    if (item_.Contains("Direccion Ad"))
                                    {
                                        direccionad = direcciondos[n + 1];//concatenado
                                    }
                                    n++;
                                }
                            }

                        }
                    }

                    /////////////////////////////////////////////////////////////////////////////////
                    PdfPCell direccion = new PdfPCell(new Phrase("Direccion:", _clienteFontBoldMin));
                    direccion.BorderWidth = 0;
                    direccion.Colspan = 1;

                    PdfPCell clDireccion = new PdfPCell(new Phrase(/*ACPDescription*/ACPRegistrationAddress/*ACPAddressTypeCode*//* ACPAddressLine*/, _clienteFontContentMinFooter)); //yoooo
                    clDireccion.BorderWidth = 0;
                    clDireccion.Colspan = 5;

                    /*En caso sea nota de credito o debito*/
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        PdfPCell condicionVenta = new PdfPCell(new Phrase("Documento que modifica:", _clienteFontBoldMin));
                        condicionVenta.BorderWidth = 0;
                        condicionVenta.Colspan = 2;


                        PdfPCell clCondicionVenta = new PdfPCell(new Phrase(DReferenceID, _clienteFontContentMinFooter));
                        clCondicionVenta.BorderWidth = 0;
                        clCondicionVenta.Colspan = 2;

                        tblInfoCliente.AddCell(direccion);
                        tblInfoCliente.AddCell(clDireccion);
                        tblInfoCliente.AddCell(condicionVenta);
                        tblInfoCliente.AddCell(clCondicionVenta);
                    }
                    else
                    {
                        NumLetra monedaLetras = new NumLetra();
                        var monedaLetra = monedaLetras.getMoneda(DocumentCurrencyCode);
                        PdfPCell moneda = new PdfPCell(new Phrase("Moneda:", _clienteFontBoldMin));
                        moneda.BorderWidth = 0;
                        moneda.Colspan = 2;

                        PdfPCell clMoneda = new PdfPCell(new Phrase(monedaLetra.ToUpper(), _clienteFontContentMinFooter));
                        clMoneda.BorderWidth = 0;
                        clMoneda.Colspan = 2;

                        /* PdfPCell condicionVenta = new PdfPCell(new Phrase("Condicion Venta:", _clienteFontBoldMin));
                         condicionVenta.BorderWidth = 0;
                         condicionVenta.Colspan = 2;


                         PdfPCell clCondicionVenta = new PdfPCell(new Phrase("", _clienteFontContentMinFooter));
                         clCondicionVenta.BorderWidth = 0;
                         clCondicionVenta.Colspan = 2;
                         */
                        tblInfoCliente.AddCell(direccion);
                        tblInfoCliente.AddCell(clDireccion);
                        tblInfoCliente.AddCell(moneda);
                        tblInfoCliente.AddCell(clMoneda);

                    }
                    if (direccionad != "")
                    {
                        if (InvoiceTypeCode == "01" | InvoiceTypeCode == "03")
                        {
                            PdfPCell direccionadad = new PdfPCell(new Phrase("Sucursal:", _clienteFontBoldMin));
                            direccionadad.BorderWidth = 0;
                            direccionadad.Colspan = 1;

                            PdfPCell clDireccionadad = new PdfPCell(new Phrase(direccionad, _clienteFontContentMinFooter)); //MP
                            clDireccionadad.BorderWidth = 0;
                            clDireccionadad.Colspan = 5;

                            PdfPCell vacioo = new PdfPCell(new Phrase(" ", _clienteFontBoldMin));
                            vacioo.BorderWidth = 0;
                            vacioo.Colspan = 2;

                            PdfPCell clvacioo = new PdfPCell(new Phrase(/*ACPDescription*/" " /*ACPAddressTypeCode*//* ACPAddressLine*/, _clienteFontContentMinFooter)); //yoooo
                            clvacioo.BorderWidth = 0;
                            clvacioo.Colspan = 2;

                            tblInfoCliente.AddCell(direccionadad);
                            tblInfoCliente.AddCell(clDireccionadad);
                            tblInfoCliente.AddCell(vacioo);
                            tblInfoCliente.AddCell(clvacioo);

                        }
                    }

                    ////////////////////////////////////////

                    //if (InvoiceTypeCode == "01" | InvoiceTypeCode == "03" || InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    //{
                    //   teclaf8 = xmlDocument.GetElementsByTagName("OrderReference").Item(0).InnerText;
                    //}

                    //MP - NUEVO

                    string compras = string.Empty;

                    //if (compras.Contains("OrderReference"))
                    if (xmlDocument.InnerXml.Contains("OrderReference"))
                    {
                        teclaf8 = xmlDocument.GetElementsByTagName("OrderReference").Item(0).InnerText;
                    }

                    //MP -NUEVO
                    //
                    //Codigo para cuando tenga datos condicion de venta 
                    string[] newCondicionVenta = new string[2];
                    if (CondicionVentaXML != "")
                    {

                        try
                        {
                            newCondicionVenta = CondicionVentaXML.Split('-');
                        }
                        catch (Exception)
                        {
                            newCondicionVenta[1] = CondicionVentaXML;
                        }

                    }
                    CondicionVentaXML = newCondicionVenta[1];
                    //

                    // Añadimos las celdas a la tabla de info cliente

                    //ACPSchemeId = "";
                    var docName = getTipoDocIdentidad(ACPSchemeId);

                    PdfPCell ruc = new PdfPCell(new Phrase(docName + " N°:", _clienteFontBoldMin));
                    ruc.BorderWidth = 0;
                    ruc.Colspan = 1;

                    PdfPCell clRUC = new PdfPCell(new Phrase(/*ACPCustomerAssignedAccountID*/ ACPPartyIdentification, _clienteFontContentMinFooter));//SE AGREGO
                    clRUC.BorderWidth = 0;
                    clRUC.Colspan = 5;
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        NumLetra monedaLetras1 = new NumLetra();
                        var monedaLetra_ = monedaLetras1.getMoneda(DocumentCurrencyCode);
                        PdfPCell moneda_ = new PdfPCell(new Phrase("Moneda:", _clienteFontBoldMin));
                        moneda_.BorderWidth = 0;
                        moneda_.Colspan = 2;

                        PdfPCell clMoneda_ = new PdfPCell(new Phrase(monedaLetra_.ToUpper(), _clienteFontContentMinFooter));
                        clMoneda_.BorderWidth = 0;
                        clMoneda_.Colspan = 2;
                        tblInfoCliente.AddCell(ruc);
                        tblInfoCliente.AddCell(clRUC);
                        tblInfoCliente.AddCell(moneda_);
                        tblInfoCliente.AddCell(clMoneda_);
                    }
                    else
                    {
                        //MP-NUEVO
                        //se agrego codigo para quitar numero a condicion de venta
                        //por el momento se comenta

                        //string[] newCondicionVenta = new string[2];

                        //    try
                        //    {
                        //        newCondicionVenta = CondicionVentaXML.Split('-');
                        //    }
                        //    catch (Exception)
                        //    {
                        //        newCondicionVenta[1] = CondicionVentaXML;
                        //    }


                        //    CondicionVentaXML = newCondicionVenta[1];

                        //MP - NUEVO
                        //NumLetra monedaLetras = new NumLetra();
                        //  var monedaLetra_ = monedaLetras.getMoneda(cabecera.Cs_tag_DocumentCurrencyCode);
                        PdfPCell moneda_ = new PdfPCell(new Phrase("Condicion de Venta", _clienteFontBoldMin));//mp 
                        moneda_.BorderWidth = 0;
                        moneda_.Colspan = 2;

                        PdfPCell clMoneda_ = new PdfPCell(new Phrase(/*valorcondicion*/CondicionVentaXML, _clienteFontContentMinFooter));
                        clMoneda_.BorderWidth = 0;
                        clMoneda_.Colspan = 2;
                        tblInfoCliente.AddCell(ruc);
                        tblInfoCliente.AddCell(clRUC);
                        tblInfoCliente.AddCell(moneda_);
                        tblInfoCliente.AddCell(clMoneda_);

                    }

                    // Añadimos las celdas a la tabla inf

                    /*En caso sea nota de credito o debito*/
                    if (InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {

                        PdfPCell motivomodifica = new PdfPCell(new Phrase("Motivo", _clienteFontBoldMin));
                        motivomodifica.BorderWidth = 0;
                        motivomodifica.Colspan = 1;

                        PdfPCell clmotivomodifica = new PdfPCell(new Phrase(DDescription, _clienteFontContentMinFooter));
                        clmotivomodifica.BorderWidth = 0;
                        clmotivomodifica.Colspan = 5;

                        clasEntityDocument doc_modificado = new clasEntityDocument();
                        string fechaModificado = doc_modificado.cs_pxBuscarFechaDocumento(DReferenceID);
                        PdfPCell docmodifica = new PdfPCell(new Phrase(" ", _clienteFontBoldMin));//Fecha Doc. Modificado:
                        docmodifica.BorderWidth = 0;
                        docmodifica.Colspan = 2;

                        PdfPCell cldocmodifica = new PdfPCell(new Phrase(fechaModificado, _clienteFontContentMinFooter));
                        cldocmodifica.BorderWidth = 0;
                        cldocmodifica.Colspan = 2;

                        tblInfoCliente.AddCell(motivomodifica);
                        tblInfoCliente.AddCell(clmotivomodifica);
                        tblInfoCliente.AddCell(docmodifica);
                        tblInfoCliente.AddCell(cldocmodifica);

                    }
                    else
                    {
                        string Serie = "";
                        string Correlativo = "";

                        List<string> SerieCorrelativo = new List<string>(GuiaRemision.Split('-'));
                        try
                        {
                            foreach (string Item in SerieCorrelativo)
                            {
                                if (Item.Length == 6)
                                    Serie = Item.Substring(2, 4);
                                else
                                    Correlativo = Item.Substring(7, 6);
                            }
                        }
                        catch
                        {
                        }

                        GuiaRemision = Serie + "-" + Correlativo;
                        //SE COMENTO MP
                        PdfPCell docmodificaOr = new PdfPCell(new Phrase("O.de Compra", _clienteFontBoldMin));
                        docmodificaOr.BorderWidth = 0;
                        docmodificaOr.Colspan = 1;

                        PdfPCell clmotivomodificaOr = new PdfPCell(new Phrase(teclaf8, _clienteFontContentMinFooter));
                        clmotivomodificaOr.BorderWidth = 0;
                        clmotivomodificaOr.Colspan = 5;
                        //
                        //SE COMENTO MP
                        PdfPCell docmodifica = new PdfPCell(new Phrase("Guia de Remision:", _clienteFontBoldMin));
                        docmodifica.BorderWidth = 0;
                        docmodifica.Colspan = 2;

                        PdfPCell cldocmodifica = new PdfPCell(new Phrase(GuiaRemision, _clienteFontContentMinFooter));
                        cldocmodifica.BorderWidth = 0;
                        cldocmodifica.Colspan = 2;
                        //
                        //MP
                        tblInfoCliente.AddCell(docmodificaOr);
                        tblInfoCliente.AddCell(clmotivomodificaOr);

                        tblInfoCliente.AddCell(docmodifica);
                        tblInfoCliente.AddCell(cldocmodifica);
                        //MP
                    }

                    /*------------------------------------*/
                    doc.Add(tblInfoCliente);
                    doc.Add(tblBlanco);

                    PdfPTable tblInfoComprobante = new PdfPTable(11);
                    tblInfoComprobante.WidthPercentage = 100;


                    // Llenamos la tabla con información
                    PdfPCell colCodigo = new PdfPCell(new Phrase("Codigo", _clienteFontBoldMin));
                    colCodigo.BorderWidthBottom = 0.75f;
                    colCodigo.BorderWidthLeft = 0.75f;
                    colCodigo.BorderWidthRight = 0.75f;
                    colCodigo.BorderWidthTop = 0.75f;
                    colCodigo.Colspan = 1;
                    colCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                    //PdfPCell colCodigo = new PdfPCell(new Phrase("Item", _clienteFontBoldMin));
                    //colCodigo.BorderWidthBottom = 0.75f;
                    //colCodigo.BorderWidthLeft = 0.75f;
                    //colCodigo.BorderWidthRight = 0.75f;
                    //colCodigo.BorderWidthTop = 0.75f;
                    //colCodigo.Colspan = 1;
                    //colCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colCantidad = new PdfPCell(new Phrase("Cantidad", _clienteFontBoldMin));
                    colCantidad.BorderWidthBottom = 0.75f;
                    colCantidad.BorderWidthLeft = 0;
                    colCantidad.BorderWidthRight = 0.75f;
                    colCantidad.BorderWidthTop = 0.75f;
                    colCantidad.Colspan = 1;
                    colCantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                    /*PdfPCell colUnidadMedida= new PdfPCell(new Phrase("Und Medida", _clienteFontBoldMin));
                    colUnidadMedida.BorderWidth = 0.75f;
                    colUnidadMedida.Colspan = 1;
                    colUnidadMedida.HorizontalAlignment = Element.ALIGN_CENTER;*/

                    PdfPCell colDescripcion = new PdfPCell(new Phrase("Descripcion", _clienteFontBoldMin));
                    colDescripcion.BorderWidthBottom = 0.75f;
                    colDescripcion.BorderWidthLeft = 0;
                    colDescripcion.BorderWidthRight = 0.75f;
                    colDescripcion.BorderWidthTop = 0.75f;
                    colDescripcion.Colspan = 7;
                    colDescripcion.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colPrecUnit = new PdfPCell(new Phrase("Valor Unitario (Sin IGV)", _clienteFontBoldMin));
                    colPrecUnit.BorderWidthBottom = 0.75f;
                    colPrecUnit.BorderWidthLeft = 0;
                    colPrecUnit.BorderWidthRight = 0.75f;
                    colPrecUnit.BorderWidthTop = 0.75f;
                    colPrecUnit.Colspan = 1;
                    colPrecUnit.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell colImporte = new PdfPCell(new Phrase("Valor Total (Sin IGV)", _clienteFontBoldMin));
                    colImporte.BorderWidthBottom = 0.75f;
                    colImporte.BorderWidthLeft = 0;
                    colImporte.BorderWidthRight = 0.75f;
                    colImporte.BorderWidthTop = 0.75f;
                    colImporte.Colspan = 1;
                    colImporte.HorizontalAlignment = Element.ALIGN_CENTER;


                    // Añadimos las celdas a la tabla                
                    tblInfoComprobante.AddCell(colCodigo);
                    tblInfoComprobante.AddCell(colCantidad);
                    // tblInfoComprobante.AddCell(colUnidadMedida);
                    tblInfoComprobante.AddCell(colDescripcion);
                    tblInfoComprobante.AddCell(colPrecUnit);
                    tblInfoComprobante.AddCell(colImporte);


                    //impuestos globales

                    List<clasEntityDocument_TaxTotal> Lista_tax_total = new List<clasEntityDocument_TaxTotal>();
                    clasEntityDocument_TaxTotal taxTotal;


                    XmlNodeList nodestaxTotal = xmlDocument.GetElementsByTagName("TaxTotal");
                    foreach (XmlNode dat in nodestaxTotal)
                    {
                        string nodoPadre = dat.ParentNode.LocalName;
                        if (nodoPadre == "Invoice" || nodoPadre == "DebitNote" || nodoPadre == "CreditNote")
                        {
                            taxTotal = new clasEntityDocument_TaxTotal();
                            XmlDocument xmlDocumentTaxtotal = new XmlDocument();
                            xmlDocumentTaxtotal.LoadXml(dat.OuterXml);
                            XmlNodeList taxAmount = xmlDocumentTaxtotal.GetElementsByTagName("TaxAmount");
                            if (taxAmount.Count > 0)
                            {
                                taxTotal.Cs_tag_TaxAmount = taxAmount.Item(0).InnerText;
                            }
                            XmlNodeList subtotal = xmlDocumentTaxtotal.GetElementsByTagName("TaxSubtotal");
                            if (subtotal.Count > 0)
                            {
                                XmlDocument xmlDocumentTaxSubtotal = new XmlDocument();
                                xmlDocumentTaxSubtotal.LoadXml(subtotal.Item(0).OuterXml);

                                var ttta = xmlDocumentTaxSubtotal.GetElementsByTagName("TaxableAmount");
                                if (ttta.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxAmount = ttta.Item(0).InnerText;
                                }
                                //
                                var subTotalAmount = xmlDocumentTaxSubtotal.GetElementsByTagName("TaxAmount");
                                if (subTotalAmount.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxAmount = subTotalAmount.Item(0).InnerText;
                                }

                                var subTotalID = xmlDocumentTaxSubtotal.GetElementsByTagName("ID");
                                if (subTotalID.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID = subTotalID.Item(0).InnerText;
                                }


                                var subTotalName = xmlDocumentTaxSubtotal.GetElementsByTagName("Name");
                                if (subTotalName.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_Name = subTotalName.Item(0).InnerText;

                                }



                                var subTotalTaxTypeCode = xmlDocumentTaxSubtotal.GetElementsByTagName("TaxTypeCode");
                                if (subTotalTaxTypeCode.Count > 0)
                                {
                                    taxTotal.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_TaxTypeCode = subTotalTaxTypeCode.Item(0).InnerText;
                                }

                            }
                            Lista_tax_total.Add(taxTotal);

                        }
                    }

                    string ttTaxableAmount = "";
                    string imp_IGV = "";
                    string imp_ISC = "";
                    string imp_OTRO = "";

                    foreach (var ress in Lista_tax_total)
                    {

                        if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "1000")
                        {//IGV
                            imp_IGV = Convert.ToString(ress.Cs_tag_TaxAmount);
                        }
                        else if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "2000")
                        {//isc
                            imp_ISC = Convert.ToString(ress.Cs_tag_TaxAmount);

                        }
                        else if (ress.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID == "9999")
                        {
                            imp_OTRO = Convert.ToString(ress.Cs_tag_TaxAmount);

                        }

                    }

                    //Additional Monetary Total
                    List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal> Lista_additional_monetary = new List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal>();
                    List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty> Lista_additional_property = new List<clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty>();

                    //SE AGREGO - MP
                    XmlNodeList Note = xmlDocument.GetElementsByTagName("Note");
                    XmlNodeList Func = xmlDocument.GetElementsByTagName("PriceTypeCode");
                    XmlNodeList Category = xmlDocument.GetElementsByTagName("TaxCategory");

                    string Namee = string.Empty;

                    //for (int x=0; x<=Category.Count; x++)//recorre item de cate
                    //{
                    Namee = Category.Item(0).FirstChild.ChildNodes.Item(0).InnerText;  // item_.Item(0)..GetElementsByTagName("TaxScheme");
                                                                                       //}
                                                                                       //XmlNodeList Namee = Category.Item.GetElementsByTagName("TaxScheme");

                    //XmlNodeList Igvv = xmlDocument.GetElementsByTagName("");

                    foreach (XmlNode not in Note)
                    {

                        if (Note.Item(0).Attributes.Count > 0)
                        {
                            if (Note.Item(0).Attributes.Item(0).InnerText == "2006")
                            {
                                CbcNote = "2006";
                                op_detraccion = LegalMonetaryTotal.Item(0).ChildNodes.Item(5).InnerText.ToString();
                                porcentaje_detraccion = xmlDocument.GetElementsByTagName("PaymentTerms").Item(0).ChildNodes.Item(1).InnerText.ToString();
                                cuenta_nacion1 = xmlDocument.GetElementsByTagName("PaymentMeans").Item(0).ChildNodes.Item(1).InnerText.ToString();//se pone la etiqueta y se busca sus hijos
                                break;
                            }
                        }
                    }
                    //MP
                    // SE AGREGO - MP
                    //INAFECTA - GRAVADA -GRATUITA
                    //foreach (XmlNode nam in Namee)
                    //{
                    if (Namee != "")
                    {
                        //if (Namee == "9998")
                        if (InvoiceTypeCode == "01" || InvoiceTypeCode == "03" || InvoiceTypeCode == "07")
                        {
                            if (Namee == "9998")
                            {
                                ttName = "9998";
                                op_inafecta = xmlDocument.GetElementsByTagName("LegalMonetaryTotal").Item(0).FirstChild.InnerText.ToString();
                            }
                        }
                        else if (Namee == "1000" || Namee == "9998")
                        {
                            ttName = "1000";
                            if (InvoiceTypeCode == "08")
                            {
                                op_gravada = xmlDocument.GetElementsByTagName("RequestedMonetaryTotal").Item(0).FirstChild.InnerText.ToString();
                            }
                            else
                            {
                                op_gravada = xmlDocument.GetElementsByTagName("LegalMonetaryTotal").Item(0).FirstChild.InnerText.ToString();
                            }
                        }
                        else if (Namee == "9996")
                        {
                            ttName = "9996";
                            op_gratuita = xmlDocument.GetElementsByTagName("TaxableAmount").Item(0).InnerText.ToString();
                        }
                    }
                    //MP

                    //    if (Namee == "9998")
                    //    {

                    //        ttName = "9998";
                    //        op_inafecta = xmlDocument.GetElementsByTagName("LegalMonetaryTotal").Item(0).FirstChild.InnerText.ToString();

                    //    }
                    //    if (InvoiceTypeCode == "08")
                    //    {
                    //        if (Namee == "1000")
                    //        {
                    //            op_gravada = xmlDocument.GetElementsByTagName("RequestedMonetaryTotal").Item(0).FirstChild.InnerText.ToString();

                    //        }
                    //    }
                    //    else if (Namee == "1000")
                    //    {
                    //        ttName = "1000";
                    //        op_gravada = xmlDocument.GetElementsByTagName("LegalMonetaryTotal").Item(0).FirstChild.InnerText.ToString();

                    //    }

                    //    else if (Namee == "9996")
                    //    {
                    //        ttName = "9996";
                    //        op_gratuita = xmlDocument.GetElementsByTagName("TaxableAmount").Item(0).InnerText.ToString();
                    //    }
                    //}               
                    //MP

                    XmlNodeList additionalInformation = xmlDocument.GetElementsByTagName("AdditionalInformation");
                    foreach (XmlNode dat in additionalInformation)
                    {
                        XmlDocument xmlDocumentinner = new XmlDocument();
                        xmlDocumentinner.LoadXml(dat.OuterXml);
                        clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal adittionalMonetary;

                        XmlNodeList LIST1 = xmlDocumentinner.GetElementsByTagName("AdditionalMonetaryTotal");
                        for (int ii = 0; ii < LIST1.Count; ii++)
                        {
                            adittionalMonetary = new clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalMonetaryTotal();

                            var ss = LIST1.Item(ii);
                            XmlDocument xmlDocumentinner1 = new XmlDocument();
                            xmlDocumentinner1.LoadXml(ss.OuterXml);

                            var id = xmlDocumentinner1.GetElementsByTagName("ID");
                            if (id.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_Id = id.Item(0).InnerText;
                                if (id.Item(0).Attributes.Count > 0)
                                {
                                    adittionalMonetary.Cs_tag_SchemeID = id.Item(0).Attributes.GetNamedItem("schemeID").Value;
                                }
                            }

                            var percent = xmlDocumentinner1.GetElementsByTagName("Percent");
                            if (percent.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_Percent = percent.Item(0).InnerText;
                            }
                            var payableAmount = xmlDocumentinner1.GetElementsByTagName("PayableAmount");
                            if (payableAmount.Count > 0)
                            {
                                adittionalMonetary.Cs_tag_PayableAmount = payableAmount.Item(0).InnerText;
                                /*** if (payableAmount.Item(0).Attributes.Count > 0)
                                 {
                                     adittionalMonetary. = payableAmount.Item(0).Attributes.GetNamedItem("currencyID").Value;
                                 }****/
                            }

                            Lista_additional_monetary.Add(adittionalMonetary);

                        }
                        clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty adittionalProperty;
                        XmlNodeList LIST2 = xmlDocumentinner.GetElementsByTagName("AdditionalProperty");
                        for (int iii = 0; iii < LIST2.Count; iii++)
                        {
                            adittionalProperty = new clasEntityDocument_UBLExtension_ExtensionContent_AdditionalInformation_AdditionalProperty();

                            var ss = LIST2.Item(iii);
                            XmlDocument xmlDocumentinner1 = new XmlDocument();
                            xmlDocumentinner1.LoadXml(ss.OuterXml);

                            var id = xmlDocumentinner1.GetElementsByTagName("ID");
                            if (id.Count > 0)
                            {
                                adittionalProperty.Cs_tag_ID = id.Item(0).InnerText;
                            }

                            var value = xmlDocumentinner1.GetElementsByTagName("Value");
                            if (value.Count > 0)
                            {
                                adittionalProperty.Cs_tag_Value = value.Item(0).InnerText;
                            }
                            var name = xmlDocumentinner1.GetElementsByTagName("Name");
                            if (name.Count > 0)
                            {
                                adittionalProperty.Cs_tag_Name = name.Item(0).InnerText;
                            }
                            Lista_additional_property.Add(adittionalProperty);
                        }
                    }
                    //Additional

                    var cuenta_nacion = "";
                    try
                    {
                        foreach (var it in Lista_additional_property)
                        {
                            if (it.Cs_tag_ID == "3001")
                            {
                                cuenta_nacion = it.Cs_tag_Value;
                            }
                        }

                    }
                    catch (Exception)
                    {
                        cuenta_nacion = "";
                    }

                    //string op_gravada = "0.00"; //SE COMENTO
                    //string op_inafecta = "0.00"; //SE COMENTO
                    string op_exonerada = "0.00";
                    //string op_gratuita = "0.00"; //SE COMENTO
                    //string op_detraccion = "0.00"; //SE COMENTO
                    //string porcentaje_detraccion = ""; //SE COMENTO
                    string total_descuentos = "0.00";
                    string op_percepcion = "0.00";
                    string tipo_op = "0";

                    foreach (var ress in Lista_additional_monetary)
                    {
                        if (ress.Cs_tag_Id == "1001")
                        {
                            op_gravada = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1002")
                        {
                            op_inafecta = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1003")
                        {
                            op_exonerada = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "2005")
                        {
                            total_descuentos = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "1004")//1004
                        {
                            op_gratuita = Convert.ToString(ress.Cs_tag_PayableAmount);

                        }
                        else if (ress.Cs_tag_Id == "2006") //SE MODIFICO MP - verificarr 2003
                        {
                            op_detraccion = Convert.ToString(ress.Cs_tag_PayableAmount);
                            porcentaje_detraccion = Convert.ToString(ress.Cs_tag_Percent);
                        }
                        else if (ress.Cs_tag_Id == "2001")
                        {
                            op_percepcion = Convert.ToString(ress.Cs_tag_PayableAmount);
                            tipo_op = Convert.ToString(ress.Cs_tag_SchemeID);
                        }

                    }
                    /* seccion de items ------ añadir items*/
                    var numero_item = 0;
                    double sub_total = 0.00;

                    List<clasEntityDocument_Line> Lista_items;
                    List<clasEntityDocument_Line_TaxTotal> Lista_items_taxtotal;
                    clasEntityDocument_Line item;
                    XmlNodeList nodeitem;
                    if (InvoiceTypeCode == "07")
                    {
                        nodeitem = xmlDocument.GetElementsByTagName("CreditNoteLine");

                    }
                    else if (InvoiceTypeCode == "08")
                    {

                        nodeitem = xmlDocument.GetElementsByTagName("DebitNoteLine");

                    }
                    else
                    {
                        nodeitem = xmlDocument.GetElementsByTagName("InvoiceLine");
                    }
                    // XmlNodeList nodeitem = xmlDocument.GetElementsByTagName("InvoiceLine");
                    // Dictionary<string, List<clasEntityDocument_Line_Description>> dictionary = new Dictionary<string, List<clasEntityDocument_Line_Description>>();
                    List<clasEntityDocument_Line_Description> Lista_items_description;
                    List<clasEntityDocument_Line_PricingReference> Lista_items_princingreference;
                    clasEntityDocument_Line_Description descripcionItem;

                    var total_items = nodeitem.Count;

                    int i = 0;
                    foreach (XmlNode dat in nodeitem)
                    {
                        i++;
                        numero_item++;
                        var valor_unitario_item = "";
                        var valor_total_item = "";
                        string condition_price = "";
                        Lista_items = new List<clasEntityDocument_Line>();
                        Lista_items_description = new List<clasEntityDocument_Line_Description>();
                        Lista_items_princingreference = new List<clasEntityDocument_Line_PricingReference>();
                        Lista_items_taxtotal = new List<clasEntityDocument_Line_TaxTotal>();
                        item = new clasEntityDocument_Line();
                        XmlDocument xmlItem = new XmlDocument();
                        xmlItem.LoadXml(dat.OuterXml);

                        XmlNodeList ItemDetail = xmlItem.GetElementsByTagName("Item");
                        if (ItemDetail.Count > 0)
                        {
                            foreach (XmlNode items in ItemDetail)
                            {
                                XmlDocument xmlItemItem = new XmlDocument();
                                xmlItemItem.LoadXml(items.OuterXml);
                                XmlNodeList taxItemIdentification = xmlItemItem.GetElementsByTagName("ID");
                                if (taxItemIdentification.Count > 0)
                                {
                                    item.Cs_tag_Item_SellersItemIdentification = taxItemIdentification.Item(0).InnerText;
                                }
                                XmlNodeList taxItemDescription = xmlItemItem.GetElementsByTagName("Description");
                                int j = 0;
                                //descripcionItem = new clasEntityDocument_Line_Description();
                                foreach (XmlNode description in taxItemDescription)
                                {
                                    j++;
                                    descripcionItem = new clasEntityDocument_Line_Description();
                                    descripcionItem.Cs_pr_Document_Line_Id = j.ToString();
                                    /* if (description.HasChildNodes)
                                     {
                                         descripcionItem.Cs_tag_Description = description.FirstChild.InnerText.Trim();
                                     }
                                     else
                                     {*/

                                    descripcionItem.Cs_tag_Description = description.InnerText.Trim();
                                    //   }

                                    Lista_items_description.Add(descripcionItem);

                                }

                                j = 0;
                            }
                            //dictionary[i.ToString()] = Lista_items_description;
                        }

                        XmlNodeList ID = xmlItem.GetElementsByTagName("ID");
                        if (ID.Count > 0)
                        {
                            item.Cs_tag_InvoiceLine_ID = ID.Item(0).InnerText;
                        }

                        XmlNodeList InvoicedQuantity;
                        if (InvoiceTypeCode == "07")
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("CreditedQuantity");

                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }
                        }
                        else if (InvoiceTypeCode == "08")
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("DebitedQuantity");
                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }
                        }
                        else
                        {
                            InvoicedQuantity = xmlItem.GetElementsByTagName("InvoicedQuantity");
                            if (InvoicedQuantity.Count > 0)
                            {
                                item.Cs_tag_invoicedQuantity = InvoicedQuantity.Item(0).InnerText;
                                if (InvoicedQuantity.Item(0).Attributes.Count > 0)
                                {
                                    item.Cs_tag_InvoicedQuantity_unitCode = InvoicedQuantity.Item(0).Attributes.GetNamedItem("unitCode").Value;
                                }
                            }

                        }


                        XmlNodeList LineExtensionAmount = xmlItem.GetElementsByTagName("LineExtensionAmount");
                        if (LineExtensionAmount.Count > 0)
                        {
                            item.Cs_tag_LineExtensionAmount_currencyID = LineExtensionAmount.Item(0).InnerText;
                        }
                        clasEntityDocument_Line_PricingReference lines_pricing_reference;
                        XmlNodeList PricingReference = xmlItem.GetElementsByTagName("PricingReference");
                        if (PricingReference.Count > 0)
                        {
                            XmlDocument xmlItemItem = new XmlDocument();
                            xmlItemItem.LoadXml(PricingReference.Item(0).OuterXml);
                            XmlNodeList AlternativeConditionPrice = xmlItemItem.GetElementsByTagName("AlternativeConditionPrice");
                            foreach (XmlNode itm in AlternativeConditionPrice)
                            {
                                XmlDocument xmlItemPricingReference = new XmlDocument();
                                xmlItemPricingReference.LoadXml(itm.OuterXml);
                                lines_pricing_reference = new clasEntityDocument_Line_PricingReference();
                                XmlNodeList PriceAmount = xmlItemPricingReference.GetElementsByTagName("PriceAmount");
                                if (PriceAmount.Count > 0)
                                {
                                    lines_pricing_reference.Cs_tag_PriceAmount_currencyID = PriceAmount.Item(0).InnerText;
                                }
                                XmlNodeList PriceTypeCode = xmlItemPricingReference.GetElementsByTagName("PriceTypeCode");
                                if (PriceTypeCode.Count > 0)
                                {
                                    lines_pricing_reference.Cs_tag_PriceTypeCode = PriceTypeCode.Item(0).InnerText;
                                }
                                Lista_items_princingreference.Add(lines_pricing_reference);
                            }


                        }

                        clasEntityDocument_Line_TaxTotal taxTotalItem;
                        XmlNodeList TaxTotal = xmlItem.GetElementsByTagName("TaxTotal");
                        if (TaxTotal.Count > 0)
                        {
                            foreach (XmlNode taxitem in TaxTotal)
                            {
                                taxTotalItem = new clasEntityDocument_Line_TaxTotal();
                                XmlDocument xmlItemTaxtotal = new XmlDocument();
                                xmlItemTaxtotal.LoadXml(taxitem.OuterXml);
                                XmlNodeList taxItemAmount = xmlItemTaxtotal.GetElementsByTagName("TaxAmount");
                                if (taxItemAmount.Count > 0)
                                {
                                    taxTotalItem.Cs_tag_TaxAmount_currencyID = taxItemAmount.Item(0).InnerText;
                                }
                                XmlNodeList itemsubtotal = xmlItemTaxtotal.GetElementsByTagName("TaxSubtotal");
                                if (itemsubtotal.Count > 0)
                                {
                                    XmlDocument xmlItemTaxSubtotal = new XmlDocument();
                                    xmlItemTaxSubtotal.LoadXml(itemsubtotal.Item(0).OuterXml);
                                    //SE AGREGO - MP
                                    var ttta = xmlItemTaxSubtotal.GetElementsByTagName("TaxableAmount");
                                    if (ttta.Count > 0)
                                    {
                                        ttTaxableAmount = ttta.Item(0).InnerText;
                                    }
                                    //MP
                                    var subTotalAmount = xmlItemTaxSubtotal.GetElementsByTagName("TaxAmount");
                                    if (subTotalAmount.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxAmount_currencyID = subTotalAmount.Item(0).InnerText;
                                    }
                                    var subTotalID = xmlItemTaxSubtotal.GetElementsByTagName("ID");
                                    if (subTotalID.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_ID = subTotalID.Item(0).InnerText;
                                    }
                                    var subTotalName = xmlItemTaxSubtotal.GetElementsByTagName("Name");
                                    if (subTotalName.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_Name = subTotalName.Item(0).InnerText;
                                    }
                                    var subTotalTaxTypeCode = xmlItemTaxSubtotal.GetElementsByTagName("TaxTypeCode");
                                    if (subTotalTaxTypeCode.Count > 0)
                                    {
                                        taxTotalItem.Cs_tag_TaxSubtotal_TaxCategory_TaxScheme_TaxTypeCode = subTotalTaxTypeCode.Item(0).InnerText;
                                    }

                                }
                                Lista_items_taxtotal.Add(taxTotalItem);
                            }
                        }

                        XmlNodeList Price = xmlItem.GetElementsByTagName("Price");
                        if (Price.Count > 0)
                        {
                            XmlDocument xmlItemPrice = new XmlDocument();
                            xmlItemPrice.LoadXml(Price.Item(0).OuterXml);
                            XmlNodeList PriceAmount = xmlItemPrice.GetElementsByTagName("PriceAmount");
                            if (PriceAmount.Count > 0)
                            {
                                item.Cs_tag_Price_PriceAmount = PriceAmount.Item(0).InnerText;
                            }
                        }

                        if (op_gratuita != "0.00")
                        {
                            foreach (var itm in Lista_items_princingreference)
                            {
                                if (itm.Cs_tag_PriceTypeCode == "02")
                                {
                                    condition_price = itm.Cs_tag_PriceAmount_currencyID;
                                }
                            }
                        }
                        var text_detalle = "";
                        foreach (var det_it in Lista_items_description)
                        {
                            text_detalle += det_it.Cs_tag_Description + " \n";
                        }

                        //codigo del producto
                        PdfPCell itCodigo = new PdfPCell(new Phrase(item.Cs_tag_Item_SellersItemIdentification, _clienteFontContentMinFooter));
                        itCodigo.Colspan = 1;
                        if (numero_item == total_items & op_detraccion == "0.00")
                        {
                            itCodigo.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itCodigo.BorderWidthBottom = 0.75f;
                        }
                        itCodigo.BorderWidthLeft = 0.75f;
                        itCodigo.BorderWidthRight = 0.75f;
                        itCodigo.BorderWidthTop = 0;
                        itCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                        //item del producto
                        //PdfPCell itCodigo = new PdfPCell(new Phrase(numero_item.ToString(), _clienteFontContentMinFooter));
                        //itCodigo.Colspan = 1;
                        //if (numero_item == total_items & op_detraccion == "0.00")
                        //{
                        //    itCodigo.BorderWidthBottom = 0.75f;

                        //}
                        //else
                        //{
                        //    itCodigo.BorderWidthBottom = 0.75f;
                        //}
                        //itCodigo.BorderWidthLeft = 0.75f;
                        //itCodigo.BorderWidthRight = 0.75f;
                        //itCodigo.BorderWidthTop = 0;
                        //itCodigo.HorizontalAlignment = Element.ALIGN_CENTER;

                        PdfPCell itCantidad = new PdfPCell(new Phrase(item.Cs_tag_invoicedQuantity, _clienteFontContentMinFooter));
                        itCantidad.Colspan = 1;
                        if (numero_item == total_items & op_detraccion == "0.00")
                        {
                            itCantidad.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itCantidad.BorderWidthBottom = 0.75f;
                        }

                        itCantidad.BorderWidthLeft = 0;
                        itCantidad.BorderWidthRight = 0.75f;
                        itCantidad.BorderWidthTop = 0;
                        itCantidad.HorizontalAlignment = Element.ALIGN_CENTER;

                        /* PdfPCell itUnidadMedida = new PdfPCell(new Phrase(item.Cs_tag_InvoicedQuantity_unitCode, _clienteFontContentMinFooter));
                         itUnidadMedida.Colspan = 1;
                         if (numero_item == total_items & op_detraccion == "0.00")
                         {
                             itUnidadMedida.BorderWidthBottom = 0.75f;

                         }
                         else
                         {
                             itUnidadMedida.BorderWidthBottom = 0.75f;
                         }

                         itUnidadMedida.BorderWidthLeft = 0;
                         itUnidadMedida.BorderWidthRight = 0.75f;
                         itUnidadMedida.BorderWidthTop = 0;
                         itUnidadMedida.HorizontalAlignment = Element.ALIGN_CENTER;*/

                        PdfPCell itDescripcion = new PdfPCell(new Phrase(text_detalle, _clienteFontContentMinFooter));
                        itDescripcion.Colspan = 7;
                        if (numero_item == total_items & op_detraccion == "0.00")
                        {
                            itDescripcion.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itDescripcion.BorderWidthBottom = 0.75f;
                        }

                        itDescripcion.BorderWidthLeft = 0;
                        itDescripcion.BorderWidthRight = 0.75f;
                        itDescripcion.BorderWidthTop = 0;
                        itDescripcion.PaddingBottom = 5f;
                        itDescripcion.HorizontalAlignment = Element.ALIGN_LEFT;

                        if (op_gratuita != "0.00")
                        {
                            valor_unitario_item = condition_price;
                        }
                        else
                        {
                            valor_unitario_item = item.Cs_tag_Price_PriceAmount;
                        }

                        PdfPCell itPrecUnit = new PdfPCell(new Phrase(double.Parse(valor_unitario_item, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContentMinFooter));
                        itPrecUnit.Colspan = 1;
                        if (numero_item == total_items & op_detraccion == "0.00")
                        {
                            itPrecUnit.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itPrecUnit.BorderWidthBottom = 0.75f;
                        }

                        itPrecUnit.BorderWidthLeft = 0;
                        itPrecUnit.BorderWidthRight = 0.75f;
                        itPrecUnit.BorderWidthTop = 0;
                        itPrecUnit.HorizontalAlignment = Element.ALIGN_CENTER;


                        if (op_gratuita != "0.00")
                        {
                            if (valor_unitario_item == "")
                            {
                                valor_unitario_item = "0.00";
                            }
                            double valor_total_item_1 = double.Parse(valor_unitario_item, CultureInfo.InvariantCulture) * double.Parse(item.Cs_tag_invoicedQuantity, CultureInfo.InvariantCulture);
                            valor_total_item = valor_total_item_1.ToString();
                        }
                        else
                        {
                            valor_total_item = item.Cs_tag_LineExtensionAmount_currencyID;
                        }
                        PdfPCell itImporte = new PdfPCell(new Phrase(double.Parse(valor_total_item, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContentMinFooter));
                        itImporte.Colspan = 1;
                        if (numero_item == total_items & op_detraccion == "0.00")
                        {
                            itImporte.BorderWidthBottom = 0.75f;

                        }
                        else
                        {
                            itImporte.BorderWidthBottom = 0.75f;
                        }

                        itImporte.BorderWidthLeft = 0;
                        itImporte.BorderWidthRight = 0.75f;
                        itImporte.BorderWidthTop = 0;
                        itImporte.HorizontalAlignment = Element.ALIGN_CENTER;

                        //sub_total += Double.Parse(item.Cs_tag_LineExtensionAmount_currencyID);
                        // sub_total += double.Parse(item.Cs_tag_LineExtensionAmount_currencyID, CultureInfo.InvariantCulture);
                        // Añadimos las celdas a la tabla
                        tblInfoComprobante.AddCell(itCodigo);
                        tblInfoComprobante.AddCell(itCantidad);
                        // tblInfoComprobante.AddCell(itUnidadMedida);
                        tblInfoComprobante.AddCell(itDescripcion);
                        tblInfoComprobante.AddCell(itPrecUnit);
                        tblInfoComprobante.AddCell(itImporte);
                    }


                    if (op_detraccion != "0.00")
                    {
                        //agregar mensaje

                        PdfPCell celda_blanco = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco.Colspan = 1;
                        celda_blanco.BorderWidthBottom = 0.75f;
                        celda_blanco.BorderWidthLeft = 0;
                        celda_blanco.BorderWidthRight = 0.75f;
                        celda_blanco.BorderWidthTop = 0;

                        PdfPCell celda_blanco_right = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco_right.Colspan = 1;
                        celda_blanco_right.BorderWidthBottom = 0.75f;
                        celda_blanco_right.BorderWidthLeft = 0;
                        celda_blanco_right.BorderWidthRight = 0.75f;
                        celda_blanco_right.BorderWidthTop = 0;

                        PdfPCell celda_blanco_left = new PdfPCell(new Phrase(" ", _clienteFontContent));
                        celda_blanco_left.Colspan = 1;
                        celda_blanco_left.BorderWidthBottom = 0.75f;
                        celda_blanco_left.BorderWidthLeft = 0.75f;
                        celda_blanco_left.BorderWidthRight = 0.75f;
                        celda_blanco_left.BorderWidthTop = 0;

                        var parrafo = new Paragraph();
                        parrafo.Font = _clienteFontContentMinFooter;
                        //if (valor_operacion == "2006") //
                        if (CbcNote == "2006")
                        {
                            parrafo.Add("Operación sujeta al Sistema de Pago de Obligaciones Tributarias con el Gobierno Central \n");
                            parrafo.Add("SPOT " + porcentaje_detraccion + "% " + cuenta_nacion1 + " \n");
                        }

                        PdfPCell celda_parrafo = new PdfPCell(parrafo);
                        celda_parrafo.Colspan = 7;
                        celda_parrafo.BorderWidthBottom = 0.75f;
                        celda_parrafo.BorderWidthLeft = 0;
                        celda_parrafo.BorderWidthRight = 0.75f;
                        celda_parrafo.BorderWidthTop = 0;
                        celda_parrafo.PaddingTop = 10f;
                        celda_parrafo.HorizontalAlignment = Element.ALIGN_CENTER;

                        tblInfoComprobante.AddCell(celda_blanco_left);
                        tblInfoComprobante.AddCell(celda_blanco);
                        //tblInfoComprobante.AddCell(celda_blanco);
                        tblInfoComprobante.AddCell(celda_parrafo);
                        tblInfoComprobante.AddCell(celda_blanco);
                        tblInfoComprobante.AddCell(celda_blanco_right);

                    }
                    /* ------end items------*/
                    doc.Add(tblInfoComprobante);
                    doc.Add(tblBlanco);



                    if (InvoiceTypeCode == "03" | InvoiceTypeCode == "07" | InvoiceTypeCode == "08")
                    {
                        PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                        tblInfoOperacionesGratuitas.WidthPercentage = 100;

                        PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase(" ", _clienteFontContentMinFooter));
                        infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                        infoTotalOpGratuitas.BorderWidthRight = 0;
                        infoTotalOpGratuitas.Colspan = 5;
                        infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                        PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontBoldMin));
                        infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                        infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                        infoTotalOpGratuitasLabel.Colspan = 3;
                        infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_RIGHT;

                        var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                        PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                        infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                        infoTotalOpGratuitasVal.Colspan = 2;
                        infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                        tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                        doc.Add(tblInfoOperacionesGratuitas);

                        doc.Add(tblBlanco);
                        if (InvoiceTypeCode == "03")
                        {
                            /*----------- Monto total en letras --------------*/
                            NumLetra totalLetras = new NumLetra();
                            PdfPTable tblInfoMontoTotal = new PdfPTable(10);

                            tblInfoMontoTotal.WidthPercentage = 100;

                            PdfPCell infoTotal = new PdfPCell(new Phrase("SON: " + totalLetras.Convertir(LMTPayableAmount, true, DocumentCurrencyCode), _clienteFontContent));
                            infoTotal.BorderWidth = 0.75f;
                            infoTotal.Colspan = 7;
                            infoTotal.HorizontalAlignment = Element.ALIGN_LEFT;

                            tblInfoMontoTotal.AddCell(infoTotal);


                            PdfPTable tbl_monto_total1 = new PdfPTable(2);
                            tbl_monto_total1.WidthPercentage = 100;


                            var monedaDatos2 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell labelMontoTotal1 = new PdfPCell(new Phrase("IMPORTE TOTAL:", _clienteFontBold));
                            labelMontoTotal1.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell valueMontoTotal1 = new PdfPCell(new Phrase(monedaDatos2.CurrencySymbol + " " + double.Parse(LMTPayableAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            valueMontoTotal1.HorizontalAlignment = Element.ALIGN_RIGHT;

                            tbl_monto_total1.AddCell(labelMontoTotal1);
                            tbl_monto_total1.AddCell(valueMontoTotal1);

                            PdfPCell contenedor = new PdfPCell(tbl_monto_total1);
                            contenedor.Colspan = 3;
                            contenedor.Border = 0;
                            contenedor.PaddingLeft = 10f;
                            tblInfoMontoTotal.AddCell(contenedor);
                            doc.Add(tblInfoMontoTotal);
                            /*-------------End Monto Total----------------*/
                            doc.Add(tblBlanco);
                        }


                    }
                    else
                    {

                        if (op_gratuita != "0.00")
                        {
                            /*Monto de Transferencia Gratuita*/

                            PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                            tblInfoOperacionesGratuitas.WidthPercentage = 100;

                            PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase("TRANSFERENCIA GRATUITA DE UN BIEN Y/O SERVICIO PRESTADO GRATUITAMENTE", _clienteFontContentMinFooter));
                            infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                            infoTotalOpGratuitas.BorderWidthRight = 0;
                            infoTotalOpGratuitas.Colspan = 6;
                            infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                            PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontContentMinFooter));
                            infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                            infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                            infoTotalOpGratuitasLabel.Colspan = 3;
                            infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_CENTER;

                            var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                            infoTotalOpGratuitasVal.Colspan = 1;
                            infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                            doc.Add(tblInfoOperacionesGratuitas);

                            doc.Add(tblBlanco);
                        }
                        else
                        {


                            PdfPTable tblInfoOperacionesGratuitas = new PdfPTable(10);
                            tblInfoOperacionesGratuitas.WidthPercentage = 100;

                            PdfPCell infoTotalOpGratuitas = new PdfPCell(new Phrase(" ", _clienteFontContentMinFooter));
                            infoTotalOpGratuitas.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitas.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitas.BorderWidthLeft = 0.75f;
                            infoTotalOpGratuitas.BorderWidthRight = 0;
                            infoTotalOpGratuitas.Colspan = 5;
                            infoTotalOpGratuitas.HorizontalAlignment = Element.ALIGN_LEFT;

                            PdfPCell infoTotalOpGratuitasLabel = new PdfPCell(new Phrase("Valor de venta de operaciones gratuitas", _clienteFontBoldMin));
                            infoTotalOpGratuitasLabel.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasLabel.BorderWidthLeft = 0;
                            infoTotalOpGratuitasLabel.BorderWidthRight = 0;
                            infoTotalOpGratuitasLabel.Colspan = 3;
                            infoTotalOpGratuitasLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
                            //if (ttName == "9996")
                            //{
                            var monedaDatos1 = GetCurrencySymbol(DocumentCurrencyCode);
                            PdfPCell infoTotalOpGratuitasVal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + double.Parse(op_gratuita /*ttTaxableAmount*/, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            infoTotalOpGratuitasVal.BorderWidthTop = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthBottom = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthRight = 0.75f;
                            infoTotalOpGratuitasVal.BorderWidthLeft = 0;
                            infoTotalOpGratuitasVal.Colspan = 2;
                            infoTotalOpGratuitasVal.HorizontalAlignment = Element.ALIGN_RIGHT;


                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitas);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasLabel);
                            tblInfoOperacionesGratuitas.AddCell(infoTotalOpGratuitasVal);
                            doc.Add(tblInfoOperacionesGratuitas);
                            doc.Add(tblBlanco);
                            //}
                        }
                    }



                    /*----------- CASO BOLETA SOLO MONTO TOTAL --------------*/
                    if (InvoiceTypeCode == "03")
                    {
                        /*  PdfPTable tblMontoTotal = new PdfPTable(10);
                          tblMontoTotal.WidthPercentage = 100;

                          PdfPCell monto_blanco = new PdfPCell(new Phrase(" ", _clienteFontContent));
                          monto_blanco.Border = 0;
                          monto_blanco.Colspan = 6;
                          tblMontoTotal.AddCell(monto_blanco);

                          PdfPTable tbl_monto_total = new PdfPTable(2);
                          tbl_monto_total.WidthPercentage = 100;
                          var monedaDatos1 = GetCurrencySymbol(cabecera.Cs_tag_DocumentCurrencyCode);
                          PdfPCell labelMontoTotal = new PdfPCell(new Phrase("IMPORTE TOTAL:", _clienteFontBold));
                          labelMontoTotal.HorizontalAlignment = Element.ALIGN_LEFT;
                          PdfPCell valueMontoTotal = new PdfPCell(new Phrase(monedaDatos1.CurrencySymbol + " " + cabecera.Cs_tag_LegalMonetaryTotal_PayableAmount_currencyID, _clienteFontContent));
                          valueMontoTotal.HorizontalAlignment = Element.ALIGN_RIGHT;

                          tbl_monto_total.AddCell(labelMontoTotal);
                          tbl_monto_total.AddCell(valueMontoTotal);

                          PdfPCell monto_total = new PdfPCell(tbl_monto_total);
                          monto_total.Border = 0;
                          monto_total.Colspan = 4;
                          tblMontoTotal.AddCell(monto_total);

                          doc.Add(tblMontoTotal);*/
                    }
                    /*-------------End Monto Total----------------*/

                    //FOOTER
                    PdfPTable tblInfoFooter = new PdfPTable(10);
                    tblInfoFooter.WidthPercentage = 100;

                    //comentarios
                    PdfPTable tblInfoComentarios = new PdfPTable(1);
                    tblInfoComentarios.WidthPercentage = 100;

                    //tblInfoComentarios.TotalWidth = 144f;
                    //tblInfoComentarios.LockedWidth = true;

                    //SE COMENTO - MP
                    PdfPCell tituComentarios = new PdfPCell(new Phrase("Observaciones:", _clienteFontBold));
                    tituComentarios.Border = 0;
                    tituComentarios.HorizontalAlignment = Element.ALIGN_LEFT;
                    tituComentarios.PaddingBottom = 5f;
                    if (InvoiceTypeCode == "03")
                    {
                        //cuando es boleta
                        tituComentarios.PaddingTop = -15f;
                    }
                    else
                    {
                        tituComentarios.PaddingTop = -5f;
                    }

                    tblInfoComentarios.AddCell(tituComentarios);



                    var comentarios_string = teclaf8 + " " + teclavtrlm;

                    PdfPCell contComentarios = new PdfPCell(new Phrase(teclavtrlm, _clienteFontContentMinFooter));//se llama la observacion: teclavtrlm
                    contComentarios.BorderWidth = 0.75f;
                    contComentarios.PaddingBottom = 5f;
                    contComentarios.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tblInfoComentarios.AddCell(contComentarios);
                    //MP

                    /* if (cabecera.Cs_tag_InvoiceTypeCode != "03")
                     {*/

                    //SE COMENTO - MP
                    PdfPCell tituDatos = new PdfPCell(new Phrase("DATOS:", _clienteFontBold));
                    tituDatos.Border = 0;
                    tituDatos.HorizontalAlignment = Element.ALIGN_LEFT;
                    tituDatos.PaddingBottom = 5f;
                    tblInfoComentarios.AddCell(tituDatos);
                    //MP

                    //SE AGREGO PARA ESCRIBIR EN DURO OTRAS CUENTAS
                    //var td1 = new Paragraph();
                    //td1.Font = _clienteFontBoldMin;
                    //td1.Add("");
                    //td1.Add("Cuentas 1: 0125634896 \n");
                    //td1.Add("Cuentas 2: 0125634896 \n");
                    //td1.Add("Cuentas 3: 0125634896 \n");
                    //pa.Add(sucursal);


                    //PdfPCell tituDatos1 = new PdfPCell(td1);
                    //tituDatos1.Border = 0;
                    //tituDatos1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //tituDatos1.PaddingBottom = 5f;
                    //tblInfoComentarios.AddCell(tituDatos1);
                    //MPMP                            

                    // SE COMENTO : cuentas y orden - inferior izquierda
                    /* TABLA PARA NRO ORDEN PEDIDO Y CUENTAS BANCARIAS*/
                    PdfPTable tblOrdenCuenta = new PdfPTable(11);
                    tblOrdenCuenta.WidthPercentage = 100;
                    PdfPCell labelOrden = new PdfPCell(new Phrase("", _clienteFontBoldContentMinFooter));
                    labelOrden.Colspan = 2;
                    labelOrden.Border = 0;
                    labelOrden.HorizontalAlignment = Element.ALIGN_LEFT;
                    PdfPCell valueOrden = new PdfPCell(new Phrase("", _clienteFontContent));
                    valueOrden.Colspan = 9;
                    valueOrden.Border = 0;
                    valueOrden.HorizontalAlignment = Element.ALIGN_LEFT;
                    tblOrdenCuenta.AddCell(labelOrden);
                    tblOrdenCuenta.AddCell(valueOrden);
                    ////MP

                    //SE COMENTO - MP
                    PdfPCell labelCuentas = new PdfPCell(new Phrase("Ctas Bancarias:", _clienteFontBoldContentMinFooter));
                    labelCuentas.Colspan = 2;
                    labelCuentas.Border = 0;
                    labelCuentas.HorizontalAlignment = Element.ALIGN_LEFT;


                    var pdat = new Paragraph();
                    pdat.Font = _clienteFontContentMinFooter;
                    pdat.Add(cuentasbancarias);//se llama las cuentas
                    PdfPCell valueCuentas = new PdfPCell(pdat);
                    valueCuentas.Colspan = 9;
                    valueCuentas.Border = 0;
                    valueCuentas.HorizontalAlignment = Element.ALIGN_LEFT;

                    tblOrdenCuenta.AddCell(labelCuentas);
                    tblOrdenCuenta.AddCell(valueCuentas);

                    tblInfoComentarios.AddCell(tblOrdenCuenta);
                    //MP

                    PdfPCell cellBlanco = new PdfPCell(new Phrase("", _clienteFontBoldContentMinFooter));
                    cellBlanco.Border = 0;

                    tblInfoComentarios.AddCell(cellBlanco);
                    // }
                    /*PdfPCell contDatos = new PdfPCell(pdat);
                    contDatos.BorderWidth = 0.75f;
                    contDatos.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    tblInfoComentarios.AddCell(contDatos);
                    */

                    //resumen 
                    PdfPTable tblInfoResumen = new PdfPTable(4);
                    tblInfoResumen.WidthPercentage = 100;

                    //tblInfoResumen.TotalWidth = 144f;
                    //tblInfoResumen.LockedWidth = true;
                    sub_total += double.Parse(op_gravada, CultureInfo.InvariantCulture);

                    if (InvoiceTypeCode != "03")
                    {
                        // moneda

                        var monedaDatos = GetCurrencySymbol(DocumentCurrencyCode);
                        string output_subtotal = "";


                        if (op_gratuita == "0.00")
                        {
                            output_subtotal = sub_total.ToString("#,0.00", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            output_subtotal = "0.00";
                        }

                        PdfPCell resItem6 = new PdfPCell(new Phrase("Sub Total", _clienteFontBold));
                        resItem6.Colspan = 2;
                        resItem6.HorizontalAlignment = Element.ALIGN_LEFT;
                        //PdfPCell resvalue6 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + LMTLineExtensionAmount/*ttTaxableAmount*//*LMTTaxInclusiveAmount*/, /*output_subtotal*/ _clienteFontContent));//Se podria dar valor a output_subtotal para usarlo asi como op_gravada //sin decimal-MP
                        PdfPCell resvalue6 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(LMTLineExtensionAmount).ToString("#,0.00", CultureInfo.InvariantCulture)/*ttTaxableAmount*//*LMTTaxInclusiveAmount*/, /*output_subtotal*/ _clienteFontContent));//Se podria dar valor a output_subtotal para usarlo asi como op_gravada//con decimal 
                        resvalue6.Colspan = 2;
                        resvalue6.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem6);
                        tblInfoResumen.AddCell(resvalue6);

                        PdfPCell resItem7 = new PdfPCell(new Phrase("Otros Cargos", _clienteFontBold));
                        resItem7.Colspan = 2;
                        resItem7.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue7 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(LMTChargeTotalAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue7.Colspan = 2;
                        resvalue7.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem7);
                        tblInfoResumen.AddCell(resvalue7);

                        PdfPCell resItem8 = new PdfPCell(new Phrase("Descuento Global", _clienteFontBold));
                        resItem8.Colspan = 2;
                        resItem8.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue8 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(total_descuentos, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue8.Colspan = 2;
                        resvalue8.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem8);
                        tblInfoResumen.AddCell(resvalue8);


                        PdfPCell resItem1 = new PdfPCell(new Phrase("Operaciones Gravadas", _clienteFontBold));
                        resItem1.Colspan = 2;
                        resItem1.HorizontalAlignment = Element.ALIGN_LEFT;
                        //mp
                        //if (ttName == "1000")
                        //{
                        PdfPCell resvalue1 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_gravada /*LMTLineExtensionAmount*/, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));//por el momento se puso LMTLineExtensionAmount
                        resvalue1.Colspan = 2;
                        resvalue1.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem1);
                        tblInfoResumen.AddCell(resvalue1);
                        //}
                        //mp
                        //mp
                        //if (ttName == "9998") 
                        //{
                        PdfPCell resItem2 = new PdfPCell(new Phrase("Operaciones Inafectas", _clienteFontBold));
                        resItem2.Colspan = 2;
                        resItem2.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue2 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_inafecta /*LMTLineExtensionAmount*/, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue2.Colspan = 2;
                        resvalue2.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem2);
                        tblInfoResumen.AddCell(resvalue2);
                        //}
                        //mp
                        PdfPCell resItem3 = new PdfPCell(new Phrase("Operaciones Exoneradas", _clienteFontBold));
                        resItem3.Colspan = 2;
                        resItem3.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue3 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_exonerada, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue3.Colspan = 2;
                        resvalue3.HorizontalAlignment = Element.ALIGN_RIGHT;

                        tblInfoResumen.AddCell(resItem3);
                        tblInfoResumen.AddCell(resvalue3);

                        if (imp_IGV != "")
                        {
                            PdfPCell resItem4_1 = new PdfPCell(new Phrase("IGV", _clienteFontBold));
                            resItem4_1.Colspan = 2;
                            resItem4_1.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_1 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(imp_IGV, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue4_1.Colspan = 2;
                            resvalue4_1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_1);
                            tblInfoResumen.AddCell(resvalue4_1);
                        }
                        /*if (imp_ISC != "")
                        {
                            PdfPCell resItem4_2 = new PdfPCell(new Phrase("ISC", _clienteFontBold));
                            resItem4_2.Colspan = 2;
                            resItem4_2.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_2 = new PdfPCell(new Phrase(imp_ISC, _clienteFontContent));
                            resvalue4_2.Colspan = 2;
                            resvalue4_2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_2);
                            tblInfoResumen.AddCell(resvalue4_2);
                        }
                        if (imp_OTRO != "")
                        {
                            PdfPCell resItem4_3 = new PdfPCell(new Phrase("Otros tributos", _clienteFontBold));
                            resItem4_3.Colspan = 2;
                            resItem4_3.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue4_3 = new PdfPCell(new Phrase(imp_OTRO, _clienteFontContent));
                            resvalue4_3.Colspan = 2;
                            resvalue4_3.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem4_3);
                            tblInfoResumen.AddCell(resvalue4_3);
                        }*/
                        string importeString = "IMPORTE TOTAL:";
                        if (op_percepcion != "0.00")
                        {
                            importeString = "TOTAL:";
                        }
                        else
                        {
                            importeString = "IMPORTE TOTAL:";

                        }

                        PdfPCell resItem5 = new PdfPCell(new Phrase(importeString, _clienteFontBold));
                        resItem5.Colspan = 2;
                        resItem5.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue5 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(LMTPayableAmount, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                        resvalue5.Colspan = 2;
                        resvalue5.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tblInfoResumen.AddCell(resItem5);
                        tblInfoResumen.AddCell(resvalue5);

                        if (op_percepcion != "0.00")
                        {
                            PdfPCell resItem51 = new PdfPCell(new Phrase("PERCEPCION:", _clienteFontBold));
                            resItem51.Colspan = 2;
                            resItem51.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue51 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + double.Parse(op_percepcion, CultureInfo.InvariantCulture).ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue51.Colspan = 2;
                            resvalue51.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem51);
                            tblInfoResumen.AddCell(resvalue51);

                            double new_total = Convert.ToDouble(LMTPayableAmount, CultureInfo.CreateSpecificCulture("en-US")) + Convert.ToDouble(op_percepcion, CultureInfo.CreateSpecificCulture("en-US"));

                            PdfPCell resItem52 = new PdfPCell(new Phrase("TOTAL VENTA:", _clienteFontBold));
                            resItem52.Colspan = 2;
                            resItem52.HorizontalAlignment = Element.ALIGN_LEFT;
                            PdfPCell resvalue52 = new PdfPCell(new Phrase(monedaDatos.CurrencySymbol + " " + new_total.ToString("#,0.00", CultureInfo.InvariantCulture), _clienteFontContent));
                            resvalue52.Colspan = 2;
                            resvalue52.HorizontalAlignment = Element.ALIGN_RIGHT;
                            tblInfoResumen.AddCell(resItem52);
                            tblInfoResumen.AddCell(resvalue52);


                        }



                        PdfPCell resItem9 = new PdfPCell(new Phrase("", _clienteFontBold));
                        resItem9.Colspan = 2;
                        resItem9.Border = 0;
                        resItem9.PaddingBottom = 0f;
                        resItem9.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell resvalue9 = new PdfPCell(new Phrase("", _clienteFontContent));
                        resvalue9.Colspan = 2;
                        resvalue9.Border = 0;
                        resvalue9.PaddingBottom = 0f;
                        resvalue9.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tblInfoResumen.AddCell(resItem9);
                        tblInfoResumen.AddCell(resvalue9);


                    }
                    //lado izquierdo
                    PdfPCell tblInfoFooterLeft = new PdfPCell(tblInfoComentarios);
                    if (InvoiceTypeCode != "03")
                    {
                        tblInfoFooterLeft.Colspan = 6;
                        tblInfoFooterLeft.PaddingRight = 10f;
                    }
                    else
                    {
                        tblInfoFooterLeft.Colspan = 10;
                        tblInfoFooterLeft.PaddingRight = 0;
                    }

                    tblInfoFooterLeft.Border = 0;

                    tblInfoFooter.AddCell(tblInfoFooterLeft);
                    //lado derecho

                    PdfPCell tblInfoFooterRight = new PdfPCell(tblInfoResumen);
                    tblInfoFooterRight.Colspan = 4;
                    tblInfoFooterRight.Border = 0;
                    tblInfoFooter.AddCell(tblInfoFooterRight);


                    doc.Add(tblInfoFooter);
                    doc.Add(tblBlanco);
                    if (InvoiceTypeCode != "03")
                    {
                        /*----------- Monto total en letras --------------*/
                        NumLetra totalLetras = new NumLetra();
                        PdfPTable tblInfoMontoTotal = new PdfPTable(1);
                        tblInfoMontoTotal.WidthPercentage = 100;
                        PdfPCell infoTotal = new PdfPCell(new Phrase("SON: " + totalLetras.Convertir(LMTPayableAmount, true, DocumentCurrencyCode), _clienteFontContent));
                        infoTotal.BorderWidth = 0.75f;
                        infoTotal.HorizontalAlignment = Element.ALIGN_LEFT;
                        tblInfoMontoTotal.AddCell(infoTotal);
                        doc.Add(tblInfoMontoTotal);
                        /*-------------End Monto Total----------------*/
                        doc.Add(tblBlanco);
                    }

                    PdfPTable tblFooter = new PdfPTable(10);
                    tblFooter.WidthPercentage = 100;
                    tblFooter.SpacingBefore = 5;

                    var p = new Paragraph();
                    p.Font = _clienteFontBold;
                    if (op_percepcion != "0.00")
                    {
                        string tipoOperacion = Documento.getTipoOperacion(tipo_op);
                        p.Add("Incorporado al regimen de agentes de Percepcion de IGV - " + tipoOperacion + " (D.S 091-2013) 01/02/2014 \n\n");
                    }
                    p.Add(digestValue + "\n\n");
                    p.Add(info_general.TextoRepresentacionImpresa);
                    p.Add("Puede consultar su comprobante en cpecontasiscorp.com/ConsultaLPFServiciosIntegrales/ \n");

                    PdfPCell DataHash = new PdfPCell(new Phrase(digestValue, _clienteFontBold));
                    DataHash.Border = 0;
                    DataHash.Colspan = 6;
                    DataHash.HorizontalAlignment = Element.ALIGN_CENTER;
                    // DataHash.PaddingTop = 5f;                

                    PdfPCell campo1 = new PdfPCell(p);
                    campo1.Colspan = 6;
                    campo1.Border = 0;
                    campo1.PaddingTop = 0f;
                    campo1.HorizontalAlignment = Element.ALIGN_CENTER;

                    //codigo de barras                               
                    //var hash = new clsNegocioXML();
                    //var hash_obtenido=hash.cs_fxHash(cabecera.Cs_pr_Document_Id);

                    Dictionary<EncodeHintType, object> ob = new Dictionary<EncodeHintType, object>() {
                                {EncodeHintType.ERROR_CORRECTION,ErrorCorrectionLevel.Q }
                            };

                    //yoooooooooooooooooo
                    var textQR =/* ASPCustomerAssignedAccountID + " | " +*/ ASPPartyIdentification + " | " + InvoiceTypeCode + " | " + doc_serie + "-" + doc_correlativo + " | " + imp_IGV + " | " + LMTPayableAmount + " | " + IssueDate + " | " + ACPAdditionalAccountID + " | " + ACPCustomerAssignedAccountID + " |";

                    BarcodeQRCode qrcode = new BarcodeQRCode(textQR, 400, 400, ob);

                    iTextSharp.text.Image qrcodeImage = qrcode.GetImage();

                    /* BarcodePDF417 barcod = new BarcodePDF417();
                     barcod.SetText(cabecera.Cs_tag_AccountingSupplierParty_CustomerAssignedAccountID+" | "+ cabecera.Cs_tag_InvoiceTypeCode+" | "+ doc_serie+" | "+doc_correlativo+" | "+ impuestos_globales.Cs_tag_TaxSubtotal_TaxAmount+" | "+ cabecera.Cs_tag_LegalMonetaryTotal_PayableAmount_currencyID+" | "+ cabecera.Cs_tag_IssueDate+" | "+cabecera.Cs_tag_AccountingCustomerParty_AdditionalAccountID+" | "+cabecera.Cs_tag_AccountingCustomerParty_CustomerAssignedAccountID+" | "+ digestValue + " | "+signatureValue+" |");
                     barcod.ErrorLevel = 5;
                     barcod.Options = BarcodePDF417.PDF417_FORCE_BINARY;

                     iTextSharp.text.Image imagePDF417 = barcod.GetImage();*/
                    //qrcodeImage.ScaleAbsolute(100f, 90f);
                    PdfPCell blanco12 = new PdfPCell();
                    // blanco12.Image = qrcodeImage;
                    blanco12.AddElement(new Chunk(qrcodeImage, 55f, -65f));
                    blanco12.Border = 0;
                    blanco12.PaddingTop = 15f;
                    blanco12.Colspan = 4;


                    PdfPCell blanco121 = new PdfPCell(new Paragraph(" "));
                    blanco121.Border = 0;
                    blanco121.Colspan = 4;

                    tblFooter.AddCell(campo1);
                    tblFooter.AddCell(blanco12);
                    //tblFooter.AddCell(campo1);
                    // tblFooter.AddCell(blanco121);

                    doc.Add(tblFooter);


                    doc.Close();
                    File.SetAttributes(newFile, FileAttributes.Normal);
                    writer.Close();

                    url = newFileServer;
                    rutas[0] = newFile;
                    rutas[1] = newxml;
                    rutas[2] = newFileServer;
                    rutas[3] = newXmlServer;
                }
            }////aca
        }
        catch (Exception es)
        {
            url = es.Message;

            string ruta_error = System.AppDomain.CurrentDomain.BaseDirectory + "Info\\log_email.log";
            bool flag = !File.Exists(ruta_error);
            if (flag)
            {
                File.Create(ruta_error).Close();
            }
            TextWriter tw = new StreamWriter(ruta_error, true);
            tw.WriteLine("<>" + DateTime.Now.ToString() + ": Intervalo: " + url);
            tw.Close();
        }


        return rutas;
    }
    public static Documento getByTipo(string type)
    {
        var tipo = "";
        var imagen = "";
        var nombre = "";
        var texto = "";
        if (type == "01")
        {
            //factura
            tipo = "01";
            imagen = "~/images/logo_min.jpg";
            nombre = "FACTURA ELECTRONICA";
            texto = "Representacion impresa de la Factura electronica \n";
        }
        else if (type == "03")
        {   //Boleta
            tipo = "03";
            imagen = "~/images/logo_min.jpg";
            nombre = "Boleta de Venta Electronica";
            texto = "Representacion impresa de la Boleta de Venta Electronica \n";
        }
        else if (type == "07")
        {   //Boleta
            tipo = "07";
            imagen = "~/images/logo_min.jpg";
            nombre = "Nota de Credito Electronica";
            texto = "Representacion impresa de la Nota de Credito Electronica \n";
        }
        else if (type == "08")
        {   //Boleta
            tipo = "08";
            imagen = "~/images/logo_min.jpg";
            nombre = "Nota de Debito Electronica";
            texto = "Representacion impresa de la Nota de Debito Electronica \n";
        }

        Documento doc = new Documento(tipo, imagen, nombre, texto);
        return doc;
    }

    public static System.Globalization.RegionInfo GetCurrencySymbol(string code)
    {
        System.Globalization.RegionInfo regionInfo = (from culture in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.InstalledWin32Cultures)
                                                      where culture.Name.Length > 0 && !culture.IsNeutralCulture
                                                      let region = new System.Globalization.RegionInfo(culture.LCID)
                                                      where String.Equals(region.ISOCurrencySymbol, code, StringComparison.InvariantCultureIgnoreCase)
                                                      select region).First();

        return regionInfo;
    }
    public static string getTipoDocIdentidad(string codigo)
    {
        string documento = "";
        switch (codigo)
        {
            case "0":
                documento = "DOC TRIB NO DOM SIN RUC";
                break;
            case "1":
                documento = "DNI";
                break;
            case "4":
                documento = "Carnet de Extranjeria";
                break;
            case "6":
                documento = "RUC";
                break;
            case "7":
                documento = "Pasaporte";
                break;
            default:
                documento = "No definido";
                break;
        }
        return documento;
    }
    public static string getTipoOperacion(string codigo)
    {
        string documento = "";
        switch (codigo)
        {
            case "01":
                documento = "Venta Interna";
                break;
            case "02":
                documento = "Exportacion";
                break;
            case "03":
                documento = "No domiciliados";
                break;
            case "04":
                documento = "Venta Interna - Anticipos";
                break;
            case "05":
                documento = "Venta Itinerante";
                break;
            default:
                documento = "";
                break;
        }
        return documento;
    }
}