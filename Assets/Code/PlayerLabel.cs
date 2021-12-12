using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace Code
{
    public sealed class PlayerLabel : MonoBehaviour
    {
        public void DrawLabel(Camera labelCamera)
        {
            if (labelCamera == null)
            {
                return;
            }

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.green;
            var objects = ClientScene.objects;
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects.ElementAt(i).Value;
                Debug.Log($"Iterating over {obj.name}");
                var position = labelCamera.WorldToScreenPoint(obj.transform.position);

                var collider = obj.GetComponent<Collider>();
                Debug.Log($"Condition {obj.name}: {collider != null} {labelCamera.Visible(collider)} {obj.transform != transform}");
                if (collider != null && labelCamera.Visible(collider) && obj.transform != transform)
                {
                    GUI.Label(
                        new Rect(new Vector2(position.x, Screen.height - position.y),
                            new Vector2(10.0f, name.Length * 10.5f)), obj.name, style);
                }
            }
        }
    }
}