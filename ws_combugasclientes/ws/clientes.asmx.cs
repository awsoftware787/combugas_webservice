using RestSharp;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
    public class clientes : WebService
    {
        [WebMethod(EnableSession = true)]
        public ajaxResponse registroClienteValidacion(string _strNombre, string _strTelefono, string _strContrasena)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            #region consulta para determinar si ya se ha creado una cuenta con el teléfono especificado
            var cuentaExiste = (
                from SQLCuenta in context.Cliente_app
                where SQLCuenta.telefono.Equals(_strTelefono)
                select SQLCuenta
            ).FirstOrDefault();

            if (cuentaExiste != null)
            {
                var datosCliente = (
                    from SQLCliente in context.Cliente
                    where SQLCliente.id_cliente.Equals(cuentaExiste.id_cliente)
                    select new
                    {
                        _cCliente = SQLCliente.id_cliente,
                        _cNombre = SQLCliente.nombre,
                        _cIdTelefono = cuentaExiste.id_telefono,
                        _cDireccion = (
                            from SQLCDireccion in context.Cliente_Direccion
                            join SQLDireccion in context.Direccion on SQLCDireccion.id_direccion equals SQLDireccion.id_direccion
                            where SQLCDireccion.id_cliente.Equals(SQLCliente.id_cliente)
                            select new
                            {
                                _idDireccion = SQLCDireccion.id_direccion,
                                _descrDireccion = SQLDireccion.colonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                            }
                        ),
                        _cuenta = cuentaExiste.id_cliente_app
                    }
                );
                var jsonExiste = jsonSerializer.Serialize(datosCliente);
                Response.Result = true;
                Response.Message = "EX";
                Response.Data = jsonExiste;
                return Response;
            }
            #endregion

            #region consulta para determinar si el número de teléfono está registrado en catálogo de clientes
            var clientesExistente = (
                from SQLTelefono in context.Telefono
                join SQLCliente in context.Cliente_Telefono on SQLTelefono.id_telefono equals SQLCliente.no_telefono
                join SQLCuenta in context.Cliente_app on SQLTelefono.id_telefono equals SQLCuenta.id_telefono into c
                from SQLCuenta in c.DefaultIfEmpty()
                where SQLTelefono.no_telefono.Equals(_strTelefono)
                select new
                {
                    _cCliente = SQLCliente.id_cliente,
                    _cNombre = SQLCliente.Cliente.nombre,
                    _cIdTelefono = SQLTelefono.id_telefono,
                    _cDireccion = (
                        from SQLCDireccion in context.Cliente_Direccion
                        join SQLDireccion in context.Direccion on SQLCDireccion.id_direccion equals SQLDireccion.id_direccion
                        where SQLCDireccion.id_cliente.Equals(SQLCliente.id_cliente)
                        select new
                        {
                            _idDireccion = SQLCDireccion.id_direccion,
                            _descrDireccion = SQLDireccion.colonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                        }
                    ),
                    _cuenta = (SQLCuenta == null) ? null : (int?)SQLCuenta.id_cliente_app
                }
            ).ToList();
            if (clientesExistente.Count > 0)
            {
                string[] palabras = _strNombre.Trim().Split(' ');

                var pornombre = (
                    from datos in clientesExistente
                    where datos._cNombre.Contains(_strNombre) || datos._cNombre.Contains(palabras[0])
                    select datos
                ).ToList();

                if (pornombre.Count > 0)
                {
                    var jsonNExiste = jsonSerializer.Serialize(pornombre);
                    Response.Result = true;
                    Response.Message = "EX";
                    Response.Data = jsonNExiste;
                    return Response;
                }

                var jsonExiste = jsonSerializer.Serialize(clientesExistente);
                Response.Result = true;
                Response.Message = "EX";
                Response.Data = jsonExiste;
                return Response;
            }
            #endregion

            Cliente clienteNuevo = new Cliente();
            clienteNuevo.nombre = _strNombre;
            clienteNuevo.id_clasificacion = 3;  //Cliente normal
            clienteNuevo.id_mercado = 1;        //Doméstico
            clienteNuevo.id_segmento = 1;       //Hogar
            clienteNuevo.id_Canal = 2;          //menudeo
            clienteNuevo.id_servicio = 6;       //ambos servicios
            clienteNuevo.id_subCanal = 1;       //Hogar
            clienteNuevo.status = true;         //Activo
            clienteNuevo.alta = DateTime.Now.Date;
            clienteNuevo.hora_alta = DateTime.Now.TimeOfDay;
            clienteNuevo.ulti_mod = DateTime.Now.Date;
            clienteNuevo.ulti_hora = DateTime.Now.TimeOfDay;
            context.Cliente.InsertOnSubmit(clienteNuevo);
            context.SubmitChanges();

            Telefono telefonoNuevo = new Telefono();
            telefonoNuevo.no_telefono = _strTelefono;
            telefonoNuevo.no_secuencia = 1;
            telefonoNuevo.alta = DateTime.Now.Date;
            telefonoNuevo.status = true;
            telefonoNuevo.tipo_telefono = "CELULAR";
            telefonoNuevo.recibir_mensajes = 0;
            context.Telefono.InsertOnSubmit(telefonoNuevo);
            context.SubmitChanges();

            Cliente_Telefono nuevoClienteTelefono = new Cliente_Telefono();
            nuevoClienteTelefono.no_telefono = telefonoNuevo.id_telefono;
            nuevoClienteTelefono.id_cliente = clienteNuevo.id_cliente;
            nuevoClienteTelefono.alta = DateTime.Now.Date;
            nuevoClienteTelefono.status = true;
            context.Cliente_Telefono.InsertOnSubmit(nuevoClienteTelefono);
            context.SubmitChanges();

            Cliente_app nuevasCredenciales = new Cliente_app();
            nuevasCredenciales.id_cliente = clienteNuevo.id_cliente;
            nuevasCredenciales.id_telefono = telefonoNuevo.id_telefono;
            nuevasCredenciales.telefono = _strTelefono;
            nuevasCredenciales.contrasenia = _strContrasena;
            nuevasCredenciales.ultimo_acceso = DateTime.Now;
            nuevasCredenciales.alta = DateTime.Now;
            nuevasCredenciales.estatus = false;
            nuevasCredenciales.codigo_verificacion = codigoVerficacion(_strTelefono);
            context.Cliente_app.InsertOnSubmit(nuevasCredenciales);
            context.SubmitChanges();

            string telefono = new String(_strTelefono.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono
            string URLSITIO = ConfigurationManager.AppSettings["URLSITIO"];
            string PUERTOSITIO = ConfigurationManager.AppSettings["PUERTOSITIO"];
            string URLS = URLSITIO + PUERTOSITIO + "/app/requestnotinformation.aspx?cntnotinfo=" + clienteNuevo.id_cliente + "&cnttel=" + telefonoNuevo.id_telefono;
            string msj = "COMBUGAS. SE LE ESTARA ENVIANDO INFORMACION DE SUS PEDIDOS VIA SMS. SI ESTA DE ACUERDO EN RECIBIR DICHA INFORMACION DE CLIC EN EL SIGUIENTE ENLACE (" + URLS + ")";
            enviarSMSUrl(telefono, msj);
            var jsonNuevo = jsonSerializer.Serialize(nuevasCredenciales.id_cliente_app);
            Response.Result = true;
            Response.Message = "OK";
            Response.Data = jsonNuevo;
            return Response;
        }
        class objInfoBips
        {
            public objmensajes[] messages { get; set; }
            public objTracking tracking { get; set; }
        }

        class objmensajes
        {
            public string from { get; set; }
            public objdestinations[] destinations { get; set; }
            public string text { get; set; }
        }

        class objdestinations
        {
            public string to { get; set; }
        }

        class objTracking
        {
            public string track { get; set; }
            public string type { get; set; }

        }

        #region envio de sms por medio de la plataforma infobipURL
        static void enviarSMSUrl(string numero_telefonico, string mensaje)
        {
            const string API_KEY = "2d4a670df2800b28ae68d9a66762f4da-5095712a-4a4d-4c83-b574-56fe35a74532";

            objdestinations para = new objdestinations();
            para.to = "+52" + numero_telefonico;

            objdestinations[] array = new objdestinations[1];
            array[0] = para;

            objmensajes objetoMensaje = new objmensajes();
            objetoMensaje.from = "COMBUGAS";
            objetoMensaje.destinations = array;
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
            System.Diagnostics.Debug.WriteLine(jsonINFO);

            var client = new RestClient("https://lkdew.api.infobip.com/sms/1/text/advanced");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json");
            request.AddHeader("authorization", "App " + API_KEY);
            request.AddParameter("application/json", jsonINFO, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
        #endregion


        [WebMethod(EnableSession = true)]
        public ajaxResponse registroClienteDirecto(int? _intClave, string _strNombre, string _strTelefono, string _strContrasena, int? _intClaveTelefono)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                if (_intClave == null || _intClave == 0)
                {
                    Cliente clienteNuevo = new Cliente();
                    clienteNuevo.nombre = _strNombre;
                    clienteNuevo.id_clasificacion = 3;  //Cliente normal
                    clienteNuevo.id_mercado = 1;        //Doméstico
                    clienteNuevo.id_segmento = 1;       //Hogar
                    clienteNuevo.id_Canal = 2;          //menudeo
                    clienteNuevo.id_servicio = 6;       //ambos servicios
                    clienteNuevo.id_subCanal = 1;       //Hogar
                    clienteNuevo.status = true;         //Activo
                    clienteNuevo.alta = DateTime.Now.Date;
                    clienteNuevo.hora_alta = DateTime.Now.TimeOfDay;
                    clienteNuevo.ulti_mod = DateTime.Now.Date;
                    clienteNuevo.ulti_hora = DateTime.Now.TimeOfDay;
                    context.Cliente.InsertOnSubmit(clienteNuevo);
                    context.SubmitChanges();

                    Telefono telefonoNuevo = new Telefono();
                    telefonoNuevo.no_telefono = _strTelefono;
                    telefonoNuevo.no_secuencia = 1;
                    telefonoNuevo.alta = DateTime.Now.Date;
                    telefonoNuevo.status = true;
                    telefonoNuevo.tipo_telefono = "CELULAR";
                    telefonoNuevo.recibir_mensajes = 0;
                    context.Telefono.InsertOnSubmit(telefonoNuevo);
                    context.SubmitChanges();

                    Cliente_Telefono nuevoClienteTelefono = new Cliente_Telefono();
                    nuevoClienteTelefono.no_telefono = telefonoNuevo.id_telefono;
                    nuevoClienteTelefono.id_cliente = clienteNuevo.id_cliente;
                    nuevoClienteTelefono.alta = DateTime.Now.Date;
                    nuevoClienteTelefono.status = true;
                    context.Cliente_Telefono.InsertOnSubmit(nuevoClienteTelefono);
                    context.SubmitChanges();

                    Cliente_app nuevasCredenciales = new Cliente_app();
                    nuevasCredenciales.id_cliente = clienteNuevo.id_cliente;
                    nuevasCredenciales.id_telefono = telefonoNuevo.id_telefono;
                    nuevasCredenciales.telefono = _strTelefono;
                    nuevasCredenciales.contrasenia = _strContrasena;
                    nuevasCredenciales.ultimo_acceso = DateTime.Now;
                    nuevasCredenciales.alta = DateTime.Now;
                    nuevasCredenciales.estatus = false;
                    nuevasCredenciales.codigo_verificacion = codigoVerficacion(_strTelefono);
                    context.Cliente_app.InsertOnSubmit(nuevasCredenciales);
                    context.SubmitChanges();
                    string telefono = new String(_strTelefono.Where(Char.IsDigit).ToArray());
                    string URLSITIO = ConfigurationManager.AppSettings["URLSITIO"];
                    string PUERTOSITIO = ConfigurationManager.AppSettings["PUERTOSITIO"];
                    string URLS = URLSITIO + PUERTOSITIO + "/app/requestnotinformation.aspx?cntnotinfo=" + clienteNuevo.id_cliente + "&cnttel=" + telefonoNuevo.id_telefono;
                    string msj = "COMBUGAS. SE LE ESTARA ENVIANDO INFORMACION DE SUS PEDIDOS VIA SMS. SI ESTA DE ACUERDO EN RECIBIR DICHA INFORMACION DE CLIC EN EL SIGUIENTE ENLACE (" + URLS + ")";
                    enviarSMSUrl(telefono, msj);
                    var jsonNuevo = jsonSerializer.Serialize(nuevasCredenciales.id_cliente_app);
                    Response.Result = true;
                    Response.Message = "OK";
                    Response.Data = jsonNuevo;
                }
                else
                {
                    Cliente clienteNuevo = (
                        from SQLCliente in context.Cliente
                        where SQLCliente.id_cliente.Equals(_intClave)
                        select SQLCliente
                    ).FirstOrDefault();

                    Cliente_app nuevasCredenciales = new Cliente_app();
                    nuevasCredenciales.id_cliente = clienteNuevo.id_cliente;
                    nuevasCredenciales.id_telefono = (int)_intClaveTelefono;
                    nuevasCredenciales.telefono = _strTelefono;
                    nuevasCredenciales.contrasenia = _strContrasena;
                    nuevasCredenciales.ultimo_acceso = DateTime.Now;
                    nuevasCredenciales.alta = DateTime.Now;
                    nuevasCredenciales.estatus = false;
                    nuevasCredenciales.codigo_verificacion = codigoVerficacion(_strTelefono);
                    context.Cliente_app.InsertOnSubmit(nuevasCredenciales);
                    context.SubmitChanges();

                    var jsonNuevo = jsonSerializer.Serialize(nuevasCredenciales.id_cliente_app);
                    Response.Result = true;
                    Response.Message = "OK";
                    Response.Data = jsonNuevo;
                }
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("Error al procesar el registro - " + ex);
                Response.Result = false;
                Response.Message = "ERR";
                Response.Data = jsonError;
            }
            return Response;
        }

        #region generar y enviar código de verificacion
        string codigoVerficacion(string _strTelefono)
        {
            string telefono = new String(_strTelefono.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono
            Random generator = new Random();
            String codigo = generator.Next(100000, 1000000).ToString("D6"); //obtiene un numero random de 6 digitos

            try
            {
                enviarSMS(telefono, "COMBUGAS APP: Su código de verificación es: " + codigo);
            }
            catch (WebException ex)
            {
                Debug.Write(ex.Message);
            }
            return codigo;
        }
        bool codigoVerficacionReenviado(string _strTelefono, string _strCodigo)
        {
            string telefono = new String(_strTelefono.Where(Char.IsDigit).ToArray()); //obtiene solo numeros de la cadena del telefono

            try
            {
                enviarSMS(telefono, "COMBUGAS APP: Su código de verificación es: " + _strCodigo);
                return true;
            }
            catch (WebException ex)
            {
                Debug.Write(ex.Message);
                return false;
            }
        }
        #endregion

        [WebMethod(EnableSession = true)]
        public ajaxResponse eliminarCuenta(int id_cliente)
        {
            ajaxResponse response = new ajaxResponse();
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var cliente_app = context.Cliente_app.Where(x => x.id_cliente == id_cliente).ToList();
                if (cliente_app.Count > 0)
                {
                    context.Cliente_app.DeleteAllOnSubmit(cliente_app);
                    context.SubmitChanges();
                    response.Result = true;
                    response.Message = "CLIENTEAPPELIMINADO";
                }
                else
                {
                    response.Result = false;
                    response.Message = "CLIENTEAPPNOENCONTRADO";
                }
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Message = ex.Message.ToUpper();
            }
            return response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse login(string _strTelefono, string _strContrasena)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                bool existeTelefono = (
                     from SQLUsuario in context.Cliente_app
                     where SQLUsuario.telefono.Equals(_strTelefono)
                     select SQLUsuario
                ).Any();

                if (existeTelefono)
                {
                    var usuario = (
                        from SQLUsuario in context.Cliente_app
                        where SQLUsuario.telefono.Equals(_strTelefono) && SQLUsuario.contrasenia.Equals(_strContrasena)
                        select new
                        {
                            _claveApp = SQLUsuario.id_cliente_app,
                            _clave = SQLUsuario.id_cliente,
                            _nombre = SQLUsuario.Cliente.nombre,
                            _telefono = SQLUsuario.id_telefono,
                            _activo = (bool)SQLUsuario.estatus,
                            _bloqueado = SQLUsuario.bloqueado,
                            _motivo_bloqueado = SQLUsuario.motivo_bloqueado,
                            _tieneDireccion = (
                                from SQLDireccion in context.Cliente_Direccion
                                where SQLDireccion.id_cliente.Equals(SQLUsuario.id_cliente)
                                select SQLDireccion
                            ).Count(),
                            _mercado = SQLUsuario.Cliente.id_mercado,
                            _subCanal = SQLUsuario.Cliente.id_subCanal
                        }
                    ).FirstOrDefault();

                    if (usuario != null)
                    {
                        var jsonLogin = jsonSerializer.Serialize(usuario);
                        Response.Result = true;
                        if (!usuario._activo)
                        {
                            Response.Message = "NACT";
                        }
                        else
                        {
                            Response.Message = "LOG";
                            Cliente_app clienteLogin = (
                                from SQLUsr in context.Cliente_app
                                where SQLUsr.id_cliente_app.Equals(usuario._claveApp)
                                select SQLUsr
                            ).FirstOrDefault();
                            clienteLogin.ultimo_acceso = DateTime.Now;
                            context.SubmitChanges();
                        }
                        Response.Data = jsonLogin;
                    }
                    else
                    {
                        var jsonLogin = jsonSerializer.Serialize("ERROR DE USUARIO O CONTRASEÑA");
                        Response.Result = false;
                        Response.Message = "NOTLOG";
                        Response.Data = jsonLogin;
                    }
                }
                else
                {
                    var jsonLogin = jsonSerializer.Serialize("NO EXISTE TELEFONO");
                    Response.Result = false;
                    Response.Message = "NOTLOG";
                    Response.Data = jsonLogin;
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = "ERR500";
                Response.Data = jsonSerializer.Serialize("Error: " + ex.Message);
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse actualizaCorreo(int _intClaveUsuario, string _strCorrreo)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Cliente_app cliente = (
                    from SQLCliente in context.Cliente_app
                    where SQLCliente.id_cliente.Equals(_intClaveUsuario)
                    select SQLCliente
                ).FirstOrDefault();

                cliente.correo = _strCorrreo;

                context.SubmitChanges();

                var jsonLogin = jsonSerializer.Serialize("EXITO");
                Response.Result = true;
                Response.Message = "ACTCORREO";
                Response.Data = jsonLogin;
            }
            catch (Exception ex)
            {
                var jsonACT = jsonSerializer.Serialize("ERROR " + ex.Message);
                Response.Result = false;
                Response.Message = "ERR500";
                Response.Data = jsonACT;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public double obtenerPrecioGasLP()
        {
            double precio_gaslp = 0;
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                context.CommandTimeout = 0;
                precio_gaslp = (double)(from p in context.producto where p.id_producto == 9 select p.precio).FirstOrDefault();
            }
            catch (Exception ex)
            {
                precio_gaslp = 0;
            }
            return precio_gaslp;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse ValidarFechaEntrega()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();

            try
            {
                using (ContextCombugasDataContext context = new ContextCombugasDataContext())
                using (var comando = new System.Data.SqlClient.SqlCommand(
                    "dbo.sp_ValidarFechaEntrega", (System.Data.SqlClient.SqlConnection)context.Connection))
                {
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Add("@Fecha", System.Data.SqlDbType.Date).Value = DateTime.Today;
                    context.Connection.Open();
                    using (var resultado = comando.ExecuteReader())
                    {
                        if (!resultado.Read())
                        {
                            throw new InvalidOperationException("No se obtuvo el resultado de la validacion de entrega.");
                        }

                        var jsonINFO = jsonSerializer.Serialize(new
                        {
                            permite_entrega = Convert.ToBoolean(resultado["permite_entrega"]),
                            mensaje = resultado["mensaje"] == DBNull.Value ? null : Convert.ToString(resultado["mensaje"])
                        });
                        Response.Result = true;
                        Response.Message = "INFO";
                        Response.Data = jsonINFO;
                    }
                }
            }
            catch (Exception ex)
            {
                var jsonINFO = jsonSerializer.Serialize("ERROR" + ex.Message);
                Response.Result = false;
                Response.Message = "ERR500";
                Response.Data = jsonINFO;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse traerInfoCliente(int _intClaveUsuario)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var clienteapp = (
                    from SQLCiente in context.Cliente_app
                    where SQLCiente.id_cliente.Equals(_intClaveUsuario)
                    select new
                    {
                        _nombre = SQLCiente.Cliente.nombre,
                        _telefono = SQLCiente.telefono,
                        _correo = SQLCiente.correo,
                        _tieneDireccion = (
                            from SQLDireccion in context.Cliente_Direccion
                            join CD in context.Cliente_Direccion on SQLDireccion.id_direccion equals CD.id_direccion
                            where SQLDireccion.id_cliente.Equals(_intClaveUsuario) && CD.status.Equals(true)
                            select SQLDireccion
                        ).Count()
                    }
                ).FirstOrDefault();

                var jsonINFO = jsonSerializer.Serialize(clienteapp);
                Response.Result = true;
                Response.Message = "INFO";
                Response.Data = jsonINFO;
            }
            catch (Exception ex)
            {
                var jsonINFO = jsonSerializer.Serialize("ERROR" + ex.Message);
                Response.Result = false;
                Response.Message = "ERR500";
                Response.Data = jsonINFO;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse verificarCuenta(int _intClaveUsuario, string _strCodigoVerif)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Cliente_app cuenta = (
                    from SQLCuenta in context.Cliente_app
                    where SQLCuenta.id_cliente_app.Equals(_intClaveUsuario)
                    select SQLCuenta
                ).FirstOrDefault();

                if (cuenta != null)
                {
                    if (cuenta.codigo_verificacion.Equals(_strCodigoVerif))
                    {
                        cuenta.estatus = true;
                        context.SubmitChanges();

                        Response.Result = true;
                        Response.Message = "VERIF";
                    }
                    else
                    {
                        Response.Result = false;
                        Response.Message = "NOVERIF";
                    }
                }
                else
                {
                    Response.Result = false;
                    Response.Message = "NOCLAVE";
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = "VERIFERR";
                Response.Data = ex.Message;
            }
            return Response;
        }

        #region para reenviar el codigo de verificacion despues del login
        [WebMethod]
        public ajaxResponse reenviarCodigo(int _intClaveUsuario)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Cliente_app cuenta = (
                    from SQLCuenta in context.Cliente_app
                    where SQLCuenta.id_cliente_app.Equals(_intClaveUsuario)
                    select SQLCuenta
                ).FirstOrDefault();

                if (cuenta != null)
                {
                    codigoVerficacionReenviado(cuenta.telefono, cuenta.codigo_verificacion);
                    Response.Result = true;
                    Response.Message = "OK";
                }
                else
                {
                    Response.Result = false;
                    Response.Message = "NOCLAVE";
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = "VERIFERR";
                Response.Data = ex.Message;
            }
            return Response;
        }
        #endregion

        [WebMethod]
        public ajaxResponse recuperarContrasena(string _strTelefono)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Cliente_app cliente = (
                    from SQLCliente in context.Cliente_app
                    where (
                        SQLCliente.telefono.Equals(_strTelefono) &&
                        SQLCliente.Telefono1.no_telefono.Equals(_strTelefono)
                    )
                    select SQLCliente
                ).FirstOrDefault();

                if (cliente != null)
                {
                    string telefono = new String(_strTelefono.Where(Char.IsDigit).ToArray()); //obtiene solo numeos de la cadena del telefono

                    enviarSMS(telefono, "COMBUGAS APP: Su clave de acceso es: " + cliente.contrasenia);

                    Response.Result = true;
                    Response.Message = "RECUPERAOK";
                }
                else
                {
                    Response.Result = true;
                    Response.Message = "NOTEL";
                }
            }
            catch (Exception ex)
            {
                Response.Result = false;
                Response.Message = "RECUPERAERROR";
                Response.Data = ex.Message;
            }
            return Response;
        }

        [WebMethod]
        public ajaxResponse pendienteFormulario(int _intClaveCliente)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var pedidosPendientesPorConfirmar = (
                    from SQLPedidos in context.Pedido
                    where (
                        SQLPedidos.Id_Cliente.Equals(_intClaveCliente) &&
                        SQLPedidos.status_pedido == true &&
                        SQLPedidos.origen.Equals(2) &&
                        SQLPedidos.Pedido_confirmado_operador == true &&
                        (from c in context.calificaciones_app where c.calif_ref_pedido.Equals(SQLPedidos.Id_Pedido) select c.calif_id).Count().Equals(0)
                    )
                    select new
                    {
                        Id_Pedido = SQLPedidos.Id_Pedido,
                        DescripcionDireccion = SQLPedidos.Direccion.Descripcion_Direccion.FirstOrDefault().descripcion
                    }
                ).ToList();
                if (pedidosPendientesPorConfirmar.Count() > 0)
                {
                    var jsonINFO = jsonSerializer.Serialize(pedidosPendientesPorConfirmar);
                    Response.Result = true;
                    Response.Message = "CONFIRMA";
                    Response.Data = jsonINFO;
                }
                else
                {
                    var jsonINFO = jsonSerializer.Serialize("NO HAY PEDIDOS PENDIENTES POR CONFIRMAR");
                    Response.Result = true;
                    Response.Message = "NOCONFIRMA";
                    Response.Data = jsonINFO;
                }
            }
            catch (Exception ex)
            {
                var jsonINFO = jsonSerializer.Serialize("ERROR" + ex.Message);
                Response.Result = false;
                Response.Message = "ERRORCONFIRMA";
                Response.Data = jsonINFO;
            }
            return Response;
        }

        #region envio de sms por medio de la plataforma infobip
        void enviarSMS(string numero_telefonico, string mensaje)
        {
            //const string API_KEY = "edbe7af3462d0a80d00f9aa414c6834e-3588fa14-3994-4937-913d-762a204e3573";
             string API_KEY = "2d4a670df2800b28ae68d9a66762f4da-5095712a-4a4d-4c83-b574-56fe35a74532";

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
            Debug.WriteLine(response.StatusCode.ToString());
        }
        #endregion
    }
    #region objetos
    class objMasmensajes
    {
        public string usuario { get; set; }
        public string password { get; set; }
        public string celular { get; set; }
        public string mensaje { get; set; }
    }
    class objInfoBip
    {
        public string from { get; set; }
        public string to { get; set; }
        public string text { get; set; }
    }
    #endregion
}