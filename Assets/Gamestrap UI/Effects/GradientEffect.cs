using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Gamestrap
{
    [AddComponentMenu("UI/Gamestrap UI/Gradient")]
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
    public class GradientEffect : BaseVertexEffect
#else
    public class GradientEffect : BaseMeshEffect  
#endif
    {
        public Color top = Color.white;
        public Color bottom = Color.white;

        //        //IMPORTANT: If you are using 5.2 and a patch 5.2.1p1 or above then delete from HERE to line 40 and also delete line 54:
#if UNITY_5_2_0 || UNITY_5_2_1
        public override void ModifyMesh(Mesh mesh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> list = new List<UIVertex>();
            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                vertexHelper.GetUIVertexStream(list);
            }

            ModifyVertices(list);  // calls the old ModifyVertices which was used on pre 5.2

            using (VertexHelper vertexHelper2 = new VertexHelper())
            {
                vertexHelper2.AddUIVertexTriangleStream(list);
                vertexHelper2.FillMesh(mesh);
            }
        }
#elif !(UNITY_4_6 || UNITY_5_0 || UNITY_5_1)
//        // ------------------------------------------------------------------>>>>>>  TO HERE!!
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;
            
            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
#endif // And HERE (just the line)

#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
        public override void ModifyVertices(List<UIVertex> vertexList)
#else
        public void ModifyVertices(List<UIVertex> vertexList)
#endif
        {
            if (!IsActive() || vertexList.Count < 4)
            {
                return;
            }
            #if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
            if (vertexList.Count == 4)
            {
                SetVertexColor(vertexList, 0, bottom);
                SetVertexColor(vertexList, 1, top);
                SetVertexColor(vertexList, 2, top);
                SetVertexColor(vertexList, 3, bottom);
                #else //This if has to be changed if you are using version 5.2.1p3 or later patches of 5.2.1 Use the bottom code for it to work.
            if (vertexList.Count == 6)
            {    
                SetVertexColor(vertexList, 0, bottom);
                SetVertexColor(vertexList, 1, top);
                SetVertexColor(vertexList, 2, top);
                SetVertexColor(vertexList, 3, top);
                SetVertexColor(vertexList, 4, bottom);
                SetVertexColor(vertexList, 5, bottom);
                #endif
            }
            else
            {
                float bottomPos = vertexList[vertexList.Count - 1].position.y;
                float topPos =vertexList[0].position.y;

                float height = topPos - bottomPos;

                for (int i = 0; i < vertexList.Count; i++)
                {
                    UIVertex v = vertexList[i];
                    v.color *= Color.Lerp(top, bottom, ((v.position.y) - bottomPos) / height);
                    vertexList[i] = v;
                }
            }
        }

        private void SetVertexColor(List<UIVertex> vertexList, int index, Color color)
        {
            UIVertex v = vertexList[index];
            v.color = color;
            vertexList[index] = v;
        }
    }
}
