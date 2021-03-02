using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeGenerator
{
    public class CodeGeneratorWindow : EditorWindow
    {
        private static CodeGeneratorWindow m_Instance = null;
        private static readonly string defaultFileExtension = ".cs";

        private TextAsset template = null;
        private VariableContainer cachedVariables = null;
        private string cachedPath = "null";
        private string newFileName = string.Empty;

        // Right above Assets/Create/C# Script 
        [MenuItem("Assets/Create/Script from Template", false, 80)]
        public static void ShowWindow()
        {
            m_Instance = GetWindow<CodeGeneratorWindow>("Code Generator");
        }

        private void OnEnable()
        {
            m_Instance = this;
            UpdateAssetPath();
            Selection.selectionChanged -= UpdateAssetPath;
            Selection.selectionChanged += UpdateAssetPath;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= UpdateAssetPath;
            m_Instance = null;
        }

        private static void UpdateAssetPath()
        {
            if (m_Instance != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!m_Instance.cachedPath.Equals(path))
                {
                    try
                    {
                        m_Instance.cachedPath = (IsFolder(path) ? path : Path.GetDirectoryName(path)) + "/";
                    }
                    catch
                    {
                        m_Instance.cachedPath = "Assets/";
                    }
                    finally
                    {
                        m_Instance.Repaint();
                    }
                }
            }
        }

        public static bool IsFolder(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        private void OnGUI()
        {
            using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                template = EditorGUILayout.ObjectField("Template", template, typeof(TextAsset), false) as TextAsset;

                if (changeCheckScope.changed)
                {
                    cachedVariables = CodeGenerator.ExtractVariables(template);
                }
            }

            if (GUILayout.Button("Open Template"))
            {
                AssetDatabase.OpenAsset(template);
            }

            GUILayout.Space(20);
            GUILayoutOption variableLayoutOption, replaceLayoutOption;
            GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
            if (CalculateLabelWidth(boldLabelStyle, 20, out float width))
            {
                variableLayoutOption = GUILayout.Width(width);
                replaceLayoutOption = GUILayout.Width(position.width - width - 10);
            }
            else
            {
                variableLayoutOption = replaceLayoutOption = GUILayout.Width(position.width * 0.5f);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = new GUIStyle(EditorStyles.toolbar) { alignment = TextAnchor.MiddleCenter };
                EditorGUILayout.LabelField("Variable", style, variableLayoutOption);
                EditorGUILayout.LabelField("Replace", style, replaceLayoutOption);
            }

            if (cachedVariables != null)
            {
                for (int i = 0; i < cachedVariables.Count; ++i)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        Variable v = cachedVariables[i];
                        if (GUIExtension.ButtonLabel(v.key, boldLabelStyle, variableLayoutOption))
                        {
                            AssetDatabase.OpenAsset(template, v.line);
                            GUIUtility.ExitGUI();
                        }
                        v.value = EditorGUILayout.TextField(v.value, replaceLayoutOption);
                        cachedVariables[i] = v;
                    }
                }
            }

            GUILayout.FlexibleSpace();

            newFileName = EditorGUILayout.TextField("File Name", newFileName);
            if (!string.IsNullOrEmpty(newFileName) && !newFileName.Contains("."))
                newFileName += defaultFileExtension;
        
            string fullPath = cachedPath + newFileName;
            EditorGUILayout.TextField("File Path", fullPath);

            bool notEnoughArguments = template == null || cachedVariables == null || 
                string.IsNullOrEmpty(newFileName) || EditorApplication.isCompiling;
            using (new EditorGUI.DisabledGroupScope(notEnoughArguments))
            {
                if (GUILayout.Button("Save"))
                {
                    string str = CodeGenerator.Generate(template, cachedVariables);
                    CodeGenerator.SaveAs(str, fullPath);
                }
            }
        }

        private bool CalculateLabelWidth(GUIStyle style, float margin, out float width)
        {
            width = 0;
            if (cachedVariables != null)
            {
                for (int i = 0; i < cachedVariables.Count; ++i)
                {
                    Vector2 size = style.CalcSize(new GUIContent(cachedVariables[i].key));
                    if (size.x > width)
                        width = size.x;
                }

                width += margin;
            }

            return width > margin;
        }
    }
}
