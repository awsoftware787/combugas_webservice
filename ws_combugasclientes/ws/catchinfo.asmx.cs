using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml.Linq;
using ws_combugasclientes.core;
using Geocoding;
using Geocoding.Google;
using System.ComponentModel;
using System.Web.Script.Services;

namespace ws_combugasclientes.ws
{
    /// <summary>
    /// Descripción breve de catchinfo
    /// </summary>
    [WebService(Namespace = "awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [ScriptService]
    public class catchinfo : WebService
    {

        public class PedidoTabla
        {
            public int id_pedido { get; set; }
            public int id_cliente { get; set; }
            public int id_telefono { get; set; }
            public double? latitud { get; set; }
            public double? longitud { get; set; }
            public double inmetros { get; set; }

            public PedidoTabla()
            {
                this.id_pedido = 0;
                this.id_cliente = 0;
                this.id_telefono = 0;
                this.latitud = 0;
                this.longitud = 0;
                this.inmetros = 0;
            }
            public PedidoTabla(int id_pedido, int id_cliente, int id_telefono, double? latitud, double? longitud, double inmetros) {
                this.id_pedido = id_pedido;
                this.id_cliente = id_cliente;
                this.id_telefono = id_telefono;
                this.latitud = latitud;
                this.longitud = longitud;
                this.inmetros = inmetros;
            }

        }
        class objInfoBip
        {
            public string from { get; set; }
            public string to { get; set; }
            public string text { get; set; }
        }

        [WebMethod]
        public string GuardarDatosServicio(int CVENTA, DateTime FECHA, string SERVICIO, decimal VOLUMEN, decimal PRECIO, string TELEFONO, string LATITUD, string LONGITUD, string VEHICULO)
        {
            string estatus = "ERROR";
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            List<PedidoTabla> lista = new List<PedidoTabla>();
            var jsonSerializer = new JavaScriptSerializer();
            string MAPKEY = ConfigurationManager.AppSettings["KEY_MAPS"];
            DateTime actual = DateTime.Now;
            bool continuar = true;
            string carburacion_on = "";
            int metros_minimos = int.Parse(ConfigurationManager.AppSettings["metros_minimos"]); ;
            var longi = Convert.ToDouble(LONGITUD);
            var lat = Convert.ToDouble(LATITUD);
            var que_paso = "";
            //string address = "123 something st, somewhere";
            //string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            //WebRequest request = WebRequest.Create(requestUri);
            //WebResponse response = request.GetResponse();
            //XDocument xdoc = XDocument.Load(response.GetResponseStream());

            //XElement result = xdoc.Element("GeocodeResponse").Element("result");
            //XElement locationElement = result.Element("geometry").Element("location");
            //XElement lat = locationElement.Element("lat");
            //XElement lng = locationElement.Element("lng");
            try
            {
                int online_luis_alan = int.Parse(ConfigurationManager.AppSettings["Productivo_online_luis"]);
                if (online_luis_alan == 1)
                {

                    string textoMensaje = "INFO TARJETA AT-" + VEHICULO + ". SERVICIO:" + SERVICIO + ", CLAVE VENTA:" + CVENTA + ", LITROS: " + VOLUMEN + ", PRECIO: " + PRECIO + ", COORDENADAS: (" + LATITUD + "," + LONGITUD + "). FECHA: " + FECHA;
                    
                    enviarSMS("8711579303", textoMensaje);
                    enviarSMS("8711137045", textoMensaje);
                    enviarSMS("8711996475", textoMensaje);

                    que_paso = que_paso + "[se envio mensaje, line:93]";
                }


                #region SI VIENE CON CVENTA
                if (CVENTA != 0)
                {
                    que_paso = que_paso + "[si trae clave, line:101]";

                    #region SI TRAE COORDENADAS
                    if (longi < 0 && lat > 0)
                    {
                        que_paso = que_paso + "[si trae coordenadas, line:101]";

                        #region CARBURACIONES
                        var consulta = (from car in context.estaciones_carburacion where car.ec_status == true select car);
                        if (consulta.Count() > 0) { que_paso = que_paso + "[si hay carburaciones, line:108]"; }
                        foreach (var cb in consulta)
                        {
                            double metros = GetDistance(longi, lat, cb.ec_longitud, cb.ec_latitud);
                            if (metros < 50)
                            {
                                continuar = false;
                                carburacion_on = cb.ec_descripcion;
                            }
                        }
                        #endregion

                        que_paso = que_paso + "[paso carburaciones, line:121]";

                        #region NO ESTA EN CARBURACION

                        if (continuar)
                        {
                            que_paso = que_paso + "[consultara pedidos, line:127]";

                            #region CONSULTAR PROCEDIMIENTO
                            var consultasp = context.sp_pedidoscercanostarjeta(FECHA, VEHICULO).ToList();
                            #endregion

                            que_paso = que_paso + "[consulto pedidos, line:133]";

                            #region SI TRAE PEDIDOS
                            if (consultasp.Count() > 0) // si trae datos la unidad en la fecha
                            {
                                que_paso = que_paso + "[trae pedidos, line:138]";

                                #region Recorrer Resultados
                                foreach (var item in consultasp)
                                {
                                    double casaLat = 0;
                                    double casaLon = 0;
                                    string direccion = "";
                                    double metros = 0.0;
                                    int id_tel = item.Id_Telefono;
                                    int online = int.Parse(ConfigurationManager.AppSettings["Productivo_online"]);

                                    #region COORDENADAS GEOCODER
                                    if (item.lat_B == null || item.lat_B < 0)
                                    {
                                        direccion = item.direccionT;
                                        direccion = direccion.Replace(" NO APLICA,", "");
                                        direccion = direccion.Replace(", ENTRE:", "");
                                        direccion = direccion.Replace(" INTERIOR: ,", "");
                                        direccion = direccion.Replace(" -", "");
                                        direccion = direccion.Replace("\r", "");
                                        direccion = direccion.Replace("\n", "");
                                        direccion = direccion.Replace("  ", "");
                                        IGeocoder geocoder = new GoogleGeocoder() { ApiKey = MAPKEY };
                                        IEnumerable<Address> addresses = geocoder.Geocode(direccion);
                                        casaLat = addresses.First().Coordinates.Latitude;
                                        casaLon = addresses.First().Coordinates.Longitude;

                                        //string address = "123 something st, somewhere";
                                        //string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

                                        //WebRequest request = WebRequest.Create(requestUri);
                                        //WebResponse response = request.GetResponse();
                                        //XDocument xdoc = XDocument.Load(response.GetResponseStream());

                                        //XElement result = xdoc.Element("GeocodeResponse").Element("result");
                                        //XElement locationElement = result.Element("geometry").Element("location");
                                        //XElement lat = locationElement.Element("lat");
                                        //XElement lng = locationElement.Element("lng");

                                        Direccion d = new Direccion();
                                        d = context.Direccion.Where(x => x.id_direccion == item.id_direccion).FirstOrDefault();
                                        d.lat_B = casaLat;
                                        d.lon_B = casaLon;
                                        context.SubmitChanges();
                                    }
                                    else
                                    {
                                        casaLat = (double)item.lat_B;
                                        casaLon = (double)item.lon_B;
                                    }
                                    #endregion

                                    #region CALCULO DE DISTANCIA
                                    metros = GetDistance(longi, lat, casaLon, casaLat);
                                    if (metros > metros_minimos)//GEOCODES FALLO
                                    {
                                        metros = GetDistance(longi, lat, item.longitud, item.latitud);
                                        casaLon = (double)item.longitud;
                                        casaLat = (double)item.latitud;
                                    }
                                    #endregion

                                    #region AGREGAR A LISTA
                                    lista.Add(new PedidoTabla(
                                        item.Id_Pedido,
                                        item.Id_Cliente,
                                        item.Id_Telefono,
                                        casaLat,
                                        casaLon,
                                        metros
                                    ));
                                    #endregion

                                }
                                #endregion

                                que_paso = que_paso + "[lleno pedidos, line:202]";

                                #region Lista Contiene Resultados
                                if (lista.Count > 0)
                                {

                                    // ORDENAR EL  MAS CERCANO
                                    lista.Sort((p, q) => p.inmetros.CompareTo(q.inmetros));
                                    //PEDIDO
                                    int id_pedido = lista[0].id_pedido;
                                    // ID TELEFONO AL QUE SE ENVIARA
                                    int id_telefono = lista[0].id_telefono;
                                    // TELEFONO CON MASCARA
                                    var tel = (from Tel in context.Telefono where Tel.id_telefono.Equals(id_telefono) select Tel.no_telefono).FirstOrDefault();
                                    // TELEFONO SIN MASCARA
                                    string telefono = new String(tel.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono
                                    // ID CLIENTE
                                    int id_cliente = lista[0].id_cliente;
                                    //NOMBRE DEL CLIENTE
                                    var nombreCliente = (from cli in context.Cliente where cli.id_cliente.Equals(id_cliente) select cli.nombre).FirstOrDefault();
                                    //TOTAL SURTIDO
                                    decimal Total = VOLUMEN * PRECIO;
                                    //TEXTO A ENVIAR
                                    string textoMensaje = "ESTIMADO CLIENTE COMBUGAS LE INFORMA QUE EN ESTE MOMENTO SE LE SURTIERON " + VOLUMEN + " LITROS DE NUESTRO AUTOTANQUE NUMERO " +
                                        VEHICULO + " EL IMPORTE FUE DE: " + Total + " PESOS A UN PRECIO POR LITRO DE: " + PRECIO + " PESOS, GRACIAS POR SU PREFERENCIA.";
                                    // METROS 
                                    double metros = lista[0].inmetros;

                                    #region EN LINEA
                                    int online = int.Parse(ConfigurationManager.AppSettings["Productivo_online"]);
                                    int online_luis = int.Parse(ConfigurationManager.AppSettings["Productivo_online_luis"]);
                                    #endregion

                                    //int cuenta_celular_permitido = (int)(from tels in context.Telefono where tels.id_telefono == id_telefono select tels.id_telefono).Count();//--&& tels.tipo_telefono.Contains("CELULAR")
                                    int cuenta_celular_permitido = 1;

                                    que_paso = que_paso + "[lleno datos, line:238]";

                                    int direccion_actua = int.Parse(ConfigurationManager.AppSettings["direcAct"]);

                                    #region SI METROS MAYOR A 150
                                    if (metros > metros_minimos)
                                    {
                                        Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                                        tm.SERVICIO = SERVICIO;
                                        tm.LONGITUD = LONGITUD;
                                        tm.LATITUD = LATITUD;
                                        tm.PRECIO = PRECIO;
                                        tm.TELEFONO = TELEFONO;
                                        tm.CVENTA = CVENTA;
                                        tm.FECHA = FECHA;
                                        tm.ID_CLIENTE = id_cliente;
                                        tm.ID_TELEFONO = id_telefono;
                                        tm.VEHICULO = VEHICULO;
                                        tm.VOLUMEN = VOLUMEN;
                                        tm.METROS = metros;
                                        tm.OBSERVACIONES = "METROS MAYOR";
                                        tm.FECHA_SISTEMA = actual;
                                        tm.PEDIDO_RELACIONADO = id_pedido;
                                        context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                                        context.SubmitChanges();
                                        estatus = "OK";
                                        que_paso = que_paso + "[metros mayor, line:262]";

                                    }
                                    #endregion

                                    #region SI METROS MENOR O IGUAL A 150 y TIPO PERMITIDO
                                    else if (cuenta_celular_permitido >= 1)
                                    {
                                        Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                                        tm.SERVICIO = SERVICIO;
                                        tm.LONGITUD = LONGITUD;
                                        tm.LATITUD = LATITUD;
                                        tm.PRECIO = PRECIO;
                                        tm.TELEFONO = TELEFONO;
                                        tm.CVENTA = CVENTA;
                                        tm.FECHA = FECHA;
                                        tm.ID_CLIENTE = id_cliente;
                                        tm.ID_TELEFONO = id_telefono;
                                        tm.VEHICULO = VEHICULO;
                                        tm.VOLUMEN = VOLUMEN;
                                        tm.METROS = metros;
                                        tm.OBSERVACIONES = "CORRECTO ";
                                        tm.FECHA_SISTEMA = actual;
                                        tm.PEDIDO_RELACIONADO = id_pedido;
                                        context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                                        context.SubmitChanges();
                                        estatus = "OK";
                                        if (online == 1)
                                        {
                                            enviarSMS(telefono, textoMensaje);
                                            que_paso = que_paso + "[Envio Mensaje, line:292]";
                                        }
                                        que_paso = que_paso + "[correcto, line:294]";
                                    }
                                    #endregion

                                    #region METROS MENOR IGUAL A 150 y TIPO NO PERMITIDO 
                                    else
                                    {
                                        Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                                        tm.SERVICIO = SERVICIO;
                                        tm.LONGITUD = LONGITUD;
                                        tm.LATITUD = LATITUD;
                                        tm.PRECIO = PRECIO;
                                        tm.TELEFONO = TELEFONO;
                                        tm.CVENTA = CVENTA;
                                        tm.FECHA = FECHA;
                                        tm.ID_CLIENTE = id_cliente;
                                        tm.ID_TELEFONO = id_telefono;
                                        tm.VEHICULO = VEHICULO;
                                        tm.VOLUMEN = VOLUMEN;
                                        tm.METROS = metros;
                                        tm.OBSERVACIONES = " TIPO TELEFONICO";
                                        tm.FECHA_SISTEMA = actual;
                                        tm.PEDIDO_RELACIONADO = id_pedido;
                                        context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                                        context.SubmitChanges();
                                        estatus = "OK";
                                        if (online == 1)
                                        {
                                            enviarSMS(telefono, textoMensaje);
                                        }
                                        que_paso = que_paso + "[tipo no permitido, line:324]";
                                    }
                                    #endregion

                                }
                                #endregion

                                #region Lista No Contiene Resutlados
                                else
                                {
                                    que_paso = que_paso + "[lista sin pedidos, line:334]";
                                    //int id_unidad = (from uni in context.truck where uni.numero.Contains(VEHICULO) && uni.id_tipounidad == 2 select uni.id_truck).FirstOrDefault();
                                    //int id_operador = (
                                    //    from SQLAsignacionRuta in context.Asignaciones
                                    //    where SQLAsignacionRuta.id_truck.Equals(id_unidad) && SQLAsignacionRuta.asignacion_activa.Equals(true)
                                    //    select SQLAsignacionRuta.id_operador
                                    //).FirstOrDefault();

                                    //int ulti_pedido = (from pd in context.Pedido_Detalle
                                    //                   join rp in context.Radios_Pedidos on pd.Id_Pedido equals rp.id_pedido
                                    //                   where pd.id_operador == id_operador && pd.id_producto_entregado == 9
                                    //                   && pd.id_servicio == 1 && pd.tipo_unidad == 2 && pd.Pedido.status_pedido == true && pd.Pedido.Completo == false
                                    //                   && pd.Pedido.Pedido_confirmado_operador == true && rp.id_operador == id_operador && rp.tipo.Equals("ENTREGA")
                                    //                   orderby rp.fecha descending
                                    //                   select pd.Id_Pedido
                                    //                   ).FirstOrDefault();
                                    //int cuenta_existencia = (from tm in context.Tarjeta_Mensajes where tm.PEDIDO_RELACIONADO.Equals(ulti_pedido)
                                    //                         && tm.FECHA <= actual select tm.id_tarjeta_mensaje).Count();
                                    //if (cuenta_existencia == 0)
                                    //{
                                    //    // ID TELEFONO AL QUE SE ENVIARA
                                    //    int id_telefono = (from uni in context.Pedido where uni.Id_Pedido == ulti_pedido select uni.Id_Telefono).FirstOrDefault();
                                    //    // TELEFONO CON MASCARA
                                    //    var tel = (from Tel in context.Telefono where Tel.id_telefono.Equals(id_telefono) select Tel.no_telefono).FirstOrDefault();
                                    //    // TELEFONO SIN MASCARA
                                    //    string telefono = new String(tel.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono
                                    //                                                                     // ID CLIENTE
                                    //    int id_cliente = (from uni in context.Pedido where uni.Id_Pedido == ulti_pedido select uni.Id_Cliente).FirstOrDefault();
                                    //    //NOMBRE DEL CLIENTE
                                    //    var nombreCliente = (from cli in context.Cliente where cli.id_cliente.Equals(id_cliente) select cli.nombre).FirstOrDefault();
                                    //    //TOTAL SURTIDO
                                    //    decimal Total = VOLUMEN * PRECIO;
                                    //    //TEXTO A ENVIAR
                                    //    string textoMensaje = "ESTIMADO CLIENTE COMBUGAS LE INFORMA QUE EN ESTE MOMENTO SE LE SURTIERON " + VOLUMEN + " LITROS DE NUESTRO AUTOTANQUE NUMERO " +
                                    //        VEHICULO + " EL IMPORTE FUE DE: " + Total + " PESOS A UN PRECIO POR LITRO DE: " + PRECIO + " PESOS, GRACIAS POR SU PREFERENCIA.";
                                    //    int online_luis = int.Parse(ConfigurationManager.AppSettings["Productivo_online_luis"]);

                                    //    Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                                    //    tm.SERVICIO = SERVICIO;
                                    //    tm.LONGITUD = LONGITUD;
                                    //    tm.LATITUD = LATITUD;
                                    //    tm.PRECIO = PRECIO;
                                    //    tm.TELEFONO = TELEFONO;
                                    //    tm.CVENTA = CVENTA;
                                    //    tm.FECHA = FECHA;
                                    //    tm.ID_CLIENTE = id_cliente;
                                    //    tm.ID_TELEFONO = id_telefono;
                                    //    tm.VEHICULO = VEHICULO;
                                    //    tm.VOLUMEN = VOLUMEN;
                                    //    tm.METROS = 0;
                                    //    tm.OBSERVACIONES = " SE ENVIA MENSAJE AL ULTIMO REGISTRADO, QUE NO ESTE EN TABLA DE MENSAJES ENVIADOS, SUCEDE CUANDO SE ENTREGA EL PEDIDO ANTES DE SURTIR";
                                    //    tm.FECHA_SISTEMA = actual;
                                    //    tm.PEDIDO_RELACIONADO = ulti_pedido;
                                    //    context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                                    //    context.SubmitChanges();
                                    //    string mensaje_metros_mal = "PEDIDO: " + ulti_pedido + ", PEDIDO SURTIDO ANTES DE DESPACHAR TELEFONO: " + telefono;
                                    //    if (online_luis == 1)
                                    //    {
                                    //        enviarSMS("8711579303", mensaje_metros_mal);
                                    //    }

                                    //}

                                    estatus = "OK";
                                }
                                #endregion
                            }
                            #endregion
                            #region NO HAY DATOS
                            else
                            {
                                que_paso = que_paso + "[no hay datos, line:405]";
                                //int id_unidad = (from uni in context.truck where uni.numero.Contains(VEHICULO) && uni.id_tipounidad == 2 select uni.id_truck).FirstOrDefault();
                                //int id_operador = (
                                //    from SQLAsignacionRuta in context.Asignaciones
                                //    where SQLAsignacionRuta.id_truck.Equals(id_unidad) && SQLAsignacionRuta.asignacion_activa.Equals(true)
                                //    select SQLAsignacionRuta.id_operador
                                //).FirstOrDefault();

                                //int ulti_pedido = (from pd in context.Pedido_Detalle
                                //                   join rp in context.Radios_Pedidos on pd.Id_Pedido equals rp.id_pedido
                                //                   where pd.id_operador == id_operador && pd.id_producto_entregado == 9
                                //                   && pd.id_servicio == 1 && pd.tipo_unidad == 2 && pd.Pedido.status_pedido == true && pd.Pedido.Completo == false
                                //                   && pd.Pedido.Pedido_confirmado_operador == true && rp.id_operador == id_operador && rp.tipo.Equals("ENTREGA")
                                //                   orderby rp.fecha descending
                                //                   select pd.Id_Pedido
                                //                   ).FirstOrDefault();
                                //int cuenta_existencia = (from tm in context.Tarjeta_Mensajes
                                //                         where tm.PEDIDO_RELACIONADO.Equals(ulti_pedido)
                                //                         && tm.FECHA <= actual
                                //                         select tm.id_tarjeta_mensaje).Count();
                                //if (cuenta_existencia == 0)
                                //{
                                //    // ID TELEFONO AL QUE SE ENVIARA
                                //    int id_telefono = (from uni in context.Pedido where uni.Id_Pedido == ulti_pedido select uni.Id_Telefono).FirstOrDefault();
                                //    // TELEFONO CON MASCARA
                                //    var tel = (from Tel in context.Telefono where Tel.id_telefono.Equals(id_telefono) select Tel.no_telefono).FirstOrDefault();
                                //    // TELEFONO SIN MASCARA
                                //    string telefono = new String(tel.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono
                                //                                                                     // ID CLIENTE
                                //    int id_cliente = (from uni in context.Pedido where uni.Id_Pedido == ulti_pedido select uni.Id_Cliente).FirstOrDefault();
                                //    //NOMBRE DEL CLIENTE
                                //    var nombreCliente = (from cli in context.Cliente where cli.id_cliente.Equals(id_cliente) select cli.nombre).FirstOrDefault();
                                //    //TOTAL SURTIDO
                                //    decimal Total = VOLUMEN * PRECIO;
                                //    //TEXTO A ENVIAR
                                //    string textoMensaje = "ESTIMADO CLIENTE COMBUGAS LE INFORMA QUE EN ESTE MOMENTO SE LE SURTIERON " + VOLUMEN + " LITROS DE NUESTRO AUTOTANQUE NUMERO " +
                                //        VEHICULO + " EL IMPORTE FUE DE: " + Total + " PESOS A UN PRECIO POR LITRO DE: " + PRECIO + " PESOS, GRACIAS POR SU PREFERENCIA.";
                                //    int online_luis = int.Parse(ConfigurationManager.AppSettings["Productivo_online_luis"]);

                                //    Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                                //    tm.SERVICIO = SERVICIO;
                                //    tm.LONGITUD = LONGITUD;
                                //    tm.LATITUD = LATITUD;
                                //    tm.PRECIO = PRECIO;
                                //    tm.TELEFONO = TELEFONO;
                                //    tm.CVENTA = CVENTA;
                                //    tm.FECHA = FECHA;
                                //    tm.ID_CLIENTE = id_cliente;
                                //    tm.ID_TELEFONO = id_telefono;
                                //    tm.VEHICULO = VEHICULO;
                                //    tm.VOLUMEN = VOLUMEN;
                                //    tm.METROS = 0;
                                //    tm.OBSERVACIONES = " SE ENVIA MENSAJE AL ULTIMO REGISTRADO, QUE NO ESTE EN TABLA DE MENSAJES ENVIADOS, SUCEDE CUANDO SE ENTREGA EL PEDIDO ANTES DE SURTIR";
                                //    tm.FECHA_SISTEMA = actual;
                                //    tm.PEDIDO_RELACIONADO = ulti_pedido;
                                //    context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                                //    context.SubmitChanges();
                                //    string mensaje_metros_mal = "PEDIDO: " + ulti_pedido + ", PEDIDO SURTIDO ANTES DE DESPACHAR TELEFONO 2: " + telefono;
                                //    if (online_luis == 1)
                                //    {
                                //        enviarSMS("8711579303", mensaje_metros_mal);
                                //    }
                                //}

                                estatus = "OK";
                            }
                            #endregion

                        }
                        #endregion
                        #region SI ESTA EN CARBURACION
                        else
                        {
                            Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                            tm.SERVICIO = SERVICIO;
                            tm.LONGITUD = LONGITUD;
                            tm.LATITUD = LATITUD;
                            tm.PRECIO = PRECIO;
                            tm.TELEFONO = TELEFONO;
                            tm.CVENTA = CVENTA;
                            tm.FECHA = FECHA;
                            tm.VEHICULO = VEHICULO;
                            tm.VOLUMEN = VOLUMEN;
                            tm.OBSERVACIONES = "" + "EN CARBURACION: " + carburacion_on;
                            tm.FECHA_SISTEMA = actual;
                            context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                            context.SubmitChanges();
                            estatus = "EN_CARBURACION";
                            que_paso = que_paso + "[EN_CARBURACION, line:493]";
                        }
                        #endregion
                    }
                    #endregion
                    #region ELSE COORDENADAS
                    else
                    {
                        Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                        tm.SERVICIO = SERVICIO;
                        tm.LONGITUD = LONGITUD;
                        tm.LATITUD = LATITUD;
                        tm.PRECIO = PRECIO;
                        tm.TELEFONO = TELEFONO;
                        tm.CVENTA = CVENTA;
                        tm.FECHA = FECHA;
                        tm.VEHICULO = VEHICULO;
                        tm.VOLUMEN = VOLUMEN;
                        tm.OBSERVACIONES = "" + "Sin coordenadas ";
                        tm.FECHA_SISTEMA = actual;
                        context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                        context.SubmitChanges();
                        estatus = "SIN_COORDENADAS";
                        que_paso = que_paso + "[SIN_COORDENADAS, line:514]";
                    }
                    #endregion

                }
                #endregion
                #region ELSE CLAVE
                else
                {
                    Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                    tm.SERVICIO = SERVICIO;
                    tm.LONGITUD = LONGITUD;
                    tm.LATITUD = LATITUD;
                    tm.PRECIO = PRECIO;
                    tm.TELEFONO = TELEFONO;
                    tm.CVENTA = CVENTA;
                    tm.FECHA = FECHA;
                    tm.VEHICULO = VEHICULO;
                    tm.VOLUMEN = VOLUMEN;
                    tm.OBSERVACIONES = "" + "Sin clave de venta ";
                    tm.FECHA_SISTEMA = actual;
                    context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                    context.SubmitChanges();
                    estatus = "SIN_CVENTA";
                    que_paso = que_paso + "[SIN_CVENTA, line:539]";

                }

                #endregion

                que_paso = que_paso + "[finaliza try, line:545]";
            }
            catch (Exception ex)
            {
                estatus = "ERROR";

                Tarjeta_Mensajes tm = new Tarjeta_Mensajes();
                tm.SERVICIO = SERVICIO;
                tm.LONGITUD = LONGITUD;
                tm.LATITUD = LATITUD;
                tm.PRECIO = PRECIO;
                tm.TELEFONO = TELEFONO;
                tm.CVENTA = CVENTA;
                tm.FECHA = FECHA;
                tm.VEHICULO = "C00ERROR";
                tm.VOLUMEN = VOLUMEN;
                tm.OBSERVACIONES = "" + ex.Message+"**"+ que_paso;
                tm.FECHA_SISTEMA = actual;
                context.Tarjeta_Mensajes.InsertOnSubmit(tm);
                context.SubmitChanges();
            }

            return estatus;
        }


        [WebMethod]
        public string TEST_MENSAJES(string TELEFONO)
        {
            string estatus = "ERROR";
           

            try
            {
                string MAPKEY = ConfigurationManager.AppSettings["KEY_MAPS"];

                string address = "boulevard constitucion 385, ampliaciion los angeles, torreon";
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&key=" + MAPKEY, Uri.EscapeDataString(address));

                WebRequest request = WebRequest.Create(requestUri);
                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());

                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                XElement lat = locationElement.Element("lat");
                XElement lng = locationElement.Element("lng");

                enviarSMSNew(TELEFONO, "PRUEBA VELOCIDAD: "+DateTime.Now + "lati: "+lat);
                estatus = "OK";
                                        
            }
            catch (Exception ex)
            {
                estatus = "ERROR";

                
            }

            return estatus;
        }






        #region envio de sms por medio de la plataforma infobip
        void enviarSMS(string numero_telefonico, string mensaje)
        {
            //const string API_KEY = "502cc4a452abc23a7576ad4ea0439556-d7901f01-fdc5-4b8e-a9b0-4fcb9e07bd44";
            const string API_KEY = "0090b530e3d587082d04daa4ba0d3ac5-21a42973-713a-4498-8ae9-a7ed137b778d";
            objInfoBip objetoMensaje = new objInfoBip();
            objetoMensaje.from = "COMBUGAS";
            objetoMensaje.to = "+52" + numero_telefonico;
            objetoMensaje.text = mensaje;

            var jsonSerializer = new JavaScriptSerializer();
            var jsonINFO = jsonSerializer.Serialize(objetoMensaje);
            System.Diagnostics.Debug.WriteLine(jsonINFO);

            var client = new RestClient("https://lkdew.api.infobip.com/sms/2/text/single");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "App " + API_KEY);
            request.AddParameter("application/json", jsonINFO, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        #endregion

        #region envio de sms por medio de la plataforma infobip 22
        void enviarSMSNew(string numero_telefonico, string mensaje)
        {
            //const string API_KEY = "502cc4a452abc23a7576ad4ea0439556-d7901f01-fdc5-4b8e-a9b0-4fcb9e07bd44";
            const string API_KEY = "0090b530e3d587082d04daa4ba0d3ac5-21a42973-713a-4498-8ae9-a7ed137b778d";
            objInfoBip objetoMensaje = new objInfoBip();
            objetoMensaje.from = "COMBUGAS";
            objetoMensaje.to = "+52" + numero_telefonico;
            objetoMensaje.text = mensaje;

            var jsonSerializer = new JavaScriptSerializer();
            var jsonINFO = jsonSerializer.Serialize(objetoMensaje);
            System.Diagnostics.Debug.WriteLine(jsonINFO);

            var client = new RestClient("https://lkdew.api.infobip.com/sms/2/text/single");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "App " + API_KEY);
            request.AddParameter("application/json", jsonINFO, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        #endregion

        private static double GetDistance(double long1InDegrees, double lat1InDegrees, double? long2InDegrees, double? lat2InDegrees)
        {
            double lats = (double)(lat1InDegrees - lat2InDegrees);
            double lngs = (double)(long1InDegrees - long2InDegrees);

            //Paso a metros
            double latm = lats * 60 * 1852;
            double lngm = (lngs * Math.Cos((double)lat1InDegrees * Math.PI / 180)) * 60 * 1852;
            double distInMeters = Math.Sqrt(Math.Pow(latm, 2) + Math.Pow(lngm, 2));
            return distInMeters;
        }
    }
}
