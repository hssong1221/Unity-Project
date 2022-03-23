using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System;

using com.ThreeCS.McCree;
using Photon.Pun;

// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
//[DefaultExecutionOrder(-102)]

namespace com.ThreeCS.McCree
{
    public class LocalNavMeshBuilder : MonoBehaviour
    {
        // The center of the build
        public Transform m_Tracked;

        // The size of the build bounds
        public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

        NavMeshData m_NavMesh;
        AsyncOperation m_Operation;
        NavMeshDataInstance m_Instance;
        List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

        IEnumerator Start()
        {
            while (true)
            {
                UpdateNavMesh(true);
                yield return m_Operation;
            }
        }

        void OnEnable()
        {
            // Construct and add navmesh
            m_NavMesh = new NavMeshData();
            m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
            m_Tracked = null;
            StartCoroutine(MatchPlayer());
            UpdateNavMesh(false);
        }

        IEnumerator MatchPlayer()
        {
            while (m_Tracked == null)
            {
                Debug.Log("플레이어 찾는 중 + NAV MESH");
                Debug.Log(m_Tracked);

                try
                {
                    Debug.Log("네비메쉬할당 진입!");
                    foreach (GameObject player in GameManager.Instance.playerList)
                    {
                        if (player.GetComponent<PhotonView>().IsMine)
                        {
                            Debug.Log("네비메쉬가 플레이어 찾음");
                            m_Tracked = player.transform;
                            break;
                        }
                    }
                }
                catch(NullReferenceException)
                {
                    Debug.Log("플레이어 리스트가 아직 생성되지 않았음");
                }
                //m_Tracked = transform;
                // 플레이어 리스트에서 내 거 찾아서 네비에이전트 붙이기
                
                yield return null;
            }
            Debug.Log("플레이어 찾고 나감 + NAV MESH");
        }

        void OnDisable()
        {
            // Unload navmesh and clear handle
            m_Instance.Remove();
        }

        void UpdateNavMesh(bool asyncUpdate = false)
        {
            NavMeshSourceTag.Collect(ref m_Sources);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            var bounds = QuantizedBounds();

            if (asyncUpdate)
                m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
            else
                NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        }

        static Vector3 Quantize(Vector3 v, Vector3 quant)
        {
            float x = quant.x * Mathf.Floor(v.x / quant.x);
            float y = quant.y * Mathf.Floor(v.y / quant.y);
            float z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
        }

        Bounds QuantizedBounds()
        {
            // Quantize the bounds to update only when theres a 10% change in size
            var center = m_Tracked ? m_Tracked.position : transform.position;
            return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
        }

        void OnDrawGizmosSelected()
        {
            if (m_NavMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(m_NavMesh.sourceBounds.center, m_NavMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            var bounds = QuantizedBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            var center = m_Tracked ? m_Tracked.position : transform.position;
            Gizmos.DrawWireCube(center, m_Size);
        }
    }
}

