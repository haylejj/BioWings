using BioWings.Application.DTOs;
using BioWings.Application.Interfaces;
using BioWings.Domain.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace BioWings.Application.Services
{
    /// <summary>
    /// Yetkilendirme tanımlarını reflection ile tarayan ve sağlayan servis
    /// </summary>
    public class AuthorizationDefinitionProvider : IAuthorizationDefinitionProvider
    {
        /// <summary>
        /// Tüm AuthorizeDefinition attribute'larını tarar ve yetkilendirme tanımlarını döndürür
        /// </summary>
        /// <returns>Yetkilendirme tanımlarının listesi</returns>
        public List<AuthorizeDefinitionViewModel> GetAuthorizeDefinitions()
        {
            var authorizeDefinitions = new List<AuthorizeDefinitionViewModel>();
            var types = new List<Type>();

            // WebAPI assembly'sini tara
            var webApiAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.Contains("BioWings.WebAPI") == true);

            if (webApiAssembly != null)
            {
                types.AddRange(webApiAssembly.GetTypes());
            }

            // WebUI assembly'sini de tara (gelecekte kullanım için)
            var webUiAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.Contains("BioWings.UI") == true);

            if (webUiAssembly != null)
            {
                types.AddRange(webUiAssembly.GetTypes());
            }

            // Controller sınıflarını filtrele
            var controllerTypes = types.Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                (type.IsSubclassOf(typeof(Controller)) || type.IsSubclassOf(typeof(ControllerBase)))
            ).ToList();

            foreach (var controllerType in controllerTypes)
            {
                var controllerName = GetControllerName(controllerType.Name);

                // Sadece Action düzeyindeki AuthorizeDefinition attribute'ları (controller bazında attribute koymuyoruz)
                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                
                foreach (var method in methods)
                {
                    var actionAttributes = method.GetCustomAttributes<AuthorizeDefinitionAttribute>();
                    foreach (var attribute in actionAttributes)
                    {
                        // HTTP method'unu al ve action adına ekle
                        var httpMethod = method.GetCustomAttributes()
                            .FirstOrDefault(a => a is HttpMethodAttribute) as HttpMethodAttribute;

                        var actionName = method.Name + (httpMethod != null ? $" [{httpMethod.HttpMethods.First()}]" : "");

                        authorizeDefinitions.Add(new AuthorizeDefinitionViewModel
                        {
                            ControllerName = controllerName,
                            ActionName = actionName,
                            MenuName = attribute.MenuName,
                            ActionType = attribute.ActionType.ToString(),
                            Definition = attribute.Definition,
                            AreaName = attribute.AreaName
                        });
                    }
                }
            }

            return authorizeDefinitions.OrderBy(x => x.AreaName)
                                     .ThenBy(x => x.ControllerName)
                                     .ThenBy(x => x.ActionName)
                                     .ToList();
        }

        /// <summary>
        /// Controller sınıf adından controller adını çıkarır
        /// </summary>
        /// <param name="controllerTypeName">Controller sınıf adı</param>
        /// <returns>Controller adı</returns>
        private static string GetControllerName(string controllerTypeName)
        {
            const string controllerSuffix = "Controller";
            
            if (controllerTypeName.EndsWith(controllerSuffix))
            {
                return controllerTypeName.Substring(0, controllerTypeName.Length - controllerSuffix.Length);
            }
            
            return controllerTypeName;
        }
    }
} 