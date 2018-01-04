using Anahuac.CRM.EnviaOportunidadABanner.CRM;
using Anahuac.CRM.EnviaOportunidadABanner.Cross;
using Anahuac.CRM.EnviaOportunidadABanner.Cross.Extensiones;
using Anahuac.CRM.EnviaOportunidadABanner.DataLayer;
using Microsoft.Xrm.Sdk;
using System;
using XRM;
using System.ServiceModel;
using Newtonsoft.Json;
using System.Threading;

namespace Anahuac.CRM.EnviaOportunidadABanner
{
    public class EnviaIOportunidad : IPlugin
    {
        //private readonly string preImageUpdate = "UpdateImage";
        private readonly string postCreateImageUpdate = "CreateImage";


        public void Execute(IServiceProvider serviceProvider)
        {
            string vsMensaje = "Iniciando Log...";
            // ConexionSQL ConSQL;
            ServerConnection _cnx;
            _cnx = new ServerConnection(serviceProvider);
            _cnx.context.SharedVariables.Clear();
            VariablesRepository u = new VariablesRepository(_cnx);
            #region " Validaciones de Plugin "
            if (_cnx.context.MessageName != "Create" && _cnx.context.MessageName != "Update")
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is not Update or Create");
                vsMensaje += (char)10 + "Is not Update or Create";
                return;
            }

            if (_cnx.context.Stage != 40)
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Invalid Stage");
                vsMensaje += (char)10 + "Invalid Stage";
                return;
            }

            if (_cnx.context.Depth > 4)
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Deepth has exceded");
                vsMensaje += (char)10 + "Deepth has exceded";
                return;
            }

            if (!_cnx.context.InputParameters.Contains("Target"))
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Do not Contains Target");
                vsMensaje += (char)10 + "Do not Contains Target";
                return;
            }

            if (!(_cnx.context.InputParameters["Target"] is Entity))
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is Not an Entity");
                vsMensaje += (char)10 + "Is Not an Entity";
                return;
            }

            if (_cnx.context.PrimaryEntityName != "opportunity")
            {
                _cnx.context.SharedVariables.Add("AbortProcess", "Is not a opportunity");
                vsMensaje += (char)10 + "Is not a opportunity";
                return;
            }


            #endregion

            try
            {
                #region Validacion Registro

                IPluginExecutionContext context = _cnx.context;
                Entity currententidad = (Entity)_cnx.context.InputParameters["Target"];
                Entity postCreateImageEntity = (context.PostEntityImages != null &&
                   context.PostEntityImages.Contains(this.postCreateImageUpdate)) ? context.PostEntityImages[this.postCreateImageUpdate] : null;

                if (postCreateImageEntity == null)
                {
                    postCreateImageEntity = new Entity("opportunity");
                }

                Opportunity dbRecord = postCreateImageEntity.ToEntity<Opportunity>();
                Opportunity currentrecord = currententidad.ToEntity<Opportunity>();
                //Opportunity OpportunitytoUpdate = new Opportunity();
                //OpportunitytoUpdate.OpportunityId = currentrecord.OpportunityId;

                vsMensaje += (char)10 + "Id Oportunidad from Context: " + currentrecord.OpportunityId.Value;

                //var StageId = dbRecord.StageId != null ? dbRecord.StageId : currentrecord.StageId;
                var Origen = dbRecord.ua_origen != null ? dbRecord.ua_origen : currentrecord.ua_origen;
                //Tomamos la descripcion del origen
                string OrigenString = dbRecord.ua_origen == null ? "" : dbRecord.FormattedValues["ua_origen"];


                //Validar el origen para enviar la oportunidad con el numero de solicitud
                if (Origen != null)
                {
                    vsMensaje += (char)10 + "Origen de Oportunidad:" + ((OptionSetValue)Origen).Value;
                }
                else
                {
                    vsMensaje += (char)10 + "No se tiene el origen del registro";
                    //Se le asigna el valor 1, para indicar que es naicdo en CRM
                    Origen = new OptionSetValue(1);
                }

                //VariablesRepository u = new VariablesRepository(_cnx);
                vsMensaje += (char)10 + "Fase oportunidad currentrecord  : " + currentrecord.StageId;
                vsMensaje += (char)10 + "Fase de la oportunidad dbRecord : " + dbRecord.StageId;
                {
                    vsMensaje += (char)10 + "Validando que no venga de Banner y esté en la fase correcta ";
                    if ((((OptionSetValue)Origen).Value != 3))
                    {
                        vsMensaje += (char)10 + "No viene de Banner y se ejecutó.";
                        vsMensaje += (char)10 + "Oportunidad from DB: " + dbRecord.Id;

                        var idbaner = dbRecord.ua_idbanner != null ? dbRecord.ua_idbanner : currentrecord.ua_idbanner;
                        var idbaner2 = currentrecord.ua_idbanner != null ? currentrecord.ua_idbanner : currentrecord.ua_idbanner;
                        vsMensaje += (char)10 + "Obteniendo id prospecto";

                        //vsMensaje +="Validando IdBanner de Oportunidad");
                        vsMensaje += (char)10 + "instanciando VariablesRepository";

                        {
                            vsMensaje += (char)10 + "Validando Origen de la Oportunidad";
                            var numsolicitud = dbRecord.ua_numero_solicitud != null ? dbRecord.ua_numero_solicitud : currentrecord.ua_numero_solicitud; //dbRecord.rs_numerosolicitud
                            vsMensaje += (char)10 + "Obteniendo periodo, programa y VPDI";
                            var periodo = dbRecord.ua_periodo != null ? dbRecord.ua_periodo : currentrecord.ua_periodo;
                            var programa = dbRecord.ua_programa_asesor != null ? dbRecord.ua_programa_asesor : currentrecord.ua_programa_asesor;
                            var vpdi = dbRecord.ua_codigo_vpd != null ? dbRecord.ua_codigo_vpd : currentrecord.ua_codigo_vpd;
                            vpdi = vpdi.Trim();
                            DateTime? FechaCreacion = dbRecord.CreatedOn != null ? dbRecord.CreatedOn : currentrecord.CreatedOn;
                            var Asesor = dbRecord.OwnerId != null ? dbRecord.OwnerId : currentrecord.OwnerId;
                            //Ticket 4543
                            var vsNivel = "";
                            var vsCampus = "";
                            try
                            {
                                vsMensaje += (char)10 + "Recuperando los datos Nivel y Campus del Contexto...";
                                vsMensaje += (char)10 + "Obteniendo el valor Campus...";
                                vsCampus = dbRecord.ua_codigo_campus.Name != null ? dbRecord.ua_codigo_campus.Name : null;
                                vsNivel = "";
                                vsMensaje += (char)10 + "Valor de ua_desc_nivel.Name: " + dbRecord.ua_desc_nivel.Name;
                                if (!string.IsNullOrWhiteSpace(dbRecord.ua_desc_nivel.Name))
                                {
                                    vsMensaje += (char)10 + "Obteniendo el valor Nivel...";
                                    vsNivel = u.obtenerNivel(dbRecord.ua_desc_nivel.Id);
                                }
                            }
                            catch
                            {
                                vsMensaje += (char)10 + "Entré al Catch de los datos Nivel-Campus, intentando recuperarlos mediante consulta...";
                                var guidCampus = new Guid();
                                var guidNivel = new Guid();
                                try
                                {
                                    guidCampus = u.getObtieneDatosOportunidad(dbRecord.Id, "ua_codigo_campus");
                                    guidNivel = u.getObtieneDatosOportunidad(dbRecord.Id, "ua_desc_nivel");
                                }
                                catch { }
                                vsMensaje += (char)10 + "Guids consultados...";
                                if (guidCampus != Guid.Empty)
                                {
                                    vsMensaje += (char)10 + "Obteniendo el valor Campus...";
                                    vsCampus = u.ObtenerCodigo(guidCampus, "businessunit", "businessunitid", "name");
                                    vsMensaje += (char)10 + "Valor Campus obtenido...";
                                }
                                if (guidNivel != Guid.Empty)
                                {
                                    vsMensaje += (char)10 + "Obteniendo el valor Nivel...";
                                    vsNivel = "LC";
                                    vsMensaje += (char)10 + "Valor Nivel obtenido...";
                                }
                                if (string.IsNullOrWhiteSpace(vsNivel)) vsNivel = "LC";
                                vsMensaje += (char)10 + "Datos Recuperados, Campus: " + vsCampus + ", Nivel: " + vsNivel;
                            }

                            if (periodo == null)
                            {
                                vsMensaje += (char)10 + "La oportunidad no tiene periodo asignado";
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Periodo");
                                return;
                            }

                            if (programa == null)
                            {
                                vsMensaje += (char)10 + "La oportunidad no tiene programa asignado";
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Programa");
                                return;
                            }

                            if (vpdi == null)
                            {
                                vsMensaje += (char)10 + "La oportunidad no tiene VPDI asignado";
                                return;
                            }


                            if (FechaCreacion == null)
                            {
                                vsMensaje += (char)10 + "La oportunidad no tiene Fecha de creacion";
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains Createon");
                                return;
                            }
                            if (Asesor == null)
                            {
                                vsMensaje += (char)10 + "La oportunidad no tiene un asesor asignado";
                                _cnx.context.SharedVariables.Add("AbortProcess", "It not Contains owner asign");
                                return;
                            }

                            #region Recuperacion de datos del CRM y validacion
                            vsMensaje += (char)10 + " recuperando variablesRepository";
                            vsMensaje += (char)10 + " mapeando la oportunidad";
                            var idopportunity = currentrecord.OpportunityId; // dbRecord.OpportunityId != null ? dbRecord.OpportunityId : currentrecord.OpportunityId;
                            //Objeta para mandarDatosBannerServicio 1
                            var CrearCuentBaner = new CrearCuentaBanner();
                            //Objeto mandar datos banner serivicio 33
                            var oportunidad = new AlmacenaIdOportunidad();

                            oportunidad.Id_Oportunidad = idopportunity.Value.ToString();
                            CrearCuentBaner.id_Oportunidad = idopportunity.Value.ToString();
                            oportunidad.Id_Banner = idbaner;


                            // CrearCuentBaner.Id_Banner_Vinculante = idbaner;
                            string scorreo = "";
                            if (Asesor != null)
                            {
                                vsMensaje += (char)10 + " Obteniendo Nombre y correo del asesor";
                                //Servicio 1
                                CrearCuentBaner.Nombre_Reclutador = u.GetDatosAsesor(Asesor.Id, out scorreo);
                                CrearCuentBaner.Correo_Reclutador = scorreo;
                                //Servicio 33
                                oportunidad.Nombre_Reclutador = CrearCuentBaner.Nombre_Reclutador;
                                oportunidad.Correo_Reclutador = CrearCuentBaner.Correo_Reclutador;
                            }

                            if (FechaCreacion != null)
                            {
                                vsMensaje += (char)10 + "Fecha de creacion";

                                var f = FechaCreacion.Value.Date;
                                vsMensaje += (char)10 + "Fecha  obtenida " + f;
                                CrearCuentBaner.Fecha_Creacion = FechaCreacion.Value.ToUniversalTime();
                                oportunidad.Fecha_Creacion = FechaCreacion.Value.ToUniversalTime();
                                vsMensaje += (char)10 + "Convertiendo a cultureinfo la fecha";
                            }

                            CrearCuentBaner.Origen = OrigenString;
                            oportunidad.Origen = OrigenString;


                            vsMensaje += (char)10 + "Obteniendo periodo de variables repository";
                            oportunidad.Periodo = u.ObtenerPeriodo(periodo.Id);
                            //CrearCuentaBanner
                            CrearCuentBaner.Periodo = oportunidad.Periodo;
                            vsMensaje += (char)10 + "Periodo encontrado " + oportunidad.Periodo;

                            vsMensaje += (char)10 + "Obteniendo programa de variables repository " + programa.Id;
                            oportunidad.Programa = u.ObtenerPrograma(programa.Id);
                            //crearCuenta
                            CrearCuentBaner.Programa = oportunidad.Programa;
                            vsMensaje += (char)10 + "Programa obtenido " + oportunidad.Programa;
                            //Codido de la escuela
                            vsMensaje += (char)10 + "validando que no sea nulo programa";
                            if (!string.IsNullOrWhiteSpace(CrearCuentBaner.Programa))
                            {
                                vsMensaje += (char)10 + "Obteniendo codigo escuela";
                                CrearCuentBaner.Escuela = u.CodigoEscuela(oportunidad.Programa);
                                oportunidad.Escuela = CrearCuentBaner.Escuela;
                            }

                            //CreateCuenta
                            vsMensaje += (char)10 + "Obteniendo vpdi de variables repository";
                            //CrearCuentBaner.Campus = vpdi;
                            vsMensaje += (char)10 + "Obteniendo vpdi de variables repository";
                            oportunidad.VPD = vpdi;
                            //CrearCuenta
                            CrearCuentBaner.VPD = vpdi;
                            vsMensaje += (char)10 + "Obteniendo IdCuenta de variables repository";
                            vsMensaje += (char)10 + "-----------------------------------";
                            //int a = 0;
                            //int b = 1 / a;
                            //var idcuent = u.GetIdCuenta(idbaner);
                            var idcuent = u.GetIdCuentaBYOportunidad(oportunidad.Id_Oportunidad);
                            vsMensaje += (char)10 + "Cuenta obtenida= " + idcuent.ToString();

                            if (numsolicitud == null)
                            {
                                oportunidad.Numero_Solicitud = 0;
                            }
                            else
                            {
                                oportunidad.Numero_Solicitud = numsolicitud.Value;

                            }

                            vsMensaje += (char)10 + "numero de solicitud " + oportunidad.Numero_Solicitud;

                            vsMensaje += (char)10 + "Validando Banner";
                            vsMensaje += (char)10 + "IDBANNER-----------" + idbaner;
                            if (!string.IsNullOrWhiteSpace(idbaner))
                            {
                                oportunidad.Id_Cuenta = idcuent;
                                oportunidad.Campus = vsCampus;
                                oportunidad.Nivel = vsNivel;

                                try
                                {
                                    vsMensaje += (char)10 + "Validando oportunity";
                                    oportunidad.Validate();
                                    vsMensaje += (char)10 + "Ejecutamos metodo de CrearOportunidad";
                                    u.CrearOportunidad(oportunidad);
                                    Thread.Sleep(500);
                                    string vsRetorno = u.enviaDatosDomicilioBanner(new Guid(CrearCuentBaner.id_Oportunidad), idbaner, CrearCuentBaner.VPD, CrearCuentBaner.Periodo);
                                    vsMensaje += (char)10 + vsRetorno;
                                    Thread.Sleep(500);
                                    vsMensaje += u.enviaDatosTelefonosBanner(new Guid(CrearCuentBaner.id_Oportunidad), idbaner, CrearCuentBaner.VPD, CrearCuentBaner.Periodo) + (char)10;

                                }
                                catch (Exception ex)
                                {
                                    vsMensaje += (char)10 + "Error al enviar datos a Banner mediante el servicio 33: " + ex.Message + (char)10;
                                    vsMensaje += (char)10 + "Periodo: " + periodo.Name + (char)10;
                                    vsMensaje += (char)10 + "Programa: " + programa.Name + (char)10;
                                    vsMensaje += (char)10 + "VPDI: " + vpdi + (char)10;
                                    vsMensaje += (char)10 + "Periodo: " + periodo.Name + (char)10;
                                    vsMensaje += (char)10 + "Error en la informacion a enviar: " + vsMensaje;
                                    u.enviaBitacoraBanner(vsMensaje);
                                    return;
                                }


                            }
                            else//Creamos cuenta y oportundad serivicio 1
                            {
                                vsMensaje += (char)10 + "*****ENTRAMOS  A EJECUTAR EL 1*****";
                                vsMensaje += (char)10 + "Obteniendo periodo de Sexo y FechaNacimiento de  repository";
                                DateTime FechaNA = default(DateTime);

                                CrearCuentBaner.Nivel = vsNivel;
                                CrearCuentBaner.Campus = vsCampus;

                                CrearCuentBaner.id_Cta = idcuent;

                                vsMensaje += (char)10 + "Obtenemos la fecha de nacimiento de esta cuenta " + CrearCuentBaner.id_Cta;
                                var Sexo = u.ObtenerSexoFechaNacimiento(CrearCuentBaner.id_Cta, out FechaNA);
                                vsMensaje += (char)10 + "-----------------------";

                                vsMensaje += (char)10 + "Convertiendo la fecha customFecha " + FechaNA;
                                CrearCuentBaner.Fecha_Nacimiento = new FechaNacimiento { Year = FechaNA.Year, Month = FechaNA.Month, Day = FechaNA.Day };
                                vsMensaje += (char)10 + "Fecha convertida " + CrearCuentBaner.Fecha_Nacimiento;

                                vsMensaje += (char)10 + "-----------GetSexo------------";
                                CrearCuentBaner.Sexo = Sexo == "2" ? "M" : "F";
                                vsMensaje += (char)10 + "Sexo " + CrearCuentBaner.Sexo;

                                vsMensaje += (char)10 + " Obteniendo el id del contacto principal de la Opportunidad " + CrearCuentBaner.id_Oportunidad;
                                //Datos basicos del contacto
                                //Guid idContactoPrincipalCuenta = u.ObtenerContactoPrincipalCuenta(new Guid(CrearCuentBaner.id_Cta));
                                Guid idContactoPrincipalCuenta = u.ObtenerProspectoDeLaOportunidad(new Guid(CrearCuentBaner.id_Oportunidad));
                                vsMensaje += (char)10 + "-----------idProspecto------------" + idContactoPrincipalCuenta;
                                vsMensaje += (char)10 + "-----------------------";

                                var dtbc = u.GetDatosBasicosProsByProspecto(idContactoPrincipalCuenta.ToString());
                                if (dtbc != null)
                                {

                                    CrearCuentBaner.Nombre = dtbc.NombreC;
                                    vsMensaje += (char)10 + "--------Nombre--------" + CrearCuentBaner.Nombre;
                                    CrearCuentBaner.Segundo_Nombre = dtbc.Segundo_NombreC;
                                    vsMensaje += (char)10 + "----Segundo Nombre------" + CrearCuentBaner.Segundo_Nombre;
                                    if (!string.IsNullOrWhiteSpace(dtbc.ApellidoPaternoC))
                                        CrearCuentBaner.ApellidoPaterno = dtbc.ApellidoPaternoC;
                                    vsMensaje += (char)10 + "----Apellido P------" + CrearCuentBaner.ApellidoPaterno;

                                    if (!string.IsNullOrWhiteSpace(dtbc.Apellido_MaternoC))
                                        CrearCuentBaner.Apellido_Materno = dtbc.Apellido_MaternoC;
                                    vsMensaje += (char)10 + "----Apellido M------" + CrearCuentBaner.Apellido_Materno;
                                    //vsMensaje +="----NumeroTelefono------" + dtbc.TelefonoC.Codigo_Area);
                                    if (dtbc.TelefonoC != null)
                                    {
                                        vsMensaje += (char)10 + "----NumeroTelefono------" + dtbc.TelefonoC.Numero_Telefono;
                                        CrearCuentBaner.Telefono = dtbc.TelefonoC;
                                    }
                                    vsMensaje += (char)10 + "----Correo------" + dtbc.CorreoC.Correo_Electronico;
                                    CrearCuentBaner.Correo = dtbc.CorreoC;

                                }

                                //Crearcuenta
                                try
                                {
                                    vsMensaje += (char)10 + "Validando oportunity";
                                    CrearCuentBaner.Validate();
                                    string idbanerRespues = u.CrearoCeuntaYoportundiad(CrearCuentBaner, idContactoPrincipalCuenta);
                                    vsMensaje += (char)10 + "idbanenr regresado";
                                    dbRecord.ua_idbanner = idbanerRespues;
                                    vsMensaje += (char)10 + "idbanenr actualizar " + dbRecord.ua_idbanner;
                                    _cnx.service.Update(dbRecord);
                                    vsMensaje += (char)10 + "Actualizo el idbanner de la oportundiad " + dbRecord.Id;
                                    vsMensaje += (char)10 + "Consultando idbanner prospecto";
                                    var cuent = new Account();
                                    cuent.AccountId = new Guid(CrearCuentBaner.id_Cta);
                                    cuent.ua_idbanner = idbanerRespues;
                                    vsMensaje += (char)10 + "Actualizando el objeto Cuenta...";
                                    _cnx.service.Update(cuent);
                                    vsMensaje += (char)10 + "Se actualizó la cuenta.";
                                    vsMensaje += u.enviaDatosTelefonosBanner(new Guid(CrearCuentBaner.id_Oportunidad), cuent.ua_idbanner, CrearCuentBaner.VPD, CrearCuentBaner.Periodo) + (char)10;
                                    Thread.Sleep(500);
                                    string vsRetorno = u.enviaDatosDomicilioBanner(new Guid(CrearCuentBaner.id_Oportunidad), cuent.ua_idbanner, CrearCuentBaner.VPD, CrearCuentBaner.Periodo);
                                    vsMensaje += (char)10 + vsRetorno;
                                    Thread.Sleep(500);
                                    //vsMensaje += u.enviaDatosTelefonosBanner(new Guid(CrearCuentBaner.id_Oportunidad), cuent.ua_idbanner, CrearCuentBaner.VPD, CrearCuentBaner.Periodo) + (char)10;
                                }
                                catch (BusinessTypeException ex)
                                {
                                    vsMensaje += (char)10 + "Error al enviar datos a Banner mediante los servicioc 1, 38 o 39: " + ex.Message + (char)10;
                                    vsMensaje += (char)10 + JsonConvert.SerializeObject(CrearCuentBaner);
                                    vsMensaje += (char)10 + "Error en la informacion a enviar:" + ex.Message;
                                    u.enviaBitacoraBanner(vsMensaje);
                                }

                            }
                            #endregion
                        }
                        #endregion
                    }//Fin del if==
                }//Fin del if null
                _cnx.trace.Trace(vsMensaje);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                vsMensaje += (char)10 + "The application terminated with an error.";
                vsMensaje += (char)10 + "Timestamp: " + ex.Detail.Timestamp;
                vsMensaje += (char)10 + "Code: " + ex.Detail.ErrorCode;
                vsMensaje += (char)10 + "Message: " + ex.Detail.Message;
                vsMensaje += (char)10 + (string.Format("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault"));
                u.enviaBitacoraBanner(vsMensaje);
                _cnx.trace.Trace(vsMensaje);
                throw new Exception("Error: ", ex);
            }
            catch (System.TimeoutException ex)
            {
                vsMensaje += (char)10 + "The application terminated with an error.";
                vsMensaje += (char)10 + "Message: " + ex.Message;
                vsMensaje += (char)10 + "Stack Trace: " + ex.StackTrace;
                u.enviaBitacoraBanner(vsMensaje);
                _cnx.trace.Trace(vsMensaje);
                throw new Exception("Error: ", ex);
            }
            catch (System.Exception ex)
            {
                vsMensaje += (char)10 + "The application terminated with an error.";
                vsMensaje += (char)10 + ex.Message;
                if (ex.InnerException != null)
                {
                    vsMensaje += (char)10 + ex.InnerException.Message;

                    FaultException<OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<OrganizationServiceFault>;
                    if (fe != null)
                    {
                        vsMensaje += (char)10 + "Timestamp: " + fe.Detail.Timestamp;
                        vsMensaje += (char)10 + "Code: " + fe.Detail.ErrorCode;
                        vsMensaje += (char)10 + "Message: " + fe.Detail.Message;
                        vsMensaje += (char)10 + "Trace: " + fe.Detail.TraceText;
                    }
                }
                u.enviaBitacoraBanner(vsMensaje);
                _cnx.trace.Trace(vsMensaje);
                throw new Exception("Error: ", ex);
            }

        }
    }
}
