using Anahuac.CRM.EnviaOportunidadABanner.CRM;
using Anahuac.CRM.EnviaOportunidadABanner.Cross;
using Anahuac.CRM.EnviaOportunidadABanner.Cross.Extensiones;
using Anahuac.CRM.EnviaOportunidadABanner.DataLayer;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
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
using System.ServiceModel;


namespace Anahuac.CRM.EnviaOportunidadABanner
{
    public class EnviaIOportunidad : IPlugin
    {
        //private readonly string preImageUpdate = "UpdateImage";
        private readonly string postCreateImageUpdate = "CreateImage";
       

        public void Execute(IServiceProvider serviceProvider)
        {
           
            // ConexionSQL ConSQL;
            ServerConnection _cnx;
            _cnx = new ServerConnection(serviceProvider);
            _cnx.context.SharedVariables.Clear();

            //ConSQL = new ConexionSQL();

            // ConSQL.InsertarBanderasBDPrueba("inicia el plug in");
            #region " Validaciones de Plugin "


            if (_cnx.context.MessageName != "Create" && _cnx.context.MessageName != "Update")
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is not Update or Create");
                _cnx.trace.Trace("Is not Update or Create");
                return;
            }

            if (_cnx.context.Stage != 40)
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Invalid Stage");
                _cnx.trace.Trace("Invalid Stage");
                return;
            }

            if (_cnx.context.Depth > 4)
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Deepth has exceded");
                _cnx.trace.Trace("Deepth has exceded");
                return;
            }

            if (!_cnx.context.InputParameters.Contains("Target"))
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Do not Contains Target");
                _cnx.trace.Trace("Do not Contains Target");
                return;
            }

            if (!(_cnx.context.InputParameters["Target"] is Entity))
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is Not an Entity");
                _cnx.trace.Trace("Is Not an Entity");
                return;
            }

            if (_cnx.context.PrimaryEntityName != "opportunity")
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is not a opportunity");
                _cnx.trace.Trace("Is not a opportunity");
                return;
            }


            #endregion

            try
            {
                //ConexionSQL cone = new ConexionSQL();


                #region Validacion Registro

                IPluginExecutionContext context = _cnx.context;
                Entity currententidad = (Entity)_cnx.context.InputParameters["Target"];
                //Entity postImageEntity = (context.PostEntityImages != null &&
                //    context.PostEntityImages.Contains(this.preImageUpdate)) ? context.PostEntityImages[this.preImageUpdate] : null;

                Entity postCreateImageEntity = (context.PostEntityImages != null &&
                   context.PostEntityImages.Contains(this.postCreateImageUpdate)) ? context.PostEntityImages[this.postCreateImageUpdate] : null;


                //if (_cnx.context.MessageName == "Update")
                //    if (postImageEntity == null)
                //    {
                //        _cnx.trace.Trace("La configuracion del plugin en el evento Update es invalida");
                //        return;
                //    }

                if (postCreateImageEntity == null)
                {
                    postCreateImageEntity = new Entity("opportunity");
                }

                //var insertarSQL = new AlmacenaIdOportunidad();



                Opportunity dbRecord = postCreateImageEntity.ToEntity<Opportunity>();
                Opportunity currentrecord = currententidad.ToEntity<Opportunity>();
                //Opportunity OpportunitytoUpdate = new Opportunity();
                //OpportunitytoUpdate.OpportunityId = currentrecord.OpportunityId;

                _cnx.trace.Trace("Id Oportunidad from Context  {0} ", currentrecord.OpportunityId.Value);

                //var StageId = dbRecord.StageId != null ? dbRecord.StageId : currentrecord.StageId;
                var Origen = dbRecord.ua_origen != null ? dbRecord.ua_origen : currentrecord.ua_origen;

                //Validar el origen para enviar la oportunidad con el numero de solicitud
                if (Origen != null)
                {
                    _cnx.trace.Trace("Origen de Oportunidad: {0}", ((OptionSetValue)Origen).Value);


                    //if (Origen.ToString() == "3") // si el origen es 3 (Banner)
                    //{
                    //    _cnx.trace.Trace("Validando Num de solicitud de Oportunidad");
                    //    //validar si se cuenta con solicitud
                    //    if (numsolicitud == null)
                    //    {
                    //        _cnx.trace.Trace("El origen del registro es Banner y no cuenta con numero de solicitud ");
                    //        return;
                    //    }
                    //    _cnx.trace.Trace("No de solicitud a enviar:{0}", numsolicitud.Value);

                    //}
                }
                else
                {
                    _cnx.trace.Trace("No se tiene el origen del registro");
                    //Se le asigna el valor 1, para indicar que es naicdo en CRM
                    Origen = new OptionSetValue(1);
                }

                //string pros1 = "";
                //_cnx.trace.Trace(" obteniendo dbrecord ");
                //var prospecto1 = dbRecord.OriginatingLeadId != null ? dbRecord.OriginatingLeadId : dbRecord.OriginatingLeadId;
                //if (prospecto1 != null)
                //{
                //    pros1 = ((EntityReference)prospecto1).Id.ToString();
                //    ProspectoP = pros1;
                //    _cnx.trace.Trace("Prospecto obtenido dbrecord  " + ProspectoP);
                //}

                VariablesRepository u = new VariablesRepository(_cnx);

                //LogBanner log = new LogBanner();

                _cnx.trace.Trace("fase oportunidad currentrecord  : " + currentrecord.StageId);
                _cnx.trace.Trace("fase de la oportunidad dbRecord : " + dbRecord.StageId);
                //if (u.ExisteProspecto(ProspectoP.Trim(), out log) == false)
                {
                    //5b82ce2a-4971-4835-9577-4a2dcbe59986  fase de oportunidad
                    //bfc9108c-8389-406b-9166-2c3298a2e41f  fase de solicitante
                    _cnx.trace.Trace("Validando que no benda de banner y este en al fase correcta ");
                    // if ((currentrecord.StageId.ToString() == "5b82ce2a-4971-4835-9577-4a2dcbe59986") && (((OptionSetValue)Origen).Value != 3))
                    if ((((OptionSetValue)Origen).Value != 3))
                    {
                        _cnx.trace.Trace("No biene de Banner y se ejecuto ");
                        _cnx.trace.Trace("Oportunidad from DB: " + dbRecord.Id);

                        var idbaner = dbRecord.ua_idbanner != null ? dbRecord.ua_idbanner : currentrecord.ua_idbanner;
                        var idbaner2 = currentrecord.ua_idbanner != null ? currentrecord.ua_idbanner : currentrecord.ua_idbanner;
                        _cnx.trace.Trace("obteniendo id prospecto");

                        //var prospecto = currentrecord.OriginatingLeadId != null ? currentrecord.OriginatingLeadId : currentrecord.OriginatingLeadId;
                        //if (prospecto != null)
                        //{
                        //    string pros = ((EntityReference)prospecto).Id.ToString();
                        //    _cnx.trace.Trace("Prospecto obtenido curren " + prospecto);
                        //}



                        _cnx.trace.Trace("Validando IdBanner de Oportunidad");
                        _cnx.trace.Trace(" instanciando VariablesRepository");

                        //if (string.IsNullOrWhiteSpace(idbaner))
                        //{
                        //    _cnx.trace.Trace("La oportunidad no cuenta con Id de Baner ");
                        //    _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Id Banner");
                        //    return;
                        //}
                        //else
                        {
                            _cnx.trace.Trace("Validando Origen de la Oportunidad");
                            //var numsolicitud = dbRecord.rs_numerosolicitud != null ? dbRecord.rs_numerosolicitud : currentrecord.rs_numerosolicitud; //dbRecord.rs_numerosolicitud
                            var numsolicitud = dbRecord.ua_numero_solicitud != null ? dbRecord.ua_numero_solicitud : currentrecord.ua_numero_solicitud; //dbRecord.rs_numerosolicitud


                            _cnx.trace.Trace("obteniendo periodo programa y vpdi");
                            var periodo = dbRecord.ua_periodo != null ? dbRecord.ua_periodo : currentrecord.ua_periodo;
                            //var programa = dbRecord.ua_programav2 != null ? dbRecord.ua_programav2 : currentrecord.ua_programav2;
                            var programa = dbRecord.ua_programa_asesor != null ? dbRecord.ua_programa_asesor : currentrecord.ua_programa_asesor;

                            var vpdi = dbRecord.ua_codigo_vpd != null ? dbRecord.ua_codigo_vpd : currentrecord.ua_codigo_vpd;
                            vpdi = vpdi.Trim();

                            _cnx.trace.Trace("buscame el perdido de esta oportunidad" + currentrecord.OpportunityId.Value.ToString());

                            // string PeriodoBusuqeda = u.GetPerdiodoOpo(currentrecord.OpportunityId.Value.ToString());

                            _cnx.trace.Trace("Datos Encontrados:");
                            // _cnx.trace.Trace(PeriodoBusuqeda);

                            if (periodo == null)
                            {
                                _cnx.trace.Trace("La oportunidad no tiene periodo asignado");
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Periodo");
                                return;
                            }

                            if (programa == null)
                            {
                                _cnx.trace.Trace("La oportunidad no tiene programa asignado");
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Programa");
                                return;
                            }

                            if (vpdi == null)
                            {
                                _cnx.trace.Trace("La oportunidad no tiene VPDI asignado");
                                return;
                            }

                            #region Recuperacion de datos del CRM y validacion
                            _cnx.trace.Trace(" recuperando variablesRepository");
                            //VariablesRepository u = new VariablesRepository(_cnx);


                            // si es nuevo el registro se le asigna un id
                            //if (_cnx.context.MessageName == "Create")
                            //    currentrecord.OpportunityId = Guid.NewGuid();
                            _cnx.trace.Trace(" mapeando la oportunidad");
                            var idopportunity = currentrecord.OpportunityId; // dbRecord.OpportunityId != null ? dbRecord.OpportunityId : currentrecord.OpportunityId;
                            //Objeta para mandarDatosBannerServicio 1
                            var CrearCuentBaner = new CrearCuentaBanner();
                            //Objeto mandar datos banner serivicio 33
                            var oportunidad = new AlmacenaIdOportunidad();

                            oportunidad.Id_Oportunidad = idopportunity.Value.ToString();
                            CrearCuentBaner.id_Oportunidad = idopportunity.Value.ToString();
                            oportunidad.Id_Banner = idbaner;
                           
                            // CrearCuentBaner.Id_Banner_Vinculante = idbaner;


                            _cnx.trace.Trace("Obteniendo periodo de variables repository");
                            oportunidad.Periodo = u.ObtenerPeriodo(periodo.Id);
                            //CrearCuentaBanner
                            CrearCuentBaner.Periodo = oportunidad.Periodo;
                            _cnx.trace.Trace("Periodo encontrado " + oportunidad.Periodo);

                            _cnx.trace.Trace("Obteniendo programa de variables repository " + programa.Id);
                            oportunidad.Programa = u.ObtenerPrograma(programa.Id);
                            //crearCuenta
                            CrearCuentBaner.Programa = oportunidad.Programa;
                            _cnx.trace.Trace("Programa obtenido " + oportunidad.Programa);
                            //CreateCuenta
                            _cnx.trace.Trace("Obteniendo vpdi de variables repository");
                            CrearCuentBaner.Campus = vpdi;
                            _cnx.trace.Trace("Obteniendo vpdi de variables repository");
                            oportunidad.VPD = vpdi;
                            //CrearCuenta
                            CrearCuentBaner.VPD = vpdi;
                            _cnx.trace.Trace("Obteniendo IdCuenta de variables repository");
                            _cnx.trace.Trace("-----------------------------------");
                            //var idcuent = u.GetIdCuenta(idbaner);
                            var idcuent = u.GetIdCuentaBYOportunidad(oportunidad.Id_Oportunidad);
                            _cnx.trace.Trace("Cuenta obtenida= " + idcuent.ToString());

                            if (numsolicitud == null)
                            {
                                oportunidad.Numero_Solicitud = 0;
                            }
                            else
                            {
                                oportunidad.Numero_Solicitud = numsolicitud.Value;

                            }

                            _cnx.trace.Trace("numero de solicitud " + oportunidad.Numero_Solicitud);

                            _cnx.trace.Trace("Validando Banner");
                            _cnx.trace.Trace("IDBANNER-----------" + idbaner);
                            if (!string.IsNullOrWhiteSpace(idbaner))
                            {
                                oportunidad.Id_Cuenta = idcuent;
                              
                                try
                                {
                                    _cnx.trace.Trace("Validando oportunity");
                                    oportunidad.Validate();
                                    _cnx.trace.Trace("Ejecutamos metodo de CrearOportunidad");
                                    u.CrearOportunidad(oportunidad);
                                }
                                catch (BusinessTypeException ex)
                                {
                                    _cnx.trace.Trace("Periodo: {0}", periodo.Id);
                                    _cnx.trace.Trace("Programa: {0}", programa.Id);
                                    _cnx.trace.Trace("VPD: {0}", vpdi);
                                    _cnx.trace.Trace("Error en la informacion a enviar: {0}", ex.Message);

                                    return;
                                }


                            }
                            else//Creamos cuenta y oportundad serivicio 1
                            {
                                _cnx.trace.Trace("*****ENTRAMOS  A EJECUTAR EL 1*****");
                                _cnx.trace.Trace("Obteniendo periodo de Sexo y FechaNacimiento de  repository");
                                DateTime FechaNA = default(DateTime);
                                CrearCuentBaner.id_Cta = idcuent;
                               
                                _cnx.trace.Trace("Ontenemos la fehca de nacimiento de esta cuenta " + CrearCuentBaner.id_Cta);
                                var Sexo = u.ObtenerSexoFechaNacimiento(CrearCuentBaner.id_Cta, out FechaNA);
                                _cnx.trace.Trace("-----------------------");

                                _cnx.trace.Trace("Convertiendo la fecha customFEcha " + FechaNA);
                                CrearCuentBaner.Fecha_Nacimiento = new FechaNacimiento { Year = FechaNA.Year, Month = FechaNA.Month, Day = FechaNA.Day };
                                _cnx.trace.Trace("Fecha convertida " + CrearCuentBaner.Fecha_Nacimiento);

                                _cnx.trace.Trace("-----------GetSEXXO------------");
                                CrearCuentBaner.Sexo = Sexo == "2" ? "M" : "F";
                                _cnx.trace.Trace("Sexo " + CrearCuentBaner.Sexo);

                                _cnx.trace.Trace(" Obteniendo el id del contacto principal de la Opportunidad " + CrearCuentBaner.id_Oportunidad);
                                //Datos basicos del contacto
                                //Guid idContactoPrincipalCuenta = u.ObtenerContactoPrincipalCuenta(new Guid(CrearCuentBaner.id_Cta));
                                Guid idContactoPrincipalCuenta = u.ObtenerProspectoDeLaOportunidad(new Guid(CrearCuentBaner.id_Oportunidad));
                                _cnx.trace.Trace("-----------idProspecto------------" + idContactoPrincipalCuenta);
                                _cnx.trace.Trace("-----------------------");

                                var dtbc = u.GetDatosBasicosProsByProspecto(idContactoPrincipalCuenta.ToString());
                                if (dtbc != null)
                                {

                                    CrearCuentBaner.Nombre = dtbc.NombreC;
                                    _cnx.trace.Trace("--------Nombre--------" + CrearCuentBaner.Nombre);
                                    CrearCuentBaner.Segundo_Nombre = dtbc.Segundo_NombreC;
                                    _cnx.trace.Trace("----Segundo Nombre------" + CrearCuentBaner.Segundo_Nombre);
                                    if (!string.IsNullOrWhiteSpace(dtbc.ApellidoPaternoC))
                                        CrearCuentBaner.ApellidoPaterno = dtbc.ApellidoPaternoC;
                                    _cnx.trace.Trace("----Apellido P------" + CrearCuentBaner.ApellidoPaterno);

                                    if (!string.IsNullOrWhiteSpace(dtbc.Apellido_MaternoC))
                                        CrearCuentBaner.Apellido_Materno = dtbc.Apellido_MaternoC;
                                    _cnx.trace.Trace("----Apellido M------" + CrearCuentBaner.Apellido_Materno);
                                    //_cnx.trace.Trace("----NumeroTelefono------" + dtbc.TelefonoC.Codigo_Area);
                                    if (dtbc.TelefonoC != null)
                                    {
                                        _cnx.trace.Trace("----NumeroTelefono------" + dtbc.TelefonoC.Numero_Telefono);
                                        CrearCuentBaner.Telefono = dtbc.TelefonoC;
                                    }
                                    _cnx.trace.Trace("----Correo------" + dtbc.CorreoC.Correo_Electronico);
                                    CrearCuentBaner.Correo = dtbc.CorreoC;

                                }

                                //Crearcuenta


                                try
                                {
                                    _cnx.trace.Trace("Validando oportunity");
                                    CrearCuentBaner.Validate();
                                    string idbanerRespues = u.CrearoCeuntaYoportundiad(CrearCuentBaner, idContactoPrincipalCuenta);
                                   
                                    _cnx.trace.Trace("idbanenr regresado");

                                    dbRecord.ua_idbanner = idbanerRespues;

                                    _cnx.trace.Trace("idbanenr actualziar " + dbRecord.ua_idbanner);

                                    _cnx.service.Update(dbRecord);

                                    _cnx.trace.Trace("actualizo el idbanner de la oportundiad " + dbRecord.Id);

                                    _cnx.trace.Trace("Consultano idbanner prospecto");


                                    var cuent = new Account();
                                    cuent.AccountId = new Guid(CrearCuentBaner.id_Cta);
                                    cuent.ua_idbanner = idbanerRespues;
                                    _cnx.trace.Trace("Actualziando el objeto cuenta");
                                    _cnx.service.Update(cuent);
                                    _cnx.trace.Trace("se actualuzo la cuenta");

                                   


                                }
                                catch (BusinessTypeException ex)
                                {
                                    _cnx.trace.Trace("d_Cta: {0}", CrearCuentBaner.id_Cta);
                                    _cnx.trace.Trace("id_Oportunidad: {0}", CrearCuentBaner.id_Oportunidad);
                                    _cnx.trace.Trace("Nombre: {0}", CrearCuentBaner.Nombre);
                                    _cnx.trace.Trace("ApellidoPaterno: {0}", CrearCuentBaner.ApellidoPaterno);
                                    _cnx.trace.Trace("Fecha_Nacimiento: {0}", CrearCuentBaner.Fecha_Nacimiento.Year);
                                    _cnx.trace.Trace("Campus: {0}", CrearCuentBaner.Campus);
                                    _cnx.trace.Trace("Sexo: {0}", CrearCuentBaner.Sexo);
                                    _cnx.trace.Trace("Periodo: {0}", CrearCuentBaner.Periodo);
                                    _cnx.trace.Trace("VPD: {0}", CrearCuentBaner.VPD);
                                    _cnx.trace.Trace("Programa: {0}", CrearCuentBaner.Programa);

                                    _cnx.trace.Trace("Error en la informacion a enviar: {0}", ex.Message);
                                    //_cnx.context.SharedVariables.Add("AbortProcess", string.Format("Error en la informacion a enviar: {0}", ex.Message));
                                    //OpportunitytoUpdate.Attributes["rs_banderaenviooportunidad"] = new OptionSetValue(0);// No Enviado
                                    // _cnx.service.Update(OpportunitytoUpdate);
                                    return;
                                }

                            }//Fin else crearCeunta



                            #endregion

                        }



                        #endregion



                    }//Fin del if==


                }//Fin del if null
               
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _cnx.trace.Trace("The application terminated with an error.");
                _cnx.trace.Trace("Timestamp: {0}", ex.Detail.Timestamp);
                _cnx.trace.Trace("Code: {0}", ex.Detail.ErrorCode);
                _cnx.trace.Trace("Message: {0}", ex.Detail.Message);
                _cnx.trace.Trace("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                _cnx.trace.Trace("The application terminated with an error.");
                _cnx.trace.Trace("Message: {0}", ex.Message);
                _cnx.trace.Trace("Stack Trace: {0}", ex.StackTrace);
                _cnx.trace.Trace("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                _cnx.trace.Trace("The application terminated with an error.");
                _cnx.trace.Trace(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    _cnx.trace.Trace(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        _cnx.trace.Trace("Timestamp: {0}", fe.Detail.Timestamp);
                        _cnx.trace.Trace("Code: {0}", fe.Detail.ErrorCode);
                        _cnx.trace.Trace("Message: {0}", fe.Detail.Message);
                        _cnx.trace.Trace("Trace: {0}", fe.Detail.TraceText);
                        _cnx.trace.Trace("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }

        }
    }
}
