using System;
using System.Linq;
using System.Web.Services;
using ws_combugasclientes.core;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace ws_combugasclientes.ws
{
    [WebService(Namespace = "awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class traspasos : WebService
    {

        #region CLASES
        public class EntTotalesVend
        {
            public string vendedor { get; set; }
            public double cilindros_30kg { get; set; }
            public double cilindros_45kg { get; set; }
            public double garrafones_awa { get; set; }
            public double garrafones_alk { get; set; }
        }
        #endregion
        [WebMethod]
        public decimal obtenerPrecioProducto(int id_producto)
        {
            decimal precio = 0;
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                precio = (decimal)(from producto in context.producto
                                   where producto.id_producto == id_producto
                                   select producto.precio).FirstOrDefault();
            }
            catch (Exception ex)
            {
                precio = -1;
            }
            return precio;
        }

        [WebMethod]
        public void actualizarCarburacion(string estacion, string direccion, double latitud, double longitud, int id_estacion)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                estaciones_carburacion carburacion = context.estaciones_carburacion.Where(x => x.id_ec == id_estacion).FirstOrDefault();
                carburacion.ec_descripcion = estacion;
                carburacion.ec_direccion = direccion;
                carburacion.ec_latitud = latitud;
                carburacion.ec_longitud = longitud;
                context.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToUpper());
            }
        }

        [WebMethod]
        public ajaxResponse obtenerVentasMesCC(int anio, int mes)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var ventas_cc = (from i in context.sp_ventasMesCC(anio, mes) select i).ToList();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(ventas_cc);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse AppFechas(string fecha_ini, string fecha_fin)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                DateTime fechaIni = DateTime.Parse(fecha_ini);
                DateTime fechaFin = DateTime.Parse(fecha_fin);
                var totales_app = (from i in context.sp_pedidos_app_fechas(fechaIni, fechaFin) select i).ToList();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(totales_app);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse WhatsappFechas(string fecha_ini, string fecha_fin)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                DateTime fechaIni = DateTime.Parse(fecha_ini);
                DateTime fechaFin = DateTime.Parse(fecha_fin);
                var totales_app = (from i in context.sp_pedidos_whatsapp_fechas(fechaIni, fechaFin) select i).ToList();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(totales_app);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse AppAnual(string anio)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var totales_app = (from i in context.sp_pedidos_app_anual(anio) select i).ToList();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(totales_app);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse WhatsappAnual(string anio)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                var totales_app = (from i in context.sp_pedidos_whatsapp_anual(anio) select i).ToList();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(totales_app);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse TotalesCC(DateTime fecha_ini, DateTime fecha_fin, int planta, int departamento)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            List<EntTotalesVend> items = new List<EntTotalesVend>();
            try
            {
                int? idPlanta = null;
                int? TipoDep = null;
                if (planta > 0)
                {
                    idPlanta = planta;
                }
                if (departamento > 0)
                {
                    TipoDep = departamento;
                }

                var operadores = (from x in context.sp_DistinctOperador(fecha_ini, fecha_fin, idPlanta, TipoDep) select x.id_operador).ToList();

                for (int i = 0; i < operadores.Count(); i++)
                {
                    int? id_operador = operadores[i];

                    int? no_empleado = (from o in context.operador where o.id_operador == id_operador select o.no_empleado).FirstOrDefault();
                    string nombre = (from o in context.operador where o.id_operador == id_operador select o.nombre).FirstOrDefault();
                    string apellidop = (from o in context.operador where o.id_operador == id_operador select o.apellidoP).FirstOrDefault();
                    string apellidom = (from o in context.operador where o.id_operador == id_operador select o.apellidoM).FirstOrDefault();

                    string vendedor = string.Format("{0} - {1} {2} {3}", no_empleado, nombre, apellidop, apellidom);

                    double cil30kg = (from p in context.sp_totalProductosOperador(2, fecha_ini, fecha_fin, idPlanta, id_operador) select p.cantidad).FirstOrDefault();

                    double cil45kg = (from p in context.sp_totalProductosOperador(3, fecha_ini, fecha_fin, idPlanta, id_operador) select p.cantidad).FirstOrDefault();

                    double awa = (from p in context.sp_totalProductosOperador(4, fecha_ini, fecha_fin, idPlanta, id_operador) select p.cantidad).FirstOrDefault();

                    double alk = (from p in context.sp_totalProductosOperador(7, fecha_ini, fecha_fin, idPlanta, id_operador) select p.cantidad).FirstOrDefault();

                    EntTotalesVend item = new EntTotalesVend();
                    item.vendedor = vendedor;
                    item.cilindros_30kg = cil30kg;
                    item.cilindros_45kg = cil45kg;
                    item.garrafones_awa = awa;
                    item.garrafones_alk = alk;
                    items.Add(item);
                }

                var json = new JavaScriptSerializer().Serialize(items);
                response.Result = true;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public string validaVehiculoActivo(string no_vehiculo)
        {
            string respuesta = "";
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                var vehiculo = context.truck.Where(x => x.numero == no_vehiculo && x.status.Equals(true)).FirstOrDefault();
                if (vehiculo != null)
                {
                    var asignacion = context.Asignaciones.Where(x => x.id_truck == vehiculo.id_truck && x.asignacion_activa.Equals(true)).ToList();
                    if (asignacion.Count > 0)
                    {
                        respuesta = "SIVEHICULO";
                    }
                    else
                    {
                        respuesta = "El vehículo no se encuentra en circulación";
                    }
                }
                else
                {
                    respuesta = "El vehículo no existe o no está fuera de actividad";
                }
            }
            catch (Exception ex)
            {
                respuesta = ex.Message.ToUpper();
            }
            return respuesta;
        }
    }
}