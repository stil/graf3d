using System.Collections.Generic;
using System.IO;
using System.Linq;
using graf3d.Engine.Komponenty;
using graf3d.Engine.Struktury;
using Newtonsoft.Json.Linq;

namespace graf3d.Demo.Utils
{
    /// <summary>
    ///     Klasa importująca sceny stworzone w programie Unity3D.
    /// </summary>
    public static class SceneImporter
    {
        public static Scene LoadJsonFile(string fileName)
        {
            var scene = new Scene
            {
                Meshes = new List<Mesh>()
            };

            var data = File.ReadAllText(fileName);
            var json = JObject.Parse(data);

            // Import wielościanów.
            foreach (var jToken in (JArray) json["meshes"])
            {
                var meshJson = (JObject) jToken;
                if (meshJson["positions"].Type == JTokenType.Null)
                {
                    continue;
                }
                var verticesArray = (JArray) meshJson["positions"];
                var indicesArray = (JArray) meshJson["indices"];

                var verticesCount = verticesArray.Count/3;
                var facesCount = indicesArray.Count/3;

                var mesh = new Mesh(verticesCount, facesCount)
                {
                    Position = ParseVector3(meshJson["position"]),
                    Rotation = ParseQuaternion(meshJson["rotation"]),
                    Scaling = ParseVector3(meshJson["scaling"]),
                    Name = (string) meshJson["name"]
                };

                // Import wierzchołków.
                for (var i = 0; i < verticesCount; i++)
                {
                    mesh.Vertices[i] = ParseVector3(verticesArray, i*3);
                }

                // Import trójkątnych ścian.
                for (var i = 0; i < facesCount; i++)
                {
                    var a = (int) indicesArray[i*3];
                    var b = (int) indicesArray[i*3 + 1];
                    var c = (int) indicesArray[i*3 + 2];
                    mesh.Faces[i] = new Face(a, b, c);
                }

                scene.Meshes.Add(mesh);
            }

            // Import ustawień kamery.
            var cameraTarget = ParseVector3(json["cameras"][0]["target"]);
            var cameraPosition = ParseVector3(json["cameras"][0]["position"]);

            scene.Camera = new Camera
            {
                Position = cameraPosition,
                //Rotation = ParseQuaternion(json["cameras"][0]["rotation"]),
                LookDirection = (cameraTarget - cameraPosition).Normalize(),
                FieldOfViewRadians = (float) json["cameras"][0]["fov"]
            };

            // Poniższy kod umieszcza siatkę "podłogi" jako najwyżej na liście.
            // Dzięki temu będzie rysowana pierwsza, a co za tym idzie będzie ukrywana za wielościanami na pierwszym planie.
            var plane = scene.Meshes.First(m => m.Name == "Plane");
            scene.Meshes.Remove(plane);
            scene.Meshes.Insert(0, plane);

            return scene;
        }

        private static Vector3 ParseVector3(JToken token, int offset = 0)
        {
            return new Vector3(
                (float) token[offset + 0],
                (float) token[offset + 1],
                (float) token[offset + 2]
            );
        }

        private static Quaternion ParseQuaternion(JToken token)
        {
            return Quaternion.RotationYawPitchRoll(
                (float) token[1],
                (float) token[0],
                (float) token[2]
            );
        }
    }
}