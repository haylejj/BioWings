using BioWings.Domain.Enums;
using System;

namespace BioWings.Domain.Attributes
{
    /// <summary>
    /// Controller action'larına yetkilendirme tanımlaması eklemek için kullanılan attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Hangi menüye ait olduğu
        /// </summary>
        public string MenuName { get; }

        /// <summary>
        /// Yetki türü (Read, Write, Update, Delete)
        /// </summary>
        public ActionType ActionType { get; }

        /// <summary>
        /// Açıklayıcı tanım
        /// </summary>
        public string Definition { get; }

        /// <summary>
        /// Hangi Area'ya ait olduğu
        /// </summary>
        public string AreaName { get; }

        /// <summary>
        /// AuthorizeDefinition attribute'unu başlatır
        /// </summary>
        /// <param name="menuName">Hangi menüye ait olduğu</param>
        /// <param name="actionType">Yetki türü</param>
        /// <param name="definition">Açıklayıcı tanım</param>
        /// <param name="areaName">Hangi Area'ya ait olduğu (AreaNames'ten alınmalı)</param>
        public AuthorizeDefinitionAttribute(string menuName, ActionType actionType, string definition, string areaName)
        {
            MenuName = menuName ?? throw new ArgumentNullException(nameof(menuName));
            ActionType = actionType;
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
        }
    }
} 