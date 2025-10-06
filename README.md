# InnoSetup Installer Tool for Unity

**A Unity tool package to create a Windows installer for your Unity desktop projects with Inno Setup.**

[![Unity Asset Store](https://img.shields.io/badge/Unity%20Asset%20Store-Available-green?style=for-the-badge&logo=unity)](https://assetstore.unity.com/packages/slug/336724)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](LICENSE)
[![GitHub Issues](https://img.shields.io/github/issues/AdriaPm/innosetup-installer-tool?style=for-the-badge&logo=github)](https://github.com/AdriaPm/innosetup-installer-tool/issues)

---

## 🚀 Overview

The **InnoSetup Installer Tool** is a Unity Editor Extension designed to automate the creation of Windows installers (`.exe`) for your Standalone Windows 64-bit builds.

It integrates the functionality of the free Inno Setup Compiler directly into the Unity Editor, streamlining the final step of distributing your PC game.

### Features

-   ✅ **One-Click Workflow:** Builds your Unity game, generates the Inno Setup script, and compiles the final installer executable.
-   ✅ **Auto-Configuration:** Automatically uses your **Company Name**, **Product Name**, and **Version** from Unity's Player Settings.
-   ✅ **Custom Options:** Supports adding a **Custom Setup Icon** and including a **EULA** file.
-   ✅ **Path Validation:** Provides visual feedback and checks for correct paths to external compiler files.

---

## ⚙️ Installation & Requirements

### Prerequisites

| Component | Requirement | Note |
| :--- | :--- | :--- |
| **Unity Editor** | 2020 LTS or newer | The tool is an Editor Extension. |
| **Target Platform** | Standalone Windows 64-bit | The tool is specifically designed for Windows installers. |
| **Inno Setup Compiler** | **External Download Required** | The compiler (`ISCC.exe`) cannot be bundled due to licensing. |

### How to Install

⬇️**Unity Asset Store:** Download for FREE the package directly through the [Unity Asset Store](https://assetstore.unity.com/packages/slug/336724) and import it to the Unity Editor.


### External Compiler Setup

Before using the tool, you must manually download the Inno Setup Compiler:

1.  **Download:** [Download the portable version of Inno Setup here](https://github.com/AdriaPm/innosetup-installer-tool/raw/refs/heads/main/innosetup-portable-win32-6.2.0-5.zip)
2.  **Extract:** Unzip the downloaded file to a safe, permanent location on your system (e.g., `C:\Tools\InnoSetup\`).
3.  **Configure:** Open the **InnoSetup Installer Tool** window in Unity (**Tools** > **InnoSetup Installer**) and use the **Browse** buttons in **Step 3** to link the following files:
    * `ISCC.exe` (Found inside the folder you Downloaded & Extracted.)
    * `BuildScriptTemplate.iss` (Found inside the asset package.)
    
---

## 📖 Usage & Documentation

For a detailed guide on how to use the tool, including all 5 setup steps and troubleshooting, please refer to the official documentation:

* **Official Documentation Website:** https://innosetupinstallertool.vercel.app/
* **Video Tutorial:** [![Youtube Video](https://github.com/AdriaPm/innosetup-installer-tool/blob/main/Documentation/InnoSetupInstallerTool_VideoThumbnail.png)](https://www.youtube.com/watch?v=DIjPTk9NXb4)

---

## 🤝 Contribution & Support

We welcome contributions, feedback, and issue reports from the community!

### 🐛 Reporting Issues

If you find a bug, have a feature request, or encounter unexpected behavior, please use the **Issues** tab on this repository.

* **Before submitting a new issue,** please check existing issues to see if your problem has already been reported.
* **When reporting a bug,** include the following:
    * Unity Version
    * Operating System (OS)
    * A detailed description of the bug and steps to reproduce it.
    * Any relevant error logs from the Unity Console.

### 💡 Feature Requests

If you have an idea for a new feature or improvement, feel free to open a discussion or an issue with the tag `[Feature Request]`.

### 💻 Contributing Code

We are open to pull requests (PRs)! If you want to contribute code:

1.  Fork the repository.
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your changes (`git commit -m 'Add AmazingFeature'`).
4.  Push to the branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

---

## ⭐ Credits

This project is maintained by:

-   **Adria Pons** ([@AdriaPm](https://github.com/AdriaPm))
-   **Julia Serra** ([@softdrawss](https://github.com/softdrawss))

## ⚖️ License

Distributed under the MIT License. See `LICENSE` for more information.

---
*(Last Updated: **October 2025**)*
