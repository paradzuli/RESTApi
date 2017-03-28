﻿using System;
using System.Web.Script.Serialization;


namespace RestWebApi.Helpers
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }
}