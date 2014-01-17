using UnityEngine;
using System.Collections;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity
{
	public static class XmlUtils {
		public static bool TryGetInt(this XmlNode node, string key, out int value)
		{
            value = default(int);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = int.TryParse(attr.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

			return parsed;
		}

        public static bool TryGetFloat(this XmlNode node, string key, out float value)
        {
            value = default(float);
            var attr = node.Attributes[key];
            bool parsed = false;
            if (attr != null)
            {
                parsed = float.TryParse(attr.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value);
            }

			return parsed;
        }

        public static bool TryGetVector2(this XmlNode node, out Vector2 value)
        {
            value = default(Vector2);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;

            return parsed;
        }

        public static bool TryGetVector3(this XmlNode node, out Vector3 value)
        {
            value = default(Vector3);
            float tmp;
            bool parsed = true;
            if (TryGetFloat(node, "x", out tmp)) value.x = tmp;
            else parsed = false;
            if (TryGetFloat(node, "y", out tmp)) value.y = tmp;
            else parsed = false;
            if (TryGetFloat(node, "z", out tmp)) value.z = tmp;
            else parsed = false;

            return parsed;
        }
	}
}