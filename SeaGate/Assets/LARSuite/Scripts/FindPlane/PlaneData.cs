using UnityEngine;
namespace LARSuite
{
    public class PlaneData
    {
        public Mesh mesh;
        public GameObject go;
        bool isActive = false;


        public PlaneData()
        {
            go = new GameObject();
            mesh = new Mesh();
            go.SetActive(false);
            MeshCollider collider = go.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();

            //设置材质
            Material material = Resources.Load<Material>("Trigrid");
            meshRenderer.material = material;
        }
        public void setColor(Color color)
        {
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.material.color = color;
        }
        public bool getMeshState()
        {
            return isActive;
        }

        public void setPosition(Vector3 position)
        {
            go.transform.position = position;
        }

        public void setMeshState(bool isActive)
        {
            if (isActive)
            {
                mesh.RecalculateBounds();
            }
            this.isActive = isActive;
            go.SetActive(isActive);
        }

        public void destroySelf()
        {
            GameObject.Destroy(go);
            mesh = null;
        }
    }
}