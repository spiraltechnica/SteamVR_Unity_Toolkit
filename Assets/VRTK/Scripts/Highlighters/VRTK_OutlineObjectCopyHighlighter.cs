// Outline Object Copy|Highlighters|0030
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections.Generic;

    public class VRTK_OutlineObjectCopyHighlighter : VRTK_BaseHighlighter
    {
        public float thickness = 1f;
        public GameObject customOutlineModel;
        private Material stencilOutline;
        private GameObject highlightModel;
        private string[] copyComponents = new string[] { "UnityEngine.MeshFilter", "UnityEngine.MeshRenderer" };

        public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
        {

            if (stencilOutline == null)
            {
                stencilOutline = Instantiate((Material)Resources.Load("OutlineBasic"));
            }

            SetOptions(options);
            CreateHighlightModel();
        }

        public override void Highlight(Color? color, float duration = 0f)
        {
            if (highlightModel)
            {
                stencilOutline.SetFloat("_Thickness", thickness);
                stencilOutline.SetColor("_OutlineColor", (Color)color);

                highlightModel.SetActive(true);
            }
        }

        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            if (highlightModel)
            {
                highlightModel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Destroy(highlightModel);
            Destroy(stencilOutline);
        }

        private void SetOptions(Dictionary<string, object> options = null)
        {
            var tmpThickness = GetOption<float>(options, "thickness");
            if (tmpThickness > 0f)
            {
                thickness = tmpThickness;
            }

            var tmpCustomModel = GetOption<GameObject>(options, "customOutlineModel");
            if (tmpCustomModel != null)
            {
                customOutlineModel = tmpCustomModel;
            }
        }

        private void CreateHighlightModel()
        {
            if(customOutlineModel != null)
            {
                customOutlineModel = (customOutlineModel.GetComponent<Renderer>() ? customOutlineModel : customOutlineModel.GetComponentInChildren<Renderer>().gameObject);
            }

            GameObject copyModel = customOutlineModel;
            if (copyModel == null)
            {
                copyModel = (GetComponent<Renderer>() ? gameObject : GetComponentInChildren<Renderer>().gameObject);
            }

            if (copyModel == null)
            {
                Debug.LogError("No Renderer has been found on the model to add highlighting to");
                return;
            }

            highlightModel = new GameObject(name + "_HighlightModel");
            highlightModel.transform.position = transform.position;
            highlightModel.transform.rotation = transform.rotation;
            highlightModel.transform.localScale = transform.localScale;
            highlightModel.transform.SetParent(transform);

            foreach (var component in copyModel.GetComponents<Component>())
            {
                if(System.Array.IndexOf(copyComponents, component.GetType().ToString()) >= 0)
                {
                    Utilities.CloneComponent(component, highlightModel);
                }
            }

            var copyMesh = copyModel.GetComponent<MeshFilter>();
            highlightModel.GetComponent<MeshFilter>().mesh = copyMesh.mesh;
            highlightModel.GetComponent<Renderer>().material = stencilOutline;
            highlightModel.SetActive(false);
        }
    }
}
