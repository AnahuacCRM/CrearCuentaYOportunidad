using Anahuac.CRM.EnviaOportunidadABanner.CRM;
using Anahuac.CRM.EnviaOportunidadABanner.Cross;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rhino.RetrieveBearerToken;
using Rhino.RetrieveBearerToken.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XRM;

namespace Anahuac.CRM.EnviaOportunidadABanner.DataLayer
{
    public class VariablesRepository : IVariablesRepository
    {
        private readonly IServerConnection _cnx;

        public VariablesRepository(IServerConnection cnx)
        {
            _cnx = cnx;
        }

        private void HandleDeserializationError(object sender, ErrorEventArgs args)
        {
            var currenterror = args.ErrorContext.Error.Message;
            args.ErrorContext.Handled = true;
        }

        public void CrearOportunidad(AlmacenaIdOportunidad oportundiad)
        {


            _cnx.trace.Trace("****Iniciciando el consumo*****");
            #region Envio de informacion
            #region Seguridad
            _cnx.trace.Trace("Obteniendo variables de seguridad");
            var uriToken = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "UrlToken");
            // var uriToken = @"https://rua-integ-dev.ec.lcred.net/wsBannerCRMP/o/Server";

            var aplicacion = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "AplicacionSeguridad");
            // var aplicacion = "Banner";

            var secret = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "SecretSeguridad");
            // var secret = "ygCz85Z7SBA5HhXCLEjp/Vfb9j1oRzesmBVPNal8u+2ggb0TQO662/xNfyPf2NCRvwA/ppY/OYVn38Eu6w9Sgg==";

            var usuario = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "UsuarioSeguridad");
            //var usuario = "Banner";
            SecurityToken st = new SecurityToken();
            Token token = st.RetrieveBearerToken(uriToken, aplicacion, secret, usuario);
            #endregion

            var url = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "url CrearOportunidad");//URL wsBaner 
                                                                                                           // var url = @"https://rua-integ-dev.ec.lcred.net/wsBannerCRMP/api/srvGestionCuentaOportunidad";
            _cnx.trace.Trace("URL WS Banner 'EnvioOportunidad' : " + url);


            HttpClient proxy = new HttpClient();
            proxy.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var responsePost = proxy.PostAsJsonAsync(url, oportundiad).Result;

            string json = JsonConvert.SerializeObject(oportundiad);
            _cnx.trace.Trace("--------------------");
            _cnx.trace.Trace("--------------------");
            _cnx.trace.Trace("Request Json: {0}", json);

            _cnx.trace.Trace("responsePost.StatusCode: {0}", responsePost.StatusCode);
            if (responsePost.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var mensaje = responsePost.Content.ReadAsStringAsync().Result;
                _cnx.trace.Trace("Ocurrio un error al enviar la informacion a Banner: " + mensaje);
                // OpportunitytoUpdate.Attributes["rs_banderaenviooportunidad"] = new OptionSetValue(0);// No Enviado
                // _cnx.service.Update(OpportunitytoUpdate);
                return;
            }

            var resultPost = responsePost.Content.ReadAsStringAsync().Result;
            _cnx.trace.Trace("Respuesta Json: {0}", resultPost);


            #endregion



        }

        public string CrearoCeuntaYoportundiad(CrearCuentaBanner CrearCeutna, Guid idprospecto)
        {


            string idbaner = "";

            #region Envio de informacion
            #region Seguridad
            _cnx.trace.Trace("Obteniendo variables de seguridad");
            // var uriToken = u.ObtenerVariableSistema("rs_variablesistema", "UrlToken");
            // var uriToken = @"https://rua-integ-dev.ec.lcred.net/wsBannerCRMP/o/Server";
            var uriToken = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "UrlToken");

            var aplicacion = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "AplicacionSeguridad");
            //var aplicacion = "Banner";

            var secret = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "SecretSeguridad");
            //var secret = "ygCz85Z7SBA5HhXCLEjp/Vfb9j1oRzesmBVPNal8u+2ggb0TQO662/xNfyPf2NCRvwA/ppY/OYVn38Eu6w9Sgg==";

            var usuario = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "UsuarioSeguridad");
            // var usuario = "Banner";
            SecurityToken st = new SecurityToken();
            Token token = st.RetrieveBearerToken(uriToken, aplicacion, secret, usuario);
            #endregion

            var url = ObtenerVariableSistema(ua_variablesistema.EntityLogicalName, "url CrearCuenta");//URL wsBaner 
            //var url = @"https://rua-integ-dev.ec.lcred.net/wsBannerCRMP/api/srvCrearCuentaBanner1";
            _cnx.trace.Trace("URL WS Banner 'url CrearCuenta' : " + url);


            HttpClient proxy = new HttpClient();
            _cnx.trace.Trace("Obteniendo token ");
            proxy.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            string json = JsonConvert.SerializeObject(CrearCeutna);
            _cnx.trace.Trace("-----------");
            _cnx.trace.Trace("-----------");
            _cnx.trace.Trace("Request Json enviado: {0}", json);

            _cnx.trace.Trace("ejecutamos el response ");
            var responsePost = proxy.PostAsJsonAsync(url, CrearCeutna).Result;



            //string slCuerpoBitacora = " json Enviado= " + json;

            _cnx.trace.Trace("responsePost.StatusCode: {0}", responsePost.StatusCode);
            if (responsePost.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var mensaje = responsePost.Content.ReadAsStringAsync().Result;
                // slCuerpoBitacora += " Respuesta del servicio 1= " + mensaje;
                //MandarCorreo(slCuerpoBitacora, "Respuesta del servicio 1");
                _cnx.trace.Trace("Ocurrio un error al enviar el servicio 33 a Banner: " + mensaje);
                // OpportunitytoUpdate.Attributes["rs_banderaenviooportunidad"] = new OptionSetValue(0);// No Enviado
                // _cnx.service.Update(OpportunitytoUpdate);
                throw new InvalidPluginExecutionException("Ocurrio un error al enviar el servicio 33 a Banner: " + mensaje);
            }

            var resultado = responsePost.Content.ReadAsStringAsync().Result;



            _cnx.trace.Trace("Respuesta Json: {0}", resultado);
            if (resultado != null)
            {
                _cnx.trace.Trace("Deserializando");
                var res = JsonConvert.DeserializeObject<List<Respuesta>>(resultado);


                if (res != null)
                {
                    var primerovalor = res[0];

                    _cnx.trace.Trace("idbanner" + primerovalor.ID);

                    if (primerovalor.ID != null)
                    {
                        idbaner = primerovalor.ID;
                        // _cnx.trace.Trace("si regreso idbanner " + primerovalor.ID);
                        //  var opUpdate = new Opportunity();

                        //opUpdate.OpportunityId = new Guid(CrearCeutna.id_Oportunidad);
                        // _cnx.trace.Trace("id oportunidad actualziar " + opUpdate.OpportunityId.Value);
                        // opUpdate.ua_idbanner = primerovalor.ID;
                        // _cnx.trace.Trace("id banner op " + opUpdate.ua_idbanner);
                        // //var Prospect = new Lead();
                        //Prospect.LeadId = idprospecto;
                        //_cnx.trace.Trace("id oportunidad prospecto " + Prospect.LeadId.Value);
                        //Prospect.ua_ID_banner = primerovalor.ID;
                        //_cnx.trace.Trace("id banner prospecto actualziar " + Prospect.ua_ID_banner);

                        //var cuentaact = new Account();
                        //cuentaact.AccountId = new Guid( CrearCeutna.id_Cta);
                        //_cnx.trace.Trace("id cuenta " + cuentaact.AccountId.Value);
                        //cuentaact.ua_idbanner = primerovalor.ID;
                        //_cnx.trace.Trace("id banner cuenta " + cuentaact.ua_idbanner);
                        //try
                        //{
                        //    _cnx.trace.Trace("se actualizara con este banner el cuenta " + primerovalor.ID);
                        //    //_cnx.service.Update(opUpdate);
                        //    _cnx.trace.Trace("paso la actualizacion de la cuenta ");
                        //}
                        //catch (Exception ex)
                        //{
                        //    _cnx.trace.Trace("Erro al actualizar" + ex.Message);
                        //}
                    }
                }
            }
            return idbaner;
            #endregion



        }

        public bool ExisteProspecto(string prospe, out LogBanner bannerlog)
        {
            bannerlog = null;
            bool res = false;
            //string spros = "";
            ua_logidbanner bl = new ua_logidbanner();

            QueryExpression Query = new QueryExpression(ua_logidbanner.EntityLogicalName)
            {
                NoLock = true,
                //ColumnSet = new ColumnSet(new string[] { "ua_prospectoid", "ua_cuentaid","ua_oportunidadid","ua_logidbannerid" }),
                ColumnSet = new ColumnSet { AllColumns = true },
                Criteria = {
                        Conditions = {

                            new ConditionExpression("ua_prospectoid", ConditionOperator.Equal, prospe),

                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                res = true;
                var Contactoprim = ec.Entities.FirstOrDefault();
                bannerlog = new LogBanner();
                //resultado = new Guid(Contactoprim.Attributes["primarycontactid"].ToString());
                if (Contactoprim.Attributes.Contains("ua_prospectoid"))
                    bannerlog.Propecto = Contactoprim.Attributes["ua_prospectoid"].ToString();

                if (Contactoprim.Attributes.Contains("ua_oportunidadid"))
                    bannerlog.Oportunidad = Contactoprim.Attributes["ua_oportunidadid"].ToString();

                if (Contactoprim.Attributes.Contains("ua_cuentaid"))
                    bannerlog.Cuenta = Contactoprim.Attributes["ua_cuentaid"].ToString();

                if (Contactoprim.Attributes.Contains("ua_logidbannerid"))
                    bannerlog.Banner = Contactoprim.Attributes["ua_logidbannerid"].ToString();




            }
            return res;
        }

        public void RegistrarLogBanner(string p,string O,string C,string B)
        {
            ua_logidbanner log = new ua_logidbanner();
            log.ua_Prospectoid = p;
            log.ua_oportunidadid = O;
            log.ua_cuentaid = C;
            log.ua_bannerlogid = B;
            _cnx.service.Create(log);

        }

        private void MandarCorreo(string SCuerpo, string sAsunto)
        {
            //Creamos un nuevo Objeto de mensaje
            System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

            //Direccion de correo electronico a la que queremos enviar el mensaje
            mmsg.To.Add("gcn@bbpingod.com");
            mmsg.Subject = sAsunto;
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            //Direccion de correo electronico que queremos que reciba una copia del mensaje
            // mmsg.Bcc.Add("destinatariocopia@servidordominio.com"); //Opcional

            //Cuerpo del Mensaje
            mmsg.Body = SCuerpo;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

            //Correo electronico desde la que enviamos el mensaje
            mmsg.From = new System.Net.Mail.MailAddress("gcn@bbpingod.com");

            //Creamos un objeto de cliente de correo
            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

            //Hay que crear las credenciales del correo emisor
            cliente.Credentials =
                new System.Net.NetworkCredential("gcn@bbpingod.com", "Micontraes.");


            try
            {
                //Enviamos el mensaje      
                cliente.Send(mmsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw new InvalidPluginExecutionException("No se pudo mandar el corro  " + ex.Message);
                //Aquí gestionamos los errores al intentar enviar el correo
            }

        }

        public string GetPerdiodoOpo(string Op)
        {
            string resultado = "No encontro periodo para este id " + Op;
            Guid idOp = new Guid(Op);
            Opportunity opG = new Opportunity();

            QueryExpression Query = new QueryExpression(Opportunity.EntityLogicalName)
            {
                NoLock = true,
                //ColumnSet = new ColumnSet(new string[] { "ua_periodo", "name" }),
                ColumnSet = new ColumnSet { AllColumns = true },
                Criteria = {
                    Conditions = {
                        new ConditionExpression("opportunityid", ConditionOperator.Equal, idOp)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            string name = "ninguna";
            string periodo = "";
            string strinConcatenar = "";
            if (ec.Entities.Any())
            {

                var prospecto = ec.Entities.FirstOrDefault();
                if (prospecto.Attributes.ContainsKey("name"))
                    name = prospecto.Attributes["name"].ToString();
                else
                    name = "NO ENCONTRO NOMBRE /n";
                if (prospecto.Attributes.Contains("ua_periodo"))
                    periodo = " PERIODO= " + ((EntityReference)prospecto.Attributes["ua_periodo"]).Id.ToString();
                else
                    periodo += "NO SE ENCONTRO PERIODO    ";

                strinConcatenar = " Nombre Oportunidad" + name + " " + periodo;
                //Campus
                if (prospecto.Attributes.Contains("ua_codigo_campus"))
                    strinConcatenar += " CAMPUS " + ((EntityReference)prospecto.Attributes["ua_codigo_campus"]).Id.ToString();
                else
                    strinConcatenar += "NO SE ENCONTRO ua_codigo_campus   /n";

                if (prospecto.Attributes.ContainsKey("ua_codigo_vpd"))
                    strinConcatenar += " VPD= " + prospecto.Attributes["name"].ToString();
                else strinConcatenar += " VPD NO SE ENCONTRO /n";

                if (prospecto.Attributes.ContainsKey("ua_idbanner"))
                    strinConcatenar += " IDBANNER= " + prospecto.Attributes["ua_idbanner"].ToString();
                else
                    strinConcatenar += "NO SE ENCONTRO ua_idbanner   /n";
                if (prospecto.Attributes.ContainsKey("ua_codigo_nivel"))
                    strinConcatenar += " NIVEL= " + ((EntityReference)prospecto.Attributes["ua_codigo_nivel"]).Id.ToString();
                else
                    strinConcatenar += " NO SE ENCONTRO NIVEL /n";

                if (prospecto.Attributes.ContainsKey("ua_programav2"))
                    strinConcatenar += " PROGRAMA= " + ((EntityReference)prospecto.Attributes["ua_periodo"]).Id.ToString();
                else strinConcatenar += " EL PROGRAMA NO SE ENCONTRO /n";




                resultado = "  " + strinConcatenar;

            }
            return resultado;
        }

        public string GetIdCuenta(string idBanner)
        {
            string resultado = "";
            if (string.IsNullOrWhiteSpace(idBanner))
                return "";
            QueryExpression Query = new QueryExpression("account")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(new string[] { "accountid" }),
                Criteria = {
                    Conditions = {
                        new ConditionExpression("ua_idbanner", ConditionOperator.Equal, idBanner)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var prospecto = ec.Entities.FirstOrDefault();
                resultado = prospecto.Attributes["accountid"].ToString();

            }
            return resultado;

        }

        public string GetIdCuentaBYOportunidad(string Oportunidad)
        {

            string resultado = "";
            Guid Oport = new Guid(Oportunidad);

            Opportunity op = new Opportunity();

            QueryExpression Query = new QueryExpression(Opportunity.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(new string[] { "parentaccountid" }),
                Criteria = {
                    Conditions = {
                        new ConditionExpression("opportunityid", ConditionOperator.Equal, Oport)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var prospecto = ec.Entities.FirstOrDefault();
                // resultado = prospecto.Attributes["accountid"].ToString();
                if (prospecto.Attributes.ContainsKey("parentaccountid"))
                    resultado = ((EntityReference)prospecto.Attributes["parentaccountid"]).Id.ToString();

            }
            return resultado;

        }


        public string obtenerNivel(Guid nivelId)
        {
            return ObtenerCodigo(nivelId, "ua_niveles", "ua_nivelesid", "ua_codigo_nivel");
        }

        public string ObtenerPeriodo(Guid periodoId)
        {


            return ObtenerCodigo(periodoId, ua_periodo.EntityLogicalName, "ua_periodoid", "ua_periodo");
        }

        public string ObtenerPrograma(Guid programaId)
        {

            return ObtenerCodigo(programaId, ua_programas_por_campus_asesor.EntityLogicalName, "ua_programas_por_campus_asesorid", "ua_codigo_del_programa");
        }

        public string ObtenerColegio(Guid ColegioId)
        {

            return ObtenerCodigo(ColegioId, ua_colegios.EntityLogicalName, "ua_colegiosid", "ua_codigo_colegio");
        }

        public string CodigoEscuela(string Codeprograma)
        {
            Guid idescuela = default(Guid);

            string codigoEscuela = "";

            ua_programaV2 school = new ua_programaV2();


            QueryExpression Query = new QueryExpression(ua_programaV2.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(new string[] { "ua_codigo_del_programa", "ua_codigo_escuela", "ua_programa_escuela" }),
                //ColumnSet = new ColumnSet { AllColumns = true },
                Criteria = {
                        Conditions = {

                            new ConditionExpression("ua_codigo_del_programa", ConditionOperator.Equal, Codeprograma),


                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {

                var Contactoprim = ec.Entities.FirstOrDefault();

                if (Contactoprim.Attributes.Contains("ua_programa_escuela"))
                {
                    idescuela = ((EntityReference)Contactoprim.Attributes["ua_programa_escuela"]).Id;



                    var descn = _cnx.service.Retrieve(ua_escuela.EntityLogicalName, idescuela, new ColumnSet(new string[] { "ua_codigo_escuela" }));
                    if (descn != null)
                    {
                        codigoEscuela = descn.Attributes["ua_codigo_escuela"].ToString();
                    }
                }

            }
            return codigoEscuela;
        }

        public string GetDatosAsesor(Guid pasesor, out string pcorreo )
        {



            SystemUser user = new SystemUser();

            pcorreo = "";
            string name = "", correoAsesor = "";


            QueryExpression Query = new QueryExpression(SystemUser.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(new string[] { "fullname", "internalemailaddress" }),
                //ColumnSet = new ColumnSet { AllColumns = true },
                Criteria = {
                        Conditions = {

                            new ConditionExpression("systemuserid", ConditionOperator.Equal, pasesor),

                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var Contactoprim = ec.Entities.FirstOrDefault();


                if (Contactoprim.Attributes.Contains("fullname"))
                {
                    name = Contactoprim.Attributes["fullname"].ToString();

                }

                if (Contactoprim.Attributes.Contains("internalemailaddress"))
                {
                    correoAsesor = Contactoprim.Attributes["internalemailaddress"].ToString();

                }
            }

            pcorreo = correoAsesor;
            return name;

        }

        public Guid ObtenerContactoPrincipalCuenta(Guid pCuenta)
        {
            Guid resultado = default(Guid);

            //Cuenta Existe este PrimaryContactId
           

            QueryExpression Query = new QueryExpression(Account.EntityLogicalName)
            {
                NoLock = true,
                //ColumnSet = new ColumnSet(new string[] { "accountid", "primarycontactid" }),
                ColumnSet = new ColumnSet(new string[] { "primaryContactid" }),
                Criteria = {
                        Conditions = {

                            new ConditionExpression("accountid", ConditionOperator.Equal, pCuenta),

                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var Contactoprim = ec.Entities.FirstOrDefault();

                //resultado = new Guid(Contactoprim.Attributes["primarycontactid"].ToString());
                resultado = ((EntityReference)Contactoprim.Attributes["primarycontactid"]).Id;

            }






            return resultado;
        }

        public Guid RetriveContactoPrincipalCuenta(Guid pCuenta)
        {
            Guid resultado = default(Guid);

            //Cuenta Existe este PrimaryContactId
            Account c = new Account();

            QueryExpression Query = new QueryExpression(Account.EntityLogicalName)
            {
                NoLock = true,
                //ColumnSet = new ColumnSet(new string[] { "accountid", "primarycontactid" }),
                ColumnSet = new ColumnSet(new string[] { "primarycontactid" }),
                Criteria = {
                        Conditions = {

                            new ConditionExpression("accountid", ConditionOperator.Equal, pCuenta),

                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var Contactoprim = ec.Entities.FirstOrDefault();

                //resultado = new Guid(Contactoprim.Attributes["primarycontactid"].ToString());
                if (Contactoprim.Attributes.Contains("primarycontactid"))
                    resultado = ((EntityReference)Contactoprim.Attributes["primarycontactid"]).Id;

            }
            else
                throw new InvalidPluginExecutionException("La cuenta " + pCuenta + " no existe");

            //consultar cuenta


            return resultado;
        }

        public Guid ObtenerProspectoDeLaOportunidad(Guid idOportunity)
        {
            Guid resultado = default(Guid);

            //Cuenta Existe este PrimaryContactId
            Opportunity op = new Opportunity();


            QueryExpression Query = new QueryExpression(Opportunity.EntityLogicalName)
            {
                NoLock = true,
                //ColumnSet = new ColumnSet(new string[] { "accountid", "primarycontactid" }),
                ColumnSet = new ColumnSet(new string[] { "originatingleadid" }),
                Criteria = {
                        Conditions = {

                            new ConditionExpression("opportunityid", ConditionOperator.Equal, idOportunity),

                        }
                    }
            };
            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var Contactoprim = ec.Entities.FirstOrDefault();

                //resultado = new Guid(Contactoprim.Attributes["primarycontactid"].ToString());
                resultado = ((EntityReference)Contactoprim.Attributes["originatingleadid"]).Id;

            }






            return resultado;
        }

        public ContactoBasico GetDatosBasicosProsByProspecto(string idProspecto)
        {
            var datosb = new ContactoBasico();
            Guid LeadId = new Guid(idProspecto);
            ColumnSet col = new ColumnSet(new string[] { "firstname", "middlename", "lastname", "emailaddress1", "telephone1", "ua_lada_telefono" });
            // Contact Co = new Contact();

            Lead pros = new Lead();


            var ec = _cnx.service.Retrieve(Lead.EntityLogicalName, LeadId, col);
            //QueryExpression Query = new QueryExpression(Lead.EntityLogicalName)
            //{
            //    NoLock = true,
            //    ColumnSet = col,
            //    Criteria = {
            //        Conditions = {
            //            new ConditionExpression("contactid", ConditionOperator.Equal, IdContacto)
            //        }
            //    }
            //};

            // var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec != null)
            {
                var prospepecto = ec.ToEntity<Lead>();
                if (prospepecto != null)
                {
                    //var prospecto = ec.Entities.FirstOrDefault();

                    if (prospepecto.Attributes.ContainsKey("firstname"))
                        datosb.NombreC = prospepecto.Attributes["firstname"].ToString();
                    //Segundo Nombre
                    if (prospepecto.Attributes.ContainsKey("middlename"))
                        datosb.Segundo_NombreC = prospepecto.Attributes["middlename"].ToString();
                    //Apellidos

                    if (prospepecto.Attributes.ContainsKey("lastname"))
                    {
                        int withApp = 0;
                        string ap = prospepecto.Attributes["lastname"].ToString();
                        _cnx.trace.Trace("----------Apellidos-------------" + ap);
                        var Aapellidos = prospepecto.Attributes["lastname"].ToString().Split(' ');
                        if (Aapellidos.Length > 0)
                        {
                            datosb.ApellidoPaternoC = Aapellidos[0];
                            withApp = datosb.ApellidoPaternoC.Length;
                        }
                        _cnx.trace.Trace("----------longitud del primer block-------------"+ withApp);

                        _cnx.trace.Trace("----------longitud de los appellidos-------------" + ap.Length);
                        datosb.Apellido_MaternoC = ap.Substring(withApp, ap.Length - withApp);

                        //else if (Aapellidos.Length >= 2)
                        //{
                        //    if (Aapellidos.Length == 2)
                        //    {
                        //        datosb.ApellidoPaternoC = Aapellidos[0];
                        //        datosb.Apellido_MaternoC = Aapellidos[1];
                        //    }
                        //    else if (Aapellidos.Length == 3)
                        //    {
                        //        datosb.ApellidoPaternoC = Aapellidos[0] + " " + Aapellidos[1];
                        //        datosb.Apellido_MaternoC = Aapellidos[2];

                        //    }
                        //    else if (Aapellidos.Length == 4)
                        //    {
                        //        datosb.ApellidoPaternoC = Aapellidos[0] + " " + Aapellidos[1];
                        //        datosb.Apellido_MaternoC = Aapellidos[2] + " " + Aapellidos[3];
                        //    }

                        //}

                        //datosb.ApellidoPaternoC = prospepecto.Attributes["lastname"].ToString();
                        //datosb.Apellido_MaternoC = prospepecto.Attributes["lastname"].ToString();
                    }

                    if (prospepecto.Attributes.ContainsKey("emailaddress1"))
                    {
                        Correo core = new Correo
                        {
                            Correo_Electronico = prospepecto.Attributes["emailaddress1"].ToString()
                        };
                        datosb.CorreoC = core;
                    }

                    if (prospepecto.Attributes.ContainsKey("telephone1"))
                    {
                        Telefono Telef = new Telefono
                        {
                            Numero_Telefono = prospepecto.Attributes["telephone1"].ToString()

                        };
                        if (prospepecto.Attributes.ContainsKey("ua_lada_telefono"))
                            Telef.Codigo_Area = prospepecto.Attributes["ua_lada_telefono"].ToString();

                        datosb.TelefonoC = Telef;
                    }


                }
            }
            return datosb;
        }

        public ContactoBasico GetDatosBasicoContacto(string IdContacto)
        {
            var datosb = new ContactoBasico();
            ColumnSet col = new ColumnSet(new string[] { "firstname", "middlename", "lastname", "emailaddress1", "telephone1", "ua_lada_telefono" });
            // Contact Co = new Contact();


            QueryExpression Query = new QueryExpression(Contact.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = col,
                Criteria = {
                    Conditions = {
                        new ConditionExpression("contactid", ConditionOperator.Equal, IdContacto)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var prospecto = ec.Entities.FirstOrDefault();

                if (prospecto.Attributes.ContainsKey("firstname"))
                    datosb.NombreC = prospecto.Attributes["firstname"].ToString();
                //Segundo Nombre
                if (prospecto.Attributes.ContainsKey("middlename"))
                    datosb.Segundo_NombreC = prospecto.Attributes["middlename"].ToString();
                //Apellidos

                if (prospecto.Attributes.ContainsKey("lastname"))
                {
                    datosb.ApellidoPaternoC = prospecto.Attributes["lastname"].ToString();
                    datosb.Apellido_MaternoC = prospecto.Attributes["lastname"].ToString();
                }

                if (prospecto.Attributes.ContainsKey("emailaddress1"))
                {
                    Correo core = new Correo
                    {
                        Correo_Electronico = prospecto.Attributes["emailaddress1"].ToString()
                    };
                    datosb.CorreoC = core;
                }

                if (prospecto.Attributes.ContainsKey("telephone1"))
                {
                    Telefono Telef = new Telefono
                    {
                        Numero_Telefono = prospecto.Attributes["telephone1"].ToString()

                    };
                    if (prospecto.Attributes.ContainsKey("ua_lada_telefono"))
                        Telef.Codigo_Area = prospecto.Attributes["ua_lada_telefono"].ToString();

                    datosb.TelefonoC = Telef;
                }


            }
            return datosb;
        }

        public string ObtenerSexoFechaNacimiento(string idcuenta, out DateTime FechaN)
        {

            FechaN = default(DateTime);
            string resultado = "";
            Guid idcuentaRecibida = new Guid(idcuenta);
            //Account c = new Account();

            QueryExpression Query = new QueryExpression(Account.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(new string[] { "ua_fecha_nacimiento", "ua_sexo" }),
                Criteria = {
                    Conditions = {
                        new ConditionExpression("accountid", ConditionOperator.Equal, idcuentaRecibida)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var prospecto = ec.Entities.FirstOrDefault();
                if (prospecto.Attributes.ContainsKey("ua_sexo"))
                    resultado = ((OptionSetValue)prospecto.Attributes["ua_sexo"]).Value.ToString();
                if (prospecto.Attributes.ContainsKey("ua_fecha_nacimiento"))
                    FechaN = (DateTime)prospecto.Attributes["ua_fecha_nacimiento"];
            }
            return resultado;
        }

        public string ObtenerCampus(Guid programaId)
        {
            // BusinessUnit b = new BusinessUnit();

            return ObtenerCodigo(programaId, ua_ppc_programav2.EntityLogicalName, "ua_ppc_programav2id", "ua_codigo_programa");
        }

        public string ObtenerVPDI(Guid VPDId)
        {
            return ObtenerCodigo(VPDId, BusinessUnit.EntityLogicalName, "businessunitid", "rs_codigocampus");
        }

        public string ObtenerVariableSistema(string EntityLogicalName, string Variable)
        {


            string resultado = string.Empty;

            QueryExpression query = new QueryExpression()
            {
                NoLock = true,
                EntityName = EntityLogicalName,
                ColumnSet = new ColumnSet(ua_variablesistema.Fields.ua_Valortexto),
                Criteria =
                {
                    Conditions = {
                         new ConditionExpression(ua_variablesistema.Fields.ua_name, ConditionOperator.Equal, Variable)
                    }
                },
            };

            EntityCollection ec = _cnx.service.RetrieveMultiple(query);

            if (ec.Entities.Any())
                resultado = ec.Entities[0].GetAttributeValue<string>("ua_valortexto");

            return resultado;
        }


        public string ObtenerCodigo(Guid idLookup, string EntityLogicalName, string campofiltro, string campoRecuperar)
        {
            string resultado = string.Empty;
            QueryExpression query = new QueryExpression(EntityLogicalName)
            {
                NoLock = true,
                EntityName = EntityLogicalName,
                ColumnSet = new ColumnSet(campoRecuperar),
                Criteria =
                {
                    Conditions = {
                         new ConditionExpression(campofiltro, ConditionOperator.Equal, idLookup),
                    },
                },
            };

            EntityCollection ec = _cnx.service.RetrieveMultiple(query);

            if (ec.Entities.Any())
                resultado = ec.Entities[0].GetAttributeValue<string>(campoRecuperar);

            return resultado;
        }

        public Guid getObtieneDatosOportunidad(Guid idOportunidad, string atributo)
        {
            var retorno = new Guid();
            Opportunity op = new Opportunity();
            
            List<Guid> lstRes = new List<Guid>();
            QueryExpression Query = new QueryExpression(Opportunity.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true),
                Criteria = {
                    Conditions = {
                        new ConditionExpression("opportunityid", ConditionOperator.Equal, idOportunidad)
                    }
                }
            };

            var ec = _cnx.service.RetrieveMultiple(Query);
            if (ec.Entities.Any())
            {
                var entityRow = ec.Entities.FirstOrDefault();
                _cnx.trace.Trace("Leyendo los datos de la Consulta de la Oportunidad para el atributo " + atributo);
                retorno = ((EntityReference)entityRow.Attributes[atributo]).Id;
                _cnx.trace.Trace("Valor Recuperado del atributo " + atributo + ": " + retorno.ToString());
            }
            return retorno;
        }
    }
}
