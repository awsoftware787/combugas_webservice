using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using ws_combugasclientes.core;
using Geocoding;
using Geocoding.Google;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Device.Location;
using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Xml;
using System.IO;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace ws_combugasclientes.ws
{
    [WebService(Namespace = "http://awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Operadores : WebService
    {
        #region Clases
        public class OperadorLogin
        {
            public int id { get; set; }
            public int? idruta { get; set; }
            public string nombre { get; set; }
            public string apellido1 { get; set; }
            public string apellido2 { get; set; }
            public string ruta { get; set; }
            public string ayudante { get; set; }
            public int? tipoUni { get; set; }
            public int? tipo { get; set; }
            public int? id_truck { get; set; }
            public string no_economico { get; set; }

            public OperadorLogin()
            {
                this.id = 0;
                this.idruta = 0;
                this.nombre = "";
                this.apellido1 = "";
                this.apellido2 = "";
                this.ruta = "";
                this.ayudante = "";
                this.tipoUni = 0;
                this.tipo = 0;
                this.id_truck = 0;
                this.no_economico = "";
            }

            public OperadorLogin(int id, string nombre, string apellido1, string apellido2, string ruta, string ayudante, int? idruta, int? tipoUni, int? tipo, int? id_truck = null, string no_economico = null)
            {
                this.id = id;
                this.nombre = nombre;
                this.apellido1 = apellido1;
                this.apellido2 = apellido2;
                this.ruta = ruta;
                this.ayudante = ayudante;
                this.idruta = idruta;
                this.tipoUni = tipoUni;
                this.tipo = tipo;
                this.id_truck = id_truck;
                this.no_economico = no_economico;
            }
        }

        public class PedidoTabla
        {
            public int id { get; set; }
            public string cliente { get; set; }
            public string direccion { get; set; }
            public string tipo { get; set; }
            public string pedidos { get; set; }
            public string radios { get; set; }
            public string ACTITUD { get; set; }
            public string RECEPCIONISTA { get; set; }
            public string MANGUERA { get; set; }
            public string METODOPAGO { get; set; }
            public string RESTRICCION { get; set; }
            public string ATENCIONA { get; set; }
            public string CLAVET { get; set; }
            public bool? BASCULA { get; set; }
            public bool? CLAVE { get; set; }
            public bool? CREDITO { get; set; }
            public double? LAT { get; set; }
            public double? LON { get; set; }
            public string FECHAHORA { get; set; }
            public string FASE { get; set; }
            public string STATUS { get; set; }
            public bool req { get; set; }
            public bool obs { get; set; }
            public double inmetros { get; set; }
            public int casanova { get; set; }
            public bool? tieneIncidencia { get; set; }
            public bool? entregado { get; set; }
            public int? regresar { get; set; }
            public string ValeTodo { get; set; }

            public PedidoTabla()
            {
                this.id = 0;
                this.cliente = "";
                this.direccion = "";
                this.tipo = "";
                this.pedidos = "";
                this.radios = "";
                this.LAT = 0;
                this.LON = 0;
                this.ACTITUD = "";
                this.RECEPCIONISTA = "";
                this.METODOPAGO = "";
                this.MANGUERA = "";
                this.RESTRICCION = "";
                this.ATENCIONA = "";
                this.CLAVET = "";
                this.BASCULA = false;
                this.CLAVE = false;
                this.CREDITO = false;
                this.FECHAHORA = "";
                this.FASE = "";
                this.STATUS = "";
                this.req = false;
                this.obs = false;
                this.inmetros = 0;
                this.casanova = 0;
                this.tieneIncidencia = false;
                this.entregado = false;
                this.regresar = 0;
                this.ValeTodo = "";
            }

            public PedidoTabla(int id, string cliente, string direccion, string tipo, string pedidos, string radios, string RECEPCIONISTA, double? LAT,
                double? LON, string MANGUERA, string RESTRICCION, string ATENCIONA, string METODOPAGO, string CLAVET, bool? BASCULA
                , bool? CLAVE, bool? CREDITO, string FECHAHORA, string FASE, string STATUS, bool req, bool obs, double? inmetros, int casanova, bool? tieneIncidencia, bool? entregado, int? regresar, string ValeTodo)
            {
                this.id = id;
                this.cliente = cliente;
                this.direccion = direccion;
                this.tipo = tipo;
                this.pedidos = pedidos;
                this.radios = radios;
                this.LAT = LAT;
                this.LON = LON;
                this.RECEPCIONISTA = RECEPCIONISTA;
                this.MANGUERA = MANGUERA;
                this.RESTRICCION = RESTRICCION;
                this.ATENCIONA = ATENCIONA;
                this.METODOPAGO = METODOPAGO;
                this.CLAVET = CLAVET;
                this.BASCULA = BASCULA;
                this.CLAVE = CLAVE;
                this.CREDITO = CREDITO;
                this.FECHAHORA = FECHAHORA;
                this.FASE = FASE;
                this.STATUS = STATUS;
                this.req = req;
                this.obs = obs;
                this.inmetros = (double)inmetros;
                this.casanova = casanova;
                this.tieneIncidencia = tieneIncidencia;
                this.entregado = entregado;
                this.regresar = regresar;
                this.ValeTodo = ValeTodo;
            }
        }

        public class PDETA
        {
            public string PRODUCTO { get; set; }
            public int? IDPRODUCTO { get; set; }
            public int? aplica { get; set; }
            public string TIPOUNIDAD { get; set; }
            public string UNIDADLIQUIDACION { get; set; }
            public string CANTIDAD { get; set; }
            public string IMPORTE { get; set; }
            public string KILOS { get; set; }
            public string LITROS { get; set; }
            public string PINICIO { get; set; }
            public string PFINAL { get; set; }
            public int? ID_PD { get; set; }
            public decimal? preciokg { get; set; }
            public decimal? preciolt { get; set; }
            public PDETA()
            {
                this.ID_PD = 0;
                this.aplica = 0;
                this.preciokg = 0;
                this.preciolt = 0;
                this.IDPRODUCTO = 0;
                this.PFINAL = "";
                this.PINICIO = "";
                this.LITROS = "";
                this.KILOS = "";
                this.IMPORTE = "";
                this.CANTIDAD = "";
                this.UNIDADLIQUIDACION = "";
                this.TIPOUNIDAD = "";
                this.PRODUCTO = "";
            }
            public PDETA(int? ID_PD, string PRODUCTO, string TIPOUNIDAD, string UNIDADLIQUIDACION, string CANTIDAD, string IMPORTE, string KILOS, string LITROS, string PINICIO,
                string PFINAL, int? IDPRODUCTO, decimal? preciokg, decimal? preciolt, int? aplica)
            {
                this.ID_PD = ID_PD;
                this.aplica = aplica;
                this.preciokg = preciokg;
                this.preciolt = preciolt;
                this.IDPRODUCTO = IDPRODUCTO;
                this.PFINAL = PFINAL;
                this.PINICIO = PINICIO;
                this.LITROS = LITROS;
                this.KILOS = KILOS;
                this.IMPORTE = IMPORTE;
                this.CANTIDAD = CANTIDAD;
                this.UNIDADLIQUIDACION = UNIDADLIQUIDACION;
                this.TIPOUNIDAD = TIPOUNIDAD;
                this.PRODUCTO = PRODUCTO;
            }
        }

        public class PedidoV
        {
            public int? CLIENTE { get; set; }
            public int? DIRECCION { get; set; }
            public int? IDPEDIDO { get; set; }
            public string TIPOCLIENTE { get; set; }
            public string ACTITUD { get; set; }
            public string RECEPCIONISTA { get; set; }
            public string MANGUERA { get; set; }
            public string METODOPAGO { get; set; }
            public string RESTRICCION { get; set; }
            public string ATENCIONA { get; set; }
            public string CLAVET { get; set; }
            public bool? BASCULA { get; set; }
            public bool? CLAVE { get; set; }
            public bool? CREDITO { get; set; }
            public double? LAT { get; set; }
            public double? LON { get; set; }
            public PedidoV()
            {
                this.CLIENTE = 0;
                this.DIRECCION = 0;
                this.IDPEDIDO = 0;
                this.LAT = 0;
                this.LON = 0;
                this.TIPOCLIENTE = "";
                this.ACTITUD = "";
                this.RECEPCIONISTA = "";
                this.METODOPAGO = "";
                this.MANGUERA = "";
                this.RESTRICCION = "";
                this.ATENCIONA = "";
                this.CLAVET = "";
                this.BASCULA = false;
                this.CLAVE = false;
                this.CREDITO = false;
            }
            public PedidoV(int? CLIENTE, int? DIRECCION, int? IDPEDIDO, double? LAT, double? LON, string TIPOCLIENTE, string ACTITUD,
                string RECEPCIONISTA, string MANGUERA, string RESTRICCION, string ATENCIONA, string METODOPAGO, string CLAVET, bool? BASCULA
                , bool? CLAVE, bool? CREDITO)
            {
                this.CLIENTE = CLIENTE;
                this.DIRECCION = DIRECCION;
                this.IDPEDIDO = IDPEDIDO;
                this.LAT = LAT;
                this.LON = LON;
                this.TIPOCLIENTE = TIPOCLIENTE;
                this.ACTITUD = ACTITUD;
                this.RECEPCIONISTA = RECEPCIONISTA;
                this.MANGUERA = MANGUERA;
                this.RESTRICCION = RESTRICCION;
                this.ATENCIONA = ATENCIONA;
                this.METODOPAGO = METODOPAGO;
                this.CLAVET = CLAVET;
                this.BASCULA = BASCULA;
                this.CLAVE = CLAVE;
                this.CREDITO = CREDITO;
            }
        }

        public class REQUI
        {
            public string Requisito { get; set; }

            public REQUI()
            {
                this.Requisito = "";
            }
            public REQUI(string Requisito)
            {
                this.Requisito = Requisito;
            }
        }

        public class TANQUE
        {
            public int ID { get; set; }
            public int? Capacidad { get; set; }
            public string UNIDAD { get; set; }

            public TANQUE()
            {
                this.ID = 0;
                this.Capacidad = 0;
                this.UNIDAD = "";
            }
            public TANQUE(int ID, int? Capacidad, string UNIDAD)
            {
                this.ID = ID;
                this.Capacidad = Capacidad;
                this.UNIDAD = UNIDAD;
            }
        }

        public class INSTALACIONES
        {

            public string QUIEN { get; set; }
            public string AREA { get; set; }
            public string PUERTA { get; set; }
            public string HORARIO { get; set; }

            public INSTALACIONES()
            {
                this.QUIEN = "";
                this.AREA = "";
                this.PUERTA = "";
                this.HORARIO = "";
            }
            public INSTALACIONES(string QUIEN, string AREA, string PUERTA, string HORARIO)
            {
                this.QUIEN = QUIEN;
                this.AREA = AREA;
                this.PUERTA = PUERTA;
                this.HORARIO = HORARIO;
            }
        }

        public class GEOCERCA
        {

            public int ID_RUTA { get; set; }
            public int ID_PUNTO { get; set; }
            public double LAT { get; set; }
            public double LON { get; set; }

            public GEOCERCA()
            {
                this.ID_RUTA = 0;
                this.ID_PUNTO = 0;
                this.LAT = 0;
                this.LON = 0;
            }
            public GEOCERCA(int ID_RUTA, int ID_PUNTO, double LAT, double LON)
            {
                this.ID_RUTA = ID_RUTA;
                this.ID_PUNTO = ID_PUNTO;
                this.LAT = LAT;
                this.LON = LON;
            }
        }

        public class OBSERVACION
        {

            public string HORA { get; set; }
            public string OBSERVACIONES { get; set; }
            public string FASE { get; set; }


            public OBSERVACION()
            {
                this.HORA = "";
                this.OBSERVACIONES = "";
                this.FASE = "";

            }
            public OBSERVACION(string HORA, string OBSERVACIONES, string FASE)
            {
                this.HORA = HORA;
                this.OBSERVACIONES = OBSERVACIONES;
                this.FASE = FASE;

            }
        }

        public class SYNC
        {
            public int ID { get; set; }
            public string Desc { get; set; }

            public SYNC()
            {
                this.ID = 0;
                this.Desc = "";
            }
            public SYNC(int ID, string Desc)
            {
                this.ID = ID;
                this.Desc = Desc;
            }
        }
        public class SYNC2
        {
            public int ID { get; set; }
            public string Desc { get; set; }
            public decimal? plit { get; set; }
            public decimal? pkil { get; set; }
            public SYNC2()
            {
                this.ID = 0;
                this.Desc = "";
                this.plit = 0;
                this.pkil = 0;
            }
            public SYNC2(int ID, string Desc, decimal? plit, decimal? pkil)
            {
                this.ID = ID;
                this.Desc = Desc;
                this.plit = plit;
                this.pkil = pkil;
            }
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


        public class ENTREGADOS
        {
            public int IDPEDIDO { get; set; }
            public string FECHAHORA { get; set; }
            public string CLIENTE { get; set; }
            public string DIRECCION { get; set; }
            public string DETALLES { get; set; }
            public string LITROS { get; set; }

            public ENTREGADOS()
            {
                this.IDPEDIDO = 0;
                this.FECHAHORA = "";
                this.CLIENTE = "";
                this.DIRECCION = "";
                this.DETALLES = "";
                this.LITROS = "";
            }
            public ENTREGADOS(int IDPEDIDO, string FECHAHORA, string CLIENTE, string DIRECCION, string DETALLES, string LITROS)
            {
                this.IDPEDIDO = IDPEDIDO;
                this.FECHAHORA = FECHAHORA;
                this.CLIENTE = CLIENTE;
                this.DIRECCION = DIRECCION;
                this.DETALLES = DETALLES;
                this.LITROS = LITROS;
            }
        }


        class objmensajes
        {
            public string from { get; set; }
            public string to { get; set; }
            public string text { get; set; }
        }
        class objTracking
        {
            public string track { get; set; }
            public string type { get; set; }
        }
        class objInfoBips
        {
            public objmensajes[] messages { get; set; }
            public objTracking tracking { get; set; }
        }
        #endregion

        #region INSERSION DE COORDENADAS DE ARDUINO
        [WebMethod]
        public string ArduinoCoodenadas(string precio_gas, string total, string litros, string servicio, string fecha, string latitud, string longitud, string id_truck)
        {
            string response = "1";
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();

                DateTime fe = Convert.ToDateTime(fecha.ToString());
                string dia = (int)fe.Day < 10 ? "0" + fe.Day.ToString() : fe.Day.ToString();
                string mes = (int)fe.Month < 10 ? "0" + fe.Month.ToString() : fe.Month.ToString();
                string hora = (int)fe.Hour < 10 ? "0" + fe.Hour.ToString() : fe.Hour.ToString();
                string min = (int)fe.Minute < 10 ? "0" + fe.Minute.ToString() : fe.Minute.ToString();
                string sec = (int)fe.Second < 10 ? "0" + fe.Second.ToString() : fe.Second.ToString();

                DateTime Fecha = DateTime.Parse(fe.Year + "-" + mes + "-" + dia + "T" + hora + ":" + min + ":" + sec);
                var asignaciones = context.Asignaciones.Where(x => x.id_truck == Convert.ToInt32(id_truck) && x.asignacion_activa.Equals(true)).FirstOrDefault();
                if (asignaciones != null)
                {
                    arduino obj_arduino = new arduino();
                    obj_arduino.precio_gas = Convert.ToDouble(precio_gas);
                    obj_arduino.total = Convert.ToDouble(total);
                    obj_arduino.litros = Convert.ToDouble(litros);
                    obj_arduino.servicio = Convert.ToInt32(servicio);
                    obj_arduino.fecha = Fecha;
                    obj_arduino.latitud = latitud;
                    obj_arduino.longitud = longitud;
                    obj_arduino.id_truck = asignaciones.id_truck;
                    obj_arduino.id_operador = asignaciones.id_operador;
                    obj_arduino.id_ayudante = asignaciones.id_ayudante;
                    obj_arduino.status = 0;
                    context.arduino.InsertOnSubmit(obj_arduino);
                    context.SubmitChanges();
                }
                else
                {
                    response = "VERIFIQUE QUE LA UNIDAD SE ENCUENTRE EN CIRCULACIÓN";
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                int line = frame.GetFileLineNumber();
                response = ex.Message.ToUpper() + " - EN LINEA - " + line.ToString();
            }
            return response;
        }

        private static string enviarSMS(string numero_telefonico, string mensaje)
        {
            //const string API_KEY = "edbe7af3462d0a80d00f9aa414c6834e-3588fa14-3994-4937-913d-762a204e3573";
            const string API_KEY = "8322360f7006795581031be0b7745540-7b905b58-084f-4803-9332-4b6e5ec7581c";

            objInfoBip objetoMensaje = new objInfoBip();
            objetoMensaje.from = "COMBUGAS";
            objetoMensaje.to = "+52" + numero_telefonico;
            objetoMensaje.text = mensaje;

            var jsonSerializer = new JavaScriptSerializer();
            var jsonINFO = jsonSerializer.Serialize(objetoMensaje);
            Debug.WriteLine(jsonINFO);

            var client = new RestClient("https://lkdew.api.infobip.com/sms/2/text/single");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "App " + API_KEY);
            request.AddParameter("application/json", jsonINFO, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.StatusCode.ToString();
        }

        static void enviarSMSUrl(string numero_telefonico, string mensaje)
        {
            const string API_KEY = "502cc4a452abc23a7576ad4ea0439556-d7901f01-fdc5-4b8e-a9b0-4fcb9e07bd44";

            objmensajes objetoMensaje = new objmensajes();
            objetoMensaje.from = "COMBUGAS";
            objetoMensaje.to = "+52" + numero_telefonico;
            objetoMensaje.text = mensaje;

            objmensajes[] msjs = new objmensajes[1];
            msjs[0] = objetoMensaje;

            objTracking track = new objTracking();
            track.track = "URL";
            track.type = "SOCIAL_INVITES";

            objInfoBips infob = new objInfoBips();
            infob.messages = msjs;
            infob.tracking = track;

            var jsonSerializer = new JavaScriptSerializer();
            var jsonINFO = jsonSerializer.Serialize(infob);
            Debug.WriteLine(jsonINFO);

            var client = new RestClient("https://lkdew.api.infobip.com/sms/1/text/advanced");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "App " + API_KEY);
            request.AddParameter("application/json", jsonINFO, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        #endregion

        #region OBTENER PRECIO DE LOS CILINDROS
        [WebMethod]
        public ajaxResponse preciosCilindros()
        {
            ajaxResponse Response = new ajaxResponse();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                context.CommandTimeout = 0;
                var productos = (from p in context.producto
                                 where (p.id_producto == 2 || p.id_producto == 3)
                                 select new
                                 {
                                     p.id_producto,
                                     p.precio
                                 }).ToList();

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(productos);
                Response.Result = true;
                Response.Data = json;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Data = ex.Message;
            }
            return Response;
        }
        #endregion

        #region REGISTRO DE OBSERVACIONES 
        [WebMethod]
        public string InsertaObservacion(int ID, string OBSERVACION)
        {
            ajaxResponse Response = new ajaxResponse();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                string texto_repartidor = string.Format("APP REPARTIDOR  -- {0}-- ", OBSERVACION);
                var ventas = context.ventas_repartidor.Where(x => x.id_pedido == ID).FirstOrDefault();
                string nombre_operador = "";
                if (ventas != null)
                {
                    var operador = context.operador.Where(x => x.id_operador == ventas.id_operador).FirstOrDefault();
                    nombre_operador = string.Format("{0}-{1} {2} {3}", operador.no_empleado, operador.nombre, operador.apellidoP, operador.apellidoM);
                }
                if (nombre_operador != "")
                {
                    texto_repartidor += string.Format("| -- REGISTRADO POR {0} (FECHA: {1}) --", nombre_operador, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                }
                string respuesta = REGISTRO_OBSERVACION(ID, texto_repartidor);
                Response.Result = respuesta == "OK" ? true : false;
                Response.Message = respuesta;
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = ex.Message.ToUpper();
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(Response);
            return json;
        }
        #endregion

        #region Login
        [WebMethod]
        public OperadorLogin[] LoginOp(string usuario, string contrasenia, string imeiAndroid = null)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;

            List<OperadorLogin> lista = new List<OperadorLogin>();
            /*
            var imeis = context.imei.Where(x => x.status == 1).ToList();
            if (string.IsNullOrWhiteSpace(imeiAndroid) ||
                imeiAndroid.Equals("unknown", StringComparison.OrdinalIgnoreCase) ||
                !imeis.Any(item => item.IMEI == imeiAndroid))
            {
                lista.Add(new OperadorLogin(-3, "", "", "", "", "" + " " + "" + " " + "", 0, 0, 0));
                return lista.ToArray();
            }
            */

            var objeto = from op in context.operador
                         where op.username == usuario & op.pass == contrasenia & op.tipo_operador.accesoapp == true & op.status == true
                         select op;

            if (objeto != null && objeto.Count() > 0)
            {
                Turnos turno = new Turnos();
                turno = context.Turnos.Where(x => DateTime.Now.TimeOfDay >= x.horainicio & DateTime.Now.TimeOfDay <= x.horafin).SingleOrDefault();
                int turnos = turno.id_turno;

                foreach (var ope in objeto)
                {
                    int? tipo = ope.id_tipo_operador;
                    var asignacion = from asig in context.Asignaciones
                                     where asig.id_operador == ope.id_operador & asig.asignacion_activa == true &
                                    asig.id_turno == turnos & asig.id_ruta != 1
                                     orderby asig.fecha descending
                                     select asig;
                    if (asignacion == null)
                    {
                        lista.Add(new OperadorLogin(-1, "", "", "", "", "" + " " + "" + " " + "", 0, 0, tipo));
                    }
                    else
                    {
                        int? idayuda = 0;
                        int? ruta = 0;
                        int? Unid = 0;
                        string rutaUlti = "";
                        int? id_truck = 0;
                        string no_economico = "";

                        foreach (var a in asignacion)
                        {
                            idayuda = a.id_ayudante;
                            rutaUlti += "[" + a.Rutas.clave_ruta + " - " + a.Rutas.nombre + "]";
                            ruta = a.id_ruta;
                            Unid = a.truck.id_tipounidad;
                            id_truck = a.id_truck;
                            no_economico = (from i in context.truck where i.id_truck == a.id_truck select i.numero).FirstOrDefault();
                        }
                        if (idayuda == 0)
                        {
                            if (ruta > 0)
                            {
                                lista.Add(new OperadorLogin(ope.id_operador, ope.nombre + "", ope.apellidoP + "", ope.apellidoM + "", rutaUlti,
                        ope.nombre + " " + ope.apellidoP + " " + ope.apellidoM, ruta, Unid, tipo, id_truck, no_economico));

                                login_op login = new login_op();
                                login.id_operador = ope.id_operador;
                                login.fecha_login = DateTime.Now;
                                context.login_op.InsertOnSubmit(login);
                                context.SubmitChanges();
                            }
                            else
                            {
                                lista.Add(new OperadorLogin(-1, "", "", "", "", "" + " " + "" + " " + "", 0, 0, tipo, id_truck, no_economico));
                            }
                        }
                        else
                        {
                            operador ayudante = context.operador.Where(x => x.id_operador == idayuda).SingleOrDefault();
                            lista.Add(new OperadorLogin(ope.id_operador, ope.nombre + "", ope.apellidoP + "", ope.apellidoM + "", rutaUlti,
                        ayudante.nombre + " " + ayudante.apellidoP + " " + ayudante.apellidoM, ruta, Unid, tipo, id_truck, no_economico));

                            login_op login = new login_op();
                            login.id_operador = ope.id_operador;
                            login.fecha_login = DateTime.Now;
                            context.login_op.InsertOnSubmit(login);
                            context.SubmitChanges();
                        }
                    }
                }
            }
            else
            {
                lista.Add(new OperadorLogin(-2, "", "", "", "", "" + " " + "" + " " + "", 0, 0, 0));
            }
            return lista.ToArray();
        }

        [WebMethod]
        public GEOCERCA[] GEOCERCAS(int id_operador)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            Turnos turno = new Turnos();
            turno = context.Turnos.Where(x => DateTime.Now.TimeOfDay >= x.horainicio & DateTime.Now.TimeOfDay <= x.horafin).SingleOrDefault();
            int turnos = turno.id_turno;
            List<GEOCERCA> lista = new List<GEOCERCA>();
            var Asignaciones = from asig in context.Asignaciones
                               where asig.id_operador == id_operador & asig.asignacion_activa == true & asig.id_turno == turnos
                               select asig;
            foreach (var a in Asignaciones)
            {
                try
                {
                    var tq = context.GeoRuta.Where(x => x.id_ruta == a.id_ruta & x.Rutas.estatus == true).ToList();
                    if (tq != null)
                    {
                        foreach (var asa in tq)
                        {

                            lista.Add(new GEOCERCA(asa.id_ruta, asa.id_georuta, asa.latitud, asa.longitud));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return lista.ToArray();
        }
        #endregion

        #region Sincronizacion Inicial

        [WebMethod]
        public SYNC2[] PRODUCTOS()
        {
            List<SYNC2> lista = new List<SYNC2>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var Req = from pro in context.producto where pro.status == true && pro.es_det == 0 select pro;
                foreach (var a in Req)
                {
                    lista.Add(new SYNC2(a.id_producto, a.descripcion, a.precio, a.precio_kilo));
                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }

        [WebMethod]
        public SYNC[] MOTIVOS()
        {
            List<SYNC> lista = new List<SYNC>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var Req = from pro in context.Incidencias where pro.status == true select pro;
                lista.Add(new SYNC(0, "--SELECCIONE MOTIVO--"));
                foreach (var a in Req)
                {
                    lista.Add(new SYNC(a.id_incidencia, a.descripcion));
                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }

        [WebMethod]
        public ajaxResponse insert_gasparoff(int id_truck, int id_operador)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                gaspar_off off = new gaspar_off();
                off.id_truck = id_truck;
                off.id_operador = id_operador;
                off.fecha = DateTime.Now;
                context.gaspar_off.InsertOnSubmit(off);
                context.SubmitChanges();
                response.Result = true;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse LLEGUE(int ID)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                /*CONSULTA PARA SACAR EL TELÉFONO DEL CLIENTE PARA ENVIAR EL WHATSAPP*/
                Pedido pedido = context.Pedido.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                Telefono telefono = (from t in context.Telefono where t.id_telefono == pedido.Id_Telefono select t).FirstOrDefault();
                string telefono_cliente = new String(telefono.no_telefono.Where(Char.IsDigit).ToArray());

                /*CONSULTA PARA SACAR LOS DATOS DEL VENDEDOR QUE VA A SURTIR*/
                int? id_operador = (from v in context.ventas_repartidor where v.id_pedido == ID select v.id_operador).FirstOrDefault();
                operador op = context.operador.Where(x => x.id_operador == id_operador).FirstOrDefault();
                string nombre_repartidor = string.Format("{0} {1}", op.nombre, op.apellidoP);

                /*CUERPO DEL MENSAJE A ENVIAR*/
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var accountSid = ConfigurationManager.AppSettings["TWILIO_ACCOUNT_SID"];
                var authToken = ConfigurationManager.AppSettings["TWILIO_AUTH_TOKEN"];
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: string.Format("El repartidor {0} de COMBUGAS🔥 ya se encuentra en su domicilio.", nombre_repartidor),
                    from: new PhoneNumber("whatsapp:+14155238886"),
                    to: new PhoneNumber("whatsapp:+5218713459923")
                    //to: new PhoneNumber("whatsapp:+5218715990744")
                );

                Console.WriteLine(message.Sid);

                response.Result = true;
                response.Message = "Mensaje enviado";
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public ajaxResponse REGISTROSGASPAR(string no_servicio, float total, float litros, int id_operador, int id_truck, int id_ruta, int? ID = null)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                string unidad = (from t in context.truck where t.id_truck == id_truck select t.numero).FirstOrDefault();
                operador op = context.operador.Where(x => x.id_operador == id_operador).FirstOrDefault();
                string vendedor = string.Format("{0} - {1} {2} {3}", op.no_empleado, op.nombre, op.apellidoP, op.apellidoM);

                DateTime fecha_servicio = DateTime.Now;
                string mensaje = "";
                mensaje = string.Format("*SERVICIO SURTIDO*\n" +
                     "Vendedor: {5}\n" +
                     "Unidad: {6}\n" +
                     "No. servicio: {0}\n" +
                     "Fecha de surtido: {1}\n" +
                     "Litros: {2}\n" +
                     "Total: ${3}\n" +
                     "Pedido: {4}\n", no_servicio,
                     fecha_servicio.ToString("dd/MM/yyyy hh:mm:ss tt"),
                     litros, total, ID == null ? "LIBRE" : ID.ToString(), vendedor.ToUpper(), unidad.ToUpper()
                     );

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var accountSid = ConfigurationManager.AppSettings["TWILIO_ACCOUNT_SID"];
                var authToken = ConfigurationManager.AppSettings["TWILIO_AUTH_TOKEN"];
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: mensaje,
                    from: new PhoneNumber("whatsapp:+14155238886"),
                    to: new PhoneNumber("whatsapp:+5218713459923")
                    //to: new PhoneNumber("whatsapp:+5218715990744")
                );

                #region REGISTRO DE RESPUESTA PARA REPORTE
                registros_gaspar gaspar = new registros_gaspar();
                gaspar.fecha_surtido = fecha_servicio;
                gaspar.Id_Pedido = ID;
                gaspar.no_servicio = no_servicio;
                gaspar.total = total;
                gaspar.litros = litros;
                gaspar.id_operador = id_operador;
                gaspar.id_truck = id_truck;
                gaspar.id_ruta = id_ruta;
                gaspar.cuerpo_mensaje = mensaje;
                context.registros_gaspar.InsertOnSubmit(gaspar);
                context.SubmitChanges();
                #endregion

                response.Result = true;
                response.Message = "Mensaje enviado";
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod]
        public SYNC[] BANCOS()
        {
            List<SYNC> lista = new List<SYNC>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var Req = from pro in context.bancos where pro.status == true select pro;
                lista.Add(new SYNC(0, "--SELECCIONE BANCO--"));
                foreach (var a in Req)
                {
                    lista.Add(new SYNC(a.id_banco, a.nombre_banco));
                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }
        #endregion

        #region Monitor Tablet o Mapa
        [WebMethod]
        public PedidoTabla[] PedidoTablaLlena(int ruta, double lat, double longi, int id_operador)
        {
            ajaxResponse Response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            List<PedidoTabla> lista = new List<PedidoTabla>();
            List<PedidoTabla> lista2 = new List<PedidoTabla>();

            var consultarsp = context.sp_Operadores_App(1, id_operador, 0).ToList();
            var pedidosSQL = (
                    from i in (
                        consultarsp
                    )
                    group i by new
                    {
                        i.casanova,
                        i.clave,
                        i.credito,
                        i.DescCliente,
                        i.direccionT,
                        i.fase,
                        i.Id_Direccion,
                        i.fecha_creacion,
                        i.horaPedido,
                        i.horaRadio,
                        i.Id_Pedido,
                        i.latitud,
                        i.longitud,
                        i.nombre,
                        i.origen,
                        i.Pedido_confirmado_operador,
                        i.pedido_programado,
                        i.referencias,
                        i.req_clave,
                        i.si_observaciones,
                        i.si_req,
                        i.status_pedido,
                        i.fecha_a_surtir,
                        i.hora_a_surtir,
                        i.en_radios,
                        i.fecha_en_radios,
                        i.hora_en_radios,
                        i.Capturista,
                        i.Metodo,
                        i.tiene_incidencia,
                        i.regresar_pedido,
                        i.ValeTodo
                    } into g
                    select new
                    {
                        g.Key.casanova,
                        g.Key.clave,
                        g.Key.credito,
                        g.Key.Id_Direccion,
                        g.Key.DescCliente,
                        g.Key.direccionT,
                        g.Key.fase,
                        g.Key.fecha_creacion,
                        g.Key.horaPedido,
                        g.Key.horaRadio,
                        g.Key.Id_Pedido,
                        g.Key.latitud,
                        g.Key.longitud,
                        g.Key.nombre,
                        g.Key.origen,
                        g.Key.Pedido_confirmado_operador,
                        g.Key.pedido_programado,
                        g.Key.referencias,
                        g.Key.req_clave,
                        g.Key.si_observaciones,
                        g.Key.si_req,
                        g.Key.status_pedido,
                        g.Key.fecha_a_surtir,
                        g.Key.hora_a_surtir,
                        g.Key.en_radios,
                        g.Key.fecha_en_radios,
                        g.Key.hora_en_radios,
                        g.Key.Capturista,
                        g.Key.Metodo,
                        g.Key.tiene_incidencia,
                        g.Key.regresar_pedido,
                        g.Key.ValeTodo
                    }
            );

            foreach (var p in pedidosSQL)
            {
                string manguera = " ";
                string restric = " ";
                bool? bascula = false;
                bool credito = false;
                bool si_observaciones = false;
                bool si_req = false;
                bool agregarP = false;
                DateTime fechaActual = DateTime.Now.Date;
                TimeSpan horaActual = DateTime.Now.TimeOfDay;
                DateTime fechaProgramada;
                TimeSpan horaProgramada;
                bool esProgramado = false;
                int casas = 0;

                #region If Credito, Observaciones, Requisitos
                if (p.credito != null)
                {
                    credito = (bool)p.credito;
                }
                if (p.si_observaciones > 0)
                {
                    si_observaciones = true;
                }
                if (p.si_req > 0)
                {
                    si_req = true;
                }
                #endregion

                #region Foreach Especificaciones y Restricciones
                var ESPTEC = from pa in context.Esp_Tec_Direccion where pa.id_direccion == p.Id_Direccion select pa;
                foreach (var sa in ESPTEC)
                {
                    manguera = sa.tam_manguera;
                    bascula = sa.bascula;
                }
                var REST = from rt in context.observaciones_pred
                           join rtt in context.Res_Pago_Direccion on rt.id_ob equals rtt.id_restriccion
                           where rtt.id_direccion == p.Id_Direccion
                           select rt;
                foreach (var asa in REST)
                {
                    restric = asa.descripcion;
                }
                #endregion

                #region PREGUNTA PROGRAMADO
                if (p.pedido_programado == null)
                {
                    esProgramado = false;
                    agregarP = true;
                }
                else
                {
                    esProgramado = (bool)p.pedido_programado;

                }
                #endregion

                #region ES PROGRAMADO
                if (esProgramado)
                {
                    if (fechaActual >= p.fecha_a_surtir)
                    {
                        agregarP = true;
                    }
                    else
                    {
                        agregarP = false;
                    }
                }
                else
                {
                    agregarP = true;
                }
                #endregion

                #region AGREGAR
                if (agregarP)
                {
                    string dir = p.direccionT;
                    dir = dir.Replace("ENTRE: -  - ,", "");
                    dir = dir.Replace("NO APLICA,", "");
                    dir = dir.Replace("INTERIOR: ,", "");
                    dir = dir.Replace("\n\r", "");
                    dir = dir.Replace("\r\n", "");
                    dir = dir.Replace("\r", "");
                    dir = dir.Replace("\n", "");
                    string radihora = "Por Asignar";
                    if (p.horaRadio == null)
                    {
                        radihora = "Por Asignar";
                    }
                    else
                    {
                        radihora = p.horaRadio.ToString().Substring(0, 8);
                    }
                    int valueRegresa = 0;
                    if (p.regresar_pedido == null)
                    {
                        valueRegresa = 0;
                    }
                    else
                    {
                        valueRegresa = (int)p.regresar_pedido;
                    }
                    if (p.casanova == true)
                    {
                        casas = 1;
                        lista.Add(new PedidoTabla(p.Id_Pedido, p.nombre, dir, p.DescCliente, p.horaPedido.ToString().Substring(0, 8), radihora, p.Capturista,
                           p.latitud, p.longitud,
                            manguera + " ", restric + " ", p.referencias + " ", p.Metodo,
                           p.clave + " ", bascula, p.req_clave, credito, p.fecha_creacion.ToString().Substring(0, 10) +
                           " - " + p.horaPedido.ToString().Substring(0, 8), p.fase, p.status_pedido.ToString(), si_req, si_observaciones, GetDistance(longi, lat, p.longitud, p.latitud), casas, p.tiene_incidencia, p.Pedido_confirmado_operador, valueRegresa,
                           p.ValeTodo == null ? "" : p.ValeTodo
                           ));
                    }
                    else
                    {
                        lista2.Add(new PedidoTabla(p.Id_Pedido, p.nombre, dir, p.DescCliente, p.horaPedido.ToString().Substring(0, 8), radihora, p.Capturista,
                           p.latitud, p.longitud,
                            manguera + " ", restric + " ", p.referencias + " ", p.Metodo,
                           p.clave + " ", bascula, p.req_clave, credito, p.fecha_creacion.ToString().Substring(0, 10) +
                           " - " + p.horaPedido.ToString().Substring(0, 8), p.fase, p.status_pedido.ToString(), si_req, si_observaciones, GetDistance(longi, lat, p.longitud, p.latitud), casas, p.tiene_incidencia, p.Pedido_confirmado_operador, valueRegresa,
                            p.ValeTodo == null ? "" : p.ValeTodo
                           ));
                    }
                }
                #endregion
            }

            lista2.Sort((p, q) => p.inmetros.CompareTo(q.inmetros));
            List<PedidoTabla> listasa = new List<PedidoTabla>();
            listasa.AddRange(lista);
            listasa.AddRange(lista2);

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(listasa);
            return listasa.ToArray();
        }

        private static double GetDistance(double long1InDegrees, double lat1InDegrees, double? long2InDegrees, double? lat2InDegrees)
        {
            double lats = (double)(lat1InDegrees - lat2InDegrees);
            double lngs = (double)(long1InDegrees - long2InDegrees);

            double latm = lats * 60 * 1852;
            double lngm = (lngs * Math.Cos((double)lat1InDegrees * Math.PI / 180)) * 60 * 1852;
            double distInMeters = Math.Sqrt(Math.Pow(latm, 2) + Math.Pow(lngm, 2));
            return distInMeters;
        }
        #endregion

        #region Entregados
        [WebMethod]
        public ENTREGADOS[] ENTREGAS(int id_operador)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            List<ENTREGADOS> lista = new List<ENTREGADOS>();
            var consultaProcedimiento = context.sp_Operadores_App_Entregados(1, id_operador).ToList();
            var pedidosAgrupados = (
                                    from orden in
                                    (
                                        consultaProcedimiento
                                    )
                                    group orden by new
                                    {
                                        orden.Id_Pedido,
                                        orden.Nombre,
                                        orden.Hora,
                                        orden.DireccionT,
                                        orden.Detalles,
                                        orden.LitrosSurtidos
                                    }
                                    into g
                                    select new
                                    {
                                        g.Key.Id_Pedido,
                                        g.Key.Nombre,
                                        g.Key.Hora,
                                        g.Key.DireccionT,
                                        g.Key.Detalles,
                                        g.Key.LitrosSurtidos
                                    }
                                );

            foreach (var item in pedidosAgrupados)
            {
                string dir = item.DireccionT;
                dir = dir.Replace("ENTRE: -  - ,", "");
                dir = dir.Replace("NO APLICA,", "");
                dir = dir.Replace("INTERIOR: ,", "");
                dir = dir.Replace("INTERIOR: 0,", "");
                dir = dir.Replace("\n\r", "");
                dir = dir.Replace("\r\n", "");
                dir = dir.Replace("\r", "");
                dir = dir.Replace("\n", "");
                string litros = "0";
                if (item.LitrosSurtidos == null | item.LitrosSurtidos == "null")
                {
                    litros = "0";
                }
                else
                {
                    litros = "" + item.LitrosSurtidos;
                }
                lista.Add(new ENTREGADOS(item.Id_Pedido, item.Hora, item.Nombre, dir, item.Detalles, litros));
            }

            return lista.ToArray();
        }
        #endregion

        #region ASIGNADO?
        [WebMethod]
        public int[] ASIGNADO(int ID_PEDIDO, int ID_OPERADOR)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            List<int> respuesta = new List<int>();
            try
            {
                Asigna_Pedido ap = new Asigna_Pedido();
                ap = context.Asigna_Pedido.Where(x => x.id_pedido == ID_PEDIDO && x.id_operador == ID_OPERADOR).SingleOrDefault();
                if (ap != null)
                {
                    if (ap.id_operador != ID_OPERADOR)
                    {
                        respuesta.Add(2);//DESASIGNADO
                    }
                    else
                    {
                        respuesta.Add(1);//ASIGNADO
                    }
                }
                else
                {
                    respuesta.Add(0);//DESASIGNADO
                }
            }
            catch (Exception ex)
            {
                respuesta.Add(0);
            }
            return respuesta.ToArray();
        }
        #endregion

        #region Detalle Del Pedido
        [WebMethod]
        public PDETA[] PedidoDetalleTabla(int ID)
        {
            ajaxResponse Response = new ajaxResponse();
            List<PDETA> lista = new List<PDETA>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var agrupacion = from p in context.Pedido_Detalle where p.Id_Pedido == ID select p;


                foreach (var grupo in agrupacion)
                {
                    producto pro = new producto();
                    pro = context.producto.Where(x => x.id_producto == grupo.Id_producto).SingleOrDefault();
                    var aplica = 0;
                    if (grupo.Id_producto == 9)
                    {
                        aplica = 1;
                    }
                    else
                    {
                        aplica = 0;
                    }
                    var preciokl = pro.precio_kilo;
                    producto pros = new producto();
                    pros = context.producto.Where(x => x.id_producto == 9).SingleOrDefault();
                    var preciolt = pros.precio;
                    if (grupo.id_servicio == 3 || grupo.id_servicio == 9)
                    {
                        string importe = Math.Round(grupo.importe, 2, MidpointRounding.AwayFromZero).ToString();
                        lista.Add(new PDETA(grupo.Id_Pedido_Detalle, grupo.producto.descripcion, grupo.TipoUnidad.tipo_unidad, "NO APLICA", grupo.cantidad + "", importe, "NO APLICA", "NO APLICA", "NO APLICA", "NO APLICA", grupo.Id_producto, 0, 0, 0));
                    }
                    else
                    {
                        lista.Add(new PDETA(
                            grupo.Id_Pedido_Detalle,
                            grupo.producto.descripcion,
                            grupo.TipoUnidad.tipo_unidad,
                            grupo.unidad_de_liq,
                            Math.Round(grupo.cantidad, 2, MidpointRounding.AwayFromZero) + "",
                            Math.Round(grupo.importe, 2, MidpointRounding.AwayFromZero) + "",
                            Math.Round((double)grupo.Kg_Lts_a_surtir, 2, MidpointRounding.AwayFromZero) + "",
                            Math.Round((double)grupo.litros_a_surtir, 2, MidpointRounding.AwayFromZero) + "",
                            grupo.porc_inicio + "",
                            grupo.porc_final + "",
                            grupo.Id_producto,
                            preciokl,
                            preciolt,
                            aplica));
                    }
                }
            }
            catch (Exception ex)
            {
                lista.Add(new PDETA(0,
                              ex.Message.ToString(),
                              "",
                              "",
                              "",
                               "",
                               "",
                              "",
                              "",
                               "",
                             0,
                              0,
                              0,
                              0));
            }
            return lista.ToArray();
        }

        [WebMethod]
        public REQUI[] REQUISITOS(int IDPEDIDO)
        {

            List<REQUI> lista = new List<REQUI>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                Pedido pedido = new Pedido();
                pedido = context.Pedido.Where(x => x.Id_Pedido == IDPEDIDO).SingleOrDefault();
                int IDDIRECCION = pedido.Id_Direccion;

                var Req = context.Req_Pago_Direccion.Where(x => x.id_direccion == IDDIRECCION).ToList();
                if (Req != null)
                {
                    foreach (var a in Req)
                    {

                        lista.Add(new REQUI(a.observaciones_pred.descripcion));
                    }
                }
                else
                {
                    lista.Add(new REQUI("NO TIENE REQUISITOS DE PAGO"));
                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }

        [WebMethod]
        public TANQUE[] TANQUES(int IDPEDIDO)
        {

            List<TANQUE> lista = new List<TANQUE>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                Pedido pedido = new Pedido();
                pedido = context.Pedido.Where(x => x.Id_Pedido == IDPEDIDO).SingleOrDefault();
                int IDDIRECCION = pedido.Id_Direccion;

                var tq = context.Esp_Tec_Direccion.Where(x => x.id_direccion == IDDIRECCION & x.status == true).ToList();
                if (tq != null)
                {
                    foreach (var a in tq)
                    {

                        lista.Add(new TANQUE(a.id_esp, a.num_tanques, a.cap_tanque));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }

        [WebMethod]
        public INSTALACIONES[] INSTALACION(int IDPEDIDO)
        {

            List<INSTALACIONES> lista = new List<INSTALACIONES>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                Pedido pedido = new Pedido();
                pedido = context.Pedido.Where(x => x.Id_Pedido == IDPEDIDO).SingleOrDefault();
                try
                {
                    lista.Add(new INSTALACIONES(pedido.Quen_Recibe + " ", pedido.Area + " ", pedido.Puerta_recibe + " ",
                    pedido.horario_entrega_inicial + " - " + pedido.horario_entrega_final));
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }

        public static string REGISTRO_OBSERVACION(int ID, string OBSERVACION)
        {
            string respuesta = "";
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                observaciones_pedido o = new observaciones_pedido();

                o.Id_Pedido = ID;
                o.Id_Usuario = 10;
                o.descripcion = OBSERVACION;
                o.fecha = DateTime.Now.Date;
                o.hora = DateTime.Now.TimeOfDay;
                o.fase = "RADIOS";
                context.observaciones_pedido.InsertOnSubmit(o);
                context.SubmitChanges();

                string observacion = "[" + OBSERVACION + "]";
                Pedido ped = context.Pedido.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                if (ped.observa_pedido != "" && ped.observa_pedido != null)
                {
                    observacion = "<br>[" + OBSERVACION + "]";
                }
                ped.observa_pedido = ped.observa_pedido + observacion;
                context.SubmitChanges();

                movimientos_pedidos mov = new movimientos_pedidos();
                mov.id_usuario = 10;
                mov.id_pedido = ID;
                mov.hora = DateTime.Now.TimeOfDay;
                mov.fecha = DateTime.Now.Date;
                mov.funcion = "OBSERVACION PEDIDO";
                mov.sistema = "PEDIDO: " + ID + ", HORA OBSERVACION: " + mov.hora + ", FASE: RADIOS";
                mov.datos = "" + ID + "," + mov.hora;
                context.movimientos_pedidos.InsertOnSubmit(mov);
                context.SubmitChanges();
                respuesta = "OK";
            }
            catch (Exception ex)
            {
                respuesta = ex.Message.ToUpper();
            }
            return respuesta;
        }

        [WebMethod]
        public OBSERVACION[] OBSERVAPEDIDO(int IDPEDIDO)
        {
            List<OBSERVACION> lista = new List<OBSERVACION>();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                try
                {
                    var Obs = context.observaciones_pedido.Where(x => x.Id_Pedido == IDPEDIDO).ToList();
                    if (Obs != null)
                    {
                        foreach (var a in Obs)
                        {
                            lista.Add(new OBSERVACION(a.hora.ToString().Substring(0, 8), a.descripcion, a.fase));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
            return lista.ToArray();
        }
        #endregion

        #region Actualizacion Domicilio
        [WebMethod]
        public int[] ACTUALIZAUBICACION(int IDPEDIDO, double LATITUD, double LONGITUD)
        {
            List<int> respuesta = new List<int>();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                Pedido pedido = new Pedido();
                pedido = context.Pedido.Where(x => x.Id_Pedido == IDPEDIDO).SingleOrDefault();
                int IDDIRECCION = pedido.Id_Direccion;
                int IDOPERADOR = (int)(from pd in context.Pedido_Detalle where pd.Id_Pedido == IDPEDIDO select pd.id_operador).FirstOrDefault();

                Direccion d = new Direccion();
                d = context.Direccion.Where(x => x.id_direccion == IDDIRECCION).SingleOrDefault();
                double casaLat = 0, casaLon = 0;
                string direccion = "";

                if (d.lat_B == null || d.lat_B < 0)
                {
                    direccion = d.calles.tipo_calle.descripcion + " " + d.calles.descripcion + " " + d.no_exterior + ", " + d.colonias.tipo_asentamiento.descripcion +
                        " " + d.colonias.descripcion + ", " + d.ciudades.descripcion + ", " + d.estados.descripcion + ", Mexico.";
                    direccion = direccion.Replace(" NO APLICA,", "");
                    direccion = direccion.Replace(", ENTRE:", "");
                    direccion = direccion.Replace(" INTERIOR: ,", "");
                    direccion = direccion.Replace(" -", "");
                    direccion = direccion.Replace("\r", "");
                    direccion = direccion.Replace("\n", "");
                    direccion = direccion.Replace("  ", "");
                    string MAPKEY = ConfigurationManager.AppSettings["KEY_MAPS"];


                    IGeocoder geocoder = new GoogleGeocoder() { ApiKey = MAPKEY };
                    IEnumerable<Address> addresses = geocoder.Geocode(direccion);
                    casaLat = addresses.First().Coordinates.Latitude;
                    casaLon = addresses.First().Coordinates.Longitude;

                    d.lat_B = casaLat;
                    d.lon_B = casaLon;
                    context.SubmitChanges();
                }
                else
                {
                    casaLat = (double)d.lat_B;
                    casaLon = (double)d.lon_B;
                }

                double metros = 0.0;
                metros = GetDistance(LONGITUD, LATITUD, casaLon, casaLat);
                if (metros > 120)
                {
                    metros = GetDistance(LONGITUD, LATITUD, d.longitud, d.latitud);
                    casaLat = (double)d.latitud;
                    casaLon = (double)d.longitud;
                }
                if (metros < 120)
                {
                    LONGITUD = casaLon;
                    LATITUD = casaLat;
                    #region traer ruta basado en la direccion
                    int idRuta = calculaRuta(LATITUD, LONGITUD);
                    #endregion
                    if (LATITUD > 0 && LONGITUD < 0)
                    {
                        try
                        {
                            double? lts = d.latitud;
                            double? lon = d.longitud;
                            double? rut = d.id_ruta;
                            d.latitud = LATITUD;
                            d.longitud = LONGITUD;
                            if (d.lat_B < 0 || d.lat_B == null)
                            {
                                d.lat_B = LATITUD;
                                d.lon_B = LONGITUD;
                            }
                            d.id_ruta = idRuta;
                            d.direccion_actualizada_app = 1;
                            context.SubmitChanges();

                            #region Movimientos Pedido
                            movimientos_pedidos m = new movimientos_pedidos();
                            m.fecha = DateTime.Now.Date;
                            m.hora = DateTime.Now.TimeOfDay;
                            m.id_pedido = IDPEDIDO;
                            m.id_usuario = 10;
                            m.sistema = "SE ACTUALIZO DIRECCION DEL CLIENTE POR MEDIO DE APP OPERADOR";
                            m.funcion = "ACTUALIZA DIRECCION";
                            m.datos = "PEDIDO : " + IDPEDIDO + " | OPERADOR: " + IDOPERADOR + " | DIRECCION: " + IDDIRECCION + " | DATOS: LAT: " + LATITUD + " LON: " + LONGITUD + " RUTA: " + idRuta + " | ANTERIOR  DATOS: LAT: " + lts + " LON: " + lon + " RUTA: " + rut;
                            context.movimientos_pedidos.InsertOnSubmit(m);
                            context.SubmitChanges();
                            #endregion

                            pedido.regresar_pedido = 0;
                            pedido.se_regreso_pedido = 0;
                            pedido.es_manual = 0;
                            context.SubmitChanges();

                            respuesta.Add(1);
                        }
                        catch (Exception sec)
                        {
                            #region Movimientos Pedido
                            movimientos_pedidos m = new movimientos_pedidos();
                            m.fecha = DateTime.Now.Date;
                            m.hora = DateTime.Now.TimeOfDay;
                            m.id_pedido = IDPEDIDO;
                            m.id_usuario = 10;
                            m.sistema = "ERROR AL ACTUALIZAR";
                            m.funcion = "ACTUALIZA DIRECCION ERROR";
                            m.datos = "PEDIDO : " + IDPEDIDO + " | OPERADOR: " + IDOPERADOR + " | DIRECCION: " + IDDIRECCION + " | DATOS: LAT: " + LATITUD + " LON: " + LONGITUD + " RUTA: " + idRuta + " | ERROR: " + sec.Message;
                            context.movimientos_pedidos.InsertOnSubmit(m);
                            context.SubmitChanges();

                            #endregion
                            respuesta.Add(0);
                        }
                    }
                    else
                    {
                        respuesta.Add(0);
                    }
                }
                else
                {
                    respuesta.Add(1);
                    pedido.regresar_pedido = 0;
                    pedido.se_regreso_pedido = 0;
                    pedido.es_manual = 1;
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                #region Movimientos Pedido
                movimientos_pedidos m = new movimientos_pedidos();
                m.fecha = DateTime.Now.Date;
                m.hora = DateTime.Now.TimeOfDay;
                m.id_pedido = IDPEDIDO;
                m.id_usuario = 10;
                m.sistema = "ERROR AL ACTUALIZAR";
                m.funcion = "ACTUALIZA DIRECCION ERROR";
                m.datos = "PEDIDO : " + IDPEDIDO + " | ERROR: " + ex.Message;
                context.movimientos_pedidos.InsertOnSubmit(m);
                context.SubmitChanges();

                #endregion
                respuesta.Add(0);
            }
            return respuesta.ToArray();
        }


        int calculaRuta(double lat, double lon)
        {
            int idRutaAutotanque = -1;
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                #region consultas de rutas
                var geocercas = (
                    from SQLRutas in context.Rutas
                    where SQLRutas.id_tipo_ruta.Equals(1)
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
                foreach (var ruta in geocercas)
                {
                    List<Loc> puntos = new List<Loc>();
                    foreach (var geocerca in ruta._geocercasCarb)
                    {
                        puntos.Add(new Loc(geocerca._geoLat, geocerca._geoLong));
                    }
                    if (puntos.Count > 0)
                    {
                        bool estaengeocerca = IsPointInPolygon(puntos, new Loc((Double)lat, (Double)lon));
                        if (estaengeocerca)
                        {
                            idRutaAutotanque = ruta._idRuta;
                            break;
                        }
                    }
                }
                #endregion

                #region loop para medir la distancia entre los pines para deterimar ruta mas cercana
                if (idRutaAutotanque.Equals(-1))
                {
                    List<distanciasCoordenadas> distancias = new List<distanciasCoordenadas>();
                    foreach (var ruta in geocercas)
                    {
                        foreach (var geocerca in ruta._geocercasCarb)
                        {
                            var coordenadasdireccion = new GeoCoordinate((double)lat, (double)lon);
                            var coordenadaspin = new GeoCoordinate(geocerca._geoLat, geocerca._geoLong);
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
                return -1;
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
        #endregion

        #region Actualizar Tanque
        [WebMethod]
        public int[] ACTUALIZATANQUE(int IDTANQUE, int CAPACIDAD, string UNIDAD, bool ACTIVO)
        {
            List<int> actualizado = new List<int>();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            Esp_Tec_Direccion pedido = context.Esp_Tec_Direccion.Where(x => x.id_esp == IDTANQUE).SingleOrDefault();
            pedido.num_tanques = CAPACIDAD;
            pedido.cap_tanque = UNIDAD;
            pedido.status = ACTIVO;
            try
            {
                context.SubmitChanges();
                actualizado.Add(1);
            }
            catch (Exception)
            {
                actualizado.Add(0);
            }
            return actualizado.ToArray();
        }
        #endregion

        #region Metodos Sin Usar

        [WebMethod]
        public string CANCELARPEDIDO(int ID, int IDCANCELA)
        {
            string mensaje = "Cancelado";
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                Pedido o = new Pedido();
                o = context.Pedido.Where(x => x.Id_Pedido == ID).SingleOrDefault();
                if (o != null)
                {
                    o.fase_cancelacion = "RADIOS";
                    o.Id_Cancelacion = IDCANCELA;
                    o.status_pedido = false;
                    o.enasignacion = true;
                    o.asignado = true;
                    context.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                mensaje = "Sin Cancelar";
            }
            return mensaje;
        }
        #endregion

        #region NO SURTIR PEDIDO
        [WebMethod]
        public int[] NOSURTIRPEDIDO(int ID, int IDNOSURTIR)
        {
            List<int> respuesta = new List<int>();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                var incidencia = context.Incidencias.Where(x => x.id_incidencia.Equals(IDNOSURTIR)).FirstOrDefault();
                string texto_observacion = string.Format("SE MARCÓ EL PEDIDO COMO NO SURTIDO CON EL SIGUIENTE MOTIVO: {0}", incidencia.descripcion);
                InsertaObservacion(ID, texto_observacion);

                respuesta.Add(1);
            }
            catch (Exception ex)
            {
                #region Movimientos Pedido
                movimientos_pedidos m = new movimientos_pedidos();
                m.fecha = DateTime.Now.Date;
                m.hora = DateTime.Now.TimeOfDay;
                m.id_pedido = ID;
                m.id_usuario = 10;
                m.sistema = "ERROR AL ACTUALIZAR";
                m.funcion = "NO SURTIR PEDIDO";
                m.datos = "PEDIDO : " + ID + " | ERROR: " + ex.Message;
                context.movimientos_pedidos.InsertOnSubmit(m);
                context.SubmitChanges();
                #endregion
                respuesta.Add(0);
            }
            return respuesta.ToArray();
        }
        #endregion

        [WebMethod]
        public string entregaPedidoFL(int ID)
        {
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            Pedido var_pedido = context.Pedido.Where(x => x.Id_Pedido == ID).FirstOrDefault();
            string ValeTodo = var_pedido.ValeTodo;
            string no_operador = (from p in context.Pedido
                                  join d in context.Pedido_Detalle on p.Id_Pedido equals d.Id_Pedido
                                  join op in context.operador on d.id_operador equals op.id_operador
                                  where p.Id_Pedido == ID
                                  select op.no_empleado).FirstOrDefault().ToString();

            string usuario_cc = (from p in context.Pedido
                                 join u in context.usuarios on p.Id_Usuario equals u.id_usuario
                                 where p.Id_Pedido == ID
                                 select u.username).FirstOrDefault();

            XmlDocument soapEnvelopeXml = new XmlDocument();

            string bodyXML = string.Format(@"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                      <soap:Body>
                                        <RegistrarPedidoRepartidor xmlns='http://tempuri.org/'>
                                          <foliovale>{0}</foliovale>
                                          <vendedor>{1}</vendedor>
                                          <usuario>{2}</usuario>
                                          <numpedido>{3}</numpedido>
                                        </RegistrarPedidoRepartidor>
                                      </soap:Body>
                                    </soap:Envelope>", var_pedido.ValeTodo, no_operador, usuario_cc, ID.ToString());

            soapEnvelopeXml.LoadXml(bodyXML);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://finlag.dyndns.org/wscombugas/WSCombugas.asmx");
            webRequest.Headers.Add("SOAPAction", "http://tempuri.org/RegistrarPedidoRepartidor");
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Method = "POST";

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResult);
            XmlNodeList elemlist = xmlDoc.GetElementsByTagName("RegistrarPedidoRepartidorResult");

            var_pedido.RespuestaRegistro = elemlist[0].InnerXml;
            var_pedido.ReciboValeGas = true;
            context.SubmitChanges();
            return elemlist[0].InnerXml;
        }

        #region ENTREGAR PEDIDO
        [WebMethod]
        public string[] ENTREGAPEDIDO(int ID, string CANTIDAD, string IMPORTE, string KILOS,
   string LITROS, string PINICIAL, string PFINAL, bool SURTIDO, string IPD, string IDPRO, int IDOPERA, string IN, string OBSERVACIONES_INSTALACION = null, string CAPACIDADES = null, bool validacionValeGas = false)
        {
            string quepasa = "";
            List<string> actualizado = new List<string>();
            string[] IDPEDIDODETA, IDPRODUCTO, CANTIDADES, IMPORTES, KILO, LITRO, PINICI, PFINA, PCAPACIDADES;
            IDPEDIDODETA = IPD.Split('|');
            IDPRODUCTO = IDPRO.Split('|');
            CANTIDADES = CANTIDAD.Split('|');
            IMPORTES = IMPORTE.Split('|');
            KILO = KILOS.Split('|');
            LITRO = LITROS.Split('|');
            PINICI = PINICIAL.Split('|');
            PFINA = PFINAL.Split('|');
            try
            {
                PCAPACIDADES = CAPACIDADES.Split('|');
            }
            catch (Exception ex)
            {
                PCAPACIDADES = null;
            }
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            try
            {
                Pedido var_pedido = context.Pedido.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                string ValeTodo = var_pedido.ValeTodo;
                int cilindroT = 0;
                int cilindroC = 0;
                int garrafon = 0;
                int garrafon_alkalina = 0;
                int six_500 = 0;
                int six_l = 0;
                int six_500_alk = 0;
                double litrosE = 0;
                if (var_pedido.regresar_pedido == 0 || var_pedido.regresar_pedido == null)
                {
                    bool? valor = false;
                    bool sientregado = false;
                    if (var_pedido != null)
                    {
                        valor = var_pedido.asignado;
                    }
                    if (valor == true & SURTIDO)
                    {
                        if (ValeTodo != null)
                        {
                            if (ValeTodo.Length > 0)
                            {
                                if (validacionValeGas)
                                {
                                    #region CONSULTAS PARA TRAER DATOS NECESARIOS PARA CONSUMIR EL WEBSERVICES
                                    string no_operador = (from p in context.Pedido
                                                          join d in context.Pedido_Detalle on p.Id_Pedido equals d.Id_Pedido
                                                          join op in context.operador on d.id_operador equals op.id_operador
                                                          where p.Id_Pedido == ID
                                                          select op.no_empleado).FirstOrDefault().ToString();

                                    string usuario_cc = (from p in context.Pedido
                                                         join u in context.usuarios on p.Id_Usuario equals u.id_usuario
                                                         where p.Id_Pedido == ID
                                                         select u.username).FirstOrDefault();
                                    #endregion

                                    #region LLAMADA A WEBSERVICES DE FINLAG
                                    XmlDocument soapEnvelopeXml = new XmlDocument();

                                    string bodyXML = string.Format(@"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                      <soap:Body>
                                        <RegistrarPedidoRepartidor xmlns='http://tempuri.org/'>
                                          <foliovale>{0}</foliovale>
                                          <vendedor>{1}</vendedor>
                                          <usuario>{2}</usuario>
                                          <numpedido>{3}</numpedido>
                                        </RegistrarPedidoRepartidor>
                                      </soap:Body>
                                    </soap:Envelope>", var_pedido.ValeTodo, no_operador, usuario_cc, ID.ToString());

                                    soapEnvelopeXml.LoadXml(bodyXML);

                                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://finlag.dyndns.org/wscombugas/WSCombugas.asmx");
                                    webRequest.Headers.Add("SOAPAction", "http://tempuri.org/RegistrarPedidoRepartidor");
                                    webRequest.ContentType = "text/xml; charset=utf-8";
                                    webRequest.Method = "POST";

                                    using (Stream stream = webRequest.GetRequestStream())
                                    {
                                        soapEnvelopeXml.Save(stream);
                                    }

                                    IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                                    asyncResult.AsyncWaitHandle.WaitOne();

                                    string soapResult;
                                    using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                                    {
                                        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                                        {
                                            soapResult = rd.ReadToEnd();
                                        }
                                    }

                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(soapResult);
                                    XmlNodeList elemlist = xmlDoc.GetElementsByTagName("RegistrarPedidoRepartidorResult");

                                    var_pedido.RespuestaRegistro = elemlist[0].InnerXml;
                                    var_pedido.ReciboValeGas = validacionValeGas;
                                    context.SubmitChanges();

                                    if (elemlist[0].InnerXml != "VALE REGISTRADO")
                                    {
                                        actualizado.Add("FINLAGUNA");
                                        actualizado.Add(elemlist[0].InnerXml);
                                        return actualizado.ToArray();
                                    }
                                    else
                                    {
                                        #region REALIZAR PROCESO NORMAL
                                        #region Salvar region despues de comentar lineas
                                        int ET = IDPEDIDODETA.Count();
                                        int detallesReales = (from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd.Id_Pedido_Detalle).Count();
                                        if (ET > 0)
                                        {
                                            #region DETALLES DE ENTREGA
                                            for (int i = 0; i < ET; i++)
                                            {
                                                #region Declaracion de Variables
                                                int Id_Pedido_Det = 0;
                                                int Id_Producto = 0;
                                                double Cantidad = 0;
                                                double Importe = 0.0;
                                                double Kilos = 0.0;
                                                double Litros = 0.0;
                                                int porc_ini = 0;
                                                int porc_fin = 0;
                                                #endregion

                                                #region Llenado de variables
                                                try
                                                {
                                                    Id_Pedido_Det = Convert.ToInt32(IDPEDIDODETA[i], CultureInfo.InvariantCulture);
                                                    Id_Producto = Convert.ToInt32(IDPRODUCTO[i], CultureInfo.InvariantCulture);
                                                    Cantidad = Convert.ToDouble(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                    Importe = Convert.ToDouble(double.Parse(IMPORTES[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                    Kilos = Convert.ToDouble(double.Parse(KILO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                    Litros = Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                    porc_ini = Convert.ToInt32(PINICI[i], CultureInfo.InvariantCulture);
                                                    porc_fin = Convert.ToInt32(PFINA[i], CultureInfo.InvariantCulture);
                                                }
                                                catch (Exception llenado)
                                                {
                                                    quepasa += "['llenado':{" + llenado.Message + "}]";
                                                    #region MOVIMIENTOS PEDIDOS
                                                    movimientos_pedidos movs = new movimientos_pedidos();
                                                    movs.id_usuario = 10;
                                                    movs.id_pedido = ID;
                                                    movs.hora = DateTime.Now.TimeOfDay;
                                                    movs.fecha = DateTime.Now.Date;
                                                    movs.funcion = "INCOMPLETO APP OPERADOR";
                                                    movs.sistema = "NO TIENE DETALLES ";
                                                    movs.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                                    context.movimientos_pedidos.InsertOnSubmit(movs);
                                                    context.SubmitChanges();

                                                    #endregion
                                                    actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                                                    return actualizado.ToArray();
                                                }
                                                #endregion

                                                if (Id_Pedido_Det == -9 & detallesReales == ET & detallesReales > 1)
                                                {
                                                    actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " ");
                                                    return actualizado.ToArray();
                                                }
                                                else if (Id_Pedido_Det == -9 & detallesReales == ET & detallesReales == 1)
                                                {
                                                    IDPEDIDODETA[i] = (from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd.Id_Pedido_Detalle).FirstOrDefault() + "";
                                                    Id_Pedido_Det = Convert.ToInt32(IDPEDIDODETA[i], CultureInfo.InvariantCulture);
                                                }

                                                #region Si es nueva entrega IF
                                                if (Id_Pedido_Det == -9)
                                                {
                                                    try
                                                    {
                                                        Pedido_Detalle pd = new Pedido_Detalle();
                                                        #region llenado inicial de detalle
                                                        pd.Id_Pedido = ID;
                                                        pd.Id_producto = Id_Producto;
                                                        pd.id_operador = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.id_operador).FirstOrDefault();
                                                        pd.id_servicio = (from prd in context.producto where prd.id_producto == pd.Id_producto select prd.id_servicio).FirstOrDefault();
                                                        pd.cantidad = Cantidad;
                                                        pd.importe = Importe;
                                                        pd.Kg_Lts_a_surtir = Kilos;
                                                        pd.litros_a_surtir = Litros;
                                                        pd.porc_final = porc_fin;
                                                        pd.porc_inicio = porc_ini;
                                                        pd.tipo_unidad = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.tipo_unidad).FirstOrDefault();
                                                        pd.unidad_de_liq = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();

                                                        #endregion

                                                        #region llenado de entrega del detalle
                                                        pd.cantidad_entregada = Cantidad;
                                                        pd.id_producto_entregado = Id_Producto;
                                                        pd.importe_entregada = Importe;
                                                        pd.Kg_Lts_a_surtir_entregada = Kilos;
                                                        pd.litros_a_surtir_entregados = Litros;
                                                        pd.porc_final_entregada = porc_fin;
                                                        pd.porc_inicio_entregada = porc_ini;
                                                        pd.unidad_de_liq_entregada = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();
                                                        context.Pedido_Detalle.InsertOnSubmit(pd);
                                                        context.SubmitChanges();
                                                        sientregado = true;

                                                        if (Id_Producto == 2)//cilindro 30
                                                        {
                                                            cilindroT = cilindroT + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 3)//cilindro 45
                                                        {
                                                            cilindroC = cilindroC + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 4)//awa normal
                                                        {
                                                            garrafon = garrafon + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 7)//awa alkalina
                                                        {
                                                            garrafon_alkalina = garrafon_alkalina + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 8)//six 500ml
                                                        {
                                                            six_500 = six_500 + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 10)//six 1L
                                                        {
                                                            six_l = six_l + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 14)//six 500ml alkalina
                                                        {
                                                            six_500_alk = six_500_alk + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 9)//estacionario
                                                        {
                                                            litrosE = litrosE + Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                        }
                                                        #endregion
                                                    }
                                                    catch (Exception)
                                                    {
                                                        sientregado = true;
                                                    }
                                                }
                                                #endregion
                                                #region Solo entrega de pedido
                                                else
                                                {
                                                    Pedido_Detalle pd = new Pedido_Detalle();
                                                    pd = context.Pedido_Detalle.Where(x => x.Id_Pedido == ID & x.Id_Pedido_Detalle == Convert.ToInt32(IDPEDIDODETA[i])).SingleOrDefault();
                                                    try
                                                    {
                                                        #region llenado de entrega del detalle
                                                        pd.cantidad_entregada = Cantidad;
                                                        pd.id_producto_entregado = Id_Producto;
                                                        pd.importe_entregada = Importe;
                                                        pd.Kg_Lts_a_surtir_entregada = Kilos;
                                                        pd.litros_a_surtir_entregados = Litros;
                                                        pd.porc_final_entregada = porc_fin;
                                                        pd.porc_inicio_entregada = porc_ini;
                                                        pd.unidad_de_liq_entregada = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();
                                                        context.SubmitChanges();
                                                        sientregado = true;

                                                        if (Id_Producto == 2)//cilindro 30
                                                        {
                                                            cilindroT = cilindroT + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 3)//cilindro 45
                                                        {
                                                            cilindroC = cilindroC + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 4)//awa normal
                                                        {
                                                            garrafon = garrafon + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 7)//awa alkalina
                                                        {
                                                            garrafon_alkalina = garrafon_alkalina + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 8)//six 500ml
                                                        {
                                                            six_500 = six_500 + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 10)//six 1L
                                                        {
                                                            six_l = six_l + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 14)//six 500ml alkalina
                                                        {
                                                            six_500_alk = six_500_alk + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                                        }
                                                        if (Id_Producto == 9)//estacionario
                                                        {
                                                            litrosE = litrosE + Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                                        }
                                                        #endregion
                                                    }
                                                    catch (Exception)
                                                    {
                                                        sientregado = false;
                                                    }
                                                }
                                                #endregion
                                            }
                                            #endregion
                                            if (sientregado)
                                            {
                                                #region marcar todos los demas pedido detalle en 0
                                                int detallesNoEntregados = (from pd in context.Pedido_Detalle
                                                                            where pd.Id_Pedido.Equals(ID) & pd.cantidad_entregada == null
                                                                            select pd.Id_Pedido_Detalle).Count();
                                                if (detallesNoEntregados > 0)
                                                {
                                                    var ejecutaProc = context.sp_Operadores_App_NoEntregados(1, ID).ToList();
                                                    foreach (var item in ejecutaProc)
                                                    {
                                                        if (item.IDPEDIDO == ID)
                                                        {
                                                            quepasa += "[DETALLES SIN ENTREGA CON EXITO] ";
                                                        }
                                                        else
                                                        {
                                                            quepasa += "[DETALLES SIN ENTREGA SIN EXITO] ";
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region CICLO PEDIDO
                                                ciclo_pedido c = new ciclo_pedido();
                                                c = context.ciclo_pedido.Where(x => x.id_pedido == ID).SingleOrDefault();
                                                if (c != null)
                                                {
                                                    c.hora_entregado = DateTime.Now.TimeOfDay;
                                                    try
                                                    {
                                                        #region Primera asignacion de ciclo
                                                        var fechahora = c.alta_radios.ToString().Substring(0, 10) + " " + c.hora_radiado.ToString();
                                                        var fechahora2 = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();
                                                        DateTime fecha1 = Convert.ToDateTime(fechahora);

                                                        DateTime fecha2 = Convert.ToDateTime(fechahora2);

                                                        TimeSpan result = fecha2.Subtract(fecha1);

                                                        c.dif_radi_entre = result;
                                                        c.alta_entregado = DateTime.Now.Date;
                                                        context.SubmitChanges();
                                                        quepasa += "[ACTUALIZO CICLO PEDIDO]";
                                                        #endregion
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        quepasa += "[ERROR EN CICLO PEDIDO 1: " + ex.Message + "]";
                                                        try
                                                        {
                                                            #region Segunda asignacion de ciclo
                                                            c.alta_radios = DateTime.Now.Date;
                                                            c.hora_radiado = DateTime.Now.TimeOfDay;
                                                            c.hora_entregado = DateTime.Now.TimeOfDay;

                                                            var fechahora = c.alta_radios.ToString().Substring(0, 10) + " " + c.hora_radiado.ToString();
                                                            var fechahora2 = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();

                                                            DateTime date2 = DateTime.Parse(fechahora2);
                                                            DateTime date1 = DateTime.Parse(fechahora);
                                                            #endregion

                                                            #region fecha despues
                                                            if (date2 > date1)
                                                            {
                                                                quepasa += "[date2 > date1]";
                                                                try
                                                                {
                                                                    quepasa += "[c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]";
                                                                    c.dif_radi_entre = c.hora_entregado - c.hora_radiado;
                                                                    context.SubmitChanges();
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    quepasa += "[trono:   c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]";
                                                                    c.dif_radi_entre = c.hora_radiado - c.hora_entregado;
                                                                    quepasa += "[c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]= " + c.dif_radi_entre;
                                                                    context.SubmitChanges();
                                                                }
                                                            }
                                                            #endregion

                                                            #region Misma fecha
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    c.dif_radi_entre = date2 - date1;
                                                                    context.SubmitChanges();
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    c.dif_radi_entre = date1 - date2;
                                                                    context.SubmitChanges();
                                                                }
                                                            }
                                                            #endregion

                                                            c.alta_entregado = DateTime.Now.Date;
                                                            context.SubmitChanges();
                                                            quepasa += "[ACTUALIZO CILCO PEDIDO 2]";
                                                        }
                                                        catch (Exception exo)
                                                        {
                                                            quepasa += "[TRONO CICLO PEDIDO 2: " + exo.Message + "]";
                                                        }
                                                    }

                                                }
                                                #endregion

                                                #region ENTREGA
                                                if (var_pedido != null)
                                                {
                                                    quepasa += "[SURTIENDO]";
                                                    var_pedido.Pedido_confirmado_operador = true;
                                                    var_pedido.fase = "CONFIRMACIÓN";
                                                    var_pedido.asignado = true;
                                                    var_pedido.enasignacion = true;
                                                    actualizado.Add("1");
                                                    context.SubmitChanges();
                                                    #region DIARIOS
                                                    pedidos_diarios pedDiaD = new pedidos_diarios();
                                                    pedDiaD = context.pedidos_diarios.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                                                    if (pedDiaD != null)
                                                    {
                                                        pedDiaD.Fecha_Hora_Entrega = DateTime.Now;
                                                        pedDiaD.Usuario_Entrega = 10;
                                                        pedDiaD.Usuario_Entrega_Nombre = "APPMOVIL";

                                                        context.SubmitChanges();
                                                    }

                                                    #endregion
                                                }
                                                #endregion

                                                #region EN SITIO
                                                var ensi = (from sit in context.EnSitio
                                                            where sit.Id_Pedido == ID
                                                            select sit).ToList();
                                                foreach (var m in ensi)
                                                {
                                                    context.EnSitio.DeleteOnSubmit(m);
                                                    context.SubmitChanges();

                                                }
                                                if (IN.Length > 3)
                                                {
                                                    try
                                                    {
                                                        EnSitio ensitio = new EnSitio();
                                                        ensitio.CheckOut = DateTime.Now.TimeOfDay;
                                                        quepasa += "[ENSITIO] : " + DateTime.Now.TimeOfDay + " | ";
                                                        ensitio.CheckIn = TimeSpan.Parse(IN);
                                                        quepasa += "[ENSITIO] : " + TimeSpan.Parse(IN) + " | ";
                                                        ensitio.Diferiencia = ensitio.CheckOut - ensitio.CheckIn;
                                                        quepasa += "[ENSITIO] : " + ensitio.CheckOut + "- " + ensitio.CheckIn + " | ";
                                                        ensitio.Fecha = DateTime.Now.Date;
                                                        ensitio.Id_Operador = IDOPERA;
                                                        ensitio.Id_Pedido = ID;
                                                        context.EnSitio.InsertOnSubmit(ensitio);
                                                        context.SubmitChanges();
                                                        quepasa += "[ENTRO A ENSITIO] : " + DateTime.Now.TimeOfDay + " | ";
                                                    }
                                                    catch (Exception re)
                                                    {
                                                        quepasa += "[ERROR ENSITIO]: | " + re.Message + " |";
                                                        EnSitio ensitio = new EnSitio();
                                                        ensitio.CheckIn = ensitio.CheckOut;
                                                        ensitio.Diferiencia = ensitio.CheckOut - ensitio.CheckIn;
                                                        ensitio.Fecha = DateTime.Now.Date;
                                                        ensitio.Id_Operador = IDOPERA;
                                                        ensitio.Id_Pedido = ID;
                                                        context.EnSitio.InsertOnSubmit(ensitio);
                                                        context.SubmitChanges();
                                                        quepasa += "[ENTRO A ENSITIO 2]";
                                                    }
                                                }
                                                #endregion

                                                #region RADIOS PEDIDOS
                                                Radios_Pedidos rp = new Radios_Pedidos();
                                                rp.fecha = DateTime.Now.Date;
                                                rp.id_operador = IDOPERA;
                                                rp.id_pedido = ID;
                                                rp.id_ruta = (int)var_pedido.Id_ruta;
                                                rp.id_usuario = 10;
                                                rp.tiempo = DateTime.Now.TimeOfDay;
                                                rp.tipo = "ENTREGA";
                                                context.Radios_Pedidos.InsertOnSubmit(rp);
                                                context.SubmitChanges();
                                                #endregion

                                                #region MOVIMIENTOS PEDIDOS
                                                movimientos_pedidos mov = new movimientos_pedidos();
                                                mov.id_usuario = 10;
                                                mov.id_pedido = ID;
                                                mov.hora = c.hora_entregado;
                                                mov.fecha = DateTime.Now.Date;
                                                mov.funcion = "ENTREGA PEDIDO";
                                                mov.sistema = "PEDIDO ENTREGADO DESDE APP OPERADOR (PEDIDO,HORA,OPERADOR)";
                                                mov.datos = "" + ID + "," + mov.hora + "," + IDOPERA + ", LOG:" + quepasa;
                                                context.movimientos_pedidos.InsertOnSubmit(mov);
                                                context.SubmitChanges();

                                                #endregion

                                                #region TIEMPO FASES
                                                try
                                                {
                                                    tiempos_monitorizacion_fases pedidofase = (
                                                        from SQLPedidoFase in context.tiempos_monitorizacion_fases
                                                        where SQLPedidoFase.idpedido.Equals(ID)
                                                        select SQLPedidoFase
                                                        ).FirstOrDefault();
                                                    quepasa += "[TIEMPOS FASES 1]";
                                                    DateTime fechacapturado = pedidofase.tmffechacaptura;
                                                    quepasa += "[TIEMPOS FASES 2]" + fechacapturado;
                                                    DateTime fechaentregado = DateTime.Now;
                                                    quepasa += "[TIEMPOS FASES 2]" + fechaentregado;
                                                    TimeSpan tiempodeentrega = fechaentregado - fechacapturado;
                                                    quepasa += "[TIEMPOS FASES 2]" + tiempodeentrega;

                                                    Asigna_Pedido asignacionpedidoentrega = (
                                                        from SQLASignacionPedido in context.Asigna_Pedido
                                                        where SQLASignacionPedido.id_pedido.Equals(ID)
                                                        select SQLASignacionPedido
                                                        ).FirstOrDefault();
                                                    Asignaciones asignacionderuta = (
                                                    from SQLAsignacionRuta in context.Asignaciones
                                                    where SQLAsignacionRuta.id_ruta.Equals(asignacionpedidoentrega.id_ruta) && SQLAsignacionRuta.id_operador.Equals(asignacionpedidoentrega.id_operador) && SQLAsignacionRuta.asignacion_activa.Equals(true)
                                                    select SQLAsignacionRuta
                                                    ).FirstOrDefault();
                                                    if (asignacionderuta.Equals(null))
                                                    {
                                                        asignacionderuta = (
                                                        from SQLAsignacionRuta in context.Asignaciones
                                                        where SQLAsignacionRuta.id_ruta.Equals(asignacionpedidoentrega.id_ruta) && SQLAsignacionRuta.id_operador.Equals(asignacionpedidoentrega.id_operador) && SQLAsignacionRuta.asignacion_activa.Equals(false)
                                                        orderby SQLAsignacionRuta.id_asignacion descending
                                                        select SQLAsignacionRuta
                                                        ).FirstOrDefault();
                                                    }
                                                    pedidofase.tmffechaentrega = fechaentregado;
                                                    pedidofase.tmftiempoentrega = tiempodeentrega.TotalSeconds;
                                                    pedidofase.tmfusuarioentrega = asignacionpedidoentrega.id_operador;
                                                    pedidofase.tmfunidadentrega = asignacionderuta.id_truck;

                                                    context.SubmitChanges();
                                                }
                                                catch (Exception exs)
                                                {
                                                    quepasa += "[TIEMPOS FASES 2]" + exs.Message.ToString();
                                                }
                                                #endregion

                                                #region PROCEDIMIENTO AJUSTA VENTAS EN CASO DE QUE NO SE GUARDEN BIEN
                                                try
                                                {
                                                    int x = context.AjustaVentasRepartidor(ID, 10, null, null);
                                                    Console.WriteLine("SI");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.Write("NO");
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region MOVIMIENTOS PEDIDOS
                                                movimientos_pedidos mov = new movimientos_pedidos();
                                                mov.id_usuario = 10;
                                                mov.id_pedido = ID;
                                                mov.hora = DateTime.Now.TimeOfDay;
                                                mov.fecha = DateTime.Now.Date;
                                                mov.funcion = "INCOMPLETO APP OPERADOR";
                                                mov.sistema = "NO TIENE DETALLES ";
                                                mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                                context.movimientos_pedidos.InsertOnSubmit(mov);
                                                context.SubmitChanges();
                                                #endregion

                                                actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                                            }
                                            #region CAMBIA SERVICIO
                                            var deta = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) && pd.cantidad_entregada > 0 select pd;

                                            int idServicio = var_pedido.Id_Servicio;
                                            if (deta.All(x => x.id_servicio.Equals(1))) // gas
                                            {
                                                idServicio = 1;
                                            }
                                            else if (deta.All(x => x.id_servicio.Equals(3)))
                                            {
                                                idServicio = 3;
                                            }
                                            else
                                            {
                                                idServicio = 6;
                                            }

                                            var_pedido.Id_Servicio = idServicio;
                                            context.SubmitChanges();
                                            #endregion

                                            #region VENTAS
                                            var ventas = (from ve in context.ventas_repartidor where ve.id_pedido == ID select ve).FirstOrDefault();
                                            if (ventas != null)
                                            {
                                                ventas.cil_t = cilindroT;
                                                ventas.cil_c = cilindroC;
                                                ventas.awa_g = garrafon;
                                                ventas.awa_alk_g = garrafon_alkalina;
                                                ventas.six_500 = six_500;
                                                ventas.six_l = six_l;
                                                ventas.six_500_alk = six_500_alk;
                                                ventas.lit_e = litrosE;
                                                ventas.es_venta = true;
                                                if (ventas.fecha.Value.Date != DateTime.Now.Date)
                                                {
                                                    ventas.fecha = DateTime.Now;
                                                }
                                                context.SubmitChanges();
                                            }
                                            else
                                            {
                                                ventas_repartidor vrep = new ventas_repartidor();
                                                vrep.cil_t_a = cilindroT;
                                                vrep.cil_c_a = cilindroC;
                                                vrep.awa_g_a = garrafon;
                                                vrep.awa_alk_g_a = garrafon_alkalina;
                                                ventas.six_500_a = six_500;
                                                ventas.six_l_a = six_l;
                                                ventas.six_500_alk_a = six_500_alk;
                                                vrep.lit_e_a = litrosE;
                                                vrep.cil_t = cilindroT;
                                                vrep.cil_c = cilindroC;
                                                vrep.awa_g = garrafon;
                                                vrep.awa_alk_g = garrafon_alkalina;
                                                ventas.six_500 = six_500;
                                                ventas.six_l = six_l;
                                                ventas.six_500_alk = six_500_alk;
                                                vrep.lit_e = litrosE;
                                                vrep.es_venta = true;
                                                vrep.fecha = DateTime.Now;
                                                vrep.id_pedido = ID;
                                                vrep.id_operador = IDOPERA;
                                                context.ventas_repartidor.InsertOnSubmit(vrep);
                                                context.SubmitChanges();
                                            }
                                            #endregion

                                            #region Si entrega y es app. Confirma
                                            int origen = 0;
                                            origen = var_pedido.origen;
                                            int user_movil = 10;
                                            configuraciontbl config = new configuraciontbl();
                                            config = context.configuraciontbl.Where(x => x.id_configuracion == 1).FirstOrDefault();
                                            if (config.configuracion_temporada == 1)
                                            {
                                                origen = 2;
                                                user_movil = 10;
                                            }
                                            if (sientregado && origen.Equals(2))
                                            {
                                                var IPDs = (
                                                    from SQLDetalles in context.Pedido_Detalle
                                                    where SQLDetalles.Id_Pedido.Equals(ID)
                                                    select SQLDetalles
                                                ).ToList();

                                                IPDs.ForEach(
                                                    x =>
                                                    {
                                                        x.cantidad_confirmado = Convert.ToDouble(x.cantidad_entregada);
                                                        x.importe_confirmado = x.importe_entregada;
                                                        x.Kg_Lts_a_surtir_confirmado = x.Kg_Lts_a_surtir_entregada;
                                                        x.litros_a_surtir_confirmados = x.litros_a_surtir_entregados;
                                                        x.porc_final_confirmado = x.porc_final_entregada;
                                                        x.porc_inicio_confirmado = x.porc_inicio_entregada;
                                                        x.id_producto_confirmado = x.id_producto_entregado;
                                                        x.unidad_de_liq_confirmado = x.unidad_de_liq;
                                                    }
                                                );

                                                #region Ciclo Pedido
                                                ciclo_pedido c = context.ciclo_pedido.Where(x => x.id_pedido == ID).SingleOrDefault();
                                                if (c != null)
                                                {
                                                    c.hora_confirmado = DateTime.Now.TimeOfDay;
                                                    c.alta_confirmado = DateTime.Now.Date;
                                                    var entrega = c.alta_entregado.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();
                                                    var confirmado = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_confirmado.ToString();
                                                    DateTime date2 = DateTime.Parse(confirmado);
                                                    DateTime date1 = DateTime.Parse(entrega);
                                                    if (date2 > date1)
                                                    {
                                                        try
                                                        {
                                                            c.dif_entre_confirma = c.hora_confirmado - c.hora_entregado;
                                                            context.SubmitChanges();
                                                        }
                                                        catch (Exception)
                                                        {
                                                            c.dif_entre_confirma = c.hora_entregado - c.hora_confirmado;
                                                            context.SubmitChanges();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        c.dif_entre_confirma = date2 - date1;
                                                        context.SubmitChanges();
                                                    }
                                                    var recepcion = c.creacion.ToString().Substring(0, 10) + " " + c.hora_recepcion.ToString();
                                                    DateTime date3 = DateTime.Parse(recepcion);
                                                    if (date2 > date3)
                                                    {
                                                        try
                                                        {
                                                            c.total_time = c.hora_confirmado - c.hora_recepcion;
                                                            context.SubmitChanges();
                                                        }
                                                        catch (Exception)
                                                        {
                                                            c.total_time = c.hora_recepcion - c.hora_confirmado;
                                                            context.SubmitChanges();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        c.total_time = date2 - date3;
                                                        context.SubmitChanges();
                                                    }
                                                }
                                                #endregion

                                                if (var_pedido != null)
                                                {
                                                    var_pedido.Pedido_confirmado_cliente = true;
                                                    var_pedido.verificar_litrometro = true;
                                                    var_pedido.Entrega_Nota = true;
                                                    var_pedido.Metodo_Pago_Confirmado = var_pedido.Id_Metodo_Pago;
                                                    var_pedido.dar_cambio = true;
                                                    var_pedido.conformidad_servicio = true;
                                                    var_pedido.motivo_no_conforme = null;
                                                    var_pedido.volveria_comprar = true;
                                                    var_pedido.motivo_no_volver_comprar = null;
                                                    var_pedido.quien_confirma = var_pedido.Cliente.nombre;
                                                    var_pedido.Completo = true;
                                                    var_pedido.Observaciones = "PEDIDO :" + ID + ", ENTREGADO, CONFIRMADO POR: " + var_pedido.Cliente.nombre + ", ESTATUS: FINALIZADO. HORA DE FINALIZACIÓN: " + DateTime.Now.TimeOfDay + ", FECHA : " + DateTime.Now.Date.ToShortDateString();
                                                    var_pedido.fase = "CONFIRMACIÓN";
                                                    context.SubmitChanges();

                                                    #region DIARIOS
                                                    pedidos_diarios pedDia = new pedidos_diarios();
                                                    pedDia = context.pedidos_diarios.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                                                    if (pedDia != null)
                                                    {
                                                        pedDia.Fecha_Hora_Confirmacion = DateTime.Now;
                                                        pedDia.Usuario_Confirmacion = user_movil;
                                                        pedDia.Usuario_Confirmacion_Nombre = "APPMOVIL";

                                                        context.SubmitChanges();
                                                    }
                                                    #endregion
                                                }
                                                try
                                                {
                                                    #region funciones para tiempos de fase
                                                    tiempos_monitorizacion_fases pedidofase = (
                                                        from SQLPedidoFase in context.tiempos_monitorizacion_fases
                                                        where SQLPedidoFase.idpedido.Equals(ID)
                                                        select SQLPedidoFase
                                                    ).FirstOrDefault();

                                                    DateTime fechacapturado = pedidofase.tmffechacaptura;
                                                    DateTime fechaconfirmado = DateTime.Now;
                                                    TimeSpan tiempodeconfirma = fechaconfirmado - fechacapturado;

                                                    pedidofase.tmffechaconfirmacion = fechaconfirmado;
                                                    pedidofase.tmftiempoconfirmacion = tiempodeconfirma.TotalSeconds;
                                                    pedidofase.tmfusuarioconfirmacion = 10;

                                                    context.SubmitChanges();
                                                    #endregion
                                                }
                                                catch (Exception hexs)
                                                {

                                                }

                                                #region FUNCIONES CONFIRMACION PEDIDO
                                                Confirmacion_Pedidos rp = new Confirmacion_Pedidos();
                                                rp.fecha = DateTime.Now.Date;
                                                rp.id_operador = (int)(from pd in context.Pedido_Detalle where pd.Id_Pedido == ID select pd.id_operador).FirstOrDefault();
                                                rp.id_pedido = ID;
                                                rp.id_ruta = (int)var_pedido.Id_ruta;
                                                rp.id_usuario = 10;
                                                rp.tiempo = DateTime.Now.TimeOfDay;
                                                rp.tipo = "CONFIRMACION";
                                                context.Confirmacion_Pedidos.InsertOnSubmit(rp);
                                                context.SubmitChanges();
                                                #endregion

                                                #region MOVIMIENTOS PEDIDOS
                                                movimientos_pedidos mov = new movimientos_pedidos();
                                                mov.id_usuario = 10;
                                                mov.id_pedido = ID;
                                                mov.hora = c.hora_entregado;
                                                mov.fecha = DateTime.Now.Date;
                                                mov.funcion = "CONFIRMA PEDIDO TABLET";
                                                mov.sistema = "PEDIDO: " + ID + ", HORA CONFIRMA: " + mov.hora + ". - POR OPERADOR: " + rp.id_operador;
                                                mov.datos = "" + ID + "," + mov.hora;
                                                context.movimientos_pedidos.InsertOnSubmit(mov);
                                                context.SubmitChanges();
                                                #endregion

                                                #region CAMBIA SERVICIO
                                                var detas = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) && (pd.cantidad_entregada > 0) select pd;
                                                int idServicioss = var_pedido.Id_Servicio;
                                                if (detas.All(x => x.id_servicio.Equals(1))) // gas
                                                {
                                                    idServicioss = 1;
                                                }
                                                else if (detas.All(x => x.id_servicio.Equals(3)))
                                                {
                                                    idServicioss = 3;
                                                }
                                                else
                                                {
                                                    idServicioss = 6;
                                                }
                                                if (idServicioss != 6)
                                                {
                                                    detas = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd;
                                                    foreach (var item in detas)
                                                    {

                                                        item.id_servicio = idServicioss;
                                                        context.SubmitChanges();
                                                    }
                                                }
                                                var_pedido.Id_Servicio = idServicioss;
                                                context.SubmitChanges();
                                                #endregion
                                            }
                                            #endregion

                                            if (config.configuracion_temporada == 1)
                                            {
                                                var_pedido.regresar_pedido = 0;
                                                var_pedido.se_regreso_pedido = 0;
                                                var_pedido.es_manual = 0;
                                                context.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            #region MOVIMIENTOS PEDIDOS
                                            movimientos_pedidos mov = new movimientos_pedidos();
                                            mov.id_usuario = 10;
                                            mov.id_pedido = ID;
                                            mov.hora = DateTime.Now.TimeOfDay;
                                            mov.fecha = DateTime.Now.Date;
                                            mov.funcion = "INCOMPLETO APP OPERADOR";
                                            mov.sistema = "NO TIENE DETALLES ";
                                            mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                            context.movimientos_pedidos.InsertOnSubmit(mov);
                                            context.SubmitChanges();

                                            #endregion
                                            actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                                        }
                                        #endregion

                                        #region observaciones sobre la instalación
                                        try
                                        {
                                            if (OBSERVACIONES_INSTALACION != "" && OBSERVACIONES_INSTALACION != null)
                                            {
                                                string texto_instalacion = string.Format("INSTALACIÓN-> {0} - APP REPARTIDOR -", OBSERVACIONES_INSTALACION);
                                                REGISTRO_OBSERVACION(ID, texto_instalacion);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        #endregion

                                        #region funciones para actualizar la capacidad del tanque
                                        try
                                        {
                                            var caracteristicas_instalacion = (
                                            from SQLCaracteristicas in context.Esp_Tec_Direccion
                                            where SQLCaracteristicas.id_direccion.Equals(var_pedido.Id_Direccion)
                                            select SQLCaracteristicas
                                        ).FirstOrDefault();
                                            if (caracteristicas_instalacion != null)
                                            {
                                                int capacidades = Convert.ToInt32(PCAPACIDADES[0]);
                                                if (capacidades > 45)
                                                {
                                                    caracteristicas_instalacion.num_tanques = capacidades;
                                                    caracteristicas_instalacion.cap_tanque = "LTS";
                                                    context.SubmitChanges();
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            movimientos_pedidos mov = new movimientos_pedidos();
                                            mov.id_usuario = 10;
                                            mov.id_pedido = ID;
                                            mov.hora = DateTime.Now.TimeOfDay;
                                            mov.fecha = DateTime.Now.Date;
                                            mov.funcion = "ERROR EN CAPACIDAD -" + PCAPACIDADES[0];
                                            mov.sistema = ex.Message.ToUpper();
                                            mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa + ex.Message;
                                            context.movimientos_pedidos.InsertOnSubmit(mov);
                                            context.SubmitChanges();
                                        }
                                        #endregion
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    if (OBSERVACIONES_INSTALACION != "" && OBSERVACIONES_INSTALACION != null)
                                    {
                                        string texto_instalacion = string.Format("INSTALACIÓN-> {0} - APP REPARTIDOR -", OBSERVACIONES_INSTALACION);
                                        REGISTRO_OBSERVACION(ID, texto_instalacion);
                                    }

                                    DateTime fechaEntrega = DateTime.Now;
                                    string mensaje = string.Format("SE REALIZA INTENTO DE ENTREGA A LAS {0} - NO SE CONFIRMA VALEGAS EN APP REPARTIDOR", fechaEntrega.ToString("h:mm:ss tt"));

                                    REGISTRO_OBSERVACION(ID, mensaje);

                                    var_pedido.ReciboValeGas = validacionValeGas;
                                    context.SubmitChanges();

                                    actualizado.Add("1");
                                }
                            }
                        }
                        else
                        {
                            #region REALIZAR PROCESO NORMAL
                            #region Salvar region despues de comentar lineas
                            int ET = IDPEDIDODETA.Count();
                            int detallesReales = (from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd.Id_Pedido_Detalle).Count();
                            if (ET > 0)
                            {
                                #region DETALLES DE ENTREGA
                                for (int i = 0; i < ET; i++)
                                {
                                    #region Declaracion de Variables
                                    int Id_Pedido_Det = 0;
                                    int Id_Producto = 0;
                                    double Cantidad = 0;
                                    double Importe = 0.0;
                                    double Kilos = 0.0;
                                    double Litros = 0.0;
                                    int porc_ini = 0;
                                    int porc_fin = 0;
                                    #endregion

                                    #region Llenado de variables
                                    try
                                    {
                                        Id_Pedido_Det = Convert.ToInt32(IDPEDIDODETA[i], CultureInfo.InvariantCulture);
                                        Id_Producto = Convert.ToInt32(IDPRODUCTO[i], CultureInfo.InvariantCulture);
                                        Cantidad = Convert.ToDouble(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture).ToString("N2"));
                                        Importe = Convert.ToDouble(double.Parse(IMPORTES[i], CultureInfo.InvariantCulture).ToString("N2"));
                                        Kilos = Convert.ToDouble(double.Parse(KILO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                        Litros = Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                        porc_ini = Convert.ToInt32(PINICI[i], CultureInfo.InvariantCulture);
                                        porc_fin = Convert.ToInt32(PFINA[i], CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception llenado)
                                    {
                                        quepasa += "['llenado':{" + llenado.Message + "}]";
                                        #region MOVIMIENTOS PEDIDOS
                                        movimientos_pedidos movs = new movimientos_pedidos();
                                        movs.id_usuario = 10;
                                        movs.id_pedido = ID;
                                        movs.hora = DateTime.Now.TimeOfDay;
                                        movs.fecha = DateTime.Now.Date;
                                        movs.funcion = "INCOMPLETO APP OPERADOR";
                                        movs.sistema = "NO TIENE DETALLES ";
                                        movs.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                        context.movimientos_pedidos.InsertOnSubmit(movs);
                                        context.SubmitChanges();

                                        #endregion
                                        actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                                        return actualizado.ToArray();
                                    }
                                    #endregion

                                    if (Id_Pedido_Det == -9 & detallesReales == ET & detallesReales > 1)
                                    {
                                        actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " ");
                                        return actualizado.ToArray();
                                    }
                                    else if (Id_Pedido_Det == -9 & detallesReales == ET & detallesReales == 1)
                                    {
                                        IDPEDIDODETA[i] = (from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd.Id_Pedido_Detalle).FirstOrDefault() + "";
                                        Id_Pedido_Det = Convert.ToInt32(IDPEDIDODETA[i], CultureInfo.InvariantCulture);
                                    }

                                    #region Si es nueva entrega IF
                                    if (Id_Pedido_Det == -9)
                                    {
                                        try
                                        {
                                            Pedido_Detalle pd = new Pedido_Detalle();
                                            #region llenado inicial de detalle
                                            pd.Id_Pedido = ID;
                                            pd.Id_producto = Id_Producto;
                                            pd.id_operador = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.id_operador).FirstOrDefault();
                                            pd.id_servicio = (from prd in context.producto where prd.id_producto == pd.Id_producto select prd.id_servicio).FirstOrDefault();
                                            pd.cantidad = Cantidad;
                                            pd.importe = Importe;
                                            pd.Kg_Lts_a_surtir = Kilos;
                                            pd.litros_a_surtir = Litros;
                                            pd.porc_final = porc_fin;
                                            pd.porc_inicio = porc_ini;
                                            pd.tipo_unidad = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.tipo_unidad).FirstOrDefault();
                                            pd.unidad_de_liq = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();

                                            #endregion

                                            #region llenado de entrega del detalle
                                            pd.cantidad_entregada = Cantidad;
                                            pd.id_producto_entregado = Id_Producto;
                                            pd.importe_entregada = Importe;
                                            pd.Kg_Lts_a_surtir_entregada = Kilos;
                                            pd.litros_a_surtir_entregados = Litros;
                                            pd.porc_final_entregada = porc_fin;
                                            pd.porc_inicio_entregada = porc_ini;
                                            pd.unidad_de_liq_entregada = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();
                                            context.Pedido_Detalle.InsertOnSubmit(pd);
                                            context.SubmitChanges();
                                            sientregado = true;

                                            if (Id_Producto == 2)//cilindro 30
                                            {
                                                cilindroT = cilindroT + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 3)//cilindro 45
                                            {
                                                cilindroC = cilindroC + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 4)//awa normal
                                            {
                                                garrafon = garrafon + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 7)//awa alkalina
                                            {
                                                garrafon_alkalina = garrafon_alkalina + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 8)//six 500ml
                                            {
                                                six_500 = six_500 + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 10)//six 1L
                                            {
                                                six_l = six_l + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 14)//six 500ml alkalina
                                            {
                                                six_500_alk = six_500_alk + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 9)//estacionario
                                            {
                                                litrosE = litrosE + Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                            }
                                            #endregion
                                        }
                                        catch (Exception)
                                        {
                                            sientregado = true;
                                        }
                                    }
                                    #endregion
                                    #region Solo entrega de pedido
                                    else
                                    {
                                        Pedido_Detalle pd = new Pedido_Detalle();
                                        pd = context.Pedido_Detalle.Where(x => x.Id_Pedido == ID & x.Id_Pedido_Detalle == Convert.ToInt32(IDPEDIDODETA[i])).SingleOrDefault();
                                        try
                                        {
                                            #region llenado de entrega del detalle
                                            pd.cantidad_entregada = Cantidad;
                                            pd.id_producto_entregado = Id_Producto;
                                            pd.importe_entregada = Importe;
                                            pd.Kg_Lts_a_surtir_entregada = Kilos;
                                            pd.litros_a_surtir_entregados = Litros;
                                            pd.porc_final_entregada = porc_fin;
                                            pd.porc_inicio_entregada = porc_ini;
                                            pd.unidad_de_liq_entregada = (from pds in context.Pedido_Detalle where pds.Id_Pedido == ID select pds.unidad_de_liq).FirstOrDefault();
                                            context.SubmitChanges();
                                            sientregado = true;

                                            if (Id_Producto == 2)//cilindro 30
                                            {
                                                cilindroT = cilindroT + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 3)//cilindro 45
                                            {
                                                cilindroC = cilindroC + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 4)//awa normal
                                            {
                                                garrafon = garrafon + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 7)//awa alkalina
                                            {
                                                garrafon_alkalina = garrafon_alkalina + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 8)//six 500ml
                                            {
                                                six_500 = six_500 + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 10)//six 1L
                                            {
                                                six_l = six_l + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 14)//six 500ml alkalina
                                            {
                                                six_500_alk = six_500_alk + Convert.ToInt32(double.Parse(CANTIDADES[i], CultureInfo.InvariantCulture));
                                            }
                                            if (Id_Producto == 9)//estacionario
                                            {
                                                litrosE = litrosE + Convert.ToDouble(double.Parse(LITRO[i], CultureInfo.InvariantCulture).ToString("N2"));
                                            }
                                            #endregion
                                        }
                                        catch (Exception)
                                        {
                                            sientregado = false;
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                                if (sientregado)
                                {
                                    #region marcar todos los demas pedido detalle en 0
                                    int detallesNoEntregados = (from pd in context.Pedido_Detalle
                                                                where pd.Id_Pedido.Equals(ID) & pd.cantidad_entregada == null
                                                                select pd.Id_Pedido_Detalle).Count();
                                    if (detallesNoEntregados > 0)
                                    {
                                        var ejecutaProc = context.sp_Operadores_App_NoEntregados(1, ID).ToList();
                                        foreach (var item in ejecutaProc)
                                        {
                                            if (item.IDPEDIDO == ID)
                                            {
                                                quepasa += "[DETALLES SIN ENTREGA CON EXITO] ";
                                            }
                                            else
                                            {
                                                quepasa += "[DETALLES SIN ENTREGA SIN EXITO] ";
                                            }
                                        }
                                    }
                                    #endregion

                                    #region CICLO PEDIDO
                                    ciclo_pedido c = new ciclo_pedido();
                                    c = context.ciclo_pedido.Where(x => x.id_pedido == ID).SingleOrDefault();
                                    if (c != null)
                                    {
                                        c.hora_entregado = DateTime.Now.TimeOfDay;
                                        try
                                        {
                                            #region Primera asignacion de ciclo
                                            var fechahora = c.alta_radios.ToString().Substring(0, 10) + " " + c.hora_radiado.ToString();
                                            var fechahora2 = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();
                                            DateTime fecha1 = Convert.ToDateTime(fechahora);

                                            DateTime fecha2 = Convert.ToDateTime(fechahora2);

                                            TimeSpan result = fecha2.Subtract(fecha1);

                                            c.dif_radi_entre = result;
                                            c.alta_entregado = DateTime.Now.Date;
                                            context.SubmitChanges();
                                            quepasa += "[ACTUALIZO CICLO PEDIDO]";
                                            #endregion
                                        }
                                        catch (Exception ex)
                                        {
                                            quepasa += "[ERROR EN CICLO PEDIDO 1: " + ex.Message + "]";
                                            try
                                            {
                                                #region Segunda asignacion de ciclo
                                                c.alta_radios = DateTime.Now.Date;
                                                c.hora_radiado = DateTime.Now.TimeOfDay;
                                                c.hora_entregado = DateTime.Now.TimeOfDay;

                                                var fechahora = c.alta_radios.ToString().Substring(0, 10) + " " + c.hora_radiado.ToString();
                                                var fechahora2 = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();

                                                DateTime date2 = DateTime.Parse(fechahora2);
                                                DateTime date1 = DateTime.Parse(fechahora);
                                                #endregion

                                                #region fecha despues
                                                if (date2 > date1)
                                                {
                                                    quepasa += "[date2 > date1]";
                                                    try
                                                    {
                                                        quepasa += "[c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]";
                                                        c.dif_radi_entre = c.hora_entregado - c.hora_radiado;
                                                        context.SubmitChanges();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        quepasa += "[trono:   c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]";
                                                        c.dif_radi_entre = c.hora_radiado - c.hora_entregado;
                                                        quepasa += "[c.dif_radi_entre = c.hora_entregado - c.hora_radiado;]= " + c.dif_radi_entre;
                                                        context.SubmitChanges();
                                                    }
                                                }
                                                #endregion

                                                #region Misma fecha
                                                else
                                                {
                                                    try
                                                    {
                                                        c.dif_radi_entre = date2 - date1;
                                                        context.SubmitChanges();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        c.dif_radi_entre = date1 - date2;
                                                        context.SubmitChanges();
                                                    }
                                                }
                                                #endregion

                                                c.alta_entregado = DateTime.Now.Date;
                                                context.SubmitChanges();
                                                quepasa += "[ACTUALIZO CILCO PEDIDO 2]";
                                            }
                                            catch (Exception exo)
                                            {
                                                quepasa += "[TRONO CICLO PEDIDO 2: " + exo.Message + "]";
                                            }
                                        }

                                    }
                                    #endregion

                                    #region ENTREGA
                                    if (var_pedido != null)
                                    {
                                        quepasa += "[SURTIENDO]";
                                        var_pedido.Pedido_confirmado_operador = true;
                                        var_pedido.fase = "CONFIRMACIÓN";
                                        var_pedido.asignado = true;
                                        var_pedido.enasignacion = true;
                                        actualizado.Add("1");
                                        context.SubmitChanges();
                                        #region DIARIOS
                                        pedidos_diarios pedDiaD = new pedidos_diarios();
                                        pedDiaD = context.pedidos_diarios.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                                        if (pedDiaD != null)
                                        {
                                            pedDiaD.Fecha_Hora_Entrega = DateTime.Now;
                                            pedDiaD.Usuario_Entrega = 10;
                                            pedDiaD.Usuario_Entrega_Nombre = "APPMOVIL";

                                            context.SubmitChanges();
                                        }

                                        #endregion
                                    }
                                    #endregion

                                    #region EN SITIO
                                    var ensi = (from sit in context.EnSitio
                                                where sit.Id_Pedido == ID
                                                select sit).ToList();
                                    foreach (var m in ensi)
                                    {
                                        context.EnSitio.DeleteOnSubmit(m);
                                        context.SubmitChanges();
                                    }
                                    if (IN.Length > 3)
                                    {
                                        try
                                        {
                                            EnSitio ensitio = new EnSitio();
                                            ensitio.CheckOut = DateTime.Now.TimeOfDay;
                                            quepasa += "[ENSITIO] : " + DateTime.Now.TimeOfDay + " | ";
                                            ensitio.CheckIn = TimeSpan.Parse(IN);
                                            quepasa += "[ENSITIO] : " + TimeSpan.Parse(IN) + " | ";
                                            ensitio.Diferiencia = ensitio.CheckOut - ensitio.CheckIn;
                                            quepasa += "[ENSITIO] : " + ensitio.CheckOut + "- " + ensitio.CheckIn + " | ";
                                            ensitio.Fecha = DateTime.Now.Date;
                                            ensitio.Id_Operador = IDOPERA;
                                            ensitio.Id_Pedido = ID;
                                            context.EnSitio.InsertOnSubmit(ensitio);
                                            context.SubmitChanges();
                                            quepasa += "[ENTRO A ENSITIO] : " + DateTime.Now.TimeOfDay + " | ";
                                        }
                                        catch (Exception re)
                                        {
                                            quepasa += "[ERROR ENSITIO]: | " + re.Message + " |";
                                            EnSitio ensitio = new EnSitio();
                                            ensitio.CheckIn = ensitio.CheckOut;
                                            ensitio.Diferiencia = ensitio.CheckOut - ensitio.CheckIn;
                                            ensitio.Fecha = DateTime.Now.Date;
                                            ensitio.Id_Operador = IDOPERA;
                                            ensitio.Id_Pedido = ID;
                                            context.EnSitio.InsertOnSubmit(ensitio);
                                            context.SubmitChanges();
                                            quepasa += "[ENTRO A ENSITIO 2]";
                                        }
                                    }
                                    #endregion

                                    #region RADIOS PEDIDOS
                                    Radios_Pedidos rp = new Radios_Pedidos();
                                    rp.fecha = DateTime.Now.Date;
                                    rp.id_operador = IDOPERA;
                                    rp.id_pedido = ID;
                                    rp.id_ruta = (int)var_pedido.Id_ruta;
                                    rp.id_usuario = 10;
                                    rp.tiempo = DateTime.Now.TimeOfDay;
                                    rp.tipo = "ENTREGA";
                                    context.Radios_Pedidos.InsertOnSubmit(rp);
                                    context.SubmitChanges();
                                    #endregion

                                    #region MOVIMIENTOS PEDIDOS
                                    movimientos_pedidos mov = new movimientos_pedidos();
                                    mov.id_usuario = 10;
                                    mov.id_pedido = ID;
                                    mov.hora = c.hora_entregado;
                                    mov.fecha = DateTime.Now.Date;
                                    mov.funcion = "ENTREGA PEDIDO";
                                    mov.sistema = "PEDIDO ENTREGADO DESDE APP OPERADOR (PEDIDO,HORA,OPERADOR)";
                                    mov.datos = "" + ID + "," + mov.hora + "," + IDOPERA + ", LOG:" + quepasa;
                                    context.movimientos_pedidos.InsertOnSubmit(mov);
                                    context.SubmitChanges();

                                    #endregion

                                    #region TIEMPO FASES
                                    try
                                    {
                                        tiempos_monitorizacion_fases pedidofase = (
                                            from SQLPedidoFase in context.tiempos_monitorizacion_fases
                                            where SQLPedidoFase.idpedido.Equals(ID)
                                            select SQLPedidoFase
                                            ).FirstOrDefault();
                                        quepasa += "[TIEMPOS FASES 1]";
                                        DateTime fechacapturado = pedidofase.tmffechacaptura;
                                        quepasa += "[TIEMPOS FASES 2]" + fechacapturado;
                                        DateTime fechaentregado = DateTime.Now;
                                        quepasa += "[TIEMPOS FASES 2]" + fechaentregado;
                                        TimeSpan tiempodeentrega = fechaentregado - fechacapturado;
                                        quepasa += "[TIEMPOS FASES 2]" + tiempodeentrega;

                                        Asigna_Pedido asignacionpedidoentrega = (
                                            from SQLASignacionPedido in context.Asigna_Pedido
                                            where SQLASignacionPedido.id_pedido.Equals(ID)
                                            select SQLASignacionPedido
                                            ).FirstOrDefault();
                                        Asignaciones asignacionderuta = (
                                        from SQLAsignacionRuta in context.Asignaciones
                                        where SQLAsignacionRuta.id_ruta.Equals(asignacionpedidoentrega.id_ruta) && SQLAsignacionRuta.id_operador.Equals(asignacionpedidoentrega.id_operador) && SQLAsignacionRuta.asignacion_activa.Equals(true)
                                        select SQLAsignacionRuta
                                        ).FirstOrDefault();
                                        if (asignacionderuta.Equals(null))
                                        {
                                            asignacionderuta = (
                                            from SQLAsignacionRuta in context.Asignaciones
                                            where SQLAsignacionRuta.id_ruta.Equals(asignacionpedidoentrega.id_ruta) && SQLAsignacionRuta.id_operador.Equals(asignacionpedidoentrega.id_operador) && SQLAsignacionRuta.asignacion_activa.Equals(false)
                                            orderby SQLAsignacionRuta.id_asignacion descending
                                            select SQLAsignacionRuta
                                            ).FirstOrDefault();
                                        }
                                        pedidofase.tmffechaentrega = fechaentregado;
                                        pedidofase.tmftiempoentrega = tiempodeentrega.TotalSeconds;
                                        pedidofase.tmfusuarioentrega = asignacionpedidoentrega.id_operador;
                                        pedidofase.tmfunidadentrega = asignacionderuta.id_truck;

                                        context.SubmitChanges();
                                    }
                                    catch (Exception exs)
                                    {
                                        quepasa += "[TIEMPOS FASES 2]" + exs.Message.ToString();
                                    }
                                    #endregion

                                    #region PROCEDIMIENTO AJUSTA VENTAS EN CASO DE QUE NO SE GUARDEN BIEN
                                    try
                                    {
                                        int x = context.AjustaVentasRepartidor(ID, 10, null, null);
                                        Console.WriteLine("SI");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.Write("NO");
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region MOVIMIENTOS PEDIDOS
                                    movimientos_pedidos mov = new movimientos_pedidos();
                                    mov.id_usuario = 10;
                                    mov.id_pedido = ID;
                                    mov.hora = DateTime.Now.TimeOfDay;
                                    mov.fecha = DateTime.Now.Date;
                                    mov.funcion = "INCOMPLETO APP OPERADOR";
                                    mov.sistema = "NO TIENE DETALLES ";
                                    mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                    context.movimientos_pedidos.InsertOnSubmit(mov);
                                    context.SubmitChanges();
                                    #endregion

                                    actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                                }
                                #region CAMBIA SERVICIO
                                var deta = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) && pd.cantidad_entregada > 0 select pd;

                                int idServicio = var_pedido.Id_Servicio;
                                if (deta.All(x => x.id_servicio.Equals(1))) // gas
                                {
                                    idServicio = 1;
                                }
                                else if (deta.All(x => x.id_servicio.Equals(3)))
                                {
                                    idServicio = 3;
                                }
                                else
                                {
                                    idServicio = 6;
                                }

                                var_pedido.Id_Servicio = idServicio;
                                context.SubmitChanges();
                                #endregion

                                #region VENTAS
                                var ventas = (from ve in context.ventas_repartidor where ve.id_pedido == ID select ve).FirstOrDefault();
                                if (ventas != null)
                                {
                                    ventas.cil_t = cilindroT;
                                    ventas.cil_c = cilindroC;
                                    ventas.awa_g = garrafon;
                                    ventas.awa_alk_g = garrafon_alkalina;
                                    ventas.six_500 = six_500;
                                    ventas.six_l = six_l;
                                    ventas.six_500_alk = six_500_alk;
                                    ventas.lit_e = litrosE;
                                    ventas.es_venta = true;
                                    if (ventas.fecha.Value.Date != DateTime.Now.Date)
                                    {
                                        ventas.fecha = DateTime.Now;
                                    }
                                    context.SubmitChanges();
                                }
                                else
                                {
                                    ventas_repartidor vrep = new ventas_repartidor();
                                    vrep.cil_t_a = cilindroT;
                                    vrep.cil_c_a = cilindroC;
                                    vrep.awa_g_a = garrafon;
                                    vrep.awa_alk_g_a = garrafon_alkalina;
                                    ventas.six_500_a = six_500;
                                    ventas.six_l_a = six_l;
                                    ventas.six_500_alk_a = six_500_alk;
                                    vrep.lit_e_a = litrosE;
                                    vrep.cil_t = cilindroT;
                                    vrep.cil_c = cilindroC;
                                    vrep.awa_g = garrafon;
                                    vrep.awa_alk_g = garrafon_alkalina;
                                    ventas.six_500 = six_500;
                                    ventas.six_l = six_l;
                                    ventas.six_500_alk = six_500_alk;
                                    vrep.lit_e = litrosE;
                                    vrep.es_venta = true;
                                    vrep.fecha = DateTime.Now;
                                    vrep.id_pedido = ID;
                                    vrep.id_operador = IDOPERA;
                                    context.ventas_repartidor.InsertOnSubmit(vrep);
                                    context.SubmitChanges();
                                }
                                #endregion

                                #region Si entrega y es app. Confirma
                                int origen = 0;
                                origen = var_pedido.origen;
                                int user_movil = 10;
                                configuraciontbl config = new configuraciontbl();
                                config = context.configuraciontbl.Where(x => x.id_configuracion == 1).FirstOrDefault();
                                if (config.configuracion_temporada == 1)
                                {
                                    origen = 2;
                                    user_movil = 10;
                                }
                                if (sientregado && origen.Equals(2))
                                {
                                    var IPDs = (
                                        from SQLDetalles in context.Pedido_Detalle
                                        where SQLDetalles.Id_Pedido.Equals(ID)
                                        select SQLDetalles
                                    ).ToList();

                                    IPDs.ForEach(
                                        x =>
                                        {
                                            x.cantidad_confirmado = Convert.ToDouble(x.cantidad_entregada);
                                            x.importe_confirmado = x.importe_entregada;
                                            x.Kg_Lts_a_surtir_confirmado = x.Kg_Lts_a_surtir_entregada;
                                            x.litros_a_surtir_confirmados = x.litros_a_surtir_entregados;
                                            x.porc_final_confirmado = x.porc_final_entregada;
                                            x.porc_inicio_confirmado = x.porc_inicio_entregada;
                                            x.id_producto_confirmado = x.id_producto_entregado;
                                            x.unidad_de_liq_confirmado = x.unidad_de_liq;
                                        }
                                    );

                                    #region Ciclo Pedido
                                    ciclo_pedido c = context.ciclo_pedido.Where(x => x.id_pedido == ID).SingleOrDefault();
                                    if (c != null)
                                    {
                                        c.hora_confirmado = DateTime.Now.TimeOfDay;
                                        c.alta_confirmado = DateTime.Now.Date;
                                        var entrega = c.alta_entregado.ToString().Substring(0, 10) + " " + c.hora_entregado.ToString();
                                        var confirmado = DateTime.Now.Date.ToString().Substring(0, 10) + " " + c.hora_confirmado.ToString();
                                        DateTime date2 = DateTime.Parse(confirmado);
                                        DateTime date1 = DateTime.Parse(entrega);
                                        if (date2 > date1)
                                        {
                                            try
                                            {
                                                c.dif_entre_confirma = c.hora_confirmado - c.hora_entregado;
                                                context.SubmitChanges();
                                            }
                                            catch (Exception)
                                            {
                                                c.dif_entre_confirma = c.hora_entregado - c.hora_confirmado;
                                                context.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            c.dif_entre_confirma = date2 - date1;
                                            context.SubmitChanges();
                                        }
                                        var recepcion = c.creacion.ToString().Substring(0, 10) + " " + c.hora_recepcion.ToString();
                                        DateTime date3 = DateTime.Parse(recepcion);
                                        if (date2 > date3)
                                        {
                                            try
                                            {
                                                c.total_time = c.hora_confirmado - c.hora_recepcion;
                                                context.SubmitChanges();
                                            }
                                            catch (Exception)
                                            {
                                                c.total_time = c.hora_recepcion - c.hora_confirmado;
                                                context.SubmitChanges();
                                            }
                                        }
                                        else
                                        {
                                            c.total_time = date2 - date3;
                                            context.SubmitChanges();
                                        }
                                    }
                                    #endregion

                                    if (var_pedido != null)
                                    {
                                        var_pedido.Pedido_confirmado_cliente = true;
                                        var_pedido.verificar_litrometro = true;
                                        var_pedido.Entrega_Nota = true;
                                        var_pedido.Metodo_Pago_Confirmado = var_pedido.Id_Metodo_Pago;
                                        var_pedido.dar_cambio = true;
                                        var_pedido.conformidad_servicio = true;
                                        var_pedido.motivo_no_conforme = null;
                                        var_pedido.volveria_comprar = true;
                                        var_pedido.motivo_no_volver_comprar = null;
                                        var_pedido.quien_confirma = var_pedido.Cliente.nombre;
                                        var_pedido.Completo = true;
                                        var_pedido.Observaciones = "PEDIDO :" + ID + ", ENTREGADO, CONFIRMADO POR: " + var_pedido.Cliente.nombre + ", ESTATUS: FINALIZADO. HORA DE FINALIZACIÓN: " + DateTime.Now.TimeOfDay + ", FECHA : " + DateTime.Now.Date.ToShortDateString();
                                        var_pedido.fase = "CONFIRMACIÓN";
                                        context.SubmitChanges();

                                        #region DIARIOS
                                        pedidos_diarios pedDia = new pedidos_diarios();
                                        pedDia = context.pedidos_diarios.Where(x => x.Id_Pedido == ID).FirstOrDefault();
                                        if (pedDia != null)
                                        {
                                            pedDia.Fecha_Hora_Confirmacion = DateTime.Now;
                                            pedDia.Usuario_Confirmacion = user_movil;
                                            pedDia.Usuario_Confirmacion_Nombre = "APPMOVIL";

                                            context.SubmitChanges();
                                        }
                                        #endregion
                                    }
                                    try
                                    {
                                        #region funciones para tiempos de fase
                                        tiempos_monitorizacion_fases pedidofase = (
                                            from SQLPedidoFase in context.tiempos_monitorizacion_fases
                                            where SQLPedidoFase.idpedido.Equals(ID)
                                            select SQLPedidoFase
                                        ).FirstOrDefault();

                                        DateTime fechacapturado = pedidofase.tmffechacaptura;
                                        DateTime fechaconfirmado = DateTime.Now;
                                        TimeSpan tiempodeconfirma = fechaconfirmado - fechacapturado;

                                        pedidofase.tmffechaconfirmacion = fechaconfirmado;
                                        pedidofase.tmftiempoconfirmacion = tiempodeconfirma.TotalSeconds;
                                        pedidofase.tmfusuarioconfirmacion = 10;

                                        context.SubmitChanges();
                                        #endregion
                                    }
                                    catch (Exception hexs)
                                    {

                                    }

                                    #region FUNCIONES CONFIRMACION PEDIDO
                                    Confirmacion_Pedidos rp = new Confirmacion_Pedidos();
                                    rp.fecha = DateTime.Now.Date;
                                    rp.id_operador = (int)(from pd in context.Pedido_Detalle where pd.Id_Pedido == ID select pd.id_operador).FirstOrDefault();
                                    rp.id_pedido = ID;
                                    rp.id_ruta = (int)var_pedido.Id_ruta;
                                    rp.id_usuario = 10;
                                    rp.tiempo = DateTime.Now.TimeOfDay;
                                    rp.tipo = "CONFIRMACION";
                                    context.Confirmacion_Pedidos.InsertOnSubmit(rp);
                                    context.SubmitChanges();
                                    #endregion

                                    #region MOVIMIENTOS PEDIDOS
                                    movimientos_pedidos mov = new movimientos_pedidos();
                                    mov.id_usuario = 10;
                                    mov.id_pedido = ID;
                                    mov.hora = c.hora_entregado;
                                    mov.fecha = DateTime.Now.Date;
                                    mov.funcion = "CONFIRMA PEDIDO TABLET";
                                    mov.sistema = "PEDIDO: " + ID + ", HORA CONFIRMA: " + mov.hora + ". - POR OPERADOR: " + rp.id_operador;
                                    mov.datos = "" + ID + "," + mov.hora;
                                    context.movimientos_pedidos.InsertOnSubmit(mov);
                                    context.SubmitChanges();
                                    #endregion

                                    #region CAMBIA SERVICIO
                                    var detas = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) && (pd.cantidad_entregada > 0) select pd;
                                    int idServicioss = var_pedido.Id_Servicio;
                                    if (detas.All(x => x.id_servicio.Equals(1))) // gas
                                    {
                                        idServicioss = 1;
                                    }
                                    else if (detas.All(x => x.id_servicio.Equals(3)))
                                    {
                                        idServicioss = 3;
                                    }
                                    else
                                    {
                                        idServicioss = 6;
                                    }
                                    if (idServicioss != 6)
                                    {
                                        detas = from pd in context.Pedido_Detalle where pd.Id_Pedido.Equals(ID) select pd;
                                        foreach (var item in detas)
                                        {

                                            item.id_servicio = idServicioss;
                                            context.SubmitChanges();
                                        }
                                    }
                                    var_pedido.Id_Servicio = idServicioss;
                                    context.SubmitChanges();
                                    #endregion
                                }
                                #endregion

                                if (config.configuracion_temporada == 1)
                                {
                                    var_pedido.regresar_pedido = 0;
                                    var_pedido.se_regreso_pedido = 0;
                                    var_pedido.es_manual = 0;
                                    context.SubmitChanges();
                                }
                            }
                            else
                            {
                                #region MOVIMIENTOS PEDIDOS
                                movimientos_pedidos mov = new movimientos_pedidos();
                                mov.id_usuario = 10;
                                mov.id_pedido = ID;
                                mov.hora = DateTime.Now.TimeOfDay;
                                mov.fecha = DateTime.Now.Date;
                                mov.funcion = "INCOMPLETO APP OPERADOR";
                                mov.sistema = "NO TIENE DETALLES ";
                                mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa;
                                context.movimientos_pedidos.InsertOnSubmit(mov);
                                context.SubmitChanges();

                                #endregion
                                actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
                            }
                            #endregion

                            #region observaciones sobre la instalación
                            try
                            {
                                if (OBSERVACIONES_INSTALACION != "" && OBSERVACIONES_INSTALACION != null)
                                {
                                    string texto_instalacion = string.Format("INSTALACIÓN-> {0} - APP REPARTIDOR -", OBSERVACIONES_INSTALACION);
                                    REGISTRO_OBSERVACION(ID, texto_instalacion);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            #endregion

                            #region funciones para actualizar la capacidad del tanque
                            try
                            {
                                var caracteristicas_instalacion = (
                                from SQLCaracteristicas in context.Esp_Tec_Direccion
                                where SQLCaracteristicas.id_direccion.Equals(var_pedido.Id_Direccion)
                                select SQLCaracteristicas
                            ).FirstOrDefault();
                                if (caracteristicas_instalacion != null)
                                {
                                    int capacidades = Convert.ToInt32(PCAPACIDADES[0]);
                                    if (capacidades > 45)
                                    {
                                        caracteristicas_instalacion.num_tanques = capacidades;
                                        caracteristicas_instalacion.cap_tanque = "LTS";
                                        context.SubmitChanges();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                movimientos_pedidos mov = new movimientos_pedidos();
                                mov.id_usuario = 10;
                                mov.id_pedido = ID;
                                mov.hora = DateTime.Now.TimeOfDay;
                                mov.fecha = DateTime.Now.Date;
                                mov.funcion = "ERROR EN CAPACIDAD -" + PCAPACIDADES[0];
                                mov.sistema = ex.Message.ToUpper();
                                mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa + ex.Message;
                                context.movimientos_pedidos.InsertOnSubmit(mov);
                                context.SubmitChanges();
                            }
                            #endregion
                            #endregion
                        }
                    }
                    #region NO SURTIDO
                    if (!SURTIDO)
                    {
                        if (var_pedido != null)
                        {
                            var_pedido.Id_Motivo_No_Surtido = 11;
                            var_pedido.status_pedido = true;
                            var_pedido.enasignacion = true;
                            var_pedido.asignado = true;
                            context.SubmitChanges();
                        }
                        movimientos_pedidos mov = new movimientos_pedidos();
                        mov.id_usuario = ((usuarios)HttpContext.Current.Session["sesionUsuario"]).id_usuario;
                        mov.id_pedido = ID;
                        mov.hora = DateTime.Now.TimeOfDay;
                        mov.fecha = DateTime.Now.Date;
                        mov.funcion = "NO SURTIO PEDIDO OPERADOR";
                        mov.sistema = "PEDIDO: " + ID + ", HORA NOSURTIDO: " + mov.hora + ", FASE: RADIOS";
                        mov.datos = "" + ID + "," + mov.hora;
                        context.movimientos_pedidos.InsertOnSubmit(mov);
                        context.SubmitChanges();
                    }
                    #endregion
                }
                else
                {
                    actualizado.Add("0");
                }
            }
            catch (Exception ex)
            {
                #region MOVIMIENTOS PEDIDOS
                movimientos_pedidos mov = new movimientos_pedidos();
                mov.id_usuario = 10;
                mov.id_pedido = ID;
                mov.hora = DateTime.Now.TimeOfDay;
                mov.fecha = DateTime.Now.Date;
                mov.funcion = "INCOMPLETO APP OPERADOR";
                mov.sistema = "NO TIENE DETALLES ";
                mov.datos = "" + "0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa + ex.Message;
                context.movimientos_pedidos.InsertOnSubmit(mov);
                context.SubmitChanges();
                #endregion
                actualizado.Add("0 " + IPD + " " + CANTIDAD + " " + IMPORTE + " " + KILOS + " " + LITROS + " " + PINICIAL + " " + PFINAL + " " + IDPRO + " " + quepasa);
            }
            return actualizado.ToArray();
        }
        #endregion


        #region VALIDA VALE X VENTA LIBRE FINLAG
        [WebMethod]
        public ajaxResponse ValidaValeLibre(string folio_vale, int id_producto, int id_operador)
        {
            ajaxResponse response = new ajaxResponse();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            context.CommandTimeout = 0;
            string soapResult;
            try
            {
                string capacidad = "";
                if (id_producto == 2)
                {
                    capacidad = "TANQUE 30 KG";
                }
                else if (id_producto == 3)
                {
                    capacidad = "TANQUE 45 KG";
                }

                decimal? precio = (from i in context.producto where i.id_producto == id_producto select i.precio).FirstOrDefault();
                string no_operador = (from i in context.operador where i.id_operador == id_operador select i.no_empleado).FirstOrDefault().ToString();

                XmlDocument soapEnvelopeXml = new XmlDocument();
                string bodyXML = string.Format(@"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                  <soap:Body>
                    <ValidacionLibre xmlns='http://tempuri.org/'>
                      <foliovale>{0}</foliovale>
                      <capacidad>{1}</capacidad>
                      <precio>{2}</precio>
                      <usuario>{3}</usuario>
                    </ValidacionLibre>
                  </soap:Body>
                </soap:Envelope>", folio_vale, capacidad, precio, no_operador);

                soapEnvelopeXml.LoadXml(bodyXML);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://finlag.dyndns.org/wscombugas/WSCombugas.asmx");
                webRequest.Headers.Add("SOAPAction", "http://tempuri.org/ValidacionLibre");
                webRequest.ContentType = "text/xml; charset=utf-8";
                webRequest.Method = "POST";

                using (Stream stream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                asyncResult.AsyncWaitHandle.WaitOne();

                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(soapResult);

                string Capacidad = xmlDoc.GetElementsByTagName("Capacidad")[0].InnerXml;
                string Validacion = xmlDoc.GetElementsByTagName("Validacion")[0].InnerXml;
                string Mensaje = xmlDoc.GetElementsByTagName("Mensaje")[0].InnerXml;

                var datosFinLag = new
                {
                    Capacidad = Capacidad,
                    Validacion = Validacion,
                    Mensaje = Mensaje
                };

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(datosFinLag);

                response.Result = true;
                response.Message = Validacion;
                response.Data = json;
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }
        #endregion
    }
}
