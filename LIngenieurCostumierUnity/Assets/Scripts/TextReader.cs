using System.IO;
using ExcelDataReader;
using UnityEngine;
using System.Collections.Generic;

public class ExcelReaderSimple : MonoBehaviour
{   
    public Transform squelette;
    private Dictionary<string, Transform> squeletteData = new Dictionary<string, Transform>();

    public string excelFileName = "testExcel.xlsx"; // Fichier dans StreamingAssets
    
    private float ratio = 32; //transformation de cm a unite de unity

    void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, excelFileName);
        // Nécessaire pour les fichiers Excel
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        if (!File.Exists(filePath))
        {
            Debug.LogError("Fichier introuvable: " + filePath);
            return;
        }

        BuildBoneDatabase(squelette);


        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // Mode "Reader" (sans DataSet)
                do
                {
                    while (reader.Read()) // Lit ligne par ligne
                    {   
                        print(reader.GetValue(0));

                        // if (reader.GetValue(0)?.ToString() == "hanches"){
                        //     string valeurText = reader.GetValue(1)?.ToString();
                        //     float valeurFloat = float.Parse(valeurText);
                        //     MoveBoneLocal(GetBone("LeftUpLeg"),"x", (-((valeurFloat/2)-35))/ratio );
                        //     MoveBoneLocal(GetBone("RightUpLeg"),"x",(((valeurFloat/2)-35)) /ratio  );

                        //     MoveBoneLocal(GetBone("LeftLeg"),"x", (((valeurFloat/2)-35))/ratio );
                        //     MoveBoneLocal(GetBone("RightLeg"),"x",-(((valeurFloat/2)-35)) /ratio  );
                        // }

                        if (reader.GetValue(0)?.ToString() == "cuisses"){
                            string valeurText = reader.GetValue(1)?.ToString();
                            float valeurFloat = float.Parse(valeurText);
                            MoveBoneLocal(GetBone("LeftLeg"),"y", ((valeurFloat)-45)/ratio);
                            MoveBoneLocal(GetBone("RightLeg"),"y",((valeurFloat)-45)/ratio);
                        }
                        
                        if (reader.GetValue(0)?.ToString() == "jambes"){
                            string valeurText = reader.GetValue(1)?.ToString();
                            float valeurFloat = float.Parse(valeurText);
                            MoveBoneLocal(GetBone("LeftFoot"),"y", ((valeurFloat)-45)/(ratio*2));
                            MoveBoneLocal(GetBone("RightFoot"),"y",((valeurFloat)-45)/(ratio*2));
                            MoveBoneLocal(GetBone("LeftFoot"),"z", -((valeurFloat)-45)/(ratio*2));
                            MoveBoneLocal(GetBone("RightFoot"),"z",-((valeurFloat)-45)/(ratio*2));
                        }

                        if (reader.GetValue(0)?.ToString() == "avant-bras"){
                            string valeurText = reader.GetValue(1)?.ToString();
                            float valeurFloat = float.Parse(valeurText);
                            MoveBoneLocal(GetBone("LeftForeArm"),"y", ((valeurFloat)-40)/ratio);
                            MoveBoneLocal(GetBone("RightForeArm"),"y",((valeurFloat)-40)/ratio);
                            
                        }

                        if (reader.GetValue(0)?.ToString() == "bras"){
                            string valeurText = reader.GetValue(1)?.ToString();
                            float valeurFloat = float.Parse(valeurText);
                            MoveBoneLocal(GetBone("LeftHand"),"y", ((valeurFloat)-40)/ratio);
                            MoveBoneLocal(GetBone("RightHand"),"y",((valeurFloat)-40)/ratio);
                        }

                    }
                } while (reader.NextResult()); // Passe à la feuille suivante
            }
        }
    }

    public void BuildBoneDatabase(Transform rootBone)
    {
        if (rootBone == null)
        {
            Debug.LogError("RootBone est null !");
            return;
        }

        squeletteData.Clear();
        AddBonesRecursively(rootBone);
    }

    private void AddBonesRecursively(Transform bone)
    {
        if (bone == null) return;

        // Ajoute l'os actuel au dictionnaire (avec son nom comme clé)
        if (!squeletteData.ContainsKey(bone.name))
        {
            squeletteData.Add(bone.name, bone);
        }
        else
        {
            Debug.LogWarning($"Os en doublon détecté : {bone.name}");
        }

        // Parcourt récursivement tous les enfants
        foreach (Transform child in bone)
        {
            AddBonesRecursively(child);
        }
    }

    public Transform GetBone(string boneName)
    {
        squeletteData.TryGetValue(boneName, out Transform bone);
        return bone;
    }

    public static void MoveBoneLocal(Transform bone, string axis, float value)
    {
        if (bone == null)
        {
            Debug.LogError("L'os est null !");
            return;
        }

        Vector3 localMovement = Vector3.zero;

        switch (axis.ToLower())
        {
            case "x":
                localMovement = Vector3.right * value; // Axe X local
                break;
            case "y":
                localMovement = Vector3.up * value;    // Axe Y local
                break;
            case "z":
                localMovement = Vector3.forward * value; // Axe Z local
                break;
            default:
                Debug.LogError("Axe invalide. Utilisez 'x', 'y' ou 'z'.");
                return;
        }

        // Déplacement dans l'espace LOCAL (même si l'os est tourné)
        bone.Translate(localMovement, Space.Self);
    }
}