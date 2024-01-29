using PluginConfig.API.Fields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static UnityEngine.UIElements.UIRAtlasAllocator;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using System.Reflection;

namespace UltrakillStyleEditor
{
    public static class Utils
    {
        private enum TagType
        {
            Color,
            Bold,
            Italic,
            Size
        }

        private static Dictionary<string, Color> validColorWords = new Dictionary<string, Color>()
        {
            { "aqua", Color.cyan },
            { "black", Color.black },
            { "blue", Color.blue },
            { "brown", new Color(0xa5 / 255f, 0x2a / 255f, 0x2a / 255f) },
            { "cyan", Color.cyan },
            { "darkblue", new Color(0x00 / 255f, 0x00 / 255f, 0xa0 / 255f) },
            { "fuchsia", Color.magenta },
            { "green", new Color(0x00 / 255f, 0x80 / 255f, 0x00 / 255f) },
            { "grey", Color.grey },
            { "lightblue", new Color(0xad / 255f, 0xd8 / 255f, 0xe6 / 255f) },
            { "lime", Color.green },
            { "magenta", Color.magenta },
            { "maroon", new Color(0x80 / 255f, 0x00, 0x00) },
            { "navy", new Color(0x00, 0x00, 0x80 / 255f) },
            { "olive", new Color(0x80 / 255f, 0x80 / 255f, 0x00) },
            { "orange", new Color(0xff / 255f, 0xa5 / 255f, 0x00) },
            { "purple", new Color(0x80 / 255f, 0x00, 0x80 / 255f) },
            { "red", Color.red },
            { "silver", new Color(0xc0 / 255f, 0xc0 / 255f, 0xc0 / 255f) },
            { "teal", new Color(0x00, 0x80 / 255f, 0x80 / 255f) },
            { "white", Color.white },
            { "yellow", new Color(0xff / 255f, 0xff / 255f, 0x00) }
        };

        public static FormattedString FormattedStringFromFormattedText(string text)
        {
            FormattedStringBuilder builder = new FormattedStringBuilder();
            Stack<TagType> tagStack = new Stack<TagType>();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '<')
                {
                    int closingEnd = text.IndexOf('>', i + 1);
                    if (closingEnd == -1)
                    {
                        builder += c;
                        continue;
                    }
                    else
                    {
                        string tag = text.Substring(i + 1, closingEnd - i - 1).ToLower();
                        if (tag == "b")
                        {
                            tagStack.Push(TagType.Bold);
                            builder.currentFormat.bold = true;
                            i = closingEnd;
                            continue;
                        }
                        else if (tag == "/b")
                        {
                            if (tagStack.Count != 0 && tagStack.Peek() == TagType.Bold)
                            {
                                tagStack.Pop();
                                builder.currentFormat.bold = false;
                                i = closingEnd;
                                continue;
                            }
                        }
                        else if (tag == "i")
                        {
                            tagStack.Push(TagType.Italic);
                            builder.currentFormat.italic = true;
                            i = closingEnd;
                            continue;
                        }
                        else if (tag == "/i")
                        {
                            if (tagStack.Count != 0 && tagStack.Peek() == TagType.Italic)
                            {
                                tagStack.Pop();
                                builder.currentFormat.italic = false;
                                i = closingEnd;
                                continue;
                            }
                        }
                        else if (tag == "size" || tag.StartsWith("size="))
                        {
                            tagStack.Push(TagType.Size);
                            i = closingEnd;
                            continue;
                        }
                        else if (tag == "/size")
                        {
                            if (tagStack.Count != 0 && tagStack.Peek() == TagType.Size)
                            {
                                tagStack.Pop();
                                i = closingEnd;
                                continue;
                            }
                        }
                        else if (tag == "/color")
                        {
                            if (tagStack.Count != 0 && tagStack.Peek() == TagType.Color)
                            {
                                tagStack.Pop();
                                i = closingEnd;
                                continue;
                            }
                        }
                        else if (tag == "color")
                        {
                            builder.currentFormat.color = Color.white;
                            tagStack.Push(TagType.Color);
                            i = closingEnd;
                            continue;
                        }
                        else if (tag.StartsWith("color="))
                        {
                            string colorTxt = tag.Substring(6);
                            Color newColor = new Color();

                            if (!validColorWords.TryGetValue(colorTxt, out newColor))
                            {
                                if (!ColorUtility.TryParseHtmlString(colorTxt, out newColor))
                                    newColor = Color.white;
                            }

                            builder.currentFormat.color = newColor;
                            tagStack.Push(TagType.Color);
                            i = closingEnd;
                            continue;
                        }
                    }
                }

                builder += c;
            }

            return builder.Build();
        }
    }

    public static class ReflectionUtils
    {
        public static MethodInfo InstanceMethod(Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

		public static MethodInfo InstanceMethod<T>(string name)
        {
            return InstanceMethod(typeof(T), name);
        }

		public static MethodInfo StaticMethod(Type type, string name)
		{
			return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static PropertyInfo InstanceProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static PropertyInfo InstanceProperty<T>(string name)
		{
			return InstanceProperty(typeof(T), name);
		}

		public static PropertyInfo StaticProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static FieldInfo InstanceField(Type type, string name)
		{
			return type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static FieldInfo InstanceField<T>(string name)
		{
			return InstanceField(typeof(T), name);
		}

		public static FieldInfo StaticField(Type type, string name)
		{
			return type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}
}
