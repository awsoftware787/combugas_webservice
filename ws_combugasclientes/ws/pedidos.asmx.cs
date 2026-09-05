using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.Services;
using ws_combugasclientes.core;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using System.Web.Script.Services;

namespace ws_combugasclientes.ws
{
    [WebService(Namespace = "awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class pedidos : WebService
    {
        private const string TOKEN = "5a0b24d5f6a5f769e92dd0be";
        //private static string URL = "https://awsoftware.goldenm.solutions/data/get?token=" + TOKEN + "&method=datosactuales&filter=nroSerie&values=";
        private static string URL = "https://awsoftware.wscomdata.com/api/v2/?token=MAXGASOJRYJPPHHCKIIJPSFMDOHMZIOWDKXLQFUNGYRIGFWJJMUFZSNNKMPPHVBUAHVNOTA&filter=noserie&values=";

        [WebMethod]
        public ajaxResponse getPrecios()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            using (ContextCombugasDataContext context = new ContextCombugasDataContext())
            {
                try
                {
                    var montos = (from i in context.montominimo_estacionario
                                  where i.id == 1
                                  select new
                                  {
                                      i.montominimo_dinero,
                                      i.montominimo_litros
                                  }).FirstOrDefault();

                    decimal monto_minimo = montos == null ? 0m : (decimal)montos.montominimo_dinero;
                    decimal litros_minimo = montos == null ? 0m : (decimal)montos.montominimo_litros;

                    // El LEFT JOIN conserva productos históricos que aún no tienen tipo asignado.
                    // Un status nulo se considera disponible; solo se omiten productos
                    // marcados explícitamente como inactivos.
                    // La consulta se materializa una sola vez para evitar consultas por producto.
                    var productos = (from producto in context.producto
                                     join tipoProducto in context.tipo_producto
                                         on producto.id_tipo_producto equals (int?)tipoProducto.id_tipo_producto
                                         into tiposProducto
                                     from tipoProducto in tiposProducto.DefaultIfEmpty()
                                     where (producto.status != null && producto.status == true)
                                     && producto.visible_app == true
                                     orderby producto.id_servicio,
                                             producto.id_tipo_producto,
                                             producto.id_producto
                                     select new
                                     {
                                         producto.id_producto,
                                         producto.id_servicio,
                                         producto.id_tipo_producto,
                                         tipo_producto = tipoProducto == null ? null : tipoProducto.descripcion,
                                         producto.descripcion,
                                         producto.precio,
                                         producto.url_icono
                                     }).ToList();

                    if (productos.Any())
                    {
                        var precios = productos.Select(producto => new
                        {
                            _idProducto = producto.id_producto,
                            _idServicio = producto.id_servicio,
                            _idTipoProducto = producto.id_tipo_producto,
                            _tipoProducto = producto.tipo_producto,
                            _descripcionProducto = producto.descripcion,
                            _precioProducto = producto.precio,
                            _urlIcono = ObtenerUrlPublicaIcono(producto.url_icono),
                            _montoMinimoEst = monto_minimo,
                            _litroMinimoEst = litros_minimo
                        }).ToList();

                        Response.Result = true;
                        Response.Message = "PROD";
                        Response.Data = jsonSerializer.Serialize(precios);
                    }
                    else
                    {
                        Response.Result = false;
                        Response.Message = "NOPROD";
                        Response.Data = jsonSerializer.Serialize("NO SE ENCONTRARON PRODUCTOS");
                    }
                }
                catch (Exception ex)
                {
                    var jsonNoProd = jsonSerializer.Serialize("ERROR :" + ex.Message);
                    Response.Result = false;
                    Response.Message = "NOPROD";
                    Response.Data = jsonNoProd;
                }
            }
            return Response;
        }

        private string ObtenerUrlPublicaIcono(string urlIcono)
        {
            string urlPublica = ConfigurationManager.AppSettings["UrlPublica"];
            string iconoPredeterminado = ConfigurationManager.AppSettings["IconoProductoPredeterminado"];
            HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;

            // Compatibilidad con la configuración histórica que apunta al sitio que hospeda /Images.
            if (string.IsNullOrWhiteSpace(urlPublica))
            {
                urlPublica = string.Concat(
                    ConfigurationManager.AppSettings["URLSITIO"],
                    ConfigurationManager.AppSettings["PUERTOSITIO"]);
            }

            return ProductoIconUrlBuilder.Build(
                urlIcono,
                urlPublica,
                request == null ? null : request.Url,
                request == null ? null : request.ApplicationPath,
                iconoPredeterminado);
        }

        [WebMethod]
        public ajaxResponse getMontosMinimos()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var consulta = context.montominimo_estacionario.Where(x => x.id.Equals(1)).ToList();
                var resultSQL = from i in consulta
                                select new
                                {
                                    i.montominimo_dinero,
                                    i.montominimo_litros
                                };

                var jsonProd = jsonSerializer.Serialize(resultSQL);
                Response.Result = true;
                Response.Message = "MONTOSMINIMOS";
                Response.Data = jsonProd;
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("ERROR: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERRORMONTOSMINIMOS";
                Response.Data = jsonError;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse getMontosMinimosIOS()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                var montos = from i in context.montominimo_estacionario
                             where i.id == 1
                             select new
                             {
                                 i.montominimo_dinero,
                                 i.montominimo_litros
                             };

                var jsonProd = jsonSerializer.Serialize(montos);
                Response.Result = true;
                Response.Message = "MONTOSMINIMOSIOS";
                Response.Data = jsonProd;
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("ERROR: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERRORMONTOSMINIMOSIOS";
                Response.Data = jsonError;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse getTiemposFases()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var tiempos = (
                    from SQLTiempo in context.tiempos_fases
                    where SQLTiempo.tf_fase.Contains("ENTREGA")
                    select new
                    {
                        _idTF = SQLTiempo.id_tiempo_fases,
                        _faseTF = SQLTiempo.tf_fase,
                        _tiempoTF = SQLTiempo.tf_tiempo,
                        _unidad = SQLTiempo.tf_unidad
                    }
                );

                if (tiempos != null)
                {
                    var jsonProd = jsonSerializer.Serialize(tiempos);
                    Response.Result = true;
                    Response.Message = "TIEMPO";
                    Response.Data = jsonProd;
                }
                else
                {
                    var jsonProd = jsonSerializer.Serialize("NO HAY RESULTADOS");
                    Response.Result = true;
                    Response.Message = "NOTIEMPO";
                    Response.Data = jsonProd;
                }
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("ERROR: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERRORTF";
                Response.Data = jsonError;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse getPedidos(int _intIdCliente)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            try
            {
                var pedidos = (
                    from SQLPedidos in context.Pedido
                    where SQLPedidos.Id_Cliente.Equals(_intIdCliente)

                    select new
                    {
                        _idPedido = SQLPedidos.Id_Pedido,
                        _fechaPedido = SQLPedidos.fecha_creacion.ToShortDateString(),
                        _horaPedido = string.Format("{0:t}", SQLPedidos.hora_creacion).Substring(0, 8),
                        _direccion = new
                        {
                            _idDireccion = SQLPedidos.Direccion.id_direccion,
                            _descrDireccion = SQLPedidos.Direccion.Descripcion_Direccion.FirstOrDefault().descripcion,
                            _descr_tipo_calle = SQLPedidos.Direccion.calles.tipo_calle.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _descr_calle = SQLPedidos.Direccion.calles.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _id_cerrada = SQLPedidos.Direccion.id_segmento,
                            _descripcion_cerrada = SQLPedidos.Direccion.Segmento_Colonia1.descripcion.Replace("\r\n", "").Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _no_interior = SQLPedidos.Direccion.no_interior,
                            _no_exterior = SQLPedidos.Direccion.no_exterior,
                            _descr_colonia = SQLPedidos.Direccion.colonias.tipo_asentamiento.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "") + " " + SQLPedidos.Direccion.colonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _descr_ciudad = SQLPedidos.Direccion.ciudades.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _descr_estado = SQLPedidos.Direccion.estados.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            _descr_cp = "",
                            _latitud = SQLPedidos.Direccion.latitud,
                            _longitud = SQLPedidos.Direccion.longitud,
                        },
                        _detallePedido = (
                            from SQLDetalle in SQLPedidos.Pedido_Detalle
                            select new
                            {
                                _idProducto = (SQLDetalle.id_producto_confirmado == null) ? SQLDetalle.Id_producto : SQLDetalle.id_producto_confirmado,
                                _descrProducto = (SQLDetalle.id_producto_confirmado == null) ? SQLDetalle.producto.descripcion : (from SQLProducto in context.producto where SQLProducto.id_producto.Equals(SQLDetalle.id_producto_confirmado) select SQLProducto.descripcion).FirstOrDefault(),
                                _detCantidad = SQLDetalle.cantidad,
                                _detImporte = SQLDetalle.importe
                            }
                        ),
                        _idMetodoPago = SQLPedidos.Id_Metodo_Pago,
                        _descrMetodoPago = SQLPedidos.Metodo_Pago.descripcion,
                        _estatusPedido = SQLPedidos.status_pedido,
                        _completo = SQLPedidos.Completo,
                        _confirmadoOperador = SQLPedidos.Pedido_confirmado_operador,
                        _confirmadoCliente = SQLPedidos.Pedido_confirmado_cliente
                    }
                ).OrderByDescending(x => x._idPedido).ToList();

                if (pedidos.Count > 0)
                {
                    var listaPedidos = (dynamic)null;
                    if (pedidos.Count > 10)
                    {
                        listaPedidos = pedidos.Take(10);
                    }
                    else
                    {
                        listaPedidos = pedidos;
                    }
                    var jsonPed = jsonSerializer.Serialize(listaPedidos);
                    Response.Result = true;
                    Response.Message = "OKPED";
                    Response.Data = jsonPed;
                }
                else
                {
                    var jsonPed = jsonSerializer.Serialize(pedidos);
                    Response.Result = true;
                    Response.Message = "NOPED";
                    Response.Data = jsonPed;
                }
            }
            catch (Exception ex)
            {
                var jsonProd = jsonSerializer.Serialize("Ha ocurrido un error: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonProd;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse getUnPedido(int _intIdPedido)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var pedido = (
                    from SQLPedido in context.Pedido
                    where SQLPedido.Id_Pedido.Equals(_intIdPedido)
                    select new
                    {
                        _idDireccion = SQLPedido.Direccion.id_direccion,
                        _descrDireccion = SQLPedido.Direccion.Descripcion_Direccion.FirstOrDefault().descripcion,
                        _descr_tipo_calle = SQLPedido.Direccion.calles.tipo_calle.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _descr_calle = SQLPedido.Direccion.calles.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _id_cerrada = SQLPedido.Direccion.id_segmento,
                        _descripcion_cerrada = SQLPedido.Direccion.Segmento_Colonia1.descripcion.Replace("\r\n", "").Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _no_interior = SQLPedido.Direccion.no_interior,
                        _no_exterior = SQLPedido.Direccion.no_exterior,
                        _descr_colonia = SQLPedido.Direccion.colonias.tipo_asentamiento.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "") + " " + SQLPedido.Direccion.colonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _descr_ciudad = SQLPedido.Direccion.ciudades.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _descr_estado = SQLPedido.Direccion.estados.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _descr_cp = SQLPedido.Direccion.cp.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _latitud = SQLPedido.Direccion.latitud,
                        _longitud = SQLPedido.Direccion.longitud,
                        _asignacion = (
                            from SQLAsignacion in SQLPedido.Asigna_Pedido
                            where SQLAsignacion.id_pedido.Equals(_intIdPedido)
                            select new
                            {
                                _idOperador = SQLAsignacion.id_operador,
                                _nombreOperador = SQLAsignacion.operador.nombre + " " + SQLAsignacion.operador.apellidoP + " " + SQLAsignacion.operador.apellidoM,
                                _idRuta = SQLAsignacion.id_ruta,
                                _Vehiculo = (
                                    from SQLVehiculo in SQLAsignacion.Rutas.Asignaciones
                                    let pocisiones = getVehiculoPoscion(SQLVehiculo.truck.no_serie)
                                    where SQLVehiculo.asignacion_activa.Equals(true)
                                    select new
                                    {
                                        _numeroVehiculo = SQLVehiculo.truck.numero,
                                        _descrVehiculo = SQLVehiculo.truck.marca + " " + SQLVehiculo.truck.modelo,
                                        _latitudVehiculo = pocisiones.Lt,
                                        _longitudVehiculo = pocisiones.Lg
                                    }
                                ).FirstOrDefault()
                            }
                        )

                    }
                ).FirstOrDefault();

                if (pedido != null)
                {
                    var jsonPed = jsonSerializer.Serialize(pedido);
                    Response.Result = true;
                    Response.Message = "OKPED";
                    Response.Data = jsonPed;
                }
                else
                {
                    var jsonPed = jsonSerializer.Serialize(pedido);
                    Response.Result = true;
                    Response.Message = "NOPED";
                    Response.Data = jsonPed;
                }
            }
            catch (Exception ex)
            {
                var jsonPed = jsonSerializer.Serialize("Ha ocurrido un error: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonPed;
            }
            return Response;
        }

        [WebMethod]
        public Loc getVehiculoPoscion(string noSerie)
        {
            Loc pocision = new Loc();
            var jsonSerializer = new JavaScriptSerializer();
            string urlcomdata = URL + noSerie;
            try
            {
                #region consulta a comdata
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlcomdata);
                WebResponse response = request.GetResponse();
                string jsonVehiculosCD = String.Empty;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    jsonVehiculosCD = reader.ReadToEnd();
                }
                var results = JsonConvert.DeserializeObject<dynamic>(jsonVehiculosCD);
                int i = 0;
                float lat = 0;
                float lon = 0;
                foreach (var item in results)
                {

                    lat = results[i].posinfo.latitude;
                    lon = results[i].posinfo.longitude;

                }
                #endregion

                if (lat > 0)
                {
                    pocision.Lt = lat;
                    pocision.Lg = lon;
                }
                else
                {
                    pocision.Lt = 0;
                    pocision.Lg = 0;
                }
            }
            catch (Exception)
            {
                pocision.Lt = 0;
                pocision.Lg = 0;
            }
            return pocision;
        }

        [WebMethod]
        public ajaxResponse getMotivosDeCancelacion()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var motivoscancelacion = (
                    from SQLMotivosCancelacion in context.Motivo_Cancelacion
                    where SQLMotivosCancelacion.status.Equals(true)
                    select new
                    {
                        SQLMotivosCancelacion.id_Motivo,
                        SQLMotivosCancelacion.descripcion
                    }
                ).ToList();
                if (motivoscancelacion.Count > 0)
                {
                    var jsonMot = jsonSerializer.Serialize(motivoscancelacion);
                    Response.Result = true;
                    Response.Message = "MOT";
                    Response.Data = jsonMot;
                }
                else
                {
                    var jsonMot = jsonSerializer.Serialize("");
                    Response.Result = true;
                    Response.Message = "NOMOT";
                    Response.Data = jsonMot;
                }
            }
            catch (Exception ex)
            {
                var jsonMot = jsonSerializer.Serialize("error: " + ex.Message);
                Response.Result = true;
                Response.Message = "ERRMOT";
                Response.Data = jsonMot;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse calificarServicio(bool _blEntregado, string _strPuntuacion, string _strComentarios, int _intPedido, int _intCliente)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                calificaciones_app nuevapuntuacion = new calificaciones_app();
                nuevapuntuacion.calif_entregado = _blEntregado;
                nuevapuntuacion.calif_puntuacion = Double.Parse(_strPuntuacion);
                nuevapuntuacion.calif_comentarios = _strComentarios;
                nuevapuntuacion.calif_ref_pedido = _intPedido;
                nuevapuntuacion.calif_cliente = _intCliente;
                nuevapuntuacion.calif_fecha = DateTime.Now;
                context.calificaciones_app.InsertOnSubmit(nuevapuntuacion);
                context.SubmitChanges();

                Response.Result = true;
                Response.Message = "CALIF";
            }
            catch (Exception EX)
            {
                Response.Result = false;
                Response.Message = "CALIFERR";
            }
            return Response;
        }

        [WebMethod]
        public string testWS()
        {
            return "OK";
        }

        [WebMethod]
        public ajaxResponse cancelarPedido(int _intIdPedido)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Pedido pedido = (
                    from SQLPedido in context.Pedido
                    where SQLPedido.Id_Pedido.Equals(_intIdPedido)
                    select SQLPedido
                ).FirstOrDefault();
                if (pedido != null)
                {
                    pedido.status_pedido = false;
                    pedido.Id_Cancelacion = 16;
                    pedido.fase_cancelacion = "RADIOS";
                    context.SubmitChanges();

                    cancelaciones_pedidos motivo = new cancelaciones_pedidos();
                    motivo.id_motivo = 16;
                    motivo.id_usuario = 10;
                    motivo.fecha = DateTime.Now.Date;
                    motivo.tiempo = DateTime.Now.TimeOfDay;
                    motivo.motivo_texto = "CANCELADO POR EL CLIENTE DESDE APP MOVIL";
                    motivo.id_pedido = _intIdPedido;
                    context.cancelaciones_pedidos.InsertOnSubmit(motivo);
                    context.SubmitChanges();

                    #region valida que no cancele demasiado
                    Cliente_app cliente = (
                        from SQLCliente in context.Cliente_app
                        where SQLCliente.id_cliente.Equals(pedido.Id_Cliente)
                        select SQLCliente
                    ).FirstOrDefault();

                    if (cliente != null)
                    {
                        int cantidadcancelados = cliente.cont_cancelados + 1;
                        if (cantidadcancelados == 4)
                        {
                            cliente.bloqueado = true;
                            cliente.cont_cancelados = 0;
                            cliente.motivo_bloqueado = "CANCELACIONES REITERADAS";
                            Response.Message = "LOCK";
                        }
                        else
                        {
                            cliente.cont_cancelados = cantidadcancelados;
                            Response.Message = "CANCELADO";
                        }
                    }
                    context.SubmitChanges();
                    #endregion
                    Response.Result = true;
                    Response.Data = "";
                }
                else
                {
                    Response.Result = false;
                    Response.Message = "NOCANCELADO";
                    Response.Data = "";
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = "NOCANCELADO";
                Response.Data = "";
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse validaSalvarPedido(int _intIdDireccion, int _intIdCliente, int _intIdTelefono, int _intIdMetodoPago, string _strDetallePedido, string _observacionesPedido = null)
        {
            ajaxResponse Response = new ajaxResponse();

            #region validacion de hora en la que se intenta generar el pedido
            TimeSpan start = new TimeSpan(7, 0, 0);
            TimeSpan end = new TimeSpan(20, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            bool permitir = false;
            if ((now > start) && (now < end))
            {
                permitir = true;
            }

            if (!permitir)
            {
                Response.Result = false;
                Response.Message = "NOHORARIO";
                Response.Data = "";
                return Response;
            }
            #endregion

            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                bool tieneCilindros = false;
                #region json detalles
                List<objDetalle> detalles = jsonSerializer.Deserialize<List<objDetalle>>(_strDetallePedido);

                bool soloCroquetas;
                bool todosSonAgua;
                bool tieneCroquetas;
                string validacionProductos = validaProductosPedido(detalles, context, out soloCroquetas, out todosSonAgua, out tieneCroquetas);
                if (validacionProductos != "PRODUCTO_VALIDO")
                {
                    Response.Result = false;
                    Response.Message = "PEDIDOERR";
                    Response.Data = validacionProductos;
                    return Response;
                }

                List<objDetalle> pedidoCilindrera = new List<objDetalle>();
                List<objDetalle> pedidoEstacionario = new List<objDetalle>();
                List<objDetalle> pedidoAwa = new List<objDetalle>();

                if (detalles.Where(x => x.clave == 2 || x.clave == 3).Count() > 0)
                {
                    tieneCilindros = true;
                }

                if (todosSonAgua)
                {
                    pedidoAwa.AddRange(detalles);
                }
                else
                {
                    foreach (var item in detalles)
                    {
                        if (item.clave == 9)
                        {
                            pedidoEstacionario.Add(item);
                        }
                        else
                        {
                            if (esProductoAguaEspecial(item.clave))
                            {
                                pedidoAwa.Add(item);
                            }
                            else
                            {
                                if (item.clave == 4)
                                {
                                    if (tieneCilindros || tieneCroquetas)
                                    {
                                        pedidoCilindrera.Add(item);
                                    }
                                    else
                                    {
                                        pedidoAwa.Add(item);
                                    }
                                }
                                else
                                {
                                    pedidoCilindrera.Add(item);
                                }
                            }
                        }
                    }
                }

                List<int> serviciosPedido = new List<int>();
                if (pedidoCilindrera.Count > 0)
                {
                    serviciosPedido.Add(calculaIdServicio(pedidoCilindrera, context));
                }
                if (pedidoEstacionario.Count > 0)
                {
                    serviciosPedido.Add(calculaIdServicio(pedidoEstacionario, context));
                }
                if (pedidoAwa.Count > 0)
                {
                    serviciosPedido.Add(calculaIdServicio(pedidoAwa, context));
                }

                List<int> serviciosPedidoDistintos = serviciosPedido.Distinct().ToList();
                if (serviciosPedidoDistintos.Contains(0) || context.servicio.Count(x => serviciosPedidoDistintos.Contains(x.id_servicio) && x.status == true) != serviciosPedidoDistintos.Count)
                {
                    Response.Result = false;
                    Response.Message = "PEDIDOERR";
                    Response.Data = "CONFIGURACION_INVALIDA";
                    return Response;
                }

                #endregion

                Direccion direccion = (
                    from SQLDireccion in context.Direccion
                    where SQLDireccion.id_direccion.Equals(_intIdDireccion)
                    select SQLDireccion
                ).FirstOrDefault();

                string idpedidoguardado = string.Empty;
                if (direccion != null)
                {
                    DateTime hoyfecha = DateTime.Now.Date;
                    TimeSpan hoyhora = DateTime.Now.TimeOfDay;

                    #region CILINDRERA
                    if (pedidoCilindrera.Count() > 0)
                    {
                        #region consulta del operador
                        int turnoC = context.Turnos.Where(x => DateTime.Now.TimeOfDay >= x.horainicio & DateTime.Now.TimeOfDay <= x.horafin).SingleOrDefault().id_turno;
                        int idServicioPedidoC = calculaIdServicio(pedidoCilindrera, context);
                        int tipoVehiculoC;
                        int idRuta;
                        Asignaciones consultaAsignacionC = resuelveAsignacionPedido(
                            direccion,
                            idServicioPedidoC,
                            1,
                            1,
                            turnoC,
                            context,
                            out tipoVehiculoC,
                            out idRuta
                        );

                        int? idOperadorC = 0;
                        if (consultaAsignacionC != null)
                        {
                            idOperadorC = consultaAsignacionC.id_operador;
                        }

                        if (idOperadorC == 0 || idOperadorC == null)
                        {
                            Response.Result = false;
                            Response.Data = "NOASIG";
                            Response.Message = "ERRASIGNACION";
                            return Response;
                        }
                        #endregion

                        #region creacion e insercion del pedido
                        Pedido nuevoPedidoCil = new Pedido();
                        nuevoPedidoCil.Id_Direccion = _intIdDireccion;
                        nuevoPedidoCil.Id_Cliente = _intIdCliente;
                        nuevoPedidoCil.Id_Telefono = _intIdTelefono;
                        nuevoPedidoCil.fecha_creacion = hoyfecha;
                        nuevoPedidoCil.hora_creacion = hoyhora;
                        nuevoPedidoCil.origen = 2;
                        nuevoPedidoCil.Id_Usuario = 10;
                        nuevoPedidoCil.Id_Metodo_Pago = _intIdMetodoPago;
                        nuevoPedidoCil.Id_Actitud = 7;
                        nuevoPedidoCil.fase = "RADIOS";
                        nuevoPedidoCil.Id_Servicio = idServicioPedidoC;
                        nuevoPedidoCil.No_Llamadas = 0;
                        nuevoPedidoCil.Pedido_confirmado_operador = false;
                        nuevoPedidoCil.Pedido_confirmado_cliente = false;
                        nuevoPedidoCil.Completo = false;
                        nuevoPedidoCil.verificar_litrometro = false;
                        nuevoPedidoCil.Entrega_Nota = false;
                        nuevoPedidoCil.dar_cambio = false;
                        nuevoPedidoCil.conformidad_servicio = false;
                        nuevoPedidoCil.volveria_comprar = false;
                        nuevoPedidoCil.status_pedido = true;
                        nuevoPedidoCil.Id_ruta = idRuta;
                        nuevoPedidoCil.enasignacion = true;
                        nuevoPedidoCil.asignado = true;
                        nuevoPedidoCil.tiene_incidencia = false;
                        nuevoPedidoCil.check_por_whatsapp = false;
                        nuevoPedidoCil.pedido_programado = false;
                        context.Pedido.InsertOnSubmit(nuevoPedidoCil);
                        context.SubmitChanges();
                        #endregion

                        idpedidoguardado = jsonSerializer.Serialize(nuevoPedidoCil.Id_Pedido);

                        #region insercion de los detalles
                        List<Pedido_Detalle> detallePedidoC = new List<Pedido_Detalle>();
                        foreach (objDetalle item in pedidoCilindrera)
                        {
                            int? claveServicio = (
                                from SQLProducto in context.producto
                                where SQLProducto.id_producto.Equals(item.clave)
                                select new
                                {
                                    SQLProducto.id_servicio
                                }
                            ).FirstOrDefault().id_servicio;

                            string unidadLiquidacion = "0";
                            if (item.clave == 2 || item.clave == 3)
                            {
                                unidadLiquidacion = "KILOS";
                            }
                            else if (item.clave == 9)
                            {
                                unidadLiquidacion = "LITROS";
                            }

                            Pedido_Detalle undetalle = new Pedido_Detalle();
                            undetalle.Id_Pedido = nuevoPedidoCil.Id_Pedido;
                            undetalle.Id_producto = item.clave;

                            undetalle.cantidad = Convert.ToDouble(item.cantidad.ToString("N2"));

                            double vPrecio = (double)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                            double vImporte = vPrecio * item.cantidad;
                            undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));

                            undetalle.unidad_de_liq = unidadLiquidacion;

                            #region conversiones
                            if (claveServicio == 1 && item.clave == 2) //30kg
                            {
                                undetalle.Kg_Lts_a_surtir = 30;
                                decimal preciocilindro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;
                                decimal preciolitro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(9)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                                decimal preciokg = preciocilindro / 30;
                                decimal litros = preciocilindro / preciolitro;
                                undetalle.litros_a_surtir = (double?)litros;
                            }
                            if (claveServicio == 1 && item.clave == 3) //45kg
                            {
                                undetalle.Kg_Lts_a_surtir = 45;
                                decimal preciocilindro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;
                                decimal preciolitro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(9)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                                decimal preciokg = preciocilindro / 45;
                                decimal litros = preciocilindro / preciolitro;
                                undetalle.litros_a_surtir = (double?)litros;
                            }
                            /*PRODUCTOS SIN CONVERSION DE GAS: AGUA O CROQUETAS*/
                            if (claveServicio == 3 || claveServicio == 9)
                            {
                                undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));
                                undetalle.Kg_Lts_a_surtir = 0;
                                undetalle.litros_a_surtir = 0;
                            }
                            #endregion
                            undetalle.porc_inicio = 0;
                            undetalle.porc_final = 0;

                            undetalle.tipo_unidad = tipoVehiculoC;
                            undetalle.id_operador = idOperadorC;
                            undetalle.id_servicio = claveServicio;
                            detallePedidoC.Add(undetalle);
                        }
                        context.Pedido_Detalle.InsertAllOnSubmit(detallePedidoC);
                        context.SubmitChanges();
                        #endregion

                        #region ciclo del pedido
                        ciclo_pedido uncicloC = new ciclo_pedido();
                        uncicloC.id_pedido = nuevoPedidoCil.Id_Pedido;
                        uncicloC.hora_recepcion = hoyhora;
                        uncicloC.hora_radiado = hoyhora;
                        uncicloC.alta_radios = hoyfecha;
                        uncicloC.dif_recep_radi = hoyhora - hoyhora;
                        uncicloC.id_usuario = 10;
                        uncicloC.creacion = hoyfecha;
                        uncicloC.status = true;
                        context.ciclo_pedido.InsertOnSubmit(uncicloC);
                        context.SubmitChanges();
                        #endregion

                        #region asignación del pedido
                        Asigna_Pedido asignapedidoC = new Asigna_Pedido();
                        asignapedidoC.id_pedido = nuevoPedidoCil.Id_Pedido;
                        asignapedidoC.id_operador = idOperadorC;
                        asignapedidoC.id_ruta = idRuta;
                        asignapedidoC.id_usuario = 10;
                        asignapedidoC.creado = hoyfecha;
                        asignapedidoC.hora = hoyhora;
                        asignapedidoC.status = true;
                        guardaAsignacionConVentaRepartidor(context, asignapedidoC, detallePedidoC);
                        #endregion

                        #region movimientos pedidos
                        movimientos_pedidos mov = new movimientos_pedidos();
                        mov.id_usuario = 10;
                        mov.id_pedido = nuevoPedidoCil.Id_Pedido;
                        mov.hora = DateTime.Now.TimeOfDay;
                        mov.fecha = DateTime.Now.Date;
                        mov.funcion = "ASIGNA PEDIDO";
                        mov.sistema = "PEDIDO: " + nuevoPedidoCil.Id_Pedido + ", HORA PRIMER RADIDADO: " + mov.hora + ", HORA  RADIDADO: " + mov.hora + ",VECES RADIADO: 1";
                        mov.datos = "" + nuevoPedidoCil.Id_Pedido + "," + mov.hora + "," + mov.hora + ", 1";
                        context.movimientos_pedidos.InsertOnSubmit(mov);
                        context.SubmitChanges();
                        #endregion

                        #region inserta info de radios pedidos
                        Radios_Pedidos rp = new Radios_Pedidos();
                        rp.fecha = DateTime.Now.Date;
                        rp.id_operador = (int)idOperadorC;
                        rp.id_pedido = nuevoPedidoCil.Id_Pedido;
                        rp.id_ruta = (int)nuevoPedidoCil.Id_ruta;
                        rp.id_usuario = 10;
                        rp.tiempo = DateTime.Now.TimeOfDay;
                        rp.tipo = "ASIGNA";
                        context.Radios_Pedidos.InsertOnSubmit(rp);
                        context.SubmitChanges();
                        #endregion

                        #region guarda los tiempos de captura
                        tiempos_monitorizacion_fases tmfC = new tiempos_monitorizacion_fases();
                        tmfC.idpedido = nuevoPedidoCil.Id_Pedido;
                        tmfC.tmftiempocaptura = 0;
                        tmfC.tmffechacaptura = DateTime.Now;

                        string vehiculodepedidoC = (from SQLVehiculo in context.TipoUnidad where SQLVehiculo.id_tipounidad.Equals(tipoVehiculoC) select SQLVehiculo).FirstOrDefault().tipo_unidad;

                        double tiemporadiosC = 0;
                        double tiempoentregaC = 0;
                        double tiempoconfirmaC = 0;
                        #region tiempo de fase radios
                        var tiempofaseradiosC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("RADIO") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseradiosC != null)
                        {
                            if (tiempofaseradiosC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiemporadiosC = tiempofaseradiosC.tf_tiempo;
                            }
                            if (tiempofaseradiosC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseradiosC.tf_tiempo * 60;
                                tiemporadiosC = segundos;
                            }
                            if (tiempofaseradiosC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseradiosC.tf_tiempo * 60) * 60;
                                tiemporadiosC = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase entrega
                        var tiempofaseentregaC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("ENTREGA") && SQLTiempoFase.tf_fase.Contains(vehiculodepedidoC) && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseentregaC != null)
                        {
                            if (tiempofaseentregaC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoentregaC = tiempofaseentregaC.tf_tiempo;
                            }
                            if (tiempofaseentregaC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseentregaC.tf_tiempo * 60;
                                tiempoentregaC = segundos;
                            }
                            if (tiempofaseentregaC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseentregaC.tf_tiempo * 60) * 60;
                                tiempoentregaC = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase confirmación
                        var tiempofaseconfirmaC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("CONFIRMA") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseconfirmaC != null)
                        {
                            if (tiempofaseconfirmaC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoconfirmaC = tiempofaseconfirmaC.tf_tiempo;
                            }
                            if (tiempofaseconfirmaC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseconfirmaC.tf_tiempo * 60;
                                tiempoconfirmaC = segundos;
                            }
                            if (tiempofaseconfirmaC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseconfirmaC.tf_tiempo * 60) * 60;
                                tiempoconfirmaC = segundos;
                            }
                        }
                        #endregion

                        if (tiemporadiosC != 0)
                        {
                            tmfC.tmfradiar = tmfC.tmffechacaptura.AddSeconds(tiemporadiosC);
                        }
                        if (tiempoentregaC != 0)
                        {
                            tmfC.tmfentregar = tmfC.tmffechacaptura.AddSeconds(tiempoentregaC);
                        }
                        if (tiempoconfirmaC != 0)
                        {
                            tmfC.tmfconfirmar = tmfC.tmffechacaptura.AddSeconds(tiempoconfirmaC);
                        }

                        tmfC.tmfusuariocaptura = 10;
                        context.tiempos_monitorizacion_fases.InsertOnSubmit(tmfC);
                        context.SubmitChanges();
                        #endregion

                        #region funciones para tiempos de fase
                        tiempos_monitorizacion_fases pedidofase = (
                            from SQLPedidoFase in context.tiempos_monitorizacion_fases
                            where SQLPedidoFase.idpedido.Equals(nuevoPedidoCil.Id_Pedido)
                            select SQLPedidoFase
                        ).FirstOrDefault();

                        DateTime fechacapturado = pedidofase.tmffechacaptura;
                        DateTime fecharadiado = DateTime.Now;
                        TimeSpan tiempoderadiado = fecharadiado - fechacapturado;

                        pedidofase.tmffecharadios = fecharadiado;
                        pedidofase.tmfusuarioentrega = idOperadorC;
                        pedidofase.tmftiemporadios = tiempoderadiado.TotalSeconds;
                        pedidofase.tmfusuarioradios = 10;
                        context.SubmitChanges();
                        #endregion

                        #region Observaciones del pedido 
                        if (_observacionesPedido != null)
                        {
                            if (_observacionesPedido.Length > 0)
                            {
                                string observacionesPedido = string.Format("CLAVE DE ACCESO: {0}", _observacionesPedido);
                                observaciones_pedido obs = new observaciones_pedido();
                                obs.Id_Pedido = nuevoPedidoCil.Id_Pedido;
                                obs.Id_Usuario = 10;
                                obs.descripcion = observacionesPedido.ToUpper();
                                obs.fecha = hoyfecha;
                                obs.hora = hoyhora;
                                obs.fase = "RADIOS";
                                context.observaciones_pedido.InsertOnSubmit(obs);
                                context.SubmitChanges();
                                Pedido ped = context.Pedido.Where(x => x.Id_Pedido == nuevoPedidoCil.Id_Pedido).FirstOrDefault();
                                ped.observa_pedido = ped.observa_pedido + "[" + observacionesPedido.ToUpper() + "]";
                                context.SubmitChanges();
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region ESTACIONARIO
                    if (pedidoEstacionario.Count() > 0)
                    {
                        #region consulta del operador
                        int turnoE = context.Turnos.Where(x => DateTime.Now.TimeOfDay >= x.horainicio & DateTime.Now.TimeOfDay <= x.horafin).SingleOrDefault().id_turno;
                        int idServicioPedidoE = calculaIdServicio(pedidoEstacionario, context);
                        int tipoVehiculo;
                        int idRuta;
                        Asignaciones consultaAsignacion = resuelveAsignacionPedido(
                            direccion,
                            idServicioPedidoE,
                            2,
                            2,
                            turnoE,
                            context,
                            out tipoVehiculo,
                            out idRuta
                        );

                        int? idOperadorE = 0;
                        if (consultaAsignacion != null)
                        {
                            idOperadorE = consultaAsignacion.id_operador;
                        }

                        if (idOperadorE == 0 || idOperadorE == null)
                        {
                            Response.Result = false;
                            Response.Data = "NOASIG";
                            Response.Message = "ERRASIGNACION";
                            return Response;
                        }
                        #endregion

                        #region creacion e insercion del pedido
                        Pedido nuevoPedidoE = new Pedido();
                        nuevoPedidoE.Id_Direccion = _intIdDireccion;
                        nuevoPedidoE.Id_Cliente = _intIdCliente;
                        nuevoPedidoE.Id_Telefono = _intIdTelefono;
                        nuevoPedidoE.fecha_creacion = hoyfecha;
                        nuevoPedidoE.hora_creacion = hoyhora;
                        nuevoPedidoE.origen = 2;
                        nuevoPedidoE.Id_Usuario = 10;
                        nuevoPedidoE.Id_Metodo_Pago = _intIdMetodoPago;
                        nuevoPedidoE.Id_Actitud = 7;
                        nuevoPedidoE.fase = "RADIOS";
                        nuevoPedidoE.Id_Servicio = idServicioPedidoE;
                        nuevoPedidoE.No_Llamadas = 0;
                        nuevoPedidoE.Pedido_confirmado_operador = false;
                        nuevoPedidoE.Pedido_confirmado_cliente = false;
                        nuevoPedidoE.Completo = false;
                        nuevoPedidoE.verificar_litrometro = false;
                        nuevoPedidoE.Entrega_Nota = false;
                        nuevoPedidoE.dar_cambio = false;
                        nuevoPedidoE.conformidad_servicio = false;
                        nuevoPedidoE.volveria_comprar = false;
                        nuevoPedidoE.status_pedido = true;
                        nuevoPedidoE.Id_ruta = idRuta;
                        nuevoPedidoE.enasignacion = true;
                        nuevoPedidoE.asignado = true;
                        nuevoPedidoE.tiene_incidencia = false;
                        nuevoPedidoE.pedido_programado = false;
                        nuevoPedidoE.check_por_whatsapp = false;
                        context.Pedido.InsertOnSubmit(nuevoPedidoE);
                        context.SubmitChanges();
                        #endregion

                        idpedidoguardado = jsonSerializer.Serialize(nuevoPedidoE.Id_Pedido);

                        #region insercion de los detalles
                        List<Pedido_Detalle> detallePedidoE = new List<Pedido_Detalle>();
                        foreach (objDetalle item in pedidoEstacionario)
                        {
                            Pedido_Detalle undetalle = new Pedido_Detalle();
                            undetalle.Id_Pedido = nuevoPedidoE.Id_Pedido;
                            undetalle.Id_producto = item.clave;
                            undetalle.unidad_de_liq = "LITROS";
                            string descricion = item.descripcionProducto;
                            int tipo_estacionario = 0;

                            if (item.descripcionProducto == null || item.descripcionProducto == "")
                            {
                                tipo_estacionario = 0;
                            }
                            else
                            {
                                if (descricion.Contains("litros gas estacionario = $"))
                                {
                                    tipo_estacionario = 2;
                                }
                                else
                                {
                                    tipo_estacionario = 1;
                                }
                            }

                            #region conversiones
                            double preciokilo = (double)(
                                 from SQLProducto in context.producto
                                 where SQLProducto.id_producto.Equals(item.clave)
                                 select new
                                 {
                                     precio = SQLProducto.precio_kilo
                                 }
                             ).FirstOrDefault().precio;

                            double preciolitro = (double)(
                               from SQLProducto in context.producto
                               where SQLProducto.id_producto.Equals(item.clave)
                               select new
                               {
                                   precio = SQLProducto.precio
                               }
                           ).FirstOrDefault().precio;

                            if (tipo_estacionario == 1)
                            {
                                double vImporte = item.importe;
                                undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));

                                double vCantidad = vImporte / preciolitro;
                                undetalle.cantidad = Convert.ToDouble(vCantidad.ToString("N2"));
                                undetalle.litros_a_surtir = Convert.ToDouble(vCantidad.ToString("N2"));

                                double kilos = vImporte / preciokilo;
                                undetalle.Kg_Lts_a_surtir = Convert.ToDouble(kilos.ToString("N2"));
                            }
                            else if (tipo_estacionario == 2)
                            {
                                double vCantidad = item.cantidad;
                                undetalle.cantidad = Convert.ToDouble(vCantidad.ToString("N2"));
                                undetalle.litros_a_surtir = Convert.ToDouble(vCantidad.ToString("N2"));

                                double vImporte = vCantidad * preciolitro;
                                undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));

                                double kilos = vImporte / preciokilo;
                                undetalle.Kg_Lts_a_surtir = Convert.ToDouble(kilos.ToString("N2"));
                            }
                            /*EXCEPCION QUE HAGA LO ORIGINAL EN CASO DE QUE AUN NO SE HAYA PUBLICADO LA APP*/
                            else
                            {
                                undetalle.litros_a_surtir = Convert.ToDouble(item.cantidad.ToString("N2"));
                                double kilos = item.importe / preciokilo;
                                undetalle.Kg_Lts_a_surtir = Convert.ToDouble(kilos.ToString("N2"));

                                undetalle.cantidad = Convert.ToDouble(item.cantidad.ToString("N2"));
                                undetalle.importe = Convert.ToDouble(item.importe.ToString("N2"));
                            }
                            #endregion

                            undetalle.porc_inicio = 0;
                            undetalle.porc_final = 0;

                            undetalle.tipo_unidad = tipoVehiculo;
                            undetalle.id_operador = idOperadorE;
                            undetalle.id_servicio = 1;
                            detallePedidoE.Add(undetalle);
                        }
                        context.Pedido_Detalle.InsertAllOnSubmit(detallePedidoE);
                        context.SubmitChanges();
                        #endregion

                        #region ciclo del pedido
                        ciclo_pedido unciclo = new ciclo_pedido();
                        unciclo.id_pedido = nuevoPedidoE.Id_Pedido;
                        unciclo.hora_recepcion = hoyhora;
                        unciclo.hora_radiado = hoyhora;
                        unciclo.alta_radios = hoyfecha;
                        unciclo.dif_recep_radi = hoyhora - hoyhora;
                        unciclo.id_usuario = 10;
                        unciclo.creacion = hoyfecha;
                        unciclo.status = true;
                        context.ciclo_pedido.InsertOnSubmit(unciclo);
                        context.SubmitChanges();
                        #endregion

                        #region asignacion del pedido
                        Asigna_Pedido asignapedido = new Asigna_Pedido();
                        asignapedido.id_pedido = nuevoPedidoE.Id_Pedido;
                        asignapedido.id_operador = idOperadorE;
                        asignapedido.id_ruta = idRuta;
                        asignapedido.id_usuario = 10;
                        asignapedido.creado = hoyfecha;
                        asignapedido.hora = hoyhora;
                        asignapedido.status = true;
                        guardaAsignacionConVentaRepartidor(context, asignapedido, detallePedidoE);
                        #endregion

                        #region movimientos pedidos
                        movimientos_pedidos mov = new movimientos_pedidos();
                        mov.id_usuario = 10;
                        mov.id_pedido = nuevoPedidoE.Id_Pedido;
                        mov.hora = DateTime.Now.TimeOfDay;
                        mov.fecha = DateTime.Now.Date;
                        mov.funcion = "ASIGNA PEDIDO";
                        mov.sistema = "PEDIDO: " + nuevoPedidoE.Id_Pedido + ", HORA PRIMER RADIDADO: " + mov.hora + ", HORA  RADIDADO: " + mov.hora + ",VECES RADIADO: 1";
                        mov.datos = "" + nuevoPedidoE.Id_Pedido + "," + mov.hora + "," + mov.hora + ", 1";
                        context.movimientos_pedidos.InsertOnSubmit(mov);
                        context.SubmitChanges();
                        #endregion

                        #region inserta info de radios pedidos
                        Radios_Pedidos rp = new Radios_Pedidos();
                        rp.fecha = DateTime.Now.Date;
                        rp.id_operador = (int)idOperadorE;
                        rp.id_pedido = nuevoPedidoE.Id_Pedido;
                        rp.id_ruta = (int)nuevoPedidoE.Id_ruta;
                        rp.id_usuario = 10;
                        rp.tiempo = DateTime.Now.TimeOfDay;
                        rp.tipo = "ASIGNA";
                        context.Radios_Pedidos.InsertOnSubmit(rp);
                        context.SubmitChanges();
                        #endregion

                        #region guarda los tiempos de captura
                        tiempos_monitorizacion_fases tmf = new tiempos_monitorizacion_fases();
                        tmf.idpedido = nuevoPedidoE.Id_Pedido;
                        tmf.tmftiempocaptura = 0;
                        tmf.tmffechacaptura = DateTime.Now;

                        int tipovehiculo = tipoVehiculo;
                        string vehiculodepedido = (from SQLVehiculo in context.TipoUnidad where SQLVehiculo.id_tipounidad.Equals(tipovehiculo) select SQLVehiculo).FirstOrDefault().tipo_unidad;
                        if (vehiculodepedido.Contains("AWA"))
                        {
                            vehiculodepedido = "CILIN";
                        }

                        double tiemporadios = 0;
                        double tiempoentrega = 0;
                        double tiempoconfirma = 0;
                        #region tiempo de fase radios
                        var tiempofaseradios = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("RADIO") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseradios != null)
                        {
                            if (tiempofaseradios.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiemporadios = tiempofaseradios.tf_tiempo;
                            }
                            if (tiempofaseradios.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseradios.tf_tiempo * 60;
                                tiemporadios = segundos;
                            }
                            if (tiempofaseradios.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseradios.tf_tiempo * 60) * 60;
                                tiemporadios = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase entrega
                        var tiempofaseentrega = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("ENTREGA") && SQLTiempoFase.tf_fase.Contains(vehiculodepedido) && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseentrega != null)
                        {
                            if (tiempofaseentrega.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoentrega = tiempofaseentrega.tf_tiempo;
                            }
                            if (tiempofaseentrega.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseentrega.tf_tiempo * 60;
                                tiempoentrega = segundos;
                            }
                            if (tiempofaseentrega.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseentrega.tf_tiempo * 60) * 60;
                                tiempoentrega = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase confirmación
                        var tiempofaseconfirma = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("CONFIRMA") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseconfirma != null)
                        {
                            if (tiempofaseconfirma.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoconfirma = tiempofaseconfirma.tf_tiempo;
                            }
                            if (tiempofaseconfirma.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseconfirma.tf_tiempo * 60;
                                tiempoconfirma = segundos;
                            }
                            if (tiempofaseconfirma.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseconfirma.tf_tiempo * 60) * 60;
                                tiempoconfirma = segundos;
                            }
                        }
                        #endregion

                        if (tiemporadios != 0)
                        {
                            tmf.tmfradiar = tmf.tmffechacaptura.AddSeconds(tiemporadios);
                        }
                        if (tiempoentrega != 0)
                        {
                            tmf.tmfentregar = tmf.tmffechacaptura.AddSeconds(tiempoentrega);
                        }
                        if (tiempoconfirma != 0)
                        {
                            tmf.tmfconfirmar = tmf.tmffechacaptura.AddSeconds(tiempoconfirma);
                        }

                        tmf.tmfusuariocaptura = 10;
                        context.tiempos_monitorizacion_fases.InsertOnSubmit(tmf);
                        context.SubmitChanges();
                        #endregion

                        #region funciones para tiempos de fase
                        tiempos_monitorizacion_fases pedidofase = (
                            from SQLPedidoFase in context.tiempos_monitorizacion_fases
                            where SQLPedidoFase.idpedido.Equals(nuevoPedidoE.Id_Pedido)
                            select SQLPedidoFase
                        ).FirstOrDefault();

                        DateTime fechacapturado = pedidofase.tmffechacaptura;
                        DateTime fecharadiado = DateTime.Now;
                        TimeSpan tiempoderadiado = fecharadiado - fechacapturado;

                        pedidofase.tmffecharadios = fecharadiado;
                        pedidofase.tmfusuarioentrega = idOperadorE;
                        pedidofase.tmftiemporadios = tiempoderadiado.TotalSeconds;
                        pedidofase.tmfusuarioradios = 10;
                        context.SubmitChanges();
                        #endregion

                        #region Observaciones del pedido 
                        if (_observacionesPedido != null)
                        {
                            if (_observacionesPedido.Length > 0)
                            {
                                string observacionesPedido = string.Format("CLAVE DE ACCESO: {0}", _observacionesPedido);
                                observaciones_pedido obs = new observaciones_pedido();
                                obs.Id_Pedido = nuevoPedidoE.Id_Pedido;
                                obs.Id_Usuario = 10;
                                obs.descripcion = observacionesPedido.ToUpper();
                                obs.fecha = hoyfecha;
                                obs.hora = hoyhora;
                                obs.fase = "RADIOS";
                                context.observaciones_pedido.InsertOnSubmit(obs);
                                context.SubmitChanges();
                                Pedido ped = context.Pedido.Where(x => x.Id_Pedido == nuevoPedidoE.Id_Pedido).FirstOrDefault();
                                ped.observa_pedido = ped.observa_pedido + "[" + observacionesPedido.ToUpper() + "]";
                                context.SubmitChanges();
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region AWA
                    if (pedidoAwa.Count > 0)
                    {
                        #region consulta del operador
                        int turnoC = context.Turnos.Where(x => DateTime.Now.TimeOfDay >= x.horainicio & DateTime.Now.TimeOfDay <= x.horafin).SingleOrDefault().id_turno;
                        int idServicioPedidoAwa = calculaIdServicio(pedidoAwa, context);
                        int tipoVehiculoC;
                        int idRuta;
                        Asignaciones consultaAsignacionC = resuelveAsignacionPedido(
                            direccion,
                            idServicioPedidoAwa,
                            3,
                            3,
                            turnoC,
                            context,
                            out tipoVehiculoC,
                            out idRuta
                        );

                        int? idOperadorC = 0;
                        if (consultaAsignacionC != null)
                        {
                            idOperadorC = consultaAsignacionC.id_operador;
                        }

                        if (idOperadorC == 0 || idOperadorC == null)
                        {
                            Response.Result = false;
                            Response.Data = "NOASIG";
                            Response.Message = "ERRASIGNACION";
                            return Response;
                        }
                        #endregion

                        #region creacion e insercion del pedido
                        Pedido nuevoPedidoAwa = new Pedido();
                        nuevoPedidoAwa.Id_Direccion = _intIdDireccion;
                        nuevoPedidoAwa.Id_Cliente = _intIdCliente;
                        nuevoPedidoAwa.Id_Telefono = _intIdTelefono;
                        nuevoPedidoAwa.fecha_creacion = hoyfecha;
                        nuevoPedidoAwa.hora_creacion = hoyhora;
                        nuevoPedidoAwa.origen = 2;
                        nuevoPedidoAwa.Id_Usuario = 10;
                        nuevoPedidoAwa.Id_Metodo_Pago = _intIdMetodoPago;
                        nuevoPedidoAwa.Id_Actitud = 7;
                        nuevoPedidoAwa.fase = "RADIOS";
                        nuevoPedidoAwa.Id_Servicio = idServicioPedidoAwa;
                        nuevoPedidoAwa.No_Llamadas = 0;
                        nuevoPedidoAwa.Pedido_confirmado_operador = false;
                        nuevoPedidoAwa.Pedido_confirmado_cliente = false;
                        nuevoPedidoAwa.Completo = false;
                        nuevoPedidoAwa.verificar_litrometro = false;
                        nuevoPedidoAwa.Entrega_Nota = false;
                        nuevoPedidoAwa.dar_cambio = false;
                        nuevoPedidoAwa.conformidad_servicio = false;
                        nuevoPedidoAwa.volveria_comprar = false;
                        nuevoPedidoAwa.status_pedido = true;
                        nuevoPedidoAwa.Id_ruta = idRuta;
                        nuevoPedidoAwa.enasignacion = true;
                        nuevoPedidoAwa.asignado = true;
                        nuevoPedidoAwa.tiene_incidencia = false;
                        nuevoPedidoAwa.check_por_whatsapp = false;
                        nuevoPedidoAwa.pedido_programado = false;
                        context.Pedido.InsertOnSubmit(nuevoPedidoAwa);
                        context.SubmitChanges();
                        #endregion

                        idpedidoguardado = jsonSerializer.Serialize(nuevoPedidoAwa.Id_Pedido);

                        #region insercion de los detalles
                        List<Pedido_Detalle> detallePedidoAwa = new List<Pedido_Detalle>();
                        foreach (objDetalle item in pedidoAwa)
                        {
                            int? claveServicio = (
                                from SQLProducto in context.producto
                                where SQLProducto.id_producto.Equals(item.clave)
                                select new
                                {
                                    SQLProducto.id_servicio
                                }
                            ).FirstOrDefault().id_servicio;

                            string unidadLiquidacion = "0";
                            if (item.clave == 2 || item.clave == 3)
                            {
                                unidadLiquidacion = "KILOS";
                            }
                            else if (item.clave == 9)
                            {
                                unidadLiquidacion = "LITROS";
                            }

                            Pedido_Detalle undetalle = new Pedido_Detalle();
                            undetalle.Id_Pedido = nuevoPedidoAwa.Id_Pedido;
                            undetalle.Id_producto = item.clave;

                            undetalle.cantidad = Convert.ToDouble(item.cantidad.ToString("N2"));

                            double vPrecio = (double)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                            double vImporte = vPrecio * item.cantidad;
                            undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));

                            undetalle.unidad_de_liq = unidadLiquidacion;

                            #region conversiones
                            if (claveServicio == 1 && item.clave == 2) //30kg
                            {
                                undetalle.Kg_Lts_a_surtir = 30;
                                decimal preciocilindro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;
                                decimal preciolitro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(9)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                                decimal preciokg = preciocilindro / 30;
                                decimal litros = preciocilindro / preciolitro;
                                undetalle.litros_a_surtir = (double?)litros;
                            }
                            if (claveServicio == 1 && item.clave == 3) //45kg
                            {
                                undetalle.Kg_Lts_a_surtir = 45;
                                decimal preciocilindro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(item.clave)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;
                                decimal preciolitro = (decimal)(
                                    from SQLProducto in context.producto
                                    where SQLProducto.id_producto.Equals(9)
                                    select new
                                    {
                                        precio = SQLProducto.precio
                                    }
                                ).FirstOrDefault().precio;

                                decimal preciokg = preciocilindro / 45;
                                decimal litros = preciocilindro / preciolitro;
                                undetalle.litros_a_surtir = (double?)litros;
                            }
                            /*TODOS LOS PRODUCTOS DE AWA*/
                            if (claveServicio == 3)
                            {
                                undetalle.importe = Convert.ToDouble(vImporte.ToString("N2"));
                                undetalle.Kg_Lts_a_surtir = 0;
                                undetalle.litros_a_surtir = 0;
                            }
                            #endregion
                            undetalle.porc_inicio = 0;
                            undetalle.porc_final = 0;

                            undetalle.tipo_unidad = tipoVehiculoC;
                            undetalle.id_operador = idOperadorC;
                            undetalle.id_servicio = claveServicio;
                            detallePedidoAwa.Add(undetalle);
                        }
                        context.Pedido_Detalle.InsertAllOnSubmit(detallePedidoAwa);
                        context.SubmitChanges();
                        #endregion

                        #region ciclo del pedido
                        ciclo_pedido uncicloC = new ciclo_pedido();
                        uncicloC.id_pedido = nuevoPedidoAwa.Id_Pedido;
                        uncicloC.hora_recepcion = hoyhora;
                        uncicloC.hora_radiado = hoyhora;
                        uncicloC.alta_radios = hoyfecha;
                        uncicloC.dif_recep_radi = hoyhora - hoyhora;
                        uncicloC.id_usuario = 10;
                        uncicloC.creacion = hoyfecha;
                        uncicloC.status = true;
                        context.ciclo_pedido.InsertOnSubmit(uncicloC);
                        context.SubmitChanges();
                        #endregion

                        #region asignación del pedido
                        Asigna_Pedido asignapedidoC = new Asigna_Pedido();
                        asignapedidoC.id_pedido = nuevoPedidoAwa.Id_Pedido;
                        asignapedidoC.id_operador = idOperadorC;
                        asignapedidoC.id_ruta = idRuta;
                        asignapedidoC.id_usuario = 10;
                        asignapedidoC.creado = hoyfecha;
                        asignapedidoC.hora = hoyhora;
                        asignapedidoC.status = true;
                        guardaAsignacionConVentaRepartidor(context, asignapedidoC, detallePedidoAwa);
                        #endregion

                        #region movimientos pedidos
                        movimientos_pedidos mov = new movimientos_pedidos();
                        mov.id_usuario = 10;
                        mov.id_pedido = nuevoPedidoAwa.Id_Pedido;
                        mov.hora = DateTime.Now.TimeOfDay;
                        mov.fecha = DateTime.Now.Date;
                        mov.funcion = "ASIGNA PEDIDO";
                        mov.sistema = "PEDIDO: " + nuevoPedidoAwa.Id_Pedido + ", HORA PRIMER RADIDADO: " + mov.hora + ", HORA  RADIDADO: " + mov.hora + ",VECES RADIADO: 1";
                        mov.datos = "" + nuevoPedidoAwa.Id_Pedido + "," + mov.hora + "," + mov.hora + ", 1";
                        context.movimientos_pedidos.InsertOnSubmit(mov);
                        context.SubmitChanges();
                        #endregion

                        #region inserta info de radios pedidos
                        Radios_Pedidos rp = new Radios_Pedidos();
                        rp.fecha = DateTime.Now.Date;
                        rp.id_operador = (int)idOperadorC;
                        rp.id_pedido = nuevoPedidoAwa.Id_Pedido;
                        rp.id_ruta = (int)nuevoPedidoAwa.Id_ruta;
                        rp.id_usuario = 10;
                        rp.tiempo = DateTime.Now.TimeOfDay;
                        rp.tipo = "ASIGNA";
                        context.Radios_Pedidos.InsertOnSubmit(rp);
                        context.SubmitChanges();
                        #endregion

                        #region guarda los tiempos de captura
                        tiempos_monitorizacion_fases tmfC = new tiempos_monitorizacion_fases();
                        tmfC.idpedido = nuevoPedidoAwa.Id_Pedido;
                        tmfC.tmftiempocaptura = 0;
                        tmfC.tmffechacaptura = DateTime.Now;

                        string vehiculodepedidoC = (from SQLVehiculo in context.TipoUnidad where SQLVehiculo.id_tipounidad.Equals(tipoVehiculoC) select SQLVehiculo).FirstOrDefault().tipo_unidad;

                        double tiemporadiosC = 0;
                        double tiempoentregaC = 0;
                        double tiempoconfirmaC = 0;
                        #region tiempo de fase radios
                        var tiempofaseradiosC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("RADIO") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseradiosC != null)
                        {
                            if (tiempofaseradiosC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiemporadiosC = tiempofaseradiosC.tf_tiempo;
                            }
                            if (tiempofaseradiosC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseradiosC.tf_tiempo * 60;
                                tiemporadiosC = segundos;
                            }
                            if (tiempofaseradiosC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseradiosC.tf_tiempo * 60) * 60;
                                tiemporadiosC = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase entrega
                        var tiempofaseentregaC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("ENTREGA") && SQLTiempoFase.tf_fase.Contains(vehiculodepedidoC) && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseentregaC != null)
                        {
                            if (tiempofaseentregaC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoentregaC = tiempofaseentregaC.tf_tiempo;
                            }
                            if (tiempofaseentregaC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseentregaC.tf_tiempo * 60;
                                tiempoentregaC = segundos;
                            }
                            if (tiempofaseentregaC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseentregaC.tf_tiempo * 60) * 60;
                                tiempoentregaC = segundos;
                            }
                        }
                        #endregion
                        #region tiempo de fase confirmación
                        var tiempofaseconfirmaC = (
                            from SQLTiempoFase in context.tiempos_fases
                            where SQLTiempoFase.tf_fase.Contains("CONFIRMA") && SQLTiempoFase.tf_status.Equals(true)
                            select new
                            {
                                SQLTiempoFase.tf_tiempo,
                                SQLTiempoFase.tf_unidad
                            }
                        ).FirstOrDefault();

                        if (tiempofaseconfirmaC != null)
                        {
                            if (tiempofaseconfirmaC.tf_unidad.Contains("SEGUNDOS"))
                            {
                                tiempoconfirmaC = tiempofaseconfirmaC.tf_tiempo;
                            }
                            if (tiempofaseconfirmaC.tf_unidad.Contains("MINUTOS"))
                            {
                                double segundos = tiempofaseconfirmaC.tf_tiempo * 60;
                                tiempoconfirmaC = segundos;
                            }
                            if (tiempofaseconfirmaC.tf_unidad.Contains("HORAS"))
                            {
                                double segundos = (tiempofaseconfirmaC.tf_tiempo * 60) * 60;
                                tiempoconfirmaC = segundos;
                            }
                        }
                        #endregion

                        if (tiemporadiosC != 0)
                        {
                            tmfC.tmfradiar = tmfC.tmffechacaptura.AddSeconds(tiemporadiosC);
                        }
                        if (tiempoentregaC != 0)
                        {
                            tmfC.tmfentregar = tmfC.tmffechacaptura.AddSeconds(tiempoentregaC);
                        }
                        if (tiempoconfirmaC != 0)
                        {
                            tmfC.tmfconfirmar = tmfC.tmffechacaptura.AddSeconds(tiempoconfirmaC);
                        }

                        tmfC.tmfusuariocaptura = 10;
                        context.tiempos_monitorizacion_fases.InsertOnSubmit(tmfC);
                        context.SubmitChanges();
                        #endregion

                        #region funciones para tiempos de fase
                        tiempos_monitorizacion_fases pedidofase = (
                            from SQLPedidoFase in context.tiempos_monitorizacion_fases
                            where SQLPedidoFase.idpedido.Equals(nuevoPedidoAwa.Id_Pedido)
                            select SQLPedidoFase
                        ).FirstOrDefault();

                        DateTime fechacapturado = pedidofase.tmffechacaptura;
                        DateTime fecharadiado = DateTime.Now;
                        TimeSpan tiempoderadiado = fecharadiado - fechacapturado;

                        pedidofase.tmffecharadios = fecharadiado;
                        pedidofase.tmfusuarioentrega = idOperadorC;
                        pedidofase.tmftiemporadios = tiempoderadiado.TotalSeconds;
                        pedidofase.tmfusuarioradios = 10;
                        context.SubmitChanges();
                        #endregion

                        #region Observaciones del pedido 
                        if (_observacionesPedido != null)
                        {
                            if (_observacionesPedido.Length > 0)
                            {
                                string observacionesPedido = string.Format("CLAVE DE ACCESO: {0}", _observacionesPedido);
                                observaciones_pedido obs = new observaciones_pedido();
                                obs.Id_Pedido = nuevoPedidoAwa.Id_Pedido;
                                obs.Id_Usuario = 10;
                                obs.descripcion = observacionesPedido.ToUpper();
                                obs.fecha = hoyfecha;
                                obs.hora = hoyhora;
                                obs.fase = "RADIOS";
                                context.observaciones_pedido.InsertOnSubmit(obs);
                                context.SubmitChanges();
                                Pedido ped = context.Pedido.Where(x => x.Id_Pedido == nuevoPedidoAwa.Id_Pedido).FirstOrDefault();
                                ped.observa_pedido = ped.observa_pedido + "[" + observacionesPedido.ToUpper() + "]";
                                context.SubmitChanges();
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                Response.Result = true;
                Response.Data = idpedidoguardado;
                Response.Message = "PEDIDOOK";
            }
            catch (Exception)
            {
                Response.Result = false;
                Response.Message = "PEDIDOERR";
                Response.Data = "ERROR_INTERNO";
            }
            return Response;
        }

        private string validaProductosPedido(List<objDetalle> detalles, ContextCombugasDataContext context, out bool soloCroquetas, out bool todosSonAgua, out bool tieneCroquetas)
        {
            soloCroquetas = false;
            todosSonAgua = false;
            tieneCroquetas = false;
            if (detalles == null || detalles.Count == 0)
            {
                return "LISTA_VACIA";
            }

            if (detalles.Any(x => x == null || x.cantidad <= 0 || double.IsNaN(x.cantidad) || double.IsInfinity(x.cantidad)))
            {
                return "CANTIDAD_INVALIDA";
            }

            List<int> claves = detalles.Select(x => x.clave).Distinct().ToList();
            var productos = context.producto
                .Where(x => claves.Contains(x.id_producto))
                .Select(x => new
                {
                    x.id_producto,
                    x.status,
                    x.precio,
                    x.id_servicio
                })
                .ToList();

            List<int> serviciosActivos = context.servicio
                .Where(x => x.status == true)
                .Select(x => x.id_servicio)
                .ToList();

            foreach (objDetalle detalle in detalles)
            {
                var producto = productos.FirstOrDefault(x => x.id_producto == detalle.clave);
                if (producto == null)
                {
                    return "PRODUCTO_NO_EXISTE";
                }
                if (producto.status != true)
                {
                    return "PRODUCTO_INACTIVO";
                }
                if (!producto.precio.HasValue || !producto.id_servicio.HasValue || !serviciosActivos.Contains(producto.id_servicio.Value))
                {
                    return "CONFIGURACION_INVALIDA";
                }
            }

            soloCroquetas = true;
            todosSonAgua = true;
            foreach (objDetalle detalle in detalles)
            {
                var producto = productos.First(x => x.id_producto == detalle.clave);
                if (producto.id_servicio.Value == 9)
                {
                    tieneCroquetas = true;
                }
                if (producto.id_servicio.Value != 9)
                {
                    soloCroquetas = false;
                }
                if (producto.id_servicio.Value != 3)
                {
                    todosSonAgua = false;
                }
            }

            return "PRODUCTO_VALIDO";
        }

        private void guardaAsignacionConVentaRepartidor(
            ContextCombugasDataContext context,
            Asigna_Pedido asignacion,
            List<Pedido_Detalle> detalles)
        {
            // Usar los detalles calculados: en estacionario la app puede pedir por importe.
            int cilindros30 = detalles.Where(x => x.Id_producto == 2).Sum(x => Convert.ToInt32(x.cantidad));
            int cilindros45 = detalles.Where(x => x.Id_producto == 3).Sum(x => Convert.ToInt32(x.cantidad));
            int garrafones = detalles.Where(x => x.Id_producto == 4).Sum(x => Convert.ToInt32(x.cantidad));
            int garrafonesAlkalinos = detalles.Where(x => x.Id_producto == 7).Sum(x => Convert.ToInt32(x.cantidad));
            int six500 = detalles.Where(x => x.Id_producto == 8).Sum(x => Convert.ToInt32(x.cantidad));
            int sixLitro = detalles.Where(x => x.Id_producto == 10).Sum(x => Convert.ToInt32(x.cantidad));
            int six500Alkalino = detalles.Where(x => x.Id_producto == 14).Sum(x => Convert.ToInt32(x.cantidad));
            double litros = detalles.Where(x => x.Id_producto == 9).Sum(x => x.litros_a_surtir ?? 0);

            bool abrirConexion = context.Connection.State == System.Data.ConnectionState.Closed;
            if (abrirConexion)
                context.Connection.Open();

            try
            {
                using (var transaccion = context.Connection.BeginTransaction())
                {
                    context.Transaction = transaccion;
                    // El bloqueo de rango evita insertar dos registros para el mismo pedido.
                    // Si ya existe, conservar sus cantidades y su estado de venta.
                    context.ExecuteCommand(@"
                        INSERT INTO dbo.ventas_repartidor
                            (id_pedido, id_operador, fecha, es_venta,
                             cil_t_a, cil_c_a, awa_g_a, awa_alk_g_a,
                             six_500_a, six_l_a, six_500_alk_a, lit_e_a)
                        SELECT {0}, {1}, {2}, 0, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}
                        WHERE NOT EXISTS (
                            SELECT 1 FROM dbo.ventas_repartidor WITH (UPDLOCK, HOLDLOCK)
                            WHERE id_pedido = {0})",
                        asignacion.id_pedido, asignacion.id_operador,
                        asignacion.creado, cilindros30, cilindros45, garrafones,
                        garrafonesAlkalinos, six500, sixLitro, six500Alkalino, litros);

                    context.Asigna_Pedido.InsertOnSubmit(asignacion);
                    context.SubmitChanges();
                    transaccion.Commit();
                }
            }
            finally
            {
                context.Transaction = null;
                if (abrirConexion)
                    context.Connection.Close();
            }
        }

        private Asignaciones resuelveAsignacionPedido(
            Direccion direccion,
            int idServicio,
            int tipoUnidadPredeterminado,
            int idTipoRuta,
            int idTurno,
            ContextCombugasDataContext context,
            out int idTipoUnidad,
            out int idRuta)
        {
            idRuta = 0;

            //int? tipoUnidadConfigurado = context.servicioUnidad
            //    .Where(x => x.id_servicio == idServicio && x.status == true)
            //    .OrderByDescending(x => x.id)
            //    .Select(x => (int?)x.id_tipounidad)
            //    .FirstOrDefault();

            //int tipoUnidadResuelto = tipoUnidadConfigurado ?? tipoUnidadPredeterminado;
            //tipoUnidadResuelto = tipoUnidadResuelto == 9 ? 1 : tipoUnidadResuelto;
            int tipoUnidadResuelto = tipoUnidadPredeterminado;
            idTipoUnidad = tipoUnidadResuelto;

            bool unidadActiva = context.TipoUnidad.Any(x =>
                x.id_tipounidad == tipoUnidadResuelto &&
                x.status == true
            );
            if (!unidadActiva)
            {
                return null;
            }

            List<Asignaciones> asignacionesCompatibles = context.Asignaciones
                .Where(x =>
                    x.id_turno == idTurno &&
                    x.asignacion_activa == true &&
                    x.Rutas.estatus == true &&
                    x.Rutas.id_tipo_ruta == idTipoRuta &&
                    x.truck.status == true &&
                    x.truck.id_tipounidad == tipoUnidadResuelto
                )
                .OrderByDescending(x => x.id_asignacion)
                .ToList();

            if (asignacionesCompatibles.Count == 0)
            {
                return null;
            }

            if (direccion.id_ruta.HasValue)
            {
                Asignaciones asignacionRutaDireccion = asignacionesCompatibles
                    .FirstOrDefault(x => x.id_ruta == direccion.id_ruta.Value);
                if (asignacionRutaDireccion != null)
                {
                    idRuta = asignacionRutaDireccion.id_ruta;
                    return asignacionRutaDireccion;
                }
            }

            if (!direccion.latitud.HasValue || !direccion.longitud.HasValue)
            {
                return null;
            }

            List<int> rutasConUnidadCompatible = asignacionesCompatibles
                .Select(x => x.id_ruta)
                .Distinct()
                .ToList();

            List<Rutas> rutas = context.Rutas
                .Where(x => rutasConUnidadCompatible.Contains(x.id_ruta))
                .ToList();

            Loc coordenadasDireccion = new Loc(direccion.latitud.Value, direccion.longitud.Value);
            foreach (var ruta in rutas)
            {
                List<Loc> puntos = ruta.GeoRuta
                    .Select(x => new Loc(x.latitud, x.longitud))
                    .ToList();

                if (puntos.Count > 0 && IsPointInPolygon(puntos, coordenadasDireccion))
                {
                    int rutaSeleccionada = ruta.id_ruta;
                    idRuta = rutaSeleccionada;
                    return asignacionesCompatibles.First(x => x.id_ruta == rutaSeleccionada);
                }
            }

            var rutaMasCercana = rutas
                .SelectMany(ruta => ruta.GeoRuta.Select(punto => new
                {
                    ruta.id_ruta,
                    distancia = new System.Device.Location.GeoCoordinate(
                        direccion.latitud.Value,
                        direccion.longitud.Value
                    ).GetDistanceTo(new System.Device.Location.GeoCoordinate(
                        punto.latitud,
                        punto.longitud
                    ))
                }))
                .OrderBy(x => x.distancia)
                .FirstOrDefault();

            if (rutaMasCercana == null)
            {
                return null;
            }

            int rutaCercanaSeleccionada = rutaMasCercana.id_ruta;
            idRuta = rutaCercanaSeleccionada;
            return asignacionesCompatibles.First(x => x.id_ruta == rutaCercanaSeleccionada);
        }

        private static bool esProductoAguaEspecial(int idProducto)
        {
            return idProducto == 7 || idProducto == 8 || idProducto == 10 || idProducto == 14;
        }

        private int calculaIdServicio(List<objDetalle> detalles, ContextCombugasDataContext context)
        {
            if (detalles == null || detalles.Count == 0 || detalles.Any(x => x == null))
            {
                return 0;
            }

            List<int> claves = detalles.Select(x => x.clave).Distinct().ToList();
            var serviciosProductos = context.producto
                .Where(x => claves.Contains(x.id_producto))
                .Select(x => new { x.id_producto, x.id_servicio })
                .ToList();

            if (serviciosProductos.Count != claves.Count)
            {
                return 0;
            }

            bool tieneGas = false;
            bool tieneAgua = false;
            bool tieneCroquetas = false;

            foreach (objDetalle item in detalles)
            {
                var producto = serviciosProductos.First(x => x.id_producto == item.clave);
                if (!producto.id_servicio.HasValue)
                {
                    return 0;
                }

                switch (producto.id_servicio.Value)
                {
                    case 1:
                        tieneGas = true;
                        break;
                    case 3:
                        tieneAgua = true;
                        break;
                    case 9:
                        tieneCroquetas = true;
                        break;
                    default:
                        return 0;
                }
            }

            return obtieneIdServicioPedido(tieneGas, tieneAgua, tieneCroquetas);
        }

        private static int obtieneIdServicioPedido(bool tieneGas, bool tieneAgua, bool tieneCroquetas)
        {
            if (tieneGas && tieneAgua && tieneCroquetas)
            {
                return 12;
            }
            if (tieneGas && tieneCroquetas)
            {
                return 10;
            }
            if (tieneAgua && tieneCroquetas)
            {
                return 11;
            }
            if (tieneGas && tieneAgua)
            {
                return 6;
            }
            if (tieneCroquetas)
            {
                return 9;
            }
            if (tieneAgua)
            {
                return 3;
            }
            if (tieneGas)
            {
                return 1;
            }

            return 0;
        }

        private static int traeRutaAwa(int idDireccion)
        {
            int idRutaAwa = 0;
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                #region consultas de dirección y rutas
                var coordenadas = (
                    from SQLDireccion in context.Direccion
                    where SQLDireccion.id_direccion.Equals(idDireccion)
                    select new
                    {
                        _lat = SQLDireccion.latitud,
                        _long = SQLDireccion.longitud
                    }
                ).FirstOrDefault();

                var geocercasawa = (
                    from SQLRutas in context.Rutas
                    where SQLRutas.id_tipo_ruta.Equals(3)
                    select new
                    {
                        _idRuta = SQLRutas.id_ruta,
                        _geocercasCarb = (
                            from GeoRutas in SQLRutas.GeoRuta
                            select new
                            {
                                _geoRuta = GeoRutas.id_georuta,
                                _geoLat = GeoRutas.latitud,
                                _geoLong = GeoRutas.longitud
                            }
                        )
                    }
                );
                #endregion

                #region loop para detectar si las coordenadas de dirección corresponden a una geocerca
                foreach (var ruta in geocercasawa)
                {
                    List<Loc> puntos = new List<Loc>();
                    foreach (var geocerca in ruta._geocercasCarb)
                    {
                        puntos.Add(new Loc(geocerca._geoLat, geocerca._geoLong));
                    }
                    if (puntos.Count > 0)
                    {
                        bool estaengeocerca = IsPointInPolygon(puntos, new Loc((Double)coordenadas._lat, (Double)coordenadas._long));
                        if (estaengeocerca)
                        {
                            idRutaAwa = ruta._idRuta;
                            break;
                        }
                    }
                }
                #endregion

                #region loop para medir la distancia entre los pines para deterimar ruta mas cercana
                if (idRutaAwa.Equals(0))
                {
                    List<distanciasCoordenadas> distancias = new List<distanciasCoordenadas>();
                    foreach (var ruta in geocercasawa)
                    {
                        foreach (var geocerca in ruta._geocercasCarb)
                        {
                            var coordenadasdireccion = new System.Device.Location.GeoCoordinate((double)coordenadas._lat, (double)coordenadas._long);
                            var coordenadaspin = new System.Device.Location.GeoCoordinate(geocerca._geoLat, geocerca._geoLong);
                            double distancia = coordenadasdireccion.GetDistanceTo(coordenadaspin);
                            distancias.Add(new distanciasCoordenadas(ruta._idRuta, geocerca._geoRuta, distancia, geocerca._geoLat, geocerca._geoLong));
                        }
                    }
                    if (distancias.Count > 0)
                    {
                        double menordistancia = distancias.Min(dis => dis.distancia);
                        idRutaAwa = distancias.Where(dis => dis.distancia.Equals(menordistancia)).FirstOrDefault().idRuta;
                    }
                }
                return idRutaAwa;
                #endregion
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static int traeRutaAutotanque(double _lat, double _long)
        {
            int idRutaAutotanque = 0;
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                #region consultas de dirección y rutas
                var coordenadas = new { _lat, _long };
                var geocercasautotanque = (
                    from SQLRutas in context.Rutas
                    where SQLRutas.id_tipo_ruta.Equals(2)
                    select new
                    {
                        _idRuta = SQLRutas.id_ruta,
                        _geocercasCarb = (
                            from GeoRutas in SQLRutas.GeoRuta
                            select new
                            {
                                _geoRuta = GeoRutas.id_georuta,
                                _geoLat = GeoRutas.latitud,
                                _geoLong = GeoRutas.longitud
                            }
                        )
                    }
                );
                #endregion

                #region loop para detectar si las coordenadas de dirección corresponden a una geocerca
                foreach (var ruta in geocercasautotanque)
                {
                    List<Loc> puntos = new List<Loc>();
                    foreach (var geocerca in ruta._geocercasCarb)
                    {
                        puntos.Add(new Loc(geocerca._geoLat, geocerca._geoLong));
                    }
                    if (puntos.Count > 0)
                    {
                        bool estaengeocerca = IsPointInPolygon(puntos, new Loc((Double)coordenadas._lat, (Double)coordenadas._long));
                        if (estaengeocerca)
                        {
                            idRutaAutotanque = ruta._idRuta;
                            break;
                        }
                    }
                }
                #endregion

                #region loop para medir la distancia entre los pines para deterimar ruta mas cercana
                if (idRutaAutotanque.Equals(0))
                {
                    List<distanciasCoordenadas> distancias = new List<distanciasCoordenadas>();
                    foreach (var ruta in geocercasautotanque)
                    {
                        foreach (var geocerca in ruta._geocercasCarb)
                        {
                            var coordenadasdireccion = new System.Device.Location.GeoCoordinate((double)coordenadas._lat, (double)coordenadas._long);
                            var coordenadaspin = new System.Device.Location.GeoCoordinate(geocerca._geoLat, geocerca._geoLong);
                            double distancia = coordenadasdireccion.GetDistanceTo(coordenadaspin);
                            distancias.Add(new distanciasCoordenadas(ruta._idRuta, geocerca._geoRuta, distancia, geocerca._geoLat, geocerca._geoLong));
                        }
                    }
                    if (distancias.Count > 0)
                    {
                        double menordistancia = distancias.Min(dis => dis.distancia);
                        idRutaAutotanque = distancias.Where(dis => dis.distancia.Equals(menordistancia)).FirstOrDefault().idRuta;
                    }
                }
                #endregion

                return idRutaAutotanque;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static bool IsPointInPolygon(List<Loc> poly, Loc point)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if ((((poly[i].Lt <= point.Lt) && (point.Lt < poly[j].Lt)) || ((poly[j].Lt <= point.Lt) && (point.Lt < poly[i].Lt))) && (point.Lg < (poly[j].Lg - poly[i].Lg) * (point.Lt - poly[i].Lt) / (poly[j].Lt - poly[i].Lt) + poly[i].Lg))
                {
                    c = !c;
                }
            }
            return c;
        }
    }

    #region OBJETOS
    class objDetalle
    {
        public int clave { get; set; }
        public double cantidad { get; set; }
        public double importe { get; set; }
        public string descripcionProducto { get; set; }
    }
    public class Loc
    {
        private double lt;
        private double lg;

        public double Lg
        {
            get { return lg; }
            set { lg = value; }
        }

        public double Lt
        {
            get { return lt; }
            set { lt = value; }
        }

        public Loc(double lt, double lg)
        {
            this.lt = lt;
            this.lg = lg;
        }

        public Loc()
        {
        }
    }
    public class distanciasCoordenadas
    {
        public int idRuta { get; set; }
        public int idGeoRutaPin { get; set; }
        public double distancia { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }

        public distanciasCoordenadas(int ruta, int georuta, double distancia, double latitud, double longitud)
        {
            this.idRuta = ruta;
            this.idGeoRutaPin = georuta;
            this.distancia = distancia;
            this.latitud = latitud;
            this.longitud = longitud;
        }
    }

    public class distanciasCarb
    {
        public int id_ec { get; set; }
        public double distancia { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }

        public distanciasCarb(int id_ec, double distancia, double latitud, double longitud)
        {
            this.id_ec = id_ec;
            this.distancia = distancia;
            this.latitud = latitud;
            this.longitud = longitud;
        }
    }

    class objCOMDATA
    {
        public string nombre { get; set; }
        public string dominio { get; set; }
        public string nroSerie { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public double altitud { get; set; }
        public double curso { get; set; }
        public int cantidad_de_satelites { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
    }
    #endregion
}
