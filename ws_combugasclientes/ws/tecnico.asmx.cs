using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using ws_combugasclientes.core;

namespace ws_combugasclientes.ws
{
    [WebService(Namespace = "awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class tecnico : WebService
    {
        [WebMethod]
        public ajaxResponse LoginTecnico(string usuario, string contrasenia)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                var objTecnico = from op in context.operador
                                 where op.username == usuario && op.pass == contrasenia &&
                                 op.tipo_operador.accesoapp == true && op.status == true
                                 && op.id_tipo_operador == 7
                                 select new
                                 {
                                     op.id_operador,
                                     op.no_empleado,
                                     op.nombre,
                                     op.apellidoP,
                                     op.apellidoM
                                 };

                if (objTecnico != null && objTecnico.Count() > 0)
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(objTecnico);
                    Response.Result = true;
                    Response.Message = "OKLOGIN";
                    Response.Data = json;
                }
                else
                {
                    Response.Result = false;
                    Response.Message = "El usuario y/o contraseña son incorrectos";
                    Response.Data = null;
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse obtenerReportesPendientes(int id_operador)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var fugas_tecnico = from i in context.sp_fugas_tecnico(id_operador) select i;
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(fugas_tecnico);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = json;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse obtenerReportesTerminados(int id_operador)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var fugas_tecnico = from i in context.sp_fugas_fin_tecnico(id_operador) select i;
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(fugas_tecnico);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = json;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse CerrarReporte(int id_fuga, string pesoTanque, string pesoTanqueSalida, string fechaTanque, string fechaTanqueSalida, string folioSalida, string folioTecnico, bool CargoRepartidor, int? id_tecnico_entrega)
        {
            ajaxResponse Response = new ajaxResponse();
            try
            {
                Quejas_Fugas obj_qf = new Quejas_Fugas();
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                context.CommandTimeout = 0;
                obj_qf = context.Quejas_Fugas.Where(x => x.Id_fugas == id_fuga).SingleOrDefault();
                if (obj_qf != null)
                {
                    if (obj_qf.fecha_solucion != null && obj_qf.fecha_asignacion != null)
                    {
                        obj_qf.status_reporte = false;
                        obj_qf.persona_confirma = "";
                        obj_qf.fecha_entrada_tanque = convierteAFecha(fechaTanque);
                        obj_qf.fecha_salida_tanque = convierteAFecha(fechaTanqueSalida);
                        obj_qf.tanqPeso = pesoTanque;
                        obj_qf.tanqPesoSalida = pesoTanqueSalida;
                        obj_qf.user_finaliza = 10;
                        obj_qf.fecha_confirmacion_reporte = DateTime.Now.Date;
                        obj_qf.hora_confirmacion_reporte = DateTime.Now.TimeOfDay;
                        obj_qf.bit_cargoRepartidor = CargoRepartidor;
                        obj_qf.folio_salida = folioSalida;
                        obj_qf.folio_tecnico = folioTecnico;
                        obj_qf.id_tecnico_entrega = id_tecnico_entrega;

                        context.SubmitChanges();
                        Response.Result = true;
                        Response.Message = "OKCIERRE";
                        Response.Data = null;
                    }
                    else
                    {
                        if (obj_qf.fecha_asignacion == null)
                        {
                            Response.Message = "El reporte no tiene asignado un técnico que revisó la falla";
                        }
                        if (obj_qf.id_falla_fuga == null)
                        {
                            Response.Message = "El reporte aún no tiene registrada una solución por parte del técnico";
                        }
                        Response.Result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse obtenerFallas()
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var objFallas = from ff in context.fallas_fugas
                                where ff.status == true
                                select ff;

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(objFallas);
                Response.Result = true;
                Response.Message = "OKFALAS";
                Response.Data = json;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse obtenerFotosFuga(int id_fuga)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var fotosf = (from f in context.fotos_fuga
                              join t in context.operador on f.id_usuario equals t.id_operador
                              where f.id_fuga == id_fuga
                              orderby f.fecha_captura descending
                              select new
                              {
                                  f.ruta,
                                  f.extension,
                                  f.fecha_captura,
                                  t.no_empleado,
                                  t.nombre,
                                  t.apellidoP,
                                  t.apellidoM
                              });

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(fotosf);
                Response.Result = true;
                Response.Message = "OKFOTOS";
                Response.Data = json;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        [WebMethod]
        public int registrarFoto(int id_fuga, int id_tecnico, string extension)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                string ruta = "http://cgtng.sytes.net:8033/cargafugas/";
                fotos_fuga fotosf = new fotos_fuga();
                fotosf.id_fuga = id_fuga;
                fotosf.id_usuario = id_tecnico;
                fotosf.extension = extension;
                fotosf.fecha_captura = DateTime.Now;
                context.fotos_fuga.InsertOnSubmit(fotosf);
                context.SubmitChanges();

                var updateRuta = context.fotos_fuga.Where(x => x.id_foto_fuga == fotosf.id_foto_fuga).FirstOrDefault();
                ruta += $"{fotosf.id_foto_fuga}.{extension}";
                updateRuta.ruta = ruta;
                context.SubmitChanges();

                return fotosf.id_foto_fuga;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        [WebMethod]
        public void eliminarEvidencia(int id_foto_fuga)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            var fotoFuga = context.fotos_fuga.Where(x => x.id_foto_fuga == id_foto_fuga).FirstOrDefault();
            context.fotos_fuga.DeleteOnSubmit(fotoFuga);
            context.SubmitChanges();
        }

        [WebMethod]
        public ajaxResponse solucionReporte(int id_reporte_fuga, string reporte_tecnico, string solucion_diagnostico, int? id_falla_confirmada, string falla_confirmada_texto, bool? tanq10, bool? revision_planta, bool? taponBronce, int? tecnico_soluciona, int tipo_de_queja, bool? noGente, bool? clienteCancela)
        {
            ajaxResponse Response = new ajaxResponse();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                context.CommandTimeout = 0;
                Quejas_Fugas obj_quejas = new Quejas_Fugas();
                obj_quejas = context.Quejas_Fugas.Where(x => x.Id_fugas == id_reporte_fuga).SingleOrDefault();
                if (obj_quejas != null)
                {
                    if (obj_quejas.fecha_asignacion != null)
                    {
                        obj_quejas.reporte_tecnico = reporte_tecnico;
                        obj_quejas.solucion_diagnostico = solucion_diagnostico;
                        obj_quejas.id_falla_fuga = id_falla_confirmada;
                        obj_quejas.falla_confirmada = falla_confirmada_texto;
                        obj_quejas.id_tecnico_soluciono = tecnico_soluciona;
                        obj_quejas.tanq10 = tanq10;
                        obj_quejas.tanqRevisado = revision_planta;
                        obj_quejas.taponBronce = taponBronce;
                        obj_quejas.noGente = noGente;
                        obj_quejas.clienteCancela = clienteCancela;
                        obj_quejas.fecha_solucion = DateTime.Now.Date;
                        obj_quejas.hora_solucion = DateTime.Now.TimeOfDay;
                        obj_quejas.tipo_queja = tipo_de_queja;

                        context.SubmitChanges();
                        Response.Message = "OKSOLUCION";
                        Response.Result = true;
                    }
                    else
                    {
                        Response.Message = "No puede registrar una solución sin asignar un técnico al reporte";
                        Response.Result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = string.Format("EXCEPTION: {0}", ex.Message.ToUpper());
                Response.Data = null;
            }
            return Response;
        }

        private static DateTime? convierteAFecha(string strFecha)
        {
            try
            {
                DateTime fechacomparativo = DateTime.ParseExact(strFecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return fechacomparativo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
