using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace CodeGenerator
{
    public static class CodeGenerator
    {
        private static readonly Regex regex = new Regex(@"%%[A-Za-z0-9_ ]+%%");

        public static VariableContainer ExtractVariables(TextAsset textAsset)
        {
            HashSet<string> keys = new HashSet<string>();
            List<Variable> values = new List<Variable>();
            if (textAsset != null)
            {
                using (StringReader reader = new StringReader(textAsset.text))
                {
                    string s = null;
                    int line = 1;
                    while ((s = reader.ReadLine()) != null)
                    {
                        MatchCollection matches = regex.Matches(s);
                        for (int i = 0; i < matches.Count; ++i)
                        {
                            string v = matches[i].Value;
                            if (!keys.Contains(v))
                            {
                                keys.Add(v);
                                values.Add(new Variable(line, v));
                            }
                        }

                        ++line;
                    }
                }
            }

            return new VariableContainer(values);
        }

        public static string Generate(TextAsset template, VariableContainer variables)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (variables == null)
                throw new ArgumentNullException("variables");

            string str = template.text;
            for (int i = 0; i < variables.Count; ++i)
            {
                str = str.Replace(variables[i].key, variables[i].value);
            }

            return str;
        }

        public static void SaveAs(string str, string path)
        {
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("Warning", "File already exist. Overwrite it?", "Yes", "No"))
                    return;
            }

            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(str);
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "The given path is not valid.\n\n" + e.Message, "OK");
            }
            finally
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
