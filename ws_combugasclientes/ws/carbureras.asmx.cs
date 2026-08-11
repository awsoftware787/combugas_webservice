using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Web.Services;
using ws_combugasclientes.core;

namespace ws_combugasclientes.ws
{
    [WebService(Namespace = "http://awserver.noip.me:8888/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class carbureras : WebService
    {

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

                #region VERIFICAR SI SE ENCUENTRA EN UNA CARBURACIÓN
                int? id_estacion = ValidarUbicacion(Convert.ToDouble(latitud), Convert.ToDouble(longitud));
                #endregion

                #region INSERCIÓN DE LOS DATOS DEL ARDUINO
                arduino obj_arduino = new arduino();
                obj_arduino.precio_gas = Convert.ToDouble(precio_gas);
                obj_arduino.total = Convert.ToDouble(total);
                obj_arduino.litros = Convert.ToDouble(litros);
                obj_arduino.servicio = Convert.ToInt32(servicio);
                obj_arduino.fecha = Fecha;
                obj_arduino.latitud = latitud;
                obj_arduino.longitud = longitud;
                obj_arduino.id_ec = id_estacion;
                obj_arduino.id_truck = Convert.ToInt32(id_truck);
                context.arduino.InsertOnSubmit(obj_arduino);
                context.SubmitChanges();
                #endregion
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

        private static int? ValidarUbicacion(double _lat, double _long)
        {
            int? id_carburacion = null;
            ContextCombugasDataContext context = new ContextCombugasDataContext();
            try
            {
                #region consultas de dirección y rutas
                var coordenadas = new { _lat, _long };

                var listCarburaciones = (
                    from SQLCarburaciones in context.estaciones_carburacion
                    where SQLCarburaciones.ec_tipo.Equals(2) || SQLCarburaciones.ec_tipo.Equals(3)
                    select new
                    {
                        _idCarburacion = SQLCarburaciones.id_ec,
                        _geoLat = SQLCarburaciones.ec_latitud,
                        _geoLong = SQLCarburaciones.ec_longitud
                    }
                );
                #endregion

                #region loop para detectar si las coordenadas de dirección corresponden a una carburación
                foreach (var carb in listCarburaciones)
                {
                    List<Loc> puntos = new List<Loc>();
                    puntos.Add(new Loc((double)carb._geoLat, (double)carb._geoLong));
                    if (puntos.Count > 0)
                    {
                        bool estaengeocerca = IsPointInPolygon(puntos, new Loc((Double)coordenadas._lat, (Double)coordenadas._long));
                        if (estaengeocerca)
                        {
                            id_carburacion = carb._idCarburacion;
                            break;
                        }
                    }
                }
                #endregion

                #region loop para medir la distancia entre los pines para determinar carburación
                if (id_carburacion.Equals(0) || id_carburacion.Equals(null))
                {
                    List<distanciasCarb> distancias = new List<distanciasCarb>();
                    foreach (var carb in listCarburaciones)
                    {
                        var coordenadasgaspar = new GeoCoordinate((double)coordenadas._lat, (double)coordenadas._long);
                        var coordenadaspin = new GeoCoordinate((double)carb._geoLat, (double)carb._geoLong);
                        double distancia = coordenadasgaspar.GetDistanceTo(coordenadaspin);
                        distancias.Add(new distanciasCarb(carb._idCarburacion, distancia, (double)carb._geoLat, (double)carb._geoLong));
                    }
                    if (distancias.Count > 0)
                    {
                        double menordistancia = distancias.Min(dis => dis.distancia);
                        /*SI LA UBICACION CORRESPONDE A QUE SE ENCUENTRA A MENOS DE 100 
                         * METROS DE UNA CARBURACION*/
                        if (menordistancia <= 200)
                        {
                            id_carburacion = distancias.Where(dis => dis.distancia.Equals(menordistancia)).FirstOrDefault().id_ec;
                        }
                        else
                        {
                            id_carburacion = null;
                        }
                    }
                }
                #endregion
                return id_carburacion;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}