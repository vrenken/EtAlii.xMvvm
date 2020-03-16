namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using DotLiquid;
    using UnityEngine;

    public class T4FileGenerator
    {
        public static void Initialize(string templatesFolder)
        {
        }
        public void Generate(string outputFileName, string template, Dictionary<string, object> data)
        {
            try
            {
                var templateInstance = Template.Parse(template); // Parses and compiles the template
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}