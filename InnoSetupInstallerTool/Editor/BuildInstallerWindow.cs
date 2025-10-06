using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace APM.InnoSetupInstallerTool
{
    public class BuildInstallerWindow : EditorWindow
    {
        // EditorPrefs keys
        private const string KEY_TemplatePath = "BuildInstaller_TemplatePath";
        private const string KEY_InnoCompilerPath = "BuildInstaller_InnoCompilerPath";
        private const string KEY_useSetupIcon = "BuildInstaller_useSetupIcon";
        private const string KEY_InstallerIconPath = "BuildInstaller_InstallerIconPath";
        private const string KEY_IncludeEULA = "BuildInstaller_IncludeEULA";
        private const string KEY_EULAPath = "BuildInstaller_EULAPath";

        // Installer Tool variables
        private string templatePath;
        private string scriptOutputPath;
        private string innoCompilerPath;

        private bool useSetupIcon;
        private string installerIconPath;

        private bool includeEULA;
        private string eulaPath;

        // Custom GUI Style for the title
        private GUIStyle titleStyle;

        // Fields for validation
        private bool isCompilerPathValid = false;
        private bool isTemplatePathValid = false;

        // Constant filenames for validation
        private const string ISCC_FILENAME = "ISCC.exe";
        private const string TEMPLATE_FILENAME = "BuildScriptTemplate.iss";

        [MenuItem("Tools/InnoSetup Installer")]
        public static void ShowWindow()
        {
            GetWindow<BuildInstallerWindow>("InnoSetup Installer");
        }

        private void OnEnable()
        {
            // Min window size
            minSize = new Vector2(1280, 675);

            // Load settings from EditorPrefs or use default values if not set
            templatePath = EditorPrefs.GetString(KEY_TemplatePath, "");
            innoCompilerPath = EditorPrefs.GetString(KEY_InnoCompilerPath, "");

            useSetupIcon = EditorPrefs.GetBool(KEY_useSetupIcon, false);
            installerIconPath = EditorPrefs.GetString(KEY_InstallerIconPath, "");

            includeEULA = EditorPrefs.GetBool(KEY_IncludeEULA, false);
            eulaPath = EditorPrefs.GetString(KEY_EULAPath, "");

            if (!string.IsNullOrEmpty(templatePath))
            {
                scriptOutputPath = Path.Combine(Path.GetDirectoryName(templatePath), "BuildScript.iss");
            }
            else
            {
                scriptOutputPath = "";
            }

            isCompilerPathValid = IsValidFile(innoCompilerPath, ISCC_FILENAME);
            isTemplatePathValid = IsValidFile(templatePath, TEMPLATE_FILENAME);
        }

        private bool IsValidFile(string path, string expectedFilename)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }
            return Path.GetFileName(path).Equals(expectedFilename, System.StringComparison.OrdinalIgnoreCase);
        }


        private void OnGUI()
        {
            // Title
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.boldLabel);
                titleStyle.fontSize = 24;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            }

            GUILayout.Space(10);

            GUILayout.Label("InnoSetup Installer Tool", titleStyle);

            GUILayout.Space(10);

            GUILayout.Label("Create a Windows installer for your Unity desktop projects with Inno Setup. Follow the steps below:", EditorStyles.largeLabel);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // STEP 1
            GUILayout.Label("1. Set 'Company Name', 'Product Name', 'Version' number, and 'Default Icon' image:", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Player Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // STEP 2
            GUILayout.Label("2. Configure the Build Settings (Scenes, Compression Method, Asset Import Overrides, etc):", EditorStyles.boldLabel);
            if (GUILayout.Button("Open Build Settings"))
            {
                System.Type buildSettingsType = System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor");
                if (buildSettingsType != null)
                {
                    EditorWindow.GetWindow(buildSettingsType).Show();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Unable to open Build Settings window.", "OK");
                }
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // STEP 3
            GUILayout.Label("3. Download & configure Inno Setup Paths:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The Inno Setup Compiler (ISCC.exe) is required but cannot be included in this package. " +
                "Please download the portable version, unzip the file, and use the browse button below to locate the 'ISCC.exe' file on your system.",
                MessageType.Info
            );

            if (GUILayout.Button("Download Inno Setup Portable"))
            {
                Application.OpenURL("https://github.com/AdriaPm/innosetup-installer-tool/raw/refs/heads/main/innosetup-portable-win32-6.2.0-5.zip");
            }

            EditorGUILayout.Space(5);

            // Inno Compiler (ISCC.exe) path
            EditorGUILayout.BeginHorizontal();
            Color originalColor = GUI.contentColor;
            if (isCompilerPathValid)
            {
                GUI.contentColor = Color.green;
                GUILayout.Label("✅ 'ISCC.exe' Path:", GUILayout.Width(120));
            }
            else
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("❌ 'ISCC.exe' Path:", GUILayout.Width(120));
            }
            GUI.contentColor = originalColor;

            innoCompilerPath = EditorGUILayout.TextField(innoCompilerPath);

            if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
            {
                string initialDir = string.IsNullOrEmpty(innoCompilerPath) ? Application.dataPath : Path.GetDirectoryName(innoCompilerPath);

                string selected = EditorUtility.OpenFilePanel("Select ISCC.exe", initialDir, "exe");
                if (!string.IsNullOrEmpty(selected))
                {
                    innoCompilerPath = selected;
                    EditorPrefs.SetString(KEY_InnoCompilerPath, innoCompilerPath);
                    isCompilerPathValid = IsValidFile(innoCompilerPath, ISCC_FILENAME);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Inno Template script (BuildTemplateScript.iss) path
            EditorGUILayout.BeginHorizontal();
            if (isTemplatePathValid)
            {
                GUI.contentColor = Color.green;
                GUILayout.Label("✅ 'BuildScriptTemplate.iss' Path:", GUILayout.Width(200));
            }
            else
            {
                GUI.contentColor = Color.red;
                GUILayout.Label("❌ 'BuildScriptTemplate.iss' Path:", GUILayout.Width(200));
            }
            GUI.contentColor = originalColor;

            templatePath = EditorGUILayout.TextField(templatePath);

            if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
            {
                string initialDir = string.IsNullOrEmpty(templatePath) ? Application.dataPath : Path.GetDirectoryName(templatePath);

                string selected = EditorUtility.OpenFilePanel("Select 'BuildScriptTemplate.iss' File", initialDir, "iss");
                if (!string.IsNullOrEmpty(selected))
                {
                    templatePath = selected;
                    isTemplatePathValid = IsValidFile(templatePath, TEMPLATE_FILENAME);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Paths"))
            {
                EditorPrefs.SetString(KEY_TemplatePath, templatePath);
                if (!string.IsNullOrEmpty(innoCompilerPath))
                    EditorPrefs.SetString(KEY_InnoCompilerPath, innoCompilerPath);

                isCompilerPathValid = IsValidFile(innoCompilerPath, ISCC_FILENAME);
                isTemplatePathValid = IsValidFile(templatePath, TEMPLATE_FILENAME);

                EditorUtility.DisplayDialog("Paths Saved", "Installer paths saved successfully.", "OK");
            }


            // "Reset All Paths" button
            if (GUILayout.Button("Reset All Paths"))
            {
                if (EditorUtility.DisplayDialog("Confirm Reset", "Are you sure you want to reset ALL paths and options to their defaults? This will clear all saved paths and checkboxes.", "Reset All", "Cancel"))
                {
                    EditorPrefs.DeleteKey(KEY_TemplatePath);
                    EditorPrefs.DeleteKey(KEY_InnoCompilerPath);
                    EditorPrefs.DeleteKey(KEY_InstallerIconPath);
                    EditorPrefs.DeleteKey(KEY_EULAPath);
                    EditorPrefs.DeleteKey(KEY_useSetupIcon);
                    EditorPrefs.DeleteKey(KEY_IncludeEULA);

                    // Reload the editor window
                    OnEnable();
                    Repaint();

                    EditorUtility.DisplayDialog("Reset Complete", "All paths and options have been reset.", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // STEP 4
            GUILayout.Label("4. Custom Installer Options:", EditorStyles.boldLabel);

            // Custom Setup Icon
            useSetupIcon = EditorGUILayout.Toggle("Custom Setup Icon", useSetupIcon);
            if (useSetupIcon)
            {
                EditorGUILayout.BeginHorizontal();
                installerIconPath = EditorGUILayout.TextField(new GUIContent("Setup Icon Path (.ico)", "The path to a custom .ico file to be used as the installer's icon. This requires 'Custom Setup Icon' to be enabled."), installerIconPath);
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
                {
                    string selected = EditorUtility.OpenFilePanel("Select Setup Icon (.ico)", Application.dataPath, "ico");
                    if (!string.IsNullOrEmpty(selected))
                    {
                        if (Path.GetExtension(selected).ToLower() != ".ico")
                        {
                            EditorUtility.DisplayDialog("Invalid File", "Please select a valid .ico file.", "OK");
                        }
                        else
                        {
                            installerIconPath = selected;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // EULA Agreement
            includeEULA = EditorGUILayout.Toggle("Include EULA File", includeEULA);
            if (includeEULA)
            {
                EditorGUILayout.BeginHorizontal();
                eulaPath = EditorGUILayout.TextField(new GUIContent("EULA File Path", "The path to a plain text (.txt) file containing your End User License Agreement (EULA). This requires 'Include EULA File' to be enabled."), eulaPath);
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(60)))
                {
                    string selected = EditorUtility.OpenFilePanel("Select EULA File", Application.dataPath, "txt");
                    if (!string.IsNullOrEmpty(selected))
                        eulaPath = selected;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Save Custom Options"))
            {
                EditorPrefs.SetBool(KEY_useSetupIcon, useSetupIcon);
                EditorPrefs.SetString(KEY_InstallerIconPath, installerIconPath);
                EditorPrefs.SetBool(KEY_IncludeEULA, includeEULA);
                EditorPrefs.SetString(KEY_EULAPath, eulaPath);

                EditorUtility.DisplayDialog("Options Saved", "Custom installer options saved successfully.", "OK");
            }


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // STEP 5
            GUILayout.Label("5. Build and create the installer:", EditorStyles.boldLabel);
            if (GUILayout.Button("Build Installer"))
            {
                isCompilerPathValid = IsValidFile(innoCompilerPath, ISCC_FILENAME);
                isTemplatePathValid = IsValidFile(templatePath, TEMPLATE_FILENAME);

                // Validation check for mandatory paths (Step 3)
                if (!isTemplatePathValid || !isCompilerPathValid)
                {
                    string errorMsg = "Both 'ISCC.exe' and 'BuildScriptTemplate.iss' must be correctly set in Step 3.";
                    if (!isCompilerPathValid) errorMsg += "\n- Please verify the path to ISCC.exe.";
                    if (!isTemplatePathValid) errorMsg += "\n- Please verify the path to BuildScriptTemplate.iss.";

                    EditorUtility.DisplayDialog("Error", errorMsg, "OK");
                    return;
                }

                // Validation check for optional paths (Step 4)
                if ((useSetupIcon && string.IsNullOrEmpty(installerIconPath)) || (includeEULA && string.IsNullOrEmpty(eulaPath)))
                {
                    string errorMessage = "A path in Step 4 is missing.\n\n";
                    if (useSetupIcon && string.IsNullOrEmpty(installerIconPath))
                    {
                        errorMessage += " • The 'Custom Setup Icon' checkbox is enabled, but no icon path is set.\n";
                    }
                    if (includeEULA && string.IsNullOrEmpty(eulaPath))
                    {
                        errorMessage += " • The 'Include EULA File' checkbox is enabled, but no EULA file path is set.\n";
                    }
                    EditorUtility.DisplayDialog("Error", errorMessage, "OK");
                    return;
                }

                if (EditorUtility.DisplayDialog("Confirm Build", "Are you sure you want to build the installer with these settings?", "Build", "Cancel"))
                {
                    BuildInstallerProcess();
                }
            }

            GUILayout.Space(5);

            // Check if the path is valid before trying to use it
            string currentIssDir = "";
            string currentOutputFolder = "";
            bool currentOutputFolderExists = false;

            if (!string.IsNullOrEmpty(templatePath))
            {
                currentIssDir = Path.GetDirectoryName(templatePath);
                currentOutputFolder = Path.Combine(currentIssDir, "OutputInstaller");
                currentOutputFolderExists = Directory.Exists(currentOutputFolder);
            }

            GUI.enabled = currentOutputFolderExists;

            if (GUILayout.Button("Open Installer Output Folder"))
            {
                Process.Start("explorer.exe", currentOutputFolder);
            }

            GUI.enabled = true;

            // Footer
            GUILayout.FlexibleSpace();
            GUILayout.Label("For more information, please visit the documentation website:", EditorStyles.helpBox);

            if (GUILayout.Button("https://innosetupinstallertool.vercel.app", EditorStyles.linkLabel))
            {
                Application.OpenURL("https://innosetupinstallertool.vercel.app/");
            }
            GUILayout.Space(5);
            GUILayout.Label("Version 1.0 - By Adria Pons (@AdriaPm) & Julia Serra (@softdrawss)", EditorStyles.miniLabel);
        }

        private void BuildInstallerProcess()
        {
            // Gather Project Settings from PlayerSettings
            string productName = PlayerSettings.productName;
            string version = PlayerSettings.bundleVersion;
            string companyName = PlayerSettings.companyName;

            string installerIconToken = "";
            if (useSetupIcon && !string.IsNullOrEmpty(installerIconPath))
                installerIconToken = installerIconPath;

            string eulaToken = "";
            if (includeEULA && !string.IsNullOrEmpty(eulaPath))
                eulaToken = eulaPath;


            // Define build output folder (absolute path)
            string buildFolder = Path.GetFullPath(Path.Combine("Builds", $"{productName}_{version}"));
            if (!Directory.Exists(buildFolder))
                Directory.CreateDirectory(buildFolder);

            string gameExePath = Path.Combine(buildFolder, productName + ".exe");

            // Build the Unity project (collect enabled scenes)
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
            BuildPipeline.BuildPlayer(scenes, gameExePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
            UnityEngine.Debug.Log("Unity build complete: " + gameExePath);

            string templateDirectory = Path.GetDirectoryName(templatePath);
            this.scriptOutputPath = Path.Combine(templateDirectory, "BuildScript.iss");

            // Update the Inno Setup script by replacing tokens in the template
            string issContent = File.ReadAllText(templatePath);
            issContent = issContent.Replace("{PRODUCT_NAME}", productName)
                       .Replace("{COMPANY_NAME}", companyName)
                       .Replace("{VERSION}", version)
                       .Replace("{#BUILD_PATH}", buildFolder)
                       .Replace("{SETUP_ICON}", installerIconToken)
                       .Replace("{EULA_FILE}", eulaToken);
            File.WriteAllText(this.scriptOutputPath, issContent);
            UnityEngine.Debug.Log("Updated Inno Setup script: " + this.scriptOutputPath);

            // Run the Inno Setup Compiler (ISCC.exe)
            Process process = new Process();
            process.StartInfo.FileName = innoCompilerPath;
            process.StartInfo.Arguments = $"\"{this.scriptOutputPath}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            UnityEngine.Debug.Log("Installer built successfully.");

            // Automatically open the OutputInstaller folder in Windows Explorer once the installer has been created
            string issDir = Path.GetDirectoryName(this.scriptOutputPath);
            string outputFolder = Path.Combine(issDir, "OutputInstaller");
            if (Directory.Exists(outputFolder))
            {
                Process.Start("explorer.exe", outputFolder);
                UnityEngine.Debug.Log("Opened OutputInstaller folder: " + outputFolder);
            }
            else
            {
                UnityEngine.Debug.LogWarning("OutputInstaller folder not found at: " + outputFolder);
            }
        }
    }
}