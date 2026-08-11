using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class direcciones : WebService
    {
        [WebMethod(EnableSession = true)]
        public ajaxResponse getDirecciones(int _intClaveCliente)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var consultasp = context.getDirecciones_App(_intClaveCliente).ToList();
                var DirSQL = (
                    from i in (
                        consultasp
                    )
                    group i by new
                    {
                        i._id_direccion,
                        i._descr_direccion,
                        i._descr_tipo_calle,
                        i._id_calle,
                        i._descr_calle,
                        i._no_interior,
                        i._no_exterior,
                        i._id_colonia,
                        i._descr_colonia,
                        i._id_ciudad,
                        i._descr_ciudad,
                        i._id_estado,
                        i._descr_estado,
                        i._id_zona,
                        i._descr_zona,
                        i._id_cp,
                        i._referencias,
                        i._latitud,
                        i._longitud,
                        i._observaciones,
                        i._entre_1,
                        i._entre_2,
                        i._entre_3,
                        i._id_segmento,
                        i._decr_cerrada,
                        i._req_clave,
                        i._clave,
                        i._id_ruta,
                        i._tienePedido,
                        _descr_cp = "",
                        _status = true
                    } into g
                    select new
                    {
                        g.Key._id_direccion,
                        g.Key._descr_direccion,
                        g.Key._descr_tipo_calle,
                        g.Key._id_calle,
                        g.Key._descr_calle,
                        g.Key._no_interior,
                        g.Key._no_exterior,
                        g.Key._id_colonia,
                        g.Key._descr_colonia,
                        g.Key._id_ciudad,
                        g.Key._descr_ciudad,
                        g.Key._id_estado,
                        g.Key._descr_estado,
                        g.Key._id_zona,
                        g.Key._descr_zona,
                        g.Key._id_cp,
                        g.Key._referencias,
                        g.Key._latitud,
                        g.Key._longitud,
                        g.Key._observaciones,
                        g.Key._entre_1,
                        g.Key._entre_2,
                        g.Key._entre_3,
                        g.Key._id_segmento,
                        g.Key._decr_cerrada,
                        g.Key._req_clave,
                        g.Key._clave,
                        g.Key._id_ruta,
                        g.Key._tienePedido,
                        g.Key._descr_cp,
                        g.Key._status
                    }
            );

                var jsonExiste = jsonSerializer.Serialize(DirSQL);
                Response.Result = true; 
                Response.Message = "OK";
                Response.Data = jsonExiste;
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("Error al consultar direcciones: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonError;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getUnaDireccion(int _intClaveDireccion)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var consultasp = context.getUnaDireccion_App(_intClaveDireccion).ToList();
                var DirSQL = (
                    from i in (
                        consultasp
                    )
                    group i by new
                    {
                        i._id_direccion,
                        i._descr_direccion,
                        i._descr_tipo_calle,
                        i._id_calle,
                        i._descr_calle,
                        i._no_interior,
                        i._no_exterior,
                        i._id_colonia,
                        i._descr_colonia,
                        i._id_ciudad,
                        i._descr_ciudad,
                        i._id_estado,
                        i._descr_estado,
                        i._id_zona,
                        i._descr_zona,
                        i._id_cp,
                        i._referencias,
                        i._latitud,
                        i._longitud,
                        i._observaciones,
                        i._entre_1,
                        i._entre_2,
                        i._entre_3,
                        i._id_segmento,
                        i._decr_cerrada,
                        i._req_clave,
                        i._clave,
                        i._id_ruta,
                        i._id_cerrada,
                        i._descripcion_cerrada,
                        _descr_cp = "",
                        _status = true
                    } into g
                    select new
                    {
                        g.Key._id_direccion,
                        g.Key._descr_direccion,
                        g.Key._descr_tipo_calle,
                        g.Key._id_calle,
                        g.Key._descr_calle,
                        g.Key._no_interior,
                        g.Key._no_exterior,
                        g.Key._id_colonia,
                        g.Key._descr_colonia,
                        g.Key._id_ciudad,
                        g.Key._descr_ciudad,
                        g.Key._id_estado,
                        g.Key._descr_estado,
                        g.Key._id_zona,
                        g.Key._descr_zona,
                        g.Key._id_cp,
                        g.Key._referencias,
                        g.Key._latitud,
                        g.Key._longitud,
                        g.Key._observaciones,
                        g.Key._entre_1,
                        g.Key._entre_2,
                        g.Key._entre_3,
                        g.Key._id_segmento,
                        g.Key._decr_cerrada,
                        g.Key._req_clave,
                        g.Key._clave,
                        g.Key._id_ruta,
                        g.Key._id_cerrada,
                        g.Key._descripcion_cerrada,
                        g.Key._descr_cp,
                        g.Key._status
                    }
            );
                var jsonExiste = jsonSerializer.Serialize(DirSQL);
                Response.Result = true;
                Response.Message = "DIR";
                Response.Data = jsonExiste;
            }
            catch (Exception ex)
            {
                var jsonError = jsonSerializer.Serialize("Error al consultar dirección: " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonError;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getColonias()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var todasColonias = (
                    from SQLColonias in context.colonias
                    where SQLColonias.status.Equals(true)
                    /*AGREGAR CONDICION DE QUE SOLO APAREZCAN LAS COLONIAS DE TORREON
                     GOMEZ Y LERDO*/
                    && (SQLColonias.id_ciudad.Equals(2) ||
                    SQLColonias.id_ciudad.Equals(3) || SQLColonias.id_ciudad.Equals(4))
                    select new
                    {
                        _idColonia = SQLColonias.id_colonia,
                        _tipoAsentamiento = SQLColonias.tipo_asentamiento.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _descrColonia = SQLColonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _idCiudad = SQLColonias.id_ciudad,
                        _descrCiudad = SQLColonias.ciudades.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                        _idEstado = SQLColonias.id_estado,
                        _descrEstado = SQLColonias.estados.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                    }
                ).ToList();

                var listaColonias = (
                    from SQLColonias in todasColonias
                    select new
                    {
                        _idColonia = SQLColonias._idColonia,
                        _tipoAsentamiento = SQLColonias._tipoAsentamiento,
                        _descrColonia = todasColonias.Count(x => x._descrColonia == SQLColonias._descrColonia) > 1 ? (SQLColonias._tipoAsentamiento + " " + SQLColonias._descrColonia + " " + SQLColonias._descrCiudad) : (SQLColonias._descrColonia + " " + SQLColonias._descrCiudad),
                        _idCiudad = SQLColonias._idCiudad,
                        _descrCiudad = SQLColonias._descrCiudad,
                        _idEstado = SQLColonias._idEstado,
                        _descrEstado = SQLColonias._descrEstado
                    }
                ).ToList();

                var jsonColonias = jsonSerializer.Serialize(listaColonias);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = jsonColonias;
            }
            catch (Exception ex)
            {
                var jsonColonias = jsonSerializer.Serialize("ERROR CARGANDO LAS COLONIAS " + ex.Message);
                Response.Result = false;
                Response.Message = "ERR";
                Response.Data = jsonColonias;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getColoniasDescr(string _strDescripcion)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var todasColonias = (
                    from SQLColonias in context.colonias
                    where SQLColonias.descripcion.Contains(_strDescripcion) && SQLColonias.status.Equals(true)
                    select new
                    {
                        _idColonia = SQLColonias.id_colonia,
                        _descrColonia = SQLColonias.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                    }
                ).ToList();

                var jsonColonias = jsonSerializer.Serialize(todasColonias);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = jsonColonias;
                return Response;
            }
            catch (Exception ex)
            {
                var jsonColonias = jsonSerializer.Serialize("ERROR CARGANDO LAS COLONIAS " + ex.Message);
                Response.Result = false;
                Response.Message = "ERR";
                Response.Data = jsonColonias;
                return Response;
            }
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getCerradas(int _intIdColonia)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var cerradas = (
                    from SQLCerradas in context.Segmento_Colonia1
                    where SQLCerradas.id_colonia.Equals(_intIdColonia) && SQLCerradas.status.Equals(true)
                    select new
                    {
                        _idCerrada = SQLCerradas.id_estado,
                        _descripcionCerrada = SQLCerradas.descripcion
                    }
                ).ToList();
                var jsonColonias = jsonSerializer.Serialize(cerradas);
                Response.Result = true;
                Response.Message = "OKI";
                Response.Data = jsonColonias;
            }
            catch (Exception ex)
            {
                var jsonColonias = jsonSerializer.Serialize("ERROR CARGANDO LAS CERRADAS " + ex.Message);
                Response.Result = false;
                Response.Message = "ERR";
                Response.Data = jsonColonias;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getCalles(int _intIdColonia)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var todasCalles = (
                    from SQLCalles in context.calles
                    where SQLCalles.id_colonia.Equals(_intIdColonia) && SQLCalles.status.Equals(true)
                    select new
                    {
                        _idCalle = SQLCalles.id_calle,
                        _descrCalle = SQLCalles.tipo_calle.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "") + " " + SQLCalles.descripcion.Replace("\n\r", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                    }
                ).ToList();

                var jsonCalles = jsonSerializer.Serialize(todasCalles);
                Response.Result = true;
                Response.Message = "EXITO";
                Response.Data = jsonCalles;
            }
            catch (Exception ex)
            {
                var jsonCalles = jsonSerializer.Serialize("ERROR CARGANDO LAS CALLES " + ex.Message);
                Response.Result = false;
                Response.Message = "ERR";
                Response.Data = jsonCalles;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getCarburaciones()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var carburaciones = (
                    from SQLCarburaciones in context.estaciones_carburacion
                    where (SQLCarburaciones.ec_tipo.Equals(1) | SQLCarburaciones.ec_tipo.Equals(2)) && SQLCarburaciones.ec_status == true
                    select SQLCarburaciones
                );
                var jsonCarburaciones = jsonSerializer.Serialize(carburaciones);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = jsonCarburaciones;
            }
            catch (Exception ex)
            {
                var jsonCarburaciones = jsonSerializer.Serialize("ERROR AL OBTENER CARBURACIONES " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonCarburaciones;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse getCarburaciones_mapa()
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                var carburaciones = (
                    from SQLCarburaciones in context.estaciones_carburacion
                    where SQLCarburaciones.ec_status == true
                    select SQLCarburaciones
                );
                var jsonCarburaciones = jsonSerializer.Serialize(carburaciones);
                Response.Result = true;
                Response.Message = "OK";
                Response.Data = jsonCarburaciones;
            }
            catch (Exception ex)
            {
                var jsonCarburaciones = jsonSerializer.Serialize("ERROR AL OBTENER CARBURACIONES " + ex.Message);
                Response.Result = false;
                Response.Message = "ERROR";
                Response.Data = jsonCarburaciones;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse actualizaDireccion(int _intClaveDireccion, string _strDescripcionDireccion, int _intClaveColonia, int _intClaveCalle, string _strNumero, string _strCodigoP, double _dblLatitud, double _dblLongitud, int _intIdCerrada = 1)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                Descripcion_Direccion descripcion = (
                    from SQLDescripcion in context.Descripcion_Direccion
                    where SQLDescripcion.id_direccion.Equals(_intClaveDireccion)
                    select SQLDescripcion
                ).FirstOrDefault();

                if (descripcion != null)
                {
                    descripcion.descripcion = _strDescripcionDireccion;
                }
                else
                {
                    descripcion = new Descripcion_Direccion();
                    descripcion.id_direccion = _intClaveDireccion;
                    descripcion.descripcion = _strDescripcionDireccion;
                    descripcion.jerarquia = 1;
                    context.Descripcion_Direccion.InsertOnSubmit(descripcion);
                }

                #region traer ruta basado en la direccion
                int idRuta = calculaRuta(_dblLatitud, _dblLongitud);
                #endregion

                #region consulta para traer el codigo postal seleccionado
                cp codigopostal = (
                    from SQLCP in context.cp
                    where SQLCP.descripcion.Trim().Contains(_strCodigoP.Trim())
                    select SQLCP
                ).FirstOrDefault();

                if (codigopostal == null)
                {
                    codigopostal = (
                        from SQLCP in context.cp
                        where SQLCP.descripcion.Trim().Contains("SIN CODIGO")
                        select SQLCP
                    ).FirstOrDefault();
                }
                #endregion

                Direccion direccion = (
                    from SQLDireccion in context.Direccion
                    where SQLDireccion.id_direccion.Equals(_intClaveDireccion)
                    select SQLDireccion
                ).FirstOrDefault();

                colonias colonia = (
                    from SQLColonia in context.colonias
                    where SQLColonia.id_colonia.Equals(_intClaveColonia)
                    select SQLColonia
                ).FirstOrDefault();

                direccion.id_estado = (colonia != null) ? colonia.id_estado : direccion.id_estado;
                direccion.id_ciudad = (colonia != null) ? colonia.id_ciudad : direccion.id_ciudad;
                direccion.id_colonia = _intClaveColonia;
                direccion.id_segmento = _intIdCerrada;
                direccion.id_calle = _intClaveCalle;
                direccion.no_exterior = _strNumero;
                direccion.id_cp = codigopostal.id_cp;
                direccion.latitud = _dblLatitud;
                direccion.longitud = _dblLongitud;
                direccion.id_ruta = (idRuta == -1) ? 1 : idRuta;
                context.SubmitChanges();

                var jsonCarburaciones = jsonSerializer.Serialize("ÉXITO");
                Response.Result = true;
                Response.Message = "ACTOK";
                Response.Data = jsonCarburaciones;
            }
            catch (Exception ex)
            {
                var jsonCarburaciones = jsonSerializer.Serialize("ERROR AL ACTUALIZAR LA DIRECCIÓN " + ex.Message);
                Response.Result = false;
                Response.Message = "ACTERR";
                Response.Data = jsonCarburaciones;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse desactivaDireccion(int _idDireccion, int _idCliente)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                var dir_cliente = context.Cliente_Direccion.Where(x => x.id_direccion == _idDireccion
                && x.id_cliente == _idCliente).FirstOrDefault();

                dir_cliente.status = false;
                context.SubmitChanges();

                var jsonUpdateDir = jsonSerializer.Serialize("Dirección desactivada correctamente");
                Response.Result = true;
                Response.Message = "OKDIRDESACTIVADA";
                Response.Data = jsonUpdateDir;
            }
            catch (Exception ex)
            {
                var jsonUpdateDir = jsonSerializer.Serialize("Error al desactivar la direccion: " + ex.Message.ToUpper());
                Response.Result = false;
                Response.Message = "NODIRDESACTIVADA";
                Response.Data = jsonUpdateDir;
            }
            return Response;
        }

        [WebMethod(EnableSession = true)]
        public ajaxResponse guardaDireccion(int _intClaveCliente, string _strDescripcionDireccion, int _intClaveColonia, int _intClaveCalle, string _strNumero, string _strCodigoP, double _dblLatitud, double _dblLongitud, int _intIdCerrada = 1)
        {
            ajaxResponse Response = new ajaxResponse();
            var jsonSerializer = new JavaScriptSerializer();
            ContextCombugasDataContext context = new ContextCombugasDataContext();

            try
            {
                #region consulta para traer la colonia seleccionada
                colonias colonia = (
                    from SQLColonia in context.colonias
                    where SQLColonia.id_colonia.Equals(_intClaveColonia)
                    select SQLColonia
                ).FirstOrDefault();
                #endregion

                #region consulta para traer el codigo postal seleccionado
                cp codigopostal = (
                    from SQLCP in context.cp
                    where SQLCP.descripcion.Trim().Contains(_strCodigoP.Trim())
                    select SQLCP
                ).FirstOrDefault();

                if (codigopostal == null)
                {
                    codigopostal = (
                        from SQLCP in context.cp
                        where SQLCP.descripcion.Trim().Contains("SIN CODIGO")
                        select SQLCP
                    ).FirstOrDefault();
                }
                #endregion

                #region traer ruta basado en la direccion
                int idRuta = calculaRuta(_dblLatitud, _dblLongitud);
                #endregion

                if (verificaDireccionExistente(colonia.id_zona, colonia.id_estado, colonia.id_ciudad, colonia.id_colonia, _intClaveCalle, _strNumero, _intClaveCliente))
                {
                    var jsonExiste = jsonSerializer.Serialize("");
                    Response.Result = false;
                    Response.Message = "EXISTE";
                    Response.Data = jsonExiste;
                    return Response;
                }

                Direccion nuevaDireccion = new Direccion();
                nuevaDireccion.id_calle = _intClaveCalle;
                nuevaDireccion.no_interior = "";
                nuevaDireccion.no_exterior = _strNumero;
                nuevaDireccion.id_colonia = _intClaveColonia;
                nuevaDireccion.id_ciudad = colonia.id_ciudad;
                nuevaDireccion.id_estado = colonia.id_estado;
                nuevaDireccion.id_zona = colonia.id_zona;
                nuevaDireccion.id_cp = codigopostal.id_cp;
                nuevaDireccion.referencias = "";
                nuevaDireccion.alta = DateTime.Now;
                nuevaDireccion.status = true;
                nuevaDireccion.latitud = _dblLatitud;
                nuevaDireccion.longitud = _dblLongitud;
                nuevaDireccion.observaciones = "";
                nuevaDireccion.entre_1 = "";
                nuevaDireccion.entre_2 = "";
                nuevaDireccion.entre_3 = "";
                nuevaDireccion.id_segmento = _intIdCerrada;
                nuevaDireccion.req_clave = false;
                nuevaDireccion.clave = "LITROS";
                nuevaDireccion.id_ruta = (idRuta == -1) ? 1 : idRuta;

                context.Direccion.InsertOnSubmit(nuevaDireccion);
                context.SubmitChanges();

                Cliente_Direccion nuevoClienteDireccion = new Cliente_Direccion();
                nuevoClienteDireccion.id_cliente = _intClaveCliente;
                nuevoClienteDireccion.id_direccion = nuevaDireccion.id_direccion;
                nuevoClienteDireccion.alta = DateTime.Now.Date;
                nuevoClienteDireccion.status = true;

                context.Cliente_Direccion.InsertOnSubmit(nuevoClienteDireccion);
                context.SubmitChanges();

                Descripcion_Direccion nuevaDescripcion = new Descripcion_Direccion();
                nuevaDescripcion.id_direccion = nuevaDireccion.id_direccion;
                nuevaDescripcion.descripcion = _strDescripcionDireccion;
                nuevaDescripcion.jerarquia = 1;

                context.Descripcion_Direccion.InsertOnSubmit(nuevaDescripcion);
                context.SubmitChanges();

                var jsonNewDir = jsonSerializer.Serialize("");
                Response.Result = true;
                Response.Message = "GUAR";
                Response.Data = jsonNewDir;
            }
            catch (Exception ex)
            {
                var jsonNewDir = jsonSerializer.Serialize("ERROR CARGANDO LAS CALLES " + ex.Message);
                Response.Result = false;
                Response.Message = "NO";
                Response.Data = jsonNewDir;
            }
            return Response;
        }

        bool verificaDireccionExistente(int ZONA, int ESTADO, int CIUDAD, int COLONIA, int CALLE, string noExt, int Id)
        {
            try
            {
                ContextCombugasDataContext context = new ContextCombugasDataContext();
                var da = (
                    from dir in context.Direccion
                    join dc in context.Cliente_Direccion on dir.id_direccion equals dc.id_direccion
                    where (
                        dir.id_zona == ZONA &
                        dir.id_estado == ESTADO &
                        dir.id_ciudad == CIUDAD &
                        dir.id_colonia == COLONIA &
                        dir.id_calle == CALLE &
                        dir.no_exterior == noExt &
                        dc.id_cliente == Id
                    )
                    select dir
                ).ToList();

                if (da.Count > 0) { return true; } else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
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
                            var coordenadasdireccion = new System.Device.Location.GeoCoordinate((double)lat, (double)lon);
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
            catch (Exception ex)
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

        #region objetos
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
        #endregion
    }
}