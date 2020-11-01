﻿using System;
using System.Linq;
using CommandLine;
using Newtonsoft.Json.Linq;

namespace MigrationTools.Configuration
{
    public class MigrationClientConfigJsonConverter : JsonCreationConverter<IMigrationClientConfig>
    {
        protected override IMigrationClientConfig Create(Type objectType, JObject jObject)
        {
            if (FieldExists("ObjectType", jObject))
            {
                string typename = jObject.GetValue("ObjectType").ToString();
                Type type = AppDomain.CurrentDomain.GetAssemblies()
                  .Where(a => !a.IsDynamic)
                  .SelectMany(a => a.GetTypes())
                  .FirstOrDefault(t => t.Name.Equals(typename) || t.FullName.Equals(typename));

                if(type == null)
                {
                    throw new Exception($"Unknown ObjectType: \"{typename}\" found in {jObject.ToString()}");
                }

                return (IMigrationClientConfig)Activator.CreateInstance(type);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}