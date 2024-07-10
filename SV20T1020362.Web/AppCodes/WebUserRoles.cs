﻿using Microsoft.AspNetCore.Authentication.Cookies;
using SV20T1020362.DomainModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Security.Claims;
namespace SV20T1020362.Web
{
    public class WebUserRoles
    {
        public static List<WebUserRole> ListOfRoles
        {
            get
            {
                List<WebUserRole> listOfRoles = new List<WebUserRole>();
                Type type = typeof(WebUserRoles);
                var listFields = type.GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));
                foreach (var role in listFields)
                {
                    string? roleName = role.GetRawConstantValue() as string;
                    if (roleName != null)
                    {
                        DisplayAttribute? attribute = role.GetCustomAttribute<DisplayAttribute>();
                        if (attribute != null)
                            listOfRoles.Add(new WebUserRole(roleName, attribute.Name ?? roleName));
                        else
                            listOfRoles.Add(new WebUserRole(roleName, roleName));
                    }
                }
                return listOfRoles;
            }
        }
        //TODO: Định nghĩa các role được sử dụng trong hệ thống tại đây
        [Display(Name = "Quản trị hệ thống")]
        public const string Administrator = "admin";
        [Display(Name = "Nhân viên")]
        public const string Employee = "employee";
        [Display(Name = "Khách hàng")]
        public const string Customer = " customer ";
    }
}

