using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SFMF
{
    public static class Util
    {
        public static ModSettings ReadModSettings(string modName)
        {
            var settingsFile = $@".\SFMF\ModSettings\{modName}.csv";

            if (!File.Exists(settingsFile))
                throw new ArgumentException($"There is no settings file for a mod named '{modName}'");

            var settings = new ModSettings();
            var settingsLines = File.ReadAllLines(settingsFile);

            foreach (var line in settingsLines)
            {
                if (line == "")
                    continue;

                var parts = line.Split(',');

                if (parts[0] == "Setting")
                    settings.Settings.Add(parts[1].ToLower(), parts[2]);
                else
                {
                    var keyboardKey = (KeyCode)Enum.Parse(typeof(KeyCode), parts[2]);
                    var controllerButton = GetKeyCode(parts[3]);

                    settings.Controls.Add(parts[1].ToLower(), new ModControl(keyboardKey, controllerButton));
                }
            }

            return settings;
        }

        /// <summary>
        /// Utility to get the /nity keycode from a button on an Xbox controller.
        /// </summary>
        /// <param name="button">The name of the button on an Xbox controller.</param>
        /// <returns>The Unity KeyCode for the corresponding button.</returns>
        public static KeyCode GetKeyCode(string controllerButton)
        {
            if (controllerButton == "A")
                return KeyCode.JoystickButton0;
            if (controllerButton == "B")
                return KeyCode.JoystickButton1;
            if (controllerButton == "X")
                return KeyCode.JoystickButton2;
            if (controllerButton == "Y")
                return KeyCode.JoystickButton3;
            if (controllerButton == "LB")
                return KeyCode.JoystickButton4;
            if (controllerButton == "RB")
                return KeyCode.JoystickButton5;
            if (controllerButton == "Select")
                return KeyCode.JoystickButton6;
            if (controllerButton == "Start")
                return KeyCode.JoystickButton7;
            if (controllerButton == "L3")
                return KeyCode.JoystickButton8;
            if (controllerButton == "R3")
                return KeyCode.JoystickButton9;

            return KeyCode.None;
        }

        public enum KeyboardKeys
        {
            None,
            F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
            Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Alpha0,
            B, C, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, T, U, V, X, Y, Z,
            Underscore, Equals, LeftBracket, RightBracket, Backslash, Semicolon, SingleQuote, Comma, Period, Slash,
            Tab, CapsLock, LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt,
        }

        public enum ControllerButtons
        {
            None,
            B,
            X,
            Y,
            LB,
            RB,
            Select,
            L3,
            R3
        }
    }

    public class ModSettings
    {
        public Dictionary<string, string> Settings { get; set; }
        public Dictionary<string, ModControl> Controls { get; set; }

        public ModSettings()
        {
            Settings = new Dictionary<string, string>();
            Controls = new Dictionary<string, ModControl>();
        }

        public string GetSetting(string settingName)
        {
            if (!Settings.ContainsKey(settingName.ToLower()))
                return null;

            return Settings[settingName];
        }

        public ModControl GetControl(string controlName)
        {
            if (!Controls.ContainsKey(controlName.ToLower()))
                return null;

            return Controls[controlName];
        }
    }

    public class ModControl
    {
        public KeyCode KeyboardKey { get; set; }
        public KeyCode ControllerButton { get; set; }

        public ModControl(KeyCode keyboardKey, KeyCode controllerButton)
        {
            KeyboardKey = keyboardKey;
            ControllerButton = controllerButton;
        }
    }
}
