using System;

namespace ws_combugasclientes.core
{
    internal static class ProductoIconUrlBuilder
    {
        private const string DefaultIconPath = "/Images/productos/default.webp";

        internal static string Build(
            string iconValue,
            string configuredBaseUrl,
            Uri requestUrl,
            string applicationPath,
            string configuredDefaultIcon)
        {
            string defaultIcon = string.IsNullOrWhiteSpace(configuredDefaultIcon)
                ? DefaultIconPath
                : configuredDefaultIcon.Trim();
            string icon = string.IsNullOrWhiteSpace(iconValue) ? defaultIcon : iconValue.Trim();

            Uri absoluteIcon;
            if (Uri.TryCreate(icon, UriKind.Absolute, out absoluteIcon))
            {
                if (!IsHttp(absoluteIcon))
                {
                    icon = defaultIcon;
                }
                else if (!absoluteIcon.IsLoopback &&
                         !absoluteIcon.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    return absoluteIcon.AbsoluteUri;
                }
                else
                {
                    icon = absoluteIcon.PathAndQuery;
                }
            }

            icon = icon.Replace('\\', '/');
            if (LooksLikePhysicalPath(icon))
            {
                // Nunca incorporar una ruta física, aunque la configuración por defecto sea incorrecta.
                icon = DefaultIconPath;
            }

            Uri baseUrl = GetBaseUrl(configuredBaseUrl, requestUrl, applicationPath);
            if (baseUrl == null)
            {
                return "/" + icon.TrimStart('/');
            }

            return new Uri(baseUrl, icon.TrimStart('/')).AbsoluteUri;
        }

        private static Uri GetBaseUrl(string configuredBaseUrl, Uri requestUrl, string applicationPath)
        {
            Uri configuredUri;
            if (!string.IsNullOrWhiteSpace(configuredBaseUrl) &&
                Uri.TryCreate(configuredBaseUrl.Trim(), UriKind.Absolute, out configuredUri) &&
                IsHttp(configuredUri))
            {
                return EnsureTrailingSlash(configuredUri);
            }

            if (requestUrl == null || !IsHttp(requestUrl))
            {
                return null;
            }

            string appPath = string.IsNullOrWhiteSpace(applicationPath) || applicationPath == "/"
                ? string.Empty
                : "/" + applicationPath.Trim('/');
            return new Uri(requestUrl.GetLeftPart(UriPartial.Authority) + appPath + "/");
        }

        private static Uri EnsureTrailingSlash(Uri uri)
        {
            string value = uri.AbsoluteUri;
            return value.EndsWith("/", StringComparison.Ordinal) ? uri : new Uri(value + "/");
        }

        private static bool IsHttp(Uri uri)
        {
            return uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                   uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
        }

        private static bool LooksLikePhysicalPath(string value)
        {
            return value.StartsWith("//", StringComparison.Ordinal) ||
                   (value.Length >= 3 && char.IsLetter(value[0]) && value[1] == ':' && value[2] == '/');
        }
    }
}
